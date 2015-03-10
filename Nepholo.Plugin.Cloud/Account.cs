using System.Threading;

namespace Nepholo.Plugin.Cloud
{
    public class Account
    {
        public Storage Storage { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Tokens Tokens { get; set; }
    }
}
