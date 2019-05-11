import java.io.File
import java.net.HttpURLConnection
import java.net.URL
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*


fun main(args: Array<String>) {

    val address = if (args.isNotEmpty()) args[0] else "127.0.0.1"
    val targetFile = if (args.size>1) args[1] else "iphoneDeph-${LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyy-MM-dd_HH.mm.ss"))}.json"
    val filtered = args.any { it.contains("-withfilter") }
    val endPoint = if (filtered) "smoothdepthdata" else "depthdata"
    val data = sendGet(address, 8079, endPoint) ?: throw RuntimeException("Received no response from server")

    /*
    //decode sample
    val response =Gson().fromJson(data,Response::class.java)
    val img = Base64.getDecoder().decode(response.jpgImageData)
    val depth = Base64.getDecoder().decode(response.depthData)
    val buffer = ByteBuffer.wrap(depth)
    buffer.order( ByteOrder.LITTLE_ENDIAN)
    val float2 = buffer.getFloat(0)
    println(float2)
    File("image.jpg").writeBytes(img)*/

    val file = File(targetFile)
    File(File(file.absolutePath).parent).mkdirs()
    file.writeText(data)
}


data class Response(val depthData: String,val depthWidth: Int,val depthHeight: Int,val jpgImageData: String)

fun sendGet(address: String, port: Int, endPoint: String): String? {
    val url = URL("http://$address:$port/$endPoint")
    try {
        with(url.openConnection() as HttpURLConnection) {
            requestMethod = "GET"  // optional default is GET
            println("\nSent 'GET' request to URL : $url; Response Code : $responseCode")
            if (responseCode != 200) return null

            var data = ""
            inputStream.bufferedReader().use {
                it.lines().forEach { line ->
                    if (line.isNotEmpty()) data += line
                }
            }
            return data
        }
    } catch (e: Exception) {
        println("\nSent 'GET' request to URL : $url timed out")
        return null
    }
}