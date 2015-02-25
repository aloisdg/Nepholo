using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Resources;
using DropNet;
using DropNet.Models;
using System.Threading;
using System.Security.Cryptography;

namespace Nepholo
{
    // todo
    // read https://medium.com/@jasonlong/generating-visual-designs-with-code-62e59c4881ca

    // ui
    // https://www.behance.net/gallery/17381539/Dropbox-UI-PSD
    // https://www.behance.net/gallery/20572615/Dropbox-Material-Design-2014
    // https://www.behance.net/gallery/23330969/Dropbox-Desktop

    // doc
    // http://dkdevelopment.net/what-im-doing/dropnet/
    // https://github.com/DropNet/DropNet
    // https://github.com/DropNet/DropNet.Samples

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DropNetClient _client;
        private UserLogin _userLogin;


        public MainWindow()
        {
            InitializeComponent();

            _client = new DropNetClient(AppResource.AppKey, AppResource.AppSecret);
            _client.UseSandbox = false;

            if (!(String.IsNullOrWhiteSpace(Properties.Settings.Default.Token)
                || String.IsNullOrWhiteSpace(Properties.Settings.Default.Secret)))
            {
                _client.UserLogin = new UserLogin
                {
                    Token = Properties.Settings.Default.Token,
                    Secret = Properties.Settings.Default.Secret
                };
                LoadContents();
            }
            else
            {
                Connect();
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        void Connect()
        {
            // step 1
            _client.GetTokenAsync((userLogin) =>
            {
                // step 2
                var tokenurl = _client.BuildAuthorizeUrl("http://www.google.com.au");
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    this.MainBrowser.LoadCompleted += MainBrowser_LoadCompleted;
                    this.MainBrowser.Navigate(tokenurl);
                }));
            },
            (error) =>
            {
                Console.WriteLine(error.Message);
            });

            //// step 3
            //_client.GetAccessTokenAsync((accessToken) =>
            //{
            //    //Store this token for "remember me" function
            //},
            //(error) =>
            //{
            //    //Handle error
            //});
        }

        void MainBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri.Host.Contains("google"))
            {
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    this.MainBrowser.Visibility = System.Windows.Visibility.Collapsed;
                }));

                _client.GetAccessTokenAsync((userLogin) =>
                {
                    _userLogin = userLogin;


                    var result1 = MessageBox.Show("Save password?", "Important Question", MessageBoxButton.YesNo);
                    if (result1 == MessageBoxResult.Yes)
                    {
                        Properties.Settings.Default.Token = userLogin.Token;
                        Properties.Settings.Default.Secret = userLogin.Secret;
                        Properties.Settings.Default.Save();
                    }

                    LoadContents();
                },
                (error) =>
                {
                    Console.WriteLine(error.Message);
                });
            }
        }

        private void LoadContents()
        {
            _client.GetMetaDataAsync("/", (response) =>
            {
                Console.WriteLine(String.Format("{1} folders found {0}{2} files found", Environment.NewLine
                    , response.Contents.Count(c => c.Is_Dir)
                    , response.Contents.Count(c => !c.Is_Dir)));

                foreach (var item in response.Contents)
                    Console.WriteLine(item.Path);


                //var a = response.Contents.First(dir => dir.Name == "SoundCloud");

                //LoadContents2(a.Path);

            },
            (error) =>
            {
                Console.WriteLine(error.Message);
            });
        }

        private void LoadContents2(string path)
        {
            _client.GetMetaDataAsync(path, (response) =>
            {
                //Console.WriteLine(String.Format("{1} folders found {0}{2} files found", Environment.NewLine
                //    , response.Contents.Count(c => c.Is_Dir)
                //    , response.Contents.Count(c => !c.Is_Dir)));

                foreach (var item in response.Contents)
                    Console.WriteLine(item.Path);
            },
            (error) =>
            {
                Console.WriteLine(error.Message);
            });
        }



        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            //LoadContents2(Path.Text);

        }
    }
}
