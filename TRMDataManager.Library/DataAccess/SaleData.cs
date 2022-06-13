using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly IProductData _productData;
        private readonly ISqlDataAccess _sqlData;

        public SaleData(IProductData productData,ISqlDataAccess sqlData)
        {
            _productData = productData;
            _sqlData = sqlData;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            //Start filling in the model will save to database
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                //get the information about this product
                var productInfo = _productData.GetProductById(detail.ProductId);
                if (productInfo == null)
                {
                    throw new Exception($"The product id of{detail.ProductId} could not be found in the database");
                }
                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);
                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }
                details.Add(detail);
            }
            //create the sale model
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };
            sale.Total = sale.SubTotal + sale.Tax;

                try
                {
                    _sqlData.StartTransaction("TRMData");
                     //save the sale model
                    _sqlData.SaveDataInTransaction("dbo.spSale_Insert", sale);

                    sale.Id = _sqlData.LoadDataInTransaction<int, dynamic>
                   ("dbo.spSale_LookUp", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                    //finish filling in the sale detail models
                    foreach (var item in details)
                    {
                        //save the sale details models
                        item.SaleId = sale.Id;
                        //get the id from sale model
                        _sqlData.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }
                    _sqlData.CommitTransaction();
                }
                catch
                {
                    _sqlData.RollbackTransaction();
                    throw;
                }

        }
        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sqlData.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

            return output;
        }
    }
}
