package com.example.ecopoin

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp


@Composable
fun ErrText(errMsg: String, modifier: Modifier = Modifier) {
    if(errMsg.isNotEmpty()) Text(errMsg, modifier.padding(vertical = 12.dp), textAlign = TextAlign.Center, color = Color.Red)
}

@Composable
fun LoadingOrContent(loading: Boolean, content: @Composable () -> Unit) {
    if(loading) CircularProgressIndicator(Modifier.size(24.dp), color = Color.White, strokeWidth = 3.dp)
    else content()
}

fun corner(): RoundedCornerShape {
  return RoundedCornerShape(12.dp)
}
fun corner(size: Dp): RoundedCornerShape {
    return RoundedCornerShape(size)
}

fun corner(size: Int): RoundedCornerShape {
    return RoundedCornerShape(size)
}

fun Modifier.Card(color: Color, paddingSize: Dp): Modifier {
    return clip(corner(12.dp)).background(color).padding(paddingSize)
}