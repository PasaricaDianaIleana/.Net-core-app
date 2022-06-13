using System.Collections.Generic;

namespace TRMDataManager.Library.Models
{
    public interface IInventoryData
    {
        List<InventoryModel> GetInventory();
        void SaveInventoryRecord(InventoryModel item);
    }
}