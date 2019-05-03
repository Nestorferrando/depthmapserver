
namespace UniWebServer
{

    public class DepthReadResult
    {
        public int width;
        public int height;
        public float[] data;

        public DepthReadResult(float[] data, int width, int height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }
    }

    public class DepthDataResponse
    {
        public int width;
        public int height;
        public string data;

        public DepthDataResponse(string data, int width, int height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }
    }


}