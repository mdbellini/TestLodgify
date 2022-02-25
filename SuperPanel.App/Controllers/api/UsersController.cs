using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperPanel.App.Data;
using SuperPanel.App.Data.Common;
using SuperPanel.App.Models;
using System;
using System.Linq;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<bool>> GDPRDeletion(int id)
        {
            try
            {
                var delete_result = await _userRepository.GDPRDeletion(id);
                return Ok(new { result = !delete_result.Any(), errors = string.Join(", ", delete_result) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GDPRDeletion");
            }

            return NotFound();
        }

        //public async Task<ActionResult<bool>> GDPRDeletionMasive (int[] usersId)
        //{
        //    try
        //    {
        //        var delete_result = await _userRepository.GDPRDeletion(id);
        //        return Ok(new { result = !delete_result.Any(), errors = string.Join(", ", delete_result) });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "GDPRDeletion");
        //    }

        //    return NotFound();
        //}

    }
}
