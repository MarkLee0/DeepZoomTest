using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using UIKit;

namespace DeepZoom
{
    [Register("ContainerView")]
    public class ContainerView : UIView
    {
        private int tileWidth = 256;
        private int tileHeight = 256;

        private CGPoint centerPoint;

        public ContainerView(CGRect bounds)
            : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            centerPoint = new CGPoint(Frame.Width * .5f, Frame.Height * .5f);
            AddGestureRecognizer(new UIPanGestureRecognizer(GestureRecognizerHandler));
        }

        public void RefreshZoomTileView(int tileSize, int zoomLevel)
        {
            foreach (var tile in Subviews.OfType<DeepZoomTileView>())
            {
                tile.RemoveFromSuperview();
            }

            tileHeight = tileWidth = tileSize;

            int tileCount = (int)Math.Pow(2, zoomLevel);

            for (int i = 0; i < tileCount; i++)
            {
                for (int j = 0; j < tileCount; j++)
                {
                    DeepZoomTileView tileView = new DeepZoomTileView();
                    tileView.RowIndex = i;
                    tileView.ColumnIndex = j;
                    tileView.ZoomLevel = zoomLevel;
                    tileView.TileHeight = tileView.TileWidth = tileSize;
                    CGPoint tileViewCenter = CalculateCenterPoint(i, j, zoomLevel);
                    tileView.Frame = new CGRect(tileViewCenter, new CGSize(tileView.TileWidth, tileView.TileHeight));
                    AddSubview(tileView);
                }
            }
        }

        private CGPoint CalculateCenterPoint(int rowIndex, int columnIndex, int zoomLevel)
        {
            CGPoint newCenterPoint = new CGPoint();

            nfloat xOffset = zoomLevel * tileWidth;
            nfloat yOffset = zoomLevel * tileHeight;
            if (zoomLevel == 0)
            {
                xOffset = tileWidth * .5f;
                yOffset = tileHeight * .5f;
            }

            newCenterPoint.X = centerPoint.X + columnIndex * tileWidth - xOffset;
            newCenterPoint.Y = centerPoint.Y + rowIndex * tileHeight - yOffset;

            return newCenterPoint;
        }

        private CGPoint startPoint = CGPoint.Empty;
        private nfloat offsetX;
        private nfloat offsetY;
        private void GestureRecognizerHandler(UIPanGestureRecognizer gestureRecognizer)
        {
            CGPoint point = gestureRecognizer.LocationInView(this);
            switch (gestureRecognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    if (startPoint.Equals(CGPoint.Empty))
                        startPoint = point;
                    break;
                case UIGestureRecognizerState.Changed:
                    offsetX = point.X - startPoint.X;
                    offsetY = point.Y - startPoint.Y;
                    foreach (var view in Subviews)
                    {
                        view.Center = new CGPoint(view.Center.X + offsetX, view.Center.Y + offsetY);
                    }
                    startPoint = point;
                    break;
                case UIGestureRecognizerState.Ended:
                    startPoint = CGPoint.Empty;
                    break;
                default:
                    break;
            }
        }
    }
}