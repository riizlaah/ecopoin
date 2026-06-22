package com.example.ecopoin

import android.content.Intent
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.BackHandler
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.PrimaryTabRow
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Tab
import androidx.compose.material3.Text
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.derivedStateOf
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.EcoPoinTheme
import com.example.ecopoin.ui.theme.Gray1
import kotlinx.coroutines.launch

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
                            .background(Color(0xfff8fafc)),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        LaunchedEffect(Unit) {
                            HttpClient.me()
                            HttpClient.myRank()
                        }

                        if(HttpClient.profile == null) return@Column
                        val tabs by remember {
                            derivedStateOf {
                                if(!HttpClient.isOfficer) {
                                    listOf(
                                        Pair("Home", R.drawable.house),
                                        Pair("Upload", R.drawable.upload),
                                        Pair("History", R.drawable.clock),
                                        Pair("Reward", R.drawable.gift),
                                        Pair("Ranking", R.drawable.trophy),
                                    )
                                } else {
                                    listOf(
                                        Pair("Home", R.drawable.house),
                                        Pair("History", R.drawable.clock),
                                    )
                                }
                            }
                        }
                        var selectedIdx by remember { mutableIntStateOf(0) }
                        val backStack = remember { mutableStateListOf(0) }

                        fun back() {
                            selectedIdx = if(backStack.isEmpty()) 0 else backStack.removeAt(backStack.size - 1)
                        }

                        BackHandler(backStack.isNotEmpty()) {
                           back()
                        }


                        LaunchedEffect(selectedIdx) {
                            if(backStack.isEmpty()) backStack.add(selectedIdx)
                            else if(backStack.last() != selectedIdx) backStack.add(selectedIdx)
                        }

                        when(selectedIdx) {
                            0 -> HomeScreen(Modifier.weight(1f), {selectedIdx = 4}, {selectedIdx = 1}, {selectedIdx = 3}, {selectedIdx = 2})
                            1 -> {
                                if(HttpClient.isOfficer) {
                                    HistoryScreen(Modifier.weight(1f))
                                } else {
                                    SubmitScreen(Modifier.weight(1f), {back()})
                                }
                            }
                            2 -> HistoryScreen(Modifier.weight(1f))
                            3 -> RewardScreen(Modifier.weight(1f))
                            4 -> RankingScreen(Modifier.weight(1f))
                        }

                        PrimaryTabRow(selectedIdx, containerColor = Color.White) {
                            tabs.forEachIndexed { index, (name, iconId) ->
                                val selected = selectedIdx == index
                                Tab(selected, {selectedIdx = index}, Modifier.padding(8.dp)) {
                                    Icon(painterResource(iconId), name, tint = if(selected) MaterialTheme.colorScheme.primary else Color.Gray)
                                    Text(name, color = if(selected) MaterialTheme.colorScheme.primary else Color.Gray)
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
