using System;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.Views;

public class ZoomImage : Image
{
    #region Constructiors
    public ZoomImage()
    {
        var pinch = new PinchGestureRecognizer();
        pinch.PinchUpdated += OnPinchUpdated;
        GestureRecognizers.Add(pinch);

        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(pan);

        var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        tap.Tapped += OnTapped;
        GestureRecognizers.Add(tap);

        Scale = MinScale;
        TranslationX = TranslationY = 0;
        AnchorX = AnchorY = 0.5;
    }
    #endregion

    #region Overrides
    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
    {
        Scale = MinScale;
        TranslationX = TranslationY = 0;
        AnchorX = AnchorY = 0;
        return base.OnMeasure(widthConstraint, heightConstraint);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (_width != width || _height != height)
        {
            _width = width;
            _height = height;

            Scale = MinScale;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0.5;

            this.ScaleTo(MinScale, 250, Easing.CubicInOut);
            this.TranslateTo(0.5, 0.5, 250, Easing.CubicInOut);
        }
        // ReSharper restore CompareOfFloatsByEqualityOperator
    }
    #endregion

    #region Methods
    private void OnTapped(object sender, EventArgs e)
    {
        if (Scale > MinScale)
        {
            this.ScaleTo(MinScale, 250, Easing.CubicInOut);
            this.TranslateTo(0, 0, 250, Easing.CubicInOut);
        }
        else
        {
            AnchorX = AnchorY = 0.5; //TODO tapped position
            this.ScaleTo(MaxScale, 250, Easing.CubicInOut);
        }
    }
    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
            {
                _lastX = (1 - AnchorX) * Width;
                _lastY = (1 - AnchorY) * Height;
                break;
            }
            case GestureStatus.Running:
            {
                AnchorX = Clamp(1 - (_lastX + e.TotalX) / Width, 0, 1);
                AnchorY = Clamp(1 - (_lastY + e.TotalY) / Height, 0, 1);
                break;
            }
        }
    }
    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
            {
                _startScale = Scale;
                AnchorX = e.ScaleOrigin.X;
                AnchorY = e.ScaleOrigin.Y;
                break;
            }
            case GestureStatus.Running:
            {
                var current = Scale + (e.Scale - 1) * _startScale;
                Scale = Clamp(current, MinScale * (1 - Overshoot), MaxScale * (1 + Overshoot));
                break;
            }
            case GestureStatus.Completed:
            {
                if (Scale > MaxScale) this.ScaleTo(MaxScale, 250, Easing.SpringOut);
                else if (Scale < MinScale) this.ScaleTo(MinScale, 250, Easing.SpringOut);
                break;
            }
        }
    }

    private T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
    {
        if (value.CompareTo(minimum) < 0) return minimum;
        else if (value.CompareTo(maximum) > 0) return maximum;
        else return value;
    }
    #endregion

    #region Properties
    private const double MinScale = 1;
    private const double MaxScale = 4;
    private const double Overshoot = 0.15;
    private double _startScale;
    private double _lastX, _lastY;

    private double _width;
    private double _height;
    #endregion
}