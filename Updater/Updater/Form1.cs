using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Updater
{
    public partial class Form1 : Form
    {
        private bool isRunning = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ExecuteCommand();
        }

        private void ExecuteCommand()
        {
            try
            {
                // PowerShell скрипт для обновления папки
                string script = @"
Start-Process explorer.exe '\\veyseloglu.az'
Start-Sleep -Seconds 2
$Shell = New-Object -ComObject Shell.Application
$Folder = $Shell.Namespace('\\veyseloglu.az')
$Folder.Self.InvokeVerb('Refresh')
";

                // Создание временного файла для выполнения скрипта
                string tempScriptPath = Path.GetTempFileName() + ".ps1";
                File.WriteAllText(tempScriptPath, script);

                // Запуск PowerShell-скрипта
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{tempScriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(processStartInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // Логирование вывода и ошибок
                    File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "process_log.txt"), output + error);

                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show($"PowerShell скрипт не выполнен успешно. Ошибка: {error}");
                    }
                }

                // Удаление временного файла
                File.Delete(tempScriptPath);
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "error_log.txt"), ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                int minutes = (int)numericUpDown1.Value;

                // Конвертация минут в миллисекунды
                int interval = minutes * 60 * 1000;
                timer1.Interval = interval;
                timer1.Start();
                button1.Text = "Stop";
                isRunning = true;
            }
            else
            {
                timer1.Stop();
                button1.Text = "Start";
                isRunning = false;
            }
        }
    }
}
