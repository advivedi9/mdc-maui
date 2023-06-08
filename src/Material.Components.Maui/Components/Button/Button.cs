﻿namespace Material.Components.Maui;

public class Button
    : TouchGraphicView,
        IElement,
        ITextElement,
        IBackgroundElement,
        IShapeElement,
        IStateLayerElement,
        IRippleElement,
        IContextMenuElement,
        IIconElement,
        IOutlineElement,
        IElevationElement
{
    protected override void ChangeVisualState()
    {
        this.IsVisualStateChanging = true;
        var state = this.ViewState switch
        {
            ViewState.Normal => "normal",
            ViewState.Hovered => "hovered",
            ViewState.Pressed => "pressed",
            ViewState.Disabled => "disabled",
            _ => "normal",
        };

        VisualStateManager.GoToState(this, state);
        this.IsVisualStateChanging = false;

        this.Invalidate();
    }

    public static readonly BindableProperty TextProperty = ITextElement.TextProperty;
    public static readonly BindableProperty TextColorProperty = ITextElement.TextColorProperty;
    public static readonly BindableProperty FontSizeProperty = ITextElement.FontSizeProperty;
    public static readonly BindableProperty FontFamilyProperty = ITextElement.FontFamilyProperty;
    public static readonly BindableProperty FontSlantProperty = ITextElement.FontSlantProperty;
    public static readonly BindableProperty FontWeightProperty = ITextElement.FontWeightProperty;

    public static readonly BindableProperty IconDataProperty = IIconElement.IconDataProperty;
    public static readonly BindableProperty IconColorProperty = IIconElement.IconColorProperty;

    public static readonly BindableProperty OutlineWidthProperty =
        IOutlineElement.OutlineWidthProperty;
    public static readonly BindableProperty OutlineColorProperty =
        IOutlineElement.OutlineColorProperty;
    public static readonly BindableProperty ElevationProperty = IElevationElement.ElevationProperty;

    public string Text
    {
        get => (string)this.GetValue(TextProperty);
        set => this.SetValue(TextProperty, value);
    }
    public Color TextColor
    {
        get => (Color)this.GetValue(TextColorProperty);
        set => this.SetValue(TextColorProperty, value);
    }
    public float FontSize
    {
        get => (float)this.GetValue(FontSizeProperty);
        set => this.SetValue(FontSizeProperty, value);
    }
    public string FontFamily
    {
        get => (string)this.GetValue(FontFamilyProperty);
        set => this.SetValue(FontFamilyProperty, value);
    }
    public FontSlant FontSlant
    {
        get => (FontSlant)this.GetValue(FontSlantProperty);
        set => this.SetValue(FontSlantProperty, value);
    }
    public FontWeight FontWeight
    {
        get => (FontWeight)this.GetValue(FontWeightProperty);
        set => this.SetValue(FontWeightProperty, value);
    }

    public string IconData
    {
        get => (string)this.GetValue(IconDataProperty);
        set => this.SetValue(IconDataProperty, value);
    }

    PathF IIconElement.IconPath { get; set; }

    public Color IconColor
    {
        get => (Color)this.GetValue(IconColorProperty);
        set => this.SetValue(IconColorProperty, value);
    }
    public Color OutlineColor
    {
        get => (Color)this.GetValue(OutlineColorProperty);
        set => this.SetValue(OutlineColorProperty, value);
    }

    public int OutlineWidth
    {
        get => (int)this.GetValue(OutlineWidthProperty);
        set => this.SetValue(OutlineWidthProperty, value);
    }
    public Elevation Elevation
    {
        get => (Elevation)this.GetValue(ElevationProperty);
        set => this.SetValue(ElevationProperty, value);
    }

    static Style defaultStyle;

    public Button()
    {
        this.Style = defaultStyle ??= ResourceExtension.MaterialDictionaries
            .First(x => x.GetType() == typeof(ButtonStyles))
            .FindStyle("FilledButtonStyle");

        this.Drawable = new ButtonDrawable(this);
        this.EndInteraction += this.OnEndInteraction;
    }

    void OnEndInteraction(object sender, TouchEventArgs e)
    {
        this.Command?.Execute(this.CommandParameter);
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var maxWidth = Math.Min(
            Math.Min(widthConstraint, this.MaximumWidthRequest),
            this.WidthRequest != -1 ? this.WidthRequest : double.PositiveInfinity
        );
        var maxHeight = Math.Min(
            Math.Min(heightConstraint, this.MaximumHeightRequest),
            this.HeightRequest != -1 ? this.HeightRequest : double.PositiveInfinity
        );

        var scale =
            (this.HeightRequest != -1 ? this.HeightRequest : Math.Min(40f, maxHeight)) / 40f;
        var iconSize = (!string.IsNullOrEmpty(this.IconData) ? 18f : 0f) * scale;
        var textSize = this.GetStringSize();
        //18 + iconSize + 6 + textSize.Width + 24
        var needWidth = 48f * scale + iconSize + textSize.Width;
        var needHeight = 40f * scale;

        var width =
            this.HorizontalOptions.Alignment == LayoutAlignment.Fill
                ? maxWidth
                : this.Margin.HorizontalThickness
                    + Math.Max(
                        this.MinimumWidthRequest,
                        this.WidthRequest == -1 ? Math.Min(maxWidth, needWidth) : this.WidthRequest
                    );
        var height =
            this.VerticalOptions.Alignment == LayoutAlignment.Fill
                ? maxHeight
                : this.Margin.VerticalThickness
                    + Math.Max(
                        this.MinimumHeightRequest,
                        this.HeightRequest == -1
                            ? Math.Min(maxHeight, needHeight)
                            : this.HeightRequest
                    );

        this.DesiredSize = new Size(Math.Ceiling(width), Math.Ceiling(height));
        return this.DesiredSize;
    }

    protected override void Dispose(bool disposing)
    {
        if (!this.disposedValue && disposing)
        {
            this.EndInteraction -= this.OnEndInteraction;
            ((IIconElement)this).IconPath?.Dispose();
        }
        base.Dispose(disposing);
    }
}
