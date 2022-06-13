using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRMApi.Data;
using TRMApi.Models;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserData _userData;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, IUserData userData,
            UserManager<IdentityUser> userManager,ILogger<UserController> logger)
        {
            _context = context;
            _userData = userData;
            _userManager = userManager;
            _logger = logger;
        }
        [HttpGet]
        public UserModel GetById()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _userData.GetUserById(userId).First();

        }
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
            var users = _context.Users.ToList();
            var usersRoles = from ur in _context.UserRoles
                             join r in _context.Roles on ur.RoleId equals r.Id
                             select new { ur.UserId, ur.RoleId, r.Name };
          foreach (var user in users)
              {
              ApplicationUserModel u = new ApplicationUserModel
                    {
                        Id = user.Id,
                        Email = user.Email
                    };
                u.Roles = usersRoles.Where(x => x.UserId == u.Id).ToDictionary(key=> key.RoleId, val=>val.Name);
                output.Add(u);
                }
            return output;
        }


        [HttpGet]
        [Route("Admin/GetAllRoles")]
        [Authorize(Roles = "Admin")]
        public Dictionary<string, string> GetAllRoles()
        {
                var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
                return roles;
        }
        [HttpPost]
        [Route("Admin/RemoveRole")]
        [Authorize(Roles = "Admin")]
        public async Task RemoveRole(UserRolePairModel userRolePair)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var user = await _userManager.FindByIdAsync(userRolePair.UserId);

            _logger.LogInformation("Admin {Admin} removed role {Role} from user {User}",
                loggedInUserId, userRolePair.RoleName, user.Id);
            await _userManager.RemoveFromRoleAsync(user, userRolePair.RoleName);

        }
        [HttpPost]
        [Route("Admin/AddRole")]
        [Authorize(Roles = "Admin")]
        public async Task AddRole(UserRolePairModel userRolePair)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userRolePair.UserId);

            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}",
                loggedInUserId,user.Id,userRolePair.RoleName);
            await  _userManager.AddToRoleAsync(user, userRolePair.RoleName);
        }
 
    }
}
