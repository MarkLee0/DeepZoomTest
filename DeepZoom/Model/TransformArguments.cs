using CoreGraphics;
using System;
using System.Collections.ObjectModel;

namespace DeepZoom
{
    public class TransformArguments
    {
        public TransformArguments()
        {
            ScreenPointers = new Collection<CGPoint>();
        }

        public nfloat ScreenX { get; set; }
        public nfloat ScreenY { get; set; }
        public nfloat ScreenWidth { get; set; }
        public nfloat ScreenHeight { get; set; }
        public Collection<CGPoint> ScreenPointers { get; set; }
        public nfloat OffsetX { get; set; }
        public nfloat OffsetY { get; set; }
    }
}