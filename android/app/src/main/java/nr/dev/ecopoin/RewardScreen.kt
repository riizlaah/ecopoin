package nr.dev.ecopoin

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.GridItemSpan
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import nr.dev.ecopoin.ui.theme.yellow
import java.text.DecimalFormat

@Composable
fun RewardScreen(modifier: Modifier) {
    val formatter = DecimalFormat("#,###")
    val vouchers = remember { mutableStateListOf<Voucher>() }
    var paging by remember { mutableStateOf(Pagination(1, 1)) }
    val scope = rememberCoroutineScope()

    LaunchedEffect(Unit) {
        val (p, v) = HttpClient.getVouchers(paging.page)
        if (p != null) {
            paging = p
            vouchers.clear()
            vouchers.addAll(v)
        }
    }

    LazyVerticalGrid(
        GridCells.Fixed(2),
        modifier,
        verticalArrangement = Arrangement.spacedBy(12.dp)
    ) {
        stickyHeader {
            Column(
                Modifier
                    .fillMaxWidth()
                    .background(Color.LightGray)
                    .padding(bottom = 2.dp)
                    .background(Color.White)
                    .padding(12.dp)
            ) {
                Text(
                    "Vouchers",
                    fontWeight = FontWeight.Bold,
                    fontSize = MaterialTheme.typography.displaySmall.fontSize,
                    textAlign = TextAlign.Center,
                    modifier = Modifier.fillMaxWidth()
                )
            }
        }
        item(span = { GridItemSpan(2) }) { Spacer(Modifier.height(12.dp)) }
        items(vouchers) { item ->
            Column(
                Modifier
                    .padding(horizontal = 12.dp)
                    .fillMaxWidth()
                    .clip(corner(12.dp))
                    .background(Color.White)
                    .border(1.dp, Color.LightGray, corner(12.dp))
                    .padding(12.dp)
            ) {
                var loading by remember { mutableStateOf(false) }
                var errMsg by remember { mutableStateOf("") }

                Text(item.name, fontWeight = FontWeight.Medium, textAlign = TextAlign.Center)
                Spacer(Modifier.height(12.dp))
                Text(
                    "${formatter.format(item.pointCost).replace(',', '.')} points",
                    color = yellow,
                    fontWeight = FontWeight.Medium,
                    textAlign = TextAlign.Center
                )
                Spacer(Modifier.height(24.dp))
                ErrText(errMsg, Modifier.fillMaxWidth())
                Button({
                    scope.launch {
                        loading = true
                        when (val msg = HttpClient.redeemVoucher(item.id)) {
                            "ok" -> {
                            }
                            else -> {
                                errMsg = msg
                                delay(5000)
                                errMsg = ""
                            }
                        }
                        loading = false
                    }
                }, Modifier.fillMaxWidth(), shape = corner(12.dp), enabled = !loading) {
                    LoadingOrContent(loading) {
                        Text("Redeem")
                    }
                }
            }
        }
        item(span = { GridItemSpan(2) }) {
            NavPage(
                Modifier.fillMaxWidth(),
                paging,
                { paging = paging.copy(page = paging.page - 1) },
                { paging = paging.copy(page = paging.page + 1) })
        }
    }
}
