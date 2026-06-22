package com.example.ecopoin

import android.content.ContentResolver
import android.content.SharedPreferences
import android.graphics.BitmapFactory
import android.net.Uri
import android.provider.OpenableColumns
import androidx.compose.runtime.derivedStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.getValue
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
import java.text.DecimalFormat
import java.time.LocalDateTime

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
    val role: String,
    val balance: Int,
    val submittedWeight: Double,
    val envImpact: Double
)

data class UserRank(
    val rank: Int,
    val fullName: String,
    val balance: Int,
    val submittedWeight: Double,
    val envImpact: Double,
    val points: Int,
    val fromTotal: Int = 0
)

data class Voucher(
    val id: Int,
    val name: String,
    val pointCost: Int
)

data class Paging(
    val page: Int = 1,
    val totalPage: Int = 1
)

data class File(
    val name: String,
    val mimetype: String,
    val bytes: ByteArray
)

data class WasteType(
    val id: Int,
    val name: String,
    val pointTariff: Int,
    val co2Factor: Double
)

data class ShortUser(
    val id: Int,
    val name: String,
    val email: String
)

data class Deposit(
    val id: Int,
    val resident: ShortUser,
    val wasteType: WasteType,
    val estWeight: Double,
    val estPoints: Int,
    val actWeight: Double?,
    val actPoints: Int?,
    val status: String,
    val updatedAt: LocalDateTime,
    val officer: ShortUser? = null,
    val createdAt: LocalDateTime? = null,
    val notes: String? = null,
    val rejectionReason: String? = null,
    val photoPath: String? = null
)

data class RedeemedVoucher(
    val id: Int,
    val voucher: Voucher,
    val isUsed: Boolean,
    val code: String,
    val amount: Int,
    val createdAt: LocalDateTime,
    val updatedAt: LocalDateTime
)


object HttpClient {
    val addr = "http://10.0.2.2:5000/"
    var token = ""
    lateinit var prefs: SharedPreferences

    var profile by mutableStateOf<Profile?>(null)
    val isOfficer by derivedStateOf { profile?.role == "officer" }
    var myRank by mutableStateOf<UserRank?>(null)

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
                connectTimeout = req.timeout
                readTimeout = req.timeout
                req.headers.forEach { (k, v) -> setRequestProperty(k, v) }
                if ((req.body.isNotEmpty() || req.bytes != null) && req.method in listOf(
                        "POST",
                        "PUT",
                        "PATCH"
                    )
                ) {
                    getOutputStream().buffered()
                        .use { it.write(req.bytes ?: req.body.toByteArray()) }
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
            HttpRes(-1, e.message ?: "Network error")
        } finally {
            conn.disconnect()
        }
    }

    suspend fun sendMultipart(
        route: String,
        files: Map<String, File>,
        others: Map<String, String> = emptyMap(),
        method: String = "POST",
        errMsg: String = "Error"
    ): String {
        return try {
            val res = withContext(Dispatchers.IO) {
                val boundary = "----WKFB${System.currentTimeMillis()}"
                val bb = boundary.toByteArray()
                val crlf = "\r\n".toByteArray()
                val twoH = "--".toByteArray()
                val output = ByteArrayOutputStream()
                output.run {
                    others.forEach { (k, v) ->
                        write(twoH)
                        write(bb)
                        write(crlf)
                        write("Content-Disposition: form-data; name=\"$k\"".toByteArray())
                        write(crlf)
                        write(crlf)
                        write(v.toByteArray())
                        write(crlf)
                    }
                    files.forEach { (k, f) ->
                        write(twoH)
                        write(bb)
                        write(crlf)
                        write("Content-Disposition: form-data; name=\"$k\"; filename=\"${f.name}\"".toByteArray())
                        write(crlf)
                        write("Content-Type: ${f.mimetype}".toByteArray())
                        write(crlf)
                        write(crlf)
                        write(f.bytes)
                        write(crlf)
                    }
                    write(twoH)
                    write(bb)
                    write(twoH)
                    write(crlf)
                }
                val headers = mapOf(
                    "content-type" to "multipart/form-data; boundary=$boundary",
                    "authorization" to "Bearer $token"
                )
                send(
                    HttpReq(
                        "${addr}api/$route",
                        method,
                        bytes = output.toByteArray(),
                        headers = headers
                    )
                )
            }
            if (res.body == null) errMsg
            else {
                val json = JSONObject(res.body)
                if (res.code == 200) "ok"
                else json.optString("message", errMsg)
            }
        } catch (e: Exception) {
            e.printStackTrace()
            errMsg
        }
    }

    suspend fun fetchImg(path: String): ImageBitmap? {
        return try {
            withContext(Dispatchers.IO) {
                val res = send(HttpReq("${addr}uploads/$path"), true)
                if (res.bytes == null) null
                else BitmapFactory.decodeByteArray(res.bytes, 0, res.bytes.size).asImageBitmap()
            }
        } catch (e: Exception) {
            e.printStackTrace()
            null
        }
    }

    suspend fun jsonReq(
        route: String,
        method: String = "GET",
        body: String = "",
        errMsg: String = "Error",
        onSuccess: JSONObject.() -> String
    ): String {
        val res = withContext(Dispatchers.IO) {
            val headers = if (token.isNotEmpty()) mapOf(
                "content-type" to "application/json",
                "authorization" to "Bearer $token"
            ) else mapOf("content-type" to "application/json")
            send(HttpReq("${addr}api/$route", method, body, headers))
        }
        if (res.body == null) return errMsg
        return try {
            JSONObject(res.body).run {
                if (res.code != 200) optString("message", errMsg)
                else {
                    if(isNull("data")) run(onSuccess)
                    else getJSONObject("data").run(onSuccess)
                }
            }
        } catch (e: Exception) {
            e.printStackTrace()
            errMsg
        }
    }

    suspend fun <R> jsonReqPaginate(
        route: String,
        method: String = "GET",
        body: String = "",
        errMsg: String = "Error",
        onEach: JSONObject.() -> R
    ): Pair<Paging?, List<R>> {
        val res = withContext(Dispatchers.IO) {
            val headers = if (token.isNotEmpty()) mapOf(
                "content-type" to "application/json",
                "authorization" to "Bearer $token"
            ) else mapOf("content-type" to "application/json")
            send(HttpReq("${addr}api/$route", method, body, headers))
        }
        val list = mutableListOf<R>()
        if (res.body == null) return Pair(null, emptyList())
        return try {
            val json = JSONObject(res.body)
            if (res.code != 200) {
                Pair(null, emptyList())
            } else {
                var paging: Paging? = null
                json.run {
                    if (has("pagination")) {
                        getJSONObject("pagination").run {
                            paging = Paging(getInt("page"), getInt("totalPage"))
                        }
                    }
                    getJSONArray("data").run {
                        for (i in 0 until length()) {
                            val obj = getJSONObject(i)
                            list.add(obj.run(onEach))
                        }
                    }
                    Pair(paging, list)
                }
            }
        } catch (e: Exception) {
            e.printStackTrace()
            Pair(null, emptyList())
        }
    }

    suspend fun me(): Boolean {
        return jsonReq("users/me", onSuccess = {
            profile = Profile(
                getInt("id"),
                getString("username"),
                getString("fullName"),
                getString("role"),
                getInt("balance"),
                getDouble("submittedWeight"),
                getDouble("envImpact"),
            )
            "ok"
        }) == "ok"
    }

    suspend fun login(username: String, password: String): String {
        return jsonReq(
            "users/login", "POST", """{
  "username": "$username",
  "password": "$password"
}""", "Login failed"
        ) {
            token = getString("token")
            saveToken()
            "ok"
        }
    }

    suspend fun register(
        username: String,
        fullName: String,
        email: String,
        phone: String,
        password: String
    ): String {
        return jsonReq(
            "users/register", "POST", """{
  "username": "$username",
  "fullName": "$fullName",
  "email": "$email",
  "phone": "$phone",
  "password": "$password"
}""", "Register failed"
        ) {
            "ok"
        }
    }

    suspend fun myRank(): Boolean {
        return jsonReq("leaderboard/my-rank", onSuccess = {
            myRank = UserRank(
                getInt("rank"),
                getString("fullName"),
                getInt("balance"),
                getDouble("submittedWeight"),
                getDouble("envImpact"),
                getInt("points"),
                getInt("fromTotal"),
            )
            "ok"
        }) == "ok"
    }

    suspend fun getVouchers(page: Int = 1, size: Int = 10): Pair<Paging?, List<Voucher>> {
        return jsonReqPaginate("vouchers?page=$page&size=$size", onEach = {
            Voucher(getInt("id"), getString("name"), getInt("pointCost"))
        })
    }

    suspend fun getWasteTypes(): Pair<Paging?, List<WasteType>> {
        return jsonReqPaginate("wasteTypes", onEach = {
            WasteType(
                getInt("id"),
                getString("name"),
                getInt("pointTariff"),
                getDouble("co2Factor")
            )
        })
    }

    suspend fun submitTrash(
        wasteTypeId: Int,
        estWeight: Double,
        photo: File,
        notes: String
    ): String {
        return sendMultipart(
            "deposits", mapOf("photo" to photo), mapOf(
                "wasteTypeId" to wasteTypeId.toString(),
                "estWeight" to estWeight.toString().replace('.', ','),
                "notes" to notes
            ), errMsg = "Submit Trash failed"
        )
    }

    suspend fun getDeposits(page: Int, size: Int, status: String): Pair<Paging?, List<Deposit>> {
        return jsonReqPaginate("deposits?page=$page&status=$status") {
            val resident = getJSONObject("resident").run {
                ShortUser(getInt("id"), getString("name"), getString("email"))
            }
            val wasteType = getJSONObject("wasteType").run {
                WasteType(
                    getInt("id"),
                    getString("name"),
                    getInt("pointTariff"),
                    getDouble("co2Factor")
                )
            }
            Deposit(
                getInt("id"),
                resident,
                wasteType,
                getDouble("estWeight"),
                getInt("estPoints"),
                if (isNull("actWeight")) null else getDouble("actWeight"),
                if (isNull("actPoints")) null else getInt("actPoints"),
                getString("status"),
                LocalDateTime.parse(getString("updatedAt"))
            )
        }
    }

    suspend fun getVoucherHistory(page: Int, status: String): Pair<Paging?, List<RedeemedVoucher>> {
        return jsonReqPaginate("vouchers/history?page=$page&status=$status") {
            val voucher = getJSONObject("voucher").run {
                Voucher(getInt("id"), getString("name"), getInt("pointCost"))
            }
            RedeemedVoucher(
                getInt("id"),
                voucher,
                getBoolean("isUsed"),
                getString("code"),
                getInt("amount"),
                LocalDateTime.parse(getString("createdAt")),
                LocalDateTime.parse(getString("updatedAt"))
            )
        }
    }


    suspend fun redeemVoucher(id: Int): String {
        return jsonReq("vouchers/$id", "POST", errMsg = "Can't redeem voucher") {
            "ok"
        }
    }

    suspend fun getRanks(page: Int, size: Int, range: String): Pair<Paging?, List<UserRank>> {
        return jsonReqPaginate("leaderboard?page=$page&size=$size&range=$range") {
            UserRank(
                getInt("rank"),
                getString("fullName"),
                getInt("balance"),
                getDouble("submittedWeight"),
                getDouble("envImpact"),
                getInt("points"),
            )
        }
    }

    suspend fun getDepositDetail(id: Int): Deposit? {
        var dp: Deposit? = null
        jsonReq("deposits/$id") {
            val resident = getJSONObject("resident").run {
                ShortUser(getInt("id"), getString("name"), getString("email"))
            }
            val officer = if (isNull("officer")) null else getJSONObject("officer").run {
                ShortUser(getInt("id"), getString("name"), getString("email"))
            }
            val wasteType = getJSONObject("wasteType").run {
                WasteType(
                    getInt("id"),
                    getString("name"),
                    getInt("pointTariff"),
                    getDouble("co2Factor")
                )
            }
            dp = Deposit(
                getInt("id"),
                resident,
                wasteType,
                getDouble("estWeight"),
                getInt("estPoints"),
                if (isNull("actWeight")) null else getDouble("actWeight"),
                if (isNull("actPoints")) null else getInt("actPoints"),
                getString("status"),
                LocalDateTime.parse(getString("updatedAt")),
                officer,
                LocalDateTime.parse(getString("createdAt")),
                if (isNull("notes")) null else getString("notes"),
                if (isNull("rejectionReason")) null else getString("rejectionReason"),
                if (isNull("photoPath")) null else getString("photoPath"),
            )
            "ok"
        }
        return dp
    }

    suspend fun updateDeposit(
        id: Int,
        wasteTypeId: Int,
        estWeight: Double,
        photo: File?,
        notes: String = ""
    ): String {
        return sendMultipart(
            "deposits/$id",
             if (photo == null) emptyMap() else mapOf("photo" to photo),
            mapOf(
                "wasteTypeId" to wasteTypeId.toString(),
                "estWeight" to estWeight.toString().replace('.', ','),
                "notes" to notes
            ),
             "PUT"
        )
    }

    suspend fun reviewDeposit(
        id: Int, wasteTypeId: Int,
        actWeight: Double,
        photo: File?,
        verified: Boolean,
        rejectionReason: String = ""
    ): String {
        return sendMultipart(
            "deposits/$id/verify",
            if (photo == null) emptyMap() else mapOf("photo" to photo),
            mapOf(
                "wasteTypeId" to wasteTypeId.toString(),
                "actWeight" to actWeight.toString().replace('.', ','),
                "verify" to if(verified) "true" else "false",
                "rejectionReason" to rejectionReason
            ),
            "PATCH"
        )
    }
}

fun ContentResolver.getFileName(uri: Uri): String? {
    return try {
        query(uri, null, null, null, null)?.use { q ->
            val idx = q.getColumnIndex(OpenableColumns.DISPLAY_NAME)
            if (idx != -1 && q.moveToFirst()) {
                q.getString(idx)
            } else null
        }
    } catch (e: Exception) {
        e.printStackTrace()
        null
    }
}

fun ContentResolver.getBytes(uri: Uri): ByteArray? {
    return try {
        openInputStream(uri)?.use {
            it.buffered().readBytes()
        }
    } catch (e: Exception) {
        null
    }
}


data class Tier(
    val name: String,
    val minPoint: Int,
    val maxPoint: Int
)

fun getTier(points: Int): Tier {
    val tiers = listOf(
        Tier("Bronze", 0, 499),
        Tier("Silver", 500, 999),
        Tier("Gold", 1000, 1999),
        Tier("Platinum", 2000, 3999),
        Tier("Diamond", 4000, 7999),
        Tier("Emerald", 8000, 15999),
    )
    return tiers.first { it.minPoint <= points && points <= it.maxPoint }
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
    val idx = tiers.indexOfFirst { it.minPoint <= points && points <= it.maxPoint }
    return tiers[idx + 1]
}

fun thousandFmt(n: Int): String {
    return DecimalFormat("#,###").format(n).replace(',', '.')
}
