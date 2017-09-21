namespace SysPerf
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class OverlayForm : Form
    {
        private int displayIndex;

        public OverlayForm()
        {
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.AllowTransparency = true;
            this.TransparencyKey = Color.Purple;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            this.SetLocation();
        }

        public override Color BackColor { get; set; } = Color.Purple;

        public void SwitchScreens()
        {
            this.displayIndex++;
            this.SetLocation();
        }

        public void DrawPerfCounters(double disk, double cpu)
        {
            var diskWidth = 100 - Convert.ToInt32(disk * 100);
            var cpuWidth = 100 - Convert.ToInt32(cpu * 100);

            var graphics = this.CreateGraphics();
            graphics.Clear(Color.Purple);
            graphics.DrawRectangle(Pens.Black, 0, -1, 101, 23);
            graphics.FillRectangle(Brushes.Green, 1, 0, cpuWidth, 11);
            graphics.FillRectangle(Brushes.Red, 1, 11, diskWidth, 11);
        }

        private void SetLocation()
        {
            if (this.displayIndex >= Screen.AllScreens.Length)
            {
                this.displayIndex = 0;
            }

            var screen = Screen.AllScreens[this.displayIndex];
            this.Location = new Point(screen.Bounds.X + screen.Bounds.Width - 482, screen.Bounds.Y);
        }
    }
}