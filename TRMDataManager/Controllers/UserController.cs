using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;
using TRMDataManager.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
       
        [HttpGet]
        public UserModel GetById()
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();
            return data.GetUserById(userId).First();

        } 
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        [Authorize(Roles ="Admin")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
            using (var context =  new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();
                var roles = context.Roles.ToList();

                foreach(var user in users)
                {
                    ApplicationUserModel u = new ApplicationUserModel
                    {
                        Id = user.Id,
                        Email = user.Email
                    };

                    foreach(var r in user.Roles)
                    {
                        u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
                    }
                    output.Add(u);
                }
            }
            return output;
        }


        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        [Authorize(Roles = "Admin")]
        public Dictionary<string,string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var roles = context.Roles.ToDictionary(x=>x.Id,x=>x.Name);
                return roles;
            }
        }

        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        [Authorize(Roles = "Admin")]
        public void AddRole(UserRolePairModel userRolePair)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(userRolePair.UserId, userRolePair.RoleName);
            }
        }
        [HttpPost]
        [Route("api/User/Admin/RemoveRole")]
        [Authorize(Roles = "Admin")]
        public void RemoveRole(UserRolePairModel userRolePair)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(userRolePair.UserId, userRolePair.RoleName);
            }
        }
    }
}
