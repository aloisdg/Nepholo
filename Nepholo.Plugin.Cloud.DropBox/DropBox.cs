using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DropNet;

namespace Nepholo.Plugin.Cloud.DropBox
{
    public class DropBox : ICloud
    {
        private readonly DropNetClient _client =
            new DropNetClient(ApiKeys.DropKey, ApiKeys.DropSecret) { UseSandbox = false };

        public string Name
        {
            get { return "DropBox"; }
        }

        public string Symbol
        {
            get { return ""; }
        }

        public Task<string> GetOAuthToken()
        {
            return Task.Run(() =>
            {
                _client.GetToken();
                var tokenUrl = _client.BuildAuthorizeUrl("http://aloisdg.github.io/Nepholo/");
                return tokenUrl;
            });
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
            _client.GetFileAsync(id,
            async response =>
            {
                using (var sourceStream = new FileStream(name,
                    FileMode.Append, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    var encodedText = Encoding.Unicode.GetBytes(response.Content);
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                }
            },
            error =>
            {
                Debug.WriteLine(error.Message);
                //Do something on error
            });
        }

        public void Upload(string id, string name)
        {
            using (var fileStream = System.IO.File.OpenRead(name))
            {
                //var uploaded = _client.UploadFile(id, System.IO.Path.GetFileName(name), fileStream);
                _client.UploadFileAsync(id, System.IO.Path.GetFileName(name), fileStream,
                    response =>
                    {
                        //Do something with response
                    },
                    error =>
                    {
                        Debug.WriteLine(error.Message);
                        //Do something on error
                    });
            }
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

        public Task<Account> Identify()
        {
            return Task.Run(() =>
            {
                var accountInfo = _client.AccountInfo();
                Console.WriteLine(accountInfo.display_name);
                Console.WriteLine(accountInfo.email);
                Console.WriteLine(accountInfo.quota_info);

                return accountInfo.ToAccount();
            });
        }
    }
}
