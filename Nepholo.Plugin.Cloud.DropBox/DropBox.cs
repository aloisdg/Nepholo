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
        private readonly DropNetClient _client =
            new DropNetClient(ApiKeys.DropKey, ApiKeys.DropSecret) { UseSandbox = false };

        public string GetOAuthToken()
        {
            _client.GetToken();
            var tokenUrl = _client.BuildAuthorizeUrl("http://aloisdg.github.io/Nepholo/");
            return tokenUrl;
        }

        public Task Create(string url)
        {
            return Task.Run(() =>
            {
                var accessToken = _client.GetAccessToken();
                _client.UserLogin = accessToken;
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

        public void Connect()
        {
            _client.GetAccessTokenAsync(userLogin =>
            {
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
            return Task.Run(() =>
            {
                var result = _client.GetMetaData(id);
                return result.Contents.ToFiles();
            });
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
            var neo = new File
            {
                Id = file.Path,
                Name = file.Name,
                IsFolder = file.Is_Dir,
                Size = file.Size,
                Type = file.Icon,
                Path = file.Path,
                Icon = "https://www.dropbox.com/static/images/icons128/" + file.Icon + ".png",
                ModifiedDate = file.ModifiedDate
            };
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
