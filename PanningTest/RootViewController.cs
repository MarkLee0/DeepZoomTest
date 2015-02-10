using CoreGraphics;
using System;
using UIKit;

namespace PanningTest
{
    public partial class RootViewController : UIViewController
    {
        private UIView tile = new MyTile();
        private UIView map = new UIView();
        private UIView eventView = new UIView();
        private UIView rotationView = new UIView();
        private nfloat animationDuration = 0.6f;
        private nfloat animationOffsetRatio = 0.15f;

        public RootViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            map.Frame = View.Frame;
            View.AddSubview(map);

            eventView.Frame = View.Frame;
            map.AddSubview(eventView);

            rotationView.Frame = View.Frame;
            eventView.AddSubview(rotationView);

            tile.Frame = new CGRect(10, 10, 256, 256);
            tile.BackgroundColor = UIColor.Clear;

            rotationView.AddSubview(tile);

            UIPanGestureRecognizer panGesture = new UIPanGestureRecognizer(PanHandler);
            panGesture.ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true;
            UIPinchGestureRecognizer pinchGesture = new UIPinchGestureRecognizer(PinchHandler);
            pinchGesture.ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true;
            UIRotationGestureRecognizer rotationGesture = new UIRotationGestureRecognizer(RotationHandler);
            rotationGesture.ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true;

            eventView.AddGestureRecognizer(panGesture);
            eventView.AddGestureRecognizer(pinchGesture);
            eventView.AddGestureRecognizer(rotationGesture);
        }

        private void PanHandler(UIPanGestureRecognizer gesture)
        {
            CGPoint translation = gesture.TranslationInView(rotationView);
            if (gesture.State == UIGestureRecognizerState.Began)
            {
                tile.Frame = tile.Layer.PresentationLayer.Frame;
                tile.Layer.RemoveAllAnimations();
            }
            else if (gesture.State == UIGestureRecognizerState.Changed)
            {
                var offsetX = translation.X;
                var offsetY = translation.Y;

                var newLeft = tile.Frame.GetMinX() + offsetX;
                var newTop = tile.Frame.GetMinY() + offsetY;

                tile.Frame = new CGRect(newLeft, newTop, tile.Frame.Width, tile.Frame.Height);
                gesture.SetTranslation(new CGPoint(0, 0), View);
            }
            else if (gesture.State == UIGestureRecognizerState.Ended)
            {
                var inertia = gesture.VelocityInView(rotationView);
                var offsetX = inertia.X * animationOffsetRatio;
                var offsetY = inertia.X * animationOffsetRatio;
                var newLeft = tile.Frame.GetMinX() + offsetX;
                var newTop = tile.Frame.GetMinY() + offsetY;

                UIView.Animate(animationDuration, 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.AllowUserInteraction,
                        () => { tile.Frame = new CGRect(newLeft, newTop, tile.Frame.Width, tile.Frame.Height); }, null);
            }
        }

        private void PinchHandler(UIPinchGestureRecognizer gesture)
        {
            if (gesture.State == UIGestureRecognizerState.Changed)
            {
                CGPoint touchAnchor = gesture.AnchorInView(eventView);
                ScaleView(tile, gesture.Scale, touchAnchor);
                gesture.Scale = 1;
            }
            else if (gesture.State == UIGestureRecognizerState.Ended)
            {
                CGPoint touchAnchor = gesture.AnchorInView(eventView);
                UIView.Animate(animationDuration, 0,
                    UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.AllowUserInteraction,
                    () => ScaleView(tile, 1 + gesture.Velocity * animationOffsetRatio, touchAnchor), null);
            }
        }

        private void RotationHandler(UIRotationGestureRecognizer gesture)
        {
            if (gesture.State == UIGestureRecognizerState.Changed)
            {
                rotationView.Transform = CGAffineTransform.Rotate(rotationView.Transform, gesture.Rotation);
                gesture.Rotation = 0;
            }
        }

        private void ScaleView(UIView targetView, nfloat scale, CGPoint anchor)
        {
            nfloat touchX = anchor.X;
            nfloat touchY = anchor.Y;

            nfloat left = targetView.Frame.GetMinX();
            nfloat top = targetView.Frame.GetMinY();
            nfloat right = targetView.Frame.GetMaxX();
            nfloat bottom = targetView.Frame.GetMaxY();

            nfloat leftToPinchAnchor = touchX - left;
            nfloat rightToPinchAnchor = right - touchX;
            nfloat topToPinchAnchor = touchY - top;
            nfloat bottomToPinchAnchor = bottom - touchY;

            nfloat newLeft = touchX - leftToPinchAnchor * scale;
            nfloat newRight = touchX + rightToPinchAnchor * scale;
            nfloat newTop = touchY - topToPinchAnchor * scale;
            nfloat newBottom = touchY + bottomToPinchAnchor * scale;

            targetView.Frame = new CGRect(newLeft, newTop, newRight - newLeft, newBottom - newTop);
        }
    }

    public class MyTile : UIView
    {
        private int cellCount = 10;
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            using (CGContext context = UIGraphics.GetCurrentContext())
            {
                nfloat cellSize = rect.Width / cellCount;
                context.SetLineWidth(1);
                context.SetStrokeColor(UIColor.Gray.CGColor);

                for (int row = 0; row < cellSize; row++)
                {
                    nfloat top = row * cellSize;
                    CGPoint[] pointsH = { new CGPoint(0, top), new CGPoint(rect.Width, top) };
                    context.AddLines(pointsH);
                    for (int column = 0; column < cellSize; column++)
                    {
                        nfloat left = column * cellSize;
                        CGPoint[] pointsV = { new CGPoint(left, 0), new CGPoint(left, rect.Height) };
                        context.AddLines(pointsV);
                    }
                }

                context.DrawPath(CGPathDrawingMode.FillStroke);
            }
        }

        public override void DrawRect(CGRect area, UIViewPrintFormatter formatter)
        {
            base.DrawRect(area, formatter);
            nfloat cellSize = area.Width / cellCount;
            var context = UIGraphics.GetCurrentContext();
            context.SetLineWidth(1);
            context.SetStrokeColor(UIColor.Gray.CGColor);

            for (int row = 0; row < cellSize; row++)
            {
                nfloat top = row * cellSize;
                CGPoint[] pointsH = { new CGPoint(0, top), new CGPoint(area.Width, top) };
                context.AddLines(pointsH);
                for (int column = 0; column < cellSize; column++)
                {
                    nfloat left = column * cellSize;
                    CGPoint[] pointsV = { new CGPoint(left, 0), new CGPoint(left, area.Height) };
                    context.AddLines(pointsV);
                }
            }

            context.DrawPath(CGPathDrawingMode.FillStroke);
        }
    }

    public static class ExtensionHelper
    {
        public static CGPoint AnchorInView(this UIPinchGestureRecognizer gesture, UIView view)
        {
            nint touchNumber = gesture.NumberOfTouches;
            nfloat touchXSum = 0;
            nfloat touchYSum = 0;

            for (int i = 0; i < touchNumber; i++)
            {
                CGPoint location = gesture.LocationOfTouch(i, view);
                touchXSum += location.X;
                touchYSum += location.Y;
            }

            nfloat touchX = touchXSum / touchNumber;
            nfloat touchY = touchYSum / touchNumber;

            return new CGPoint(touchX, touchY);
        }
    }
}