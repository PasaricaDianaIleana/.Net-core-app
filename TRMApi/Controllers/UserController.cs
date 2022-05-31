using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        public UserController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }
        [HttpGet]
        public UserModel GetById()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                         //RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData(_config);
            return data.GetUserById(userId).First();

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
            var user = await _userManager.FindByIdAsync(userRolePair.UserId);
            await _userManager.RemoveFromRoleAsync(user, userRolePair.RoleName);

        }
        [HttpPost]
        [Route("Admin/AddRole")]
        [Authorize(Roles = "Admin")]
        public async Task AddRole(UserRolePairModel userRolePair)
        {
            var user = await _userManager.FindByIdAsync(userRolePair.UserId);
              await  _userManager.AddToRoleAsync(user, userRolePair.RoleName);
        }
 
    }
}
