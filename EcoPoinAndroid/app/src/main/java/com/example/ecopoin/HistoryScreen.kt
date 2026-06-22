package com.example.ecopoin

import android.content.Intent
import android.util.Log
import androidx.compose.foundation.background
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
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.SecondaryTabRow
import androidx.compose.material3.Tab
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.Gray1
import com.example.ecopoin.ui.theme.Green1
import java.time.format.DateTimeFormatter

@Composable
fun HistoryScreen(modifier: Modifier) {
    val tabs = listOf("Deposits", "Vouchers")
    var currentTab by remember { mutableIntStateOf(0) }
    val dpFilters = listOf("All", "Pending", "Verified", "Rejected")
    var selectedDpFilter  by remember { mutableStateOf(dpFilters[0]) }
    var dpPaging by remember { mutableStateOf(Paging(1,1)) }
    val vcFilters = listOf("All", "Unused", "Used")
    var selectedVcFilter  by remember { mutableStateOf(vcFilters[0]) }
    var vcPaging by remember { mutableStateOf(Paging(1,1)) }
    val deposits = remember { mutableStateListOf<Deposit>() }
    val vouchers = remember { mutableStateListOf<RedeemedVoucher>() }
    val ctx = LocalContext.current


    suspend fun fetchDeposits() {
        val (p, d) = HttpClient.getDeposits(dpPaging.page, 10, selectedDpFilter)
        if(p != null) {
            dpPaging = p
            deposits.clear()
            deposits.addAll(d)
        }
    }

    suspend fun fetchVouchers() {
        if(!HttpClient.isOfficer) {
            val (p, d) = HttpClient.getVoucherHistory(vcPaging.page, selectedVcFilter)
            if(p != null) {
                vcPaging = p
                vouchers.clear()
                vouchers.addAll(d)
            }
        }
    }

    LaunchedEffect(Unit) {
        fetchDeposits()
        fetchVouchers()
    }

    LaunchedEffect(currentTab) {
        if(currentTab == 0) fetchDeposits()
        else fetchVouchers()
    }

    LaunchedEffect(selectedDpFilter) {
        fetchDeposits()
    }

    LaunchedEffect(selectedVcFilter) {
        fetchVouchers()
    }

    LazyColumn(modifier) {
        stickyHeader {
            Column(Modifier
                .fillMaxWidth()
                .background(Gray1)
                .padding(bottom = 2.dp)
                .background(Color.White)
                .padding(12.dp)) {
                Text("History", fontWeight = FontWeight.Bold, fontSize = MaterialTheme.typography.headlineLarge.fontSize)
                Spacer(Modifier.height(12.dp))
                if(!HttpClient.isOfficer) {
                    SecondaryTabRow(currentTab, containerColor = Color.White) {
                        tabs.forEachIndexed { index, tab ->
                            val selected = index == currentTab
                            Tab(selected, {currentTab = index}, Modifier.padding(8.dp)) {
                                Text(tab, fontWeight = if(selected) FontWeight.Bold else FontWeight.Normal)
                            }
                        }
                    }
                }
                Spacer(Modifier.height(12.dp))
                if(currentTab == 0) {
                    LazyRow(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                        items(dpFilters) {name ->
                            val selected = selectedDpFilter == name
                            Button({selectedDpFilter = name}, shape = corner(50), colors = ButtonDefaults.buttonColors(
                                containerColor = if(selected) Green1 else Gray1,
                                contentColor = if(selected) Color.White else Color.Black
                            )) {
                                Text(name, fontWeight = if(selected) FontWeight.Bold else FontWeight.Normal)
                            }
                        }
                    }
                } else {
                    LazyRow(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                        items(vcFilters) {name ->
                            val selected = selectedVcFilter == name
                            Button({selectedVcFilter = name}, shape = corner(50), colors = ButtonDefaults.buttonColors(
                                containerColor = if(selected) Green1 else Gray1,
                                contentColor = if(selected) Color.White else Color.Black
                            )) {
                                Text(name, fontWeight = if(selected) FontWeight.Bold else FontWeight.Normal)
                            }
                        }
                    }
                }
            }
        }
        item {
            Spacer(Modifier.height(12.dp))
        }

        if(currentTab == 0) {
            items(deposits) { item ->
                Row(Modifier
                    .fillMaxWidth()
                    .padding(start = 12.dp, end = 12.dp, bottom = 12.dp)
                    .cardBordered().clickable(onClick = {
                    val int = Intent(ctx, DepositDetailActivity::class.java).apply {
                        putExtra("id", item.id)
                    }
                    ctx.startActivity(int)
                })) {
                    Column(Modifier.weight(1f)) {
                        Text(item.wasteType.name, fontWeight = FontWeight.Bold)
                        val fmt = DateTimeFormatter.ofPattern("dd MMM")
                        Text("${item.actWeight ?: item.estWeight} kg | ${item.updatedAt.format(fmt)}")
                    }
                    Column(horizontalAlignment = Alignment.End) {
                        val pts = if(item.actPoints == null) "-" else "+${item.actPoints}"
                        Text(pts, color = Green1, fontWeight = FontWeight.Medium, textAlign = TextAlign.End)
                        Text(item.status, Modifier.card(corner(50), Gray1, 12.dp))
                    }
                }
            }
            item {
                Pagination(Modifier.fillMaxWidth(), dpPaging, {dpPaging = dpPaging.copy(page = dpPaging.page - 1)}, {dpPaging = dpPaging.copy(page = dpPaging.page + 1)})
            }
        } else {
            items(vouchers) { item ->
                Column(Modifier
                    .fillMaxWidth()
                    .padding(start = 12.dp, end = 12.dp, bottom = 12.dp)
                    .cardBordered()) {
                    Row(Modifier.fillMaxWidth()) {
                        Column(Modifier.weight(1f)) {
                            Text(item.voucher.name, fontWeight = FontWeight.Bold)
                            val fmt = DateTimeFormatter.ofPattern("dd MMM")
                            Text("${item.amount} points | ${item.updatedAt.format(fmt)}")
                        }
                        Text(if(item.isUsed) "Used" else "Unused" , Modifier.card(corner(50), Gray1, 12.dp))
                    }
                    HorizontalDivider(Modifier.fillMaxWidth())
                    Text("Code : ${item.code}", fontFamily = FontFamily.Monospace)
                }
            }
            item {
                Pagination(Modifier.fillMaxWidth(), vcPaging, {vcPaging = vcPaging.copy(page = vcPaging.page - 1)}, {vcPaging = vcPaging.copy(page = vcPaging.page + 1)})
            }
        }
    }
}