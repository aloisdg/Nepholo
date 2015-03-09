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

        public async Task Create(string url)
        {
            var authCode = (HttpUtility.ParseQueryString(url)).Get("code");

            // Exchange the Authorization Code with Access/Refresh tokens
            var token = await _client.GetAccessTokenAsync(authCode);
        }

        public void Connect()
        {
            throw new NotImplementedException();
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
