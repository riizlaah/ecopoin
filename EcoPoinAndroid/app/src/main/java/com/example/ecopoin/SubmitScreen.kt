package com.example.ecopoin

import android.content.Intent
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material3.Button
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.IconButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.Gray1
import kotlinx.coroutines.launch
import kotlin.math.round
import kotlin.math.roundToInt

@Composable
fun SubmitScreen(modifier: Modifier, onBack: () -> Unit) {
    var estWeight by remember { mutableStateOf("") }
    var notes by remember { mutableStateOf("") }
    var errMsg by remember { mutableStateOf("") }
    var loading by remember { mutableStateOf(false) }
    var opened by remember { mutableStateOf(false) }
    var postSubmit by remember { mutableStateOf(false) }
    var selectedFile by remember { mutableStateOf<File?>(null) }
    val wasteTypes = remember { mutableStateListOf<WasteType>() }
    var selectedWT by remember { mutableStateOf<WasteType?>(null) }
    val scope = rememberCoroutineScope()
    val ctx = LocalContext.current
    val openAct = rememberLauncherForActivityResult(ActivityResultContracts.PickVisualMedia()) {
        uri -> uri?.let { u ->
            scope.launch {
                val name = ctx.contentResolver.getFileName(u) ?: return@launch
                val mimetype = ctx.contentResolver.getType(u) ?: return@launch
                val bytes = ctx.contentResolver.getBytes(u) ?: return@launch
                selectedFile = File(name, mimetype, bytes)
            }
        }
    }

    LaunchedEffect(Unit) {
        val (p, v) = HttpClient.getWasteTypes()
        wasteTypes.addAll(v)
    }


    LazyColumn(modifier) {
        stickyHeader {
            Row(Modifier.fillMaxWidth().background(Color.White).padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
                IconButton(
                    onBack,
                    Modifier.size(40.dp),
                    shape = CircleShape,
                    colors = IconButtonDefaults.iconButtonColors(containerColor = Color.LightGray)
                ) {
                    Icon(painterResource(R.drawable.arr_back), "Back")
                }
                Spacer(Modifier.width(12.dp))
                Text("Submit Trash", fontWeight = FontWeight.Bold, fontSize = MaterialTheme.typography.displaySmall.fontSize)
            }
        }
        item {
            if(postSubmit) {
                Column(Modifier.fillMaxSize(), verticalArrangement = Arrangement.Center, horizontalAlignment = Alignment.CenterHorizontally) {
                    Text("Thanks!", fontWeight = FontWeight.Bold, fontSize = MaterialTheme.typography.headlineLarge.fontSize, textAlign = TextAlign.Center)
                    Spacer(Modifier.height(12.dp))
                    Text("Wait the officer to verify your request", color = Color.Gray, textAlign = TextAlign.Center)
                    Spacer(Modifier.height(24.dp))
                    Text("Estimated Points")
                    val pts = ((estWeight.toDoubleOrNull() ?: 0.0) * selectedWT!!.pointTariff).roundToInt()
                    Text("${thousandFmt(pts)} points", fontWeight = FontWeight.Bold)
                    Spacer(Modifier.height(12.dp))
                    OutlinedButton(onBack, shape = corner()) {
                        Text("Back")
                    }
                }
                return@item
            }
            Column(Modifier.fillMaxWidth().padding(20.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                Text("Category")
                Box(Modifier.fillMaxWidth()) {
                    OutlinedButton({opened = !opened}, Modifier.fillMaxWidth(), shape = corner()) {
                        Text(selectedWT?.name ?: "Choose a category")
                    }
                    DropdownMenu(opened, {opened = false}) {
                        wasteTypes.forEach { wt ->
                            DropdownMenuItem({Text(wt.name)}, {
                                selectedWT = wt
                                opened = false
                            })
                        }
                    }
                }
                Spacer(Modifier.height(24.dp))
                Text("Estimated Weight")
                OutlinedTextField(estWeight, {estWeight = it}, Modifier.fillMaxWidth(), shape = corner(), trailingIcon = {Text("kg")})
                Spacer(Modifier.height(24.dp))
                Text("Photo")
                Box(Modifier.fillMaxWidth().cardBordered(Gray1, 20.dp).clickable(onClick = {
                    openAct.launch(PickVisualMediaRequest(ActivityResultContracts.PickVisualMedia.ImageOnly))
                })) {
                    Text(selectedFile?.name ?: "Choose a PNG/JPG image")
                }
                Spacer(Modifier.height(24.dp))
                Text("Notes (optional)")
                OutlinedTextField(notes, {notes = it}, Modifier.fillMaxWidth(), shape = corner(), minLines = 2)
                Spacer(Modifier.height(12.dp))
                ErrText(errMsg, Modifier.fillMaxWidth())
                Button({
                    if(selectedWT == null) {
                        errMsg = "Category required"
                        return@Button
                    }
                    val estW = estWeight.toDoubleOrNull()
                    if(estW == null) {
                        errMsg = "Estimated weight not valid"
                        return@Button
                    }
                    if(selectedFile == null) {
                        errMsg = "Photo required"
                        return@Button
                    }
                    notes = notes.trim()
                    errMsg = ""
                    scope.launch {
                        loading = true
                        when(val msg = HttpClient.submitTrash(selectedWT!!.id, estW, selectedFile!!, notes)) {
                            "ok" -> postSubmit = true
                            else -> errMsg = msg
                        }
                        loading = false
                    }
                }, Modifier.fillMaxWidth(), shape = corner()) {
                    LoadingOrContent(loading) {
                        Text("Submit")
                    }
                }
            }
        }
    }
}