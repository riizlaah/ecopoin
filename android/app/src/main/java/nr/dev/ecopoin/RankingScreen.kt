package nr.dev.ecopoin

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import nr.dev.ecopoin.ui.theme.gray1
import nr.dev.ecopoin.ui.theme.yellow
import java.text.DecimalFormat

@Composable
fun RankingScreen(modifier: Modifier) {
    val filters = listOf("This Month", "All Time")
    var selectedFilter by remember { mutableStateOf(filters[0]) }
    val top3 = remember { mutableStateListOf<UserRank>() }
    val leaderboard = remember { mutableStateListOf<UserRank>() }
    var paging by remember { mutableStateOf(Pagination(1, 1)) }
    val formatter = DecimalFormat("#,###")

    LaunchedEffect(Unit) {
        HttpClient.me()
        HttpClient.myRank()
        val (p, v) = HttpClient.getLeaderboard(
            1,
            range = selectedFilter.replace(" ", "").lowercase()
        )
        if (p != null) {
            leaderboard.clear()
            leaderboard.addAll(v.filterIndexed { i, rank -> rank.rank > 3 })
        }

        val (p1, v1) = HttpClient.getLeaderboard(
            1,
            3,
            selectedFilter.replace(" ", "").lowercase()
        )
        if (p1 != null) {
            top3.clear()
            top3.add(v1[1])
            top3.addAll(v1.filter { it.rank != 2 })
        }
    }

    LazyColumn(modifier, verticalArrangement = Arrangement.spacedBy(12.dp)) {
        stickyHeader {
            Column(
                Modifier
                    .fillMaxWidth()
                    .background(Color.White)
                    .padding(12.dp)
            ) {
                Text(
                    "Leaderboard",
                    fontWeight = FontWeight.Bold,
                    fontSize = MaterialTheme.typography.displaySmall.fontSize,
                    textAlign = TextAlign.Center,
                    modifier = Modifier.fillMaxWidth()
                )
                Spacer(Modifier.height(12.dp))
                LazyRow(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                    items(filters) { name ->
                        val selected = name == selectedFilter
                        Button(
                            { selectedFilter = name },
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
            }
        }
        item(top3) {
            if (top3.isEmpty()) return@item
            Row(
                Modifier
                    .padding(bottom = 12.dp)
                    .fillMaxWidth()
                    .background(MaterialTheme.colorScheme.primary)
                    .padding(top = 12.dp, start = 12.dp, end = 12.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp),
                verticalAlignment = Alignment.Bottom
            ) {
                top3.forEachIndexed { i, item ->
                    val color = when(i) {
                        0 -> yellow
                        1 -> Color(0xff94a3b8)
                        else -> Color(0xffd97706)
                    }

                    Column(Modifier.weight(1f), horizontalAlignment = Alignment.CenterHorizontally, verticalArrangement = Arrangement.Bottom) {
                        Box(
                            Modifier
                                .padding(vertical = 8.dp)
                                .size(40.dp)
                                .clip(CircleShape)
                                .background(color)
                                .border(2.dp, Color.White, CircleShape),
                            contentAlignment = Alignment.Center
                        ) {
                            Text(item.fullName.first().uppercase(), fontWeight = FontWeight.Black, color = Color.White)
                        }
                        Text(item.fullName, fontWeight = FontWeight.SemiBold, color = Color.White)
                        Spacer(Modifier.height(4.dp))
                        Text(
                            "${formatter.format(item.totalPoints).replace(',', '.')} points",
                            color = Color.LightGray
                        )
                        Spacer(Modifier.height(12.dp))
                        Box(
                            Modifier
                                .fillMaxWidth()
                                .height(100.dp - (25.dp * (item.rank - 1)))
                                .clip(RoundedCornerShape(topStart = 12.dp, topEnd = 12.dp))
                                .background(color), contentAlignment = Alignment.Center
                        ) {
                            Text(
                                "${item.rank}",
                                fontWeight = FontWeight.Black,
                                fontSize = MaterialTheme.typography.headlineLarge.fontSize,
                                color = Color.White
                            )
                        }
                    }
                }
            }
        }
        items(leaderboard) { item ->
            Row(
                Modifier
                    .fillMaxWidth()
                    .padding(start = 8.dp, end = 8.dp, bottom = 12.dp)
                    .clip(corner(12.dp))
                    .background(Color.White)
                    .border(1.dp, Color.Gray, corner(12.dp))
                    .padding(12.dp),
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    "#${item.rank}",
                    Modifier.padding(horizontal = 12.dp),
                    fontWeight = FontWeight.Bold,
                    color = Color.Gray
                )
                Box(
                    Modifier
                        .padding(vertical = 8.dp)
                        .size(40.dp)
                        .clip(CircleShape)
                        .background(MaterialTheme.colorScheme.secondary),
                    contentAlignment = Alignment.Center
                ) {
                    Text(item.fullName.first().uppercase(), fontWeight = FontWeight.Black, color = Color.White)
                }
                Column(Modifier.weight(1f), horizontalAlignment = Alignment.CenterHorizontally) {
                    Text(item.fullName, fontWeight = FontWeight.Bold)
                    Spacer(Modifier.height(4.dp))
                    Text(
                        "${formatter.format(item.totalPoints).replace(',', '.')} points",
                        color = Color.Gray
                    )
                }
                val tier = getTier(item.totalPoints)
                Text(
                    tier.name.uppercase(),
                    Modifier
                        .clip(corner(12.dp))
                        .background(gray1)
                        .padding(12.dp, 8.dp),
                    fontWeight = FontWeight.Medium,
                    fontSize = MaterialTheme.typography.labelSmall.fontSize
                )
            }
        }
        item {
            Text(
                ". . .",
                color = Color.Gray,
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(vertical = 12.dp),
                textAlign = TextAlign.Center
            )
        }
        item {
            if (HttpClient.myRank == null) return@item
            val item = HttpClient.myRank!!

            Row(
                Modifier
                    .fillMaxWidth()
                    .padding(start = 8.dp, end = 8.dp, bottom = 12.dp)
                    .clip(corner(12.dp))
                    .background(Color(0xfff0fdf4))
                    .border(2.dp, Color(0xffbbf7d0), corner(12.dp))
                    .padding(12.dp),
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    "#${item.rank}",
                    Modifier.padding(horizontal = 12.dp),
                    fontWeight = FontWeight.Bold,
                    color = Color.Gray
                )
                Box(
                    Modifier
                        .padding(vertical = 8.dp)
                        .size(40.dp)
                        .clip(CircleShape)
                        .background(MaterialTheme.colorScheme.secondary),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        HttpClient.profile!!.fullName.first().uppercase(),
                        fontWeight = FontWeight.Black,
                        color = Color.White
                    )
                }
                Column(Modifier.weight(1f), horizontalAlignment = Alignment.CenterHorizontally) {
                    Text(HttpClient.profile!!.fullName, fontWeight = FontWeight.Bold)
                    Spacer(Modifier.height(4.dp))
                    Text(
                        "${formatter.format(item.totalPoints).replace(',', '.')} points",
                        color = Color.Gray
                    )
                }
                val tier = getTier(item.totalPoints)
                Text(
                    tier.name.uppercase(),
                    Modifier
                        .clip(corner(12.dp))
                        .background(gray1)
                        .padding(12.dp, 8.dp),
                    fontWeight = FontWeight.Medium,
                    fontSize = MaterialTheme.typography.labelSmall.fontSize
                )
            }
        }
    }
}