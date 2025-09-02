using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public class ColoredProgressBar : ProgressBar
    {
        public Color ProgressColor { get; set; } = Color.Green;

        public ColoredProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = e.ClipRectangle;
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), rect);
            if (this.Maximum > 0)
            {
                rect.Width = (int)(rect.Width * ((double)this.Value / this.Maximum));
                using var brush = new SolidBrush(ProgressColor);
                e.Graphics.FillRectangle(brush, 0, 0, rect.Width, rect.Height);
            }
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, this.Width - 1, this.Height - 1);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
    }
}
