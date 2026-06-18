package com.example.ecopoin

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.Shape
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp

fun corner(): RoundedCornerShape {
    return RoundedCornerShape(12.dp)
}

fun corner(size: Dp): RoundedCornerShape {
    return RoundedCornerShape(12.dp)
}

fun corner(size: Int): RoundedCornerShape {
    return RoundedCornerShape(12.dp)
}


@Composable
fun ErrText(errMsg: String, modifier: Modifier = Modifier) {
    if(errMsg.isNotEmpty()) Text(errMsg, modifier.padding(vertical = 12.dp), color = Color.Red, textAlign = TextAlign.Center)
}

@Composable
fun LoadingOrContent(loading: Boolean, content: @Composable () -> Unit) {
    if(loading) CircularProgressIndicator(Modifier.size(24.dp), strokeWidth = 3.dp, color = Color.White)
    else content()
}

fun Modifier.card(color: Color = Color.White, pad: Dp = 12.dp): Modifier {
    return clip(corner()).background(color).padding(pad)
}

fun Modifier.cardBordered(color: Color = Color.White, pad: Dp = 12.dp): Modifier {
    return clip(corner()).background(color).border(1.dp, Color.LightGray, corner()).padding(pad)
}

fun Modifier.card(shape: Shape, color: Color, pad: Dp): Modifier {
    return clip(shape).background(color).padding(pad)
}