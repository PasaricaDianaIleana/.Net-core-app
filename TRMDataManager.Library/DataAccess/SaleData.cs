using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
   public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            //Start filling in the model will save to database
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData products = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate()/100;

            foreach (var item in saleInfo.SaleDetails)
            {
               var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                //get the information about this product
                var productInfo = products.GetProductById(detail.ProductId);
                if(productInfo == null)
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

            //save the sale model
            SqlDataAccess sql = new SqlDataAccess();
            sql.SaveData("dbo.spSale_Insert", sale, "TRMData");

            //get the id from sale model
            sale.Id = sql.LoadData<int, dynamic>
                ("dbo.spSale_LookUp", new { sale.CashierId,  sale.SaleDate }, 
                     "TRMData").FirstOrDefault();
    
            //finish filling in the sale detail models
            foreach(var item in details)
            {
                //save the sale details models
                item.SaleId = sale.Id;
                sql.SaveData("dbo.spSaleDetail_Insert", item, "TRMData");
            }

        }
    }
}
