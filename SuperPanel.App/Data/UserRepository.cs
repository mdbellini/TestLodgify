using Microsoft.Extensions.Options;
using SuperPanel.App.Infrastructure;
using SuperPanel.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SuperPanel.App.Data.Common;
using System;
using System.Threading.Tasks;

namespace SuperPanel.App.Data
{
    public interface IUserRepository
    {
        IEnumerable<User> QueryAll();
        PaginatedList<User> Query(string filter, string sortField, bool sortDesc, int pageNumber, int PageSize);
        Task<string[]> GDPRDeletion(int id);
    }

    public class UserRepository : IUserRepository
    {
        private List<User> _users;
        private readonly IExternalContactsRepository _externalContactsRepository;

        public UserRepository(IOptions<DataOptions> dataOptions, IExternalContactsRepository externalContactsRepository)
        {
            // preload the set of users from file.
            var json = System.IO.File.ReadAllText(dataOptions.Value.JsonFilePath);
            _users = JsonSerializer.Deserialize<IEnumerable<User>>(json)
                .ToList();
            _externalContactsRepository = externalContactsRepository;
        }

        public IEnumerable<User> QueryAll()
        {
            return _users;
        }

        /// <summary>
        /// Get a User from his ID
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User in case of exist</returns>
        private User GetById(int id)
        {
            return _users.FirstOrDefault(uq => uq.Id == id);
        }

        private void SetAnonymized(int id)
        {
            _users.First(uq => uq.Id == id).IsAnonymized = true;
        }

        /// <summary>
        /// Get a paginated list of user, conditionally filtered by email
        /// </summary>
        /// <param name="filter">Email/Name filter</param>
        /// <param name="sortField">Current Sort Order</param>
        /// <param name="sortDesc">Order Descending</param>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        public PaginatedList<User> Query(string filter, string sortField, bool sortDesc, int pageNumber, int pageSize)
        {
            var aUsers = _users.Where(uq => string.IsNullOrWhiteSpace(filter) ||
                                              uq.Email.Contains(filter, System.StringComparison.InvariantCultureIgnoreCase) ||
                                              uq.Login.Contains(filter, System.StringComparison.InvariantCultureIgnoreCase) ||
                                              uq.Phone.Contains(filter, System.StringComparison.InvariantCultureIgnoreCase) ||
                                              uq.FirstName.Contains(filter, System.StringComparison.InvariantCultureIgnoreCase) ||
                                              uq.LastName.Contains(filter, System.StringComparison.InvariantCultureIgnoreCase)
                                              )
                                     .ToArray();

            switch (sortField)
            {
                case "FirstName":
                    aUsers = sortDesc ? aUsers.OrderByDescending(uq => uq.FirstName).ThenByDescending(uq => uq.LastName).ToArray() : aUsers.OrderBy(uq => uq.FirstName).ThenBy(uq => uq.LastName).ToArray();
                    break;
                case "Email":
                    aUsers = sortDesc ? aUsers.OrderByDescending(uq => uq.Email).ToArray() : aUsers.OrderBy(uq => uq.Email).ToArray();
                    break;
                case "Login":
                    aUsers = sortDesc ? aUsers.OrderByDescending(uq => uq.Login).ToArray() : aUsers.OrderBy(uq => uq.Login).ToArray();
                    break;
                default:
                    aUsers = sortDesc ? aUsers.OrderByDescending(uq => uq.LastName).ThenByDescending(uq => uq.FirstName).ToArray() : aUsers.OrderBy(uq => uq.LastName).ThenBy(uq => uq.FirstName).ToArray();
                    break;
            }

            return PaginatedList<User>.Create(aUsers, pageNumber, pageSize);
        }

        public async Task<string[]> GDPRDeletion(int id)
        {
            List<string> errors = new List<string>();
            try
            {
                User user = GetById(id);
                if (user == null)
                    throw new ArgumentException("User not found");

                ExternalContact? externalContact = await _externalContactsRepository.AnonymizeExternalContact(user.Email);

                if (externalContact == null)
                    errors.Add("External Contact not found");
                else if (!externalContact.isAnonymized)
                    errors.Add("External Contact not Anonymized");
                else
                    SetAnonymized(id);

            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                Console.WriteLine($"Error in GDPRDeletion -> {ex}");
            }

            return errors.ToArray();
        }
    }
}
