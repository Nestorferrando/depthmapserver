
namespace UniWebServer
{

    public class DepthReadResult
    {
        public float[] data;
        public int width;
        public int height;

        public DepthReadResult(float[] data, int width, int height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }
    }

}