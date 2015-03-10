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
            var b = BoolBlock.Tag as bool? ?? false;
            if (b)
            {
                MessageBox.Show("Done Tomorrow");
                return;
            }

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
            MessageBox.Show("Done Tomorrow");
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var b = BoolBlock.Tag as bool? ?? false;
            if (b)
            {
                MessageBox.Show("Done Tomorrow");
                return;
            }

            // string path = Environment.SpecialFolder.UserProfile + @"\Downloads";
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var obj = ((ContextMenu)menuItem.Parent).PlacementTarget as StackPanel;
            if (obj == null) return;

            var name = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), TextName.Text);

            App.Cloud.Download(obj.Tag.ToString(), name).Wait();
            Console.WriteLine(name);
            Process.Start(name);
        }

        private void SaveTo(object sender, RoutedEventArgs e)
        {
            var b = BoolBlock.Tag as bool? ?? false;
            if (b)
            {
                MessageBox.Show("Done Tomorrow");
                return;
            }

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
            var b = BoolBlock.Tag as bool? ?? false;
            if (b)
            {
                MessageBox.Show("Done Tomorrow");
                return;
            }

            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var obj = ((ContextMenu)menuItem.Parent).PlacementTarget as StackPanel;
            if (obj == null) return;

            App.Cloud.Delete(obj.Tag.ToString());
        }
    }
}
