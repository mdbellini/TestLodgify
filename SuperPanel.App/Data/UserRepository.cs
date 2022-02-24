using Microsoft.Extensions.Options;
using SuperPanel.App.Infrastructure;
using SuperPanel.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SuperPanel.App.Data.Common;

namespace SuperPanel.App.Data
{
    public interface IUserRepository
    {
        IEnumerable<User> QueryAll();
        PaginatedList<User> Query(string filter, string sortField, bool sortDesc, int pageNumber, int PageSize);
    }

    public class UserRepository : IUserRepository
    {
        private List<User> _users;

        public UserRepository(IOptions<DataOptions> dataOptions)
        {
            // preload the set of users from file.
            var json = System.IO.File.ReadAllText(dataOptions.Value.JsonFilePath);
            _users = JsonSerializer.Deserialize<IEnumerable<User>>(json)
                .ToList();
        }

        public IEnumerable<User> QueryAll()
        {
            return _users;
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
    }
}
