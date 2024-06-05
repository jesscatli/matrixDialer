using System;
using System.Drawing;
using System.Windows.Forms;

// MyTracker: Custom control for tracking levels with visual indicators.
public class MyTracker : Control
{
    // Colors for background, active foreground, and inactive foreground.
    private static readonly Color clrBg = Color.FromArgb(0x00, 0x00, 0x00);
    private static readonly Color clrFg = Color.FromArgb(0x19, 0x7B, 0x30);
    private static readonly Color clrSFg = Color.FromArgb(0xC0, 0xC0, 0xC0);
    
    // Maximum scale factor and control properties.
    private const uint MaxScaleFactor = 10000;
    private bool logarithmic;
    private uint maximumLevel = 32767;
    private uint markerPosition;
    private uint currentPosition;
    private bool active;

    // Constructor: Initializes default values and control properties.
    public MyTracker()
    {
        // Initialize default values
        logarithmic = false;
        markerPosition = 0;
        currentPosition = 0;
        active = false; // Assuming default state is inactive

        // Set initial control properties
        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    // Reset: Resets tracker to default values.
    public void Reset()
    {
        logarithmic = false;
        markerPosition = 0;
        currentPosition = 0;
        Invalidate();
    }

    // SetActive: Sets the active state of the tracker.
    public void SetActive(bool isActive)
    {
        active = isActive;
    }

    // SetMaximumLevel: Sets the maximum level of the tracker.
    public void SetMaximumLevel(uint level)
    {
        if (maximumLevel != level)
        {
            maximumLevel = level;
            markerPosition = ScaleLevel(markerPosition);
            currentPosition = ScaleLevel(currentPosition);
            Invalidate();
        }
    }

    // SetMarkerLevel: Sets the marker level of the tracker.
    public void SetMarkerLevel(uint level)
    {
        uint position = ScaleLevel(level);

        if (markerPosition != position)
        {
            markerPosition = position;
            Invalidate();
        }
    }

    // SetCurrentLevel: Sets the current level of the tracker.
    public void SetCurrentLevel(uint level)
    {
        uint position = ScaleLevel(level);

        if (currentPosition != position)
        {
            currentPosition = position;
            Invalidate();
        }
    }

    // ScaleLevel: Scales the level based on maximum level and logarithmic flag.
    private uint ScaleLevel(uint level)
    {
        if (level > maximumLevel)
            level = maximumLevel;

        return logarithmic ? level * MaxScaleFactor / maximumLevel : (uint)(Math.Log10(9.0 * level / maximumLevel + 1) * MaxScaleFactor);
    }

    // OnPaint: Overrides base method to paint the tracker control.
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics dc = e.Graphics;
        Color fillColor = active ? clrFg : clrSFg;

        Rectangle bounds = ClientRectangle;
        dc.DrawRectangle(Pens.Black, bounds);

        uint level = currentPosition * (uint)bounds.Width / MaxScaleFactor;

        Rectangle leftSide = bounds;
        leftSide.X += 1;
        leftSide.Width = (int)level + 1;
        leftSide.Y += 1;
        leftSide.Height -= 1;

        if (level > 0)
            dc.FillRectangle(new SolidBrush(fillColor), leftSide);

        if (markerPosition > 0)
        {
            uint mark = markerPosition * (uint)bounds.Width / MaxScaleFactor;
            dc.FillRectangle(new SolidBrush(mark < level ? clrBg : fillColor), (int)mark, leftSide.Y, 2, leftSide.Height - 1);
        }
    }
}
