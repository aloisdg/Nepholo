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

namespace Nepholo
{
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // step 1
            // Async
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
                _client.GetAccessTokenAsync((userLogin) =>
                {
                    _userLogin = userLogin;

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


                var a = response.Contents.First(dir => dir.Name == "SoundCloud");

                LoadContents2(a.Path);
                
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
                Console.WriteLine(String.Format("{1} folders found {0}{2} files found", Environment.NewLine
                    , response.Contents.Count(c => c.Is_Dir)
                    , response.Contents.Count(c => !c.Is_Dir)));

                foreach (var item in response.Contents)
                    Console.WriteLine(item.Path);
            },
            (error) =>
            {
                Console.WriteLine(error.Message);
            });
        }
    }
}
