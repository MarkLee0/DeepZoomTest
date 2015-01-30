using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;

namespace DeepZoom
{
    [Register("DeepZoomTileView")]
    public class DeepZoomTileView : UIView
    {  
        private double zoomLevel;
        private int tileWidth = 256;
        private int tileHeight = 256;
        private int rowIndex;
        private int columnIndex;

        public DeepZoomTileView()
        {
            Initialize();
        }

        public DeepZoomTileView(CGRect bounds)
            : base(bounds)
        {
            Initialize();
        }

        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { zoomLevel = value; }
        }
        public int TileWidth
        {
            get { return tileWidth; }
            set { tileWidth = value; }
        }
        public int TileHeight
        {
            get { return tileHeight; }
            set { tileHeight = value; }
        }
        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }
        public int ColumnIndex
        {
            get { return columnIndex; }
            set { columnIndex = value; }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            //List<CGPoint> lines = GetRandomLines(rect);
            //using (CGContext g = UIGraphics.GetCurrentContext())
            //{
            //    g.SetLineWidth(1);
            //    for (int i = 0; i < lines.Count; i += 2)
            //    {
            //        CGPath path = new CGPath();
            //        g.SetStrokeColor(UIColor.Yellow.CGColor);
            //        path.AddLines(new[] { lines[i], lines[i + 1] });

            //        g.AddPath(path);
            //    }
            //    g.DrawPath(CGPathDrawingMode.FillStroke);
            //}

            CrossLayer layer = new CrossLayer(zoomLevel, rowIndex, columnIndex);
            layer.Frame = new CGRect(0, 0, tileWidth, tileHeight);
            Layer.AddSublayer(layer);
            layer.SetNeedsDisplay();
        }

        void Initialize()
        {
            BackgroundColor = UIColor.White;
            Layer.BorderWidth = 1;
            Layer.BorderColor = UIColor.Gray.CGColor;
        }

        private static List<CGPoint> GetRandomLines(CGRect rect)
        {
            List<CGPoint> lines = new List<CGPoint>();

            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(0, (int)rect.Width);
                int y = random.Next(0, (int)rect.Width);
                lines.Add(new PointF(x, y));
            }

            return lines;
        }

        private class CrossLayer : CALayer
        {
            private double zoomLevel;
            private int rowIndex;
            private int columnIndex;
            public CrossLayer()
                : this(0.0f, 0, 0)
            {
            }

            public CrossLayer(double zoomLevel, int rowIndex, int columnIndex)
            {
                this.zoomLevel = zoomLevel;
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;
            }

            public override void DrawInContext(CGContext ctx)
            {
                base.DrawInContext(ctx);

                UIGraphics.PushContext(ctx);

                ctx.SetLineWidth(1);
                ctx.SetStrokeColor(UIColor.Green.CGColor);
                CGPath path = new CGPath();
                path.AddLines(new[] { new CGPoint(0, Frame.Height * .5f), new CGPoint(Frame.Width, Frame.Height * .5f) });
                path.AddLines(new[] { new CGPoint(Frame.Width * .5f, Frame.Height), new CGPoint(Frame.Width * .5f, 0) });
                ctx.AddPath(path);
                ctx.DrawPath(CGPathDrawingMode.FillStroke);

                ctx.SetFillColor(UIColor.Black.CGColor);
                NSString number = new NSString(string.Format("{0}:{1}:{2}", zoomLevel, rowIndex, columnIndex));
                number.DrawString(new CGPoint(Frame.Width * .5f - 18, Frame.Height * .5f - 8), UIFont.FromName("Arial", 12));

                UIGraphics.PopContext();
            }
        }
    }
}