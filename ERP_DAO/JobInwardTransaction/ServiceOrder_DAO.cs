using ERP_DTO.JobInwardTransaction;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ERP_DAO.JobInwardTransaction
{
    public class ServiceOrder_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet ServiceOrderDB(JI_ServiceOrder_DTO SVO_DTO)
        {
            Database db = new SqlDatabase(DB.Connection());
            DbCommand cmd = db.GetStoredProcCommand("JI_ServiceOrder_SP");

            db.AddInParameter(cmd, "@SVO_Id", DbType.Int32, SVO_DTO.Header.SVO_Id);
            db.AddInParameter(cmd, "@JISVOH_RegDate", DbType.Date, SVO_DTO.Header.JISVOH_RegDate);
            db.AddInParameter(cmd, "@JWCustomer", DbType.String, SVO_DTO.Header.JISVOH_JW_Customer_Name);
            db.AddInParameter(cmd, "@JISVOI_Item_Code", DbType.String, SVO_DTO.Header.JISVOI_Item_Code);
            db.AddInParameter(cmd, "@JISVOH_MS_Number", DbType.String, SVO_DTO.Header.JISVOH_MS_Number);
            return db.ExecuteDataSet(cmd);
        }
        public DataSet ServiceOrderSummaryDB(
    ServiceOrderSummary_DTO SO_DTO)
        {
            Database db =
                new SqlDatabase(DB.Connection());

            DbCommand cmd =
                db.GetStoredProcCommand(
                    "JI_ServiceOrder_Summary_SP");

            // Mode
            db.AddInParameter(
                cmd,
                "@SO_Id",
                DbType.Int32,
                SO_DTO.SO_Id);

            return db.ExecuteDataSet(cmd);
        }
        public void ServiceOrderUpdateDB(JI_ServiceOrder_DTO serviceOrderDTO)
        {
            using SqlConnection con = new SqlConnection(DB.Connection());
            con.Open();

            using SqlTransaction tr = con.BeginTransaction();

            try
            {
                ServiceOrderHeadUpdate(serviceOrderDTO, con, tr);

                ServiceOrderItemUpdate(
                    serviceOrderDTO.Header.JISVOH_Number,
                    serviceOrderDTO.Items,
                    con,
                    tr);

                tr.Commit();
            }
            catch
            {
                tr.Rollback();
                throw;
            }
        }
        public void ServiceOrderInsertDB(JI_ServiceOrder_DTO serviceOrderDTO)
        {
            using (SqlConnection con =
                new SqlConnection(DB.Connection()))
            {
                con.Open();

                using (SqlTransaction tr =
                    con.BeginTransaction())
                {
                    try
                    {
                        //-----------------------------------
                        // HEADER INSERT
                        //-----------------------------------
                        long JISVOH_Number =
                            ServiceOrderHeadInsert(
                                serviceOrderDTO,
                                con,
                                tr);

                        //-----------------------------------
                        // ITEM BULK INSERT
                        //-----------------------------------
                        ServiceOrderItemBulkInsert(
                            JISVOH_Number,
                            serviceOrderDTO.Items,
                            con,
                            tr);

                        //-----------------------------------
                        // COMMIT
                        //-----------------------------------
                        tr.Commit();
                    }
                    catch
                    {
                        tr.Rollback();
                        throw;
                    }
                }
            }
        }
        private void ServiceOrderHeadUpdate(JI_ServiceOrder_DTO dto, SqlConnection con, SqlTransaction tr)
        {
            using SqlCommand cmd = new SqlCommand("JI_ServiceOrderHead_Update_SP", con, tr);
            cmd.CommandType = CommandType.StoredProcedure;

            var h = dto.Header;

            cmd.Parameters.AddWithValue("@JISVOH_Number", h.JISVOH_Number);
            cmd.Parameters.AddWithValue("@JISVOH_RegNo", h.JISVOH_RegNo);
            cmd.Parameters.AddWithValue("@JISVOH_RegDate", h.JISVOH_RegDate);
            cmd.Parameters.AddWithValue("@JISVOH_ServiceOrderNo", h.JISVOH_ServiceOrderNo);
            cmd.Parameters.AddWithValue("@JISVOH_ServiceOrderDate", h.JISVOH_ServiceOrderDate);
            cmd.Parameters.AddWithValue("@JISVOH_JW_Customer_Number", h.JISVOH_JW_Customer_Number);
            cmd.Parameters.AddWithValue("@JISVOH_Currency_Number", h.JISVOH_Currency_Number);
            cmd.Parameters.AddWithValue("@JISVOH_PaymentTerms", h.JISVOH_PaymentTerms ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_DeliveryTerms", h.JISVOH_DeliveryTerms ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_DeliveryMode", h.JISVOH_DeliveryMode ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_Tax", h.JISVOH_Tax ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_TDC", h.JISVOH_TDC ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_Remarks", h.JISVOH_Remarks ?? "");

            cmd.ExecuteNonQuery();
        }
        private long ServiceOrderHeadInsert(
    JI_ServiceOrder_DTO dto,
    SqlConnection con,
    SqlTransaction tr)
        {
            using SqlCommand cmd = new SqlCommand(
                "JI_ServiceOrderHead_Insert_SP", con, tr);

            cmd.CommandType = CommandType.StoredProcedure;

            var h = dto.Header;

            cmd.Parameters.AddWithValue("@JISVOH_RegNo", h.JISVOH_RegNo);
            cmd.Parameters.AddWithValue("@JISVOH_RegDate", h.JISVOH_RegDate);
            cmd.Parameters.AddWithValue("@JISVOH_ServiceOrderNo", h.JISVOH_ServiceOrderNo);
            cmd.Parameters.AddWithValue("@JISVOH_ServiceOrderDate", h.JISVOH_ServiceOrderDate);
            cmd.Parameters.AddWithValue("@JISVOH_JW_Customer_Number", h.JISVOH_JW_Customer_Number);
            cmd.Parameters.AddWithValue("@JISVOH_Currency_Number", h.JISVOH_Currency_Number);
            cmd.Parameters.AddWithValue("@JISVOH_PaymentTerms", h.JISVOH_PaymentTerms ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_DeliveryTerms", h.JISVOH_DeliveryTerms ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_DeliveryMode", h.JISVOH_DeliveryMode ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_Tax", h.JISVOH_Tax ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_TDC", h.JISVOH_TDC ?? "");
            cmd.Parameters.AddWithValue("@JISVOH_Remarks", h.JISVOH_Remarks ?? "");

            return Convert.ToInt64(cmd.ExecuteScalar());
        }
        private DataTable CreateServiceOrderItemUpdateTable(List<JI_ServiceOrderItem_DTO> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JISVOI_Number", typeof(long));
            dt.Columns.Add("JISVOI_PRS_Number", typeof(long));
            dt.Columns.Add("JISVOI_Item_Number", typeof(long));
            dt.Columns.Add("JISVOI_UoM_Number", typeof(long));
            dt.Columns.Add("JISVOI_Qty", typeof(double));
            dt.Columns.Add("JISVOI_UnitPrice", typeof(double));
            dt.Columns.Add("JISVOI_Amount", typeof(double));
            dt.Columns.Add("JISVOI_DeliveryDate", typeof(DateTime));

            foreach (var item in items)
            {
                if (item.JISVOI_IsDeleted)
                    continue;

                DataRow row = dt.NewRow();

                row["JISVOI_Number"] = item.JISVOI_Number > 0 ? item.JISVOI_Number : DBNull.Value;
                row["JISVOI_PRS_Number"] = item.JISVOI_PRS_Number;
                row["JISVOI_Item_Number"] = item.JISVOI_Item_Number;
                row["JISVOI_UoM_Number"] = item.JISVOI_UoM_Number;
                row["JISVOI_Qty"] = item.JISVOI_Qty;
                row["JISVOI_UnitPrice"] = item.JISVOI_UnitPrice;
                row["JISVOI_Amount"] = item.JISVOI_Amount;
                row["JISVOI_DeliveryDate"] = item.JISVOI_DeliveryDate.HasValue ? item.JISVOI_DeliveryDate.Value : DBNull.Value;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private DataTable CreateServiceOrderItemTable(
    List<JI_ServiceOrderItem_DTO> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JISVOI_PRS_Number", typeof(long));
            dt.Columns.Add("JISVOI_Item_Number", typeof(long));
            dt.Columns.Add("JISVOI_UoM_Number", typeof(long));
            dt.Columns.Add("JISVOI_Qty", typeof(double));
            dt.Columns.Add("JISVOI_UnitPrice", typeof(double));
            dt.Columns.Add("JISVOI_Amount", typeof(double));
            dt.Columns.Add("JISVOI_DeliveryDate", typeof(DateTime));

            foreach (var item in items)
            {
                if (item.JISVOI_IsDeleted)
                    continue;

                DataRow row = dt.NewRow();

                row["JISVOI_PRS_Number"] =
                    item.JISVOI_PRS_Number;

                row["JISVOI_Item_Number"] =
                    item.JISVOI_Item_Number;

                row["JISVOI_UoM_Number"] =
                    item.JISVOI_UoM_Number;

                row["JISVOI_Qty"] =
                    item.JISVOI_Qty;

                row["JISVOI_UnitPrice"] =
                    item.JISVOI_UnitPrice;

                row["JISVOI_Amount"] =
                    item.JISVOI_Amount;

                row["JISVOI_DeliveryDate"] =
                    item.JISVOI_DeliveryDate.HasValue
                        ? item.JISVOI_DeliveryDate.Value
                        : DBNull.Value;

                dt.Rows.Add(row);
            }

            return dt;
        }
        private void ServiceOrderItemUpdate(long headerNumber, List<JI_ServiceOrderItem_DTO> items, SqlConnection con, SqlTransaction tr)
        {
            DataTable dt = CreateServiceOrderItemUpdateTable(items);

            using SqlCommand cmd = new SqlCommand("JI_ServiceOrderItem_Update_SP", con, tr);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@JISVOH_Number", headerNumber);

            SqlParameter param = cmd.Parameters.AddWithValue("@Items", dt);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.JI_ServiceOrderItem_Update_TableType";

            cmd.ExecuteNonQuery();
        }
        private void ServiceOrderItemBulkInsert(long headerNumber, List<JI_ServiceOrderItem_DTO> items, SqlConnection con, SqlTransaction tr)
        {
            DataTable dt = CreateServiceOrderItemTable(items);

            using SqlCommand cmd = new SqlCommand("JI_ServiceOrderItem_BulkInsert_SP", con, tr);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@JISVOH_Number", headerNumber);

            SqlParameter param = cmd.Parameters.AddWithValue("@Items", dt);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.JI_ServiceOrderItem_TableType";

            cmd.ExecuteNonQuery();
        }
        public DataTable GetServiceOrderHead(long customerNumber)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd =
                db.GetStoredProcCommand(
                    "JI_Get_ServiceOrderHead_SP");

            db.AddInParameter(
                cmd,
                "@CustomerNumber",
                DbType.Int64,
                customerNumber);

            DataSet ds = db.ExecuteDataSet(cmd);

            return ds.Tables[0];
        }

        #region edit
        public string GetServiceOrderJSON(long JISVOH_Number)
        {
            Database db =
                new SqlDatabase(DB.Connection());

            DbCommand cmd =
                db.GetStoredProcCommand(
                    "JI_ServiceOrder_Get_JSON_SP");

            db.AddInParameter(
                cmd,
                "@JISVOH_Number",
                DbType.Int64,
                JISVOH_Number);

            string json =
                db.ExecuteScalar(cmd)?.ToString();

            return json;
        }
        #endregion


    }
}
