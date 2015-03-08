using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DropNet;
using DropNet.Models;

namespace Nepholo.Plugin.Cloud.DropBox
{
    public class DropBox : ICloud
    {
        private readonly DropNetClient _client = new DropNetClient(ApiKeys.DropKey, ApiKeys.DropSecret) { UseSandbox = false };
        private UserLogin _userLogin;

        public string GetOAuthToken()
        {
            //_client.GetTokenAsync(userLogin =>
            //{
            //    //Dispatcher.BeginInvoke(new ThreadStart(() =>
            //    //{
            //    //    WebPanel.Visibility = Visibility.Visible;
            //    //    WebPanel.IsHitTestVisible = true;
            //    //    MainBrowser.LoadCompleted += MainBrowser_LoadCompleted;
            //    //    MainBrowser.Navigate(tokenurl);
            //    //}));
            //},
            //error =>
            //{
            //    Console.WriteLine(error.Message);
            //});

            _client.GetToken();
            var tokenUrl = _client.BuildAuthorizeUrl("http://aloisdg.github.io/Nepholo/");
            return tokenUrl;

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

        public void Create(string url)
        {
            //_client.GetAccessTokenAsync(userLogin =>
            //{
            //    _userLogin = userLogin;

            //    //if ((MessageBox.Show("Save password?", "Important Question", MessageBoxButton.YesNo)) == MessageBoxResult.Yes)
            //    //{
            //    //    Settings.Default.Token = userLogin.Token;
            //    //    Settings.Default.Secret = userLogin.Secret;
            //    //    Settings.Default.Save();
            //    //}
            //    //GetTree("/");
            //    //DisplayContents("/");
            //},
            //error =>
            //{
            //    Console.WriteLine(error.Message);
            //});
            var accessToken = _client.GetAccessToken();
            _client.UserLogin = accessToken;

        }

        public void Connect()
        {
            _client.GetAccessTokenAsync(userLogin =>
            {
                _userLogin = userLogin;

                //if ((MessageBox.Show("Save password?", "Important Question", MessageBoxButton.YesNo)) == MessageBoxResult.Yes)
                //{
                //    Settings.Default.Token = userLogin.Token;
                //    Settings.Default.Secret = userLogin.Secret;
                //    Settings.Default.Save();
                //}
                //GetTree("/");
                //DisplayContents("/");
            },
            error =>
            {
                Console.WriteLine(error.Message);
            });
        }

        public void Deconnect()
        {
            throw new NotImplementedException();
        }

        public void Download(string id, string name)
        {
            throw new NotImplementedException();
        }

        public void Upload(string id, string name, Stream content)
        {
            throw new NotImplementedException();
        }

        public Task<List<File>> GetRoot()
        {
            const string root = "/";
            return GetFolder(root);
        }

        public Task<List<File>> GetFolder(string id)
        {
            //MetaData md = new MetaData();

            //    _client.GetMetaDataAsync(id,
            //        metaData =>
            //        {
            //            md = metaData;
            //            //Do something with MetaData
            //        },
            //        error =>
            //        {
            //            Console.WriteLine(error.Message);
            //        });

            //md = _client.GetMetaData(id);
            //return md.Contents.ToFiles();

            //var result = await Extensions.AsAsync<List<MetaData>>(GetMetaDataAsync);
            //return result;
            return Task.Run(() =>
            {
                var result = _client.GetMetaData(id);
                return result.Contents.ToFiles();
            });
        }

        Task<T> AsAsync<T>(Action<Action<T>> target)
        {
            var tcs = new TaskCompletionSource<T>();
            try
            {
                target(t => tcs.SetResult(t));
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public void Identify()
        {
            throw new NotImplementedException();
        }
    }

    public static class FileConverter
    {
        public static List<File> ToFiles(this List<MetaData> metaDatas)
        {
            return metaDatas.Select(md => md.ToFile()).ToList();
        }

        public static File ToFile(this MetaData file)
        {
            var neo = new File();
            neo.Id = file.Path;
            neo.Name = file.Name;
            neo.IsFolder = file.Is_Dir;
            neo.Size = file.Size;
            neo.Type = file.Icon;
            neo.Path = file.Path;
            neo.Icon = "https://www.dropbox.com/static/images/icons64/" + file.Icon + ".png";
            neo.ModifiedDate = file.ModifiedDate;
            if (file.Contents == null || !file.Contents.Any())
            {
                neo.Files = null;
                return neo;
            }
            foreach (var item in file.Contents)
                neo.Files.Add(item.ToFile());
            return neo;
        }

        // http://stackoverflow.com/a/16830804
    }
}
