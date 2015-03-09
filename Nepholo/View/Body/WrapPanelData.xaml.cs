using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Win32;

namespace Nepholo.View.Body
{
    /// <summary>
    /// Interaction logic for WrapPanelData.xaml
    /// </summary>
    public partial class WrapPanelData : UserControl
    {
        public WrapPanelData()
        {
            InitializeComponent();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var obj = ((ContextMenu)menuItem.Parent).PlacementTarget as StackPanel;
            if (obj == null) return;
            var name = System.IO.Path.Combine(System.IO.Path.GetTempPath(), TextName.Text);

            App.Cloud.Download(obj.Tag.ToString(), name).Wait();
            Console.WriteLine(name);
            Process.Start(name);
        }

        private void Look(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SaveTo(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var obj = ((ContextMenu)menuItem.Parent).PlacementTarget as StackPanel;
            if (obj == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = TextName.Text;
            saveFileDialog.ShowDialog();
            var name = saveFileDialog.FileName;

            App.Cloud.Download(obj.Tag.ToString(), name).Wait();
            Console.WriteLine(name);
            Process.Start(name);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var obj = ((ContextMenu)menuItem.Parent).PlacementTarget as StackPanel;
            if (obj == null) return;

            App.Cloud.Delete(obj.Tag.ToString());
        }
    }
}
