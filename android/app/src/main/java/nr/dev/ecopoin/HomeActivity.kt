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
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import nr.dev.ecopoin.ui.theme.EcoPoinTheme

class HomeActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            EcoPoinTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    Column(
                        Modifier
                            .fillMaxSize()
                            .padding(innerPadding)
                            .background(Color(0xfff8fafc))
                    ) {
                        val tabs = listOf(
                            Pair("Home", R.drawable.house),
                            Pair("Submit", R.drawable.upload),
                            Pair("History", R.drawable.clock),
                            Pair("Reward", R.drawable.gift),
                            Pair("Ranking", R.drawable.trophy),
                        )
                        var selectedTab by remember { mutableIntStateOf(0) }
                        val backStack = remember { mutableStateListOf(0) }

                        BackHandler(backStack.isNotEmpty()) {
                            selectedTab = backStack.removeAt(backStack.size - 1)
                        }

                        LaunchedEffect(selectedTab) {
                            if (backStack.isEmpty()) {
                                backStack.add(selectedTab)
                            } else if (backStack.last() != selectedTab) {
                                backStack.add(selectedTab)
                            }
                        }

                        LazyColumn(Modifier.weight(1f)) {
                            item {
                                when (selectedTab) {
                                    0 -> HomeScreen(
                                        Modifier.fillMaxWidth(),
                                        { selectedTab = 4 },
                                        { selectedTab = 3 },
                                        { selectedTab = 1 })

                                    1 -> SubmitScreen(
                                        Modifier.fillMaxWidth(),
                                        {
                                            selectedTab =
                                                if (backStack.isNotEmpty()) backStack.removeAt(
                                                    backStack.size - 1
                                                ) else 0
                                        })

                                    2 -> {}
                                    3 -> {}
                                    4 -> {}
                                }
                            }
                        }
                        PrimaryTabRow(
                            selectedTab,
                            containerColor = Color.White,
                            contentColor = Color.Unspecified
                        ) {
                            tabs.forEachIndexed { idx, (name, id) ->
                                val selected = idx == selectedTab
                                Tab(selected, { selectedTab = idx }, Modifier.padding(8.dp)) {
                                    Icon(
                                        painterResource(id),
                                        contentDescription = name,
                                        tint = if (selected) MaterialTheme.colorScheme.primary else Color.Gray
                                    )
                                    Text(
                                        name,
                                        fontSize = MaterialTheme.typography.labelSmall.fontSize,
                                        color = if (selected) MaterialTheme.colorScheme.primary else Color.Gray
                                    )
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
