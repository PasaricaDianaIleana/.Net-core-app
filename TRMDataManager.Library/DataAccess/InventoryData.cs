using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;

namespace TRMDataManager.Library.Models
{
    public class InventoryData : IInventoryData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _dataAccess;

        public InventoryData(IConfiguration config,ISqlDataAccess dataAccess)
        {
            _config = config;
            _dataAccess = dataAccess;
        }
        public List<InventoryModel> GetInventory()
        {

            var output = _dataAccess.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            _dataAccess.SaveData("dbo.spInventory_Insert", item, "TRMData");
        }
    }
}
