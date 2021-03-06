﻿using System.Collections.ObjectModel;
using System.Windows;
using Nepholo.Model;
using Nepholo.Plugin.Cloud;

namespace Nepholo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ObservableCollection<Account> Accounts { get; set; }
        public static ICloud Cloud { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Accounts = Helper.DeserializeFromXmlFile<ObservableCollection<Account>>("accounts.xml");

            //// add custom accent and theme resource dictionaries
            //ThemeManager.AddAccent("CustomAccent1", new Uri("pack://application:,,,/MahAppsMetroThemesSample;component/View/Theme.xaml"));

            //// get the theme from the current application
            //var theme = ThemeManager.DetectAppStyle(Application.Current);

            //// now use the custom accent
            //ThemeManager.ChangeAppStyle(Application.Current,
            //                        ThemeManager.GetAccent("CustomAccent1"),
            //                        theme.Item1);

            base.OnStartup(e);
        }
    }
}
