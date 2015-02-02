using CoreAnimation;
using CoreGraphics;
using Foundation;
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

            CrossLayer layer = new CrossLayer(rowIndex, columnIndex);
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

        private class CrossLayer : CALayer
        {
            private int rowIndex;
            private int columnIndex;
            public CrossLayer()
                : this(0, 0)
            {
            }

            public CrossLayer(int rowIndex, int columnIndex)
            {
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;
            }

            public override void DrawInContext(CGContext ctx)
            {
                base.DrawInContext(ctx);

                ctx.SetLineWidth(1);
                ctx.SetStrokeColor(UIColor.Green.CGColor);
                ctx.AddLines(new[] { new CGPoint(0, Frame.Height * .5f), new CGPoint(Frame.Width, Frame.Height * .5f) });
                ctx.AddLines(new[] { new CGPoint(Frame.Width * .5f, Frame.Height), new CGPoint(Frame.Width * .5f, 0) });
                ctx.DrawPath(CGPathDrawingMode.Stroke);

                UIGraphics.PushContext(ctx);
                ctx.SetFillColor(UIColor.Black.CGColor);
                NSString number = new NSString(string.Format("{0}:{1}", columnIndex, rowIndex));
                number.DrawString(new CGPoint(Frame.Width * .5f - 18, Frame.Height * .5f - 8), UIFont.FromName("Arial", 12));
                UIGraphics.PopContext();
            }
        }
    }
}