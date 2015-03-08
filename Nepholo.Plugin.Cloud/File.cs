using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nepholo.Plugin.Cloud
{
    public class File
    {
       public string Id { get; set; }
       public string Name { get; set; }
       public bool IsFolder { get; set; }
       public string Size { get; set; }
       public DateTime ModifiedDate { get; set; }
       public string Type { get; set; }
       public List<File> Files { get; set; }
       public string Path { get; set; }
       public string Icon { get; set; }
    }
}
