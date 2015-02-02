using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using CoreGraphics;
using UIKit;

namespace DeepZoom
{
    public partial class RootViewController : UIViewController
    {
        private UITextField txtTileSize;
        private UITextField txtZoomLevel;
        private UILabel lblResult;
        private UILabel lblPanResult;
        private ContainerView containerView;
        private Queue panActionTimeQueue;
        private StringBuilder panResult;

        public RootViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            panActionTimeQueue = new Queue(6);
            panResult = new StringBuilder();
            containerView = new ContainerView(View.Frame);
            containerView.CurrentExtent = new CGRect(-View.Frame.Width * .5f, -View.Frame.Height * .5f, View.Frame.Width, View.Frame.Height);
            containerView.PanTimeMonitorAction = PanTimeMonitorAction;

            View.AddSubview(containerView);
            InitializeComponents();

            RefreshTileView();
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
            lblResult = new UILabel(new CGRect(10, 130, 120, 50));
            lblPanResult = new UILabel(new CGRect(10, 180, 120, 300));
            lblPanResult.LineBreakMode = UILineBreakMode.WordWrap;
            lblPanResult.Lines = 0;

            View.AddSubview(btnApply);
            View.AddSubview(txtTileSize);
            View.AddSubview(txtZoomLevel);
            View.AddSubview(lblTileSize);
            View.AddSubview(lblZoomLevel);
            View.AddSubview(lblResult);
            View.AddSubview(lblPanResult);
        }

        private void btnApply_TouchUpInside(object sender, EventArgs e)
        {
            panActionTimeQueue.Clear();
            lblPanResult.Text = string.Empty;
            RefreshTileView();
        }

        private void RefreshTileView()
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                containerView.RefreshZoomTileView(int.Parse(txtTileSize.Text), int.Parse(txtZoomLevel.Text));
            }

            sw.Stop();
            lblResult.Text = string.Format("Redraw: {0} ms", sw.ElapsedMilliseconds / 10);
        }

        private void PanTimeMonitorAction(long time)
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

            lblPanResult.Text = panResult.ToString();
        }
    }
}