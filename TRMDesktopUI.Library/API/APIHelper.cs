
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.API
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient { get; set; }
        private ILoggedInUserModel _user;

        public APIHelper(ILoggedInUserModel user)
        {
            InitializeClient();
            _user = user;
        }
        public HttpClient ApiClient { 
            get 
            {
                return _apiClient;
            } 
        }
        public void LogOfUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }
        private void InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"];
            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type","password"),
                 new KeyValuePair<string, string>("username",username),
                  new KeyValuePair<string, string>("password",password)

            });
            using (HttpResponseMessage response = await _apiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<AuthenticatedUser>(await response.Content.ReadAsStringAsync());
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //add new header to request
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using (HttpResponseMessage response = await _apiClient.GetAsync("/api/User"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<LoggedInUserModel>(await response.Content.ReadAsStringAsync());
                    _user.CreatedDate = result.CreatedDate;
                    _user.EmailAddress = result.EmailAddress;
                    _user.FirstName = result.FirstName;
                    _user.LastName = result.LastName;
                    _user.Id = result.Id;
                    _user.Token = token;

                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
