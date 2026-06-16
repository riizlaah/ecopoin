package nr.dev.ecopoin

import android.content.Intent
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.items
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
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
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
import nr.dev.ecopoin.ui.theme.yellow
import java.text.DecimalFormat

@Composable
fun HomeScreen(modifier: Modifier, onRank: () -> Unit, onVoucher: () -> Unit, onSubmitTrash: () -> Unit) {
    Column(modifier) {
        val ctx = LocalContext.current
        val formatter = DecimalFormat("#,###")
        val vouchers = remember { mutableStateListOf<Voucher>() }

        LaunchedEffect(Unit) {
            HttpClient.me()
            HttpClient.myRank()
            val (_, data) = HttpClient.getVouchers(1, 4)
            vouchers.clear()
            vouchers.addAll(data)
        }

        if (HttpClient.profile == null) return@Column
        val profile = HttpClient.profile!!

        Row(
            Modifier
                .fillMaxWidth()
                .background(Color.White)
                .padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Column(Modifier.weight(1f)) {
                Text("Welcome", color = Color.Gray)
                Text(
                    profile.fullName,
                    fontSize = MaterialTheme.typography.headlineLarge.fontSize,
                    fontWeight = FontWeight.SemiBold
                )
            }
            IconButton(
                {
                    HttpClient.token = ""
                    HttpClient.saveToken()
                    HttpClient.profile = null
                    val int = Intent(ctx, MainActivity::class.java).apply {
                        flags = Intent.FLAG_ACTIVITY_CLEAR_TASK or Intent.FLAG_ACTIVITY_NEW_TASK
                    }
                    ctx.startActivity(int)
                },
                shape = CircleShape,
                colors = IconButtonDefaults.iconButtonColors(
                    containerColor = Color.LightGray,
                    contentColor = Color.Black
                ),
                modifier = Modifier.size(42.dp)
            ) {
                Icon(painterResource(R.drawable.logout), "Log out", Modifier.rotate(180f))
            }
            Spacer(Modifier.width(12.dp))
            Box(Modifier
                .size(42.dp)
                .clip(CircleShape)
                .background(
                    MaterialTheme.colorScheme.primary
                ), contentAlignment = Alignment.Center) {
                Text(
                    profile.fullName.first().uppercase(),
                    fontWeight = FontWeight.Black,
                    textAlign = TextAlign.Center,
                    color = Color.White,
                )
            }
        }
        Column(Modifier.fillMaxWidth().padding(24.dp)) {
            Column(
                Modifier
                    .fillMaxWidth()
                    .clip(corner(12.dp))
                    .background(MaterialTheme.colorScheme.primary)
                    .padding(18.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                val tier = getTier(profile.balance)
                val nextTier = getNextTier(profile.balance)
                Row(
                    Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {

                    Text("Points Balance", color = Color.LightGray)
                    Text(
                        tier.name,
                        Modifier
                            .clip(corner(50))
                            .background(yellow)
                            .padding(12.dp),
                        fontWeight = FontWeight.Medium
                    )
                }
                Spacer(Modifier.height(36.dp))
                Text(
                    formatter.format(profile.balance).replace(',', '.'),
                    fontWeight = FontWeight.Black,
                    fontSize = MaterialTheme.typography.displayMedium.fontSize,
                    color = Color.White
                )
                Spacer(Modifier.height(12.dp))
                Text("points", color = MaterialTheme.colorScheme.secondary, fontWeight = FontWeight.Medium)
                Spacer(Modifier.height(20.dp))
                Box(
                    Modifier
                        .fillMaxWidth()
                        .clip(corner(50))
                        .background(MaterialTheme.colorScheme.secondary)
                ) {
                    Box(
                        Modifier
                            .fillMaxWidth(profile.balance.toFloat() / nextTier.minPoints)
                            .height(12.dp)
                            .clip(corner(50))
                            .background(yellow)
                    )
                }
                Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                    Text(
                        "${tier.name} · ${tier.minPoints}",
                        color = MaterialTheme.colorScheme.secondary,
                        fontSize = MaterialTheme.typography.labelSmall.fontSize,
                        fontWeight = FontWeight.Medium
                    )
                    Text(
                        "${nextTier.name} · ${nextTier.minPoints}",
                        color = MaterialTheme.colorScheme.secondary,
                        fontSize = MaterialTheme.typography.labelSmall.fontSize,
                        fontWeight = FontWeight.Medium
                    )
                }
            }
            Spacer(Modifier.height(24.dp))
            Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                Column(
                    Modifier
                        .weight(1f)
                        .clip(corner(12.dp))
                        .background(Color.White)
                        .border(1.dp, Color.LightGray, corner(12.dp))
                        .padding(12.dp)
                ) {
                    Row(Modifier.fillMaxWidth(), verticalAlignment = Alignment.CenterVertically) {
                        Icon(
                            painterResource(R.drawable.trash),
                            "Trash",
                            tint = MaterialTheme.colorScheme.primary
                        )
                        Spacer(Modifier.width(12.dp))
                        Text("Stored")
                    }
                    Text(
                        "${formatter.format(profile.totalSubmittedWeights).replace(',', '.')} kg",
                        color = MaterialTheme.colorScheme.primary,
                        fontSize = MaterialTheme.typography.headlineSmall.fontSize,
                        fontWeight = FontWeight.Bold
                    )
                }
                Column(
                    Modifier
                        .weight(1f)
                        .clip(corner(12.dp))
                        .background(Color.White)
                        .border(1.dp, Color.LightGray, corner(12.dp))
                        .padding(12.dp)
                ) {
                    Row(Modifier.fillMaxWidth(), verticalAlignment = Alignment.CenterVertically) {
                        Icon(
                            painterResource(R.drawable.wind),
                            "Wind",
                            tint = MaterialTheme.colorScheme.primary
                        )
                        Spacer(Modifier.width(12.dp))
                        Text("CO₂ Prevented")
                    }
                    Text(
                        "${formatter.format(profile.environmentalImpact).replace(',', '.')} kg",
                        color = MaterialTheme.colorScheme.primary,
                        fontSize = MaterialTheme.typography.headlineSmall.fontSize,
                        fontWeight = FontWeight.Bold
                    )
                }
            }
            Row(
                Modifier
                    .padding(top = 12.dp)
                    .fillMaxWidth()
                    .clip(corner(12.dp))
                    .background(Color.White)
                    .border(1.dp, Color.Gray, corner(12.dp))
                    .padding(12.dp)
                    .clickable(onClick = onRank),
                verticalAlignment = Alignment.CenterVertically
            ) {
                Icon(
                    painterResource(R.drawable.trophy),
                    "Trophy",
                    tint = yellow,
                    modifier = Modifier
                        .clip(corner(12.dp))
                        .background(Color(0xfffff7ed))
                        .padding(12.dp)
                )
                Spacer(Modifier.width(12.dp))
                Column(Modifier.weight(1f)) {
                    if (HttpClient.myRank == null) return@Column
                    val rank = HttpClient.myRank!!
                    Text(
                        "Your rank",
                        color = Color.Gray,
                        fontSize = MaterialTheme.typography.labelMedium.fontSize
                    )
                    val str = buildAnnotatedString {
                        withStyle(
                            SpanStyle(
                                fontWeight = FontWeight.Black,
                                fontSize = MaterialTheme.typography.headlineLarge.fontSize
                            )
                        ) {
                            append("#${rank.rank}")
                        }
                        withStyle(
                            SpanStyle(
                                color = Color.Gray,
                                fontSize = MaterialTheme.typography.labelMedium.fontSize
                            )
                        ) {
                            append(" from ${rank.fromTotal} residents")
                        }
                    }
                    Text(str)
                }
                Icon(painterResource(R.drawable.arr_forward0), "More", tint = Color.Gray)

            }
            Button(onSubmitTrash, Modifier.padding(vertical = 12.dp).fillMaxWidth(), shape = corner(12.dp)) {
                Icon(painterResource(R.drawable.cloud_upload), "Upload")
                Text("Submit Trash Now")
            }
            Row(
                Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text("Exchange Points")
                TextButton(onVoucher) { Text("More...") }
            }
            LazyVerticalGrid(
                GridCells.Fixed(2), Modifier
                    .fillMaxWidth()
                    .heightIn(200.dp, 500.dp),
                verticalArrangement = Arrangement.spacedBy(12.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                items(vouchers) { item ->
                    Column(
                        Modifier
                            .fillMaxWidth()
                            .clip(corner(12.dp))
                            .background(Color.White)
                            .border(1.dp, Color.LightGray, corner(12.dp))
                            .padding(12.dp)
                    ) {
                        Text(item.name, fontWeight = FontWeight.Medium)
                        Spacer(Modifier.height(20.dp))
                        Text("${formatter.format(item.pointCost).replace(',','.')} points", color = yellow, fontWeight = FontWeight.Medium)
                    }
                }
            }
        }
    }
}