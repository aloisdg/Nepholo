namespace Nepholo.Plugin.Cloud
{
    public class Storage
    {
        public long Remaining { get; set; }
        public long Total { get; set; }
        public long Used { get; set; }
    }

    public class Account
    {
        public Storage Storage { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
