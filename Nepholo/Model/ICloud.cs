using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nepholo.Model
{
    interface ICloud
    {
        void GetTree(string path);
        void DownloadFile(string path, string file);
        void UploadFile(string path, string file, Stream fileStream);
        void DeleteFile(string path);
        void CreateFolder(string path);
        void MoveFile(string path, string target);
    }
}
