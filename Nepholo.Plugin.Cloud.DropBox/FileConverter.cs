using System.Collections.Generic;
using System.Linq;
using DropNet.Models;

namespace Nepholo.Plugin.Cloud.DropBox
{
    public static class FileConverter
    {
        public static List<File> ToFiles(this List<MetaData> metaDatas)
        {
            return metaDatas.Select(ToFile).ToList();
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

        public static Account ToAccount(this AccountInfo accountInfo)
        {
            return new Account
            {
                Storage = new Storage
                {
                    Total = accountInfo.quota_info.quota,
                    Used = accountInfo.quota_info.normal,
                    Remaining = accountInfo.quota_info.quota - accountInfo.quota_info.normal
                },
                Email = accountInfo.email,
                Name = accountInfo.display_name
            };
        }

        // http://stackoverflow.com/a/16830804
    }
}