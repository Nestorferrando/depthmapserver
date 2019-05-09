
namespace UniWebServer
{

    public class DepthReadResult
    {
        public int depthWidth;
        public int depthHeight;
        public float[] depthData;
        public int depthWidthFilter;
        public int depthHeightFilter;
        public float[] depthDataFilter;
        public int imgWidth;
        public int imgHeight;
        public byte[] imgData;


        public DepthReadResult(float[] depthData, int depthWidth, int depthHeight, float[] depthDataFilter, int depthWidthFilter, int depthHeightFilter, byte[] imgData, int imgWidth, int imgHeight)
        {
            this.depthData = depthData;
            this.depthWidth = depthWidth;
            this.depthHeight = depthHeight;
            this.depthDataFilter = depthDataFilter;
            this.depthWidthFilter = depthWidthFilter;
            this.depthHeightFilter = depthHeightFilter;
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
        public int depthWidthFilter;
        public int depthHeightFilter;
        public string depthDataFilter;
        public string jpgImageData;

        public DepthDataResponse(string depthData, int depthWidth, int depthHeight, string depthDataFilter, int depthWidthFilter, int depthHeightFilter, string jpgImageData)
        {
            this.depthData = depthData;
            this.depthWidth = depthWidth;
            this.depthHeight = depthHeight;
            this.depthDataFilter = depthDataFilter;
            this.depthWidthFilter = depthWidthFilter;
            this.depthHeightFilter = depthHeightFilter;
            this.jpgImageData = jpgImageData;
        }
    }


}