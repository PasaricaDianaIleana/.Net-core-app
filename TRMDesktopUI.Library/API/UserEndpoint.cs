using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.API
{
    public class UserEndpoint : IUserEndpoint
    {
        private readonly IAPIHelper _apiHelper;
        public UserEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public async Task<List<UserModel>> GetAll()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllUsers"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<List<UserModel>>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<Dictionary<string,string>> GetAllRoles()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllRoles"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task AddUserToRole(string userId, string roleName)
        {
            var serializer = new JavaScriptSerializer();
            var model = new { userId, roleName };
            var json = serializer.Serialize(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsync("api/User/Admin/AddRole",stringContent))
            {
                if (response.IsSuccessStatusCode ==false)
                {
                
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task RemoveRoleFromUser(string userId, string roleName)
        {
            var serializer = new JavaScriptSerializer();
            var model = new { userId, roleName };
            var json = serializer.Serialize(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsync("api/User/Admin/RemoveRole", stringContent))
            {
                if (response.IsSuccessStatusCode == false)
                {

                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
