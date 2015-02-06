using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CoreGraphics;
using UIKit;

namespace DeepZoom
{
    public partial class RootViewController : UIViewController
    {
        private UITextField txtTileSize;
        private UITextField txtZoomLevel;
        private UILabel lblRedrawResult;
        private UILabel lblGestureResult;
        private ContainerView containerView;
        private Queue panActionTimeQueue;
        private StringBuilder panResult;
        private CGPoint startPoint = CGPoint.Empty;
        private CGPoint middlePoint = CGPoint.Empty;
        private double zoomLevel;
        private int tileSize;
        private nfloat extentWidth;
        private nfloat extentHeight;

        public RootViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            panActionTimeQueue = new Queue();
            panResult = new StringBuilder();
            containerView = new ContainerView(View.Frame);
            containerView.CurrentExtent = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
            containerView.AddGestureRecognizer(new UIPanGestureRecognizer(PanGestureRecognizerHandler));
            containerView.AddGestureRecognizer(new UIPinchGestureRecognizer(PinchGestureRecognizerHandler));

            View.AddSubview(containerView);
            InitializeComponents();

            RefreshTileView();
        }

        private void PinchGestureRecognizerHandler(UIPinchGestureRecognizer gestureRecognizer)
        {
            Stopwatch sw = Stopwatch.StartNew();

            RefreshArguments arguments = CollectArguments(gestureRecognizer);
            if (middlePoint.Equals(CGPoint.Empty))
            {
                if (gestureRecognizer.NumberOfTouches <= 1) return;
                CGPoint point1 = gestureRecognizer.LocationOfTouch(0, containerView);
                CGPoint point2 = gestureRecognizer.LocationOfTouch(1, containerView);
                nfloat x = point1.X - point2.X;
                nfloat y = point1.Y - point2.Y;

                middlePoint = new CGPoint(point1.X + x, point1.Y + y);
            }

            if (tileSize == 0)
            {
                zoomLevel = arguments.ZoomLevel;
                tileSize = arguments.TileSize;

                extentWidth = containerView.CurrentExtent.Width;
                extentHeight = containerView.CurrentExtent.Height;
            }

            arguments.Scale = gestureRecognizer.Scale;

            arguments.ZoomLevel = zoomLevel * gestureRecognizer.Scale;
            arguments.TileSize = (int)(tileSize * gestureRecognizer.Scale);
            txtZoomLevel.Text = arguments.ZoomLevel.ToString();
            txtTileSize.Text = arguments.TileSize.ToString();

            nfloat newLeft = middlePoint.X - gestureRecognizer.Scale * middlePoint.X;
            nfloat newTop = middlePoint.Y - gestureRecognizer.Scale * middlePoint.Y;
            nfloat newWidth = extentWidth * gestureRecognizer.Scale;
            nfloat newHeight = extentHeight * gestureRecognizer.Scale;
            containerView.CurrentExtent = new CGRect(newLeft, newTop, newWidth, newHeight);
            containerView.RefreshZoomTileView(arguments);

            if (gestureRecognizer.State == UIGestureRecognizerState.Ended)
            {
                middlePoint = CGPoint.Empty;
                zoomLevel = 0.0f;
                tileSize = 0;
                extentWidth = 0.0f;
                extentHeight = 0.0f;
            }

            sw.Stop();
            GestureTimeMonitorAction(sw.ElapsedMilliseconds);
        }

        private void InitializeComponents()
        {
            UIButton btnApply = new UIButton(UIButtonType.System);
            btnApply.SetTitle("Apply", UIControlState.Normal);
            btnApply.Frame = new CGRect(240, 30, 80, 50);
            btnApply.TouchUpInside += btnApply_TouchUpInside;
            txtTileSize = new UITextField(new CGRect(140, 30, 100, 50));
            txtTileSize.Text = "128";
            txtZoomLevel = new UITextField(new CGRect(140, 80, 100, 50));
            txtZoomLevel.Text = "3";
            UILabel lblTileSize = new UILabel(new CGRect(10, 30, 100, 50));
            lblTileSize.Text = "Tile Size:";
            UILabel lblZoomLevel = new UILabel(new CGRect(10, 80, 100, 50));
            lblZoomLevel.Text = "Zoom Level:";
            lblRedrawResult = new UILabel(new CGRect(10, 130, 150, 50));
            lblGestureResult = new UILabel(new CGRect(10, 180, 150, 300));
            lblGestureResult.LineBreakMode = UILineBreakMode.WordWrap;
            lblGestureResult.Lines = 0;

            View.AddSubview(btnApply);
            View.AddSubview(txtTileSize);
            View.AddSubview(txtZoomLevel);
            View.AddSubview(lblTileSize);
            View.AddSubview(lblZoomLevel);
            View.AddSubview(lblRedrawResult);
            View.AddSubview(lblGestureResult);
        }

        private void PanGestureRecognizerHandler(UIPanGestureRecognizer gestureRecognizer)
        {
            Stopwatch sw = Stopwatch.StartNew();
            RefreshArguments arguments = CollectArguments(gestureRecognizer);

            nfloat x = containerView.CurrentExtent.X - arguments.OffsetX;
            nfloat y = containerView.CurrentExtent.Y - arguments.OffsetY;
            containerView.CurrentExtent = new CGRect(new CGPoint(x, y), containerView.CurrentExtent.Size);

            containerView.RefreshZoomTileView(arguments);

            if (gestureRecognizer.State == UIGestureRecognizerState.Ended)
            {
                startPoint = CGPoint.Empty;
            }

            sw.Stop();
            GestureTimeMonitorAction(sw.ElapsedMilliseconds);
        }

        private void btnApply_TouchUpInside(object sender, EventArgs e)
        {
            panActionTimeQueue.Clear();
            lblGestureResult.Text = string.Empty;
            RefreshTileView();
        }

        private void RefreshTileView()
        {
            Stopwatch sw = Stopwatch.StartNew();

            RefreshArguments arguments = new RefreshArguments();
            arguments.ZoomLevel = double.Parse(txtZoomLevel.Text);
            arguments.TileSize = int.Parse(txtTileSize.Text);
            containerView.CurrentExtent = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);

            foreach (var tileView in containerView.Subviews.OfType<DeepZoomTileView>())
            {
                tileView.RemoveFromSuperview();
                tileView.Dispose();
            }

            for (int i = 0; i < 10; i++)
            {
                containerView.RefreshZoomTileView(arguments);
            }

            sw.Stop();
            lblRedrawResult.Text = string.Format("Redraw: {0} ms", sw.ElapsedMilliseconds / 10);
        }

        private void GestureTimeMonitorAction(long time)
        {
            panResult.Clear();
            if (panActionTimeQueue.Count > 6)
            {
                panActionTimeQueue.Dequeue();
            }
            panActionTimeQueue.Enqueue(time);

            foreach (var item in panActionTimeQueue)
            {
                panResult.AppendLine(string.Format("Pan： {0}ms", item));
            }

            lblGestureResult.Text = panResult.ToString();
        }

        private RefreshArguments CollectArguments(UIGestureRecognizer e)
        {
            RefreshArguments arguments = new RefreshArguments();
            arguments.ZoomLevel = double.Parse(txtZoomLevel.Text);
            arguments.TileSize = int.Parse(txtTileSize.Text);

            CGPoint location = e.LocationInView(containerView);
            if (startPoint.Equals(CGPoint.Empty))
                startPoint = location;

            arguments.OffsetX = location.X - startPoint.X;
            arguments.OffsetY = location.Y - startPoint.Y;

            startPoint = location;
            return arguments;
        }
    }
}