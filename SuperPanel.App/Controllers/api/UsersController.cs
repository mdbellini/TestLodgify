using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperPanel.App.Data;
using SuperPanel.App.Data.Common;
using SuperPanel.App.Models;
using System;
using System.Threading.Tasks;

namespace SuperPanel.App.Controllers.api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;

        public UsersController(ILogger<UsersController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult<PaginatedList<User>> List(string filter = "", string sortBy = "", bool sortDesc = false, int page = 1, int size = 10)
        {
            try
            {
                filter = filter != "||" ? filter : string.Empty;
                sortBy = sortBy != "||" ? sortBy : string.Empty;

                var pagListUsers = _userRepository.Query(filter, sortBy, sortDesc, page, size);

                return pagListUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ListUser");
            }

            return NotFound();
        }

    }
}
