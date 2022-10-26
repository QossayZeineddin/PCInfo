using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace PCInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.titleLabel.Content = "برنامج  لتفاصيل الجهاز والشاشة";
            this.details.Text += "Case Details تفاصيل الجهاز: " + '\n';

            String[,] caseDetails = {
                {"Brand المصَنِّع:\n", "wmic computersystem get manufacturer"},
                {"Model الموديل:\n", "wmic computersystem get model"},
                {"Model Number رقم الموديل:\n", "wmic computersystem get systemskunumber"},
                {"Serial Number الرقم التسلسلي:\n", "wmic bios get serialnumber"}
            };

            for (int i = 0; i < caseDetails.GetLength(0); i++)
            {
                String detailsLabel = caseDetails[i, 0];
                String detailsCommand = caseDetails[i, 1];

                String outp = ExecuteCommand(detailsCommand);
                outp = outp.Trim();
                var lines = Regex.Split(outp, "\r\n|\r|\n").Skip(1);
                outp = string.Join(Environment.NewLine, lines.ToArray());
                outp = outp.Trim();

                this.details.Text += detailsLabel + outp + '\n';
            }

            this.details.Text += "\nMonitor Details تفاصيل الشاشة: \n";

            String[,] monitorDetails = {
                {"Brand المصَنِّع:\n", "wmic desktopmonitor get monitormanufacturer"},
                {"Label التسمية:\n", "wmic desktopmonitor get caption"},
                {"Type النوع:\n", "wmic desktopmonitor get monitortype"},
                {"Name الاسم:\n", "wmic desktopmonitor get name"}
            };

            for (int i = 0; i < monitorDetails.GetLength(0); i++)
            {
                String detailsLabel = monitorDetails[i, 0];
                String detailsCommand = monitorDetails[i, 1];

                String outp = ExecuteCommand(detailsCommand);
                outp = outp.Trim();
                var lines = Regex.Split(outp, "\r\n|\r|\n").Skip(1);
                outp = string.Join(' ', lines.ToArray());
                outp = outp.Trim();

                this.details.Text += detailsLabel + outp + '\n';
            }

            String msnCommand = "Get-WmiObject WmiMonitorID -Namespace root\\wmi | ForEach-Object {$Serial = [System.Text.Encoding]::ASCII.GetString($_.SerialNumberID).Trim(0x00); \"{0}\" -f $Serial;}";
            String msn = ExecuteCommand(msnCommand, false);
            this.details.Text += "Serial Number الرقم التسلسلي:\n " + msn + '\n';
        }

        string ExecuteCommand(string command, bool cmd = true)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo((cmd ? "cmd.exe" : "powershell.exe"), "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

//            Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
//            Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
//            Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();

            return (String.IsNullOrEmpty(output) ? "(none)" : output);
        }
    }
}
