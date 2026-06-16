package nr.dev.ecopoin

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp

fun corner(size: Dp): RoundedCornerShape {
    return RoundedCornerShape(size)
}

fun corner(size: Int): RoundedCornerShape {
    return RoundedCornerShape(size)
}


@Composable
fun ErrText(errMsg: String, modifier: Modifier = Modifier) {
    if (errMsg.isNotEmpty()) Text(
        errMsg,
        modifier.padding(vertical = 8.dp),
        color = Color.Red,
        textAlign = TextAlign.Center
    )
}

@Composable
fun LoadingOrContent(loading: Boolean, content: @Composable () -> Unit) {
    if (loading) CircularProgressIndicator(
        Modifier.size(24.dp),
        color = Color.White,
        strokeWidth = 3.dp
    )
    else content()
}

@Composable
fun NavPage(
    modifier: Modifier = Modifier,
    paging: Pagination,
    onPrev: () -> Unit,
    onNext: () -> Unit
) {
    Row(
        modifier.padding(vertical = 12.dp),
        horizontalArrangement = Arrangement.Center,
        verticalAlignment = Alignment.CenterVertically
    ) {
        IconButton(onPrev, enabled = paging.page > 1) {
            Icon(painterResource(R.drawable.arr_back0), "Previous")
        }
        Text("${paging.page} / ${paging.totalPage}", Modifier.padding(horizontal = 12.dp))
        IconButton(onNext, enabled = paging.page < paging.totalPage) {
            Icon(painterResource(R.drawable.arr_forward0), "Next")
        }
    }
}