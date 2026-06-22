package com.example.ecopoin

import androidx.compose.animation.animateColor
import androidx.compose.animation.core.RepeatMode
import androidx.compose.animation.core.infiniteRepeatable
import androidx.compose.animation.core.rememberInfiniteTransition
import androidx.compose.animation.core.tween
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.IconButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.Shape
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.TextUnit
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

class ImgLoader {
    val caches = mutableMapOf<String, ImageBitmap>()

    suspend fun loadImg(url: String): ImageBitmap? {
        caches[url]?.let { return it }
        val img = HttpClient.fetchImg(url)
        if (img != null) caches[url] = img
        return img
    }

    fun hasCache(url: String): Boolean {
        return caches.contains(url)
    }
}


@Composable
fun NetImage(url: String, contentDesc: String, modifier: Modifier = Modifier, contentScale: ContentScale = ContentScale.Fit) {
    val loader = remember { ImgLoader() }
    var img by remember { mutableStateOf<ImageBitmap?>(null) }
    var err by remember { mutableStateOf(false) }
    var loading by remember { mutableStateOf(false) }

    LaunchedEffect(url) {
        if (!loader.hasCache(url)) loading = true
        err = false
        img = loader.loadImg(url)
        if (img == null) err = true
        loading = false
    }

    when {
        loading -> {
            val trans = rememberInfiniteTransition()
            val color by trans.animateColor(
                Color.Gray,
                Color.White,
                infiniteRepeatable(tween(750), repeatMode = RepeatMode.Reverse)
            )
            Box(modifier.background(color))
        }
        err -> {
            Box(modifier, contentAlignment = Alignment.Center) {
                Text("Failed to load image")
            }
        }
        img != null -> {
            Image(img!!, contentDesc, modifier, contentScale = contentScale)
        }
    }
}

@Composable
fun ErrText(
    errMsg: String,
    modifier: Modifier = Modifier,
    fontSize: TextUnit = TextUnit.Unspecified
) {
    if (errMsg.isNotEmpty()) Text(
        errMsg,
        modifier.padding(vertical = 12.dp),
        color = Color.Red,
        textAlign = TextAlign.Center
    )
}

@Composable
fun LoadingOrContent(loading: Boolean, content: @Composable () -> Unit) {
    if (loading) CircularProgressIndicator(
        Modifier.size(24.dp),
        strokeWidth = 3.dp,
        color = Color.White
    )
    else content()
}

@Composable
fun Pagination(modifier: Modifier, paging: Paging, onPrev: () -> Unit, onNext: () -> Unit) {
    Row(
        modifier,
        horizontalArrangement = Arrangement.Center,
        verticalAlignment = Alignment.CenterVertically
    ) {
        if (paging.page > 1) {
            IconButton(onPrev) {
                Icon(painterResource(R.drawable.arr_forward_ios), "Previous", Modifier.rotate(180f))
            }
        }
        Text(
            "${paging.page} / ${paging.totalPage}",
            Modifier.padding(horizontal = 12.dp),
            fontSize = MaterialTheme.typography.titleLarge.fontSize
        )
        if (paging.page < paging.totalPage) {
            IconButton(onNext) {
                Icon(painterResource(R.drawable.arr_forward_ios), "Next")
            }
        }
    }
}

fun Modifier.card(color: Color = Color.White, pad: Dp = 12.dp): Modifier {
    return clip(corner())
        .background(color)
        .padding(pad)
}

fun Modifier.cardBordered(color: Color = Color.White, pad: Dp = 12.dp): Modifier {
    return clip(corner())
        .background(color)
        .border(1.dp, Color.LightGray, corner())
        .padding(pad)
}

fun Modifier.card(shape: Shape, color: Color, pad: Dp): Modifier {
    return clip(shape)
        .background(color)
        .padding(pad)
}