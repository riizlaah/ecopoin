package com.example.ecopoin

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.SecondaryTabRow
import androidx.compose.material3.Tab
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.Gray1
import com.example.ecopoin.ui.theme.Green1
import com.example.ecopoin.ui.theme.Green2
import com.example.ecopoin.ui.theme.Orange1
import com.example.ecopoin.ui.theme.Silver1

@Composable
fun RankingScreen(modifier: Modifier) {
    val ranges = listOf("All Time", "This Month")
    var selectedRange by remember { mutableStateOf(ranges[0]) }
    var paging by remember { mutableStateOf(Paging(1,1)) }
    val topThree = remember { mutableStateListOf<UserRank>() }
    val rankings = remember { mutableStateListOf<UserRank>() }


    suspend fun fetchTopRanks() {
        val (_, r) = HttpClient.getRanks(1, 3, selectedRange)
        if(r.isNotEmpty()) {
            topThree.clear()
            topThree.addAll(listOf(r[1], r[0], r[2]))
        }
    }

    suspend fun fetchRanks() {
        val (p, r) = HttpClient.getRanks(paging.page, 10, selectedRange)
        if(p != null) {
            paging = p
            rankings.clear()
            rankings.addAll(r)

        }
    }

    LaunchedEffect(Unit) {
        fetchTopRanks()
        fetchRanks()
    }

    LaunchedEffect(selectedRange) {
        fetchTopRanks()
        fetchRanks()
    }

    LaunchedEffect(paging) {
        fetchRanks()
    }

    LazyColumn(modifier) {
        stickyHeader {
            Column(Modifier
                .fillMaxWidth()
                .background(Gray1)
                .padding(bottom = 2.dp)
                .background(Color.White)
                .padding(12.dp)) {
                Text(
                    "History",
                    fontWeight = FontWeight.Bold,
                    fontSize = MaterialTheme.typography.headlineLarge.fontSize
                )
                Spacer(Modifier.height(12.dp))
                LazyRow(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                    items(ranges) {name ->
                        val selected = selectedRange == name
                        Button({selectedRange = name}, shape = corner(50), colors = ButtonDefaults.buttonColors(
                            containerColor = if(selected) Green1 else Gray1,
                            contentColor = Color.Black
                        )) {
                            Text(name, fontWeight = if(selected) FontWeight.Bold else FontWeight.Normal)
                        }
                    }
                }
            }
        }
        item {
            Row(Modifier.fillMaxWidth().padding(bottom = 12.dp).background(Green1).padding(horizontal = 12.dp).padding(top = 12.dp), verticalAlignment = Alignment.Bottom, horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                topThree.forEach { item ->
                    Column(Modifier.weight(1f), horizontalAlignment = Alignment.CenterHorizontally) {
                        val color = if(item.rank == 1) Orange1 else if(item.rank == 2) Silver1 else Green2
                        Box(Modifier.padding(horizontal = 20.dp).size(42.dp).clip(CircleShape).card(CircleShape, color, 0.dp), contentAlignment = Alignment.Center) {
                            Text(item.fullName.first().uppercase(), color = Color.White, fontWeight = FontWeight.Bold)
                        }
                        Spacer(Modifier.height(8.dp))
                        Text(item.fullName, fontWeight = FontWeight.Bold, color = Color.White)
                        Text("${thousandFmt(item.points)} points", color = Gray1, fontSize = MaterialTheme.typography.labelMedium.fontSize)
                        Spacer(Modifier.height(8.dp))
                        Box(Modifier.fillMaxWidth().height(128.dp - (32.dp * (item.rank - 1))).clip(
                            RoundedCornerShape(topStart = 12.dp, topEnd = 12.dp)
                        ).background(color), contentAlignment = Alignment.Center) {
                            Text(item.rank.toString(), fontWeight = FontWeight.Bold, color = Color.White)
                        }
                    }
                }
            }
        }
        items(rankings) { item ->
            if(item.rank < 4) return@items
            Row(Modifier.fillMaxWidth().padding(horizontal = 12.dp).padding(bottom = 8.dp).cardBordered(), verticalAlignment = Alignment.CenterVertically) {
                Text("#${item.rank}", color = Color.DarkGray, fontWeight = FontWeight.SemiBold)
                Spacer(Modifier.width(8.dp))
                Box(Modifier.size(42.dp).clip(CircleShape).card(CircleShape, Green2, 0.dp), contentAlignment = Alignment.Center) {
                    Text(item.fullName.first().uppercase(), color = Color.White, fontWeight = FontWeight.Bold)
                }
                Column(Modifier.weight(1f), horizontalAlignment = Alignment.CenterHorizontally) {
                    Text(item.fullName, fontWeight = FontWeight.Bold, fontSize = MaterialTheme.typography.labelMedium.fontSize)
                    Text("${thousandFmt(item.points)} points", fontSize = MaterialTheme.typography.labelMedium.fontSize)
                }
                val tier = getTier(item.balance)
                Text(tier.name, Modifier.card(corner(50), Gray1, 6.dp), fontSize = MaterialTheme.typography.labelSmall.fontSize)
            }
        }
        item {
            Pagination(Modifier.fillMaxWidth().padding(12.dp), paging, {paging = paging.copy(page = paging.page - 1)}, {paging = paging.copy(page = paging.page + 1)})
        }
        item {
            if(HttpClient.myRank == null) return@item
            val item = HttpClient.myRank!!
            Row(Modifier.fillMaxWidth().padding(horizontal = 12.dp).padding(bottom = 8.dp).card(Color(0xfff0fdf4)), verticalAlignment = Alignment.CenterVertically) {
                Text("#${item.rank}", color = Color.DarkGray, fontWeight = FontWeight.SemiBold)
                Spacer(Modifier.width(8.dp))
                Box(Modifier.size(42.dp).clip(CircleShape).card(CircleShape, Green2, 0.dp), contentAlignment = Alignment.Center) {
                    Text(item.fullName.first().uppercase(), color = Color.White, fontWeight = FontWeight.Bold)
                }
                Column(Modifier.weight(1f), horizontalAlignment = Alignment.CenterHorizontally) {
                    Text(item.fullName, fontWeight = FontWeight.Bold, fontSize = MaterialTheme.typography.labelMedium.fontSize)
                    Text("${thousandFmt(item.points)} points", fontSize = MaterialTheme.typography.labelMedium.fontSize)
                }
                val tier = getTier(item.balance)
                Text(tier.name, Modifier.card(corner(50), Gray1, 6.dp), fontSize = MaterialTheme.typography.labelSmall.fontSize)
            }
        }
    }
}