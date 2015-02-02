using CoreGraphics;
using System;
using System.Collections.ObjectModel;

namespace DeepZoom
{
    public class GestureArguments
    {
        public GestureArguments()
        {
            ScreenPointers = new Collection<CGPoint>();
        }

        public nfloat ScreenX { get; set; }
        public nfloat ScreenY { get; set; }
        public nfloat ScreenWidth { get; set; }
        public nfloat ScreenHeight { get; set; }
        public CGRect CurrentExtent { get; set; }
        public Collection<CGPoint> ScreenPointers { get; set; }
    }
}