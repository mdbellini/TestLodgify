namespace SuperPanel.App.Models
{
    public class ExternalContact
    {
        public long id { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool isAnonymized { get; set; }
    }
}