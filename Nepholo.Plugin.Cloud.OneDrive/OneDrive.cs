using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OneDriveRestAPI;
using OneDriveRestAPI.Model;

namespace Nepholo.Plugin.Cloud.OneDrive
{
    public class OneDrive : ICloud
    {
        private static readonly Options Options = new Options
        {
            ClientId = ApiKeys.OneClient,
            ClientSecret = ApiKeys.OneSecret,
            CallbackUrl = "http://aloisdg.github.io/Nepholo/",

            AutoRefreshTokens = true,
            PrettyJson = false,
            ReadRequestsPerSecond = 2,
            WriteRequestsPerSecond = 2
        };

        private Client _client;

        public string Name
        {
            get { return "OneDrive"; }
        }

        public string Symbol
        {
            get { return ""; }
        }

        public string OAuthUrl { get; private set; }

        public Task<string> GetOAuthToken()
        {
            return Task.Run(() =>
            {
                // Initialize a new Client (without an Access/Refresh tokens)
                _client = new Client(Options);

                // Get the OAuth Request Url
                OAuthUrl = _client.GetAuthorizationRequestUrl(new[] { Scope.Basic, Scope.Signin, Scope.SkyDrive, Scope.SkyDriveUpdate });
                return OAuthUrl;
            });
        }

        public async Task<Tokens> Create(string url)
        {
            var authCode = (HttpUtility.ParseQueryString(url)).Get("code");

            // Exchange the Authorization Code with Access/Refresh tokens
            var tokens = await _client.GetAccessTokenAsync(authCode);
            return new Tokens { AccessToken = tokens.Access_Token, Token = tokens.Refresh_Token };
        }

        public void Connect(Tokens token)
        {
            // Initialize a new Client, this time by providing previously requested Access/Refresh tokens
            // var client2 = new Client(clientId, clientSecret, callbackUrl, token.Access_Token, token.Refresh_Token);
            //throw new NotImplementedException(); 
        }

        public void Deconnect()
        {
            throw new NotImplementedException();
        }

        public async Task Download(string id, string name)
        {
            using (var fileStream = System.IO.File.OpenWrite(name))
            {
                var contentStream = await _client.DownloadAsync(id);
                await contentStream.CopyToAsync(fileStream);
            }
        }

        public async Task Upload(string id, string name)
        {
            using (var fileStream = System.IO.File.OpenRead(name))
            {
                await _client.UploadAsync(id, fileStream, Name);
            }
        }

        public async Task Delete(string id)
        {
            await _client.DeleteAsync(id);
        }

        public async Task<List<File>> GetRoot()
        {
            var one = await _client.GetFolderAsync();
            Console.WriteLine(one.Name);
            var two = await _client.GetContentsAsync(one.Id);
            return two.ToFiles();
        }

        public async Task<List<File>> GetFolder(string id)
        {
            var folderContent = await _client.GetContentsAsync(id);
            return folderContent.ToFiles();
        }

        public async Task<Account> Identify()
        {
            var quota = await _client.QuotaAsync();
            Console.WriteLine(quota.Quota);
            var info = await _client.GetMeAsync();
            Console.WriteLine(info.Name);

            return info.ToAccount(quota);
        }
    }
}
