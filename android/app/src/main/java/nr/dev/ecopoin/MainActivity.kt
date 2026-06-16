package nr.dev.ecopoin

import android.content.Intent
import android.os.Bundle
import android.util.Patterns
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
import androidx.compose.ui.unit.dp
import kotlinx.coroutines.launch
import nr.dev.ecopoin.ui.theme.EcoPoinTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        HttpClient.prefs = getSharedPreferences("prefs", MODE_PRIVATE)
        HttpClient.loadToken()
        enableEdgeToEdge()
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
                            .padding(32.dp),
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
                                contentDescription = "Icon",
                                modifier = Modifier.size(100.dp).clip(corner(12.dp))
                            )
                            Spacer(Modifier.height(24.dp))
                            Text(
                                "EcoPoin",
                                fontWeight = FontWeight.Black,
                                fontSize = MaterialTheme.typography.displaySmall.fontSize
                            )
                            Spacer(Modifier.height(18.dp))
                            Text("Collect trashes, get points", color = Color.Gray)
                            Spacer(Modifier.height(24.dp))
                            Row(
                                Modifier
                                    .fillMaxWidth()
                                    .clip(corner(12.dp))
                                    .background(MaterialTheme.colorScheme.tertiary)
                                    .padding(6.dp),
                            ) {
                                tabs.forEach { tab ->
                                    Button({currentTab = tab}, Modifier.weight(1f), shape = corner(12.dp), colors = ButtonDefaults.buttonColors(
                                        containerColor = if(currentTab == tab) Color.White else Color.Transparent,
                                        contentColor = Color.DarkGray
                                    )) {
                                        Text(tab.replaceFirstChar { it.uppercase() }, fontWeight = FontWeight.Bold)
                                    }
                                }
                            }
                            Spacer(Modifier.height(12.dp))
                            if(currentTab == "login") {
                                var errMsg by remember { mutableStateOf("") }
                                var loading by remember { mutableStateOf(false) }

                                Text("Username")
                                OutlinedTextField(username, {username = it}, Modifier.fillMaxWidth(), singleLine = true)
                                Spacer(Modifier.height(16.dp))
                                Text("Password")
                                OutlinedTextField(password, {password = it}, Modifier.fillMaxWidth(), singleLine = true, visualTransformation = PasswordVisualTransformation())
                                Spacer(Modifier.height(16.dp))
                                ErrText(errMsg)
                                Button({
                                    if(username.isEmpty()) {
                                        errMsg = "Username required"
                                        return@Button
                                    }
                                    if(password.isEmpty()) {
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

                                }, Modifier.fillMaxWidth(), shape = corner(12.dp)) {
                                    LoadingOrContent(loading) {
                                        Text("Login")
                                    }
                                }
                            } else {
                                var fullName by remember { mutableStateOf("") }
                                var email by remember { mutableStateOf("") }
                                var phone by remember { mutableStateOf("") }
                                var errMsg by remember { mutableStateOf("") }
                                var loading by remember { mutableStateOf(false) }

                                Text("Username")
                                OutlinedTextField(username, {username = it}, Modifier.fillMaxWidth(), singleLine = true)
                                Spacer(Modifier.height(16.dp))
                                Text("Full Name")
                                OutlinedTextField(fullName, {fullName = it}, Modifier.fillMaxWidth(), singleLine = true)
                                Spacer(Modifier.height(16.dp))
                                Text("Email")
                                OutlinedTextField(email, {email = it}, Modifier.fillMaxWidth(), singleLine = true)
                                Spacer(Modifier.height(16.dp))
                                Text("Phone Number")
                                OutlinedTextField(phone, {phone = it}, Modifier.fillMaxWidth(), singleLine = true)
                                Spacer(Modifier.height(16.dp))
                                Text("Password")
                                OutlinedTextField(password, {password = it}, Modifier.fillMaxWidth(), singleLine = true, visualTransformation = PasswordVisualTransformation())
                                Spacer(Modifier.height(16.dp))
                                ErrText(errMsg)
                                Button({
                                    if(username.isBlank()) {
                                        errMsg = "Username required"
                                        return@Button
                                    }
                                    if(fullName.isBlank()) {
                                        errMsg = "Full Name required"
                                        return@Button
                                    }
                                    if(!Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
                                        errMsg = "Email not valid"
                                        return@Button
                                    }
                                    if(phone.isBlank()) {
                                        errMsg = "Phone number required"
                                        return@Button
                                    }
                                    if(password.isEmpty()) {
                                        errMsg = "Password required"
                                        return@Button
                                    }
                                    errMsg = ""
                                    scope.launch {
                                        loading = true
                                        when(val msg = HttpClient.register(username, fullName, email, phone, password)) {
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

                                }, Modifier.fillMaxWidth(), shape = corner(12.dp)) {
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
