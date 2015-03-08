using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DropNet;
using DropNet.Models;
using MahApps.Metro.Controls;
using Nepholo.Plugin.Cloud;
using Nepholo.Properties;
using File = Nepholo.Plugin.Cloud.File;

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

        [ImportMany(typeof(ICloud))]
        public IEnumerable<Lazy<ICloud>> GetICloud { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            CloudType = new Collection<ICloud>();

            //_client = new DropNetClient(ApiResource.DropKey, ApiResource.DropSecret) { UseSandbox = false };
            //if (!(String.IsNullOrWhiteSpace(Settings.Default.Token)
            //    || String.IsNullOrWhiteSpace(Settings.Default.Secret)))
            //{
            //    _client.UserLogin = new UserLogin
            //    {
            //        Token = Settings.Default.Token,
            //        Secret = Settings.Default.Secret
            //    };
            //    GetTree("/");
            //    DisplayContents("/");
            //}
            //else
            //{
            //    Connect();
            //}

            try
            {
                // http://stackoverflow.com/a/6753604
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

                //Check the directory exists
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //Create an assembly catalog of the assemblies with exports
                //var catalog = new AggregateCatalog(
                //    new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                //    new AssemblyCatalog(Assembly.Load("My.Other.Assembly")),
                //    new DirectoryCatalog(path, "*.dll"));

                //Create a composition container
                var container = new CompositionContainer(new DirectoryCatalog(path, "*.dll"));
                container.ComposeParts(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Loaded += MainWindow_Loaded;

        }

        private Collection<ICloud> CloudType;
        private ICloud _cloud;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var element in GetICloud.Where(element => element.Value != null))
                CloudType.Add(element.Value as ICloud);

            _cloud = CloudType.First();
            var url = _cloud.GetOAuthToken();

            IsEnabled = false;

            var wb = new WebBrowser();
            wb.LoadCompleted += wb_LoadCompleted;
            wb.Navigate(url);

            var w = new Window { Content = wb, ShowInTaskbar = false, Title = "Authentification", Owner = this };
            w.Closing += w_Closing;
            w.Show();
        }

        void wb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (!e.Uri.Host.Contains("github")) return;
            _cloud.Create(e.Uri.Query);
            //GetTree(null);
        }

        void w_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsEnabled = true;
        }

        private async void GetTree(string path, TreeViewItem item = null)
        {

            List<File> list;
            if (String.IsNullOrWhiteSpace(path))
                list = await _cloud.GetRoot();
            else
                list = await _cloud.GetFolder(path);

            InitTree(list.Where(f => f.IsFolder), item);

        }

        private async void DisplayContents(string path)
        {
            var files = await _cloud.GetFolder(path);

            if (files != null)
                ItemListBox.ItemsSource = SortContents(files);
        }

        #region old
        //void Connect()
        //{
        //    _client.GetTokenAsync(userLogin =>
        //    {
        //        var tokenurl = _client.BuildAuthorizeUrl("http://aloisdg.github.io/Nepholo/");
        //        Dispatcher.BeginInvoke(new ThreadStart(() =>
        //        {
        //            //WebPanel.Visibility = Visibility.Visible;
        //            //WebPanel.IsHitTestVisible = true;
        //            //MainBrowser.LoadCompleted += MainBrowser_LoadCompleted;
        //            //MainBrowser.Navigate(tokenurl);
        //        }));
        //    },
        //    error =>
        //    {
        //        Console.WriteLine(error.Message);
        //    });
        //}

        //void MainBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        //{
        //    if (!e.Uri.Host.Contains("github")) return;
        //    Dispatcher.BeginInvoke(new ThreadStart(() =>
        //    {
        //        //WebPanel.Visibility = Visibility.Collapsed;
        //        //WebPanel.IsHitTestVisible = false;
        //    }));

        //    _client.GetAccessTokenAsync(userLogin =>
        //    {
        //        _userLogin = userLogin;

        //        if ((MessageBox.Show("Save password?", "Important Question", MessageBoxButton.YesNo)) == MessageBoxResult.Yes)
        //        {
        //            Settings.Default.Token = userLogin.Token;
        //            Settings.Default.Secret = userLogin.Secret;
        //            Settings.Default.Save();
        //        }
        //        GetTree("/");
        //        DisplayContents("/");
        //    },
        //        error =>
        //        {
        //            Console.WriteLine(error.Message);
        //        });
        //}

        //private void GetTree(string path, TreeViewItem item = null)
        //{
        //    var md = new List<MetaData>();
        //    //_client.GetMetaDataAsync(path, response => md = response.Contents ?? md,
        //    //error =>
        //    //{
        //    //    Console.WriteLine(error.Message);
        //    //    return;
        //    //});

        //    md = _client.GetMetaData(path).Contents; //Folder

        //    Dispatcher.BeginInvoke(new ThreadStart(() =>
        //    {
        //        //InitTree(from folder in md
        //        //         where folder.Is_Dir
        //        //         select folder, item);
        //    }));
        //}

        //private void DisplayContents(string path)
        //{
        //    _client.GetMetaDataAsync(path, response =>
        //    {
        //        Dispatcher.BeginInvoke(new ThreadStart(() =>
        //        {
        //            if (response.Contents != null)
        //                ItemListBox.ItemsSource = SortContents(response.Contents);
        //        }));
        //    },
        //   error =>
        //   {
        //       Console.WriteLine(error.Message);
        //   });
        //}

        #endregion

        private IEnumerable<Nepholo.Plugin.Cloud.File> SortContents(IEnumerable<Nepholo.Plugin.Cloud.File> contents)
        {
            return contents
                .OrderBy(c => !c.IsFolder)
                .ThenBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase);
        }

        private readonly object _dummyNode = null;
        public string SelectedImagePath { get; set; }

        void InitTree(IEnumerable<Nepholo.Plugin.Cloud.File> tree, TreeViewItem tvi = null)
        {
            foreach (var item in tree.Select(s => new TreeViewItem { Header = s.Name, Tag = s.Id, FontWeight = FontWeights.Normal }))
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            GetTree(null);
            DisplayContents("/");
        }
    }
}
