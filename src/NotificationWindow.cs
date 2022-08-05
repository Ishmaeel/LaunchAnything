using System;
using System.Windows.Forms;

namespace Exiclick.LaunchAnything
{
    internal partial class NotificationWindow : Form
    {
        private readonly TimeSpan GracePeriod = TimeSpan.FromSeconds(3);
        private DateTime? StartTime;

        internal NotificationWindow(ChildInfo childProcess)
        {
            InitializeComponent();

            ChildProcess = childProcess;
        }

        public ChildInfo ChildProcess { get; }

        private void Notification_Load(object sender, EventArgs e)
        {
            timer.Enabled = true;

            var grace = GracePeriod.TotalSeconds;

            labelHint.Text = labelHint.Text.Replace("{SEC}", grace.ToString());
            labelHint.Text = labelHint.Text.Replace("{CHILD}", ChildProcess?.FileName);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!StartTime.HasValue)
            {
                StartTime = DateTime.Now;
                progressBar.Maximum = (int)GracePeriod.TotalMilliseconds;
            }

            var elapsed = DateTime.Now - StartTime.Value;
            var progress = (int)elapsed.TotalMilliseconds;

            if (progress >= progressBar.Maximum)
            {
                DialogResult = DialogResult.Yes;
                progress = progressBar.Maximum;
            }

            progressBar.Value = progress;
        }

        private void ButtonNow_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            DialogResult = DialogResult.Yes;
        }
    }
}