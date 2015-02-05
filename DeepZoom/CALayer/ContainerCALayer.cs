using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DeepZoom
{
    [Register("ContainerCAlayer")]
    public class ContainerCAlayer : CALayer
    {
        private CGRect currentExtent;

        public CGRect CurrentExtent
        {
            get { return currentExtent; }
            set { currentExtent = value; }
        }

        public ContainerCAlayer(IntPtr handler)
            : base(handler)
        {

        }

        public void RefreshZoomTileView(RefreshArguments arguments)
        {
            TileMatrix tileMatrix = new TileMatrix(arguments.TileSize, arguments.TileSize, arguments.CurrentCenter, arguments.ZoomLevel, arguments.Scale);
            IEnumerable<TileMatrixCell> drawingTiles = tileMatrix.GetTileMatrixCells(arguments.CurrentCenter, currentExtent);

            Dictionary<string, DeepZoomTileView> drawnTiles = new Dictionary<string, DeepZoomTileView>();

            foreach (var currentTile in Sublayers.OfType<DeepZoomTileView>())
            {
                string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", currentTile.ZoomLevel, currentTile.ColumnIndex, currentTile.RowIndex);
                drawnTiles[key] = currentTile;

                if (currentTile.Frame.X > currentExtent.X + currentExtent.Width ||
                    currentTile.Frame.Y > currentExtent.Y + currentExtent.Height ||
                    currentTile.Frame.X + currentTile.Frame.Width < currentExtent.X ||
                    currentTile.Frame.Y + currentTile.Frame.Height < currentExtent.Y)
                {
                    currentTile.RemoveFromSuperview();
                    currentTile.Dispose();
                }
            }
            if (arguments.TransformArguments != null)
                foreach (var drawnTile in drawnTiles.Select(item => item.Value))
                {
                    TransformTile(drawnTile, arguments.TransformArguments);
                }

            foreach (var drawingTile in drawingTiles)
            {
                string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", arguments.ZoomLevel, drawingTile.Column, drawingTile.Row);

                if (!drawnTiles.ContainsKey(key))
                {
                    DeepZoomTileCAlayer tileView = new DeepZoomTileCAlayer();
                    tileView.RowIndex = drawingTile.Row;
                    tileView.ColumnIndex = drawingTile.Column;
                    tileView.ZoomLevel = arguments.ZoomLevel;
                    tileView.TileHeight = tileView.TileWidth = arguments.TileSize;
                    tileView.Frame = drawingTile.BoundingBox;
                    AddSublayer(tileView);
                }
            }
        }

        public void TransformTile(DeepZoomTileView drawnTile, TransformArguments arguments)
        {
            drawnTile.Center = new CGPoint(drawnTile.Center.X + arguments.OffsetX, drawnTile.Center.Y + arguments.OffsetY);
        }
    }
}