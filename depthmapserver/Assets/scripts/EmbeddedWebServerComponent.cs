using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using System.Net;
using System.Collections;
using System.Runtime.InteropServices;

namespace UniWebServer
{


    public class EmbeddedWebServerComponent : MonoBehaviour
    {
        public bool startOnAwake = true;
        public int port = 8079;
        public int workerThreads = 2;
        public bool processRequestsInMainThread = true;
        public bool logRequests = true;


        private int requestCount = 0;
        private WebServer server;
        private DepthCapture capture_;
        public bool filterSelected = true;

        IEnumerator Start()
        {

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (processRequestsInMainThread)
                Application.runInBackground = true;
            server = new WebServer(port, workerThreads, processRequestsInMainThread);
            server.logRequests = logRequests;
            server.HandleRequest += HandleRequest;
            if (startOnAwake)
            {
                server.Start();
            }
            GameObject.Find("serveraddress").GetComponent<Text>().text = GetLocalIPAddress();
            GameObject.Find("status").GetComponent<Text>().text = "Camera Filtering: " + filterSelected;
            GameObject.Find("status").GetComponent<Text>().color = filterSelected ? Color.green : Color.yellow;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                capture_ = new DepthCapture();
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
                capture_.Configure(filter: filterSelected);
                capture_.Start();
            }

        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var add = "";
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    add += ip.ToString() + ":" + port + "\n";
                }
            }
            if (add.Length == 0) add = "No network adapters found";
            return add;

        }



        public static string GetJPGDataURI(byte[] imgBytes)
        {
            return "<img src=\"data:image/jpeg"
                        + ";base64,"
                        + Convert.ToBase64String(imgBytes) + "\" />";
        }



        void OnApplicationQuit()
        {
            server.Dispose();
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                capture_.Stop();
                capture_.Dispose();
            }
        }


        void Update()
        {
            if (server.processRequestsInMainThread)
            {
                server.ProcessRequests();
            }
        }


        void HandleRequest(Request request, Response response)
        {
            requestCount++;
            GameObject.Find("counter").GetComponent<Text>().text = requestCount + "";
            GameObject.Find("accesslog").GetComponent<Text>().text = DateTime.Now.ToString();

            var localPath = request.uri.LocalPath;
            Debug.Log("request localPath :" + localPath);


            switch (localPath)
            {
                case "/depthdata": onDepthData(response, false); break;
                case "/viewdepth": onViewDepth(response, false); break;
                case "/smoothdepthdata": onDepthData(response, true); break;
                case "/viewsmoothdepth": onViewDepth(response, true); break;
                default:
                    response.statusCode = 200;
                    response.message = "OK";
                    response.Write("<html><body bgcolor=\"#E6E6FA\"> <h2>depth map webserver for iphone</h2>  <h3>endpoints:</h3>  <h5>\t /viewdepth</h5> <h5>\t /depthdata</h5> <h5>\t /viewsmoothdepth</h5> <h5>\t /smoothdepthdata</h5> </body></html>");
                    break;
            }
        }

        private void onViewDepth(Response response, bool filtered)
        {
            changeFilterStatus(filtered);
            var depthMap = getDepthDataFloatArray();
            response.statusCode = 200;
            response.message = "OK";
            response.Write("<html><body bgcolor=\"#E6E6FA\"><h3>depth:</h3> " + GetJPGDataURI(getDepthJPG(depthMap)) + "<h3>image:</h3> " + GetJPGDataURI(getimgJPG(depthMap)) + "</body></html>");
        }

        private void onDepthData(Response response, bool filtered)
        {
            changeFilterStatus(filtered);

            var depthMap = getDepthDataFloatArray();
            response.statusCode = 200;
            response.message = "OK";
            var depthResponse = new DepthDataResponse(
                Convert.ToBase64String(floatToByteArray(depthMap.depthData)), depthMap.depthWidth, depthMap.depthHeight, Convert.ToBase64String(getimgJPG(depthMap)));
            response.Write(JsonUtility.ToJson(depthResponse));

        }

        private void changeFilterStatus(bool newStatus)
        {
            if (newStatus != filterSelected)
            {
                filterSelected = newStatus;

                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    capture_.Stop();
                    capture_.Dispose();
                    capture_ = new DepthCapture();
                    capture_.Configure(filter: filterSelected);
                    capture_.Start();
                }
                else System.Threading.Thread.Sleep(2000);
                GameObject.Find("status").GetComponent<Text>().text = "Camera Filtering: " + filterSelected;
                GameObject.Find("status").GetComponent<Text>().color = filterSelected ? Color.green : Color.yellow;
            }
        }

        private byte[] getDepthJPG(DepthReadResult result)
        {

            var texture = new Texture2D(result.depthWidth, result.depthHeight);
            for (var y = 0; y < result.depthHeight; y++)
            {
                for (var x = 0; x < result.depthWidth; x++)
                {
                    var v = result.depthData[y * result.depthWidth + x];
                    Color color;
                    if (float.IsNaN(v))
                    {
                        color = new Color(0f, 1f, 0f);
                    }
                    else
                    {
                        color = new Color(v, v, v);
                    }
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            var jpg = texture.EncodeToJPG(100);
            Destroy(texture);
            return jpg;
        }

        private byte[] getimgJPG(DepthReadResult result)
        {

            var texture = new Texture2D(result.imgWidth, result.imgHeight);
            for (var y = 0; y < result.imgHeight; y++)
            {
                for (var x = 0; x < result.imgWidth; x++)
                {
                    var b = result.imgData[(y * result.imgWidth + x) * 4];
                    var g = result.imgData[(y * result.imgWidth + x) * 4 + 1];
                    var r = result.imgData[(y * result.imgWidth + x) * 4 + 2];
                    var a = result.imgData[(y * result.imgWidth + x) * 4 + 3];
                    var color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            var jpg = texture.EncodeToJPG(100);
            Destroy(texture);
            return jpg;
        }


        private byte[] floatToByteArray(float[] fArray)
        {
            var byteArray = new byte[fArray.Length * 4];
            Buffer.BlockCopy(fArray, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }


        private DepthReadResult getDepthDataFloatArray()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                int width = 0, height = 0;
                float[] pixels = null;
                int imgWidth = 0, imgHeight = 0;
                byte[] imgPixels = null;
                capture_.AcquireNextFrame((pVideoData, videoWidth, videoHeight, pDepthData, depthWidth, depthHeight) =>
                {
                    width = depthWidth;
                    height = depthHeight;
                    imgWidth = videoWidth;
                    imgHeight = videoHeight;
                    pixels = new float[width * height];
                    imgPixels = new byte[imgWidth * imgHeight * 4];
                    Marshal.Copy(pDepthData, pixels, 0, width * height);
                    Marshal.Copy(pVideoData, imgPixels, 0, imgWidth * imgHeight * 4);

                });
                return new DepthReadResult(pixels, width, height, imgPixels, imgWidth, imgHeight);
            }
            else
            {
                var checkerboard = new float[4096];
                var checkerboardImg = new byte[4096 * 4];
                for (var y = 0; y < 64; y++)
                {
                    for (var x = 0; x < 64; x++)
                    {
                        checkerboard[y * 64 + x] = y / 64f;
                        checkerboardImg[(y * 64 + x) * 4] = 128;
                        checkerboardImg[(y * 64 + x) * 4 + 1] = 128;
                        checkerboardImg[(y * 64 + x) * 4 + 2] = (byte)(y * 3);
                        checkerboardImg[(y * 64 + x) * 4 + 3] = 255;

                    }
                }
                return new DepthReadResult(checkerboard, 64, 64, checkerboardImg, 64, 64);
            }
        }

    }

}