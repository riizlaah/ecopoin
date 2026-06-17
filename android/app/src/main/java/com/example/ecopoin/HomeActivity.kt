package com.example.ecopoin

import android.content.Context.MODE_PRIVATE
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
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.EcoPoinTheme
import com.example.ecopoin.ui.theme.Gray1

class HomeActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        HttpClient.prefs = getSharedPreferences("prefs", MODE_PRIVATE)
        HttpClient.loadToken()
        setContent {
            EcoPoinTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    val tabs = listOf(
                        Pair("Home", R.drawable.house),
                        Pair("Submit", R.drawable.upload),
                        Pair("History", R.drawable.clock),
                        Pair("Reward", R.drawable.gift),
                        Pair("Ranking", R.drawable.trophy),
                    )
                    var selectedTab by remember { mutableIntStateOf(0) }
                    val backStack = remember { mutableStateListOf(0) }
                    var username by remember { mutableStateOf("") }
                    var password by remember { mutableStateOf("") }


                    LaunchedEffect(Unit) {
                        HttpClient.me()
                        HttpClient.myRank()
                    }

                    BackHandler(backStack.isNotEmpty()) {
                        selectedTab = backStack.removeAt(backStack.size - 1)
                    }

                    LaunchedEffect(selectedTab) {
                        if(backStack.isEmpty()) backStack.add(selectedTab)
                        else if(backStack.last() != selectedTab) backStack.add(selectedTab)
                    }

                    Column(Modifier.fillMaxSize().padding(innerPadding).background(Color(0xfff8fafc))) {
                        when(selectedTab) {
                            0 -> {}
                            1 -> {}
                            2 -> {}
                            3 -> {}
                            4 -> {}
                        }
                        PrimaryTabRow(selectedTab, Modifier.fillMaxWidth(), containerColor = Color.White) {
                            tabs.forEachIndexed { i, (name, iconId) ->
                                val color = if(selectedTab == i) MaterialTheme.colorScheme.primary else Color.Gray
                                Tab(selectedTab == i, {selectedTab = i}, Modifier.padding(8.dp)) {
                                    Icon(painterResource(iconId), name, tint = color)
                                    Text(name, color = color, fontSize = MaterialTheme.typography.labelSmall.fontSize)
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}