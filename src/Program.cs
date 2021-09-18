using System;
using System.Windows.Forms;

namespace Exiclick.LaunchAnything
{
    internal static class Program
    {
        [STAThread]
        private static void Main(params string[] args)
        {
            try
            {
                Launcher.Launch(args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}