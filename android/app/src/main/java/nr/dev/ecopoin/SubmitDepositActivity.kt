package nr.dev.ecopoin

import android.provider.OpenableColumns
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.Image
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
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.HorizontalDivider
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
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import kotlinx.coroutines.launch
import java.text.DecimalFormat
import kotlin.math.exp

@Composable
fun SubmitDepositScreen(modifier: Modifier, onBack: () -> Unit) {
    var opened by remember { mutableStateOf(false) }
    var estimatedWeight by remember { mutableStateOf("") }
    var errMsg by remember { mutableStateOf("") }
    var notes by remember { mutableStateOf("") }
    var currentCategory by remember { mutableStateOf<WasteType?>(null) }
    val wasteTypes = remember { mutableStateListOf<WasteType>() }
    var selectedImg by remember { mutableStateOf<File?>(null) }
    var imgBitmap by remember { mutableStateOf<ImageBitmap?>(null) }
    val ctx = LocalContext.current
    val scope = rememberCoroutineScope()
    var loading by remember { mutableStateOf(false) }
    var postSubmit by remember { mutableStateOf(false) }
    val openMediaAction = rememberLauncherForActivityResult(ActivityResultContracts.PickVisualMedia()) {
        url ->
        url?.let { it ->
            try {
                val mimeType = ctx.contentResolver.getType(it) ?: ""
                if(mimeType.isEmpty() || (mimeType != "image/jpeg" && mimeType != "image/png")) return@let
                var filename = ""
                ctx.contentResolver.query(it, null, null, null, null)?.use { c ->
                    val idx = c.getColumnIndex(OpenableColumns.DISPLAY_NAME)
                    if(idx != -1 && c.moveToFirst()) {
                        filename = c.getString(idx)
                    } else {
                        filename = ""
                    }
                }
                if(filename == "") return@let
                var bytes: ByteArray? = null
                ctx.contentResolver.openInputStream(it)?.use { s ->
                    bytes = s.buffered().use { it.readBytes() }
                }
                if(bytes == null) return@let
                imgBitmap = ctx.contentResolver.asBitmap(it)
                selectedImg = File(filename, bytes, mimeType)
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    LaunchedEffect(Unit) {
        wasteTypes.clear()
        wasteTypes.addAll(HttpClient.getWasteTypes())
    }

    Column(modifier) {
        Row(Modifier
            .fillMaxWidth()
            .background(Color.White)
            .padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
            IconButton(onBack, shape = CircleShape, colors = IconButtonDefaults.iconButtonColors(containerColor = Color.LightGray)) {
                Icon(painterResource(R.drawable.arrow_back), contentDescription = "Back")
            }
            Spacer(Modifier.width(12.dp))
            Text("Submit Trash", fontWeight = FontWeight.SemiBold, fontSize = MaterialTheme.typography.headlineMedium.fontSize)
        }
        if(postSubmit) {
            Column(Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally, verticalArrangement = Arrangement.Center) {
                Text("Thanks for submitting!", fontWeight = FontWeight.Bold, fontSize = MaterialTheme.typography.headlineMedium.fontSize)
                Spacer(Modifier.height(24.dp))
                val estimatedPoints = (currentCategory?.pointTariff ?: 0) * (estimatedWeight.toDoubleOrNull() ?: 0.0)
                val pts = DecimalFormat("#,###").format(estimatedPoints).replace(',', '.')
                Text("Estimated Points")
                Text(pts, fontWeight = FontWeight.Bold, fontSize = MaterialTheme.typography.headlineMedium.fontSize)
                Spacer(Modifier.height(48.dp))
                OutlinedButton(onBack) {
                    Text("Back")
                }
            }
            return@Column
        }
        Text("Choose Category", fontWeight = FontWeight.SemiBold)
        Box(Modifier.fillMaxWidth()) {
            OutlinedButton({opened = !opened}, shape = RoundedCornerShape(12.dp), modifier = Modifier.fillMaxWidth()) {
                Text(currentCategory?.name ?: "Select a category")
            }
            DropdownMenu(opened, {opened = false}, modifier = Modifier.fillMaxWidth(0.9f)) {
                wasteTypes.forEach { item ->
                    DropdownMenuItem({Text(item.name)}, {
                        currentCategory = item
                        opened = false
                    })
                }
            }
        }
        Spacer(Modifier.height(24.dp))
        Text("Estimated Weight", fontWeight = FontWeight.SemiBold)
        OutlinedTextField(estimatedWeight, {estimatedWeight = it}, singleLine = true, modifier = Modifier.fillMaxWidth(), trailingIcon = {Text("kg")})
        Spacer(Modifier.height(24.dp))
        Text("Notes (optional)", fontWeight = FontWeight.SemiBold)
        OutlinedTextField(notes, {notes = it}, modifier = Modifier.fillMaxWidth(), minLines = 3)
        Spacer(Modifier.height(24.dp))
        if(selectedImg == null) {
            Box(Modifier
                .fillMaxWidth()
                .clip(RoundedCornerShape(12.dp))
                .background(Color.LightGray)
                .padding(24.dp, 48.dp)
                .clickable(onClick = {
                    openMediaAction.launch(PickVisualMediaRequest(ActivityResultContracts.PickVisualMedia.ImageOnly))
                })) {
                Text("Select an image (JPG/PNG)")
            }
        } else {
            Image(imgBitmap!!, contentDescription = "", modifier = Modifier.fillMaxWidth())
        }
        Spacer(Modifier.height(24.dp))
        ErrText(errMsg, Modifier.fillMaxWidth())
        Button({
            val estW = estimatedWeight.toDoubleOrNull()
            if(estW == null) {
                errMsg = "Estimated weight required"
                return@Button
            }
            if(selectedImg == null) {
                errMsg = "Photo required"
                return@Button
            }
            if(currentCategory == null) {
                errMsg = "Category must be selected"
                return@Button
            }
            errMsg = ""
            scope.launch {
                loading = true
                when(val msg = HttpClient.submitDeposit(estW, currentCategory!!.id, selectedImg!!, notes)) {
                    "ok" -> postSubmit = true
                    else -> errMsg = msg
                }
                loading = false
            }
        }, shape = RoundedCornerShape(12.dp), modifier = Modifier.fillMaxWidth()) {
            LoadingOrContent(loading) {
                Text("Submit", fontWeight = FontWeight.Bold)
            }
        }

    }
}
