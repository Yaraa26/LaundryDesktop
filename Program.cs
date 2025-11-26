using System;
using System.Windows.Forms;

namespace DesktopLaundry
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Your login form here
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application error: {ex.Message}", "Critical Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}