using ERP_DTO;
using System.Data;

namespace ERP.DataList
{
    public class TaxationSubmaster_DL
    {
        public List<TaxationSubmaster_DTO> TCList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> TCList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TCList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Category"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return TCList;
        }

        public List<TaxationSubmaster_DTO> TTList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> TTList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TTList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Type"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return TTList;
        }

        public List<TaxationSubmaster_DTO> TIList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> TIList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TIList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Impact"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return TIList;
        }

        public List<TaxationSubmaster_DTO> NAList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> NAList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                NAList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Nature"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return NAList;
        }

        public List<TaxationSubmaster_DTO> RTList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> RTList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                RTList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Type"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return RTList;
        }

        public List<TaxationSubmaster_DTO> ATList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> ATList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ATList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Assessee"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return ATList;
        }

        public List<TaxationSubmaster_DTO> GCList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> GCList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                GCList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Category"]),
                        Location = Convert.ToString(dr["Location"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return GCList;
        }

        public List<TaxationSubmaster_DTO> GTList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> GTList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                GTList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Type"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return GTList;
        }

        public List<TaxationSubmaster_DTO> GNList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> GNList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                GNList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Nature"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return GNList;
        }

        public List<TaxationSubmaster_DTO> CBList(DataTable Dt)
        {
            List<TaxationSubmaster_DTO> CBList = new List<TaxationSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CBList.Add(
                    new TaxationSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Chargeable"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return CBList;
        }
    }
}
