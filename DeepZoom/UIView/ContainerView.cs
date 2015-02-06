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
        private Dictionary<string, DeepZoomTileView> tileViewCaches;

        public CGRect CurrentExtent
        {
            get { return currentExtent; }
            set { currentExtent = value; }
        }

        public ContainerView(CGRect bounds)
            : base(bounds)
        {
            tileViewCaches = new Dictionary<string, DeepZoomTileView>();
        }

        public void RefreshZoomTileView(RefreshArguments arguments)
        {
            CGPoint defaultCenter = new CGPoint(Frame.Width * .5f, Frame.Height * .5f);

            TileMatrix tileMatrix = new TileMatrix(arguments.TileSize, arguments.TileSize, defaultCenter, arguments.ZoomLevel);
            IEnumerable<TileMatrixCell> drawingTiles = tileMatrix.GetTileMatrixCells(currentExtent);

            Dictionary<string, DeepZoomTileView> drawnTiles = new Dictionary<string, DeepZoomTileView>();

            foreach (var currentTile in Subviews.OfType<DeepZoomTileView>())
            {
                string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", currentTile.ZoomLevel, currentTile.ColumnIndex, currentTile.RowIndex);
                if (currentTile.Frame.X > currentExtent.Width ||
                    currentTile.Frame.Y > currentExtent.Height ||
                    currentTile.Frame.X + currentTile.Frame.Width < 0 ||
                    currentTile.Frame.Y + currentTile.Frame.Height < 0 ||
                    currentTile.ZoomLevel != arguments.ZoomLevel)
                {
                    currentTile.RemoveFromSuperview();
                    currentTile.Dispose();
                    //if (tileViewCaches.Count >= 100)
                    //{
                    //    string lastKey = tileViewCaches.Last().Key;
                    //    tileViewCaches[lastKey].RemoveFromSuperview();
                    //    tileViewCaches[lastKey].Dispose();
                    //    tileViewCaches.Remove(lastKey);
                    //}
                    //if (!tileViewCaches.ContainsKey(key))
                    //{
                    //    currentTile.Hidden = true;
                    //    tileViewCaches.Add(key, currentTile);
                    //}
                }
                else
                {
                    drawnTiles[key] = currentTile;
                }
            }

            TransformTile(arguments);

            foreach (var drawingTile in drawingTiles)
            {
                string key = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", arguments.ZoomLevel, drawingTile.Column, drawingTile.Row);

                if (!drawnTiles.ContainsKey(key))
                {
                    if (tileViewCaches.ContainsKey(key))
                    {
                        tileViewCaches[key].Hidden = false;
                        tileViewCaches.Remove(key);
                    }
                    else
                    {
                        DeepZoomTileView tileView = new DeepZoomTileView();
                        tileView.RowIndex = drawingTile.Row;
                        tileView.ColumnIndex = drawingTile.Column;
                        tileView.ZoomLevel = arguments.ZoomLevel;
                        tileView.TileHeight = tileView.TileWidth = arguments.TileSize;
                        tileView.Frame = drawingTile.CellExtent;
                        AddSubview(tileView);
                    }
                }
            }
        }

        private void TransformTile(RefreshArguments arguments)
        {
            foreach (var drawnTile in Subviews.OfType<DeepZoomTileView>())
            {
                if (arguments.Scale == 0.0f)
                {
                    drawnTile.Center = new CGPoint(drawnTile.Center.X + arguments.OffsetX, drawnTile.Center.Y + arguments.OffsetY);
                }
                else
                {
                    nfloat left = (nfloat)Math.Ceiling((drawnTile.Frame.X - currentExtent.X));
                    nfloat top = (nfloat)Math.Ceiling((drawnTile.Frame.Y - currentExtent.Y));
                    nfloat width = drawnTile.TileWidth;
                    nfloat height = drawnTile.TileHeight;

                    drawnTile.Frame = new CGRect(left, top, width, height);
                }
            }
        }
    }
}