package com.example.ecopoin

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.BackHandler
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
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
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.HorizontalDivider
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.IconButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.PrimaryTabRow
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Tab
import androidx.compose.material3.Text
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.derivedStateOf
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
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
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import com.example.ecopoin.ui.theme.EcoPoinTheme
import com.example.ecopoin.ui.theme.Gray1
import com.example.ecopoin.ui.theme.Green1
import kotlinx.coroutines.launch

class DepositDetailActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            EcoPoinTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    val id = intent.getIntExtra("id", 0)
                    var item by remember { mutableStateOf<Deposit?>(null) }
                    var selectedFile by remember { mutableStateOf<File?>(null) }
                    val openAct =
                        rememberLauncherForActivityResult(ActivityResultContracts.PickVisualMedia()) { uri ->
                            uri?.let { u ->
                                val filename = contentResolver.getFileName(u) ?: return@let
                                val mimeType = contentResolver.getType(u) ?: return@let
                                val bytes = contentResolver.getBytes(u) ?: return@let
                                selectedFile = File(filename, mimeType, bytes)
                            }
                        }
                    val scope = rememberCoroutineScope()

                    LaunchedEffect(Unit) {
                        item = HttpClient.getDepositDetail(id)
                    }
                    Column(Modifier
                        .fillMaxSize()
                        .padding(innerPadding)
                        .background(Color(0xfff8fafc)),
                        horizontalAlignment = Alignment.CenterHorizontally) {
                        Row(
                            Modifier
                                .fillMaxWidth()
                                .background(Color.White)
                                .padding(12.dp), verticalAlignment = Alignment.CenterVertically
                        ) {
                            IconButton(
                                { finish() },
                                Modifier.size(40.dp),
                                shape = CircleShape,
                                colors = IconButtonDefaults.iconButtonColors(containerColor = Color.LightGray)
                            ) {
                                Icon(painterResource(R.drawable.arr_back), "Back")
                            }
                            Spacer(Modifier.width(12.dp))
                            Text(
                                "Deposit Detail",
                                fontWeight = FontWeight.Bold,
                                fontSize = MaterialTheme.typography.displaySmall.fontSize
                            )
                        }
                        LazyColumn(Modifier.weight(1f).padding(12.dp)) {
                            item {
                                if (item == null) return@item
                                val dp = item!!

                                var actWeight by remember { mutableStateOf(if(HttpClient.isOfficer) dp.estWeight.toString() else "?") }
                                var estWeight by remember { mutableStateOf(dp.estWeight.toString()) }
                                var notes by remember { mutableStateOf(dp.notes ?: "") }
                                var rejectionReason by remember { mutableStateOf(dp.rejectionReason ?: "") }
                                var errMsg by remember { mutableStateOf("") }
                                var loading by remember { mutableStateOf(false) }
                                var opened by remember { mutableStateOf(false) }
                                var selectedWT by remember { mutableStateOf<WasteType?>(dp.wasteType) }
                                val wasteTypes = remember { mutableStateListOf<WasteType>() }

                                LaunchedEffect(Unit) {
                                    val (_, w) = HttpClient.getWasteTypes()
                                    if(w.isNotEmpty()) {
                                        wasteTypes.clear()
                                        wasteTypes.addAll(w)
                                    }
                                }

                                Row(
                                    Modifier.fillMaxWidth().padding(8.dp, 16.dp),
                                    verticalAlignment = Alignment.CenterVertically,
                                    horizontalArrangement = Arrangement.SpaceBetween
                                ) {
                                    Text(
                                        dp.wasteType.name,
                                        fontWeight = FontWeight.Bold,
                                        fontSize = MaterialTheme.typography.headlineSmall.fontSize
                                    )
                                    Text(dp.status)
                                }
                                Text("Submitter", fontWeight = FontWeight.Bold)
                                Row(Modifier.padding(vertical = 8.dp).fillMaxWidth().cardBordered(), verticalAlignment = Alignment.CenterVertically) {
                                    Box(Modifier.size(42.dp).clip(CircleShape).background(Green1), contentAlignment = Alignment.Center) {
                                        Text(dp.resident.name.first().uppercase(), fontWeight = FontWeight.Bold, color = Color.White)
                                    }
                                    Spacer(Modifier.width(12.dp))
                                    Text(dp.resident.name, fontWeight = FontWeight.Bold)
                                }
                                Text("Category")
                                Box(Modifier.fillMaxWidth()) {
                                    OutlinedButton({opened = !opened}, Modifier.fillMaxWidth(), enabled = dp.status == "Pending", shape = corner()) {
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
                                Text("Estimated Weight")
                                OutlinedTextField(
                                    estWeight,
                                    {estWeight = it},
                                    Modifier.fillMaxWidth(),
                                    shape = corner(),
                                    readOnly = dp.status != "Pending" || HttpClient.isOfficer,
                                    singleLine = true,
                                    trailingIcon = {Text("kg")}
                                )
                                Spacer(Modifier.height(12.dp))
                                Text("Actual Weight")
                                OutlinedTextField(
                                    actWeight,
                                    { actWeight = it },
                                    Modifier.fillMaxWidth(),
                                    shape = corner(),
                                    readOnly = dp.status != "Pending" || !HttpClient.isOfficer,
                                    singleLine = true,
                                    trailingIcon = {Text("kg")}
                                )
                                Spacer(Modifier.height(12.dp))
                                Text("Notes")
                                OutlinedTextField(
                                    notes,
                                    {notes = it},
                                    Modifier.fillMaxWidth(),
                                    shape = corner(),
                                    readOnly = dp.status != "Pending" || HttpClient.isOfficer,
                                    minLines = 2
                                )
                                Spacer(Modifier.height(12.dp))
                                HorizontalDivider(Modifier.padding(vertical = 12.dp))
                                Text("Photo")
                                NetImage(dp.photoPath!!, "Photo", Modifier.fillMaxWidth())
                                Spacer(Modifier.height(8.dp))
                                if(dp.status == "Pending") {
                                    Box(
                                        Modifier
                                            .fillMaxWidth()
                                            .cardBordered(Gray1, 20.dp)
                                            .clickable(onClick = {
                                                openAct.launch(
                                                    PickVisualMediaRequest(
                                                        ActivityResultContracts.PickVisualMedia.ImageOnly
                                                    )
                                                )
                                            })
                                    ) {
                                        Text(selectedFile?.name ?: "Choose a new PNG/JPG image (optional)")
                                    }
                                }
                                HorizontalDivider(Modifier.padding(vertical = 12.dp))
                                if((dp.status == "Pending" && HttpClient.isOfficer) || dp.status == "Rejected") {
                                    Text("Rejection Reason")
                                    OutlinedTextField(
                                        rejectionReason,
                                        { rejectionReason = it },
                                        Modifier.fillMaxWidth(),
                                        shape = corner(),
                                        readOnly = dp.status != "Pending" || !HttpClient.isOfficer,
                                        minLines = 2
                                    )
                                    Spacer(Modifier.height(12.dp))
                                }
                                if(dp.status != "Pending") return@item
                                if(HttpClient.isOfficer) {
                                    ErrText(errMsg, Modifier.fillMaxWidth())
                                    Row(Modifier.fillMaxWidth(), verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                                        Button(
                                            {
                                                if(rejectionReason.isBlank()) {
                                                    errMsg = "Rejection reason required"
                                                    return@Button
                                                }
                                                val actW = actWeight.toDoubleOrNull()
                                                if(actW == null) {
                                                    errMsg = "Actual weight not valid"
                                                    return@Button
                                                }
                                                if(selectedWT == null) {
                                                    errMsg = "Category not selected"
                                                    return@Button
                                                }
                                                errMsg = ""
                                                scope.launch {
                                                    loading = true
                                                    when(val msg = HttpClient.reviewDeposit(id, selectedWT!!.id, actW, selectedFile, false, rejectionReason)) {
                                                        "ok" -> {finish()}
                                                        else -> errMsg = msg
                                                    }
                                                    loading = false
                                                }
                                            },
                                            Modifier.weight(1f),
                                            shape = corner(),
                                            colors = ButtonDefaults.buttonColors(
                                                containerColor = Color.Red,
                                                contentColor = Color.White
                                            )
                                        ) {
                                            LoadingOrContent(loading) {
                                                Text("Reject")
                                            }
                                        }
                                        Button(
                                            {
                                                val actW = actWeight.toDoubleOrNull()
                                                if(actW == null) {
                                                    errMsg = "Actual weight not valid"
                                                    return@Button
                                                }
                                                if(selectedWT == null) {
                                                    errMsg = "Category not selected"
                                                    return@Button
                                                }
                                                errMsg = ""
                                                scope.launch {
                                                    loading = true
                                                    when(val msg = HttpClient.reviewDeposit(id, selectedWT!!.id, actW, selectedFile, true, rejectionReason)) {
                                                        "ok" -> {finish()}
                                                        else -> errMsg = msg
                                                    }
                                                    loading = false
                                                }
                                            },
                                            Modifier.weight(1f),
                                            shape = corner(),
                                        ) {
                                            LoadingOrContent(loading) {
                                                Text("Verify")
                                            }
                                        }
                                    }
                                } else {
                                    Row(Modifier.fillMaxWidth(), verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                                        OutlinedButton(
                                            {finish()},
                                            Modifier.weight(1f),
                                            shape = corner(),
                                        ) {
                                            Text("Back")
                                        }
                                        Button(
                                            {
                                                val estW = estWeight.toDoubleOrNull()
                                                if(estW == null) {
                                                    errMsg = "Actual weight not valid"
                                                    return@Button
                                                }
                                                if(selectedWT == null) {
                                                    errMsg = "Category not selected"
                                                    return@Button
                                                }
                                                errMsg = ""
                                                scope.launch {
                                                    loading = true
                                                    when(val msg = HttpClient.updateDeposit(id, selectedWT!!.id, estW, selectedFile, notes)) {
                                                        "ok" -> {finish()}
                                                        else -> errMsg = msg
                                                    }
                                                    loading = false
                                                }
                                            },
                                            Modifier.weight(1f),
                                            shape = corner(),
                                        ) {
                                            LoadingOrContent(loading) {
                                                Text("Save")
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
    }
}
