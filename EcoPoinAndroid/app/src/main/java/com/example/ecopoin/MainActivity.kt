package com.example.ecopoin

import android.content.Intent
import android.os.Bundle
import androidx.activity.ComponentActivity
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
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
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
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.EcoPoinTheme
import com.example.ecopoin.ui.theme.Gray1
import kotlinx.coroutines.launch

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        HttpClient.prefs = getSharedPreferences("prefs", MODE_PRIVATE)
        HttpClient.loadToken()
        setContent {
            EcoPoinTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    val ctx = LocalContext.current
                    LaunchedEffect(Unit) {
                        if(HttpClient.me()) {
                            val int = Intent(ctx, HomeActivity::class.java).apply {
                                flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                            }
                            ctx.startActivity(int)
                        }
                    }

                    LazyColumn(
                        Modifier
                            .fillMaxSize()
                            .padding(innerPadding)
                            .padding(24.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        item {
                            val tabs = listOf("login", "register")
                            var currentTab by remember { mutableStateOf(tabs[0]) }
                            var username by remember { mutableStateOf("") }
                            var password by remember { mutableStateOf("") }
                            val scope = rememberCoroutineScope()


                            Image(
                                painterResource(R.drawable.leaf),
                                "Icon",
                                Modifier
                                    .size(100.dp)
                                    .clip(corner(18.dp))
                            )
                            Spacer(Modifier.height(12.dp))
                            Text(
                                "EcoPoin",
                                fontWeight = FontWeight.Bold,
                                fontSize = MaterialTheme.typography.displaySmall.fontSize
                            )
                            Spacer(Modifier.height(12.dp))
                            Text("Collect trashes, get points", color = Color.Gray)
                            Row(
                                Modifier
                                    .padding(vertical = 12.dp)
                                    .fillMaxWidth()
                                    .clip(corner())
                                    .background(
                                        Gray1
                                    )
                                    .padding(8.dp)
                            ) {
                                tabs.forEach { tab ->
                                    val selected = currentTab == tab
                                    Button({currentTab = tab}, Modifier.weight(1f), shape = corner(), colors = ButtonDefaults.buttonColors(
                                        containerColor = if(selected) Color.White else Color.Transparent,
                                        contentColor = Color.DarkGray
                                    )) {
                                        Text(tab.replaceFirstChar { it.uppercase() }, fontWeight = if(selected) FontWeight.Bold else FontWeight.Normal)
                                    }
                                }
                            }
                            if(currentTab == "login") {
                                var loading by remember { mutableStateOf(false) }
                                var errMsg by remember { mutableStateOf("") }

                                Text("Username")
                                OutlinedTextField(username, {username = it}, Modifier.fillMaxWidth(), shape = corner())
                                Spacer(Modifier.height(12.dp))
                                Text("Password")
                                OutlinedTextField(password, {password = it}, Modifier.fillMaxWidth(), shape = corner(), visualTransformation = PasswordVisualTransformation())
                                Spacer(Modifier.height(12.dp))
                                ErrText(errMsg)
                                Button({
                                    if(username.isBlank()) {
                                        errMsg = "Username required"
                                        return@Button
                                    }
                                    if(password.isBlank()) {
                                        errMsg = "Password required"
                                        return@Button
                                    }
                                    errMsg = ""
                                    scope.launch {
                                        loading = true
                                        when(val msg = HttpClient.login(username, password)) {
                                            "ok" -> {
                                                val int = Intent(ctx, HomeActivity::class.java).apply {
                                                    flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                                                }
                                                ctx.startActivity(int)
                                            }
                                            else -> errMsg = msg
                                        }
                                        loading = false
                                    }
                                }, Modifier.fillMaxWidth(), shape = corner()) {
                                    LoadingOrContent(loading) {
                                        Text("Login")
                                    }
                                }
                            } else {
                                var loading by remember { mutableStateOf(false) }
                                var errMsg by remember { mutableStateOf("") }
                                var fullName by remember { mutableStateOf("") }
                                var email by remember { mutableStateOf("") }
                                var phone by remember { mutableStateOf("") }

                                Text("Username")
                                OutlinedTextField(username, {username = it}, Modifier.fillMaxWidth(), shape = corner())
                                Spacer(Modifier.height(12.dp))
                                Text("Full Name")
                                OutlinedTextField(fullName, {fullName = it}, Modifier.fillMaxWidth(), shape = corner())
                                Spacer(Modifier.height(12.dp))
                                Text("Email")
                                OutlinedTextField(email, {email = it}, Modifier.fillMaxWidth(), shape = corner())
                                Spacer(Modifier.height(12.dp))
                                Text("Phone Number")
                                OutlinedTextField(phone, {phone = it}, Modifier.fillMaxWidth(), shape = corner())
                                Spacer(Modifier.height(12.dp))
                                Text("Password")
                                OutlinedTextField(password, {password = it}, Modifier.fillMaxWidth(), shape = corner(), visualTransformation = PasswordVisualTransformation())
                                Spacer(Modifier.height(12.dp))
                                ErrText(errMsg)
                                Button({
                                    if(username.isBlank()) {
                                        errMsg = "Username required"
                                        return@Button
                                    }
                                    if(fullName.isBlank()) {
                                        errMsg = "Full name required"
                                        return@Button
                                    }
                                    if(email.isBlank()) {
                                        errMsg = "Email required"
                                        return@Button
                                    }
                                    if(phone.isBlank()) {
                                        errMsg = "Phone number required"
                                        return@Button
                                    }
                                    if(password.isBlank()) {
                                        errMsg = "Password required"
                                        return@Button
                                    }
                                    errMsg = ""
                                    scope.launch {
                                        loading = true
                                        when(val msg = HttpClient.register(username, fullName, email, phone, password)) {
                                            "ok" -> {
                                                email = ""
                                                phone = ""
                                                fullName = ""
                                                currentTab = "login"
                                            }
                                            else -> errMsg = msg
                                        }
                                        loading = false
                                    }
                                }, Modifier.fillMaxWidth(), shape = corner()) {
                                    LoadingOrContent(loading) {
                                        Text("Register")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
