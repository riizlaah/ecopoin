package nr.dev.ecopoin

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.BackHandler
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.PrimaryTabRow
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Tab
import androidx.compose.material3.Text
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import nr.dev.ecopoin.ui.theme.EcoPoinTheme
import kotlin.math.min

class HomeActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            EcoPoinTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    LaunchedEffect(Unit) {
                        if(HttpClient.profile == null) HttpClient.me()
                        HttpClient.myRank()
                    }
                    Column(
                        Modifier
                            .fillMaxSize(1f)
                            .padding(innerPadding)
                    ) {
                        val tabs = mapOf(
                            "Home" to R.drawable.house,
                            "Submit" to R.drawable.upload,
                            "History" to R.drawable.clock_4,
                            "Reward" to R.drawable.gift,
                            "Ranking" to R.drawable.trophy
                        )
                        var currentTab by remember { mutableIntStateOf(0) }
                        val backStack = remember { mutableStateListOf(0) }

                        BackHandler(backStack.isNotEmpty()) {
                            currentTab = backStack.removeAt(backStack.size - 1)
                        }

                        LazyColumn(
                            Modifier
                                .weight(1f)
                                .background(Color(0xfff8fafc))
                                .padding(12.dp)
                        ) {
                            item {
                                when(currentTab) {
                                    0 -> HomeScreen({}, {}, {})
                                    1 -> {}
                                    2 -> {}
                                    3 -> {}
                                    4 -> {}
                                    else -> {}
                                }
                            }
                        }
                        PrimaryTabRow(currentTab, containerColor = Color.White) {
                            var idx = 0
                            tabs.forEach { (name, iconId) ->
                                Tab(idx == currentTab, {currentTab = idx}, modifier = Modifier.padding(12.dp)) {
                                    Icon(painterResource(iconId), contentDescription = name)
                                    Text(name, fontSize = MaterialTheme.typography.labelMedium.fontSize)
                                }
                                idx += min(idx, 4)
                            }
                        }
                    }
                }
            }
        }
    }
}