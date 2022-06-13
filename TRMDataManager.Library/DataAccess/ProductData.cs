
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess _sqlData;

        public ProductData(ISqlDataAccess sqlData)
        {
            _sqlData = sqlData;
        }
        public List<ProductModel> GetProducts()
        {
            var output = _sqlData.LoadData<ProductModel, dynamic>("spProduce_GetAll", new { }, "TRMData");

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            var output = _sqlData.LoadData<ProductModel, dynamic>("spProduce_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

            return output;
        }
    }
}
