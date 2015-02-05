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
        private int rowCount;
        private int columnCount;
        private CGRect BoundingBox;
        private CGPoint center;
        private double scale;

        public TileMatrix(int tileWidth, int tileHeight, CGPoint center, double zoomLevel, double scale)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.center = center;
            this.scale = scale;

            rowCount = columnCount = (int)Math.Pow(2, zoomLevel);
            nfloat x = center.X - tileWidth * columnCount * .5f;
            nfloat y = center.Y - tileHeight * rowCount * .5f;

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
            nfloat tileScale = (nfloat)scale;

            for (int row = minRowIndex; row < maxRowIndex; row++)
            {
                for (int column = minColumnIndex; column < maxColumnIndex; column++)
                {
                    nfloat x = startLocation.X + tileWidth * tileScale * (column - minColumnIndex);
                    nfloat y = startLocation.Y + tileHeight * tileScale * (row - minRowIndex);

                    CGRect cellExtent = new CGRect(x, y, tileWidth * tileScale, tileHeight * tileScale);
                    TileMatrixCell returnCell = new TileMatrixCell(row, column, cellExtent);
                    drawingMatrixCells.Add(returnCell);
                }
            }

            return drawingMatrixCells;
        }

        private CGPoint CalculateStartLocation(CGPoint centerPoint, int row, int column)
        {
            CGPoint upperLeft = new CGPoint();

            nfloat xOffset = (nfloat)column * .5f * tileWidth;
            nfloat yOffset = (nfloat)row * .5f * tileHeight;
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