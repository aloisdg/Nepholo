using DropNet;
using DropNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nepholo.Model.DropBox
{
    class DropBoxCloud : ICloud
    {
        readonly DropNetClient _client;
        private UserLogin _userLogin;

        public DropBoxCloud()
        {
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
                //GetTree("/");
            }
            else
            {

            }
        }

        public void GetTree(string path)
        {
            _client.GetMetaDataAsync(path, (response) =>
            {
                foreach (var item in response.Contents)
                    Console.WriteLine(item.Path);
            },
            (error) =>
            {
                Console.WriteLine(error.Message);
            });
        }

        public void DownloadFile(string cloudpath, string localpath)
        {
            _client.GetFileAsync(cloudpath,
            (response) =>
            {
                // Do something with response
                //var a = response.Name;
            },
            (error) =>
            {
                Console.WriteLine(error.Message);
            });
        }

        public void UploadFile(string cloudpath, string localpath, Stream content)
        {
            _client.UploadFileAsync(cloudpath, localpath, content,
            (response) =>
            {
                var a = response.Name;
                //Do something with response
            },
            (error) =>
            {
                Console.WriteLine(error.Message);
            });
        }

        public void DeleteFile(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateFolder(string path)
        {
            throw new NotImplementedException();
        }

        public void MoveFile(string path, string target)
        {
            throw new NotImplementedException();
        }
    }
}
