using CoreGraphics;

namespace DeepZoom
{
    public class RefreshArguments
    {
        private int tileSize;
        private double zoomLevel;
        private CGPoint currentCenter;
        private TransformArguments transformArguments;

        public int TileSize
        {
            get { return tileSize; }
            set { tileSize = value; }
        }
        public CGPoint CurrentCenter
        {
            get { return currentCenter; }
            set { currentCenter = value; }
        }

        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { zoomLevel = value; }
        }

        public TransformArguments TransformArguments
        {
            get { return transformArguments; }
            set { transformArguments = value; }
        }
    }
}