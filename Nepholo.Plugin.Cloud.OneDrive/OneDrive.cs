﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OneDriveRestAPI;
using OneDriveRestAPI.Model;

namespace Nepholo.Plugin.Cloud.OneDrive
{
    public class OneDrive : ICloud
    {
        #region readonly
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
        #endregion

        private Client _client;

        public string OAuthUrl { get; private set; }

        public string GetOAuthToken()
        {
            // Initialize a new Client (without an Access/Refresh tokens)
            _client = new Client(Options);

            // Get the OAuth Request Url
            OAuthUrl = _client.GetAuthorizationRequestUrl(new[] { Scope.Basic, Scope.Signin, Scope.SkyDrive, Scope.SkyDriveUpdate });
            return OAuthUrl;
        }

        public async void Create(string url)
        {

            if (!url.Contains("github")) return;

            // use System.Web
            var authCode = (HttpUtility.ParseQueryString(url)).Get("code");

            // Exchange the Authorization Code with Access/Refresh tokens
            var token = await _client.GetAccessTokenAsync(authCode);


            //this.IsEnabled = false;

            //var wb = new WebBrowser();
            //wb.LoadCompleted += wb_LoadCompleted;
            //wb.Navigate(tokenurl);

            //var w = new Window { Content = wb, ShowInTaskbar = false, Title = "Authentification", Owner = this };
            //w.Closing += w_Closing;
            //w.Show();
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
            var tmp = new List<File>();
            var rootFolder = await _client.GetFolderAsync().ContinueWith(async task =>
            {
                var folderContent = await _client.GetContentsAsync(task.Result.Id);
                return folderContent;
            });
            rootFolder.Wait();
            return tmp;
        }

        public async Task<List<File>> GetFolder(string id)
        {
            var folderContent = await _client.GetContentsAsync(id);
            return folderContent.ToFiles();
        }

        public void Identify()
        {
            throw new NotImplementedException();
        }
    }

    public static class FileConverter
    {
        public static List<File> ToFiles(this IEnumerable<OneDriveRestAPI.Model.File> files)
        {
            return files.Select(file => file.ToFile()).ToList();
        }

        public static File ToFile(this OneDriveRestAPI.Model.File file)
        {
            var neo = new File
            {
                Id = file.Id,
                Name = file.Name,
                IsFolder = file.Type.Equals("folder"),
                Size = file.Size.ToString(),
                Type = file.Type,
                Path = file.From.Id,
                Icon = file.Type.Equals("folder").ToIcon(),
                ModifiedDate = DateTime.Parse(file.Updated_Time)
            };
            foreach (var item in file.Data)
                neo.Files.Add(item.ToFile());
            return neo;
        }

        private static string ToIcon(this bool isFolder)
        {
            return isFolder
                ? "https://www.dropbox.com/static/images/icons128/folder.png"
                : "https://www.dropbox.com/static/images/icons64/page_white.png";
        }
    }
}