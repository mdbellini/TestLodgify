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

        /// <summary>
        /// Search a Contact by it's id in the External Contact API
        /// </summary>
        /// <param name="id">External Contact's id</param>
        /// <returns>External Contact in case of exists</returns>
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

        /// <summary>
        /// Search a Contact by it's email in the External Contact API
        /// </summary>
        /// <param name="email">External Contact's email</param>
        /// <returns>External Contact in case of exists</returns>
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

        /// <summary>
        /// Make the anonymize call to external contacts API. Requires the email of the contact from search in external api
        /// </summary>
        /// <param name="email">Email of the local user for search in external API</param>
        /// <returns>The external contact with it's anonymized status, in case of exists in External API</returns>
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