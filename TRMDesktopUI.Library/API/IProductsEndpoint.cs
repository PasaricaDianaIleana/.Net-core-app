using System.Collections.Generic;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.API
{
    public interface IProductsEndpoint
    {
        Task<List<ProductModel>> GetAll();
    }
}