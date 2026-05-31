using ERP_DTO;
using System.Data;

namespace ERP.DataList
{
    public class InventorySubmaster_DL
    {
        public List<InventorySubmaster_DTO> MCList(DataTable Dt)
        {
            List<InventorySubmaster_DTO> ISubList = new List<InventorySubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ISubList.Add(
                    new InventorySubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Classification"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return ISubList;
        }

        public List<InventorySubmaster_DTO> MOList(DataTable Dt)
        {
            List<InventorySubmaster_DTO> ISubList = new List<InventorySubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ISubList.Add(
                    new InventorySubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Ownership"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return ISubList;
        }

        public List<InventorySubmaster_DTO> MSList(DataTable Dt)
        {
            List<InventorySubmaster_DTO> ISubList = new List<InventorySubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ISubList.Add(
                    new InventorySubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Segregation"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return ISubList;
        }

        public List<InventorySubmaster_DTO> WCList(DataTable Dt)
        {
            List<InventorySubmaster_DTO> ISubList = new List<InventorySubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ISubList.Add(
                    new InventorySubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Warehouse"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return ISubList;
        }

    }
}
