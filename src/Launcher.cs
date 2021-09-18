using System;
using System.Windows.Forms;

namespace Exiclick.LaunchAnything
{
    internal class Launcher
    {
        public static void Launch(params string[] args)
        {
            try
            {
                var exePath = Application.ExecutablePath;

                var path = System.IO.Path.GetDirectoryName(exePath);

                var childInfo = Parse(exePath);

                var childPath = System.IO.Path.Combine(path, childInfo.FileName);

                var arguments = string.Join(" ", args);

                using (var client = new SAM.API.Client())
                {
                    if (childInfo.AppId > 0)
                    {
                        try
                        {
                            client.Initialize(childInfo.AppId);
                        }
                        catch (SAM.API.ClientInitializeException)
                        {
                            using (var notification = new NotificationWindow(childInfo))
                            {
                                var result = notification.ShowDialog();

                                if (result != DialogResult.Yes)
                                    return;
                            }
                        }
                        catch (DllNotFoundException)
                        {
                            MessageBox.Show(
                                "You've caused an exceptional error!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (!System.IO.File.Exists(childPath))
                        throw new System.IO.FileNotFoundException($"File not found: {childPath}");

                    var process = System.Diagnostics.Process.Start(childPath, arguments);

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static ChildInfo Parse(string path)
        {
            var retVal = new ChildInfo();

            var remainingFileName = System.IO.Path.GetFileName(path);

            do
            {
                var parts = remainingFileName.Split(new char[] { '_' }, 2);

                if (parts.Length == 1)
                {
                    retVal.FileName = parts[0];
                    break;
                }

                if (long.TryParse(parts[0], out long result))
                {
                    retVal.AppId = result;
                }

                remainingFileName = parts[1];
            } while (true);

            return retVal;
        }
    }
}