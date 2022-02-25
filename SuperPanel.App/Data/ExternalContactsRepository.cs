using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SuperPanel.App.Models;

namespace SuperPanel.App.Data
{
    public interface IExternalContactsRepository
    {
        Task<ExternalContact?> GetExternalContact(int id);
        Task<ExternalContact?> GetExternalContact(string email);
        Task<ExternalContact?> AnonymizeExternalContact(string email);
    }


    public class ExternalContactsRepository : IExternalContactsRepository
    {
        private readonly IHttpClientFactory _clientFactory;

        public ExternalContactsRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<ExternalContact?> GetExternalContact(int id)
        {
            var client = _clientFactory.CreateClient("ExternalContactsApi");
            var request = new HttpRequestMessage(HttpMethod.Get,$"/v1/contacts/{id}");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                ExternalContact contact = await JsonSerializer.DeserializeAsync<ExternalContact>(responseStream);
                return contact;
            }

            return null;
        }

        public async Task<ExternalContact?> GetExternalContact(string email)
        {
            var client = _clientFactory.CreateClient("ExternalContactsApi");
            var request = new HttpRequestMessage(HttpMethod.Get,$"/v1/contacts/{email}");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                ExternalContact contact = await JsonSerializer.DeserializeAsync<ExternalContact>(responseStream);
                return contact;
            }

            return null;
        }

        public async Task<ExternalContact?> AnonymizeExternalContact(string email)
        {
            ExternalContact? externalContact = await GetExternalContact(email);

            if (externalContact != null)
            {
                var client = _clientFactory.CreateClient("ExternalContactsApi");
                var request = new HttpRequestMessage(HttpMethod.Put, $"/v1/contacts/{externalContact.id}/gdpr");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    ExternalContact contact = await JsonSerializer.DeserializeAsync<ExternalContact>(responseStream);
                    return contact;
                }
            }

            return null;
        }
    }
}