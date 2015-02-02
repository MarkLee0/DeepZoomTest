using CoreGraphics;

namespace DeepZoom
{
    public struct TileMatrixCell
    {
        private int row;
        private int column;
        private CGRect boundingBox;

        public TileMatrixCell(int row, int column, CGRect boundingBox)
        {
            this.row = row;
            this.column = column;
            this.boundingBox = boundingBox;
        }

        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        public int Column
        {
            get { return column; }
            set { column = value; }
        }

        public CGRect BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }
    }
}