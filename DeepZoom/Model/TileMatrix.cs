using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Collections.ObjectModel;

namespace DeepZoom
{
    public class TileMatrix
    {
        private int tileWidth;
        private int tileHeight;
        private int rowCount;
        private int columnCount;
        private CGRect BoundingBox;

        public TileMatrix(int tileWidth, int tileHeight, CGPoint center, double zoomLevel)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;

            rowCount = columnCount = 1 << (int)zoomLevel;
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
            if (maxColumnIndex >= columnCount) maxColumnIndex = columnCount - 1;
            if (minRowIndex < 0) minRowIndex = 0;
            if (maxRowIndex >= rowCount) maxRowIndex = rowCount - 1;

            nfloat offsetX = 0;
            nfloat offsetY = 0;
            if (maxRowIndex == 0) offsetX = tileWidth >> 1;
            if (maxColumnIndex == 0) offsetY = tileHeight >> 1;

            offsetX = BoundingBox.X - currentExtent.X - offsetX;
            offsetY = BoundingBox.Y - currentExtent.Y - offsetY;

            for (int rowIndex = minRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                for (int columnIndex = minColumnIndex; columnIndex <= maxColumnIndex; columnIndex++)
                {
                    nfloat x = offsetX + tileWidth * columnIndex;
                    nfloat y = offsetY + tileHeight * rowIndex;

                    CGRect cellExtent = new CGRect(x, y, tileWidth, tileHeight);
                    TileMatrixCell returnCell = new TileMatrixCell(rowIndex, columnIndex, cellExtent);
                    drawingMatrixCells.Add(returnCell);
                }
            }

            return drawingMatrixCells;
        }
    }
}