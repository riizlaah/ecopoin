package nr.dev.ecopoin

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.SecondaryTabRow
import androidx.compose.material3.Tab
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp

@Composable
fun HistoryScreen(modifier: Modifier) {
    var currentTab by remember { mutableIntStateOf(0) }
    val tabs = listOf("Deposits", "Vouchers")



    Column(modifier) {
        Column(Modifier.fillMaxWidth().background(Color.White).padding(12.dp)) {
            Text("History")
            SecondaryTabRow(currentTab, Modifier.fillMaxWidth()) {
                tabs.forEachIndexed { idx, name ->
                    Tab(currentTab == idx, {currentTab = idx}, Modifier.padding(8.dp)) {
                        Text(name)
                    }
                }
            }
            Spacer(Modifier.height(12.dp))
            if(currentTab == 0) {
                val filter = listOf("All", "Pending", "Verified", "Rejected")

                LazyRow(Modifier.fillMaxWidth()) {
                    items(filter) { item ->

                    }
                }
            } else {

            }
        }
    }
}