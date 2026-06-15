package nr.dev.ecopoin

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.BackHandler
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
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


data class TabItem(val name: String, val iconId: Int)
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
                        val tabs = listOf(
                            TabItem("Home" , R.drawable.house),
                            TabItem("Submit" , R.drawable.upload),
                            TabItem("History" , R.drawable.clock_4),
                            TabItem("Reward" , R.drawable.gift),
                            TabItem("Ranking" , R.drawable.trophy)
                        )
                        var currentTab by remember { mutableIntStateOf(0) }
                        val backStack = remember { mutableStateListOf(0) }

                        LaunchedEffect(currentTab) {
                            if(backStack.isEmpty()) return@LaunchedEffect
                            if(backStack.last() != currentTab) backStack.add(currentTab)
                        }

                        BackHandler(backStack.isNotEmpty()) {
                            currentTab = backStack.removeAt(backStack.size - 1)
                        }

                        LazyColumn(
                            Modifier
                                .weight(1f)
                                .background(Color(0xfff8fafc))
                                .padding(horizontal = 12.dp)
                        ) {
                            item {
                                when(currentTab) {
                                    0 -> HomeScreen({}, {}, {currentTab = 1})
                                    1 -> SubmitDepositScreen(Modifier.fillMaxSize().background(
                                        MaterialTheme.colorScheme.tertiary), {currentTab = backStack.removeAt(backStack.size - 1)})
                                    2 -> HistoryScreen(Modifier.fillMaxSize())
                                    3 -> {}
                                    4 -> {}
                                    else -> {}
                                }
                            }
                        }
                        PrimaryTabRow(currentTab, containerColor = Color.White) {
                            tabs.forEachIndexed { idx, item ->
                                Tab(idx == currentTab, {currentTab = idx}, modifier = Modifier.padding(12.dp)) {
                                    Icon(painterResource(item.iconId), contentDescription = item.name)
                                    Text(item.name, fontSize = MaterialTheme.typography.labelMedium.fontSize)
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}