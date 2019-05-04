import java.net.HttpURLConnection
import java.net.URL


fun main(args: Array<String>) {
    val data = sendGet("127.0.0.1", 8079, "depthdata")
    println(data)

}

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