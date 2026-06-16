package nr.dev.ecopoin

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.SecondaryTabRow
import androidx.compose.material3.Tab
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import nr.dev.ecopoin.ui.theme.gray1
import java.time.format.DateTimeFormatter

@Composable
fun HistoryScreen(modifier: Modifier) {
    val ctx = LocalContext.current

    val tabs = listOf("vouchers", "deposits")
    var selectedTab by remember { mutableIntStateOf(0) }

    val voucherFilters = listOf("all", "unused", "used")
    var selectedVoucherFilter by remember { mutableStateOf(voucherFilters[0]) }
    val vouchers = remember { mutableStateListOf<RedeemedVoucher>() }
    var pagingV by remember { mutableStateOf(Pagination(1, 1)) }

    val depositFilters = listOf("All", "Pending", "Verified", "Rejected")
    var selectedDepositFilter by remember { mutableStateOf(depositFilters[0]) }
    val deposits = remember { mutableStateListOf<Deposit>() }
    var pagingD by remember { mutableStateOf(Pagination(1, 1)) }

    suspend fun loadVouchers() {
        val (p, v) = HttpClient.getRedeemedVoucher(pagingV.page, status = selectedVoucherFilter)
        if (p != null) {
            vouchers.clear()
            pagingV = p
            vouchers.addAll(v)
        }
    }

    suspend fun loadDeposits() {
        val (p, v) = HttpClient.getDeposits(pagingV.page, status = selectedDepositFilter)
        if (p != null) {
            deposits.clear()
            pagingD = p
            deposits.addAll(v)
        }
    }

    LaunchedEffect(Unit) {
        loadVouchers()
        loadDeposits()
    }

    LaunchedEffect(selectedVoucherFilter, pagingV) {
        loadVouchers()
    }

    LaunchedEffect(selectedDepositFilter, pagingD) {
        loadDeposits()
    }

//    LaunchedEffect(pagingV) {
//        loadVouchers()
//    }
//
//    LaunchedEffect() { }

    LazyColumn(modifier, verticalArrangement = Arrangement.spacedBy(12.dp)) {
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
                    "History",
                    fontWeight = FontWeight.Bold,
                    fontSize = MaterialTheme.typography.displaySmall.fontSize,
                    textAlign = TextAlign.Center,
                    modifier = Modifier.fillMaxWidth()
                )
                SecondaryTabRow(selectedTab, Modifier.padding(vertical = 12.dp)) {
                    tabs.forEachIndexed { idx, tab ->
                        Tab(selectedTab == idx, { selectedTab = idx }, Modifier.padding(8.dp)) {
                            Text(tab.replaceFirstChar { it.uppercase() })
                        }
                    }
                }
                if (selectedTab == 0) {
                    LazyRow(
                        Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp)
                    ) {
                        items(voucherFilters) { name ->
                            val selected = name == selectedVoucherFilter
                            Button(
                                { selectedVoucherFilter = name },
                                shape = corner(50),
                                colors = ButtonDefaults.buttonColors(
                                    containerColor = if (selected) MaterialTheme.colorScheme.primary else gray1,
                                    contentColor = if (selected) Color.White else Color.Gray
                                )
                            ) {
                                Text(
                                    name.replaceFirstChar { it.uppercase() },
                                    fontWeight = if (selected) FontWeight.Bold else FontWeight.Normal
                                )
                            }
                        }
                    }
                } else {
                    LazyRow(
                        Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp)
                    ) {
                        items(depositFilters) { name ->
                            val selected = name == selectedDepositFilter
                            Button(
                                { selectedDepositFilter = name },
                                shape = corner(50),
                                colors = ButtonDefaults.buttonColors(
                                    containerColor = if (selected) MaterialTheme.colorScheme.primary else gray1,
                                    contentColor = if (selected) Color.White else Color.Gray
                                )
                            ) {
                                Text(
                                    name,
                                    fontWeight = if (selected) FontWeight.Bold else FontWeight.Normal
                                )
                            }
                        }
                    }
                }
            }
        }

        item { Spacer(Modifier.height(8.dp)) }

        if (selectedTab == 0) {
            items(vouchers) { item ->
                Column(
                    Modifier
                        .padding(horizontal = 12.dp)
                        .fillMaxWidth()
                        .alpha(if (item.isUsed) 0.75f else 1f)
                        .clip(corner(12.dp))
                        .background(Color.White)
                        .border(1.dp, Color.LightGray, corner(12.dp))
                        .padding(12.dp)
                ) {
                    Row(Modifier.fillMaxWidth()) {
                        Column(Modifier.weight(1f)) {
                            Text(item.voucher.name, fontWeight = FontWeight.Bold)
                            val createdAt =
                                item.createdAt.format(DateTimeFormatter.ofPattern("d MMM yyyy"))
                            Text("${item.amount} | $createdAt")
                        }
                        Text(if (item.isUsed) "Used" else "Not Used Yet")
                    }
                    Button({}, Modifier.fillMaxWidth(), shape = corner(12.dp)) {
                        Text("Detail")
                    }
                }
            }
            item {
                NavPage(Modifier.fillMaxWidth(), pagingV, {
                    pagingV = pagingV.copy(page = pagingV.page - 1)
                }, { pagingV = pagingV.copy(page = pagingV.page + 1) })
            }
        } else {
            items(deposits) { item ->
                Row(
                    Modifier
                        .padding(horizontal = 12.dp)
                        .fillMaxWidth()
                        .clip(corner(12.dp))
                        .background(Color.White)
                        .border(1.dp, Color.LightGray, corner(12.dp))
                        .padding(12.dp)
                        .clickable(onClick = {})
                ) {
                    Column(Modifier.weight(1f)) {
                        Text(item.wasteType.name, fontWeight = FontWeight.Bold)
                        val createdAt =
                            item.updatedAt.format(DateTimeFormatter.ofPattern("d MMM yyyy"))
                        Text("${item.actualWeight ?: item.estimatedWeight} | $createdAt")
                    }
                    Column(horizontalAlignment = Alignment.End) {
                        val pts = if (item.actualPoints == null) "-" else "+${item.actualPoints}"
                        Text(
                            "$pts",
                            fontWeight = FontWeight.SemiBold,
                            color = MaterialTheme.colorScheme.primary,
                            textAlign = TextAlign.End
                        )
                        Text(
                            item.status,
                            Modifier
                                .clip(corner(12.dp))
                                .background(gray1)
                                .padding(12.dp, 8.dp),
                            fontSize = MaterialTheme.typography.labelSmall.fontSize
                        )
                    }
                }
            }
            item {
                NavPage(Modifier.fillMaxWidth(), pagingD, {
                    pagingD = pagingD.copy(page = pagingD.page - 1)
                }, { pagingD = pagingD.copy(page = pagingD.page + 1) })
            }
        }
    }
}