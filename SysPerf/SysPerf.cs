namespace SysPerf
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class SysPerf : ApplicationContext
    {
        private const int IconDimension = 16;

        private readonly NotifyIcon notifyIcon = new NotifyIcon();

        private readonly PerformanceCounter perfCpu = new PerformanceCounter();

        private readonly PerformanceCounter perfIdle = new PerformanceCounter();

        private readonly Timer timer = new Timer();

        private readonly OverlayForm form;

        public SysPerf()
        {
            this.notifyIcon.Visible = true;

            var menuExitItem = new MenuItem("E&xit");
            menuExitItem.Click += this.MenuExitClick;

            var menuSwitchScreensItem = new MenuItem("&Switch Screen");
            menuSwitchScreensItem.Click += this.MenuSwitchScreensClick;

            this.notifyIcon.ContextMenu = new ContextMenu(new[] { menuExitItem, menuSwitchScreensItem });

            this.form = new OverlayForm();
            this.form.Show();
            this.form.Width = 102;
            this.form.Height = 23;

            this.perfIdle.CategoryName = "LogicalDisk";
            this.perfIdle.CounterName = "% Idle Time";
            this.perfIdle.InstanceName = "C:";
            this.perfCpu.CategoryName = "Processor";
            this.perfCpu.CounterName = "% Idle Time";
            this.perfCpu.InstanceName = "_Total";

            this.timer.Interval = 100;
            this.timer.Tick += this.TimerTick;
            this.timer.Start();
        }

        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Part of the Windows API")]
        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private void MenuExitClick(object sender, EventArgs e)
        {
            this.timer.Dispose();
            this.perfIdle.Dispose();
            this.notifyIcon.Dispose();
            this.form.Dispose();
            this.ExitThread();
        }

        private void MenuSwitchScreensClick(object sender, EventArgs e)
        {
            this.form.SwitchScreens();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var disk = this.perfIdle.NextValue() / 100.0;
            var cpu = this.perfCpu.NextValue() / 100.0;

            this.form.DrawPerfCounters(disk, cpu);

            var diskHeight = Convert.ToInt32((float)(disk * 14.0));
            var cpuHeight = Convert.ToInt32((float)(cpu * 14.0));
            var bitmap = new Bitmap(IconDimension, IconDimension);
            var graphics = Graphics.FromImage(bitmap);
            graphics.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            graphics.FillRectangle(Brushes.Green, 1, 1 + cpuHeight, 7, 14 - cpuHeight);
            graphics.FillRectangle(Brushes.Red, 8, 1 + diskHeight, 7, 14 - diskHeight);
            var hicon = bitmap.GetHicon();
            var icon = (Icon)Icon.FromHandle(hicon).Clone();
            DestroyIcon(hicon);
            graphics.Dispose();
            bitmap.Dispose();
            this.notifyIcon.Icon = icon;
        }
    }
}