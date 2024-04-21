using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace PinWindow
{
    public partial class MainWindow : Window
    {
        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        public MainWindow()
        {
            InitializeComponent();
            LoadProcesses();
        }

        private void LoadProcesses()
        {
            Process[] processes = Process.GetProcesses();

            processListBox.Items.Clear();

            var sortedProcesses = processes.OrderBy(p => p.MainWindowTitle);

            foreach (var process in sortedProcesses)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    string processInfo = $"{NormalizeTitle(process.MainWindowTitle)} | ({process.WorkingSet64 / 1048576} mb) |{process.Id}";

                    processListBox.Items.Add(processInfo);

                }
            }

        }

        private void BringToFrontButton_Click(object sender, RoutedEventArgs e)
        {
            if (processListBox.SelectedItem != null)
            {
                string selectedProcessName = processListBox.SelectedItem.ToString();

                string[] name = selectedProcessName.Split('|');

                int proccesId = int.Parse(name[2]);

                Process selectedProcess = Process.GetProcessById(proccesId);

                IntPtr processWindowHandle = selectedProcess.MainWindowHandle;

                SetWindowPos(processWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProcesses();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (processListBox.SelectedItem != null)
            {
                string selectedProcessName = processListBox.SelectedItem.ToString();

                string[] name = selectedProcessName.Split('|');

                int processId = int.Parse(name[2]);

                Process selectedProcess = Process.GetProcessById(processId);

                IntPtr processWindowHandle = selectedProcess.MainWindowHandle;

                SetWindowPos(processWindowHandle, 1, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                SetWindowPos(processWindowHandle, 1, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.CapsLock && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;

                SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) == (ModifierKeys.Shift | ModifierKeys.Control))
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;

                SetWindowPos(handle, 1, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                SetWindowPos(handle, 1, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        private void searchbutton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = searchtextbox.Text.ToLower();

            processListBox.Items.Clear();

            Process[] processes = Process.GetProcesses();

            var sortedProcesses = processes.OrderBy(p => p.MainWindowTitle);

            foreach (var process in sortedProcesses)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    if (process.ProcessName.ToLower().Contains(searchTerm) || process.MainWindowTitle.ToLower().Contains(searchTerm))
                    {
                        string processInfo = $"{NormalizeTitle(process.MainWindowTitle)} | ({process.WorkingSet64 / 1048576} mb) |{process.Id}";

                        processListBox.Items.Add(processInfo);
                    }
                }
            }

        }

        private string NormalizeTitle(string title)
        {
            if (title.Contains("|"))
            {
                title = title.Replace('|', ' ');
            }
            return title;
        }

    }

}
