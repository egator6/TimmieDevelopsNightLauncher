using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Launcher.files
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
            UsernameLabel.Content = $"Logged In As: {Accounts.Username}!";
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);


        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);

        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hHandle);


        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        public static extern int SuspendThread(IntPtr hThread);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);
        public static void InjectDLL(int processId, string path)
        {
            IntPtr hProcess = OpenProcess(1082, false, processId);
            IntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            uint num = (uint)((path.Length + 1) * Marshal.SizeOf(typeof(char)));
            IntPtr num2 = VirtualAllocEx(hProcess, IntPtr.Zero, num, 12288U, 4U);
            UIntPtr uIntPtr;
            WriteProcessMemory(hProcess, num2, Encoding.Default.GetBytes(path), num, out uIntPtr);
            CreateRemoteThread(hProcess, IntPtr.Zero, 0U, procAddress, num2, 0U, IntPtr.Zero);
        }

        public Process StartProcess(string path, bool Freeze, bool IsNewest = false, string otherargs = "")
        {
            Process process = new Process(); // -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiZmY0YzEyMjQ5NzU5NGI5MGJlMDk1OWYxOGM2NWQwOGIiLCJnZW5lcmF0ZWQiOjE2NDEwOTI1NjUsImNhbGRlcmFHdWlkIjoiODQ0ODdkZmMtMGMxNC00YTUyLWFmYjgtNGY1ZWM5YzQyMjg0IiwiYWNQcm92aWRlciI6IkJhdHRsRXllIiwibm90ZXMiOiIiLCJmYWxsYmFjayI6ZmFsc2V9.E74n07NqNGmPPJ7NnK9EewIIb2Yjj3YP6Ghqrsd2iBe8e-z-ZkUiUwIH0DTd78yB5UDBDXdzOKBdsD0Mdjy5_A 
            process.StartInfo.FileName = path; //-http=wininet
            process.StartInfo.Arguments = $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiOWM1MDY1MTEwYzdhNGQ3MDk1ODYyZGE1ZWU4MTU5NjIiLCJnZW5lcmF0ZWQiOjE2NDc3ODMxMDcsImNhbGRlcmFHdWlkIjoiYmEwMmEyZWItZWU2NS00NjkxLWIwYWItNjUwMzE0ODRhMTQ3IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.U9a2eGUx9bSvc3fg-SQjr87O_vdxBC7GSfoUoIOxBDxeGFGQnSUABVt7lGA_Bq9d-s5mHQRWi6CfjWtUxxMTvA -skippatchcheck" + otherargs;
            process.Start();

            if (Freeze)
            {
                foreach (object obj in process.Threads)
                {
                    ProcessThread thread = (ProcessThread)obj;
                    var Thread = OpenThread(2, false, thread.Id);
                    if (Thread == IntPtr.Zero)
                    {
                        break;
                    }
                    SuspendThread(Thread);
                }
            }
            return process;
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Hosting_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                var LoginApi = $"http://127.0.0.1:5595/Night/routes/launcher/{Accounts.Username}/hosting";
                HttpResponseMessage ResponseMessage = await client.GetAsync(LoginApi);

                if (ResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Process process = StartProcess(Accounts.FortPath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe", true, false, "-NOSSLPINNING");
                    Process process1 = StartProcess(Accounts.FortPath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe", true, false, "");
                    Process process2 = StartProcess(Accounts.FortPath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe", false, false, "-AUTH_TYPE=epic -AUTH_LOGIN=Project_Night@Night.Net -AUTH_PASSWORD=Timmie123@");
                    process2.WaitForInputIdle();
                    InjectDLL(process2.Id, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Cobalt.dll"));
                }

                if (ResponseMessage.StatusCode == System.Net.HttpStatusCode.NotAcceptable)
                {
                    var ContentDialog = new ContentDialog()
                    {
                        Title = "Status Code: 406",
                        Content = "You do not have the permission to host!",
                        PrimaryButtonText = "OK"
                    };

                    await ContentDialog.ShowAsync();
                }
            }
        }
    }
}
