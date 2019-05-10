
namespace UniWebServer
{

    public class DepthReadResult
    {
        public int depthWidth;
        public int depthHeight;
        public float[] depthData;
        public int imgWidth;
        public int imgHeight;
        public byte[] imgData;


        public DepthReadResult(float[] depthData, int depthWidth, int depthHeight, byte[] imgData, int imgWidth, int imgHeight)
        {
            this.depthData = depthData;
            this.depthWidth = depthWidth;
            this.depthHeight = depthHeight;
            this.imgData = imgData;
            this.imgWidth = imgWidth;
            this.imgHeight = imgHeight;
        }
    }

    public class DepthDataResponse
    {
        public int depthWidth;
        public int depthHeight;
        public string depthData;
        public string jpgImageData;

        public DepthDataResponse(string depthData, int depthWidth, int depthHeight, string jpgImageData)
        {
            this.depthData = depthData;
            this.depthWidth = depthWidth;
            this.depthHeight = depthHeight;
            this.jpgImageData = jpgImageData;
        }
    }


}