package nr.dev.ecopoin

import android.graphics.BitmapFactory
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
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
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.text.DecimalFormat

@Composable
fun SubmitScreen(modifier: Modifier, onBack: () -> Unit) {
    Column(modifier, horizontalAlignment = Alignment.CenterHorizontally) {
        val ctx = LocalContext.current
        var estWeight by remember { mutableStateOf("") }
        var notes by remember { mutableStateOf("") }
        var errMsg by remember { mutableStateOf("") }
        var loading by remember { mutableStateOf(false) }
        var postSubmit by remember { mutableStateOf(false) }
        var opened by remember { mutableStateOf(false) }
        var currentWasteType by remember { mutableStateOf<WasteType?>(null) }
        val wasteTypes = remember { mutableStateListOf<WasteType>() }
        var selectedFile by remember { mutableStateOf<File?>(null) }
        var previewImg by remember { mutableStateOf<ImageBitmap?>(null) }
        val scope = rememberCoroutineScope()
        val openImgAct = rememberLauncherForActivityResult(ActivityResultContracts.PickVisualMedia()) {
            uri -> uri?.let { u ->
                scope.launch {
                    val filename = ctx.contentResolver.getFilename(u) ?: return@launch
                    val mimetype = ctx.contentResolver.getType(u) ?: return@launch
                    if(!listOf("image/jpeg", "image/png").contains(mimetype)) return@launch
                    val bytes = ctx.contentResolver.getBytes(u) ?: return@launch
                    selectedFile = File(filename, mimetype, bytes)
                    withContext(Dispatchers.IO) {
                        previewImg = BitmapFactory.decodeByteArray(bytes, 0, bytes.size).asImageBitmap()
                    }
                }
        }
        }

        LaunchedEffect(Unit) {
            wasteTypes.clear()
            wasteTypes.addAll(HttpClient.getWasteTypes())
        }

        Row(
            Modifier
                .fillMaxWidth()
                .background(Color.LightGray)
                .padding(bottom = 1.dp)
                .background(Color.White)
                .padding(12.dp)
        ) {
            IconButton(
                onBack,
                shape = CircleShape,
                colors = IconButtonDefaults.iconButtonColors(
                    containerColor = Color.LightGray,
                    contentColor = Color.Black
                ),
                modifier = Modifier.size(42.dp)
            ) {
                Icon(painterResource(R.drawable.arr_back), "Back")
            }
            Spacer(Modifier.width(12.dp))
            Text(
                "Submit Trash",
                fontSize = MaterialTheme.typography.displaySmall.fontSize,
                fontWeight = FontWeight.Bold
            )
        }
        if(postSubmit) {
            Column(Modifier.fillMaxSize(), verticalArrangement = Arrangement.Center, horizontalAlignment = Alignment.CenterHorizontally) {
                Text("Thanks!", fontWeight = FontWeight.Bold, fontSize = MaterialTheme.typography.displaySmall.fontSize)
                Spacer(Modifier.height(12.dp))
                Text("Wait the officer to verify your request", color = Color.Gray)
                Spacer(Modifier.height(48.dp))
                Text("Estimated Points")
                val pts = DecimalFormat("#,###").format((estWeight.toDoubleOrNull() ?: 0.0) * currentWasteType!!.pointTariff)
                Text(pts, fontWeight = FontWeight.Bold)
                Spacer(Modifier.height(48.dp))
                OutlinedButton(onBack, shape = corner(12.dp)) {
                    Text("Back")
                }
            }
            return@Column
        }
        Column(
            Modifier
                .fillMaxWidth()
                .padding(24.dp)
        ) {
            Text("Category", fontWeight = FontWeight.Bold)
            Box(Modifier.fillMaxWidth()) {
                OutlinedButton(
                    { opened = !opened },
                    Modifier.fillMaxWidth(),
                    shape = corner(12.dp)
                ) {
                    Text(currentWasteType?.name ?: "Please select a category")
                }
                DropdownMenu(opened, { opened = false }, Modifier.fillMaxWidth()) {
                    wasteTypes.forEach { item ->
                        DropdownMenuItem({ Text(item.name) }, {
                            currentWasteType = item
                            opened = false
                        })
                    }
                }
            }
            Spacer(Modifier.height(24.dp))
            Text("Estimated Weight", fontWeight = FontWeight.Bold)
            OutlinedTextField(
                estWeight,
                { estWeight = it },
                Modifier.fillMaxWidth(),
                singleLine = true,
                trailingIcon = { Text("kg") })
            Spacer(Modifier.height(24.dp))
            Text("Trash Photo", fontWeight = FontWeight.Bold)
            if (selectedFile == null || previewImg == null) {
                Box(
                    Modifier
                        .fillMaxWidth()
                        .height(128.dp)
                        .clip(corner(12.dp))
                        .background(Color(0xfff8fafc))
                        .border(1.dp, Color.LightGray, corner(12.dp))
                        .clickable(onClick = {
                            openImgAct.launch(PickVisualMediaRequest(ActivityResultContracts.PickVisualMedia.ImageOnly))
                        }), contentAlignment = Alignment.Center
                ) {
                    Text("Select a photo (JPG/PNG)")
                }
            } else {
                Image(previewImg!!, "Preview", Modifier.fillMaxWidth().border(1.dp, Color.LightGray))
            }
            Spacer(Modifier.height(24.dp))
            Text("Notes (optional)", fontWeight = FontWeight.Bold)
            OutlinedTextField(
                notes,
                { notes = it },
                Modifier.fillMaxWidth(),
                minLines = 3)
            Spacer(Modifier.height(12.dp))
            ErrText(errMsg)
            Button({
                val estimatedWeight = estWeight.toDoubleOrNull()
                if(estimatedWeight == null) {
                    errMsg = "Estimated weight not valid"
                    return@Button
                }
                if(currentWasteType == null) {
                    errMsg = "Category required"
                    return@Button
                }
                if(selectedFile == null) {
                    errMsg = "Photo required"
                    return@Button
                }
                scope.launch {
                    loading = true
                    when(val msg = HttpClient.submitTrash(currentWasteType!!.id, estimatedWeight, selectedFile!!, notes)) {
                        "ok" -> {
                            postSubmit = true
                        }
                        else -> errMsg = msg
                    }
                    loading = false
                }

            }, Modifier.padding(vertical = 12.dp).fillMaxWidth(), shape = corner(12.dp)) {
                LoadingOrContent(loading) {
                    Text("Submit")
                }
            }
        }

    }
}