package nr.dev.ecopoin

import android.content.Intent
import android.icu.text.DecimalFormat
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
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
import androidx.compose.foundation.shape.RoundedCornerShape
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

@Composable
fun HomeScreen(onRank: () -> Unit, onVouchers: () -> Unit, onSubmitTrash: () -> Unit) {
    val vouchers = remember { mutableStateListOf<Voucher>() }
    LaunchedEffect(Unit) {
        val (paging, data) = HttpClient.getVouchers(1, 4)
        vouchers.addAll(data)
    }
    val ctx = LocalContext.current

    if (HttpClient.profile == null) return
    val profile = HttpClient.profile!!


    Row(
        Modifier
            .fillMaxWidth()
            .background(Color.White)
            .padding(12.dp, 24.dp),
        verticalAlignment = Alignment.CenterVertically
    ) {
        Column(Modifier.weight(1f)) {
            Text("Welcome", color = Color.Gray)
            Spacer(Modifier.height(12.dp))
            Text(
                profile.fullName,
                fontWeight = FontWeight.SemiBold,
                fontSize = MaterialTheme.typography.headlineSmall.fontSize
            )
        }
        IconButton(
            {
                HttpClient.token = ""
                HttpClient.profile = null
                HttpClient.myRank = null
                HttpClient.saveToken()
                val int = Intent(ctx, MainActivity::class.java).apply {
                    flags =
                        Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                }
                ctx.startActivity(int)

            },
            shape = CircleShape,
            colors = IconButtonDefaults.iconButtonColors(containerColor = MaterialTheme.colorScheme.tertiary),
            modifier = Modifier.size(42.dp)
        ) {
            Icon(
                painterResource(R.drawable.logout),
                contentDescription = "Log out",
                modifier = Modifier.rotate(180f)
            )
        }
        Spacer(Modifier.width(8.dp))
        Box(
            Modifier
                .size(42.dp)
                .clip(CircleShape)
                .background(MaterialTheme.colorScheme.primary),
            contentAlignment = Alignment.Center
        ) {
            Text(
                profile.fullName.first().uppercase(),
                fontWeight = FontWeight.Black,
                color = Color.White
            )
        }
    }
    Spacer(Modifier.height(12.dp))
    Column(
        Modifier
            .padding(vertical = 12.dp)
            .fillMaxWidth()
            .clip(RoundedCornerShape(18.dp))
            .background(
                MaterialTheme.colorScheme.secondary
            )
            .padding(12.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        val currentTier = getTier(profile.balance)
        val nextTier = getNextTier(profile.balance)
        Row(
            Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Text("Points", color = Color.LightGray, fontWeight = FontWeight.Medium)
            Text(
                currentTier.name.uppercase(),
                fontSize = MaterialTheme.typography.labelLarge.fontSize,
                modifier = Modifier
                    .clip(
                        RoundedCornerShape(50)
                    )
                    .background(Color(0xfff59e0b))
                    .padding(14.dp, 8.dp)
            )
        }
        Spacer(Modifier.height(24.dp))
        val formatter = DecimalFormat("#,###")
        val res = formatter.format(profile.balance).replace(',', '.')
        Text(
            res,
            fontSize = MaterialTheme.typography.displaySmall.fontSize,
            fontWeight = FontWeight.Bold,
            color = Color.White
        )
        Text(
            "points",
            color = Color.LightGray,
            fontSize = MaterialTheme.typography.labelMedium.fontSize,
            fontWeight = FontWeight.SemiBold
        )
        Box(
            Modifier
                .padding(top = 32.dp, bottom = 8.dp)
                .fillMaxWidth()
                .clip(RoundedCornerShape(40))
                .background(
                    MaterialTheme.colorScheme.primary
                )
        ) {
            Box(
                Modifier
                    .height(12.dp)
                    .fillMaxWidth(profile.balance.toFloat() / nextTier.minPoints)
                    .background(Color(0xfff59e0b))
            )
        }
        Row(
            Modifier.fillMaxSize(),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Text("${currentTier.name} · ${currentTier.minPoints}", color = Color.LightGray)
            Spacer(Modifier.weight(1f))
            Text("${nextTier.name} · ${nextTier.minPoints}", color = Color.LightGray)
        }
    }

    Row(Modifier
        .padding(vertical = 12.dp)
        .fillMaxWidth()) {
        Column(
            Modifier
                .weight(1f)
                .clip(RoundedCornerShape(12.dp))
                .background(Color.White)
                .border(1.dp, Color.LightGray, RoundedCornerShape(12.dp))
                .padding(12.dp)
        ) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Icon(
                    painterResource(R.drawable.trash_2),
                    contentDescription = "Trash",
                    tint = MaterialTheme.colorScheme.primary
                )
                Spacer(Modifier.width(12.dp))
                Text("Stored", fontSize = MaterialTheme.typography.labelSmall.fontSize, color = MaterialTheme.colorScheme.secondary)
            }
            Text(
                "${profile.totalSubmittedWeights} kg",
                fontWeight = FontWeight.SemiBold,
                fontSize = MaterialTheme.typography.titleLarge.fontSize,
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(vertical = 8.dp),
                textAlign = TextAlign.Center
            )
        }
        Spacer(Modifier.width(12.dp))
        Column(
            Modifier
                .weight(1f)
                .clip(RoundedCornerShape(12.dp))
                .background(Color.White)
                .border(1.dp, Color.LightGray, RoundedCornerShape(12.dp))
                .padding(12.dp)
        ) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Icon(
                    painterResource(R.drawable.wind),
                    contentDescription = "Wind",
                    tint = MaterialTheme.colorScheme.primary
                )
                Spacer(Modifier.width(12.dp))
                Text("CO₂ Prevented", fontSize = MaterialTheme.typography.labelSmall.fontSize, color = MaterialTheme.colorScheme.secondary)
            }
            Text(
                "${profile.environmentalImpact} kg",
                fontWeight = FontWeight.SemiBold,
                fontSize = MaterialTheme.typography.titleLarge.fontSize,
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(vertical = 8.dp),
                textAlign = TextAlign.Center
            )
        }
    }

    if (HttpClient.myRank == null) return
    val myRank = HttpClient.myRank!!

    Row(
        Modifier
            .padding(bottom = 12.dp)
            .fillMaxWidth()
            .clip(RoundedCornerShape(12.dp))
            .background(Color.White)
            .border(1.dp, Color.LightGray, RoundedCornerShape(12.dp))
            .padding(12.dp)
            .clickable(onClick = onRank),
        verticalAlignment = Alignment.CenterVertically
    ) {
        Icon(
            painterResource(R.drawable.trophy),
            contentDescription = "Rank",
            tint = Color(0xfff59e0b),
            modifier = Modifier
                .padding(end = 12.dp)
                .clip(RoundedCornerShape(12.dp))
                .background(Color(0xfffff7ed))
                .padding(12.dp)
        )
        Column(Modifier.weight(1f)) {
            Text("Your ranking", color = Color.Gray)
            val sb = buildAnnotatedString {
                withStyle(
                    SpanStyle(
                        fontSize = MaterialTheme.typography.headlineMedium.fontSize,
                        fontWeight = FontWeight.SemiBold
                    )
                ) {
                    append("#${myRank.rank}")
                }
                withStyle(SpanStyle(color = Color.Gray)) {
                    append(" from ${myRank.fromTotal} residents")
                }
            }
            Text(sb)
        }
        Icon(
            painterResource(R.drawable.arrow_forward_ios),
            contentDescription = "More",
            tint = Color.LightGray
        )
    }

    Button(
        onSubmitTrash,
        Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(12.dp),
        contentPadding = PaddingValues(18.dp)
    ) {
        Icon(
            painterResource(R.drawable.cloud_upload),
            contentDescription = "Upload"
        )
        Spacer(Modifier.width(12.dp))
        Text("Submit Trash Right Now", fontWeight = FontWeight.Bold)
    }

    Row(
        Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.SpaceBetween,
        verticalAlignment = Alignment.CenterVertically
    ) {
        Text("Exchange Points", fontWeight = FontWeight.Medium)
        TextButton(onVouchers) {
            Text("See more")
        }
    }

    LazyVerticalGrid(
        GridCells.Fixed(2),
        modifier = Modifier
            .fillMaxWidth()
            .heightIn(256.dp, 512.dp),
        userScrollEnabled = false,
        horizontalArrangement = Arrangement.spacedBy(12.dp),
        verticalArrangement = Arrangement.spacedBy(12.dp)
    ) {
        val formatter = DecimalFormat("#,###")
        items(vouchers) { item ->
            Column(
                Modifier
                    .fillMaxWidth(1f)
                    .clip(RoundedCornerShape(18.dp))
                    .background(Color.White)
                    .border(1.dp, Color.LightGray, RoundedCornerShape(18.dp))
                    .padding(24.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Text(item.name, fontWeight = FontWeight.Bold, textAlign = TextAlign.Center)
                val cost = formatter.format(item.pointCost).replace(',', '.')
                Spacer(Modifier.height(24.dp))
                Text("$cost points", color = Color(0xfff59e0b), textAlign = TextAlign.Center, fontWeight = FontWeight.SemiBold)
            }
        }
    }
}