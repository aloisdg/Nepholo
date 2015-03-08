using System;
using System.Collections.Generic;
using System.Linq;

namespace Nepholo.Plugin.Cloud.OneDrive
{
    public static class FileConverter
    {
        public static List<File> ToFiles(this IEnumerable<OneDriveRestAPI.Model.File> files)
        {
            return files.Select(ToFile).ToList();
        }

        public static File ToFile(this OneDriveRestAPI.Model.File file)
        {
            var neo = new File
            {
                Id = file.Id,
                Name = file.Name,
                IsFolder = file.Type.Equals("folder"),
                Size = file.Size.ToReadableBytes(),
                Type = file.Type,
                Path = file.From.Id,
                Icon = file.Type.Equals("folder").ToIcon(),
                ModifiedDate = DateTime.Parse(file.Updated_Time)
            };
            if (!file.Type.Equals("folder") || file.Data == null || !file.Data.Any())
            {
                neo.Files = null;
                return neo;
            }
            foreach (var item in file.Data)
                neo.Files.Add(item.ToFile());
            return neo;
        }

        private static string ToIcon(this bool isFolder)
        {
            return isFolder
                ? "https://www.dropbox.com/static/images/icons128/folder.png"
                : "https://www.dropbox.com/static/images/icons128/page_white.png";
        }

        // http://stackoverflow.com/a/4975942
        private static string ToReadableBytes(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num) + suf[place];
        }
    }
}