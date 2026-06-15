package nr.dev.ecopoin

import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp

@Composable
fun ErrText(msg: String, modifier: Modifier = Modifier) {
    if(msg.isNotEmpty()) Text(msg, color = Color.Red, modifier = modifier.padding(vertical = 8.dp), textAlign = TextAlign.Center)
}

@Composable
fun LoadingOrContent(loading: Boolean, modifier: Modifier = Modifier, color: Color = Color.White, content: @Composable () -> Unit) {
    if(loading) CircularProgressIndicator(modifier = modifier.size(24.dp), color = color, strokeWidth = 4.dp)
    else content()
}