using System;

namespace DeepZoom
{
    public class RefreshArguments
    {
        public nfloat OffsetX { get; set; }
        public nfloat OffsetY { get; set; }
        public int TileSize { get; set; }
        public double ZoomLevel { get; set; }
    }
}