package nr.dev.ecopoin

import android.content.ContentResolver
import android.content.SharedPreferences
import android.net.Uri
import android.provider.OpenableColumns
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.core.content.edit
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONObject
import java.io.ByteArrayOutputStream
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
    val username: String,
    val fullName: String,
    val email: String,
    val phone: String,
    val role: String,
    val balance: Int,
    val environmentalImpact: Double,
    val totalSubmittedWeights: Double
)

data class MyRank(
    val rank: Int,
    val totalPoints: Int,
    val fromTotal: Int
)

data class Pagination(
    val page: Int,
    val totalPage: Int
)

data class Voucher(
    val id: Int,
    val name: String,
    val pointCost: Int
)

data class WasteType(
    val id: Int,
    val name: String,
    val pointTariff: Int,
    val co2Factor: Double? = null
)

data class File(
    val name: String,
    val mimetype: String,
    val content: ByteArray
)

object HttpClient {
    val addr = "http://10.0.2.2:5000/"
    var token = ""
    lateinit var prefs: SharedPreferences

    var profile by mutableStateOf<Profile?>(null)
    var myRank by mutableStateOf<MyRank?>(null)

    fun loadToken() {
        token = prefs.getString("token", "") ?: ""
    }

    fun saveToken() {
        prefs.edit {
            putString("token", token)
        }
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
            HttpRes(-1, e.message ?: "Network error")
        } finally {
            conn.disconnect()
        }
    }

    suspend fun jsonReq(route: String, method: String = "GET", body: String = ""): HttpRes {
        return withContext(Dispatchers.IO) {
            val headers = if(token.isNotEmpty()) mapOf("content-type" to "application/json", "authorization" to "Bearer $token") else mapOf("content-type" to "application/json")
            send(HttpReq("${addr}ecopoin-api-v1/$route", method, body, headers))
        }
    }

    suspend fun sendMultipart(route: String, files: Map<String, File>, others: Map<String, String> = emptyMap(), method: String = "POST"): HttpRes {
        return try {
            withContext(Dispatchers.IO) {
                val boundary = "----WebkitFormBoundary${System.currentTimeMillis()}"
                val boundaryB = boundary.toByteArray()
                val crlf = "\r\n".toByteArray()
                val twoH = "--".toByteArray()
                val output = ByteArrayOutputStream()
                output.run {
                    others.forEach { (k, v) ->
                        write(twoH)
                        write(boundaryB)
                        write(crlf)
                        write("Content-Disposition: form-data; name=\"$k\"".toByteArray())
                        write(crlf)
                        write(crlf)
                        write(v.toByteArray())
                        write(crlf)
                    }
                    files.forEach { (k, file) ->
                        write(twoH)
                        write(boundaryB)
                        write(crlf)
                        write("Content-Disposition: form-data; name=\"$k\"; filename=\"${file.name}\"".toByteArray())
                        write(crlf)
                        write("Content-Type: ${file.mimetype}".toByteArray())
                        write(crlf)
                        write(crlf)
                        write(file.content)
                        write(crlf)
                    }
                    write(twoH)
                    write(boundaryB)
                    write(twoH)
                    write(crlf)
                }
                val headers = mapOf("content-type" to "multipart/form-data; boundary=$boundary", "authorization" to "Bearer $token")
                send(HttpReq("${addr}ecopoin-api-v1/$route", method, headers = headers, bytes = output.toByteArray()))
            }
        } catch (e: Exception) {
            e.printStackTrace()
            HttpRes(-1, e.message ?: "Parsing error")
        }
    }

    suspend fun login(username: String, password: String): String {
        val res = jsonReq("users/login", "POST", """{
  "username": "$username",
  "password": "$password"}""")
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
        val res = jsonReq("users/register", "POST", """{
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
        val res = jsonReq("users/me")
        if(res.body == null) return false
        return try {
            val json = JSONObject(res.body).getJSONObject("data")
            profile = json.run {
                Profile(
                    getInt("id"),
                    getString("username"),
                    getString("fullName"),
                    getString("email"),
                    getString("phone"),
                    getString("role"),
                    getInt("balance"),
                    getDouble("environmentalImpact"),
                    getDouble("totalSubmittedWeights"),
                )
            }
            res.code == 200
        } catch (e: Exception) {
            e.printStackTrace()
            false
        }
    }

    suspend fun myRank(): Boolean {
        val res = jsonReq("leaderboard/my-rank")
        if(res.body == null) return false
        return try {
            val json = JSONObject(res.body).getJSONObject("data")
            myRank = json.run {
                MyRank(
                    getInt("id"),
                    getInt("totalPoints"),
                    getInt("fromTotal"),
                )
            }
            res.code == 200
        } catch (e: Exception) {
            e.printStackTrace()
            false
        }
    }

    suspend fun getVouchers(page: Int = 1, size: Int = 20): Pair<Pagination?, List<Voucher>> {
        val res = jsonReq("vouchers?page=$page&size=$size")
        if(res.body == null || res.code != 200) return Pair(null, emptyList())
        return try {
            val json = JSONObject(res.body)
            val paging = json.getJSONObject("pagination").run {
                Pagination(getInt("page"), getInt("totalPage"))
            }
            val arr = mutableListOf<Voucher>()
            val arrJson = json.getJSONArray("data")
            for(i in 0 until arrJson.length()) {
                arr.add(arrJson.getJSONObject(i).run {
                    Voucher(getInt("id"), getString("name"), getInt("pointCost"))
                })
            }
            Pair(paging, arr)
        } catch(e: Exception) {
            e.printStackTrace()
            Pair(null, emptyList())
        }
    }

    suspend fun getWasteTypes(): List<WasteType> {
        val res = jsonReq("wasteTypes?page=1&size=20")
        if(res.body == null || res.code != 200) return emptyList()
        return try {
            val json = JSONObject(res.body)
            val arr = mutableListOf<WasteType>()
            val arrJson = json.getJSONArray("data")
            for(i in 0 until arrJson.length()) {
                arr.add(arrJson.getJSONObject(i).run {
                    WasteType(getInt("id"), getString("name"), getInt("pointTariff"))
                })
            }
            arr
        } catch(e: Exception) {
            e.printStackTrace()
            emptyList()
        }
    }

    suspend fun submitTrash(wasteTypeId: Int, estimatedWeight: Double, photo: File, notes: String): String {
        val res = sendMultipart("deposits", mapOf("photo" to photo), mapOf(
            "wasteTypeId" to wasteTypeId.toString(),
            "estimatedWeight" to estimatedWeight.toString().replace('.',','),
            "notes" to notes
        ))
        if(res.body == null) return "Submit failed"
        return try {
            val json = JSONObject(res.body)
            if(res.code == 200) return "ok"
            else json.optString("message", "Submit failed")
        } catch (e: Exception) {
            e.printStackTrace()
            "Submit failed"
        }
    }
}

data class Tier(
    val name: String,
    val minPoints: Int,
    val maxPoints: Int
)

fun getTier(points: Int): Tier {
    val tiers = listOf(
        Tier("Bronze", 0, 499),
        Tier("Silver", 500, 999),
        Tier("Gold", 1000, 1999),
        Tier("Platinum", 2000, 3999),
        Tier("Diamond", 4000, 7999),
    )
    return tiers.first { it.minPoints <= points && points <= it.maxPoints }
}

fun getNextTier(points: Int): Tier {
    val tiers = listOf(
        Tier("Bronze", 0, 499),
        Tier("Silver", 500, 999),
        Tier("Gold", 1000, 1999),
        Tier("Platinum", 2000, 3999),
        Tier("Diamond", 4000, 7999),
    )
    val idx = tiers.indexOfFirst { it.minPoints <= points && points <= it.maxPoints }
    return tiers[idx + 1]
}

fun ContentResolver.getFilename(uri: Uri): String? {
    return try {
        query(uri, null, null, null, null)?.use { q ->
            val idx = q.getColumnIndex(OpenableColumns.DISPLAY_NAME)
            if(idx != -1 && q.moveToFirst()) {
                q.getString(idx)
            } else {
                null
            }
        }
    } catch (e: Exception) {
        null
    }
}

suspend fun ContentResolver.getBytes(uri: Uri): ByteArray? {
    return try {
        withContext(Dispatchers.IO) {
            openInputStream(uri)?.use { s ->
                s.buffered().use { it.readBytes() }
            }
        }
    } catch (e: Exception) {
        null
    }
}
