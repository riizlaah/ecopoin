package com.example.ecopoin

import android.content.SharedPreferences
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.core.content.edit
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONObject
import java.net.HttpURLConnection
import java.net.URL

data class HttpReq(
    val url: String,
    val method: String = "GET",
    val body: String = "",
    val headers: Map<String, String> = emptyMap(),
    val bytes: ByteArray? = null,
    val timeout: Int = 10000
)

data class HttpRes(
    val code: Int,
    val body: String? = null,
    val bytes: ByteArray? = null,
    val error: String? = null
)

data class Profile(
    val id: Int,
    val fullName: String,
    val username: String,
    val email: String,
    val phone: String,
    val balance: Int,
    val envImpact: Double
)

object HttpClient {
    val addr = "http://10.0.2.2:5000/"
    var token = ""
    lateinit var prefs: SharedPreferences

    var profile by mutableStateOf<Profile?>(null)

    fun saveToken() {
        prefs.edit {
            putString("token", token)
        }
    }

    fun loadToken() {
        token = prefs.getString("token", "") ?: ""
    }

    fun send(req: HttpReq, getByte: Boolean = false): HttpRes {
        val conn = URL(req.url).openConnection() as HttpURLConnection
        return try {
            conn.run {
                requestMethod = req.method
                readTimeout = req.timeout
                connectTimeout = req.timeout
                req.headers.forEach { (k, v) -> setRequestProperty(k, v) }
                if((req.body.isNotEmpty() || req.bytes != null) && req.method in listOf("POST", "PUT", "PATCH")) {
                    getOutputStream().buffered().use { it.write(req.bytes ?: req.body.toByteArray()) }
                }

                connect()
                val code = responseCode
                val body = if(getByte) null else {
                    if(code in 200..299) {
                        getInputStream().bufferedReader().use { it.readText() }
                    } else {
                        errorStream?.bufferedReader()?.use { it.readText() }
                    }
                }
                val bytes = if(!getByte) null else {
                    if(code in 200..299) {
                        getInputStream().buffered().use { it.readBytes() }
                    } else {
                        errorStream?.buffered()?.use { it.readBytes() }
                    }
                }
                HttpRes(code, body, bytes)
            }
        } catch (e: Exception) {
            e.printStackTrace()
            HttpRes(-1, error = e.message ?: "Network error")
        } finally {
            conn.disconnect()
        }
    }

    suspend fun jsonReq(route: String, method: String = "GET", body: String = ""): HttpRes {
        return withContext(Dispatchers.IO) {
            val headers = if(token.isNotEmpty()) mapOf("content-type" to "application/json", "authorization" to "Bearer $token") else mapOf("content-type" to "application/json")
            send(HttpReq("${addr}ecopoin-v1/$route", method, body, headers))
        }
    }

    suspend fun login(username: String, password: String): String {
        val res = jsonReq("users/login", "POST", """{
  "username": "$username",
  "password": "$password"
}""")
        if(res.body == null) return "Login failed"
        return try {
            val json = JSONObject(res.body)
            if(res.code == 200) {
                token = json.getJSONObject("data").getString("token")
                saveToken()
                "ok"
            } else {
                json.optString("message", "Login failed")
            }
        } catch (e: Exception) {
            e.printStackTrace()
            "Login failed"
        }
    }

    suspend fun register(username: String, fullName: String, email: String, phone: String, password: String): String {
        val res = jsonReq("users/login", "POST", """{
  "username": "$username",
  "fullName": "$fullName",
  "email": "$email",
  "phone": "$phone",
  "password": "$password"
}""")
        if(res.body == null) return "Register failed"
        return try {
            val json = JSONObject(res.body)
            if(res.code == 200) {
                token = json.getJSONObject("data").getString("token")
                saveToken()
                "ok"
            } else {
                json.optString("message", "Register failed")
            }
        } catch (e: Exception) {
            e.printStackTrace()
            "Register failed"
        }
    }

    suspend fun me(): Boolean {
        val res = jsonReq("users/me", "GET")
        if(res.body == null) return false
        return try {
            val json = JSONObject(res.body).getJSONObject("data")
            profile = json.run {
                Profile(
                    getInt("id"),
                    getString("fullName"),
                    getString("username"),
                    getString("email"),
                    getString("phone"),
                    getInt("balance"),
                    getDouble("envImpact"),
                )
            }
            res.code == 200
        } catch (e: Exception) {
            e.printStackTrace()
            false
        }
    }

    suspend fun myRank(): Boolean {
        val res = jsonReq("users/me", "GET")
        if(res.body == null) return false
        return try {
            val json = JSONObject(res.body).getJSONObject("data")
            profile = json.run {
                Profile(
                    getInt("id"),
                    getString("fullName"),
                    getString("username"),
                    getString("email"),
                    getString("phone"),
                    getInt("balance"),
                    getDouble("envImpact"),
                )
            }
            res.code == 200
        } catch (e: Exception) {
            e.printStackTrace()
            false
        }
    }
}