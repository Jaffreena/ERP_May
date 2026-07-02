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
    public class JobworkInvoice_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
   //     JW_Invoice_DL JW_Inv_DL = new JW_Invoice_DL();
        public DataSet JobworkInvoice(JobworkInvoiceCreate_DTO DN_DTO)
        {
            Database db = new SqlDatabase(DB.Connection());
            DbCommand cmd = db.GetStoredProcCommand("JI_JobworkInvoice_SP");

            //   int DN_Id = 10; // INSERT MODE

            // 🔹 Mode
            db.AddInParameter(cmd, "@JW_Inv_Id", DbType.Int32, DN_DTO.Header.JW_Inv_Id);

            DN_DTO.Header.JISVIH_InvoiceDate = DateTime.Now;
            db.AddInParameter(cmd, "@JISVIH_InvoiceDate", DbType.Date, DN_DTO.Header.JISVIH_InvoiceDate);
            //db.AddInParameter(cmd, "@JIDNI_Item_Code", DbType.String, DN_DTO.Header.it);
            //db.AddInParameter(cmd, "@DN_CUS_Number", DbType.Int32, DN_DTO.Header.JISVIH_JW_Customer_Number);
            //db.AddInParameter(cmd, "@DN_ADD_ADTP_Number", DbType.Int32, DN_DTO.Header.DN_ADD_ADTP_Number);


            return db.ExecuteDataSet(cmd);
        }

        public DataSet GetDeliveryNoteItemsDB(long CustomerNumber)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_GetDeliveryNoteItems_SP");

            db.AddInParameter(cmd,
                              "@CustomerNumber",
                              DbType.Int64,
                              CustomerNumber);

            return db.ExecuteDataSet(cmd);
        }

        public DataSet GetDeliveryNote_GroupItem(long CustomerNumber, long MSNumber)
        {
            Database db = new SqlDatabase(DB.Connection());
            DbCommand cmd = db.GetStoredProcCommand("JI_GetDeliveryNote_GroupItem_SP");

            db.AddInParameter(cmd, "@CustomerNumber", DbType.Int64, CustomerNumber);
            db.AddInParameter(cmd, "@MSNumber", DbType.Int64, MSNumber);

            return db.ExecuteDataSet(cmd);
        }

        #region Header Save

        #region get default address

        public DataSet GetJWCAddressDB(long JWCNumber)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JWC_Address_GetByJWCNumber");
            // or: "JWC_Address_Select_SP"

            db.AddInParameter(cmd,
                              "@JWC_Number",
                              DbType.Int64,
                              JWCNumber);

            return db.ExecuteDataSet(cmd);
        }

        #endregion
        #region Get Jobwork Invoice Address

        public DataSet GetJobworkInvoiceAddressDB(long JISVIHNumber)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_JobworkInvoiceAddress_GetByJISVIHNumber");

            db.AddInParameter(cmd,
                              "@JISVIH_Number",
                              DbType.Int64,
                              JISVIHNumber);

            return db.ExecuteDataSet(cmd);
        }

        #endregion
        public void JobworkInvoiceInsertDB(JobworkInvoiceCreate_DTO Invoice_DTO)
         {
            using (SqlConnection con = new SqlConnection(DB.Connection()))
            {
                con.Open();

                using (SqlTransaction tr = con.BeginTransaction())
                {
                    try
                    {
                        //---------------------------------------------------
                        // HEAD INSERT
                        //---------------------------------------------------
                        long JISVIH_Number = JobworkInvoiceHeadInsert(
                            Invoice_DTO,
                            con,
                            tr);

                        //---------------------------------------------------
                        // ITEM INSERT
                        //---------------------------------------------------
                        DataTable insertedItems = JobworkInvoiceItemBulkInsert(
                            JISVIH_Number,
                            Invoice_DTO,
                            con,
                            tr);
                        // GST INSERT
                        JobworkInvoiceGSTInsert(
                            JISVIH_Number,
                            insertedItems,
                            Invoice_DTO,
                            con,
                            tr
                        );

                        //---------------------------------------------------
                        // ADDRESS INSERT
                        //---------------------------------------------------
                        JobworkInvoiceAddressInsert(
                            JISVIH_Number,
                            Invoice_DTO.Addresses,
                            con,
                            tr);

                        //---------------------------------------------------
                        // COMMIT
                        //---------------------------------------------------
                        tr.Commit();
                    }
                    catch (Exception)
                    {
                        tr.Rollback();
                        throw;
                    }
                }
            }
        }
        public void JobworkInvoiceGSTInsert(
    long JISVIH_Number,
    DataTable insertedItems,
    JobworkInvoiceCreate_DTO Invoice_DTO,
    SqlConnection con,
    SqlTransaction tr)
        {
            foreach (DataRow row in insertedItems.Rows)
            {
                long itemNo =
                    Convert.ToInt64(row["JISVII_Number"]);

                long sacNo =
                    Convert.ToInt64(row["JISVII_SAC_Number"]);

                double amount =
                    Convert.ToDouble(row["JISVII_Amount"]);

                //-----------------------------------
                // GST CALCULATION
                //-----------------------------------
                List<JobInwardInvoiceGst> gstRows =
                    CalculateGST(
                        Invoice_DTO.Header.JISVIH_TCT_Number,
                        Invoice_DTO.Header.JISVIH_InvoiceDate,
                        sacNo,
                        amount
                    );

                //-----------------------------------
                // GST INSERT
                //-----------------------------------
                int gstIndex = 1;

                foreach (var gst in gstRows)
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "JI_JWI_GST_Insert_SP",
                        con,
                        tr))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@JISVIG_JISVIH_Number", JISVIH_Number);
                        cmd.Parameters.AddWithValue("@JISVIG_JISVII_Number", itemNo);
                        cmd.Parameters.AddWithValue("@JISVIG_Index", gstIndex);
                        cmd.Parameters.AddWithValue("@JISVIG_GSTC_Number", gst.GSTCNumber);
                        cmd.Parameters.AddWithValue("@JISVIG_GSTT_Number", gst.GSTTNumber);
                        cmd.Parameters.AddWithValue("@JISVIG_GSTE_Number", gst.GSTENumber);
                        cmd.Parameters.AddWithValue("@JISVIG_AssessableValue", gst.AssessableValue);
                        cmd.Parameters.AddWithValue("@JISVIG_Percent", gst.Percentage);
                        cmd.Parameters.AddWithValue("@JISVIG_GST_Amount", gst.Amount);

                        cmd.ExecuteNonQuery();
                    }

                    gstIndex++;
                }
            }
        }

        public void JobworkInvoiceAddressInsert(
    long JISVIH_Number,
    List<JobworkInvoiceAddress_DTO> addressList,
    SqlConnection con,
    SqlTransaction tr)
        {
            long addressNo = 1;

            foreach (var address in addressList)
            {
                using (SqlCommand cmd = new SqlCommand(
                    "JI_JobworkInvoiceAddress_Insert_SP",
                    con,
                    tr))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@JISVIA_JISVIH_Number", JISVIH_Number);
                 
                    cmd.Parameters.AddWithValue("@JISVIA_ADTP_Number", address.JISVIA_ADTP_Number);
                    cmd.Parameters.AddWithValue("@JISVIA_Address_ID", address.JISVIA_Address_ID);
                    cmd.Parameters.AddWithValue("@JISVIA_Address", address.JISVIA_Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JISVIA_City", address.JISVIA_City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JISVIA_State", address.JISVIA_State ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JISVIA_Country", address.JISVIA_Country ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JISVIA_PIN", address.JISVIA_PIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JISVIA_GSTIN", address.JISVIA_GSTIN ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }

                addressNo++;
            }
        }
        public List<JobInwardInvoiceGst> SaleInvGstView(DataTable Dt)
        {
            List<JobInwardInvoiceGst> IList = new List<JobInwardInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new JobInwardInvoiceGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        GSTCNumber = dr["GSTCNumber"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(dr["GSTCNumber"]),

                        GSTTNumber = dr["GSTTNumber"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(dr["GSTTNumber"]),

                        GSTENumber = dr["GSTENumber"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(dr["GSTENumber"]),
                        TaxCategory = Convert.ToString(dr["TaxCategory"]),
                        TaxType = Convert.ToString(dr["TaxType"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        TaxElementName = Convert.ToString(dr["TaxElementName"]),
                        LoadonInventory = Convert.ToString(dr["LoadonInventory"]),
                        LoadonInventoryPercent = Convert.ToString(dr["LoadonInventoryPercent"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<JobInwardInvoiceGst> CalculateGST(
       long cluster,
       DateTime invoiceDate,
       long sacNo,
       double baseAmount)
        {
            int nInvoiceDate =
                Convert.ToInt32(invoiceDate.ToString("yyyyMMdd"));

            DataTable dt = GetTaxClusterCalculation(
                cluster,
                sacNo,
                nInvoiceDate
            ).Tables[0];

            List<JobInwardInvoiceGst> PurGST =
                new List<JobInwardInvoiceGst>();

            var GroupTotals =
                new Dictionary<long, double>();

            var gstList = SaleInvGstView(dt);

            if (gstList == null || !gstList.Any())
                return PurGST;

            var TaxIndex = gstList
                .GroupBy(gst => Convert.ToInt64(gst.TaxIndex));

            foreach (var Group in TaxIndex)
            {
                double GroupTotal = 0;
                double GroupAssessableValue = 0;

                var calculationOneItems =
                    Group.Where(x => Convert.ToInt32(x.Calculation) == 1)
                         .ToList();

                if (!calculationOneItems.Any())
                    continue;

                var first = calculationOneItems.First();

                long taxElement = Convert.ToInt64(first.TaxElement);

                foreach (var item in Group)
                {
                    double BaseElementValue = 0;

                    int calculation = Convert.ToInt32(item.Calculation);
                    int chargeable = Convert.ToInt32(item.Chargeable);

                    // -----------------------------
                    // FIRST LEVEL TAX (Base GST)
                    // -----------------------------
                    if (chargeable == 4 && calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            GroupTotal += baseAmount * (item.Percentage.Value / 100);
                            GroupAssessableValue += baseAmount;
                        }
                    }

                    // -----------------------------
                    // SECOND LEVEL TAX (On Tax)
                    // -----------------------------
                    else if (calculation == 0)
                    {
                        if (GroupTotals.ContainsKey(Convert.ToInt64(item.TaxElement)))
                        {
                            BaseElementValue =
                                GroupTotals[Convert.ToInt64(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                GroupTotal += BaseElementValue * (item.Percentage.Value / 100);
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }
                }

                PurGST.Add(new JobInwardInvoiceGst
                {
                    TaxIndex = Group.Key,

                    GSTCNumber = first.GSTCNumber ?? 0,
                    GSTTNumber = first.GSTTNumber ?? 0,
                    GSTENumber = first.GSTENumber ?? 0,

                    Percentage = first.Percentage ?? 0,

                    AssessableValue = GroupAssessableValue,
                    Amount = GroupTotal
                });

                GroupTotals[taxElement] = GroupTotal;
            }

            return PurGST;
        }
        public long JobworkInvoiceHeadInsert(
    JobworkInvoiceCreate_DTO Invoice_DTO,
    SqlConnection con,
    SqlTransaction tr)
        {
            long JISVIH_Number = 0;

            using (SqlCommand cmd = new SqlCommand("JI_JobworkInvoiceHead_Insert_SP", con, tr))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                

                cmd.Parameters.AddWithValue("@JISVIH_InvoiceNo",
                    Invoice_DTO.Header.JISVIH_InvoiceNo);

                cmd.Parameters.AddWithValue("@JISVIH_InvoiceDate",
                    Invoice_DTO.Header.JISVIH_InvoiceDate);

                cmd.Parameters.AddWithValue("@JISVIH_JW_Customer_Number",
                    Invoice_DTO.Header.JISVIH_JW_Customer_Number);
                cmd.Parameters.AddWithValue("@JISVIH_MS_Number",
                   Invoice_DTO.Header.JISVIH_MS_Number);
                cmd.Parameters.AddWithValue("@JISVIH_Currency_Number",
                    Invoice_DTO.Header.JISVIH_Currency_Number);

                cmd.Parameters.AddWithValue("@JISVIH_TCT_Number",
                    Invoice_DTO.Header.JISVIH_TCT_Number);

                cmd.Parameters.AddWithValue("@JISVIH_PaymentTerms",
                    Invoice_DTO.Header.JISVIH_PaymentTerms ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@JISVIH_PaymentMethod",
                    Invoice_DTO.Header.JISVIH_PaymentMethod ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@JISVIH_Remarks",
                    Invoice_DTO.Header.JISVIH_Remarks ?? (object)DBNull.Value);

                JISVIH_Number = Convert.ToInt64(cmd.ExecuteScalar());
            }

            return JISVIH_Number;
        }
        public DataTable JobworkInvoiceItemBulkInsert(
    long JISVIH_Number,
    JobworkInvoiceCreate_DTO Invoice_DTO,
    SqlConnection con,
    SqlTransaction tr)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JISVII_JISVIH_Number", typeof(long));
            dt.Columns.Add("JISVII_Number", typeof(long));
            dt.Columns.Add("JISVOI_Number", typeof(long)); // added
            dt.Columns.Add("JISVII_JISVOH_Number", typeof(long));
            dt.Columns.Add("JISVII_JIDNH_Number", typeof(long));
            dt.Columns.Add("JIDNI_Number", typeof(long));
            dt.Columns.Add("JISVII_PRS_Number", typeof(long));
            dt.Columns.Add("JISVII_Item_Number", typeof(long));
            dt.Columns.Add("JISVII_UoM_Number", typeof(long));
            dt.Columns.Add("JISVII_Qty", typeof(double));
            dt.Columns.Add("JISVII_UnitPrice", typeof(double));
            dt.Columns.Add("JISVII_Amount", typeof(double));
            dt.Columns.Add("JISVII_SAC_Number", typeof(long));
            dt.Columns.Add("JISVII_GST_Amount", typeof(double));

            foreach (var item in Invoice_DTO.Items)
            {
                dt.Rows.Add(
                      item.JISVII_JISVIH_Number
                    , item.JISVII_Number
                    , item.JISVOI_Number       // added
                    , item.JISVII_JISVOH_Number
                    , item.JISVII_JIDNH_Number
                    , item.JIDNI_Number
                    , item.JISVII_PRS_Number
                    , item.JISVII_Item_Number
                    , item.JISVII_UoM_Number
                    , item.JISVII_Qty
                    , item.JISVII_UnitPrice
                    , item.JISVII_Amount
                    , item.JISVII_SAC_Number
                    , item.JISVII_GST_Amount
                );
            }

            using (SqlCommand cmd = new SqlCommand(
                "JI_JobworkInvoiceItem_BulkInsert_SP",
                con,
                tr))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@JISVIH_Number",
                    JISVIH_Number);

                SqlParameter tvp = cmd.Parameters.AddWithValue(
                    "@Items",
                    dt);

                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.JobworkInvoiceItemType";

                DataTable insertedItems = new DataTable();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(insertedItems);
                }

                return insertedItems;
            }
        }
        #endregion

        #region GET DELIVERY NOTE FOR INVOICE

        public DataSet GetDeliveryNote_ForInvoice(long CustomerNumber, string DNNumbers)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_DeliveryNote_ForInvoice_SP");

            #region PARAMETERS

            db.AddInParameter(cmd,
                              "@CustomerNumber",
                              DbType.Int64,
                              CustomerNumber);

            db.AddInParameter(cmd,
                              "@DNNumbers",
                              DbType.String,
                              DNNumbers);

            #endregion

            return db.ExecuteDataSet(cmd);
        }

        #endregion

        #region taxcluster
        public DataSet Get_JW_Invoice_Taxcluster(long JWC_Number, DateTime CheckDate)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("Get_JW_Invoice_Taxcluster");

            #region PARAMETERS

            db.AddInParameter(cmd,
                              "@JWC_Number",
                              DbType.Int64,
                              JWC_Number);

            db.AddInParameter(cmd,
                              "@CheckDate",
                              DbType.Date,
                              CheckDate);

            #endregion

            return db.ExecuteDataSet(cmd);
        }
        #endregion

        #region taxcluster

        public DataSet GetTaxClusterCalculation(long JW_INV_TCT_Number,
                                                long JW_INV_SAC_Number,
                                                int JW_INV_InvoiceDate)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_GetTaxClusterCalculation_SP");

            #region PARAMETERS

            db.AddInParameter(cmd,
                              "@JW_INV_TCT_Number",
                              DbType.Int64,
                              JW_INV_TCT_Number);

            db.AddInParameter(cmd,
                              "@JW_INV_SAC_Number",
                              DbType.Int64,
                              JW_INV_SAC_Number);

            db.AddInParameter(cmd,
                              "@JW_INV_InvoiceDate",
                              DbType.Int32,
                              JW_INV_InvoiceDate);

            #endregion

            return db.ExecuteDataSet(cmd);
        }

        #endregion

        #region taxcluster sac

        public DataSet GetTaxClusterCalculationSAC(long JW_INV_TCT_Number,
                                                long JW_INV_SAC_Number,
                                                int JW_INV_InvoiceDate)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_GetTaxClusterSACCalculation_SP");

            #region PARAMETERS

            db.AddInParameter(cmd,
                              "@JW_INV_TCT_Number",
                              DbType.Int64,
                              JW_INV_TCT_Number);

            db.AddInParameter(cmd,
                              "@JW_INV_SAC_Number",
                              DbType.Int64,
                              JW_INV_SAC_Number);

            db.AddInParameter(cmd,
                              "@JW_INV_InvoiceDate",
                              DbType.Int32,
                              JW_INV_InvoiceDate);

            #endregion

            return db.ExecuteDataSet(cmd);
        }

        #endregion

        #region summary
        public DataSet GetJobworkInvoiceList()
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_JobworkInvoice_List_SP");

            return db.ExecuteDataSet(cmd);
        }
        public DataSet GetJobworkInvoiceListDetailed()
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_JobworkInvoice_ListDetailed_SP");

            return db.ExecuteDataSet(cmd);
        }
        #endregion

        #region edit
        public string  GetJobworkInvoiceJSON(long JISVIH_Number)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd =
                db.GetStoredProcCommand("JI_JobworkInvoice_Get_JSON_SP");

            db.AddInParameter(cmd,
                              "@JISVIH_Number",
                              DbType.Int64,
                              JISVIH_Number);

            DataSet ds = db.ExecuteDataSet(cmd);

            StringBuilder json = new StringBuilder();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                json.Append(row[0].ToString());
            }

            return json.ToString();
        }
        #endregion

        #region update header
        public void JobworkInvoiceUpdateDB(
    JobworkInvoiceCreate_DTO DN_DTO)
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
                        JobworkInvoiceHeaderUpdate(
                            DN_DTO,
                            con,
                            tr);

                        // Uncomment when item/address update is ready

                        JobworkInvoiceItemBulkUpdate(
                            DN_DTO,
                            con,
                            tr);

                        JobworkInvoiceAddressBulkUpdate(
                            DN_DTO,
                            con,
                            tr);

                        tr.Commit();
                    }
                    catch (Exception)
                    {
                        tr.Rollback();
                        throw;
                    }
                }
            }
        }
        public void JobworkInvoiceHeaderUpdate(
    JobworkInvoiceCreate_DTO DN_DTO,
    SqlConnection con,
    SqlTransaction tr)
        {
            using (SqlCommand cmd = new SqlCommand(
                "JI_JobworkInvoiceHead_Update_SP",
                con,
                tr))
            {
                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@JISVIH_Number",
                    DN_DTO.Header.JISVIH_Number);
                cmd.Parameters.AddWithValue(
                "@JISVIH_MS_Number",
                DN_DTO.Header.JISVIH_MS_Number);


                cmd.Parameters.AddWithValue(
                    "@JISVIH_InvoiceNo",
                    DN_DTO.Header.JISVIH_InvoiceNo);

                cmd.Parameters.AddWithValue(
                    "@JISVIH_InvoiceDate",
                    DN_DTO.Header.JISVIH_InvoiceDate);

                cmd.Parameters.AddWithValue(
                    "@JISVIH_JW_Customer_Number",
                    DN_DTO.Header.JISVIH_JW_Customer_Number);

                cmd.Parameters.AddWithValue(
                    "@JISVIH_Currency_Number",
                    DN_DTO.Header.JISVIH_Currency_Number);

                cmd.Parameters.AddWithValue(
                    "@JISVIH_TCT_Number",
                    DN_DTO.Header.JISVIH_TCT_Number);

                cmd.Parameters.AddWithValue(
                    "@JISVIH_PaymentTerms",
                    DN_DTO.Header.JISVIH_PaymentTerms
                    ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@JISVIH_PaymentMethod",
                    DN_DTO.Header.JISVIH_PaymentMethod
                    ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue(
                    "@JISVIH_Remarks",
                    DN_DTO.Header.JISVIH_Remarks
                    ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region update items
        public void JobworkInvoiceItemBulkUpdate(
    JobworkInvoiceCreate_DTO DN_DTO,
    SqlConnection con,
    SqlTransaction tr)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JISVII_JISVIH_Number", typeof(long));   // 1
            dt.Columns.Add("JISVII_Number", typeof(long));          // 2
            dt.Columns.Add("JISVOI_Number", typeof(long));          // 3 <-- Missing
            dt.Columns.Add("JISVII_JISVOH_Number", typeof(long));   // 4
            dt.Columns.Add("JISVII_JIDNH_Number", typeof(long));    // 5
            dt.Columns.Add("JIDNI_Number", typeof(long));           // 6
            dt.Columns.Add("JISVII_PRS_Number", typeof(long));      // 7
            dt.Columns.Add("JISVII_Item_Number", typeof(long));     // 8
            dt.Columns.Add("JISVII_UoM_Number", typeof(long));      // 9
            dt.Columns.Add("JISVII_Qty", typeof(decimal));          // 10
            dt.Columns.Add("JISVII_UnitPrice", typeof(decimal));    // 11
            dt.Columns.Add("JISVII_Amount", typeof(decimal));       // 12
            dt.Columns.Add("JISVII_SAC_Number", typeof(long));      // 13
            dt.Columns.Add("JISVII_GST_Amount", typeof(decimal));   // 14

            foreach (var item in DN_DTO.Items)
            {
                dt.Rows.Add(
                    DN_DTO.Header.JISVIH_Number,   // 1
                    item.JISVII_Number,            // 2
                    item.JISVOI_Number,            // 3 <-- Added
                    item.JISVII_JISVOH_Number,     // 4
                    item.JISVII_JIDNH_Number,      // 5
                    item.JIDNI_Number,             // 6
                    item.JISVII_PRS_Number,        // 7
                    item.JISVII_Item_Number,       // 8
                    item.JISVII_UoM_Number,        // 9
                    item.JISVII_Qty,               // 10
                    item.JISVII_UnitPrice,         // 11
                    item.JISVII_Amount,            // 12
                    item.JISVII_SAC_Number,        // 13
                    item.JISVII_GST_Amount         // 14
                );
            }

            using (SqlCommand cmd = new SqlCommand(
                "JI_JobworkInvoiceItem_BulkUpdate_SP",
                con,
                tr))
            {
                cmd.CommandType =
                    CommandType.StoredProcedure;

                SqlParameter param =
                    cmd.Parameters.AddWithValue(
                        "@Items",
                        dt);

                param.SqlDbType =
                    SqlDbType.Structured;

                param.TypeName =
                    "JobworkInvoiceItemType";

                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region update address
        public void JobworkInvoiceAddressBulkUpdate(
            JobworkInvoiceCreate_DTO DN_DTO,
            SqlConnection con,
            SqlTransaction tr)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JISVIA_JISVIH_Number", typeof(long));
            dt.Columns.Add("JISVIA_ADTP_Number", typeof(long));
            dt.Columns.Add("JISVIA_Address_ID", typeof(string));
            dt.Columns.Add("JISVIA_Address", typeof(string));
            dt.Columns.Add("JISVIA_City", typeof(string));
            dt.Columns.Add("JISVIA_State", typeof(string));
            dt.Columns.Add("JISVIA_Country", typeof(string));
            dt.Columns.Add("JISVIA_PIN", typeof(string));
            dt.Columns.Add("JISVIA_GSTIN", typeof(string));

            foreach (var item in DN_DTO.Addresses)
            {
                dt.Rows.Add(
                    DN_DTO.Header.JISVIH_Number,
                    item.JISVIA_ADTP_Number,
                    item.JISVIA_Address_ID,
                    item.JISVIA_Address,
                    item.JISVIA_City,
                    item.JISVIA_State,
                    item.JISVIA_Country,
                    item.JISVIA_PIN,
                    item.JISVIA_GSTIN
                );
            }
            using (SqlCommand cmd = new SqlCommand("JI_JobworkInvoiceAddress_Update_SP", con, tr))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@JISVIA_JISVIH_Number", DN_DTO.Header.JISVIH_Number);
                SqlParameter param = cmd.Parameters.AddWithValue("@Address", dt);
                param.SqlDbType = SqlDbType.Structured;
                param.TypeName = "JI_JobworkInvoiceAddress_TableType";
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region MyRegion
        public DataSet GetServiceOrderItemInfo(
      long serviceOrderNo,
      long prsNumber,
      long itemNumber,
      long uomNumber)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd =
                db.GetStoredProcCommand(
                    "JI_ServiceOrderItem_Info_SP");

            db.AddInParameter(cmd, "@JISVOI_JISVOH_Number", DbType.Int64, serviceOrderNo);
            db.AddInParameter(cmd, "@JISVOI_PRS_Number", DbType.Int64, prsNumber);
            db.AddInParameter(cmd, "@JISVOI_Item_Number", DbType.Int64, itemNumber);
            db.AddInParameter(cmd, "@JISVOI_UoM_Number", DbType.Int64, uomNumber);

            return db.ExecuteDataSet(cmd);
        }
        #endregion
    }
}
