using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
        private int zoomLevel;
        private CGRect currentExtent;

        public CGRect CurrentExtent
        {
            get { return currentExtent; }
            set { currentExtent = value; }
        }

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
            this.zoomLevel = zoomLevel;

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

        public Action<long> PanTimeMonitorAction;
        private CGPoint startPoint = CGPoint.Empty;
        private nfloat offsetX;
        private nfloat offsetY;
        private CGRect movingExtent;
        private void GestureRecognizerHandler(UIPanGestureRecognizer gestureRecognizer)
        {
            Stopwatch sw = Stopwatch.StartNew();
            CGPoint point = gestureRecognizer.LocationInView(this);
            //GestureArguments arguments = CollectArguments(gestureRecognizer);

            //RefreshZoomTileView(arguments);

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

            sw.Stop();
            if (PanTimeMonitorAction != null)
            {
                PanTimeMonitorAction(sw.ElapsedMilliseconds);
            }
        }

        private void RefreshZoomTileView(GestureArguments arguments)
        {
            TileMatrix tileMatrix = new TileMatrix(tileWidth, tileHeight, centerPoint, zoomLevel);
            IEnumerable<TileMatrixCell> drawingTiles = tileMatrix.GetTileMatrixCells(currentExtent);

            Dictionary<string, DeepZoomTileView> drawnTiles = new Dictionary<string, DeepZoomTileView>();

            foreach (var currentTile in Subviews.OfType<DeepZoomTileView>())
            {
                string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", currentTile.ZoomLevel, currentTile.ColumnIndex, currentTile.RowIndex);
                drawnTiles[key] = currentTile;
            }

            foreach (var drawnTile in drawnTiles.Select(item => item.Value))
            {
                TransformTile(drawnTile, arguments);
            }

            foreach (var drawingTile in drawingTiles)
            {
                string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", zoomLevel, drawingTile.Column, drawingTile.Row);

                if (!drawnTiles.ContainsKey(key))
                {
                    DeepZoomTileView tileView = new DeepZoomTileView();
                    tileView.RowIndex = drawingTile.Row;
                    tileView.ColumnIndex = drawingTile.Column;
                    tileView.ZoomLevel = zoomLevel;
                    tileView.TileHeight = tileHeight;
                    tileView.TileWidth = tileWidth;
                    tileView.Frame = drawingTile.BoundingBox;
                    AddSubview(tileView);
                }
            }
        }

        private void TransformTile(DeepZoomTileView drawnTile, GestureArguments arguments)
        {
            throw new NotImplementedException();
        }


        private GestureArguments CollectArguments(UIGestureRecognizer e)
        {
            GestureArguments arguments = new GestureArguments();

            CGPoint location = e.LocationInView(this);

            if (e.NumberOfTouches > 0)
            {
                for (int i = 0; i < e.NumberOfTouches; i++)
                {
                    arguments.ScreenPointers.Add(e.LocationOfTouch(i, this));
                }
            }

            arguments.ScreenX = location.X;
            arguments.ScreenY = location.Y;
            arguments.ScreenWidth = Frame.Width;
            arguments.ScreenHeight = Frame.Height;
            arguments.CurrentExtent = CurrentExtent;

            return arguments;
        }
    }
}