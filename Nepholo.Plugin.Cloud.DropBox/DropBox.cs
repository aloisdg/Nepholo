using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DropNet;
using DropNet.Models;

namespace Nepholo.Plugin.Cloud.DropBox
{
    public class DropBox : ICloud
    {
        private DropNetClient _client =
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

        public Task<Tokens> Create(string url)
        {
            return Task.Run(() =>
            {
                var accessToken = _client.GetAccessToken();
                _client.UserLogin = accessToken;
                return new Tokens { AccessToken = accessToken.Token, Token = accessToken.Secret };
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

        public void Connect(Tokens tokens)
        {
            //_client.GetAccessTokenAsync(userLogin =>
            //{
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

            _client.UserLogin = new UserLogin { Token = tokens.AccessToken, Secret = tokens.Token };
        }

        public void Deconnect()
        {
            throw new NotImplementedException();
        }

        public Task Download(string id, string name)
        {
            return Task.Run(() =>
            {
                //_client.GetFileAsync(id,
                //    async response =>
                //    {
                //        using (var sourceStream = new FileStream(name,
                //            FileMode.Append, FileAccess.Write, FileShare.None,
                //            bufferSize: 4096, useAsync: true))
                //        {
                //            var encodedText = Encoding.Unicode.GetBytes(response.Content);
                //            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                //        }
                //    },
                //    error =>
                //    {
                //        Debug.WriteLine(error.Message);
                //        //Do something on error
                //    });

                // Sync
                var fileBytes = _client.GetFile(id);
                ByteArrayToFile(name, fileBytes);

            });
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        public Task Upload(string id, string name)
        {
            return Task.Run(() =>
            {
                using (var fileStream = System.IO.File.OpenRead(name))
                {
                    //var uploaded = _client.UploadFile(id, System.IO.Path.GetFileName(name), fileStream);
                    _client.UploadFileAsync(id, Path.GetFileName(name), fileStream,
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
            });
        }

        public Task Delete(string id)
        {
            return Task.Run(() =>
            {
                _client.DeleteAsync(id,
                    response =>
                    {
                        //Do something
                    },
                    error =>
                    {
                        Debug.WriteLine(error.Message);
                        //Do something on error
                    });
            });
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
