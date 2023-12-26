using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Launcher.files;
using ModernWpf.Controls;
using Newtonsoft.Json.Linq;
using Simplify.Windows.Forms;
namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            EmailBox.Text = File.ReadAllText("Credentials\\Email.txt");
            PasswordBox.Password = File.ReadAllText("Credentials\\Password.txt");
            PathBox.Text = File.ReadAllText("Credentials\\FortnitePath.txt");
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                var LoginApi = $"http://127.0.0.1:5595/Night/routes/launcher/login/{EmailBox.Text}/{PasswordBox.Password}";
                //https://stackoverflow.com/questions/39468096/how-can-i-parse-json-string-from-httpclient
                HttpResponseMessage ResponseMessage = await client.GetAsync(LoginApi);
                var ResponseContent = ResponseMessage.Content.ReadAsStringAsync().Result;

                if (ResponseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    var ContentDialog = new ContentDialog()
                    {
                        Title = "Status: 403",
                        Content = "Login failed: Your account has been banned.",
                        PrimaryButtonText = "OK"
                    };

                    await ContentDialog.ShowAsync();
                }

                if (ResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var JsonMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(ResponseContent);

                    Accounts.Username = JsonMessage["data"]["username"].ToString();
                    Accounts.Email = EmailBox.Text;
                    Accounts.Password = PasswordBox.Password;
                    Accounts.FortPath = PathBox.Text;

                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Cobalt.dll")))
                    {
                        var ContentDialog = new ContentDialog()
                        {
                            Title = "Error",
                            Content = "The 'Cobalt.dll' is not there. Please check if it exists in your files.",
                            PrimaryButtonText = "OK"
                        };

                        await ContentDialog.ShowAsync();
                        return;
                    }

                    File.WriteAllText("Credentials\\Email.txt", EmailBox.Text);
                    File.WriteAllText("Credentials\\FortnitePath.txt", PathBox.Text);
                    File.WriteAllText("Credentials\\Password.txt", PasswordBox.Password);

                    this.Hide();
                    Home home = new Home();
                    home.Show();
                }

                if (ResponseMessage.StatusCode == HttpStatusCode.NotAcceptable)
                {
                    var ContentDialog = new ContentDialog()
                    {
                        Title = "Status: 406",
                        Content = "Login failed: Incorrect password.",
                        PrimaryButtonText = "OK"
                    };

                    await ContentDialog.ShowAsync();
                }

                if (ResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    var ContentDialog = new ContentDialog()
                    {
                        Title = "Status: 404",
                        Content = "Login failed: Your email is incorrect.",
                        PrimaryButtonText = "OK"
                    };

                    await ContentDialog.ShowAsync();
                }

            }
        }

        private async void Path_Click(object sender, RoutedEventArgs e)
        {
            var Dialog = new FolderBrowserDialog()
            {
                Description = "Select Fortnite Folder!",
            };

            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathBox.Text = Dialog.SelectedPath;
            }
            else
            {
                var ContentDialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = "Please Select A Fortnite Folder!",
                    PrimaryButtonText = "OK"
                };

                await ContentDialog.ShowAsync();
            }
        }
    }
}