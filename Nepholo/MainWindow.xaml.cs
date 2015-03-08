using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DropNet;
using DropNet.Models;
using MahApps.Metro.Controls;
using Nepholo.Properties;

namespace Nepholo
{
    // todo
    // read https://medium.com/@jasonlong/generating-visual-designs-with-code-62e59c4881ca

    // ui
    // https://www.behance.net/gallery/17381539/Dropbox-UI-PSD
    // https://www.behance.net/gallery/20572615/Dropbox-Material-Design-2014
    // https://www.behance.net/gallery/23330969/Dropbox-Desktop

    // logo
    // icon by Ben
    // http://thenounproject.com/term/cloud/42868/

    // doc
    // http://dkdevelopment.net/what-im-doing/dropnet/
    // https://github.com/DropNet/DropNet
    // https://github.com/DropNet/DropNet.Samples

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        readonly DropNetClient _client;
        private UserLogin _userLogin;


        public MainWindow()
        {
            InitializeComponent();

            _client = new DropNetClient(ApiResource.AppKey, ApiResource.AppSecret) { UseSandbox = false };
            if (!(String.IsNullOrWhiteSpace(Settings.Default.Token)
                || String.IsNullOrWhiteSpace(Settings.Default.Secret)))
            {
                _client.UserLogin = new UserLogin
                {
                    Token = Settings.Default.Token,
                    Secret = Settings.Default.Secret
                };
                GetTree("/");
                DisplayContents("/");
            }
            else
            {
                Connect();
            }
        }

        void Connect()
        {
            _client.GetTokenAsync(userLogin =>
            {
                var tokenurl = _client.BuildAuthorizeUrl("http://www.google.com.au");
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    WebPanel.Visibility = Visibility.Visible;
                    WebPanel.IsHitTestVisible = true;
                    MainBrowser.LoadCompleted += MainBrowser_LoadCompleted;
                    MainBrowser.Navigate(tokenurl);
                }));
            },
            error =>
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
            if (!e.Uri.Host.Contains("google")) return;
            Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                WebPanel.Visibility = Visibility.Collapsed;
                WebPanel.IsHitTestVisible = false;
            }));

            _client.GetAccessTokenAsync(userLogin =>
            {
                _userLogin = userLogin;

                var result1 = MessageBox.Show("Save password?", "Important Question", MessageBoxButton.YesNo);
                if (result1 == MessageBoxResult.Yes)
                {
                    Settings.Default.Token = userLogin.Token;
                    Settings.Default.Secret = userLogin.Secret;
                    Settings.Default.Save();
                }
                GetTree("/");
                DisplayContents("/");
            },
                error =>
                {
                    Console.WriteLine(error.Message);
                });
        }

        //private void LoadContents()
        //{
        //    _client.GetMetaDataAsync("/", (response) =>
        //    {
        //        Console.WriteLine(String.Format("{1} folders found {0}{2} files found", Environment.NewLine
        //            , response.Contents.Count(c => c.Is_Dir)
        //            , response.Contents.Count(c => !c.Is_Dir)));

        //        foreach (var item in response.Contents)
        //            Console.WriteLine(item.Path);

        //    },
        //    (error) =>
        //    {
        //        Console.WriteLine(error.Message);
        //    });
        //}

        private void GetTree(string path, TreeViewItem item = null)
        {
            _client.GetMetaDataAsync(path, response =>
            {
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    if (response.Contents != null)
                        InitTree(from folder in response.Contents
                                 where folder.Is_Dir
                                 select folder, item);
                }));
            },
            error =>
            {
                Console.WriteLine(error.Message);
            });
        }

        private void DisplayContents(string path)
        {
            _client.GetMetaDataAsync(path, response =>
            {
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    if (response.Contents != null)
                        ItemListBox.ItemsSource = SortContents(response.Contents);
                }));
            },
           error =>
           {
               Console.WriteLine(error.Message);
           });
        }

        private IEnumerable<MetaData> SortContents(IEnumerable<MetaData> contents)
        {
            return contents
                .OrderBy(c => !c.Is_Dir)
                .ThenBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase);
        }

        private readonly object _dummyNode = null;
        public string SelectedImagePath { get; set; }

        void InitTree(IEnumerable<MetaData> tree, TreeViewItem tvi = null)
        {
            foreach (var item in tree.Select(s => new TreeViewItem
            { Header = s.Name, Tag = s.Path, FontWeight = FontWeights.Normal }))
            {
                item.Items.Add(_dummyNode);
                item.Expanded += folder_Expanded;
                if (tvi == null)
                    FoldersItem.Items.Add(item);
                else
                    tvi.Items.Add(item);
            }
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item == null || (item.Items.Count != 1 || item.Items[0] != _dummyNode)) return;
            item.Items.Clear();
            try
            {
                GetTree(item.Tag.ToString(), item);
            }
            catch (Exception) { }
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;
            if (tree == null) return;
            var temp = tree.SelectedItem as TreeViewItem;
            if (temp == null) return;

            DisplayContents(temp.Tag.ToString());
        }
    }
}
