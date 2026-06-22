package com.example.ecopoin

import android.content.Intent
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyGridScope
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material3.Button
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.IconButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.SpanStyle
import androidx.compose.ui.text.buildAnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.text.withStyle
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.Gray1
import com.example.ecopoin.ui.theme.Green1
import com.example.ecopoin.ui.theme.Orange1
import java.text.DecimalFormat
import java.time.format.DateTimeFormatter

@Composable
fun HomeScreen(
    modifier: Modifier,
    onRank: () -> Unit,
    onSubmit: () -> Unit,
    onVouchers: () -> Unit,
    onDeposits: () -> Unit
) {
    val ctx = LocalContext.current

    Column(modifier) {

        if (HttpClient.profile == null) return@Column
        val profile = HttpClient.profile!!
        Row(
            Modifier
                .padding(bottom = 24.dp)
                .fillMaxWidth()
                .background(Color.White)
                .padding(12.dp), verticalAlignment = Alignment.CenterVertically
        ) {
            Column(Modifier.weight(1f)) {
                Text("Welcome", color = Color.Gray)
                Text(
                    profile.fullName,
                    fontWeight = FontWeight.Bold,
                    fontSize = MaterialTheme.typography.headlineLarge.fontSize
                )
            }
            IconButton(
                {
                    val int = Intent(ctx, MainActivity::class.java)
                    HttpClient.token = ""
                    HttpClient.saveToken()
                    HttpClient.profile = null
                    HttpClient.myRank = null
                    ctx.startActivity(int)
                },
                Modifier.size(40.dp),
                shape = CircleShape,
                colors = IconButtonDefaults.iconButtonColors(containerColor = Color.LightGray)
            ) {
                Icon(painterResource(R.drawable.logout), "Log out", Modifier.rotate(180f))
            }
            Spacer(Modifier.width(12.dp))
            Box(
                Modifier
                    .size(40.dp)
                    .clip(CircleShape)
                    .background(MaterialTheme.colorScheme.primary),
                contentAlignment = Alignment.Center
            ) {
                Text(profile.fullName.first().uppercase(), fontWeight = FontWeight.Bold, color = Color.White)
            }
        }
        val vouchers = remember { mutableStateListOf<Voucher>() }
        val deposits = remember { mutableStateListOf<Deposit>() }

        LaunchedEffect(Unit) {
            if(HttpClient.isOfficer) {
                val (_, d) = HttpClient.getDeposits(1, 5, "Pending")
                deposits.addAll(d)
            } else {
                val (p, v) = HttpClient.getVouchers(1, 4)
                if(p != null) {
                    vouchers.clear()
                    vouchers.addAll(v)
                }
            }
        }

        LazyColumn(Modifier.fillMaxWidth().padding(18.dp)) {
            item {
                if(HttpClient.isOfficer) {
                    Row(Modifier.fillMaxWidth().padding(bottom = 12.dp), horizontalArrangement = Arrangement.SpaceBetween, verticalAlignment = Alignment.CenterVertically) {
                        Text("Pending Deposits")
                        TextButton(onDeposits) {
                            Text("More...")
                        }
                    }
                    return@item
                }
                Column(Modifier
                    .fillMaxWidth()
                    .card(MaterialTheme.colorScheme.primary, 16.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                    val tier = getTier(profile.balance)
                    val nextTier = getNextTier(profile.balance)

                    Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                        Text("Balance", color = Gray1)
                        Text(
                            tier.name.uppercase(),
                            Modifier.card(corner(50), Orange1, 4.dp),
                            fontSize = MaterialTheme.typography.labelSmall.fontSize
                        )
                    }
                    Spacer(Modifier.height(24.dp))
                    Text(
                        thousandFmt(profile.balance),
                        fontWeight = FontWeight.Bold,
                        fontSize = MaterialTheme.typography.displayMedium.fontSize,
                        color = Color.White,
                        textAlign = TextAlign.Center
                    )
                    Spacer(Modifier.height(4.dp))
                    Text("points", color = Gray1)
                    Spacer(Modifier.height(24.dp))
                    Box(
                        Modifier
                            .fillMaxWidth()
                            .card(corner(50), MaterialTheme.colorScheme.secondary, 0.dp)
                    ) {
                        Box(
                            Modifier
                                .fillMaxWidth(profile.balance.toFloat() / nextTier.minPoint)
                                .height(12.dp)
                                .background(Orange1)
                        )
                    }
                    Spacer(Modifier.height(12.dp))
                    Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                        Text("${tier.name} - ${profile.balance}", color = Gray1)
                        Text("${nextTier.name} - ${nextTier.minPoint}", color = Gray1)
                    }
                }
                Row(Modifier
                    .fillMaxWidth()
                    .padding(vertical = 12.dp)) {
                    Column(Modifier
                        .weight(1f)
                        .cardBordered()) {
                        Row(verticalAlignment = Alignment.CenterVertically) {
                            Icon(painterResource(R.drawable.trash), "Trash", tint = MaterialTheme.colorScheme.primary)
                            Text("Submitted", color = Color.Gray)
                        }
                        Text(
                            "${profile.submittedWeight} kg",
                            fontWeight = FontWeight.Bold,
                            fontSize = MaterialTheme.typography.headlineSmall.fontSize,
                            color = MaterialTheme.colorScheme.primary
                        )
                    }
                    Spacer(Modifier.width(12.dp))
                    Column(Modifier
                        .weight(1f)
                        .cardBordered()) {
                        Row(verticalAlignment = Alignment.CenterVertically) {
                            Icon(painterResource(R.drawable.wind), "Wind", tint = MaterialTheme.colorScheme.primary)
                            Text("Prevented CO₂", color = Color.Gray)
                        }
                        Text(
                            "${profile.envImpact} kg",
                            fontWeight = FontWeight.Bold,
                            fontSize = MaterialTheme.typography.headlineSmall.fontSize,
                            color = MaterialTheme.colorScheme.primary
                        )
                    }
                }
                if (HttpClient.myRank == null) return@item
                val rank = HttpClient.myRank!!
                Row(
                    Modifier
                        .padding(bottom = 12.dp)
                        .fillMaxWidth()
                        .cardBordered()
                        .clickable(onClick = onRank), verticalAlignment = Alignment.CenterVertically
                ) {
                    Icon(
                        painterResource(R.drawable.trophy),
                        "Trophy",
                        Modifier.card(Color(0xfffff7ed), 12.dp),
                        tint = Orange1
                    )
                    Column(Modifier.padding(horizontal = 12.dp).weight(1f)) {
                        Text("Your ranking")
                        val str = buildAnnotatedString {
                            withStyle(
                                SpanStyle(
                                    fontWeight = FontWeight.Bold,
                                    fontSize = MaterialTheme.typography.headlineSmall.fontSize
                                )
                            ) {
                                append("#${rank.rank}")
                            }
                            withStyle(
                                SpanStyle(
                                    color = Color.Gray,
                                    fontSize = MaterialTheme.typography.bodySmall.fontSize
                                )
                            ) {
                                append(" from total ${rank.fromTotal} resident")
                            }
                        }
                        Text(str)
                    }
                    Icon(painterResource(R.drawable.arr_forward_ios), "More")
                }
                Button(onSubmit, Modifier
                    .fillMaxWidth()
                    .padding(bottom = 12.dp), shape = corner()) {
                    Icon(painterResource(R.drawable.cloud_upload), "Upload")
                    Spacer(Modifier.width(12.dp))
                    Text("Submit Trash Now")
                }
                Row(
                    Modifier
                        .fillMaxWidth()
                        .padding(bottom = 12.dp),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text("Exchange Points")
                    TextButton(onVouchers) {
                        Text("More...")
                    }
                }
                LazyVerticalGrid(GridCells.Fixed(2), Modifier
                    .fillMaxWidth()
                    .heightIn(128.dp, 512.dp), verticalArrangement = Arrangement.spacedBy(12.dp), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                    items(vouchers) { item ->
                        Column(Modifier.fillMaxWidth().cardBordered()) {
                            Text(item.name, Modifier.fillMaxWidth(), textAlign = TextAlign.Center, fontWeight = FontWeight.Medium)
                            Spacer(Modifier.height(12.dp))
                            Text("${thousandFmt(item.pointCost)} points", Modifier.fillMaxWidth(), textAlign = TextAlign.Center, color = Orange1, fontWeight = FontWeight.Bold)
                        }
                    }
                }
            }
            if(HttpClient.isOfficer) {
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
            }
        }
    }
}