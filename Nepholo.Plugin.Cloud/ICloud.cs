using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nepholo.Plugin.Cloud
{
    [InheritedExport(typeof (ICloud))]
    public interface ICloud
    {
        // Initialize a new Cloud for the first time
        void Create();

        // Initialize a new Client with nothing
        void Connect();

        // Remove Token
        void Deconnect();

        // Open a folder
        void OpenFolder();

        // Download something
        void Download();

        // Upload something
        void Upload();

        // List folder's content
        void Look();

        // Get user's informations
        void Identify();
    }
}
