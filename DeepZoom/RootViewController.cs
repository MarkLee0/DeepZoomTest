﻿using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace DeepZoom
{
    public partial class RootViewController : UIViewController
    {
        private UITextField txtTileSize;
        private UITextField txtZoomLevel;
        private UILabel lblResult;
        private ContainerView containerView;
        public RootViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            containerView = new ContainerView(View.Frame);
            View.AddSubview(containerView);

            InitializeComponents();

            containerView.RefreshZoomTileView(int.Parse(txtTileSize.Text), int.Parse(txtZoomLevel.Text));
        }

        private void InitializeComponents()
        {
            UIButton btnApply = new UIButton(UIButtonType.System);
            btnApply.SetTitle("Apply", UIControlState.Normal);
            btnApply.Frame = new CGRect(View.Frame.Width - 100, 30, 80, 50);
            btnApply.TouchUpInside += btnApply_TouchUpInside;
            txtTileSize = new UITextField(new CGRect(140, 30, 100, 50));
            txtTileSize.Text = "200";
            txtZoomLevel = new UITextField(new CGRect(340, 30, 100, 50));
            txtZoomLevel.Text = "3";
            UILabel lblTileSize = new UILabel(new CGRect(10, 30, 100, 50));
            lblTileSize.Text = "Tile Size:";
            UILabel lblZoomLevel = new UILabel(new CGRect(240, 30, 100, 50));
            lblZoomLevel.Text = "Zoom Level:";
            lblResult = new UILabel(new CGRect(440, 30, 100, 50));

            View.AddSubview(btnApply);
            View.AddSubview(txtTileSize);
            View.AddSubview(txtZoomLevel);
            View.AddSubview(lblTileSize);
            View.AddSubview(lblZoomLevel);
            View.AddSubview(lblResult);
        }


        void btnApply_TouchUpInside(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            containerView.RefreshZoomTileView(int.Parse(txtTileSize.Text), int.Parse(txtZoomLevel.Text));

            sw.Stop();
            lblResult.Text = string.Format("Cost {0} ms", sw.ElapsedMilliseconds);
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}