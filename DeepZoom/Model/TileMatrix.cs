using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Collections.ObjectModel;

namespace DeepZoom
{
    public class TileMatrix
    {
        private nfloat tileWidth;
        private nfloat tileHeight;
        private int rowCount;
        private int columnCount;
        private CGRect BoundingBox;
        private double scale;

        public TileMatrix(int tileWidth, int tileHeight, CGPoint center, double zoomLevel, double scale)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.scale = scale;

            //rowCount = columnCount = (int)scale * (int)Math.Pow(2, zoomLevel);
            rowCount = columnCount = (int)scale << (int)zoomLevel;
            nfloat x = center.X - tileWidth * (columnCount >> 1);
            nfloat y = center.Y - tileHeight * (rowCount >> 1);

            BoundingBox = new CGRect(x, y, tileWidth * columnCount, tileHeight * rowCount);
        }

        public IEnumerable<TileMatrixCell> GetTileMatrixCells(CGRect currentExtent)
        {
            Collection<TileMatrixCell> drawingMatrixCells = new Collection<TileMatrixCell>();

            int minColumnIndex = Convert.ToInt32(Math.Floor((currentExtent.X - BoundingBox.X) / tileWidth));
            int maxColumnIndex = Convert.ToInt32(Math.Ceiling((currentExtent.X + currentExtent.Width - BoundingBox.X) / tileWidth));
            int minRowIndex = Convert.ToInt32(Math.Floor((currentExtent.Y - BoundingBox.Y) / tileHeight));
            int maxRowIndex = Convert.ToInt32(Math.Ceiling((currentExtent.Y + currentExtent.Height - BoundingBox.Y) / tileHeight));

            if (minColumnIndex < 0) minColumnIndex = 0;
            if (maxColumnIndex >= columnCount) maxColumnIndex = columnCount;
            if (minRowIndex < 0) minRowIndex = 0;
            if (maxRowIndex >= rowCount) maxRowIndex = rowCount;

            nfloat tileScale = (nfloat)scale;

            if (maxRowIndex == 1 && maxColumnIndex == 1)
            {
                nfloat x = BoundingBox.X - tileWidth * .5f;
                nfloat y = BoundingBox.Y - tileHeight * .5f;

                CGRect cellExtent = new CGRect(x, y, tileWidth * tileScale, tileHeight * tileScale);
                TileMatrixCell returnCell = new TileMatrixCell(0, 0, cellExtent);
                drawingMatrixCells.Add(returnCell);
            }
            else
            {
                for (int row = minRowIndex; row < maxRowIndex; row++)
                {
                    for (int column = minColumnIndex; column < maxColumnIndex; column++)
                    {
                        nfloat x = BoundingBox.X + tileWidth * tileScale * column;
                        nfloat y = BoundingBox.Y + tileHeight * tileScale * row;

                        CGRect cellExtent = new CGRect(x, y, tileWidth * tileScale, tileHeight * tileScale);
                        TileMatrixCell returnCell = new TileMatrixCell(row, column, cellExtent);
                        drawingMatrixCells.Add(returnCell);
                    }
                }
            }

            return drawingMatrixCells;
        }
    }
}