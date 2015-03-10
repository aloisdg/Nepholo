using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using Nepholo.Model;
using Nepholo.Plugin.Cloud;
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
        [ImportMany(typeof(ICloud))]
        public IEnumerable<Lazy<ICloud>> GetICloud { get; set; }

        private IEnumerable<ICloud> _cloudType;
        private Window _window;

        public MainWindow()
        {
            InitializeComponent();

            _cloudType = new Collection<ICloud>();
            _window = new Window { ShowInTaskbar = false, Title = "Authentification" };

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
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var container = new CompositionContainer(new DirectoryCatalog(path, "*.dll"));
                container.ComposeParts(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillClouds();
        }

        private async void SwitchCloud()
        {
            if (App.Accounts.Any())
            {
                App.Cloud.Connect(App.Accounts.First().Tokens);
                ShowFiles();
            }
            else
            {
                IsEnabled = false;
                var url = await App.Cloud.GetOAuthToken();

                var wb = new WebBrowser();
                wb.LoadCompleted += wb_LoadCompleted;
                wb.Navigate(url);

                _window = new Window { ShowInTaskbar = false, Title = "Authentification", Content = wb, Owner = this };
                _window.Closing += w_Closing;
                _window.Show();
            }
        }

        private void FillClouds()
        {
            _cloudType = GetICloud.Where(element => element.Value != null).Select(e => e.Value);

            CloudBox.ItemsSource = _cloudType.Select(cloud => cloud.Name);
            CloudBox.SelectedIndex = 0;
        }

        private async void wb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (!e.Uri.Host.Contains("github")) return;
            await App.Cloud.Create(e.Uri.Query);
            Console.WriteLine("token ok");
            ShowFiles();
            _window.Close();
        }

        private void ShowFiles()
        {
            GetTree(null);
            DisplayContents(null);
        }

        private void w_Closing(object sender, CancelEventArgs e)
        {
            IsEnabled = true;

            AddAccount();
        }

        private static async void AddAccount()
        {
            var a = await App.Cloud.Identify();
            Console.WriteLine(a.Email);
            App.Accounts.Add(a);
        }

        private async void GetTree(string path, ItemsControl item = null)
        {
            List<File> list;
            if (String.IsNullOrWhiteSpace(path))
                list = await App.Cloud.GetRoot();
            else
                list = await App.Cloud.GetFolder(path);

            InitTree(list.Where(f => f.IsFolder), item);
        }

        private async void DisplayContents(string path)
        {
            List<File> list;
            if (String.IsNullOrWhiteSpace(path))
                list = await App.Cloud.GetRoot();
            else
                list = await App.Cloud.GetFolder(path);
            if (list != null)
                ItemListBox.ItemsSource = SortContents(list);
        }

        private static IEnumerable<File> SortContents(IEnumerable<File> contents)
        {
            return contents
                .OrderBy(c => !c.IsFolder)
                .ThenBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase);
        }

        private readonly object _dummyNode = null;
        public string SelectedImagePath { get; set; }

        void InitTree(IEnumerable<File> tree, ItemsControl tvi = null)
        {
            if (tvi == null)
                FoldersItem.Items.Clear();
            foreach (var item in tree.Select(s => new TreeViewItem { Header = s.Name, Tag = s.Id, FontWeight = FontWeights.Normal }))
            {
                item.Items.Add(_dummyNode);
                item.Expanded += ExpandFolder;
                if (tvi == null)
                    FoldersItem.Items.Add(item);
                else
                    tvi.Items.Add(item);
            }
        }

        void ExpandFolder(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item == null || (item.Items.Count != 1 || item.Items[0] != _dummyNode)) return;
            item.Items.Clear();
            try
            {
                GetTree(item.Tag.ToString(), item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;
            if (tree == null) return;
            var temp = tree.SelectedItem as TreeViewItem;
            if (temp == null) return;

            DisplayContents(temp.Tag.ToString());
        }

        private void CloudBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var box = CloudBox;
            if (box == null) return;
            App.Cloud = _cloudType.First(c => c.Name.Equals(box.SelectedItem as string));
            SwitchCloud();
        }

        private void CloseApp(object sender, CancelEventArgs e)
        {
            Helper.SerializeToXmlFile("accounts.xml", App.Accounts);
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                //GetTree(grid.Tag.ToString());
                //InitTree(grid.Tag.ToString()), item);
                DisplayContents(grid.Tag.ToString());
            }
        }
    }
}
