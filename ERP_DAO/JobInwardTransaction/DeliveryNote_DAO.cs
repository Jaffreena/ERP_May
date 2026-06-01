using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_DTO.JobInwardTransaction;
using System.Data;
using System.Data.SqlClient;

namespace ERP_DAO.JobInwardTransaction
{
    public class DeliveryNote_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet DeliveryNoteEditDB(long JIDNH_Number)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd =
                db.GetStoredProcCommand("JI_DeliveryNote_Edit_SP");

            db.AddInParameter(cmd,
                              "@JIDNH_Number",
                              DbType.Int64,
                              JIDNH_Number);

            return db.ExecuteDataSet(cmd);
        }

        public DataSet DeliveryNoteViewDB(long JIDNH_Number)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_DeliveryNote_View_SP");

            
            db.AddInParameter(cmd,
                              "@JIDNH_Number",
                              DbType.Int64,
                              JIDNH_Number);
 
            return db.ExecuteDataSet(cmd);
        }

        public DataSet DeliveryNoteDB(DeliveryNoteCreate_DTO DN_DTO)
        {
            Database db = new SqlDatabase(DB.Connection());
            DbCommand cmd = db.GetStoredProcCommand("JI_DeliveryNote_SP");

         //   int DN_Id = 10; // INSERT MODE

            // 🔹 Mode
            db.AddInParameter(cmd, "@DN_Id", DbType.Int32, DN_DTO.Header.DN_Id);

            DN_DTO.Header.JIDNH_DN_Date = DateTime.Now;
            db.AddInParameter(cmd, "@JIDNH_DN_Date", DbType.Date, DN_DTO.Header.JIDNH_DN_Date);
            db.AddInParameter(cmd, "@JIDNI_Item_Code", DbType.String, DN_DTO.Header.JIDNI_Item_Code);
            db.AddInParameter(cmd, "@DN_CUS_Number", DbType.Int32, DN_DTO.Header.DN_CUS_Number);
            db.AddInParameter(cmd, "@DN_ADD_ADTP_Number", DbType.Int32, DN_DTO.Header.DN_ADD_ADTP_Number);


            return db.ExecuteDataSet(cmd);
        }

        public DataSet DeliveryNoteSummaryDB(DeliveryNoteSummary_DTO DN_DTO)
        {
            Database db = new SqlDatabase(DB.Connection());
            DbCommand cmd = db.GetStoredProcCommand("JI_DeliveryNote_Summary_SP");

            //   int DN_Id = 10; // INSERT MODE

            // 🔹 Mode
            db.AddInParameter(cmd, "@DN_Id", DbType.Int32, DN_DTO.DN_Id);

           // DN_DTO.JIDNH_DN_Date = DateTime.Now;
           // db.AddInParameter(cmd, "@JIDNH_DN_Date", DbType.Date, DN_DTO.JIDNH_DN_Date);



            return db.ExecuteDataSet(cmd);
        }


        public DataSet DeliveryNoteCreateDB(DeliveryNoteCreate_DTO DN_DTO)
        {
            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(DB.Connection()))
            {
                con.Open();

                using (SqlTransaction tr = con.BeginTransaction())
                {
                    try
                    {
                        //-------------------------------------------------
                        // HEADER INSERT
                        //-------------------------------------------------

                        long DN_Number = 0;

                        using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO JI_DeliveryNoteHead
                    (
                        JIDNH_DN_No,
                        JIDNH_DN_Date,
                        JIDNH_MS_Number,
                        JIDNH_JW_Customer_Number,
                        JIDNH_Currency_Number,
                        JIDNH_WH_Number,
                        JIDNH_PaymentTerms,
                        JIDNH_DeliveryTerms,
                        JIDNH_DeliveryMode,
                        JIDNH_DespatchDocumentNo,
                        JIDNH_DespatchedThrough,
                        JIDNH_Remarks
                    )

                    OUTPUT INSERTED.JIDNH_Number

                    VALUES
                    (
                        @JIDNH_DN_No,
                        @JIDNH_DN_Date,
                        @JIDNH_MS_Number,
                        @JIDNH_JW_Customer_Number,
                        @JIDNH_Currency_Number,
                        @JIDNH_WH_Number,
                        @JIDNH_PaymentTerms,
                        @JIDNH_DeliveryTerms,
                        @JIDNH_DeliveryMode,
                        @JIDNH_DespatchDocumentNo,
                        @JIDNH_DespatchedThrough,
                        @JIDNH_Remarks
                    )", con, tr))
                        {
                            var h = DN_DTO.Header;

                            cmd.Parameters.AddWithValue("@JIDNH_DN_No", h.JIDNH_DN_No);
                            cmd.Parameters.AddWithValue("@JIDNH_DN_Date", h.JIDNH_DN_Date);
                            cmd.Parameters.AddWithValue("@JIDNH_MS_Number", h.JIDNH_MS_Number);
                            cmd.Parameters.AddWithValue("@JIDNH_JW_Customer_Number", h.JIDNH_JW_Customer_Number);
                            cmd.Parameters.AddWithValue("@JIDNH_Currency_Number", h.JIDNH_Currency_Number);
                            cmd.Parameters.AddWithValue("@JIDNH_WH_Number", h.JIDNH_WH_Number);
                            cmd.Parameters.AddWithValue("@JIDNH_PaymentTerms", h.JIDNH_PaymentTerms ?? "");
                            cmd.Parameters.AddWithValue("@JIDNH_DeliveryTerms", h.JIDNH_DeliveryTerms ?? "");
                            cmd.Parameters.AddWithValue("@JIDNH_DeliveryMode", h.JIDNH_DeliveryMode ?? "");
                            cmd.Parameters.AddWithValue("@JIDNH_DespatchDocumentNo", h.JIDNH_DespatchDocumentNo ?? "");
                            cmd.Parameters.AddWithValue("@JIDNH_DespatchedThrough", h.JIDNH_DespatchedThrough ?? "");
                            cmd.Parameters.AddWithValue("@JIDNH_Remarks", h.JIDNH_Remarks ?? "");

                            DN_Number = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        //-------------------------------------------------
                        // ITEM INSERT
                        //-------------------------------------------------

                        List<ItemMapDTO> insertedItems =
                            new List<ItemMapDTO>();

                        foreach (var item in DN_DTO.Items)
                        {
                            long insertedItemNumber = 0;

                            using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO JI_DeliveryNoteItem
                        (
                            JIDNI_JIDNH_Number,
                            JIDNI_PRS_Number,
                            JIDNI_Item_Number,
                            JIDNI_WH_Number,
                            JIDNI_UoM_Number,
                            JIDNI_Qty,
                            JIDNI_UnitPrice,
                            JIDNI_Amount,
                            JIDNI_JW_InvoiceTracking
                        )

                        OUTPUT INSERTED.JIDNI_Number

                        VALUES
                        (
                            @JIDNI_JIDNH_Number,
                            @JIDNI_PRS_Number,
                            @JIDNI_Item_Number,
                            @JIDNI_WH_Number,
                            @JIDNI_UoM_Number,
                            @JIDNI_Qty,
                            @JIDNI_UnitPrice,
                            @JIDNI_Amount,
                            @JIDNI_JW_InvoiceTracking
                        )", con, tr))
                            {
                                cmd.Parameters.AddWithValue("@JIDNI_JIDNH_Number", DN_Number);
                                cmd.Parameters.AddWithValue("@JIDNI_PRS_Number", item.JIDNI_PRS_Number);
                                cmd.Parameters.AddWithValue("@JIDNI_Item_Number", item.JIDNI_Item_Number);
                                cmd.Parameters.AddWithValue("@JIDNI_WH_Number", item.JIDNI_WH_Number);
                                cmd.Parameters.AddWithValue("@JIDNI_UoM_Number", item.JIDNI_UoM_Number);
                                cmd.Parameters.AddWithValue("@JIDNI_Qty", item.JIDNI_Qty);
                                cmd.Parameters.AddWithValue("@JIDNI_UnitPrice", item.JIDNI_UnitPrice);
                                cmd.Parameters.AddWithValue("@JIDNI_Amount", item.JIDNI_Amount);
                                cmd.Parameters.AddWithValue("@JIDNI_JW_InvoiceTracking", item.JIDNI_JW_InvoiceTracking);

                                insertedItemNumber =
                                    Convert.ToInt64(cmd.ExecuteScalar());
                            }

                            insertedItems.Add(new ItemMapDTO
                            {
                                ItemNumber = insertedItemNumber,
                                Qty = Convert.ToDecimal(item.JIDNI_Qty)
                            });
                        }

                        //-------------------------------------------------
                        // BATCH INSERT
                        //-------------------------------------------------

                        int batchIndex = 0;

                        foreach (var item in insertedItems)
                        {
                            decimal balanceQty = item.Qty;

                            while (balanceQty > 0 &&
                                   batchIndex < DN_DTO.deliveryNoteBatches.Count)
                            {
                                var batch =
                                    DN_DTO.deliveryNoteBatches[batchIndex];

                                decimal useQty = 0;

                                if (batch.JIDNI_BCH_BatchQty <= balanceQty)
                                {
                                    useQty =
                                        batch.JIDNI_BCH_BatchQty;

                                    batchIndex++;
                                }
                                else
                                {
                                    useQty = balanceQty;

                                    DN_DTO.deliveryNoteBatches[batchIndex]
                                        .JIDNI_BCH_BatchQty -= balanceQty;
                                }

                                //-------------------------------------------------
                                // DELIVERY NOTE BATCH
                                //-------------------------------------------------

                                long batchNumber = 0;

                                using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO JI_DeliveryNoteBatch
                            (
                                JIDNI_BCH_JIDNH_Number,
                                JIDNI_BCH_JIDNI_Number,
                                JIDNI_BCH_WH_Number,
                                JIDNI_BCH_BatchDate,
                                JIDNI_BCH_BatchNo,
                                JIDNI_BCH_BatchQty,
                                JIDNI_BCH_BatchUnitPrice,
                                JIDNI_BCH_BatchValue,
                                RefBatch_Number
                            )

                            OUTPUT INSERTED.JIDNI_BCH_Number

                            VALUES
                            (
                                @JIDNH,
                                @JIDNI,
                                @WH,
                                @BatchDate,
                                @BatchNo,
                                @Qty,
                                @UnitPrice,
                                @BatchValue,
                                @RefBatchNumber
                            )", con, tr))
                                {
                                    cmd.Parameters.AddWithValue("@JIDNH", DN_Number);
                                    cmd.Parameters.AddWithValue("@JIDNI", item.ItemNumber);
                                    cmd.Parameters.AddWithValue("@WH", batch.JIDNI_BCH_WH_Number);
                                    cmd.Parameters.AddWithValue("@BatchDate", batch.JIDNI_BCH_BatchDate);
                                    cmd.Parameters.AddWithValue("@BatchNo", batch.JIDNI_BCH_BatchNo);
                                    cmd.Parameters.AddWithValue("@Qty", useQty);
                                    cmd.Parameters.AddWithValue("@UnitPrice", batch.JIDNI_BCH_BatchUnitPrice);
                                    cmd.Parameters.AddWithValue("@BatchValue", batch.JIDNI_BCH_BatchValue);
                                     
                                    cmd.Parameters.AddWithValue("@RefBatchNumber", batch.JIDNI_BCH_Number);
                                    batchNumber =
                                        Convert.ToInt64(cmd.ExecuteScalar());
                                }

                                //-------------------------------------------------
                                // OUT COMMON BATCH
                                //-------------------------------------------------
                                long outcommon = 0;
                                using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO OUT_COMMON_BATCH
                            (
                                TransType,
                                Header_Number,
                                LineItem_Number,
                                LineBatch_Number,
                                Warehouse,
                                BatchDate,
                                BatchNo,
                                ItemStatus,
                                BatchQty,
                                BatchUnitPrice,
                                BatchValue,
                                RefBatch_Number
                            )

                            VALUES
                            (
                                @TransType,
                                @Header_Number,
                                @LineItem_Number,
                                @LineBatch_Number,
                                @Warehouse,
                                @BatchDate,
                                @BatchNo,
                                @ItemStatus,
                                @BatchQty,
                                @BatchUnitPrice,
                                @BatchValue,
                                @RefBatchNumber
                            )", con, tr))
                                {
                                    cmd.Parameters.AddWithValue("@TransType", "Delivery Note");
                                    cmd.Parameters.AddWithValue("@Header_Number", DN_Number);
                                    cmd.Parameters.AddWithValue("@LineItem_Number", item.ItemNumber);
                                    cmd.Parameters.AddWithValue("@LineBatch_Number", batchNumber);
                                    cmd.Parameters.AddWithValue("@Warehouse", batch.JIDNI_BCH_WH_Number);
                                    cmd.Parameters.AddWithValue("@BatchDate", batch.JIDNI_BCH_BatchDate);
                                    cmd.Parameters.AddWithValue("@BatchNo", batch.JIDNI_BCH_BatchNo);
                                    cmd.Parameters.AddWithValue("@ItemStatus", "Good");
                                    cmd.Parameters.AddWithValue("@BatchQty", useQty);
                                    cmd.Parameters.AddWithValue("@BatchUnitPrice", batch.JIDNI_BCH_BatchUnitPrice);
                                    cmd.Parameters.AddWithValue("@BatchValue", batch.JIDNI_BCH_BatchValue);
                                    cmd.Parameters.AddWithValue("@RefBatchNumber", batch.JIDNI_BCH_Number);

                                    outcommon =
                                      Convert.ToInt64(cmd.ExecuteScalar());
                                }

                                balanceQty -= useQty;
                            }
                        }

                        //-------------------------------------------------
                        // ADDRESS INSERT
                        //-------------------------------------------------

                        foreach (var addr in DN_DTO.Addresses)
                        {
                            using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO JI_DeliveryNoteAddress
                        (
                            JIDNA_JIDNH_Number,
                            JIDNA_ADTP_Number,
                            JIDNA_Address_ID,
                            JIDNA_Address,
                            JIDNA_City,
                            JIDNA_State,
                            JIDNA_Country,
                            JIDNA_PIN,
                            JIDNA_GSTIN
                        )

                        VALUES
                        (
                            @JIDNH,
                            @ADTP,
                            @AddressID,
                            @Address,
                            @City,
                            @State,
                            @Country,
                            @PIN,
                            @GSTIN
                        )", con, tr))
                            {
                                cmd.Parameters.AddWithValue("@JIDNH", DN_Number);
                                cmd.Parameters.AddWithValue("@ADTP", addr.JIDNA_ADTP_Number);
                                cmd.Parameters.AddWithValue("@AddressID", addr.JIDNA_Address_ID);
                                cmd.Parameters.AddWithValue("@Address", addr.JIDNA_Address ?? "");
                                cmd.Parameters.AddWithValue("@City", addr.JIDNA_City ?? "");
                                cmd.Parameters.AddWithValue("@State", addr.JIDNA_State ?? "");
                                cmd.Parameters.AddWithValue("@Country", addr.JIDNA_Country ?? "");
                                cmd.Parameters.AddWithValue("@PIN", addr.JIDNA_PIN ?? "");
                                cmd.Parameters.AddWithValue("@GSTIN", addr.JIDNA_GSTIN ?? "");

                                cmd.ExecuteNonQuery();
                            }
                        }

                        //-------------------------------------------------
                        // CLEAR TEMP
                        //-------------------------------------------------

                        using (SqlCommand cmd = new SqlCommand(@"
                    DELETE FROM temp_DeliveryNoteBatch
                ", con, tr))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        //-------------------------------------------------
                        // COMMIT
                        //-------------------------------------------------

                        tr.Commit();
                    }
                    catch
                    {
                        tr.Rollback();
                        throw;
                    }
                }
            }

            return ds;
        }

        public class ItemMapDTO
        {
            public long ItemNumber { get; set; }

            public decimal Qty { get; set; }
        }
        private DataTable ToDeliveryNoteHeaderTable(DeliveryNoteHeader_DTO h)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JIDNH_Number", typeof(long));
            dt.Columns.Add("JIDNH_DN_No", typeof(string));
            dt.Columns.Add("JIDNH_DN_Date", typeof(DateTime));
            dt.Columns.Add("JIDNH_MS_Number", typeof(long));
            dt.Columns.Add("JIDNH_JW_Customer_Number", typeof(long));
            dt.Columns.Add("JIDNH_Currency_Number", typeof(long));
            dt.Columns.Add("JIDNH_WH_Number", typeof(long));
            dt.Columns.Add("JIDNH_PaymentTerms", typeof(string));
            dt.Columns.Add("JIDNH_DeliveryTerms", typeof(string));
            dt.Columns.Add("JIDNH_DeliveryMode", typeof(string));
            dt.Columns.Add("JIDNH_DespatchDocumentNo", typeof(string));
            dt.Columns.Add("JIDNH_DespatchedThrough", typeof(string));
            dt.Columns.Add("JIDNH_Remarks", typeof(string));

            dt.Rows.Add(
                h.JIDNH_Number,
                h.JIDNH_DN_No,
                h.JIDNH_DN_Date,
                h.JIDNH_MS_Number,
                h.JIDNH_JW_Customer_Number,
                h.JIDNH_Currency_Number,
                h.JIDNH_WH_Number,
                h.JIDNH_PaymentTerms,
                h.JIDNH_DeliveryTerms,
                h.JIDNH_DeliveryMode,
                h.JIDNH_DespatchDocumentNo,
                h.JIDNH_DespatchedThrough,
                h.JIDNH_Remarks
            );

            return dt;
        }

        private DataTable ToDeliveryNoteItemTable(List<DeliveryNoteItem_DTO> items)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JIDNI_JIDNH_Number", typeof(long));
            dt.Columns.Add("JIDNI_Number", typeof(long));
            dt.Columns.Add("JIDNI_PRS_Number", typeof(long));
            dt.Columns.Add("JIDNI_Item_Number", typeof(long));
            dt.Columns.Add("JIDNI_WH_Number", typeof(long));
            dt.Columns.Add("JIDNI_UoM_Number", typeof(long));
            dt.Columns.Add("JIDNI_Qty", typeof(double));
            dt.Columns.Add("JIDNI_UnitPrice", typeof(double));
            dt.Columns.Add("JIDNI_Amount", typeof(double));
            dt.Columns.Add("JIDNI_JW_InvoiceTracking", typeof(string));

            foreach (var i in items)
            {
                dt.Rows.Add(
                    i.JIDNI_JIDNH_Number,
                    i.JIDNI_Number,
                    i.JIDNI_PRS_Number,
                    i.JIDNI_Item_Number,
                    i.JIDNI_WH_Number,
                    i.JIDNI_UoM_Number,
                    i.JIDNI_Qty,
                    i.JIDNI_UnitPrice,
                    i.JIDNI_Amount,
                    i.JIDNI_JW_InvoiceTracking
                );
            }

            return dt;
        }

        private DataTable ToDeliveryNoteAddressTable(List<DeliveryNoteAddress_DTO> addresses)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JIDNA_JIDNH_Number", typeof(long));
            dt.Columns.Add("JIDNA_Number", typeof(long));
            dt.Columns.Add("JIDNA_ADTP_Number", typeof(long));
            dt.Columns.Add("JIDNA_Address_ID", typeof(string));
            dt.Columns.Add("JIDNA_Address", typeof(string));
            dt.Columns.Add("JIDNA_City", typeof(string));
            dt.Columns.Add("JIDNA_State", typeof(string));
            dt.Columns.Add("JIDNA_Country", typeof(string));
            dt.Columns.Add("JIDNA_PIN", typeof(string));
            dt.Columns.Add("JIDNA_GSTIN", typeof(string));

            foreach (var a in addresses)
            {
                dt.Rows.Add(
                    a.JIDNA_JIDNH_Number,
                    a.JIDNA_Number,
                    a.JIDNA_ADTP_Number,
                    a.JIDNA_Address_ID,
                    a.JIDNA_Address,
                    a.JIDNA_City,
                    a.JIDNA_State,
                    a.JIDNA_Country,
                    a.JIDNA_PIN,
                    a.JIDNA_GSTIN
                );
            }

            return dt;
        }

        private DataTable ToDeliveryNoteBatchTable(List<DeliveryNoteBatch_DTO> batches)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("JIDNI_BCH_Number", typeof(long));
            dt.Columns.Add("JIDNI_BCH_JIDNH_Number", typeof(long));
            dt.Columns.Add("JIDNI_BCH_JIDNI_Number", typeof(long));
            dt.Columns.Add("JIDNI_BCH_WH_Number", typeof(long));
            dt.Columns.Add("JIDNI_BCH_BatchDate", typeof(DateTime));
            dt.Columns.Add("JIDNI_BCH_BatchNo", typeof(string));
            dt.Columns.Add("JIDNI_BCH_BatchQty", typeof(decimal));
            dt.Columns.Add("JIDNI_BCH_BatchUnitPrice", typeof(decimal));
            dt.Columns.Add("JIDNI_BCH_BatchValue", typeof(decimal));

            foreach (var b in batches)
            {
                dt.Rows.Add(
                    b.JIDNI_BCH_Number,
                    b.JIDNI_BCH_JIDNH_Number,
                    b.JIDNI_BCH_JIDNI_Number,
                    b.JIDNI_BCH_WH_Number,
                    b.JIDNI_BCH_BatchDate,
                    b.JIDNI_BCH_BatchNo,
                    b.JIDNI_BCH_BatchQty,
                    b.JIDNI_BCH_BatchUnitPrice,
                    b.JIDNI_BCH_BatchValue
                );
            }

            return dt;
        }


        public DataSet GetOtherBatchDetailsDB(long fromWarehouse, long lineItemNumber, int ItemGridIndex)
        {
            try
            {
                Database db = new SqlDatabase(DB.Connection());

                DbCommand cmd = db.GetStoredProcCommand("JI_DeliveryNote_InCommonOtherBatch_GetBatchDetails_SP");

                db.AddInParameter(cmd, "@FromWarehouse", DbType.Int64, fromWarehouse);

                db.AddInParameter(cmd, "@LineItem_Number", DbType.Int64, lineItemNumber);

                db.AddInParameter(cmd, "@ItemGridIndex", DbType.Int64, ItemGridIndex);

                return db.ExecuteDataSet(cmd);
            }
            catch (SqlException ex)
            {
                throw new Exception(
                    "SQL Error : " + ex.Message +
                    Environment.NewLine +
                    "Procedure : JI_DeliveryNote_InCommonOtherBatch_GetBatchDetails_SP",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Application Error : " + ex.Message,
                    ex
                );
            }
        }

        public DataSet GetBatchDetailsViewDB(long fromWarehouse, long lineItemNumber)
        {
            try
            {
                Database db = new SqlDatabase(DB.Connection());

                DbCommand cmd = db.GetStoredProcCommand("JI_DeliveryNote_InCommonBatch_GetBatchView_SP");

                db.AddInParameter(cmd, "@FromWarehouse", DbType.Int64, fromWarehouse);

                db.AddInParameter(cmd, "@LineItem_Number", DbType.Int64, lineItemNumber);

           

                return db.ExecuteDataSet(cmd);
            }
            catch (SqlException ex)
            {
                throw new Exception(
                    "SQL Error : " + ex.Message +
                    Environment.NewLine +
                    "Procedure : JI_DeliveryNote_InCommonBatch_GetBatchDetails_SP",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Application Error : " + ex.Message,
                    ex
                );
            }
        }


        public DataSet GetBatchDetailsDB(long fromWarehouse, long lineItemNumber,int ItemGridIndex)
        {
            try
            {
                Database db = new SqlDatabase(DB.Connection());

                DbCommand cmd = db.GetStoredProcCommand("JI_DeliveryNote_InCommonBatch_GetBatchDetails_SP");

                db.AddInParameter(cmd, "@FromWarehouse", DbType.Int64, fromWarehouse);

                db.AddInParameter(cmd, "@LineItem_Number", DbType.Int64, lineItemNumber);

                db.AddInParameter(cmd, "@ItemGridIndex", DbType.Int64, ItemGridIndex);

                return db.ExecuteDataSet(cmd);
            }
            catch (SqlException ex)
            {
                throw new Exception(
                    "SQL Error : " + ex.Message +
                    Environment.NewLine +
                    "Procedure : JI_DeliveryNote_InCommonBatch_GetBatchDetails_SP",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Application Error : " + ex.Message,
                    ex
                );
            }
        }
        public DataSet DeliveryNoteBatchSaveDB(List<DeliveryNoteBatch_DTO> batchList)
        {
            using (SqlConnection con = new SqlConnection(DB.Connection()))
            using (SqlCommand cmd = new SqlCommand("JI_DeliveryNoteBatch_Save_SP", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable dt = new DataTable();

                dt.Columns.Add("JIDNI_BCH_Number", typeof(long));
                dt.Columns.Add("JIDNI_BCH_JIDNH_Number", typeof(long));
                dt.Columns.Add("JIDNI_BCH_JIDNI_Number", typeof(long));
                dt.Columns.Add("JIDNI_BCH_WH_Number", typeof(long));
                dt.Columns.Add("JIDNI_BCH_BatchDate", typeof(DateTime));
                dt.Columns.Add("JIDNI_BCH_BatchNo", typeof(string));
                dt.Columns.Add("JIDNI_BCH_BatchQty", typeof(decimal));
                dt.Columns.Add("JIDNI_BCH_BatchUnitPrice", typeof(decimal));
                dt.Columns.Add("JIDNI_BCH_BatchValue", typeof(decimal));

                foreach (var x in batchList)
                {
                    dt.Rows.Add(
                        x.JIDNI_BCH_Number,
                        x.JIDNI_BCH_JIDNH_Number,
                        x.JIDNI_BCH_JIDNI_Number,
                        x.JIDNI_BCH_WH_Number,
                        x.JIDNI_BCH_BatchDate,
                        x.JIDNI_BCH_BatchNo,
                        x.JIDNI_BCH_BatchQty,
                        x.JIDNI_BCH_BatchUnitPrice,
                        x.JIDNI_BCH_BatchValue
                    );
                }

                SqlParameter param = cmd.Parameters.AddWithValue("@BatchList", dt);

                param.SqlDbType = SqlDbType.Structured;
                param.TypeName = "JI_DeliveryNoteBatch_Type";

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataSet ds = new DataSet();

                da.Fill(ds);

                return ds;
            }
        }
        public DataSet OutCommonBatchSaveDB(OutCommonBatch_DTO dto)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("SP_OUT_COMMON_BATCH_INSERT");

            db.AddInParameter(cmd, "@TransType", DbType.String, dto.TransType);
            db.AddInParameter(cmd, "@Header_Number", DbType.Int64, dto.Header_Number);
            db.AddInParameter(cmd, "@LineItem_Number", DbType.Int64, dto.LineItem_Number);
            db.AddInParameter(cmd, "@LineBatch_Number", DbType.Int64, dto.LineBatch_Number);
            db.AddInParameter(cmd, "@Warehouse", DbType.Int64, dto.Warehouse);
            db.AddInParameter(cmd, "@BatchDate", DbType.Date, dto.BatchDate);
            db.AddInParameter(cmd, "@BatchNo", DbType.String, dto.BatchNo);
            db.AddInParameter(cmd, "@ItemStatus", DbType.String, dto.ItemStatus);
            db.AddInParameter(cmd, "@BatchQty", DbType.Decimal, dto.BatchQty);
            db.AddInParameter(cmd, "@BatchUnitPrice", DbType.Decimal, dto.BatchUnitPrice);
            db.AddInParameter(cmd, "@BatchValue", DbType.Decimal, dto.BatchValue);

            return db.ExecuteDataSet(cmd);
        }

        public void TempDeliveryBatchSaveDB(List<TempDeliveryBatch_DTO> list)
        {
            if (list == null || list.Count == 0)
                return;

            using (SqlConnection con = new SqlConnection(DB.Connection()))
            {
                con.Open();

                using (SqlTransaction tr = con.BeginTransaction())
                {
                    try
                    {
                        // We assume all rows belong to same group
                        var first = list.First();

                        // =========================
                        // 1. DELETE MATCHING RECORDS
                        //AND DBCH_Item_Number = @ItemNo
                        //AND DBCH_Warehouse_Number = @WH
                        // =========================
                        using (SqlCommand delCmd = new SqlCommand(@"
                    DELETE FROM Temp_DeliveryNoteBatch
                    WHERE DBCH_Index = @DBCH_Index
                     
                ", con, tr))
                        {
                            delCmd.Parameters.AddWithValue("@DBCH_Index", first.DBCH_Index);
                            delCmd.Parameters.AddWithValue("@ItemNo", first.DBCH_Item_Number);
                            delCmd.Parameters.AddWithValue("@WH", first.DBCH_Warehouse_Number);

                            delCmd.ExecuteNonQuery();
                        }

                        // =========================
                        // 2. INSERT NEW ROWS
                        // =========================
                        foreach (var obj in list)
                        {
                            using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Temp_DeliveryNoteBatch
                        (
                            DBCH_Index,
                            DBCH_DBCH_Number,
                            DBCH_Item_Number,
                            DBCH_Warehouse_Number,
                            DBCH_Date,
                            DBCH_No,
                            DBCH_Qty,
                            DBCH_UnitPrice,
                            DBCH_Value,
                            Mode,
                            CreatorCode,
                            CreatorDate
                        )
                        VALUES
                        (
                            @DBCH_Index,
                            @DBCH_DBCH_Number,
                            @DBCH_Item_Number,
                            @DBCH_Warehouse_Number,
                            @DBCH_Date,
                            @DBCH_No,
                            @DBCH_Qty,
                            @DBCH_UnitPrice,
                            @DBCH_Value,
                            @Mode,
                            @CreatorCode,
                            @CreatorDate
                        )
                    ", con, tr))
                            {
                                cmd.Parameters.AddWithValue("@DBCH_Index", obj.DBCH_Index);
                                cmd.Parameters.AddWithValue("@DBCH_DBCH_Number", (object?)obj.DBCH_DBCH_Number ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@DBCH_Item_Number", obj.DBCH_Item_Number);
                                cmd.Parameters.AddWithValue("@DBCH_Warehouse_Number", obj.DBCH_Warehouse_Number);
                                cmd.Parameters.AddWithValue("@DBCH_Date", obj.DBCH_Date);
                                cmd.Parameters.AddWithValue("@DBCH_No", (object?)obj.DBCH_No ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@DBCH_Qty", obj.DBCH_Qty);
                                cmd.Parameters.AddWithValue("@DBCH_UnitPrice", (object?)obj.DBCH_UnitPrice ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@DBCH_Value", (object?)obj.DBCH_Value ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Mode", obj.Mode ?? 1);
                                cmd.Parameters.AddWithValue("@CreatorCode", obj.CreatorCode);
                                cmd.Parameters.AddWithValue("@CreatorDate", obj.CreatorDate);

                                cmd.ExecuteNonQuery();
                            }
                        }

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

        public void TempDeliveryBatchDeleteChangeItemDBRow(int index)
        {
            using (SqlConnection con = new SqlConnection(DB.Connection()))
            {
                con.Open();

                using (SqlTransaction tr = con.BeginTransaction())
                {
                    try
                    {

                        // =========================
                        // 3. DELETE INDEX GROUP
                        // =========================
                        using (SqlCommand delCmd = new SqlCommand(@"
                    DELETE FROM Temp_DeliveryNoteBatch
                    WHERE DBCH_Index = @DBCH_Index;
                ", con, tr))
                        {
                            delCmd.Parameters.AddWithValue("@DBCH_Index", index);
                            delCmd.ExecuteNonQuery();
                        }

                        

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

        public void TempDeliveryBatchDeleteDBRow(int index)
        {
            using (SqlConnection con = new SqlConnection(DB.Connection()))
            {
                con.Open();

                using (SqlTransaction tr = con.BeginTransaction())
                {
                    try
                    { 

                        // =========================
                        // 3. DELETE INDEX GROUP
                        // =========================
                        using (SqlCommand delCmd = new SqlCommand(@"
                    DELETE FROM Temp_DeliveryNoteBatch
                    WHERE DBCH_Index = @DBCH_Index;
                ", con, tr))
                        {
                            delCmd.Parameters.AddWithValue("@DBCH_Index", index);
                            delCmd.ExecuteNonQuery();
                        }

                        // =========================
                        // 4. RESEQUENCE INDEX GROUPS
                        // =========================
                        using (SqlCommand seqCmd = new SqlCommand(@"
                    ;WITH Grouped AS
                    (
                        SELECT DISTINCT DBCH_Index
                        FROM Temp_DeliveryNoteBatch
                    ),
                    Renumber AS
                    (
                        SELECT 
                            DBCH_Index,
                            ROW_NUMBER() OVER (ORDER BY DBCH_Index) AS NewIndex
                        FROM Grouped
                    )
                    UPDATE t
                    SET t.DBCH_Index = r.NewIndex
                    FROM Temp_DeliveryNoteBatch t
                    JOIN Renumber r
                        ON t.DBCH_Index = r.DBCH_Index;
                ", con, tr))
                        {
                            seqCmd.ExecuteNonQuery();
                        }

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

    }
}
