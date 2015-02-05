using System;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UIKit;

namespace DeepZoom
{
    [Register("ContainerView")]
    public class ContainerView : UIView
    {
        private CGRect currentExtent;

        public CGRect CurrentExtent
        {
            get { return currentExtent; }
            set { currentExtent = value; }
        }

        public ContainerView(CGRect bounds)
            : base(bounds)
        {
        }


        public void RefreshZoomTileView(RefreshArguments arguments)
        {
            TileMatrix tileMatrix = new TileMatrix(arguments.TileSize, arguments.TileSize, arguments.DefaultCenter, arguments.ZoomLevel, arguments.Scale);
            IEnumerable<TileMatrixCell> drawingTiles = tileMatrix.GetTileMatrixCells(currentExtent);

            Dictionary<string, DeepZoomTileView> drawnTiles = new Dictionary<string, DeepZoomTileView>();

            foreach (var currentTile in Subviews.OfType<DeepZoomTileView>())
            {
                if (currentTile.Frame.X > currentExtent.Width ||
                    currentTile.Frame.Y > currentExtent.Height ||
                    currentTile.Frame.X + currentTile.Frame.Width < 0 ||
                    currentTile.Frame.Y + currentTile.Frame.Height < 0)
                {
                    currentTile.RemoveFromSuperview();
                    currentTile.Dispose();
                }
                else
                {
                    string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", currentTile.ZoomLevel, currentTile.ColumnIndex, currentTile.RowIndex);
                    drawnTiles[key] = currentTile;
                }
            }

            if (arguments.TransformArguments != null)
                TransformTile(arguments.TransformArguments);

            foreach (var drawingTile in drawingTiles)
            {
                string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", arguments.ZoomLevel, drawingTile.Column, drawingTile.Row);

                if (!drawnTiles.ContainsKey(key))
                {
                    DeepZoomTileView tileView = new DeepZoomTileView();
                    tileView.RowIndex = drawingTile.Row;
                    tileView.ColumnIndex = drawingTile.Column;
                    tileView.ZoomLevel = arguments.ZoomLevel;
                    tileView.TileHeight = tileView.TileWidth = arguments.TileSize;
                    tileView.Frame = drawingTile.BoundingBox;
                    if (arguments.TransformArguments != null)
                    {
                        tileView.Center = new CGPoint(tileView.Center.X - arguments.TransformArguments.OffsetX, tileView.Center.Y - arguments.TransformArguments.OffsetY);
                    }
                    AddSubview(tileView);
                }
            }
        }

        private void TransformTile(TransformArguments arguments)
        {
            foreach (var drawnTile in Subviews.OfType<DeepZoomTileView>())
            {
                drawnTile.Center = new CGPoint(drawnTile.Center.X - arguments.OffsetX, drawnTile.Center.Y - arguments.OffsetY);
            }
        }
    }
}