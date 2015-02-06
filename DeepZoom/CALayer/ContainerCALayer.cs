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
            CGPoint defaultCenter = new CGPoint(Frame.Width * .5f, Frame.Height * .5f);

            TileMatrix tileMatrix = new TileMatrix(arguments.TileSize, arguments.TileSize, defaultCenter, arguments.ZoomLevel);
            IEnumerable<TileMatrixCell> drawingTiles = tileMatrix.GetTileMatrixCells(currentExtent);

            Dictionary<string, DeepZoomTileCAlayer> drawnTiles = new Dictionary<string, DeepZoomTileCAlayer>();

            foreach (var currentTile in Sublayers.OfType<DeepZoomTileCAlayer>())
            {
                string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", currentTile.ZoomLevel, currentTile.ColumnIndex, currentTile.RowIndex);
                drawnTiles[key] = currentTile;

                if (currentTile.Frame.X > currentExtent.X + currentExtent.Width ||
                    currentTile.Frame.Y > currentExtent.Y + currentExtent.Height ||
                    currentTile.Frame.X + currentTile.Frame.Width < currentExtent.X ||
                    currentTile.Frame.Y + currentTile.Frame.Height < currentExtent.Y)
                {
                    currentTile.RemoveFromSuperLayer(); ;
                    currentTile.Dispose();
                }
            }
            TransformTile(arguments);

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
                    tileView.Frame = drawingTile.CellExtent;
                    AddSublayer(tileView);
                }
            }
        }

        public void TransformTile(RefreshArguments arguments)
        {
            foreach (var drawnTile in Sublayers.OfType<DeepZoomTileCAlayer>())
            {
                drawnTile.Position = new CGPoint(drawnTile.Position.X + arguments.OffsetX,
                    drawnTile.Position.Y + arguments.OffsetY);
            }
        }
    }
}