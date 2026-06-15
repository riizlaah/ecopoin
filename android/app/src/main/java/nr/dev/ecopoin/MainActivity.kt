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
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextFieldDefaults
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
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
import kotlinx.coroutines.launch
import nr.dev.ecopoin.ui.theme.EcoPoinTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        HttpClient.prefs = getSharedPreferences("prefs", MODE_PRIVATE)
        HttpClient.loadToken()
        setContent {
            EcoPoinTheme {
                val ctx = LocalContext.current

                LaunchedEffect(Unit) {
                    if(HttpClient.me()) {
                        val int = Intent(ctx, HomeActivity::class.java).apply {
                            flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                        }
                        ctx.startActivity(int)
                    }
                }

                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    LazyColumn(
                        Modifier
                            .fillMaxSize(1f)
                            .padding(innerPadding)
                            .padding(18.dp), horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        item {
                            val tabs = listOf("login", "register")
                            var current by remember { mutableStateOf(tabs[0]) }
                            val scope = rememberCoroutineScope()

                            Image(
                                painterResource(R.drawable.leaf),
                                contentDescription = "Leaf",
                                modifier = Modifier
                                    .size(100.dp)
                                    .clip(
                                        RoundedCornerShape(12.dp)
                                    )
                            )
                            Spacer(Modifier.height(12.dp))
                            Text(
                                "EcoPoin",
                                fontSize = MaterialTheme.typography.displaySmall.fontSize,
                                fontWeight = FontWeight.Bold
                            )
                            Spacer(Modifier.height(24.dp))
                            Text("Collect trashes, get points", fontWeight = FontWeight.Light)
                            Spacer(Modifier.height(24.dp))
                            Row(
                                Modifier
                                    .fillMaxWidth()
                                    .clip(RoundedCornerShape(12.dp))
                                    .background(
                                        MaterialTheme.colorScheme.tertiary
                                    )
                            ) {
                                for (tab in tabs) {
                                    Button(
                                        {
                                            current = tab
                                        },
                                        colors = ButtonDefaults.buttonColors(
                                            containerColor = if (current == tab) Color.White else Color.Transparent,
                                            contentColor = Color.Black
                                        ),
                                        shape = RoundedCornerShape(12.dp),
                                        modifier = Modifier.weight(1f)
                                    ) {
                                        Text(
                                            tab.replaceFirstChar { it.uppercase() },
                                            fontWeight = if (current == tab) FontWeight.SemiBold else FontWeight.Normal
                                        )
                                    }
                                }
                            }
                            Spacer(Modifier.height(24.dp))
                            if (current == "login") {
                                var username by remember { mutableStateOf("") }
                                var password by remember { mutableStateOf("") }

                                var loading by remember { mutableStateOf(false) }
                                var errMsg by remember { mutableStateOf("") }

                                Text("Username")
                                OutlinedTextField(
                                    username,
                                    { username = it },
                                    singleLine = true,
                                    colors = TextFieldDefaults.colors(
                                        unfocusedContainerColor = MaterialTheme.colorScheme.tertiary,
                                    ),
                                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp)
                                )
                                Text("Password")
                                OutlinedTextField(
                                    password,
                                    { password = it },
                                    singleLine = true,
                                    colors = TextFieldDefaults.colors(
                                        unfocusedContainerColor = MaterialTheme.colorScheme.tertiary,
                                    ),
                                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp),
                                    visualTransformation = PasswordVisualTransformation()
                                )
                                Spacer(Modifier.height(12.dp))
                                ErrText(errMsg, Modifier.fillMaxWidth())
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
                                }, shape = RoundedCornerShape(12.dp), modifier = Modifier.fillMaxWidth(), contentPadding = PaddingValues(12.dp)) {
                                    LoadingOrContent(loading) {
                                        Text("Login", fontWeight = FontWeight.Bold)
                                    }
                                }
                            } else {
                                var username by remember { mutableStateOf("") }
                                var fullName by remember { mutableStateOf("") }
                                var email by remember { mutableStateOf("") }
                                var phone by remember { mutableStateOf("") }
                                var password by remember { mutableStateOf("") }
                                var loading by remember { mutableStateOf(false) }
                                var errMsg by remember { mutableStateOf("") }

                                Text("Username")
                                OutlinedTextField(
                                    username,
                                    { username = it },
                                    singleLine = true,
                                    colors = TextFieldDefaults.colors(
                                        unfocusedContainerColor = MaterialTheme.colorScheme.tertiary,
                                    ),
                                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp)
                                )
                                Text("Full Name")
                                OutlinedTextField(
                                    fullName,
                                    { fullName = it },
                                    singleLine = true,
                                    colors = TextFieldDefaults.colors(
                                        unfocusedContainerColor = MaterialTheme.colorScheme.tertiary,
                                    ),
                                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp)
                                )
                                Text("Email")
                                OutlinedTextField(
                                    email,
                                    { email = it },
                                    singleLine = true,
                                    colors = TextFieldDefaults.colors(
                                        unfocusedContainerColor = MaterialTheme.colorScheme.tertiary,
                                    ),
                                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp)
                                )
                                Text("Phone Number")
                                OutlinedTextField(
                                    phone,
                                    { phone = it },
                                    singleLine = true,
                                    colors = TextFieldDefaults.colors(
                                        unfocusedContainerColor = MaterialTheme.colorScheme.tertiary,
                                    ),
                                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp)
                                )
                                Text("Password")
                                OutlinedTextField(
                                    password,
                                    { password = it },
                                    singleLine = true,
                                    colors = TextFieldDefaults.colors(
                                        unfocusedContainerColor = MaterialTheme.colorScheme.tertiary,
                                    ),
                                    modifier = Modifier.fillMaxWidth().padding(bottom = 12.dp),
                                    visualTransformation = PasswordVisualTransformation()
                                )
                                Spacer(Modifier.height(12.dp))
                                ErrText(errMsg, Modifier.fillMaxWidth())
                                Button({
                                    if(username.isEmpty()) {
                                        errMsg = "Username required"
                                        return@Button
                                    }
                                    if(fullName.isEmpty()) {
                                        errMsg = "Full name required"
                                        return@Button
                                    }
                                    if(!Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
                                        errMsg = "Email not valid"
                                        return@Button
                                    }
                                    if(phone.isEmpty()) {
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
                                                current = "login"
                                            }
                                            else -> errMsg = msg
                                        }
                                        loading = false
                                    }
                                }, shape = RoundedCornerShape(12.dp), modifier = Modifier.fillMaxWidth(), contentPadding = PaddingValues(12.dp)) {
                                    LoadingOrContent(loading) {
                                        Text("Register", fontWeight = FontWeight.Bold)
                                    }
                                }
                                Spacer(Modifier.height(24.dp))
                            }
                        }
                    }
                }
            }
        }
    }
}