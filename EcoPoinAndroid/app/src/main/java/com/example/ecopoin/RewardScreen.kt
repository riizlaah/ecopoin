package com.example.ecopoin

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.Gray1
import com.example.ecopoin.ui.theme.Orange1
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlin.time.Duration.Companion.milliseconds

@Composable
fun RewardScreen(modifier: Modifier) {
    val vouchers = remember { mutableStateListOf<Voucher>() }
    var paging by remember { mutableStateOf(Paging(1,1)) }
    val scope = rememberCoroutineScope()

    LaunchedEffect(Unit) {
        val (p, v) = HttpClient.getVouchers(paging.page)
        if(p != null) {
            paging = p
            vouchers.clear()
            vouchers.addAll(v)
        }
    }
    Column(modifier) {
        Column(Modifier
            .fillMaxWidth()
            .background(Gray1)
            .padding(bottom = 2.dp)
            .background(Color.White)
            .padding(12.dp))
        {
            Text(
                "Vouchers",
                fontWeight = FontWeight.Bold,
                fontSize = MaterialTheme.typography.headlineLarge.fontSize
            )
            Spacer(Modifier.height(12.dp))
        }
        LazyVerticalGrid(GridCells.Fixed(2), Modifier.weight(1f).padding(12.dp), verticalArrangement = Arrangement.spacedBy(12.dp), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
            items(vouchers) { item ->
                var loading by remember { mutableStateOf(false) }
                var errMsg by remember { mutableStateOf("") }
                var btnText by remember { mutableStateOf("Redeem") }

                Column(Modifier.fillMaxWidth().cardBordered()) {
                    Text(item.name, Modifier.fillMaxWidth(), textAlign = TextAlign.Center, fontWeight = FontWeight.Medium)
                    Spacer(Modifier.height(12.dp))
                    Text("${thousandFmt(item.pointCost)} points", Modifier.fillMaxWidth(), textAlign = TextAlign.Center, color = Orange1, fontWeight = FontWeight.Bold)
                    ErrText(errMsg, fontSize = MaterialTheme.typography.labelSmall.fontSize)
                    Button({
                        scope.launch {
                            loading = true
                            when(val msg = HttpClient.redeemVoucher(item.id)) {
                                "ok" -> {
                                    btnText = "Success"
                                }
                                else -> {
                                    errMsg = msg
                                    btnText = "Error"
                                }
                            }
                            loading = false
                            delay(2000.milliseconds)
                            btnText = "Redeem"
                            errMsg = ""
                        }
                    }, Modifier.fillMaxWidth(), shape = corner()) {
                        LoadingOrContent(loading) {
                            Text(btnText)
                        }
                    }
                }
            }
        }

    }

}