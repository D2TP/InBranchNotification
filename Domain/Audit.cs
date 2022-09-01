namespace InBranchNotification.Domain
{
    public class Audit
    {
        public string activity { get; set; }
        public string mac_address { get; set; }
        public string ip_address { get; set; }
        public string client_browser { get; set; }
        public string activity_module { get; set; }
        public string activity_submodule { get; set; }
        public string inb_aduser_id { get; set; }
    }
}
