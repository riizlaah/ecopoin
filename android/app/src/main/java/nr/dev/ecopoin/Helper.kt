package nr.dev.ecopoin

import android.content.ContentResolver
import android.content.SharedPreferences
import android.graphics.BitmapFactory
import android.net.Uri
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.asImageBitmap
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
    val errors: String? = null,
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

data class Tier(
    val name: String,
    val minPoints: Int,
    val maxPoints: Int
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
)


data class Pagination(
    val page: Int,
    val totalPage: Int
)

data class File(
    val name: String,
    val content: ByteArray,
    val mimetype: String = "application/octet-stream"
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
                if ((req.body.isNotEmpty() || req.bytes != null) && req.method in listOf(
                        "POST",
                        "PUT",
                        "PATCH"
                    )
                ) {
                    getOutputStream().buffered().use {
                        it.write(req.bytes ?: req.body.toByteArray())
                    }
                }
                connect()

                val code = responseCode
                val body = if (getByte) null else {
                    if (code in 200..299) {
                        getInputStream().bufferedReader().use { it.readText() }
                    } else {
                        errorStream?.bufferedReader()?.use { it.readText() }
                    }
                }
                val bytes = if (!getByte) null else {
                    if (code in 200..299) {
                        getInputStream().buffered().use { it.readBytes() }
                    } else {
                        errorStream?.buffered()?.use { it.readBytes() }
                    }
                }
                HttpRes(code, body, bytes)
            }
        } catch (e: Exception) {
            e.printStackTrace()
            HttpRes(-1, errors = e.message ?: "Unknown error")
        }
    }

    suspend fun jsonReq(route: String, method: String = "GET", body: String = ""): HttpRes {
        return withContext(Dispatchers.IO) {
            val url = "${addr}ecopoin-api-v1/${route}"
            val headers = if (token.isNotEmpty()) mapOf(
                "content-type" to "application/json",
                "authorization" to "Bearer $token"
            ) else mapOf("content-type" to "application/json")
            send(HttpReq(url, method, body, headers))
        }
    }

    suspend fun sendMultipart(
        route: String,
        files: Map<String, File>,
        others: Map<String, String> = emptyMap(),
        method: String = "POST"
    ): HttpRes {
        val boundary = "----WebkitFormBoundary${System.currentTimeMillis()}"
        val boundaryBytes = boundary.toByteArray()
        val crlf = "\r\n".toByteArray()
        val twoH = "--".toByteArray()
        val outputStream = ByteArrayOutputStream()
        return try {
            withContext(Dispatchers.IO) {

                others.forEach { (k, v) ->
                    outputStream.run {
                        write(twoH)
                        write(boundaryBytes)
                        write(crlf)
                        write("Content-Disposition: form-data; name=\"$k\"".toByteArray())
                        write(crlf)
                        write(crlf)
                        write(v.toByteArray())
                        write(crlf)
                    }
                }
                files.forEach { (k, file) ->
                    outputStream.run {
                        write(twoH)
                        write(boundaryBytes)
                        write(crlf)
                        write("Content-Disposition: form-data; name=\"$k\"; filename=\"${file.name}\"".toByteArray())
                        write(crlf)
                        write("Content-Type: ${file.mimetype}".toByteArray())
                        write(crlf)
                        write(crlf)
                        write(file.content)
                        write(crlf)
                    }
                }
                outputStream.run {
                    write(twoH)
                    write(boundaryBytes)
                    write(twoH)
                    write(crlf)
                }
                val bytes = outputStream.toByteArray()
                val headers = mapOf(
                    "content-type" to "multipart/form-data; boundary=$boundary",
                    "authorization" to "Bearer $token"
                )
                send(HttpReq("${addr}ecopoin-api-v1/$route", method, "", headers, bytes))
            }
        } catch (e: Exception) {
            e.printStackTrace()
            HttpRes(-1, errors = e.message ?: "Unknown error")
        }

    }

    suspend fun login(username: String, password: String): String {
        val res =
            jsonReq("users/login", "POST", """{"username": "$username", "password": "$password"}""")
        if (res.body == null) return "Login failed"
        return try {
            val json = JSONObject(res.body)
            if (res.code == 200) {
                token = json.getJSONObject("data").getString("token")
                saveToken()
                "ok"
            } else {
                json.optString("message", "Login failed")
            }
        } catch (e: Exception) {
            e.printStackTrace()
            "Login Failed"
        }
    }

    suspend fun register(
        username: String,
        fullName: String,
        email: String,
        phone: String,
        password: String
    ): String {
        val res = jsonReq(
            "users/register", "POST", """{
  "username": "$username",
  "fullName": "$fullName",
  "email": "$email",
  "phone": "$phone",
  "password": "$password"
}"""
        )
        if (res.body == null) return "Register failed"
        return try {
            val json = JSONObject(res.body)
            if (res.code == 200) {
                "ok"
            } else {
                json.optString("message", "Register failed")
            }
        } catch (e: Exception) {
            e.printStackTrace()
            "Register Failed"
        }
    }

    suspend fun me(): Boolean {
        val res = jsonReq("users/me")
        if (res.body == null) return false
        return try {
            val json = JSONObject(res.body).getJSONObject("data")
            profile = Profile(
                json.getInt("id"),
                json.getString("username"),
                json.getString("fullName"),
                json.getString("email"),
                json.getString("phone"),
                json.getString("role"),
                json.getInt("balance"),
                json.getDouble("environmentalImpact"),
                json.getDouble("totalSubmittedWeights"),
            )
            res.code == 200
        } catch (e: Exception) {
            e.printStackTrace()
            false
        }
    }

    suspend fun myRank(): Boolean {
        val res = jsonReq("leaderboard/my-rank")
        if (res.body == null) return false
        return try {
            val json = JSONObject(res.body).getJSONObject("data")
            myRank = MyRank(
                json.getInt("rank"),
                json.getInt("totalPoints"),
                json.getInt("fromTotal"),
            )
            res.code == 200
        } catch (e: Exception) {
            e.printStackTrace()
            false
        }
    }

    suspend fun getVouchers(page: Int = 1, size: Int = 10): Pair<Pagination?, List<Voucher>> {
        val res = jsonReq("vouchers?page=$page&size=$size")
        if (res.body == null) return Pair(null, emptyList())
        return try {
            val json = JSONObject(res.body)
            val paging = json.getJSONObject("pagination")
            val arrJson = json.getJSONArray("data")
            val arr = mutableListOf<Voucher>()
            for (i in 0 until arrJson.length()) {
                val obj = arrJson.getJSONObject(i)
                arr.add(
                    Voucher(
                        obj.getInt("id"),
                        obj.getString("name"),
                        obj.getInt("pointCost"),
                    )
                )
            }
            Pair(
                Pagination(
                    paging.getInt("page"),
                    paging.getInt("totalPage"),
                ), arr
            )
        } catch (e: Exception) {
            e.printStackTrace()
            Pair(null, emptyList())
        }
    }

    suspend fun getWasteTypes(size: Int = 32): List<WasteType> {
        val res = jsonReq("wastetypes?page=1&size=$size")
        if (res.body == null) return emptyList()
        return try {
            val json = JSONObject(res.body)
            val arrJson = json.getJSONArray("data")
            val arr = mutableListOf<WasteType>()
            for (i in 0 until arrJson.length()) {
                val obj = arrJson.getJSONObject(i)
                arr.add(
                    WasteType(
                        obj.getInt("id"),
                        obj.getString("name"),
                        obj.getInt("pointTariff"),
                    )
                )
            }
            arr
        } catch (e: Exception) {
            e.printStackTrace()
            emptyList()
        }
    }

    suspend fun submitDeposit(
        estimatedWeight: Double,
        wasteTypeId: Int,
        photo: File,
        notes: String
    ): String {
        val res =
            sendMultipart("deposits", mapOf("photo" to photo), others = mapOf(
                "wasteTypeId" to wasteTypeId.toString(),
                "estimatedWeight" to estimatedWeight.toString().replace('.', ','),
                "notes" to notes
            ))
        if(res.body == null) return "Failed to submit"
        return try {
            val json = JSONObject(res.body)
            if(res.code == 200) return "ok"
            else json.optString("message", "Failed to submit")
        } catch (e: Exception) {
            e.printStackTrace()
            "Failed to submit"
        }
    }

}

fun getTier(points: Int): Tier {
    val tiers = listOf(
        Tier("Bronze", 0, 499),
        Tier("Silver", 500, 999),
        Tier("Gold", 1000, 1999),
        Tier("Platinum", 2000, 3999),
        Tier("Diamond", 4000, 7999),
        Tier("Emerald", 8000, 15999),
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
        Tier("Emerald", 8000, 15999),
    )
    val idx = tiers.indexOfFirst { it.minPoints <= points && points <= it.maxPoints }
    return tiers[idx + 1]
}

fun ContentResolver.asBitmap(uri: Uri): ImageBitmap? {
    return try {
        openInputStream(uri)?.use {
            it
            val bytes = it.buffered().use { b ->
                b.readBytes()
            }
            BitmapFactory.decodeByteArray(bytes, 0, bytes.size).asImageBitmap()
        }
    } catch (e: Exception) {
        e.printStackTrace()
        null
    }
}