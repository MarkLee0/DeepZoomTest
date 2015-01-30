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
        private CGPoint startLocation;
        private int zoomLevel;
        private int rowCount;
        private int columnCount;
        private CGRect BoundingBox;
        private CGPoint center;

        public TileMatrix(int tileWidth, int tileHeight, CGPoint center, int zoomLevel)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.zoomLevel = zoomLevel;
            this.center = center;

            rowCount = columnCount = (int)Math.Pow(2, zoomLevel);
            nfloat x = center.X - tileWidth * columnCount;
            nfloat y = center.Y - tileHeight * rowCount;

            BoundingBox = new CGRect(x, y, tileWidth * columnCount, tileHeight * rowCount);
        }

        public IEnumerable<TileMatrixCell> GetTileMatrixCells(CGRect currentExtent)
        {
            Collection<TileMatrixCell> drawingMatrixCells = new Collection<TileMatrixCell>();

            int minColumnIndex = Convert.ToInt32(Math.Floor((currentExtent.X - BoundingBox.X) / tileWidth));
            int maxColumnIndex = Convert.ToInt32(Math.Floor((currentExtent.X + currentExtent.Width - BoundingBox.X) / tileWidth));
            int minRowIndex = Convert.ToInt32(Math.Floor((currentExtent.Y - BoundingBox.Y) / tileHeight));
            int maxRowIndex = Convert.ToInt32(Math.Floor((currentExtent.Y + currentExtent.Height - BoundingBox.Y) / tileHeight));

            if (minColumnIndex < 0) minColumnIndex = 0;
            if (maxColumnIndex >= columnCount) maxColumnIndex = columnCount;
            if (minRowIndex < 0) minRowIndex = 0;
            if (maxRowIndex >= rowCount) maxRowIndex = rowCount;

            int rowNumber = maxRowIndex - minRowIndex;
            int columnNumber = maxColumnIndex - minColumnIndex;

            startLocation = CalculateStartLocation(center, rowNumber, columnNumber);

            for (int row = 0; row < rowNumber; row++)
            {
                for (int column = 0; column < columnNumber; column++)
                {
                    drawingMatrixCells.Add(GetCell(row, column));
                }
            }

            return drawingMatrixCells;
        }

        private TileMatrixCell GetCell(int row, int column)
        {
            nfloat x = startLocation.X + tileWidth * column;
            nfloat y = startLocation.Y + tileHeight * row;

            CGRect cellExtent = new CGRect(x, y, tileWidth, tileHeight);
            TileMatrixCell returnCell = new TileMatrixCell(row, column, cellExtent);
            return returnCell;
        }

        private CGPoint CalculateStartLocation(CGPoint centerPoint, int row, int column)
        {
            CGPoint upperLeft = new CGPoint();

            nfloat xOffset = (nfloat)Math.Ceiling(row * .5f) * tileWidth;
            nfloat yOffset = (nfloat)Math.Ceiling(column * .5f) * tileHeight;
            if (row == 1 && column == 1)
            {
                xOffset = tileWidth * .5f;
                yOffset = tileHeight * .5f;
            }

            upperLeft.X = centerPoint.X - xOffset;
            upperLeft.Y = centerPoint.Y - yOffset;

            return upperLeft;
        }
    }
}