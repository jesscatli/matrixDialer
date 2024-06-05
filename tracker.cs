/* Currently on Development 
Programmer: Jess S. Catli II
Date: 06/04/2024
Project Name: matrixPhone
Location: California, USA
*/
using System;
using System.Drawing;
using System.Windows.Forms;

public class MyIndicator : Control
{
    private static Color clrBg;
    private static Color clrFg;
    private static Color clrSFg;
    private const uint MaxScaleFactor = 10000;
    private bool logarithmic;
    private uint maximumLevel;
    private uint markerPosition;
    private uint currentPosition;
    private bool active;

    public MyIndicator()
    {
        logarithmic = false;
        maximumLevel = 32767;
        markerPosition = 0;
        currentPosition = 0;

        // some default colors
        clrBg = Color.FromArgb(0x00, 0x00, 0x00);
        clrFg = Color.FromArgb(0x19, 0x7B, 0x30);
        clrSFg = Color.FromArgb(0xC0, 0xC0, 0xC0);
    }

    public void Reset()
    {
        logarithmic = false;
        maximumLevel = 32767;
        markerPosition = 0;
        currentPosition = 0;
        Invalidate();
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
    }

    public void SetMaximumLevel(uint level)
    {
        if (maximumLevel == level)
            return;

        maximumLevel = level;
        markerPosition = ScaleLevel(markerPosition);
        currentPosition = ScaleLevel(currentPosition);
        Invalidate();
    }

    public void SetMarkerLevel(uint level)
    {
        uint position = ScaleLevel(level);

        if (markerPosition == position)
            return;

        markerPosition = position;
        Invalidate();
    }

    public void SetCurrentLevel(uint level)
    {
        uint position = ScaleLevel(level);

        if (currentPosition == position)
            return;

        currentPosition = position;
        Invalidate();
    }

    private uint ScaleLevel(uint level)
    {
        if (level > maximumLevel)
            level = maximumLevel;

        if (logarithmic)
            return level * MaxScaleFactor / maximumLevel;

        // If not logarithmic, then we have to make it so!
        return (uint)(Math.Log10(9.0 * level / maximumLevel + 1) * MaxScaleFactor);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics dc = e.Graphics;
        Color clrCur = active ? clrFg : clrSFg;

        Rectangle bounds = ClientRectangle;
        dc.DrawRectangle(Pens.Black, bounds);

        uint level = currentPosition * (uint)bounds.Width / MaxScaleFactor;

        Rectangle leftSide = bounds;
        leftSide.X += 1;
        leftSide.Width = (int)level + 1;
        leftSide.Y += 1;
        leftSide.Height -= 1;

        if (level > 0)
            dc.FillRectangle(new SolidBrush(clrCur), leftSide);

        if (markerPosition > 0)
        {
            uint mark = markerPosition * (uint)bounds.Width / MaxScaleFactor;
            if (mark < level)
                dc.FillRectangle(new SolidBrush(clrBg), (int)mark, leftSide.Y, 2, leftSide.Height - 1);
            else
                dc.FillRectangle(new SolidBrush(clrCur), (int)mark, leftSide.Y, 2, leftSide.Height - 1);
        }
    }
}
