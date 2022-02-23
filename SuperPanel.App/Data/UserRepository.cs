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
        PaginatedList<User> Query(string filter, int pageNumber, int PageSize);
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
        /// <param name="filter">Email filter</param>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        public PaginatedList<User> Query(string filter, int pageNumber, int pageSize)
        {
            var aUsers = _users.Where(uq => string.IsNullOrWhiteSpace(filter) ||
                                              uq.Email.Contains(filter))
                                     .ToArray();
            
            return PaginatedList<User>.Create(aUsers, pageNumber, pageSize);
        }
    }
}
