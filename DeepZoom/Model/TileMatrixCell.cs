using CoreGraphics;

namespace DeepZoom
{
    public struct TileMatrixCell
    {
        private int row;
        private int column;
        private CGRect cellExtent;

        public TileMatrixCell(int row, int column, CGRect cellExtent)
        {
            this.row = row;
            this.column = column;
            this.cellExtent = cellExtent;
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

        public CGRect CellExtent
        {
            get { return cellExtent; }
            set { cellExtent = value; }
        }
    }
}