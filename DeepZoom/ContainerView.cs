using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
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
            CGRect currentExtent = new CGRect(-Frame.Width * .5f, -Frame.Height * .5f, Frame.Width, Frame.Height);

            TileMatrix tileMatrix = new TileMatrix(tileWidth, tileHeight, centerPoint, zoomLevel);
            IEnumerable<TileMatrixCell> tileMatrixCells = tileMatrix.GetTileMatrixCells(currentExtent);

            foreach (var cell in tileMatrixCells)
            {
                DeepZoomTileView tileView = new DeepZoomTileView();
                tileView.RowIndex = cell.Row;
                tileView.ColumnIndex = cell.Column;
                tileView.ZoomLevel = zoomLevel;
                tileView.TileHeight = tileView.TileWidth = tileSize;
                tileView.Frame = cell.BoundingBox;
                AddSubview(tileView);
            }
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