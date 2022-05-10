using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.API
{
    public class SaleEndpoint : ISaleEndpoint
    {
        private IAPIHelper _apiHelper;
        public SaleEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public async Task PostSale(SaleModel sale)
        {
            var content = new StringContent(JsonConvert.SerializeObject(sale), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsync("/api/Sale", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    // Log succesful call?                
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

        }
    }
}
