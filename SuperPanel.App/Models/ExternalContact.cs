namespace SuperPanel.App.Models
{
    public class ExternalContact
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsAnonymized { get; set; }
    }
}