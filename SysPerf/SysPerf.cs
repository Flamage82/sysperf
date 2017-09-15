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

        public SysPerf()
        {
            this.notifyIcon.Visible = true;
            var menuItem = new MenuItem("Exit");
            menuItem.Click += this.MenuExitClick;
            this.notifyIcon.ContextMenu = new ContextMenu(new[] { menuItem });
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
            this.ExitThread();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var disk = Convert.ToInt32((float)(this.perfIdle.NextValue() / 100.0 * 14.0));
            var cpu = Convert.ToInt32((float)(this.perfCpu.NextValue() / 100.0 * 14.0));
            var bitmap = new Bitmap(IconDimension, IconDimension);
            var graphics = Graphics.FromImage(bitmap);
            graphics.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            graphics.FillRectangle(Brushes.Green, 1, 1 + cpu, 7, 14 - cpu);
            graphics.FillRectangle(Brushes.Red, 8, 1 + disk, 7, 14 - disk);
            var hicon = bitmap.GetHicon();
            var icon = (Icon)Icon.FromHandle(hicon).Clone();
            DestroyIcon(hicon);
            graphics.Dispose();
            bitmap.Dispose();
            this.notifyIcon.Icon = icon;
        }
    }
}