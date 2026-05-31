using DocumentFormat.OpenXml.Wordprocessing;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DL;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Transactions;

namespace ERP.Controllers.Sale
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class SaleRejectionController : Controller
    {
        CultureInfo India = new CultureInfo("hi-IN");
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();


        SaleRejection_DL S_DL = new SaleRejection_DL();
        SaleRejection_DAO SR_DAO = new SaleRejection_DAO();
        SaleRejection_DTO SR_DTO = new SaleRejection_DTO();
        SaleRejectionHead_DTO SRH_DTO = new SaleRejectionHead_DTO();

        SaleRejectionRegister_DTO SRR_DTO = new SaleRejectionRegister_DTO();
        List<SaleRejectionRegister_DTO> SRR_List = new List<SaleRejectionRegister_DTO>();


        //SI To SR
        SIToSaleRejectionHead_DTO SISRH_DTO = new SIToSaleRejectionHead_DTO();
        SIToSaleRejection_DAO SISR_DAO = new SIToSaleRejection_DAO();
        SIToSaleRejection_DTO SISR_DTO = new SIToSaleRejection_DTO();


        //SI Item To SR
        SIItemToSaleRejectionHead_DTO SIISRH_DTO = new SIItemToSaleRejectionHead_DTO();
        SIItemToSaleRejection_DAO SIISR_DAO = new SIItemToSaleRejection_DAO();
        SIItemToSaleRejection_DTO SIISR_DTO = new SIItemToSaleRejection_DTO();


        SRNumber_DTO SRN_DTO = new SRNumber_DTO();
        SRNumber_DAO SRN_DAO = new SRNumber_DAO();
        SRNumbering_DL SRN_DL = new SRNumbering_DL();
        List<SRNumberPrefix_DTO> SRP_List = new List<SRNumberPrefix_DTO>();
        List<SRNumberSuffix_DTO> SRS_List = new List<SRNumberSuffix_DTO>();

        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;



        //Sale Rejection Summary
        [Route("sale/transactions/sale-rejection-register-summary")]
        public IActionResult SaleRejectionRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            SRR_List = SRSummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<SaleRejectionRegister_DTO>.CreateAsync(SRR_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/transactions/sale-rejection-register-summary")]
        [HttpPost]
        public IActionResult SaleRejectionRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? SR_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            if (Mode == "DeleteAll")
            {
                SR_DTO.SR_DeleteNumbers = DeleteNumbers;
                SR_DTO.SR_Id = 31;
                SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
                DS = SR_DAO.SaleRejectionDB(SR_DTO);
            }
            else if (Mode == "Ascii" || Mode == "Excel" || Mode == "PDF")
            {
                //List<Int64> AllowedPoNumbers = new List<Int64>();
                //if (DeleteNumber.Length > 0)
                //{
                //    try
                //    {
                //        AllowedPoNumbers = DeleteNumber
                //            .Select(s => s.Trim())
                //            .Where(s => !string.IsNullOrEmpty(s))
                //            .Select(Int64.Parse)
                //            .ToList();
                //    }
                //    catch
                //    {
                //    }
                //}
                //HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

                //SRR_DTO.SIH_Id = 21;
                //SRR_DTO.SIH_CreatorCode = Convert.ToInt32(UserCode);
                //DS = SR_DAO.SaleRejectionDB(SRR_DTO);

                //if (Mode == "Ascii")
                //{
                //    List<SaleRejectionAscii> SRR_List = P_DL.POAscii(DS.Tables[0]);

                //    var Key = SRR_List;
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SIH_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    Decimal TotalQtySum = 0;
                //    Decimal TotalMaterialValueSum = 0;
                //    Decimal TotalItemMiscIncomeSum = 0;
                //    Decimal TotalHeadMiscIncomeSum = 0;

                //    var HeaderRow = typeof(SaleRejectionAscii)
                //            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                //            .Where(prop => prop.Name != nameof(SaleRejectionAscii.SIH_Number))
                //            .Select(prop =>
                //                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                //             )
                //            .ToList();

                //    var AsciiData = new StringBuilder();
                //    AsciiData.AppendLine(string.Join("\t", HeaderRow));



                //    PropertyInfo[] PropertiesToInclude = typeof(SaleRejectionAscii)
                //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                //        .Where(prop => prop.Name != nameof(SaleRejectionAscii.SIH_Number))
                //        .ToArray();

                //    foreach (var item in Key)
                //    {
                //        var rowValues = PropertiesToInclude
                //            .Select(prop => prop.GetValue(item)?.ToString() ?? "")
                //            .ToList();

                //        var escapedRowValues = rowValues
                //            .Select(val => val.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))
                //            .ToList();

                //        AsciiData.AppendLine(string.Join("\t", escapedRowValues));

                //        if (Decimal.TryParse(item.TotalQty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //        {
                //            TotalQtySum += QtyValue;
                //        }
                //        if (Decimal.TryParse(item.MaterialValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //        {
                //            TotalMaterialValueSum += MaterialValue;
                //        }
                //        if (Decimal.TryParse(item.ItemMiscIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //        {
                //            TotalItemMiscIncomeSum += ItemMiscValue;
                //        }
                //        if (Decimal.TryParse(item.HeadMiscIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //        {
                //            TotalHeadMiscIncomeSum += HeadMiscValue;
                //        }
                //    }

                //    List<String> FooterCells = new List<String>();
                //    Boolean TotalLabelAdded = false;
                //    foreach (var prop in PropertiesToInclude)
                //    {
                //        string FooterCellValue = "";

                //        if (!TotalLabelAdded && prop.Name != nameof(SaleRejectionAscii.TotalQty) &&
                //                                 prop.Name != nameof(SaleRejectionAscii.MaterialValue) &&
                //                                 prop.Name != nameof(SaleRejectionAscii.ItemMiscIncome) &&
                //                                 prop.Name != nameof(SaleRejectionAscii.HeadMiscIncome))
                //        {
                //            if (FooterCells.Count == 0)
                //            {
                //                FooterCellValue = "Total:";
                //                TotalLabelAdded = true;
                //            }
                //        }


                //        switch (prop.Name)
                //        {
                //            case nameof(SaleRejectionAscii.TotalQty):
                //                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleRejectionAscii.MaterialValue):
                //                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleRejectionAscii.ItemMiscIncome):
                //                FooterCellValue = TotalItemMiscIncomeSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleRejectionAscii.HeadMiscIncome):
                //                FooterCellValue = TotalHeadMiscIncomeSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //        }

                //        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                //    }

                //    AsciiData.AppendLine(string.Join("\t", FooterCells));

                //    String FileName = "PO-download";
                //    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                //    var contentType = "text/plain";
                //    var fileDownloadName = $"{FileName}.txt";
                //    return File(fileBytes, contentType, fileDownloadName);

                //}
                //else if (Mode == "Excel")
                //{
                //    SRR_List = S_DL.SIList(DS.Tables[0]);

                //    var Key = SRR_List.ToList();
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SIH_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    String FileName = "PO-download";
                //    using (var wb = new XLWorkbook())
                //    {
                //        var ws = wb.Worksheets.Add(SummaryDownload(Key.ToList()), "Sheet1");

                //        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //        ws.Style.Font.Bold = true;
                //        ws.Columns().AdjustToContents();

                //        using (var stream = new MemoryStream())
                //        {
                //            wb.SaveAs(stream);

                //            var content = stream.ToArray();
                //            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //            var fileDownloadName = $"{FileName}.xlsx";

                //            return File(content, contentType, fileDownloadName);
                //        }
                //    }
                //}
                //else if (Mode == "PDF")
                //{
                //    SRR_List = P_DL.POList(DS.Tables[0]);

                //    var Key = SRR_List.ToList();
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SIH_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    String PDFDownload = $@"<html>
                //    <head></head>
                //    <body>
                //        <style>
                //            html,body{{
                //                font-family: ""Noto Sans"", sans-serif;
                //                width: 100%;
                //                margin: 0;
                //                background-color: #fff;
                //            }}

                //            .table {{
                //                border-collapse: collapse;
                //                border: 1px SIlid #ccc;
                //                font-size: 0.6rem;
                //                font-family: ""Noto Sans"", sans-serif;
                //            }}
                //                .table th {{
                //                    border: 1px SIlid #ccc;
                //                    padding: 5px 8px;
                //                    vertical-align: top;
                //                background-color: #000;
                //                    color:#fff;
                //                }}

                //                .table td {{
                //                    border: 1px SIlid #ccc;
                //                    padding: 5px 8px;
                //                    vertical-align: top;
                //                }}

                //                .table th {{
                //                    font-weight: bold;
                //                }}

                //                .table tr td {{
                //                    height: 25px;
                //                }}
                //        </style>";

                //    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Buyer Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Export Order</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Amount</th><th style='width:70px'>Item Misc.Income Value</th><th style='width:70px'>Header Misc.Income Value</th></tr>";

                //    Decimal TotalQtySum = 0;
                //    Decimal TotalMaterialValueSum = 0;
                //    Decimal TotalItemMiscIncomeSum = 0;
                //    Decimal TotalHeadMiscIncomeSum = 0;

                //    if (Key.ToList().Count > 0)
                //    {
                //        foreach (var Row in Key.ToList())
                //        {
                //            String Export = "";
                //            Export = (Row.SR_ExportOrder.ToString() == "1") ? "Yes" : "No"; 

                //            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.MaterialValue));
                //            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscIncome));
                //            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscIncome));
                //            PDFDownload += $@"<tr>
                //            <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.SR_OrderDate).ToString("dd-MMM-yyyy")}</td>
                //            <td>{Row.SR_OrderNo}</td>
                //            <td>{Row.SR_BUY_Name}</td>
                //            <td style='width:30px;text-align:center'>{Row.SR_CUR_Name}</td>
                //            <td style='width:30px;text-align:center'>{Export}</td>
                //            <td>{Row.SR_MS_Name}</td>
                //            <td style='width:30px;text-align:center'>{Row.TotalItem}</td>
                //            <td style='width:30px;text-align:center'>{Row.TotalQty}</td>
                //            <td style='width:70px;text-align:right'>{Matrial}</td>
                //            <td style='width:70px;text-align:right'>{ItemExp}</td>
                //            <td style='width:70px;text-align:right'>{HeadExp}</td>
                //            </tr>";


                //            if (Decimal.TryParse(Row.SR_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //            {
                //                TotalQtySum += QtyValue;
                //            }
                //            if (Decimal.TryParse(Row.SR_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //            {
                //                TotalMaterialValueSum += MaterialValue;
                //            }
                //            if (Decimal.TryParse(Row.SR_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //            {
                //                TotalItemMiscIncomeSum += ItemMiscValue;
                //            }
                //            if (Decimal.TryParse(Row.SR_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //            {
                //                TotalHeadMiscIncomeSum += HeadMiscValue;
                //            }
                //        }
                //    }

                //    PDFDownload += $@"<tr>
                //            <td>Total</td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscIncomeSum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscIncomeSum))}</td>
                //            </tr>";


                //    PDFDownload += $@"</table></body></html>";

                //    HtmlToPdf converter = new HtmlToPdf();
                //    converter.Options.PdfPageSize = PdfPageSize.A4;
                //    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                //    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                //    converter.Options.MarginLeft = 10;
                //    converter.Options.MarginRight = 10;
                //    converter.Options.MarginTop = 10;
                //    converter.Options.MarginBottom = 10;

                //    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                //    MemoryStream memoryStream = new MemoryStream();
                //    doc.Save(memoryStream);
                //    doc.Close();

                //    return File(memoryStream.ToArray(), "application/pdf", "SR_Download.pdf");
                //}
            }
            else if (Mode == "View")
            {
                SR_DTO.SR_Number = Convert.ToInt32(SR_No);
                SR_DTO.SR_Id = 2;
                SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
                DS = SR_DAO.SaleRejectionDB(SR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PreviewSaleRejection", new { SR_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }
            else if (Mode == "Edit")
            {
                SR_DTO.SR_Number = Convert.ToInt32(SR_No);
                SR_DTO.SR_Id = 2;
                SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
                DS = SR_DAO.SaleRejectionDB(SR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (DS.Tables[0].Rows[0]["SRH_Mode"].ToString() == "1")
                    {
                        return RedirectToAction("EditSaleRejection", new { SR_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["SRH_Mode"].ToString() == "2")
                    {
                        return RedirectToAction("SIToEditSaleRejection", new { SR_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["SRH_Mode"].ToString() == "3")
                    {
                        return RedirectToAction("SIItemToEditSaleRejection", new { SR_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                }
            }

            SRR_List = SRSummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<SaleRejectionRegister_DTO>.CreateAsync(SRR_List, DPageNumber ?? 1, DPageSize));
        }
        
        DataTable SummaryDownload(List<SaleRejectionRegister_DTO> SRR_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "sale-invoice-summary";

            TableDown.Clear();
            TableDown.Columns.Add("Date");
            TableDown.Columns.Add("Order Number");
            TableDown.Columns.Add("Buyer Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Export Order");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("No. of Item");
            TableDown.Columns.Add("Qty");
            TableDown.Columns.Add("Amount");
            TableDown.Columns.Add("Item Misc Income Value");
            TableDown.Columns.Add("Head Misc Income Value");

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscIncomeSum = 0;
            Decimal TotalHeadMiscIncomeSum = 0;

            foreach (var Product in SRR_List)
            {
                //DataRow NewRow = TableDown.NewRow();
                //NewRow["Date"] = Product.SR_RejectionDate;
                //NewRow["Invoice Number"] = Product.SR_InvoiceNo;
                //NewRow["Buyer Name"] = Product.SR_BUY_Name;
                //NewRow["Currency"] = Product.SR_CUR_Name;
                //NewRow["Export Order"] = (Product.SR_ExportOrder.ToString() == "1") ? "Yes" : "No";
                //NewRow["Material Segregation"] = Product.SR_MS_Name;
                //NewRow["No. of Item"] = Product.SR_NoOfItem;
                //NewRow["Qty"] = Product.SR_Qty;
                //NewRow["Amount"] = Product.SR_TotalAmount;
                //NewRow["Item Misc Income Value"] = Product.SR_TotalItemIncome;
                //NewRow["Head Misc Income Value"] = Product.SR_TotalHeadIncome;

                //TableDown.Rows.Add(NewRow);


                //if (Decimal.TryParse(Product.SR_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //{
                //    TotalQtySum += QtyValue;
                //}
                //if (Decimal.TryParse(Product.SR_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //{
                //    TotalMaterialValueSum += MaterialValue;
                //}
                //if (Decimal.TryParse(Product.SR_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //{
                //    TotalItemMiscIncomeSum += ItemMiscValue;
                //}
                //if (Decimal.TryParse(Product.SR_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //{
                //    TotalHeadMiscIncomeSum += HeadMiscValue;
                //}
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["Date"] = "Total";
            NewRows["Order Number"] = "";
            NewRows["Buyer Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Export Order"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["No. of Item"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Amount"] = TotalMaterialValueSum;
            NewRows["Item Misc Income Value"] = TotalItemMiscIncomeSum;
            NewRows["Head Misc Income Value"] = TotalHeadMiscIncomeSum;
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }

        List<SaleRejectionRegister_DTO> SRSummaryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            SR_DTO.SR_Id = 1;
            SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            SRR_List = S_DL.SRSummaryList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
            }
            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }
            if (PageFilter?.ToLower() == "PageFilter".ToLower())
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSIrt"] = SortOrder;
            ViewData["KeySIrt"] = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["CurrentFilter"] = Search;

            var Key = SRR_List.OrderByDescending(Cs => Cs.SR_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.SR_RejectionDate.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SR_RejectionNo.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SR_BUY_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SR_CUR_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SR_MS_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SR_NoOfItem.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SR_Qty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SR_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.SR_RejectionDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.SR_RejectionDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.SR_Number);
                    break;
            }

            if (PSize != 0)
            {
                DPageSize = PSize;
            }
            Int32 Record = Key.ToList().Count;
            if (PageNumber > 1)
            {
                Int32 RecordPage = (Convert.ToInt32(PageNumber) - 1) * DPageSize;

                if (Record > RecordPage)
                {
                    if (Convert.ToInt32(PageNumber) == 0)
                    {
                        DPageNumber = 1;
                    }
                    else
                    {
                        DPageNumber = Convert.ToInt32(PageNumber);
                    }
                }
                else
                {
                    Double Page = Convert.ToDouble(Record) / Convert.ToDouble(DPageSize);
                    Int32 PageCount = Convert.ToInt32(Math.Ceiling(Page));
                    if (PageNumber > PageCount)
                    {
                        DPageNumber = Convert.ToInt32(PageCount);
                    }
                    else
                    {
                        DPageNumber = Convert.ToInt32(PageNumber);
                    }
                }
            }
            else
            {
                if (Convert.ToInt32(PageNumber) == 0)
                {
                    DPageNumber = 1;
                }
                else
                {
                    DPageNumber = Convert.ToInt32(PageNumber);
                }
            }

            Double Pages = Convert.ToDouble(Record) / Convert.ToDouble(DPageSize);
            Int32 PageCounts = Convert.ToInt32(Math.Ceiling(Pages));

            ViewBag.SumOfItem = Key.Sum(item => Convert.ToDouble(item.SR_NoOfItem));
            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.SR_Qty));
            ViewBag.SumOfMatrialValue = Key.Sum(item => Convert.ToDouble(item.SR_MaterialValue));
            ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.SR_TotalItemIncome));
            ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.SR_TotalHeadIncome));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.SR_TotalAmount));
            ViewBag.SumOfItemGst = Key.Sum(item => Convert.ToDouble(item.SR_ItemGST_Amount));
            ViewBag.SumOfHeadGst = Key.Sum(item => Convert.ToDouble(item.SR_HeadGST_Amount));
            ViewBag.SumOfInvoice = Key.Sum(item => Convert.ToDouble(item.SR_RejectionAmount));
            ViewBag.SumOfItemWHT = Key.Sum(item => Convert.ToDouble(item.SR_ItemWHT_Amount));
            ViewBag.SumOfHeadWHT = Key.Sum(item => Convert.ToDouble(item.SR_HeadWHT_Amount));
            ViewBag.SumOfRound = Key.Sum(item => Convert.ToDouble(item.SR_RoundOff));
            ViewBag.SumOfReceivable = Key.Sum(item => Convert.ToDouble(item.SR_BuyerReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        //[Route("sale/transactions/sale-invoice-register-summary/print")]
        //public String SISummaryPrint(String Search, String SelectedItem, bool AllItem)
        //{
        //    List<Int64> AllowedPoNumbers = new List<Int64>();
        //    if (!string.IsNullOrWhiteSpace(SelectedItem))
        //    {
        //        try
        //        {
        //            AllowedPoNumbers = SelectedItem.Split(',')
        //                .Select(s => s.Trim())
        //                .Where(s => !string.IsNullOrEmpty(s))
        //                .Select(Int64.Parse)
        //                .ToList();
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

        //    if (Search == null)
        //    {
        //        Search = "";
        //    }
        //    SRR_DTO.SR_Id = 21;
        //    SRR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
        //    DS = SR_DAO.SaleRejectionDB(SRR_DTO);
        //    SRR_List = S_DL.SISummaryList(DS.Tables[0]);

        //    var Key = SRR_List.OrderByDescending(Cs => Cs.SR_Number);

        //    if (AllItem)
        //    {
        //        if (!String.IsNullOrEmpty(Search))
        //        {
        //            Key = Key.Where(K => K.SR_RejectionDate!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_MS_Name!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_NoOfItem!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_Qty!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_TotalAmount!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SR_Number);
        //        }
        //    }
        //    else if (!string.IsNullOrWhiteSpace(SelectedItem))
        //    {
        //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SR_Number)).OrderByDescending(K => K.SR_Number);
        //    }
        //    else
        //    {
        //        if (!String.IsNullOrEmpty(Search))
        //        {
        //            Key = Key.Where(K => K.SR_RejectionDate!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_MS_Name!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_NoOfItem!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_Qty!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_TotalAmount!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
        //             K.SR_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SR_Number);
        //        }
        //    }

        //    String PDFDownload = $@"<html>
        //                    <head></head>
        //                    <body>
        //                        <style>
        //                            html,body{{
        //                                font-family: ""Noto Sans"", sans-serif;
        //                                width: 100%;
        //                                margin: 0;
        //                                background-color: #fff;
        //                            }}

        //                            .table {{
        //                                border-collapse: collapse;
        //                                border: 1px SIlid #ccc;
        //                                font-size: 0.6rem;
        //                                font-family: ""Noto Sans"", sans-serif;
        //                            }}
        //                                .table th {{
        //                                    border: 1px SIlid #ccc;
        //                                    padding: 5px 8px;
        //                                    vertical-align: top;
        //                                background-color: #000;
        //                                    color:#fff;
        //                                }}

        //                                .table td {{
        //                                    border: 1px SIlid #ccc;
        //                                    padding: 5px 8px;
        //                                    vertical-align: top;
        //                                }}

        //                                .table th {{
        //                                    font-weight: bold;
        //                                }}

        //                                .table tr td {{
        //                                    height: 25px;
        //                                }}
        //                        </style>";

        //    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Buyer Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Export Order</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Amount</th><th style='width:70px'>Item Misc.Income Value</th><th style='width:70px'>Header Misc.Income Value</th></tr>";

        //    Decimal TotalQtySum = 0;
        //    Decimal TotalAmountSum = 0;
        //    Decimal TotalItemMiscIncomeSum = 0;
        //    Decimal TotalHeadMiscIncomeSum = 0;

        //    if (Key.ToList().Count > 0)
        //    {
        //        foreach (var Row in Key.ToList())
        //        {
        //            String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SR_TotalAmount));
        //            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SR_TotalItemIncome));
        //            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SR_TotalHeadIncome));
        //            PDFDownload += $@"<tr>
        //                            <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.SR_RejectionDate).ToString("dd-MMM-yyyy")}</td>
        //                            <td>{Row.SR_InvoiceNo}</td>
        //                            <td>{Row.SR_BUY_Name}</td>
        //                            <td style='width:30px;text-align:center'>{Row.SR_CUR_Name}</td>
        //                            <td style='width:30px;text-align:center'>{Row.SR_ExportOrder}</td>
        //                            <td>{Row.SR_MS_Name}</td>
        //                            <td style='width:30px;text-align:center'>{Row.SR_NoOfItem}</td>
        //                            <td style='width:30px;text-align:center'>{Row.SR_Qty}</td>
        //                            <td style='width:70px;text-align:right'>{Amount}</td>
        //                            <td style='width:70px;text-align:right'>{ItemExp}</td>
        //                            <td style='width:70px;text-align:right'>{HeadExp}</td>
        //                            </tr>";


        //            if (Decimal.TryParse(Row.SR_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
        //            {
        //                TotalQtySum += QtyValue;
        //            }
        //            if (Decimal.TryParse(Row.SR_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
        //            {
        //                TotalAmountSum += MaterialValue;
        //            }
        //            if (Decimal.TryParse(Row.SR_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
        //            {
        //                TotalItemMiscIncomeSum += ItemMiscValue;
        //            }
        //            if (Decimal.TryParse(Row.SR_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
        //            {
        //                TotalHeadMiscIncomeSum += HeadMiscValue;
        //            }
        //        }
        //    }


        //    PDFDownload += $@"<tr>
        //                            <td>Total</td>
        //                            <td></td>
        //                            <td></td>
        //                            <td></td>
        //                            <td></td>
        //                            <td></td>
        //                            <td></td>
        //                            <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
        //                            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalAmountSum))}</td>
        //                            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscIncomeSum))}</td>
        //                            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscIncomeSum))}</td>
        //                            </tr>";

        //    PDFDownload += $@"</table></body></html>";

        //    return PDFDownload;
        //}



        //Sale Order Create


        //Sale Invoice Detailed
        [Route("sale/transactions/sale-rejection-register-detailed")]
        public IActionResult SaleRejectionRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            SRR_List = SRDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<SaleRejectionRegister_DTO>.CreateAsync(SRR_List, DPageNumber ?? 1, DPageSize));
        }
        
        [Route("sale/transactions/sale-rejection-register-detailed")]
        [HttpPost]
        public IActionResult SaleRejectionRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? SR_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            if (Mode == "Ascii" || Mode == "Excel" || Mode == "PDF")
            {
                //List<Int64> AllowedPoNumbers = new List<Int64>();
                //if (DeleteNumber.Length > 0)
                //{
                //    try
                //    {
                //        AllowedPoNumbers = DeleteNumber
                //            .Select(s => s.Trim())
                //            .Where(s => !string.IsNullOrEmpty(s))
                //            .Select(Int64.Parse)
                //            .ToList();
                //    }
                //    catch
                //    {
                //    }
                //}
                //HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

                //SI_DTO.Id = 22;
                //SI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                //DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                //if (Mode == "Ascii")
                //{
                //    List<SaleInvoiceDetailAscii> SRR_List = P_DL.PODetailAscii(DS.Tables[0]);

                //    var Key = SRR_List;
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POH_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    Decimal TotalQtySum = 0;
                //    Decimal TotalMaterialValueSum = 0;
                //    Decimal TotalItemMiscExpenseSum = 0;
                //    Decimal TotalHeadMiscExpenseSum = 0;

                //    var HeaderRow = typeof(SaleInvoiceDetailAscii)
                //            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                //            .Where(prop => prop.Name != nameof(SaleInvoiceDetailAscii.POH_Number))
                //            .Select(prop =>
                //                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                //             )
                //            .ToList();

                //    var AsciiData = new StringBuilder();
                //    AsciiData.AppendLine(string.Join("\t", HeaderRow));


                //    PropertyInfo[] PropertiesToInclude = typeof(SaleInvoiceDetailAscii)
                //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                //        .Where(prop => prop.Name != nameof(SaleInvoiceDetailAscii.POH_Number))
                //        .ToArray();

                //    foreach (var item in Key)
                //    {
                //        var rowValues = PropertiesToInclude
                //            .Select(prop => prop.GetValue(item)?.ToString() ?? "")
                //            .ToList();

                //        var escapedRowValues = rowValues
                //            .Select(val => val.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))
                //            .ToList();

                //        AsciiData.AppendLine(string.Join("\t", escapedRowValues));

                //        if (Decimal.TryParse(item.POI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //        {
                //            TotalQtySum += QtyValue;
                //        }
                //        if (Decimal.TryParse(item.POI_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //        {
                //            TotalMaterialValueSum += MaterialValue;
                //        }
                //        if (Decimal.TryParse(item.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //        {
                //            TotalItemMiscExpenseSum += ItemMiscValue;
                //        }
                //        if (Decimal.TryParse(item.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //        {
                //            TotalHeadMiscExpenseSum += HeadMiscValue;
                //        }
                //    }

                //    List<String> FooterCells = new List<String>();
                //    Boolean TotalLabelAdded = false;
                //    foreach (var prop in PropertiesToInclude)
                //    {
                //        string FooterCellValue = "";

                //        if (!TotalLabelAdded && prop.Name != nameof(SaleInvoiceDetailAscii.POI_Qty) &&
                //                                 prop.Name != nameof(SaleInvoiceDetailAscii.POI_Amount) &&
                //                                 prop.Name != nameof(SaleInvoiceDetailAscii.ItemMiscExpense) &&
                //                                 prop.Name != nameof(SaleInvoiceDetailAscii.HeadMiscExpense))
                //        {
                //            if (FooterCells.Count == 0)
                //            {
                //                FooterCellValue = "Total:";
                //                TotalLabelAdded = true;
                //            }
                //        }

                //        switch (prop.Name)
                //        {
                //            case nameof(SaleInvoiceDetailAscii.POI_Qty):
                //                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceDetailAscii.POI_Amount):
                //                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceDetailAscii.ItemMiscExpense):
                //                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceDetailAscii.HeadMiscExpense):
                //                FooterCellValue = TotalHeadMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //        }

                //        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                //    }

                //    AsciiData.AppendLine(string.Join("\t", FooterCells));

                //    String FileName = "PO-download";
                //    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                //    var contentType = "text/plain";
                //    var fileDownloadName = $"{FileName}.txt";
                //    return File(fileBytes, contentType, fileDownloadName);

                //}
                //else if (Mode == "Excel")
                //{
                //    SRR_List = P_DL.PODetailList(DS.Tables[0]);

                //    var Key = SRR_List.ToList();
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POI_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    String FileName = "PO-download";
                //    using (var wb = new XLWorkbook())
                //    {
                //        var ws = wb.Worksheets.Add(PODetailsDownload(Key.ToList()), "Sheet1");

                //        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //        ws.Style.Font.Bold = true;
                //        ws.Columns().AdjustToContents();

                //        using (var stream = new MemoryStream())
                //        {
                //            wb.SaveAs(stream);

                //            var content = stream.ToArray();
                //            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //            var fileDownloadName = $"{FileName}.xlsx";

                //            return File(content, contentType, fileDownloadName);
                //        }
                //    }
                //}
                //else if (Mode == "PDF")
                //{
                //    SRR_List = P_DL.PODetailList(DS.Tables[0]);

                //    var Key = SRR_List.ToList();
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POI_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    String PDFDownload = $@"<html>
                //    <head></head>
                //    <body>
                //        <style>
                //            html,body{{
                //                font-family: ""Noto Sans"", sans-serif;
                //                width: 100%;
                //                margin: 0;
                //                background-color: #fff;
                //            }}

                //            .table {{
                //                border-collapse: collapse;
                //                border: 1px SIlid #ccc;
                //                font-size: 0.5rem;
                //                font-family: ""Noto Sans"", sans-serif;
                //            }}
                //                .table th {{
                //                    border: 1px SIlid #ccc;
                //                    padding: 5px 8px;
                //                    vertical-align: top;
                //                background-color: #000;
                //                    color:#fff;
                //                }}

                //                .table td {{
                //                    border: 1px SIlid #ccc;
                //                    padding: 5px 8px;
                //                    vertical-align: top;
                //                }}

                //                .table th {{
                //                    font-weight: bold;
                //                }}

                //                .table tr td {{
                //                    height: 25px;
                //                }}
                //        </style>";

                //    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Buyer Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Import Order</th><th>Material Segregation</th><th>Item Group</th><th>Item Code</th><th>Item Description</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:30px;text-align:center'>Unit Price</th><th style='width:70px'>Amount</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:70px'>Delivery Date</th></tr>";

                //    Decimal TotalQtySum = 0;
                //    Decimal TotalMaterialValueSum = 0;
                //    Decimal TotalItemMiscExpenseSum = 0;
                //    Decimal TotalHeadMiscExpenseSum = 0;

                //    if (Key.ToList().Count > 0)
                //    {
                //        foreach (var Row in Key.ToList())
                //        {
                //            String Import = "";
                //            if (Row.POH_ImportOrder == 1)
                //            {
                //                Import = "Yes";
                //            }
                //            else
                //            {
                //                Import = "No";
                //            }

                //            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.POI_Amount));
                //            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscExpense));
                //            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscExpense));
                //            PDFDownload += $@"<tr>
                //            <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.POH_Date).ToString("dd-MMM-yyyy")}</td>
                //            <td>{Row.POH_OrderNo}</td>
                //            <td>{Row.POH_Buyer_Number}</td>
                //            <td style='width:30px;text-align:center'>{Row.POH_Currency_Number}</td>
                //            <td style='width:30px;text-align:center'>{Import}</td>
                //            <td>{Row.POH_MS_Number}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_ITM_Group}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_ITM_Code}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_ITM_Description}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_UoM}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_Qty}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_UnitPrice}</td>
                //            <td style='width:70px;text-align:right'>{Matrial}</td>
                //            <td style='width:70px;text-align:right'>{ItemExp}</td>
                //            <td style='width:70px;text-align:right'>{HeadExp}</td>
                //            <td style='width:70px;text-align:right'>{Row.POI_DeliveryDate}</td>
                //            </tr>";


                //            if (Decimal.TryParse(Row.POI_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //            {
                //                TotalQtySum += QtyValue;
                //            }
                //            if (Decimal.TryParse(Row.POI_Amount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //            {
                //                TotalMaterialValueSum += MaterialValue;
                //            }
                //            if (Decimal.TryParse(Row.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //            {
                //                TotalItemMiscExpenseSum += ItemMiscValue;
                //            }
                //            if (Decimal.TryParse(Row.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //            {
                //                TotalHeadMiscExpenseSum += HeadMiscValue;
                //            }
                //        }
                //    }

                //    PDFDownload += $@"<tr>
                //            <td>Total</td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                //            <td></td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                //            <td></td>
                //            </tr>";


                //    PDFDownload += $@"</table></body></html>";

                //    HtmlToPdf converter = new HtmlToPdf();
                //    converter.Options.PdfPageSize = PdfPageSize.A4;
                //    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                //    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                //    converter.Options.MarginLeft = 10;
                //    converter.Options.MarginRight = 10;
                //    converter.Options.MarginTop = 10;
                //    converter.Options.MarginBottom = 10;

                //    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                //    MemoryStream memoryStream = new MemoryStream();
                //    doc.Save(memoryStream);
                //    doc.Close();

                //    return File(memoryStream.ToArray(), "application/pdf", "Print_Download.pdf");
                //}
            }
            else if (Mode == "View")
            {
                SR_DTO.SR_Number = Convert.ToInt32(SR_No);
                SR_DTO.SR_Id = 2;
                SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
                DS = SR_DAO.SaleRejectionDB(SR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PreviewSaleRejection", new { SR_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }
            else if (Mode == "Edit")
            {
                SR_DTO.SR_Number = Convert.ToInt32(SR_No);
                SR_DTO.SR_Id = 2;
                SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
                DS = SR_DAO.SaleRejectionDB(SR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (DS.Tables[0].Rows[0]["SRH_Mode"].ToString() == "1")
                    {
                        return RedirectToAction("EditSaleRejection", new { SR_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["SRH_Mode"].ToString() == "2")
                    {
                        return RedirectToAction("SIToEditSaleRejection", new { SR_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["SRH_Mode"].ToString() == "3")
                    {
                        return RedirectToAction("SIItemToEditSaleRejection", new { SR_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                }
            }

            SRR_List = SRDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<SaleRejectionRegister_DTO>.CreateAsync(SRR_List, DPageNumber ?? 1, DPageSize));
        }
        
        List<SaleRejectionRegister_DTO> SRDetailedGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            SR_DTO.SR_Id = 5;
            SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            SRR_List = S_DL.SRDetailedList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
            }
            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }
            if (PageFilter?.ToLower() == "PageFilter".ToLower())
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSIrt"] = SortOrder;
            ViewData["KeySIrt"] = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["CurrentFilter"] = Search;

            var Key = SRR_List.OrderByDescending(Cs => Cs.SR_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.SR_RejectionDate!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_RejectionNo!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_UoM_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SR_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SR_TotalAmount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SR_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SR_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SR_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.SR_RejectionDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.SR_RejectionDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.SR_Number);
                    break;
            }

            if (PSize != 0)
            {
                DPageSize = PSize;
            }
            Int32 Record = Key.ToList().Count;
            if (PageNumber > 1)
            {
                Int32 RecordPage = (Convert.ToInt32(PageNumber) - 1) * DPageSize;

                if (Record > RecordPage)
                {
                    if (Convert.ToInt32(PageNumber) == 0)
                    {
                        DPageNumber = 1;
                    }
                    else
                    {
                        DPageNumber = Convert.ToInt32(PageNumber);
                    }
                }
                else
                {
                    DPageNumber = Convert.ToInt32(PageNumber) - 1;
                }
            }
            else
            {
                if (Convert.ToInt32(PageNumber) == 0)
                {
                    DPageNumber = 1;
                }
                else
                {
                    DPageNumber = Convert.ToInt32(PageNumber);
                }
            }

            Double Pages = Convert.ToDouble(Record) / Convert.ToDouble(DPageSize);
            Int32 PageCounts = Convert.ToInt32(Math.Ceiling(Pages));

            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.SR_Qty));
            ViewBag.SumOfMaterialValue = Key.Sum(item => Convert.ToDouble(item.SR_MaterialValue));
            ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.SR_TotalItemIncome));
            ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.SR_TotalHeadIncome));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.SR_TotalAmount));
            ViewBag.SumOfItemGST = Key.Sum(item => Convert.ToDouble(item.SR_ItemGST_Amount));
            ViewBag.SumOfHeadGST = Key.Sum(item => Convert.ToDouble(item.SR_HeadGST_Amount));
            ViewBag.SumOfRejectionAmount = Key.Sum(item => Convert.ToDouble(item.SR_RejectionAmount));
            ViewBag.SumOfItemWHT = Key.Sum(item => Convert.ToDouble(item.SR_ItemWHT_Amount));
            ViewBag.SumOfHeadWHT = Key.Sum(item => Convert.ToDouble(item.SR_HeadWHT_Amount));
            ViewBag.SumOfRoundOff = Key.Sum(item => Convert.ToDouble(item.SR_RoundOff));
            ViewBag.SumOfReceivable = Key.Sum(item => Convert.ToDouble(item.SR_BuyerReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }




        [Route("sale/transactions/sale-rejection/{SR_No}/view")]
        public IActionResult PreviewSaleRejection(String? SR_No)
        {
            String Active = GetSIPreview(SR_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleRejectionRegisterSummary");
            }
            ViewBag.SR_No = SR_No;

            SaleGetData();
            GetSREditData(SR_No);
            return View(SRH_DTO);
        }
        String GetSIPreview(String SR_No)
        {
            SR_DTO.SR_Number = Convert.ToInt64(SR_No);
            SR_DTO.SR_Id = 71;
            SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                ViewBag.HeadPreview = S_DL.SRHeadEditList(DS.Tables[0]).FirstOrDefault();
                ViewBag.ItemPreview = S_DL.SRItemEditList(DS.Tables[1]);
                ViewBag.BuyerPreview = S_DL.BuyerList(DS.Tables[2]).FirstOrDefault();
                return "1";
            }
            else
            {
                return "0";
            }
        }





        [Route("sale/transactions/sale-rejection/create")]
        public IActionResult CreateSaleRejection()
        {
            //SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();
            //if (TempData["SH_DTO_Json"] is string SHto)
            //{
            //    SH_DTO = System.Text.Json.JsonSerializer.Deserialize<SaleInvoiceHead_DTO>(SHto);
            //}
            SRH_DTO.SRH_RejectionDate = DateTime.Now.ToString("dd-MMM-yy");
            SRH_DTO.SRH_RejectionNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            SaleGetData();
            return View(SRH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-rejection/create")]
        public IActionResult CreateSaleRejection(SaleRejectionHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SaleRejectionHead_DTO S_Head_DTO = new SaleRejectionHead_DTO();

            List<SaleRejectionItem_DTO>? ITM_DTO = new List<SaleRejectionItem_DTO>();
            List<SaleRejectionIncome_DTO>? Income_DTO = new List<SaleRejectionIncome_DTO>();
            List<SaleRejectionIIncome_DTO>? ItemIncome_DTO = new List<SaleRejectionIIncome_DTO>();
            List<SaleRejectionBatch_DTO>? ItemBatch_DTO = new List<SaleRejectionBatch_DTO>();
            List<SaleRejectionAddress_DTO>? BuyerAddress_DTO = new List<SaleRejectionAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.RejectionItem != null)
                ITM_DTO = S_DTO.RejectionItem!.Where(K => K.SRI_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SRH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SRI_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemBatch != null)
                ItemBatch_DTO = S_DTO.ItemBatch!.Where(K => K.SRI_BCH_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SRH_ADD_IsDeleted == 0).ToList();

            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SRI_MS_Number) != Convert.ToInt64(S_DTO.SRH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SRI_Qty) == 0 || Convert.ToDouble(x.SRI_UnitPrice) == 0 || Convert.ToDouble(x.SRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SRH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SRH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.RejectionItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.ItemBatch = ItemBatch_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (DirectBatchValidation(ITM_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String SIHOrderNoOld = S_DTO.SRH_RejectionNo;
                                    String SIHOrderNoNew = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd")));

                                    SR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd"));
                                    SR_DTO.SR_RejectionNo = SIHOrderNoNew;
                                    SR_DTO.SR_BUY_Number = Convert.ToInt64(S_DTO.SRH_BUY_Number);
                                    SR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 1 : 0);
                                    SR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SR_DTO.SR_MS_Number = Convert.ToInt64(S_DTO.SRH_MS_Number);
                                    SR_DTO.SR_TCT_Number = Convert.ToInt64(S_DTO.SRH_TCT_Number);
                                    SR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SR_DTO.SR_WHT_Number = Convert.ToInt64(S_DTO.SRH_WHT_Number);
                                    SR_DTO.SR_ExchangeRate = Convert.ToDouble(S_DTO.SRH_ExchangeRate) == 0 ? "1" : S_DTO.SRH_ExchangeRate;
                                    SR_DTO.SR_MaterialCost = Convert.ToDouble(S_DTO.SRH_MaterialCost);
                                    SR_DTO.SR_ItemMiscIncome = Convert.ToDouble(S_DTO.SRH_ItemMiscIncome);
                                    SR_DTO.SR_HeaderMiscIncome = Convert.ToDouble(S_DTO.SRH_HeaderMiscIncome);
                                    SR_DTO.SR_GST_Amount = Convert.ToDouble(S_DTO.SRH_GST_Amount);
                                    SR_DTO.SR_RejectionAmount = Convert.ToDouble(S_DTO.SRH_RejectionAmount);
                                    SR_DTO.SR_WHT_Amount = Convert.ToDouble(S_DTO.SRH_WHT_Amount);
                                    SR_DTO.SR_RoundOff = Convert.ToDouble(S_DTO.SRH_RoundOff);
                                    SR_DTO.SR_BuyerReceivable = Convert.ToDouble(S_DTO.SRH_BuyerReceivable);
                                    SR_DTO.SR_Id = 31;
                                    DS = SR_DAO.SaleRejectionDB(SR_DTO);

                                    OnSaleNumberGen(Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        SR_DTO.SR_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in ITM_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            SR_DTO.SR_ITM_Number = Convert.ToInt64(Item.SRI_ITM_Number);
                                            SR_DTO.SR_WH_Number = Convert.ToInt64(Item.SRI_WH_Number);
                                            SR_DTO.SR_UoM_Number = Convert.ToInt64(Item.SRI_UoM_Number);
                                            SR_DTO.SR_Qty = Convert.ToDouble(Item.SRI_Qty);
                                            SR_DTO.SR_UnitPrice = Convert.ToDouble(Item.SRI_UnitPrice);
                                            SR_DTO.SR_Amount = Convert.ToDouble(Item.SRI_Amount);
                                            SR_DTO.SR_IncomeValue = Convert.ToDouble(Item.SRI_IncomeValue);
                                            SR_DTO.SR_HSN_Number = Convert.ToInt64(Item.SRI_HSN_Number);
                                            SR_DTO.SR_GST_Amount = Convert.ToDouble(Item.SRI_GST_Amount);
                                            SR_DTO.SR_WHT_Percent = Convert.ToDouble(Item.SRI_WHT_Percent);
                                            SR_DTO.SR_WHT_Amount = Convert.ToDouble(Item.SRI_WHT_Amount);
                                            SR_DTO.SR_Id = 32;
                                            D = SR_DAO.SaleRejectionDB(SR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (Convert.ToInt64(x.SRI_INC_ITM_Number) == Item.SRI_ITM_Number) && (x.SRI_INC_ITM_Index == Item.SRI_Index));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SR_DTO.SR_ITM_Number = Convert.ToInt64(ItemInc.SRI_INC_ITM_Number);
                                                SR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                SR_DTO.SR_Id = 34;
                                                SR_DAO.SaleRejectionDB(SR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == Item.SRI_ITM_Number) && (x.SRI_BCH_ITM_Index == Item.SRI_Index));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                SR_DTO.SR_Id = 35;
                                                SR_DAO.SaleRejectionDB(SR_DTO);
                                            }
                                        }
                                        foreach (var Income in Income_DTO)
                                        {
                                            SR_DTO.SR_INC_MIC_Number = Convert.ToInt64(Income.SRH_INC_MIC_Number);
                                            SR_DTO.SR_INC_Remarks = Income.SRH_INC_Remarks;
                                            SR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(Income.SRH_INC_OCRN_Number);
                                            SR_DTO.SR_INC_CM_Number = Convert.ToInt64(Income.SRH_INC_CM_Number);
                                            SR_DTO.SR_INC_IncomeBase = Convert.ToDouble(Income.SRH_INC_IncomeBase);
                                            SR_DTO.SR_INC_IncomeValue = Convert.ToDouble(Income.SRH_INC_IncomeValue);
                                            SR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(Income.SRH_INC_ALCT_Number);
                                            SR_DTO.SR_INC_LA_Number = Convert.ToInt64(Income.SRH_INC_LA_Number);
                                            SR_DTO.SR_INC_CalculateGST = Convert.ToInt64(Income.SRH_INC_CalculateGST);
                                            SR_DTO.SR_INC_GST_Amount = Convert.ToDouble(Income.SRH_INC_GST_Amount);
                                            SR_DTO.SR_INC_SAC_Number = Convert.ToInt64(Income.SRH_INC_SAC_Number);
                                            SR_DTO.SR_INC_WHT_Percent = Convert.ToDouble(Income.SRH_INC_WHT_Percent);
                                            SR_DTO.SR_INC_WHT_Amount = Convert.ToDouble(Income.SRH_INC_WHT_Amount);
                                            SR_DTO.SR_Id = 33;
                                            SR_DAO.SaleRejectionDB(SR_DTO);
                                        }

                                        foreach (var BuyerAddress in BuyerAddress_DTO)
                                        {
                                            SR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_ADTP_Number);
                                            SR_DTO.SR_ADD_AddressID = Convert.ToString(BuyerAddress.SRH_ADD_AddressID);
                                            SR_DTO.SR_ADD_Address = Convert.ToString(BuyerAddress.SRH_ADD_Address);
                                            SR_DTO.SR_ADD_City = Convert.ToString(BuyerAddress.SRH_ADD_City);
                                            SR_DTO.SR_ADD_State = Convert.ToString(BuyerAddress.SRH_ADD_State);
                                            SR_DTO.SR_ADD_Country = Convert.ToString(BuyerAddress.SRH_ADD_Country);
                                            SR_DTO.SR_ADD_Pin = Convert.ToString(BuyerAddress.SRH_ADD_Pin);
                                            SR_DTO.SR_ADD_GSTIN = Convert.ToString(BuyerAddress.SRH_ADD_GSTIN);
                                            SR_DTO.SR_Id = 36;
                                            SR_DAO.SaleRejectionDB(SR_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    if (SIHOrderNoOld != SIHOrderNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage = "Sale Rejection number " + SIHOrderNoOld + " used by another user. Next number will be allotted to you.";
                                    }
                                    return RedirectToAction("CreateSaleRejection");
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }

                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;
                    }
                }
            }
            else if (Mode == "Direct")
            {
                SaleRejectionHead_DTO SH_DTO = new SaleRejectionHead_DTO();

                SH_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SH_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SH_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SH_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SH_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SH_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SH_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SH_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SH_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SH_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SH_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SH_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SH_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleRejection");
            }
            else if (Mode == "SITOITEM")
            {
                SIToSaleRejectionHead_DTO SHSI_DTO = new SIToSaleRejectionHead_DTO();

                SHSI_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SHSI_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SHSI_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SHSI_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SHSI_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SHSI_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SHSI_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SHSI_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SHSI_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SHSI_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SHSI_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SHSI_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SHSI_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SHSI_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSI_DTO);

                return RedirectToAction("SIToCreateSaleRejection");
            }
            else if (Mode == "SIITEMTO")
            {
                SIItemToSaleRejectionHead_DTO SHSI_DTO = new SIItemToSaleRejectionHead_DTO();

                SHSI_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SHSI_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SHSI_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SHSI_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SHSI_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SHSI_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SHSI_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SHSI_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SHSI_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SHSI_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SHSI_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SHSI_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SHSI_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SHSI_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSI_DTO);

                return RedirectToAction("SIItemToCreateSaleRejection");
            }
            SaleGetData();
            return View(Original_DTO);
        }
        Boolean DirectBatchValidation(List<SaleRejectionItem_DTO> Item_DTO, List<SaleRejectionBatch_DTO>? ItemBatch_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                //Double BatchAmount = 0;

                Int64 SRINumber = Convert.ToInt64(Item.SRI_Number);
                Int64 SRIIndex = Convert.ToInt64(Item.SRI_Index);
                Int64 SRIItem = Convert.ToInt64(Item.SRI_ITM_Number);
                Double SRIQty = Math.Abs(Convert.ToDouble(Item.SRI_Qty));
                Double SRIAmount = Convert.ToDouble(Item.SRI_Amount);

                if (SRINumber != 0)
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_SRI_Number == SRINumber));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                    }
                }
                else
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == SRIItem) && (x.SRI_BCH_ITM_Index == SRIIndex));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                    }
                }


                if (BatchQty == SRIQty) { }
                else
                {
                    Message += Item.SRI_ITM_Code + " Batch Qty  Mismatched <br/>";
                    Result = false;
                }

                if (Message != "")
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Message;
                }
            }
            return Result;
        }


        [Route("sale/transactions/sale-rejection/item")]
        public IActionResult SaleItem(String? ItemCode, String MS)
        {
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_ITM_Code = Convert.ToString(ItemCode).Trim();
            SR_DTO.SR_MS_Number = Convert.ToInt64(MS.Trim());
            SR_DTO.SR_Id = 13;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            var Item = S_DL.ItemList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("sale/transactions/sale-rejection/uom")]
        public String SaleUoM(String? UoM)
        {
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_UoM_Number = Convert.ToInt64(UoM);
            SR_DTO.SR_Id = 15;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("sale/transactions/sale-rejection/income/des")]
        public IActionResult SaleExpensiveDes(String? Title)
        {
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_INC_MIC_Number = Convert.ToInt64(Title);
            SR_DTO.SR_Id = 14;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            var Expensive = S_DL.IncomeList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("sale/transactions/sale-rejection/buyer")]
        public IActionResult SaleBuyer(String? Buyer, String? Export, String? SRHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            if (Export == null)
            {
                Export = "";
            }
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(Export) == true ? 2 : 1);
            SR_DTO.SR_ITM_Code = Convert.ToString(Buyer).Trim();
            SR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SR_DTO.SR_Id = 16;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            var Ven = S_DL.BuyerList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("sale/transactions/sale-rejection/gst")]
        public String SaleInvoiceGst(String? Cluster, String? SRHDate, String? HSN, String? BaseAmount)
        {
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SR_DTO.SR_HSN_Number = Convert.ToInt64(HSN);
            SR_DTO.SR_Id = 17;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("sale/transactions/sale-rejection/gst/view")]
        public IActionResult SaleInvoiceGstView(String? Cluster, String? SRHDate, String? HSN, String? BaseAmount)
        {
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SR_DTO.SR_HSN_Number = Convert.ToInt64(HSN);
            SR_DTO.SR_Id = 20;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleInvoiceGst> PurGST = new List<SaleInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new SaleInvoiceGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("sale/transactions/sale-rejection/income/gst")]
        public String SaleInvoiceHeaderGst(String? Cluster, String? SRHDate, String? SAC, String? BaseAmount)
        {
            SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SR_DTO.SR_INC_SAC_Number = Convert.ToInt64(SAC);
            SR_DTO.SR_Id = 21;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("sale/transactions/sale-rejection/income/gst/view")]
        public IActionResult SaleInvoiceGstHeaderView(String? Cluster, String? SRHDate, String? SAC, String? BaseAmount)
        {
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SR_DTO.SR_INC_SAC_Number = Convert.ToInt64(SAC);
            SR_DTO.SR_Id = 22;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleInvoiceGst> PurGST = new List<SaleInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new SaleInvoiceGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("sale/transactions/sale-rejection/wht")]
        public IActionResult SaleInvoiceWHT(String? Buyer, String? WHTNumber, String? SRHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Buyer == null)
            {
                Buyer = "0";
            }
            if (SRHDate == null)
            {
                SRHDate = "0";
            }
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SR_DTO.SR_WHT_Number = Convert.ToInt64(WHTNumber);
            SR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SR_DTO.SR_Id = 18;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            var WHT = S_DL.SaleInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("sale/transactions/sale-rejection/cluster")]
        public IActionResult SaleInvoiceCluster(String? Buyer, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SR_DTO.SR_Search = Cluster;
            SR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SR_DTO.SR_Id = 19;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            var InvCluster = S_DL.SaleCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("sale/transactions/sale-rejection/buyer/address")]
        public IActionResult SaleBuyerAddressID(String? Buyer, String ADTPNumber)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SR_DTO.SR_Id = 23;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            SaleRejectionAddress SRA = new SaleRejectionAddress();
            SRA.BuyerAddressId = S_DL.BuyerAddressID(DS.Tables[0]);
            SRA.BuyerAddress = S_DL.BuyerAddress(DS.Tables[1]).FirstOrDefault();
            return Json(SRA);
        }

        [Route("sale/transactions/sale-rejection/buyer/address/addressid")]
        public IActionResult SaleBuyerAddress(String? Buyer, String ADTPNumber, String AddressID)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SR_DTO.SR_ADD_AddressID = Convert.ToString(AddressID);
            SR_DTO.SR_Id = 24;
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            var Address = S_DL.BuyerAddress(DS.Tables[0]).FirstOrDefault();
            return Json(Address);
        }




        void SaleGetData()
        {
            SR_DTO.SR_RejectionDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            SR_DTO.SR_Id = 12;
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            DS = SR_DAO.SaleRejectionDB(SR_DTO);

            ViewBag.IncomeCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.Currency = Help.GetCat(DS.Tables[4]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[5]);
            ViewBag.UoM = Help.GetCat(DS.Tables[6]);
            ViewBag.LedgerAccount = Help.GetCat(DS.Tables[7]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[8]);
            ViewBag.HSN = Help.GetCat(DS.Tables[9]);
            ViewBag.WHTax = Help.GetCat(DS.Tables[10]);
            ViewBag.IsCalculate = Help.GetCat(DS.Tables[11]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[12]);
        }

        void OnSaleNumberGen(Int32 SRDate)
        {
            DataSet DS1 = new DataSet();
            SRN_DTO.SRN_Date = SRDate.ToString();
            SRN_DTO.SRN_Id = 101;
            DS1 = SRN_DAO.SRNumberDB(SRN_DTO);
            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["SRN_Method"].ToString());
                DateTime SRN_Date = Convert.ToDateTime(DateTime.ParseExact(SRDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["SR_StartingNumber"].ToString());

                        SRN_DTO.SRN_Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["SR_Number"].ToString());
                        SRN_DTO.SRN_StartingNumber = Convert.ToString(Convert.ToInt32(Number + 1));
                        SRN_DTO.SRN_Id = 103;
                        SRN_DAO.SRNumberDB(SRN_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;
                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(DS1.Tables[2].Rows[0]["SRR_Date"].ToString());
                            Start = Convert.ToInt32(DS1.Tables[2].Rows[0]["SRR_StartingNumber"].ToString());
                            Frequency = Convert.ToInt32(DS1.Tables[2].Rows[0]["SRR_Frequency"].ToString());
                        }

                        if (Frequency == 4)
                        {
                            if (Date.Month == SRN_Date.Month)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(SRN_Date.Year, SRN_Date.Month, 1);
                                EndDate = new DateTime(SRN_Date.Year, SRN_Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                        }
                        else if (Frequency == 5)
                        {
                            if (Date.Month == SRN_Date.Month && Date.Year == SRN_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else if (Date.Month != SRN_Date.Month && Date.Year == SRN_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(SRN_Date.Year, Date.Month, 1);
                                EndDate = new DateTime(SRN_Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                        }

                        SRN_DTO.SRN_Number = Convert.ToInt32(DS1.Tables[2].Rows[0]["SRR_Number"].ToString());
                        SRN_DTO.SRN_StartingNumber = Convert.ToString(Start);
                        SRN_DTO.SRN_Date = Convert.ToString(StartDate.ToString("yyyyMMdd"));
                        SRN_DTO.SRN_Method = Convert.ToString(EndDate.ToString("yyyyMMdd"));
                        SRN_DTO.SRN_Id = 102;
                        SRN_DAO.SRNumberDB(SRN_DTO);
                    }
                }
            }
        }


        [Route("sale/transactions/sale-rejection/numbering")]
        public String OnSaleNumber(Int32 SRDate)
        {
            SR_DTO.SR_RejectionDate = Convert.ToInt32(SRDate);
            SR_DTO.SR_Id = 11;
            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            DS = SR_DAO.SaleRejectionDB(SR_DTO);

            if (DS.Tables[0].Rows.Count > 0 && DS.Tables[1].Rows.Count > 0)
            {
                Int32 Number = 0;
                Int32 Order = 0;
                String Prefix = "";
                String Surfix = "";
                String Prefil = "";
                String OrderNum = "";

                if (DS.Tables[0].Rows.Count > 0)
                {
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["SRN_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["SRP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["SRS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["SR_StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["SRR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["SRR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["SRR_Frequency"].ToString());

                            Number = OrNum + 1;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                        }
                    }
                    else
                    {
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["SRR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["SRR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["SRR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["SRR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["SRR_Frequency"].ToString());

                            Number = RNumber;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                        }
                    }
                    OrderNum = Prefix + "" + Number.ToString(Prefil) + "" + Surfix;

                    return OrderNum;
                }
            }
            else
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = "SR number Not asSRgned for Given date";
            }
            return "";
        }










        //Sale Edit
        [Route("sale/transactions/sale-rejection/{SR_No}/edit")]
        public IActionResult EditSaleRejection(String? SR_No)
        {
            SaleGetData();
            String Active = GetSREditData(SR_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleRejectionRegisterSummary");
            }
            ViewBag.SR_No = SR_No;
            return View(SRH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-rejection/{SR_No}/edit")]
        public IActionResult EditSaleRejection(SaleRejectionHead_DTO S_DTO, String? Mode, String? SR_No)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SaleRejectionHead_DTO S_Head_DTO = new SaleRejectionHead_DTO();

            List<SaleRejectionItem_DTO>? ITM_DTO = new List<SaleRejectionItem_DTO>();
            List<SaleRejectionIncome_DTO>? Income_DTO = new List<SaleRejectionIncome_DTO>();
            List<SaleRejectionIIncome_DTO>? ItemIncome_DTO = new List<SaleRejectionIIncome_DTO>();
            List<SaleRejectionBatch_DTO>? ItemBatch_DTO = new List<SaleRejectionBatch_DTO>();
            List<SaleRejectionAddress_DTO>? BuyerAddress_DTO = new List<SaleRejectionAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.RejectionItem != null)
                ITM_DTO = S_DTO.RejectionItem!.Where(K => K.SRI_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SRH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SRI_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemBatch != null)
                ItemBatch_DTO = S_DTO.ItemBatch!.Where(K => K.SRI_BCH_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SRH_ADD_IsDeleted == 0).ToList();

            SR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SRI_MS_Number) != Convert.ToInt64(S_DTO.SRH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SRI_Qty) == 0 || Convert.ToDouble(x.SRI_UnitPrice) == 0 || Convert.ToDouble(x.SRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SRH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SRH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.RejectionItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.ItemBatch = ItemBatch_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (DirectBatchValidation(ITM_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    SR_DTO.SR_Number = Convert.ToInt64(S_DTO.SRH_Number);
                                    SR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd"));
                                    SR_DTO.SR_RejectionNo = S_DTO.SRH_RejectionNo;
                                    SR_DTO.SR_BUY_Number = Convert.ToInt64(S_DTO.SRH_BUY_Number);
                                    SR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 1 : 0);
                                    SR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SR_DTO.SR_MS_Number = Convert.ToInt64(S_DTO.SRH_MS_Number);
                                    SR_DTO.SR_TCT_Number = Convert.ToInt64(S_DTO.SRH_TCT_Number);
                                    SR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SR_DTO.SR_WHT_Number = Convert.ToInt64(S_DTO.SRH_WHT_Number);
                                    SR_DTO.SR_ExchangeRate = Convert.ToDouble(S_DTO.SRH_ExchangeRate) == 0 ? "1" : S_DTO.SRH_ExchangeRate;
                                    SR_DTO.SR_MaterialCost = Convert.ToDouble(S_DTO.SRH_MaterialCost);
                                    SR_DTO.SR_ItemMiscIncome = Convert.ToDouble(S_DTO.SRH_ItemMiscIncome);
                                    SR_DTO.SR_HeaderMiscIncome = Convert.ToDouble(S_DTO.SRH_HeaderMiscIncome);
                                    SR_DTO.SR_GST_Amount = Convert.ToDouble(S_DTO.SRH_GST_Amount);
                                    SR_DTO.SR_RejectionAmount = Convert.ToDouble(S_DTO.SRH_RejectionAmount);
                                    SR_DTO.SR_WHT_Amount = Convert.ToDouble(S_DTO.SRH_WHT_Amount);
                                    SR_DTO.SR_RoundOff = Convert.ToDouble(S_DTO.SRH_RoundOff);
                                    SR_DTO.SR_BuyerReceivable = Convert.ToDouble(S_DTO.SRH_BuyerReceivable);
                                    SR_DTO.SR_Id = 121;
                                    DS = SR_DAO.SaleRejectionDB(SR_DTO);

                                    String ItemDTO = string.Join(", ", ITM_DTO.Where(x => Convert.ToInt64(x.SRI_Number) != 0).Select(x => x.SRI_Number));
                                    String ItemIncomeDTO = string.Join(", ", ItemIncome_DTO.Where(x => Convert.ToInt64(x.SRI_INC_Number) != 0).Select(x => x.SRI_INC_Number));
                                    String IncomeDTO = string.Join(", ", Income_DTO.Where(x => Convert.ToInt64(x.SRH_INC_Number) != 0).Select(x => x.SRH_INC_Number));
                                    String BatchDTO = string.Join(", ", ItemBatch_DTO.Where(x => Convert.ToInt64(x.SRI_BCH_Number) != 0).Select(x => x.SRI_BCH_Number));
                                    String AddressDTO = string.Join(", ", BuyerAddress_DTO.Where(x => Convert.ToInt64(x.SRH_ADD_Number) != 0).Select(x => x.SRH_ADD_Number));

                                    SR_DTO.SR_DeleteNumbers = Convert.ToString(ItemDTO);
                                    SR_DTO.SR_Id = 101;
                                    SR_DAO.SaleRejectionDB(SR_DTO);

                                    SR_DTO.SR_DeleteNumbers = Convert.ToString(IncomeDTO);
                                    SR_DTO.SR_Id = 102;
                                    SR_DAO.SaleRejectionDB(SR_DTO);

                                    SR_DTO.SR_DeleteNumbers = Convert.ToString(ItemIncomeDTO);
                                    SR_DTO.SR_Id = 103;
                                    SR_DAO.SaleRejectionDB(SR_DTO);

                                    SR_DTO.SR_DeleteNumbers = Convert.ToString(BatchDTO);
                                    SR_DTO.SR_Id = 104;
                                    SR_DAO.SaleRejectionDB(SR_DTO);

                                    SR_DTO.SR_DeleteNumbers = Convert.ToString(AddressDTO);
                                    SR_DTO.SR_Id = 105;
                                    SR_DAO.SaleRejectionDB(SR_DTO);

                                    foreach (var Item in ITM_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        SR_DTO.SR_ITM_Number = Convert.ToInt64(Item.SRI_ITM_Number);
                                        SR_DTO.SR_WH_Number = Convert.ToInt64(Item.SRI_WH_Number);
                                        SR_DTO.SR_UoM_Number = Convert.ToInt64(Item.SRI_UoM_Number);
                                        SR_DTO.SR_Qty = Convert.ToDouble(Item.SRI_Qty);
                                        SR_DTO.SR_UnitPrice = Convert.ToDouble(Item.SRI_UnitPrice);
                                        SR_DTO.SR_Amount = Convert.ToDouble(Item.SRI_Amount);
                                        SR_DTO.SR_IncomeValue = Convert.ToDouble(Item.SRI_IncomeValue);
                                        SR_DTO.SR_HSN_Number = Convert.ToInt64(Item.SRI_HSN_Number);
                                        SR_DTO.SR_GST_Amount = Convert.ToDouble(Item.SRI_GST_Amount);
                                        SR_DTO.SR_WHT_Percent = Convert.ToDouble(Item.SRI_WHT_Percent);
                                        SR_DTO.SR_WHT_Amount = Convert.ToDouble(Item.SRI_WHT_Amount);
                                        if (Item.SRI_Number == 0)
                                        {
                                            SR_DTO.SR_Id = 32;
                                            D = SR_DAO.SaleRejectionDB(SR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (Convert.ToInt64(x.SRI_INC_ITM_Number) == Item.SRI_ITM_Number) && (x.SRI_INC_ITM_Index == Item.SRI_Index));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SR_DTO.SR_ITM_Number = Convert.ToInt64(ItemInc.SRI_INC_ITM_Number);
                                                SR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                if (ItemInc.SRI_INC_Number == 0)
                                                {
                                                    SR_DTO.SR_Id = 34;
                                                }
                                                else
                                                {
                                                    SR_DTO.SR_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_Number);
                                                    SR_DTO.SR_Id = 124;
                                                }
                                                SR_DAO.SaleRejectionDB(SR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == Item.SRI_ITM_Number) && (x.SRI_BCH_ITM_Index == Item.SRI_Index));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                if (ItemBatch.SRI_BCH_Number == 0)
                                                {
                                                    SR_DTO.SR_Id = 35;
                                                }
                                                else
                                                {
                                                    SR_DTO.SR_BCH_Number = Convert.ToInt64(ItemBatch.SRI_BCH_Number);
                                                    SR_DTO.SR_Id = 125;
                                                }
                                                SR_DAO.SaleRejectionDB(SR_DTO);
                                            }
                                        }
                                        else
                                        {
                                            SR_DTO.SR_I_Number = Convert.ToInt64(Item.SRI_Number);
                                            SR_DTO.SR_Id = 122;
                                            D = SR_DAO.SaleRejectionDB(SR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (Convert.ToInt64(x.SRI_INC_SRI_Number) == Item.SRI_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SR_DTO.SR_ITM_Number = Convert.ToInt64(ItemInc.SRI_INC_ITM_Number);
                                                SR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                if (ItemInc.SRI_INC_Number == 0)
                                                {
                                                    SR_DTO.SR_Id = 34;
                                                }
                                                else
                                                {
                                                    SR_DTO.SR_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_Number);
                                                    SR_DTO.SR_Id = 124;
                                                }
                                                SR_DAO.SaleRejectionDB(SR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_SRI_Number == Item.SRI_Number));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                if (ItemBatch.SRI_BCH_Number == 0)
                                                {
                                                    SR_DTO.SR_Id = 35;
                                                }
                                                else
                                                {
                                                    SR_DTO.SR_BCH_Number = Convert.ToInt64(ItemBatch.SRI_BCH_Number);
                                                    SR_DTO.SR_Id = 125;
                                                }
                                                SR_DAO.SaleRejectionDB(SR_DTO);
                                            }
                                        }
                                    }
                                    foreach (var Income in Income_DTO)
                                    {
                                        SR_DTO.SR_INC_MIC_Number = Convert.ToInt64(Income.SRH_INC_MIC_Number);
                                        SR_DTO.SR_INC_Remarks = Income.SRH_INC_Remarks;
                                        SR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(Income.SRH_INC_OCRN_Number);
                                        SR_DTO.SR_INC_CM_Number = Convert.ToInt64(Income.SRH_INC_CM_Number);
                                        SR_DTO.SR_INC_IncomeBase = Convert.ToDouble(Income.SRH_INC_IncomeBase);
                                        SR_DTO.SR_INC_IncomeValue = Convert.ToDouble(Income.SRH_INC_IncomeValue);
                                        SR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(Income.SRH_INC_ALCT_Number);
                                        SR_DTO.SR_INC_LA_Number = Convert.ToInt64(Income.SRH_INC_LA_Number);
                                        SR_DTO.SR_INC_CalculateGST = Convert.ToInt64(Income.SRH_INC_CalculateGST);
                                        SR_DTO.SR_INC_GST_Amount = Convert.ToDouble(Income.SRH_INC_GST_Amount);
                                        SR_DTO.SR_INC_SAC_Number = Convert.ToInt64(Income.SRH_INC_SAC_Number);
                                        SR_DTO.SR_INC_WHT_Percent = Convert.ToDouble(Income.SRH_INC_WHT_Percent);
                                        SR_DTO.SR_INC_WHT_Amount = Convert.ToDouble(Income.SRH_INC_WHT_Amount);
                                        if (Income.SRH_INC_Number == 0)
                                        {
                                            SR_DTO.SR_Id = 33;
                                        }
                                        else
                                        {
                                            SR_DTO.SR_INC_Number = Convert.ToInt64(Income.SRH_INC_Number);
                                            SR_DTO.SR_Id = 123;
                                        }
                                        SR_DAO.SaleRejectionDB(SR_DTO);
                                    }

                                    foreach (var BuyerAddress in BuyerAddress_DTO)
                                    {
                                        SR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_ADTP_Number);
                                        SR_DTO.SR_ADD_AddressID = Convert.ToString(BuyerAddress.SRH_ADD_AddressID);
                                        SR_DTO.SR_ADD_Address = Convert.ToString(BuyerAddress.SRH_ADD_Address);
                                        SR_DTO.SR_ADD_City = Convert.ToString(BuyerAddress.SRH_ADD_City);
                                        SR_DTO.SR_ADD_State = Convert.ToString(BuyerAddress.SRH_ADD_State);
                                        SR_DTO.SR_ADD_Country = Convert.ToString(BuyerAddress.SRH_ADD_Country);
                                        SR_DTO.SR_ADD_Pin = Convert.ToString(BuyerAddress.SRH_ADD_Pin);
                                        SR_DTO.SR_ADD_GSTIN = Convert.ToString(BuyerAddress.SRH_ADD_GSTIN);
                                        if (BuyerAddress.SRH_ADD_Number == 0)
                                        {
                                            SR_DTO.SR_Id = 36;
                                        }
                                        else
                                        {
                                            SR_DTO.SR_ADD_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_Number);
                                            SR_DTO.SR_Id = 126;
                                        }
                                        SR_DAO.SaleRejectionDB(SR_DTO);
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);
                                    return RedirectToAction("SaleRejectionRegisterSummary");
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;
                    }
                }
            }

            SaleGetData();
            return View(Original_DTO);
        }

        String GetSREditData(String SR_No)
        {
            SR_DTO.SR_Number = Convert.ToInt64(SR_No);
            SR_DTO.SR_Id = 51;
            SR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            DS = SR_DAO.SaleRejectionDB(SR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SRH_DTO = S_DL.SRHeadEditList(DS.Tables[0]).FirstOrDefault();
                SRH_DTO.RejectionItem = S_DL.SRItemEditList(DS.Tables[1]);
                SRH_DTO.Income = S_DL.SRIncomeEditList(DS.Tables[2]);
                SRH_DTO.ItemIncome = S_DL.SRIIncomeEditList(DS.Tables[3]);
                SRH_DTO.ItemBatch = S_DL.SRIBatchEditList(DS.Tables[4]);
                SRH_DTO.BuyerAddress = S_DL.SRHAddressEditList(DS.Tables[5]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["SRH_Mode"].ToString();
                return "1";
            }
            else
            {
                return "0";
            }
        }







        //SI To Item Sale Rejection Create
        [Route("sale/transactions/sale-rejection/si/create")]
        public IActionResult SIToCreateSaleRejection()
        {
            SIToSaleRejectionHead_DTO SHSI_DTO = new SIToSaleRejectionHead_DTO();
            if (TempData["SHSI_DTO_Json"] is string SHSODto)
            {
                SHSI_DTO = System.Text.Json.JsonSerializer.Deserialize<SIToSaleRejectionHead_DTO>(SHSODto);
            }

            if (SHSI_DTO.SRH_RejectionDate == null)
            {
                SHSI_DTO.SRH_RejectionDate = DateTime.Now.ToString("dd-MMM-yy");
                SHSI_DTO.SRH_RejectionNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            }
            else
            {
                SHSI_DTO.SRH_RejectionNo = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(SHSI_DTO.SRH_RejectionDate).ToString("yyyyMMdd")));
            }
            SIToSaleRejectionGetData();

            SHSI_DTO.SRH_RejectionNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            return View(SHSI_DTO);
        }
        void SIToSaleRejectionGetData()
        {
            SISR_DTO.SR_RejectionDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            SISR_DTO.SR_Id = 2;
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

            ViewBag.IncomeCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[4]);
            ViewBag.UoM = Help.GetCat(DS.Tables[5]);
            ViewBag.HSN = Help.GetCat(DS.Tables[6]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[7]);
            ViewBag.WHTax = Help.GetCat(DS.Tables[8]);
            ViewBag.IsCalculate = Help.GetCat(DS.Tables[9]);
            ViewBag.LedgerAccount = Help.GetCat(DS.Tables[10]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[11]);
        }

        [HttpPost]
        [Route("sale/transactions/sale-rejection/si/create")]
        public IActionResult SIToCreateSaleRejection(SIToSaleRejectionHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SIToSaleRejectionHead_DTO S_Head_DTO = new SIToSaleRejectionHead_DTO();

            List<SIToSaleRejectionItem_DTO>? ITM_DTO = new List<SIToSaleRejectionItem_DTO>();
            List<SIToSaleRejectionIncome_DTO>? Income_DTO = new List<SIToSaleRejectionIncome_DTO>();
            List<SIToSaleRejectionIIncome_DTO>? ItemIncome_DTO = new List<SIToSaleRejectionIIncome_DTO>();
            List<SIToSaleRejectionBatch_DTO>? ItemBatch_DTO = new List<SIToSaleRejectionBatch_DTO>();
            List<SIToSaleRejectionAddress_DTO>? BuyerAddress_DTO = new List<SIToSaleRejectionAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.RejectionItem != null)
                ITM_DTO = S_DTO.RejectionItem!.Where(K => K.SRI_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SRH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SRI_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemBatch != null)
                ItemBatch_DTO = S_DTO.ItemBatch!.Where(K => K.SRI_BCH_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SRH_ADD_IsDeleted == 0).ToList();

            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SRI_MS_Number) != Convert.ToInt64(S_DTO.SRH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SRI_Qty) == 0 || Convert.ToDouble(x.SRI_UnitPrice) == 0 || Convert.ToDouble(x.SRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SRH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SRH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.RejectionItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.ItemBatch = ItemBatch_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (SIToBatchValidation(ITM_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String SIHOrderNoOld = S_DTO.SRH_RejectionNo;
                                    String SIHOrderNoNew = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd")));

                                    SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd"));
                                    SISR_DTO.SR_RejectionNo = SIHOrderNoNew;
                                    SISR_DTO.SR_BUY_Number = Convert.ToInt64(S_DTO.SRH_BUY_Number);
                                    SISR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 1 : 0);
                                    SISR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SISR_DTO.SR_MS_Number = Convert.ToInt64(S_DTO.SRH_MS_Number);
                                    SISR_DTO.SR_TCT_Number = Convert.ToInt64(S_DTO.SRH_TCT_Number);
                                    SISR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SISR_DTO.SR_WHT_Number = Convert.ToInt64(S_DTO.SRH_WHT_Number);
                                    SISR_DTO.SR_ExchangeRate = Convert.ToDouble(S_DTO.SRH_ExchangeRate) == 0 ? "1" : S_DTO.SRH_ExchangeRate;
                                    SISR_DTO.SR_MaterialCost = Convert.ToDouble(S_DTO.SRH_MaterialCost);
                                    SISR_DTO.SR_ItemMiscIncome = Convert.ToDouble(S_DTO.SRH_ItemMiscIncome);
                                    SISR_DTO.SR_HeaderMiscIncome = Convert.ToDouble(S_DTO.SRH_HeaderMiscIncome);
                                    SISR_DTO.SR_GST_Amount = Convert.ToDouble(S_DTO.SRH_GST_Amount);
                                    SISR_DTO.SR_RejectionAmount = Convert.ToDouble(S_DTO.SRH_RejectionAmount);
                                    SISR_DTO.SR_WHT_Amount = Convert.ToDouble(S_DTO.SRH_WHT_Amount);
                                    SISR_DTO.SR_RoundOff = Convert.ToDouble(S_DTO.SRH_RoundOff);
                                    SISR_DTO.SR_BuyerReceivable = Convert.ToDouble(S_DTO.SRH_BuyerReceivable);
                                    SISR_DTO.SR_Id = 21;
                                    DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                    OnSaleNumberGen(Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        SISR_DTO.SR_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in ITM_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            SISR_DTO.SR_SIH_Number = Convert.ToInt64(Item.SRI_SIH_Number);
                                            SISR_DTO.SR_SII_Number = Convert.ToInt64(Item.SRI_SII_Number);
                                            SISR_DTO.SR_ITM_Number = Convert.ToInt64(Item.SRI_ITM_Number);
                                            SISR_DTO.SR_WH_Number = Convert.ToInt64(Item.SRI_WH_Number);
                                            SISR_DTO.SR_UoM_Number = Convert.ToInt64(Item.SRI_UoM_Number);
                                            SISR_DTO.SR_Qty = Convert.ToDouble(Item.SRI_Qty);
                                            SISR_DTO.SR_UnitPrice = Convert.ToDouble(Item.SRI_UnitPrice);
                                            SISR_DTO.SR_Amount = Convert.ToDouble(Item.SRI_Amount);
                                            SISR_DTO.SR_IncomeValue = Convert.ToDouble(Item.SRI_IncomeValue);
                                            SISR_DTO.SR_HSN_Number = Convert.ToInt64(Item.SRI_HSN_Number);
                                            SISR_DTO.SR_GST_Amount = Convert.ToDouble(Item.SRI_GST_Amount);
                                            SISR_DTO.SR_WHT_Percent = Convert.ToDouble(Item.SRI_WHT_Percent);
                                            SISR_DTO.SR_WHT_Amount = Convert.ToDouble(Item.SRI_WHT_Amount);
                                            SISR_DTO.SR_Id = 22;
                                            D = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SRI_INC_SII_Number == Item.SRI_SII_Number && x.SRI_INC_SIH_Number == Item.SRI_SIH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SISR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SISR_DTO.SR_SII_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_SII_INC_Number);
                                                //SISR_DTO.SR_ITM_Number = Convert.ToInt64(ItemInc.SRI_INC_ITM_Number);
                                                SISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SISR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SISR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SISR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                SISR_DTO.SR_Id = 24;
                                                SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == Item.SRI_ITM_Number) && (x.SRI_BCH_ITM_Index == Item.SRI_Index));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SISR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SISR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SISR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SISR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SISR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SISR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                SISR_DTO.SR_Id = 25;
                                                SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                            }
                                        }
                                        foreach (var Income in Income_DTO)
                                        {
                                            SISR_DTO.SR_SIH_INC_Number = Convert.ToInt64(Income.SRH_INC_SIH_INC_Number);
                                            SISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(Income.SRH_INC_MIC_Number);
                                            SISR_DTO.SR_INC_Remarks = Income.SRH_INC_Remarks;
                                            SISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(Income.SRH_INC_OCRN_Number);
                                            SISR_DTO.SR_INC_CM_Number = Convert.ToInt64(Income.SRH_INC_CM_Number);
                                            SISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(Income.SRH_INC_IncomeBase);
                                            SISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(Income.SRH_INC_IncomeValue);
                                            SISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(Income.SRH_INC_ALCT_Number);
                                            SISR_DTO.SR_INC_LA_Number = Convert.ToInt64(Income.SRH_INC_LA_Number);
                                            SISR_DTO.SR_INC_CalculateGST = Convert.ToInt64(Income.SRH_INC_CalculateGST);
                                            SISR_DTO.SR_INC_GST_Amount = Convert.ToDouble(Income.SRH_INC_GST_Amount);
                                            SISR_DTO.SR_INC_SAC_Number = Convert.ToInt64(Income.SRH_INC_SAC_Number);
                                            SISR_DTO.SR_INC_WHT_Percent = Convert.ToDouble(Income.SRH_INC_WHT_Percent);
                                            SISR_DTO.SR_INC_WHT_Amount = Convert.ToDouble(Income.SRH_INC_WHT_Amount);
                                            SISR_DTO.SR_Id = 23;
                                            SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                        }

                                        foreach (var BuyerAddress in BuyerAddress_DTO)
                                        {
                                            SISR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_ADTP_Number);
                                            SISR_DTO.SR_ADD_AddressID = Convert.ToString(BuyerAddress.SRH_ADD_AddressID);
                                            SISR_DTO.SR_ADD_Address = Convert.ToString(BuyerAddress.SRH_ADD_Address);
                                            SISR_DTO.SR_ADD_City = Convert.ToString(BuyerAddress.SRH_ADD_City);
                                            SISR_DTO.SR_ADD_State = Convert.ToString(BuyerAddress.SRH_ADD_State);
                                            SISR_DTO.SR_ADD_Country = Convert.ToString(BuyerAddress.SRH_ADD_Country);
                                            SISR_DTO.SR_ADD_Pin = Convert.ToString(BuyerAddress.SRH_ADD_Pin);
                                            SISR_DTO.SR_ADD_GSTIN = Convert.ToString(BuyerAddress.SRH_ADD_GSTIN);
                                            SISR_DTO.SR_Id = 26;
                                            SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    if (SIHOrderNoOld != SIHOrderNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage = "Sale Rejection number " + SIHOrderNoOld + " used by another user. Next number will be allotted to you.";
                                    }
                                    return RedirectToAction("SIToCreateSaleRejection");
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }

                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;
                    }
                }
            }
            else if (Mode == "Direct")
            {
                SaleRejectionHead_DTO SH_DTO = new SaleRejectionHead_DTO();

                SH_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SH_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SH_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SH_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SH_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SH_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SH_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SH_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SH_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SH_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SH_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SH_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SH_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleRejection");
            }
            else if (Mode == "SITOITEM")
            {
                SIToSaleRejectionHead_DTO SHSI_DTO = new SIToSaleRejectionHead_DTO();

                SHSI_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SHSI_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SHSI_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SHSI_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SHSI_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SHSI_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SHSI_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SHSI_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SHSI_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SHSI_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SHSI_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SHSI_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SHSI_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SHSI_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSI_DTO);

                return RedirectToAction("SIToCreateSaleRejection");
            }
            else if (Mode == "SIITEMTO")
            {
                SIItemToSaleRejectionHead_DTO SHSI_DTO = new SIItemToSaleRejectionHead_DTO();

                SHSI_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SHSI_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SHSI_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SHSI_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SHSI_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SHSI_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SHSI_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SHSI_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SHSI_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SHSI_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SHSI_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SHSI_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SHSI_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SHSI_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSI_DTO);

                return RedirectToAction("SIItemToCreateSaleRejection");
            }

            SIToSaleRejectionGetData();
            return View(Original_DTO);
        }

        Boolean SIToBatchValidation(List<SIToSaleRejectionItem_DTO> Item_DTO, List<SIToSaleRejectionBatch_DTO>? ItemBatch_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                //Double BatchAmount = 0;

                Int64 SRINumber = Convert.ToInt64(Item.SRI_Number);
                Int64 SRIIndex = Convert.ToInt64(Item.SRI_Index);
                Int64 SRIItem = Convert.ToInt64(Item.SRI_ITM_Number);
                Double SRIQty = Math.Abs(Convert.ToDouble(Item.SRI_Qty));
                Double SRIAmount = Convert.ToDouble(Item.SRI_Amount);

                if (SRINumber != 0)
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_SRI_Number == SRINumber));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                    }
                }
                else
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == SRIItem) && (x.SRI_BCH_ITM_Index == SRIIndex));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                    }
                }


                if (BatchQty == SRIQty) { }
                else
                {
                    Message += Item.SRI_ITM_Code + " Batch Qty  Mismatched <br/>";
                    Result = false;
                }

                if (Message != "")
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Message;
                }
            }
            return Result;
        }


        [Route("sale/transactions/sale-rejection/si/uom")]
        public String SIToSaleUoM(String? UoM)
        {
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_UoM_Number = Convert.ToInt64(UoM);
            SISR_DTO.SR_Id = 3;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("sale/transactions/sale-rejection/si/buyer")]
        public IActionResult SIToSaleBuyer(String? Buyer, String? Export, String? SRHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            if (Export == null)
            {
                Export = "";
            }
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(Export) == true ? 2 : 1);
            SISR_DTO.SR_ITM_Code = Convert.ToString(Buyer).Trim();
            SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SISR_DTO.SR_Id = 4;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            var Ven = S_DL.BuyerList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("sale/transactions/sale-rejection/si/gst")]
        public String SIToSaleRejectionGst(String? Cluster, String? SRHDate, String? HSN, String? BaseAmount)
        {
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SISR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SISR_DTO.SR_HSN_Number = Convert.ToInt64(HSN);
            SISR_DTO.SR_Id = 5;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("sale/transactions/sale-rejection/si/gst/view")]
        public IActionResult SIToSaleRejectionGstView(String? Cluster, String? SRHDate, String? HSN, String? BaseAmount)
        {
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SISR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SISR_DTO.SR_HSN_Number = Convert.ToInt64(HSN);
            SISR_DTO.SR_Id = 8;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleRejectionGst> PurGST = new List<SaleRejectionGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new SaleRejectionGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("sale/transactions/sale-rejection/si/wht")]
        public IActionResult SIToSaleRejectionWHT(String? Buyer, String? WHTNumber, String? SRHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Buyer == null)
            {
                Buyer = "0";
            }
            if (SRHDate == null)
            {
                SRHDate = "0";
            }
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SISR_DTO.SR_WHT_Number = Convert.ToInt64(WHTNumber);
            SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SISR_DTO.SR_Id = 6;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            var WHT = S_DL.SaleInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("sale/transactions/sale-rejection/si/cluster")]
        public IActionResult SIToSaleRejectionCluster(String? Buyer, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            SISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SISR_DTO.SR_Search = Cluster;
            SISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SISR_DTO.SR_Id = 7;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            var InvCluster = S_DL.SaleCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("sale/transactions/sale-rejection/si/invoice")]
        public IActionResult SIToSaleRejectionSI(String? Buyer, String? Export, String? MSNumber, String? SRHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }

            SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SISR_DTO.SR_MS_Number = Convert.ToInt64(MSNumber);
            SISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SISR_DTO.SR_ExportOrder = Convert.ToInt32(Convert.ToBoolean(Export) == true ? 1 : 0);
            SISR_DTO.SR_Id = 11;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            var POOrder = S_DL.SIToSRInvoice(DS.Tables[0]);
            return Json(POOrder);
        }

        [Route("sale/transactions/sale-rejection/si/invoice/item")]
        public IActionResult SIToSaleRejectionSOItem(String? SINumber)
        {
            if (SINumber == null)
            {
                SINumber = "";
            }

            SISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SISR_DTO.SR_Search = SINumber;
            SISR_DTO.SR_Id = 12;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

            SIToSR_DTO SOSI = new SIToSR_DTO();
            var SIItem = S_DL.SIToSRInvoiceItem(DS.Tables[0]);
            var SIIncome = S_DL.SIToSRIncome(DS.Tables[1]);
            var SIItemIncome = S_DL.SIToSRIIncome(DS.Tables[2]);

            SOSI.SIItems = SIItem;
            SOSI.SIIncomes = SIIncome;
            SOSI.SIItemIncomes = SIItemIncome;

            return Json(SOSI);
        }

        [Route("sale/transactions/sale-rejection/si/income/gst")]
        public String SIToSaleRejectionHeaderGst(String? Cluster, String? SRHDate, String? SAC, String? BaseAmount)
        {
            SISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SISR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SISR_DTO.SR_INC_SAC_Number = Convert.ToInt64(SAC);
            SISR_DTO.SR_Id = 13;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("sale/transactions/sale-rejection/si/income/gst/view")]
        public IActionResult SIToSaleRejectionGstHeaderView(String? Cluster, String? SRHDate, String? SAC, String? BaseAmount)
        {
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SISR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SISR_DTO.SR_INC_SAC_Number = Convert.ToInt64(SAC);
            SISR_DTO.SR_Id = 14;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleRejectionGst> PurGST = new List<SaleRejectionGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new SaleRejectionGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("sale/transactions/sale-rejection/si/buyer/address")]
        public IActionResult SIToSaleBuyerAddressID(String? Buyer, String ADTPNumber)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SISR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SISR_DTO.SR_Id = 15;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            SaleRejectionAddress SRA = new SaleRejectionAddress();
            SRA.BuyerAddressId = S_DL.BuyerAddressID(DS.Tables[0]);
            SRA.BuyerAddress = S_DL.BuyerAddress(DS.Tables[1]).FirstOrDefault();
            return Json(SRA);
        }

        [Route("sale/transactions/sale-rejection/si/buyer/address/addressid")]
        public IActionResult SIToSaleBuyerAddress(String? Buyer, String ADTPNumber, String AddressID)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SISR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SISR_DTO.SR_ADD_AddressID = Convert.ToString(AddressID);
            SISR_DTO.SR_Id = 16;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            var Address = S_DL.BuyerAddress(DS.Tables[0]).FirstOrDefault();
            return Json(Address);
        }




        //Sale Rejection SI To Rejection Edit
        [Route("sale/transactions/sale-rejection/si/{SR_No}/edit")]
        public IActionResult SITOEditSaleRejection(String SR_No)
        {
            SIToSaleRejectionGetData();
            String Active = GetSISREditData(SR_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleRejectionRegisterSummary");
            }
            ViewBag.SR_No = SR_No;

            return View(SISRH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-rejection/si/{SR_No}/edit")]
        public IActionResult SIToEditSaleRejection(SIToSaleRejectionHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SIToSaleRejectionHead_DTO S_Head_DTO = new SIToSaleRejectionHead_DTO();

            List<SIToSaleRejectionItem_DTO>? ITM_DTO = new List<SIToSaleRejectionItem_DTO>();
            List<SIToSaleRejectionIncome_DTO>? Income_DTO = new List<SIToSaleRejectionIncome_DTO>();
            List<SIToSaleRejectionIIncome_DTO>? ItemIncome_DTO = new List<SIToSaleRejectionIIncome_DTO>();
            List<SIToSaleRejectionBatch_DTO>? ItemBatch_DTO = new List<SIToSaleRejectionBatch_DTO>();
            List<SIToSaleRejectionAddress_DTO>? BuyerAddress_DTO = new List<SIToSaleRejectionAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.RejectionItem != null)
                ITM_DTO = S_DTO.RejectionItem!.Where(K => K.SRI_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SRH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SRI_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemBatch != null)
                ItemBatch_DTO = S_DTO.ItemBatch!.Where(K => K.SRI_BCH_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SRH_ADD_IsDeleted == 0).ToList();

            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SRI_MS_Number) != Convert.ToInt64(S_DTO.SRH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SRI_Qty) == 0 || Convert.ToDouble(x.SRI_UnitPrice) == 0 || Convert.ToDouble(x.SRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SRH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SRH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.RejectionItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.ItemBatch = ItemBatch_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (SIToBatchValidation(ITM_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    SISR_DTO.SR_Number = Convert.ToInt64(S_DTO.SRH_Number);
                                    SISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd"));
                                    SISR_DTO.SR_RejectionNo = Convert.ToString(S_DTO.SRH_RejectionNo);
                                    SISR_DTO.SR_BUY_Number = Convert.ToInt64(S_DTO.SRH_BUY_Number);
                                    SISR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 1 : 0);
                                    SISR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SISR_DTO.SR_MS_Number = Convert.ToInt64(S_DTO.SRH_MS_Number);
                                    SISR_DTO.SR_TCT_Number = Convert.ToInt64(S_DTO.SRH_TCT_Number);
                                    SISR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SISR_DTO.SR_WHT_Number = Convert.ToInt64(S_DTO.SRH_WHT_Number);
                                    SISR_DTO.SR_ExchangeRate = Convert.ToDouble(S_DTO.SRH_ExchangeRate) == 0 ? "1" : S_DTO.SRH_ExchangeRate;
                                    SISR_DTO.SR_MaterialCost = Convert.ToDouble(S_DTO.SRH_MaterialCost);
                                    SISR_DTO.SR_ItemMiscIncome = Convert.ToDouble(S_DTO.SRH_ItemMiscIncome);
                                    SISR_DTO.SR_HeaderMiscIncome = Convert.ToDouble(S_DTO.SRH_HeaderMiscIncome);
                                    SISR_DTO.SR_GST_Amount = Convert.ToDouble(S_DTO.SRH_GST_Amount);
                                    SISR_DTO.SR_RejectionAmount = Convert.ToDouble(S_DTO.SRH_RejectionAmount);
                                    SISR_DTO.SR_WHT_Amount = Convert.ToDouble(S_DTO.SRH_WHT_Amount);
                                    SISR_DTO.SR_RoundOff = Convert.ToDouble(S_DTO.SRH_RoundOff);
                                    SISR_DTO.SR_BuyerReceivable = Convert.ToDouble(S_DTO.SRH_BuyerReceivable);
                                    SISR_DTO.SR_Id = 121;
                                    DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                    String ItemDTO = string.Join(", ", ITM_DTO.Where(x => Convert.ToInt64(x.SRI_Number) != 0).Select(x => x.SRI_Number));
                                    String ItemIncomeDTO = string.Join(", ", ItemIncome_DTO.Where(x => Convert.ToInt64(x.SRI_INC_Number) != 0).Select(x => x.SRI_INC_Number));
                                    String IncomeDTO = string.Join(", ", Income_DTO.Where(x => Convert.ToInt64(x.SRH_INC_Number) != 0).Select(x => x.SRH_INC_Number));
                                    String BatchDTO = string.Join(", ", ItemBatch_DTO.Where(x => Convert.ToInt64(x.SRI_BCH_Number) != 0).Select(x => x.SRI_BCH_Number));
                                    String AddressDTO = string.Join(", ", BuyerAddress_DTO.Where(x => Convert.ToInt64(x.SRH_ADD_Number) != 0).Select(x => x.SRH_ADD_Number));

                                    SISR_DTO.SR_DeleteNumbers = Convert.ToString(ItemDTO);
                                    SISR_DTO.SR_Id = 101;
                                    SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                    SISR_DTO.SR_DeleteNumbers = Convert.ToString(IncomeDTO);
                                    SISR_DTO.SR_Id = 102;
                                    SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                    SISR_DTO.SR_DeleteNumbers = Convert.ToString(ItemIncomeDTO);
                                    SISR_DTO.SR_Id = 103;
                                    SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                    SISR_DTO.SR_DeleteNumbers = Convert.ToString(BatchDTO);
                                    SISR_DTO.SR_Id = 104;
                                    SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                    SISR_DTO.SR_DeleteNumbers = Convert.ToString(AddressDTO);
                                    SISR_DTO.SR_Id = 105;
                                    SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                    foreach (var Item in ITM_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        SISR_DTO.SR_SIH_Number = Convert.ToInt64(Item.SRI_SIH_Number);
                                        SISR_DTO.SR_SII_Number = Convert.ToInt64(Item.SRI_SII_Number);
                                        SISR_DTO.SR_ITM_Number = Convert.ToInt64(Item.SRI_ITM_Number);
                                        SISR_DTO.SR_WH_Number = Convert.ToInt64(Item.SRI_WH_Number);
                                        SISR_DTO.SR_UoM_Number = Convert.ToInt64(Item.SRI_UoM_Number);
                                        SISR_DTO.SR_Qty = Convert.ToDouble(Item.SRI_Qty);
                                        SISR_DTO.SR_UnitPrice = Convert.ToDouble(Item.SRI_UnitPrice);
                                        SISR_DTO.SR_Amount = Convert.ToDouble(Item.SRI_Amount);
                                        SISR_DTO.SR_IncomeValue = Convert.ToDouble(Item.SRI_IncomeValue);
                                        SISR_DTO.SR_HSN_Number = Convert.ToInt64(Item.SRI_HSN_Number);
                                        SISR_DTO.SR_GST_Amount = Convert.ToDouble(Item.SRI_GST_Amount);
                                        SISR_DTO.SR_WHT_Percent = Convert.ToDouble(Item.SRI_WHT_Percent);
                                        SISR_DTO.SR_WHT_Amount = Convert.ToDouble(Item.SRI_WHT_Amount);
                                        if (Item.SRI_Number == 0)
                                        {
                                            SISR_DTO.SR_Id = 22;
                                            D = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SRI_INC_SII_Number == Item.SRI_SII_Number && x.SRI_INC_SIH_Number == Item.SRI_SIH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SISR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SISR_DTO.SR_SII_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_SII_INC_Number);
                                                SISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SISR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SISR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SISR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                if (ItemInc.SRI_INC_Number == 0)
                                                {
                                                    SISR_DTO.SR_Id = 24;
                                                }
                                                else
                                                {
                                                    SISR_DTO.SR_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_Number);
                                                    SISR_DTO.SR_Id = 124;
                                                }
                                                SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == Item.SRI_ITM_Number) && (x.SRI_BCH_ITM_Index == Item.SRI_Index));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SISR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SISR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SISR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SISR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SISR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SISR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                SISR_DTO.SR_Id = 25;
                                                SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                            }
                                        }
                                        else
                                        {
                                            SISR_DTO.SR_I_Number = Convert.ToInt64(Item.SRI_Number);
                                            SISR_DTO.SR_Id = 122;
                                            D = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (Convert.ToInt64(x.SRI_INC_SRI_Number) == Item.SRI_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SISR_DTO.SR_I_Number = Convert.ToInt64(Item.SRI_Number);
                                                SISR_DTO.SR_SII_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_SII_INC_Number);
                                                SISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SISR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SISR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SISR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                if (ItemInc.SRI_INC_Number == 0)
                                                {
                                                    SISR_DTO.SR_Id = 24;
                                                }
                                                else
                                                {
                                                    SISR_DTO.SR_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_Number);
                                                    SISR_DTO.SR_Id = 124;
                                                }
                                                SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_SRI_Number == Item.SRI_Number));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SISR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SISR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SISR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SISR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SISR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                if (ItemBatch.SRI_BCH_Number == 0)
                                                {
                                                    SISR_DTO.SR_Id = 25;
                                                }
                                                else
                                                {
                                                    SISR_DTO.SR_BCH_Number = Convert.ToInt64(ItemBatch.SRI_BCH_Number);
                                                    SISR_DTO.SR_Id = 125;
                                                }
                                                SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                            }
                                        }

                                    }
                                    foreach (var Income in Income_DTO)
                                    {
                                        SISR_DTO.SR_SIH_INC_Number = Convert.ToInt64(Income.SRH_INC_SIH_INC_Number);
                                        SISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(Income.SRH_INC_MIC_Number);
                                        SISR_DTO.SR_INC_Remarks = Income.SRH_INC_Remarks;
                                        SISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(Income.SRH_INC_OCRN_Number);
                                        SISR_DTO.SR_INC_CM_Number = Convert.ToInt64(Income.SRH_INC_CM_Number);
                                        SISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(Income.SRH_INC_IncomeBase);
                                        SISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(Income.SRH_INC_IncomeValue);
                                        SISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(Income.SRH_INC_ALCT_Number);
                                        SISR_DTO.SR_INC_LA_Number = Convert.ToInt64(Income.SRH_INC_LA_Number);
                                        SISR_DTO.SR_INC_CalculateGST = Convert.ToInt64(Income.SRH_INC_CalculateGST);
                                        SISR_DTO.SR_INC_GST_Amount = Convert.ToDouble(Income.SRH_INC_GST_Amount);
                                        SISR_DTO.SR_INC_SAC_Number = Convert.ToInt64(Income.SRH_INC_SAC_Number);
                                        SISR_DTO.SR_INC_WHT_Percent = Convert.ToDouble(Income.SRH_INC_WHT_Percent);
                                        SISR_DTO.SR_INC_WHT_Amount = Convert.ToDouble(Income.SRH_INC_WHT_Amount);
                                        if (Income.SRH_INC_Number == 0)
                                        {
                                            SISR_DTO.SR_Id = 23;
                                        }
                                        else
                                        {
                                            SISR_DTO.SR_INC_Number = Convert.ToInt64(Income.SRH_INC_Number);
                                            SISR_DTO.SR_Id = 123;
                                        }
                                        SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                    }

                                    foreach (var BuyerAddress in BuyerAddress_DTO)
                                    {
                                        SISR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_ADTP_Number);
                                        SISR_DTO.SR_ADD_AddressID = Convert.ToString(BuyerAddress.SRH_ADD_AddressID);
                                        SISR_DTO.SR_ADD_Address = Convert.ToString(BuyerAddress.SRH_ADD_Address);
                                        SISR_DTO.SR_ADD_City = Convert.ToString(BuyerAddress.SRH_ADD_City);
                                        SISR_DTO.SR_ADD_State = Convert.ToString(BuyerAddress.SRH_ADD_State);
                                        SISR_DTO.SR_ADD_Country = Convert.ToString(BuyerAddress.SRH_ADD_Country);
                                        SISR_DTO.SR_ADD_Pin = Convert.ToString(BuyerAddress.SRH_ADD_Pin);
                                        SISR_DTO.SR_ADD_GSTIN = Convert.ToString(BuyerAddress.SRH_ADD_GSTIN);
                                        if (BuyerAddress.SRH_ADD_Number == 0)
                                        {
                                            SISR_DTO.SR_Id = 26;
                                        }
                                        else
                                        {
                                            SISR_DTO.SR_ADD_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_Number);
                                            SISR_DTO.SR_Id = 126;
                                        }
                                        SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    return RedirectToAction("SaleRejectionRegisterSummary");
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;
                    }
                }
            }
            else if (Mode == "Direct")
            {
                SaleRejectionHead_DTO SH_DTO = new SaleRejectionHead_DTO();

                SH_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SH_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SH_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SH_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SH_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SH_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SH_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SH_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SH_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SH_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SH_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SH_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SH_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleRejection");
            }
            else if (Mode == "SITOITEM")
            {
                SIToSaleRejectionHead_DTO SHSI_DTO = new SIToSaleRejectionHead_DTO();

                SHSI_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SHSI_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SHSI_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SHSI_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SHSI_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SHSI_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SHSI_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SHSI_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SHSI_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SHSI_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SHSI_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SHSI_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SHSI_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SHSI_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSI_DTO);

                return RedirectToAction("SIToCreateSaleRejection");
            }
            else if (Mode == "SIITEMTO")
            {
                SIItemToSaleRejectionHead_DTO SHSI_DTO = new SIItemToSaleRejectionHead_DTO();

                SHSI_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SHSI_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SHSI_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SHSI_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SHSI_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SHSI_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SHSI_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SHSI_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SHSI_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SHSI_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SHSI_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SHSI_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SHSI_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SHSI_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSI_DTO);

                return RedirectToAction("SIItemToCreateSaleRejection");
            }

            SIToSaleRejectionGetData();
            return View(Original_DTO);
        }
        String GetSISREditData(String SR_No)
        {
            SISR_DTO.SR_Number = Convert.ToInt64(SR_No);
            SISR_DTO.SR_Id = 61;
            SISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SISRH_DTO = S_DL.SIToSRHeadEditList(DS.Tables[0]).FirstOrDefault();
                SISRH_DTO.RejectionItem = S_DL.SIToSRItemEditList(DS.Tables[1]);
                SISRH_DTO.Income = S_DL.SIToSRIncomeEditList(DS.Tables[2]);
                SISRH_DTO.ItemIncome = S_DL.SIToSRIIncomeEditList(DS.Tables[3]);
                SISRH_DTO.ItemBatch = S_DL.SIToSRIBatchEditList(DS.Tables[4]);
                SISRH_DTO.BuyerAddress = S_DL.SIToSRHAddressEditList(DS.Tables[5]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["SRH_Mode"].ToString();
                return "1";
            }
            else
            {
                return "0";
            }
        }






        //Sale Invoice SI Item to Invoice Create
        [Route("sale/transactions/sale-rejection/siitem/create")]
        public IActionResult SIItemToCreateSaleRejection()
        {
            SIItemToSaleRejectionHead_DTO SIISRH_DTO = new SIItemToSaleRejectionHead_DTO();
            if (TempData["SHSO_DTO_Json"] is string PHPODto)
            {
                SIISRH_DTO = System.Text.Json.JsonSerializer.Deserialize<SIItemToSaleRejectionHead_DTO>(PHPODto);
            }

            if (SIISRH_DTO.SRH_RejectionDate == null)
            {
                SIISRH_DTO.SRH_RejectionDate = DateTime.Now.ToString("dd-MMM-yy");
                SIISRH_DTO.SRH_RejectionNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            }
            else
            {
                SIISRH_DTO.SRH_RejectionNo = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(SIISRH_DTO.SRH_RejectionDate).ToString("yyyyMMdd")));
            }
            SIItemToSaleRejectionGetData();

            return View(SIISRH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-rejection/siitem/create")]
        public IActionResult SIItemToCreateSaleRejection(SIItemToSaleRejectionHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SIItemToSaleRejectionHead_DTO S_Head_DTO = new SIItemToSaleRejectionHead_DTO();

            List<SIItemToSaleRejectionItem_DTO>? ITM_DTO = new List<SIItemToSaleRejectionItem_DTO>();
            List<SIItemToSaleRejectionIncome_DTO>? Income_DTO = new List<SIItemToSaleRejectionIncome_DTO>();
            List<SIItemToSaleRejectionIIncome_DTO>? ItemIncome_DTO = new List<SIItemToSaleRejectionIIncome_DTO>();
            List<SIItemToSaleRejectionBatch_DTO>? ItemBatch_DTO = new List<SIItemToSaleRejectionBatch_DTO>();
            List<SIItemToSaleRejectionAddress_DTO>? BuyerAddress_DTO = new List<SIItemToSaleRejectionAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.RejectionItem != null)
                ITM_DTO = S_DTO.RejectionItem!.Where(K => K.SRI_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SRH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SRI_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemBatch != null)
                ItemBatch_DTO = S_DTO.ItemBatch!.Where(K => K.SRI_BCH_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SRH_ADD_IsDeleted == 0).ToList();

            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SRI_MS_Number) != Convert.ToInt64(S_DTO.SRH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SRI_Qty) == 0 || Convert.ToDouble(x.SRI_UnitPrice) == 0 || Convert.ToDouble(x.SRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SRH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SRH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.RejectionItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.ItemBatch = ItemBatch_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (SIItemToBatchValidation(ITM_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String SIHInvoiceNoOld = S_DTO.SRH_RejectionNo;
                                    String SIHInvoiceNoNew = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd")));

                                    SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd"));
                                    SIISR_DTO.SR_RejectionNo = SIHInvoiceNoNew;
                                    SIISR_DTO.SR_BUY_Number = Convert.ToInt64(S_DTO.SRH_BUY_Number);
                                    SIISR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 1 : 0);
                                    SIISR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SIISR_DTO.SR_MS_Number = Convert.ToInt64(S_DTO.SRH_MS_Number);
                                    SIISR_DTO.SR_TCT_Number = Convert.ToInt64(S_DTO.SRH_TCT_Number);
                                    SIISR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SIISR_DTO.SR_WHT_Number = Convert.ToInt64(S_DTO.SRH_WHT_Number);
                                    SIISR_DTO.SR_ExchangeRate = Convert.ToDouble(S_DTO.SRH_ExchangeRate) == 0 ? "1" : S_DTO.SRH_ExchangeRate;
                                    SIISR_DTO.SR_MaterialCost = Convert.ToDouble(S_DTO.SRH_MaterialCost);
                                    SIISR_DTO.SR_ItemMiscIncome = Convert.ToDouble(S_DTO.SRH_ItemMiscIncome);
                                    SIISR_DTO.SR_HeaderMiscIncome = Convert.ToDouble(S_DTO.SRH_HeaderMiscIncome);
                                    SIISR_DTO.SR_GST_Amount = Convert.ToDouble(S_DTO.SRH_GST_Amount);
                                    SIISR_DTO.SR_RejectionAmount = Convert.ToDouble(S_DTO.SRH_RejectionAmount);
                                    SIISR_DTO.SR_WHT_Amount = Convert.ToDouble(S_DTO.SRH_WHT_Amount);
                                    SIISR_DTO.SR_RoundOff = Convert.ToDouble(S_DTO.SRH_RoundOff);
                                    SIISR_DTO.SR_BuyerReceivable = Convert.ToDouble(S_DTO.SRH_BuyerReceivable);
                                    SIISR_DTO.SR_Id = 21;
                                    DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                    OnSaleNumberGen(Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        SIISR_DTO.SR_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in ITM_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            SIISR_DTO.SR_SIH_Number = Convert.ToInt64(Item.SRI_SIH_Number);
                                            SIISR_DTO.SR_SII_Number = Convert.ToInt64(Item.SRI_SII_Number);
                                            SIISR_DTO.SR_ITM_Number = Convert.ToInt64(Item.SRI_ITM_Number);
                                            SIISR_DTO.SR_WH_Number = Convert.ToInt64(Item.SRI_WH_Number);
                                            SIISR_DTO.SR_UoM_Number = Convert.ToInt64(Item.SRI_UoM_Number);
                                            SIISR_DTO.SR_Qty = Convert.ToDouble(Item.SRI_Qty);
                                            SIISR_DTO.SR_UnitPrice = Convert.ToDouble(Item.SRI_UnitPrice);
                                            SIISR_DTO.SR_Amount = Convert.ToDouble(Item.SRI_Amount);
                                            SIISR_DTO.SR_IncomeValue = Convert.ToDouble(Item.SRI_IncomeValue);
                                            SIISR_DTO.SR_HSN_Number = Convert.ToInt64(Item.SRI_HSN_Number);
                                            SIISR_DTO.SR_GST_Amount = Convert.ToDouble(Item.SRI_GST_Amount);
                                            SIISR_DTO.SR_WHT_Percent = Convert.ToDouble(Item.SRI_WHT_Percent);
                                            SIISR_DTO.SR_WHT_Amount = Convert.ToDouble(Item.SRI_WHT_Amount);
                                            SIISR_DTO.SR_Id = 22;
                                            D = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SRI_INC_SII_Number == Item.SRI_SII_Number && x.SRI_INC_SIH_Number == Item.SRI_SIH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SIISR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SIISR_DTO.SR_SII_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_SII_INC_Number);
                                                //siISR_DTO.SR_ITM_Number = Convert.ToInt64(ItemInc.SRI_INC_ITM_Number);
                                                SIISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SIISR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SIISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SIISR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SIISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SIISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SIISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SIISR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                SIISR_DTO.SR_Id = 24;
                                                SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == Item.SRI_ITM_Number) && (x.SRI_BCH_ITM_Index == Item.SRI_Index));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SIISR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SIISR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SIISR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SIISR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SIISR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SIISR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                SIISR_DTO.SR_Id = 25;
                                                SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                            }
                                        }
                                        foreach (var Income in Income_DTO)
                                        {
                                            SIISR_DTO.SR_SIH_INC_Number = Convert.ToInt64(Income.SRH_INC_SIH_INC_Number);
                                            SIISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(Income.SRH_INC_MIC_Number);
                                            SIISR_DTO.SR_INC_Remarks = Income.SRH_INC_Remarks;
                                            SIISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(Income.SRH_INC_OCRN_Number);
                                            SIISR_DTO.SR_INC_CM_Number = Convert.ToInt64(Income.SRH_INC_CM_Number);
                                            SIISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(Income.SRH_INC_IncomeBase);
                                            SIISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(Income.SRH_INC_IncomeValue);
                                            SIISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(Income.SRH_INC_ALCT_Number);
                                            SIISR_DTO.SR_INC_LA_Number = Convert.ToInt64(Income.SRH_INC_LA_Number);
                                            SIISR_DTO.SR_INC_CalculateGST = Convert.ToInt64(Income.SRH_INC_CalculateGST);
                                            SIISR_DTO.SR_INC_GST_Amount = Convert.ToDouble(Income.SRH_INC_GST_Amount);
                                            SIISR_DTO.SR_INC_SAC_Number = Convert.ToInt64(Income.SRH_INC_SAC_Number);
                                            SIISR_DTO.SR_INC_WHT_Percent = Convert.ToDouble(Income.SRH_INC_WHT_Percent);
                                            SIISR_DTO.SR_INC_WHT_Amount = Convert.ToDouble(Income.SRH_INC_WHT_Amount);
                                            SIISR_DTO.SR_Id = 23;
                                            SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                        }

                                        foreach (var BuyerAddress in BuyerAddress_DTO)
                                        {
                                            SIISR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_ADTP_Number);
                                            SIISR_DTO.SR_ADD_AddressID = Convert.ToString(BuyerAddress.SRH_ADD_AddressID);
                                            SIISR_DTO.SR_ADD_Address = Convert.ToString(BuyerAddress.SRH_ADD_Address);
                                            SIISR_DTO.SR_ADD_City = Convert.ToString(BuyerAddress.SRH_ADD_City);
                                            SIISR_DTO.SR_ADD_State = Convert.ToString(BuyerAddress.SRH_ADD_State);
                                            SIISR_DTO.SR_ADD_Country = Convert.ToString(BuyerAddress.SRH_ADD_Country);
                                            SIISR_DTO.SR_ADD_Pin = Convert.ToString(BuyerAddress.SRH_ADD_Pin);
                                            SIISR_DTO.SR_ADD_GSTIN = Convert.ToString(BuyerAddress.SRH_ADD_GSTIN);
                                            SIISR_DTO.SR_Id = 26;
                                            SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    if (SIHInvoiceNoOld != SIHInvoiceNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage = "Sale Rejection number " + SIHInvoiceNoOld + " used by another user. Next number will be allotted to you.";
                                    }
                                    return RedirectToAction("SIItemToCreateSaleRejection");
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }

                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;
                    }
                }
            }
            else if (Mode == "Direct")
            {
                SaleRejectionHead_DTO SH_DTO = new SaleRejectionHead_DTO();

                SH_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SH_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SH_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SH_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SH_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SH_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SH_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SH_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SH_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SH_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SH_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SH_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SH_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleRejection");
            }
            else if (Mode == "SITOITEM")
            {
                SIToSaleRejectionHead_DTO SHSI_DTO = new SIToSaleRejectionHead_DTO();

                SHSI_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SHSI_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SHSI_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SHSI_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SHSI_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SHSI_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SHSI_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SHSI_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SHSI_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SHSI_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SHSI_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SHSI_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SHSI_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SHSI_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSI_DTO);

                return RedirectToAction("SIToCreateSaleRejection");
            }
            else if (Mode == "SIITEMTO")
            {
                SIItemToSaleRejectionHead_DTO SHSI_DTO = new SIItemToSaleRejectionHead_DTO();

                SHSI_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SHSI_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SHSI_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SHSI_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SHSI_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SHSI_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SHSI_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SHSI_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SHSI_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SHSI_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SHSI_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SHSI_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SHSI_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SHSI_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSI_DTO);

                return RedirectToAction("SIItemToCreateSaleRejection");
            }

            SIItemToSaleRejectionGetData();
            return View(Original_DTO);
        }
        Boolean SIItemToBatchValidation(List<SIItemToSaleRejectionItem_DTO> Item_DTO, List<SIItemToSaleRejectionBatch_DTO>? ItemBatch_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                //Double BatchAmount = 0;

                Int64 SRINumber = Convert.ToInt64(Item.SRI_Number);
                Int64 SRIIndex = Convert.ToInt64(Item.SRI_Index);
                Int64 SRIItem = Convert.ToInt64(Item.SRI_ITM_Number);
                Double SRIQty = Math.Abs(Convert.ToDouble(Item.SRI_Qty));
                Double SRIAmount = Convert.ToDouble(Item.SRI_Amount);

                if (SRINumber != 0)
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_SRI_Number == SRINumber));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                    }
                }
                else
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == SRIItem) && (x.SRI_BCH_ITM_Index == SRIIndex));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                    }
                }


                if (BatchQty == SRIQty) { }
                else
                {
                    Message += Item.SRI_ITM_Code + " Batch Qty  Mismatched <br/>";
                    Result = false;
                }

                if (Message != "")
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Message;
                }
            }
            return Result;
        }

        void SIItemToSaleRejectionGetData()
        {
            SIISR_DTO.SR_RejectionDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            SIISR_DTO.SR_Id = 1;
            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

            ViewBag.IncomeCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[4]);
            ViewBag.UoM = Help.GetCat(DS.Tables[5]);
            ViewBag.HSN = Help.GetCat(DS.Tables[6]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[7]);
            ViewBag.WHTax = Help.GetCat(DS.Tables[8]);
            ViewBag.IsCalculate = Help.GetCat(DS.Tables[9]);
            ViewBag.LedgerAccount = Help.GetCat(DS.Tables[10]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[11]);
        }

        [Route("sale/transactions/sale-rejection/siitem/buyer")]
        public IActionResult SIItemToSaleBuyer(String? Buyer, String? Export, String? SRHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            if (Export == null)
            {
                Export = "";
            }
            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SIISR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(Export) == true ? 2 : 1);
            SIISR_DTO.SR_ITM_Code = Convert.ToString(Buyer).Trim();
            SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SIISR_DTO.SR_Id = 5;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
            var Ven = S_DL.BuyerList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("sale/transactions/sale-rejection/siitem/gst")]
        public String SIItemToSaleRejectionGst(String? Cluster, String? SRHDate, String? HSN, String? BaseAmount)
        {
            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SIISR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SIISR_DTO.SR_HSN_Number = Convert.ToInt64(HSN);
            SIISR_DTO.SR_Id = 6;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("sale/transactions/sale-rejection/siitem/gst/view")]
        public IActionResult SIItemToSaleRejectionGstView(String? Cluster, String? SRHDate, String? HSN, String? BaseAmount)
        {
            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SIISR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SIISR_DTO.SR_HSN_Number = Convert.ToInt64(HSN);
            SIISR_DTO.SR_Id = 9;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleRejectionGst> PurGST = new List<SaleRejectionGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new SaleRejectionGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("sale/transactions/sale-rejection/siitem/wht")]
        public IActionResult SIItemToSaleRejectionWHT(String? Buyer, String? WHTNumber, String? SRHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Buyer == null)
            {
                Buyer = "0";
            }
            if (SRHDate == null)
            {
                SRHDate = "0";
            }
            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SIISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SIISR_DTO.SR_WHT_Number = Convert.ToInt64(WHTNumber);
            SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SIISR_DTO.SR_Id = 7;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
            var WHT = S_DL.SaleInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("sale/transactions/sale-rejection/siitem/cluster")]
        public IActionResult SIItemToSaleRejectionCluster(String? Buyer, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            SIISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SIISR_DTO.SR_Search = Cluster;
            SIISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SIISR_DTO.SR_Id = 8;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
            var InvCluster = S_DL.SaleCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("sale/transactions/sale-rejection/siitem/invoice")]
        public IActionResult SIItemToSaleRejectionSI(String? Buyer, String? Export, String? MSNumber, String? SRHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }

            SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SIISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SIISR_DTO.SR_MS_Number = Convert.ToInt64(MSNumber);
            SIISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SIISR_DTO.SR_ExportOrder = Convert.ToInt32(Convert.ToBoolean(Export) == true ? 1 : 0);
            SIISR_DTO.SR_Id = 11;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
            var POInvoice = S_DL.SIItemToSRInvoice(DS.Tables[0]);
            return Json(POInvoice);
        }

        [Route("sale/transactions/sale-rejection/siitem/invoice/item")]
        public IActionResult SIItemToSaleRejectionSIItem(String? SINumber)
        {
            if (SINumber == null)
            {
                SINumber = "";
            }

            SIISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SIISR_DTO.SR_Search = SINumber;
            SIISR_DTO.SR_Id = 12;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

            SIItemToSR_DTO SIISI = new SIItemToSR_DTO();
            var SIItem = S_DL.SIItemToSRInvoiceItem(DS.Tables[0]);
            var SIIncome = S_DL.SIItemToSRIncome(DS.Tables[1]);
            var SIItemIncome = S_DL.SIItemToSRIIncome(DS.Tables[2]);

            SIISI.SIItems = SIItem;
            SIISI.SIIncomes = SIIncome;
            SIISI.SIItemIncomes = SIItemIncome;

            return Json(SIISI);
        }

        [Route("sale/transactions/sale-rejection/siitem/income/gst")]
        public String SIItemToSaleRejectionHeaderGst(String? Cluster, String? SRHDate, String? SAC, String? BaseAmount)
        {
            SIISR_DTO.SR_CreatorCode = Convert.ToInt32(UserCode);
            SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SIISR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SIISR_DTO.SR_INC_SAC_Number = Convert.ToInt64(SAC);
            SIISR_DTO.SR_Id = 13;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("sale/transactions/sale-rejection/siitem/income/gst/view")]
        public IActionResult SIItemToSaleRejectionGstHeaderView(String? Cluster, String? SRHDate, String? SAC, String? BaseAmount)
        {
            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(SRHDate).ToString("yyyyMMdd"));
            SIISR_DTO.SR_TCT_Number = Convert.ToInt64(Cluster);
            SIISR_DTO.SR_INC_SAC_Number = Convert.ToInt64(SAC);
            SIISR_DTO.SR_Id = 14;
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleRejectionGst> PurGST = new List<SaleRejectionGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new SaleRejectionGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("sale/transactions/sale-rejection/siitem/buyer/address")]
        public IActionResult SIItemToSaleBuyerAddressID(String? Buyer, String ADTPNumber)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SISR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SISR_DTO.SR_Id = 15;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            SaleRejectionAddress SRA = new SaleRejectionAddress();
            SRA.BuyerAddressId = S_DL.BuyerAddressID(DS.Tables[0]);
            SRA.BuyerAddress = S_DL.BuyerAddress(DS.Tables[1]).FirstOrDefault();
            return Json(SRA);
        }

        [Route("sale/transactions/sale-rejection/siitem/buyer/address/addressid")]
        public IActionResult SIItemToSaleBuyerAddress(String? Buyer, String ADTPNumber, String AddressID)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            SISR_DTO.SR_BUY_Number = Convert.ToInt64(Buyer);
            SISR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SISR_DTO.SR_ADD_AddressID = Convert.ToString(AddressID);
            SISR_DTO.SR_Id = 16;
            DS = SISR_DAO.SIToSaleRejectionDB(SISR_DTO);
            var Address = S_DL.BuyerAddress(DS.Tables[0]).FirstOrDefault();
            return Json(Address);
        }





        //Sale Rejection SI Item to Rejection Edit
        [Route("sale/transactions/sale-rejection/siitem/{SR_No}/edit")]
        public IActionResult SIItemToEditSaleRejection(Int64 SR_No)
        {
            SIItemToSaleRejectionGetData();
            String Active = GetSISRItemEditData(SR_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleRejectionRegisterSummary");
            }
            ViewBag.SR_No = SR_No;

            return View(SIISRH_DTO);
        }
        String GetSISRItemEditData(Int64 SR_No)
        {
            SIISR_DTO.SR_Number = SR_No;
            SIISR_DTO.SR_Id = 61;
            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SIISRH_DTO = S_DL.SIItemToSRHeadEditList(DS.Tables[0]).FirstOrDefault();
                SIISRH_DTO.RejectionItem = S_DL.SIItemToSRItemEditList(DS.Tables[1]);
                SIISRH_DTO.Income = S_DL.SIItemToSRIncomeEditList(DS.Tables[2]);
                SIISRH_DTO.ItemIncome = S_DL.SIItemToSRIIncomeEditList(DS.Tables[3]);
                SIISRH_DTO.ItemBatch = S_DL.SIItemToSRIBatchEditList(DS.Tables[4]);
                SIISRH_DTO.BuyerAddress = S_DL.SIItemToSRHAddressEditList(DS.Tables[5]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["SRH_Mode"].ToString();
                return "1";
            }
            else
            {
                return "0";
            }
        }

        [HttpPost]
        [Route("sale/transactions/sale-rejection/siitem/{SR_No}/edit")]
        public IActionResult SIItemToEditSaleRejection(SIItemToSaleRejectionHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SIItemToSaleRejectionHead_DTO S_Head_DTO = new SIItemToSaleRejectionHead_DTO();

            List<SIItemToSaleRejectionItem_DTO>? ITM_DTO = new List<SIItemToSaleRejectionItem_DTO>();
            List<SIItemToSaleRejectionIncome_DTO>? Income_DTO = new List<SIItemToSaleRejectionIncome_DTO>();
            List<SIItemToSaleRejectionIIncome_DTO>? ItemIncome_DTO = new List<SIItemToSaleRejectionIIncome_DTO>();
            List<SIItemToSaleRejectionBatch_DTO>? ItemBatch_DTO = new List<SIItemToSaleRejectionBatch_DTO>();
            List<SIItemToSaleRejectionAddress_DTO>? BuyerAddress_DTO = new List<SIItemToSaleRejectionAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.RejectionItem != null)
                ITM_DTO = S_DTO.RejectionItem!.Where(K => K.SRI_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SRH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SRI_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemBatch != null)
                ItemBatch_DTO = S_DTO.ItemBatch!.Where(K => K.SRI_BCH_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SRH_ADD_IsDeleted == 0).ToList();

            SIISR_DTO.SR_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SRI_MS_Number) != Convert.ToInt64(S_DTO.SRH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SRI_Qty) == 0 || Convert.ToDouble(x.SRI_UnitPrice) == 0 || Convert.ToDouble(x.SRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SRH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SRH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.RejectionItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.ItemBatch = ItemBatch_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (SIItemToBatchValidation(ITM_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    SIISR_DTO.SR_Number = Convert.ToInt64(S_DTO.SRH_Number);
                                    SIISR_DTO.SR_RejectionDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SRH_RejectionDate).ToString("yyyyMMdd"));
                                    SIISR_DTO.SR_RejectionNo = Convert.ToString(S_DTO.SRH_RejectionNo);
                                    SIISR_DTO.SR_BUY_Number = Convert.ToInt64(S_DTO.SRH_BUY_Number);
                                    SIISR_DTO.SR_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SRH_ExportOrder) ? 1 : 0);
                                    SIISR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SIISR_DTO.SR_MS_Number = Convert.ToInt64(S_DTO.SRH_MS_Number);
                                    SIISR_DTO.SR_TCT_Number = Convert.ToInt64(S_DTO.SRH_TCT_Number);
                                    SIISR_DTO.SR_CUR_Number = Convert.ToInt64(S_DTO.SRH_CUR_Number);
                                    SIISR_DTO.SR_WHT_Number = Convert.ToInt64(S_DTO.SRH_WHT_Number);
                                    SIISR_DTO.SR_ExchangeRate = Convert.ToDouble(S_DTO.SRH_ExchangeRate) == 0 ? "1" : S_DTO.SRH_ExchangeRate;
                                    SIISR_DTO.SR_MaterialCost = Convert.ToDouble(S_DTO.SRH_MaterialCost);
                                    SIISR_DTO.SR_ItemMiscIncome = Convert.ToDouble(S_DTO.SRH_ItemMiscIncome);
                                    SIISR_DTO.SR_HeaderMiscIncome = Convert.ToDouble(S_DTO.SRH_HeaderMiscIncome);
                                    SIISR_DTO.SR_GST_Amount = Convert.ToDouble(S_DTO.SRH_GST_Amount);
                                    SIISR_DTO.SR_RejectionAmount = Convert.ToDouble(S_DTO.SRH_RejectionAmount);
                                    SIISR_DTO.SR_WHT_Amount = Convert.ToDouble(S_DTO.SRH_WHT_Amount);
                                    SIISR_DTO.SR_RoundOff = Convert.ToDouble(S_DTO.SRH_RoundOff);
                                    SIISR_DTO.SR_BuyerReceivable = Convert.ToDouble(S_DTO.SRH_BuyerReceivable);
                                    SIISR_DTO.SR_Id = 121;
                                    DS = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                    String ItemDTO = string.Join(", ", ITM_DTO.Where(x => Convert.ToInt64(x.SRI_Number) != 0).Select(x => x.SRI_Number));
                                    String ItemIncomeDTO = string.Join(", ", ItemIncome_DTO.Where(x => Convert.ToInt64(x.SRI_INC_Number) != 0).Select(x => x.SRI_INC_Number));
                                    String IncomeDTO = string.Join(", ", Income_DTO.Where(x => Convert.ToInt64(x.SRH_INC_Number) != 0).Select(x => x.SRH_INC_Number));
                                    String BatchDTO = string.Join(", ", ItemBatch_DTO.Where(x => Convert.ToInt64(x.SRI_BCH_Number) != 0).Select(x => x.SRI_BCH_Number));
                                    String AddressDTO = string.Join(", ", BuyerAddress_DTO.Where(x => Convert.ToInt64(x.SRH_ADD_Number) != 0).Select(x => x.SRH_ADD_Number));

                                    SIISR_DTO.SR_DeleteNumbers = Convert.ToString(ItemDTO);
                                    SIISR_DTO.SR_Id = 101;
                                    SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                    SIISR_DTO.SR_DeleteNumbers = Convert.ToString(IncomeDTO);
                                    SIISR_DTO.SR_Id = 102;
                                    SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                    SIISR_DTO.SR_DeleteNumbers = Convert.ToString(ItemIncomeDTO);
                                    SIISR_DTO.SR_Id = 103;
                                    SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                    SIISR_DTO.SR_DeleteNumbers = Convert.ToString(BatchDTO);
                                    SIISR_DTO.SR_Id = 104;
                                    SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                    SIISR_DTO.SR_DeleteNumbers = Convert.ToString(AddressDTO);
                                    SIISR_DTO.SR_Id = 105;
                                    SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                    foreach (var Item in ITM_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        SIISR_DTO.SR_SIH_Number = Convert.ToInt64(Item.SRI_SIH_Number);
                                        SIISR_DTO.SR_SII_Number = Convert.ToInt64(Item.SRI_SII_Number);
                                        SIISR_DTO.SR_ITM_Number = Convert.ToInt64(Item.SRI_ITM_Number);
                                        SIISR_DTO.SR_WH_Number = Convert.ToInt64(Item.SRI_WH_Number);
                                        SIISR_DTO.SR_UoM_Number = Convert.ToInt64(Item.SRI_UoM_Number);
                                        SIISR_DTO.SR_Qty = Convert.ToDouble(Item.SRI_Qty);
                                        SIISR_DTO.SR_UnitPrice = Convert.ToDouble(Item.SRI_UnitPrice);
                                        SIISR_DTO.SR_Amount = Convert.ToDouble(Item.SRI_Amount);
                                        SIISR_DTO.SR_IncomeValue = Convert.ToDouble(Item.SRI_IncomeValue);
                                        SIISR_DTO.SR_HSN_Number = Convert.ToInt64(Item.SRI_HSN_Number);
                                        SIISR_DTO.SR_GST_Amount = Convert.ToDouble(Item.SRI_GST_Amount);
                                        SIISR_DTO.SR_WHT_Percent = Convert.ToDouble(Item.SRI_WHT_Percent);
                                        SIISR_DTO.SR_WHT_Amount = Convert.ToDouble(Item.SRI_WHT_Amount);
                                        if (Item.SRI_Number == 0)
                                        {
                                            SIISR_DTO.SR_Id = 22;
                                            D = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SRI_INC_SII_Number == Item.SRI_SII_Number && x.SRI_INC_SIH_Number == Item.SRI_SIH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SIISR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SIISR_DTO.SR_SII_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_SII_INC_Number);
                                                SIISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SIISR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SIISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SIISR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SIISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SIISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SIISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SIISR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                if (ItemInc.SRI_INC_Number == 0)
                                                {
                                                    SIISR_DTO.SR_Id = 24;
                                                }
                                                else
                                                {
                                                    SIISR_DTO.SR_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_Number);
                                                    SIISR_DTO.SR_Id = 124;
                                                }
                                                SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_ITM_Number == Item.SRI_ITM_Number) && (x.SRI_BCH_ITM_Index == Item.SRI_Index));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SIISR_DTO.SR_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SIISR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SIISR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SIISR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SIISR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SIISR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                if (ItemBatch.SRI_BCH_Number == 0)
                                                {
                                                    SIISR_DTO.SR_Id = 25;
                                                }
                                                else
                                                {
                                                    SIISR_DTO.SR_BCH_Number = Convert.ToInt64(ItemBatch.SRI_BCH_Number);
                                                    SIISR_DTO.SR_Id = 125;
                                                }
                                                SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                            }
                                        }
                                        else
                                        {
                                            SIISR_DTO.SR_I_Number = Convert.ToInt64(Item.SRI_Number);
                                            SIISR_DTO.SR_Id = 122;
                                            D = SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SRI_INC_SII_Number == Item.SRI_SII_Number && x.SRI_INC_SIH_Number == Item.SRI_SIH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SIISR_DTO.SR_I_Number = Convert.ToInt64(Item.SRI_Number);
                                                SIISR_DTO.SR_SII_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_SII_INC_Number);
                                                SIISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(ItemInc.SRI_INC_MIC_Number);
                                                SIISR_DTO.SR_INC_Remarks = ItemInc.SRI_INC_Remarks;
                                                SIISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(ItemInc.SRI_INC_OCRN_Number);
                                                SIISR_DTO.SR_INC_CM_Number = Convert.ToInt64(ItemInc.SRI_INC_CM_Number);
                                                SIISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(ItemInc.SRI_INC_IncomeBase);
                                                SIISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(ItemInc.SRI_INC_IncomeValue);
                                                SIISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(ItemInc.SRI_INC_ALCT_Number);
                                                SIISR_DTO.SR_INC_LA_Number = Convert.ToInt64(ItemInc.SRI_INC_LA_Number);
                                                if (ItemInc.SRI_INC_Number == 0)
                                                {
                                                    SIISR_DTO.SR_Id = 24;
                                                }
                                                else
                                                {
                                                    SIISR_DTO.SR_INC_Number = Convert.ToInt64(ItemInc.SRI_INC_Number);
                                                    SIISR_DTO.SR_Id = 124;
                                                }
                                                SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.SRI_BCH_SRI_Number == Item.SRI_Number));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                SIISR_DTO.SR_I_Number = Convert.ToInt64(Item.SRI_Number);
                                                SIISR_DTO.SR_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.SRI_BCH_Date).ToString("yyyyMMdd"));
                                                SIISR_DTO.SR_BCH_No = Convert.ToString(ItemBatch.SRI_BCH_No);
                                                SIISR_DTO.SR_BCH_Qty = Convert.ToDouble(ItemBatch.SRI_BCH_Qty);
                                                SIISR_DTO.SR_BCH_UnitPrice = Convert.ToDouble(ItemBatch.SRI_BCH_UnitPrice);
                                                SIISR_DTO.SR_BCH_Value = Convert.ToDouble(ItemBatch.SRI_BCH_Value);
                                                if (ItemBatch.SRI_BCH_Number == 0)
                                                {
                                                    SIISR_DTO.SR_Id = 25;
                                                }
                                                else
                                                {
                                                    SIISR_DTO.SR_BCH_Number = Convert.ToInt64(ItemBatch.SRI_BCH_Number);
                                                    SIISR_DTO.SR_Id = 125;
                                                }
                                                SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                            }
                                        }
                                    }
                                    foreach (var Income in Income_DTO)
                                    {
                                        SIISR_DTO.SR_SIH_INC_Number = Convert.ToInt64(Income.SRH_INC_SIH_INC_Number);
                                        SIISR_DTO.SR_INC_MIC_Number = Convert.ToInt64(Income.SRH_INC_MIC_Number);
                                        SIISR_DTO.SR_INC_Remarks = Income.SRH_INC_Remarks;
                                        SIISR_DTO.SR_INC_OCRN_Number = Convert.ToInt64(Income.SRH_INC_OCRN_Number);
                                        SIISR_DTO.SR_INC_CM_Number = Convert.ToInt64(Income.SRH_INC_CM_Number);
                                        SIISR_DTO.SR_INC_IncomeBase = Convert.ToDouble(Income.SRH_INC_IncomeBase);
                                        SIISR_DTO.SR_INC_IncomeValue = Convert.ToDouble(Income.SRH_INC_IncomeValue);
                                        SIISR_DTO.SR_INC_ALCT_Number = Convert.ToInt64(Income.SRH_INC_ALCT_Number);
                                        SIISR_DTO.SR_INC_LA_Number = Convert.ToInt64(Income.SRH_INC_LA_Number);
                                        SIISR_DTO.SR_INC_CalculateGST = Convert.ToInt64(Income.SRH_INC_CalculateGST);
                                        SIISR_DTO.SR_INC_GST_Amount = Convert.ToDouble(Income.SRH_INC_GST_Amount);
                                        SIISR_DTO.SR_INC_SAC_Number = Convert.ToInt64(Income.SRH_INC_SAC_Number);
                                        SIISR_DTO.SR_INC_WHT_Percent = Convert.ToDouble(Income.SRH_INC_WHT_Percent);
                                        SIISR_DTO.SR_INC_WHT_Amount = Convert.ToDouble(Income.SRH_INC_WHT_Amount);
                                        if (Income.SRH_INC_Number == 0)
                                        {
                                            SIISR_DTO.SR_Id = 23;
                                        }
                                        else
                                        {
                                            SIISR_DTO.SR_INC_Number = Convert.ToInt64(Income.SRH_INC_Number);
                                            SIISR_DTO.SR_Id = 123;
                                        }
                                        SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                    }
                                    foreach (var BuyerAddress in BuyerAddress_DTO)
                                    {
                                        SIISR_DTO.SR_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_ADTP_Number);
                                        SIISR_DTO.SR_ADD_AddressID = Convert.ToString(BuyerAddress.SRH_ADD_AddressID);
                                        SIISR_DTO.SR_ADD_Address = Convert.ToString(BuyerAddress.SRH_ADD_Address);
                                        SIISR_DTO.SR_ADD_City = Convert.ToString(BuyerAddress.SRH_ADD_City);
                                        SIISR_DTO.SR_ADD_State = Convert.ToString(BuyerAddress.SRH_ADD_State);
                                        SIISR_DTO.SR_ADD_Country = Convert.ToString(BuyerAddress.SRH_ADD_Country);
                                        SIISR_DTO.SR_ADD_Pin = Convert.ToString(BuyerAddress.SRH_ADD_Pin);
                                        SIISR_DTO.SR_ADD_GSTIN = Convert.ToString(BuyerAddress.SRH_ADD_GSTIN);
                                        if (BuyerAddress.SRH_ADD_Number == 0)
                                        {
                                            SIISR_DTO.SR_Id = 26;
                                        }
                                        else
                                        {
                                            SIISR_DTO.SR_INC_Number = Convert.ToInt64(BuyerAddress.SRH_ADD_Number);
                                            SIISR_DTO.SR_Id = 126;
                                        }
                                        SIISR_DAO.SIItemToSaleRejectionDB(SIISR_DTO);
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    return RedirectToAction("SaleRejectionRegisterSummary");
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;
                    }
                }
            }
            else if (Mode == "Direct")
            {
                SaleRejectionHead_DTO SH_DTO = new SaleRejectionHead_DTO();

                SH_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SH_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SH_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SH_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SH_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SH_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SH_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SH_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SH_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SH_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SH_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SH_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SH_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleRejection");
            }
            else if (Mode == "SOITEMTO")
            {
                SaleRejectionHead_DTO SH_DTO = new SaleRejectionHead_DTO();

                SH_DTO.SRH_RejectionNo = S_DTO.SRH_RejectionNo;
                SH_DTO.SRH_RejectionDate = S_DTO.SRH_RejectionDate;
                SH_DTO.SRH_BUY_Number = Convert.ToString(S_DTO.SRH_BUY_Number);
                SH_DTO.SRH_BUY_LOC_Number = Convert.ToString(S_DTO.SRH_BUY_LOC_Number);
                SH_DTO.SRH_ExportOrder = Convert.ToString(S_DTO.SRH_ExportOrder);
                SH_DTO.SRH_CUR_Number = Convert.ToString(S_DTO.SRH_CUR_Number);
                SH_DTO.SRH_MS_Number = Convert.ToString(S_DTO.SRH_MS_Number);
                SH_DTO.SRH_ExchangeRate = Convert.ToString(S_DTO.SRH_ExchangeRate);
                SH_DTO.SRH_TCT_Number = Convert.ToString(S_DTO.SRH_TCT_Number);
                SH_DTO.SRH_WHT_Number = Convert.ToString(S_DTO.SRH_WHT_Number);
                SH_DTO.SRH_WHT_Tax = Convert.ToString(S_DTO.SRH_WHT_Tax);
                SH_DTO.SRH_WHT_Percent = Convert.ToString(S_DTO.SRH_WHT_Percent);
                SH_DTO.SRH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SRH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("SOItemToCreateSaleRejection");
            }
            SIItemToSaleRejectionGetData();
            return View(Original_DTO);
        }




        //Sale Order Numbering
        [Route("sale/setup/sale-rejection-numbering")]
        public IActionResult SRNumbering()
        {
            GetSRNumber();
            return View(SRN_DTO);
        }

        [Route("sale/setup/sale-rejection-numbering")]
        [HttpPost]
        public IActionResult SRNumbering(SRNumber_DTO PN_DTO)
        {
            bool IsValid = false;
            SRNumber_DTO P_Head_DTO = new SRNumber_DTO();

            List<SRNumberReset_DTO>? Reset_DTO = new List<SRNumberReset_DTO>();
            List<SRNumberPrefix_DTO>? Prefix_DTO = new List<SRNumberPrefix_DTO>();
            List<SRNumberSuffix_DTO>? Suffix_DTO = new List<SRNumberSuffix_DTO>();

            P_Head_DTO = SRN_DTO;

            if (PN_DTO.SRNumberReset != null)
                Reset_DTO = PN_DTO.SRNumberReset!.Where(K => !K.SRR_IsDeleted).ToList();

            if (PN_DTO.SRNumberPrefix != null)
                Prefix_DTO = PN_DTO.SRNumberPrefix!.Where(K => !K.SRP_IsDeleted).ToList();

            if (PN_DTO.SRNumberSuffix != null)
                Suffix_DTO = PN_DTO.SRNumberSuffix!.Where(K => !K.SRS_IsDeleted).ToList();

            if (PN_DTO.SRN_Method == "2")
            {
                String ResetDTO = string.Join(", ", Reset_DTO.Where(x => Convert.ToInt64(x.SRR_Number) != 0).Select(x => x.SRR_Number));
                String PrefixDTO = string.Join(", ", Prefix_DTO.Where(x => Convert.ToInt64(x.SRP_Number) != 0).Select(x => x.SRP_Number));
                String SuffixDTO = string.Join(", ", Suffix_DTO.Where(x => Convert.ToInt64(x.SRS_Number) != 0).Select(x => x.SRS_Number));

                SRN_DTO.SRN_DeleteNumbers = Convert.ToString(ResetDTO);
                SRN_DTO.SRN_Id = 31;
                SRN_DAO.SRNumberDB(SRN_DTO);

                SRN_DTO.SRN_DeleteNumbers = Convert.ToString(PrefixDTO);
                SRN_DTO.SRN_Id = 32;
                SRN_DAO.SRNumberDB(SRN_DTO);

                SRN_DTO.SRN_DeleteNumbers = Convert.ToString(SuffixDTO);
                SRN_DTO.SRN_Id = 33;
                SRN_DAO.SRNumberDB(SRN_DTO);

                SRN_DTO.SRN_CreatorCode = Convert.ToInt32(UserCode);

                SRN_DTO.SRN_Method = PN_DTO.SRN_Method;
                if (PN_DTO.SRN_Number == 0)
                {
                    SRN_DTO.SRN_Id = 11;
                }
                else
                {
                    SRN_DTO.SRN_Id = 41;
                    SRN_DTO.SRN_Number = PN_DTO.SRN_Number;
                }
                SRN_DAO.SRNumberDB(SRN_DTO);

                foreach (var Reset in Reset_DTO)
                {
                    SRN_DTO.SRN_Date = Convert.ToString(Convert.ToDateTime(Reset.SRR_Date).ToString("yyyyMMdd"));
                    SRN_DTO.SRN_StartingNumber = Convert.ToInt32(Reset.SRR_StartingNumber).ToString();
                    SRN_DTO.SRN_NumberofDigits = Convert.ToInt32(Reset.SRR_NumberofDigits).ToString();
                    SRN_DTO.SRN_PrefilZero = Convert.ToInt64(Reset.SRR_PrefilZero).ToString();
                    SRN_DTO.SRN_Frequency = Convert.ToInt64(Reset.SRR_Frequency).ToString();
                    if (Reset.SRR_Number == 0)
                    {
                        SRN_DTO.SRN_Id = 12;
                    }
                    else
                    {
                        SRN_DTO.SRN_Id = 42;
                        SRN_DTO.SRN_Number = Reset.SRR_Number;
                    }
                    SRN_DAO.SRNumberDB(SRN_DTO);
                }

                foreach (var Prefix in Prefix_DTO)
                {
                    SRN_DTO.SRN_Date = Convert.ToString(Convert.ToDateTime(Prefix.SRP_Date).ToString("yyyyMMdd"));
                    SRN_DTO.SRN_Particulars = Convert.ToString(Prefix.SRP_Particulars);
                    if (Prefix.SRP_Number == 0)
                    {
                        SRN_DTO.SRN_Id = 13;
                    }
                    else
                    {
                        SRN_DTO.SRN_Id = 43;
                        SRN_DTO.SRN_Number = Prefix.SRP_Number;
                    }
                    SRN_DAO.SRNumberDB(SRN_DTO);
                }

                foreach (var Suffix in Suffix_DTO)
                {
                    SRN_DTO.SRN_Date = Convert.ToString(Convert.ToDateTime(Suffix.SRS_Date).ToString("yyyyMMdd"));
                    SRN_DTO.SRN_Particulars = Convert.ToString(Suffix.SRS_Particulars);
                    if (Suffix.SRS_Number == 0)
                    {
                        SRN_DTO.SRN_Id = 14;
                    }
                    else
                    {
                        SRN_DTO.SRN_Id = 44;
                        SRN_DTO.SRN_Number = Suffix.SRS_Number;
                    }
                    SRN_DAO.SRNumberDB(SRN_DTO);
                }
                SRN_DTO.Reset();
                Reset_DTO = null;
                Prefix_DTO = null;
                Suffix_DTO = null;
                ModelState.Clear();
            }
            else if (PN_DTO.SRN_Method == "3")
            {
                SRN_DTO.SRN_Method = PN_DTO.SRN_Method;
                if (PN_DTO.SRN_Number == 0)
                {
                    SRN_DTO.SRN_Id = 21;
                }
                else
                {
                    SRN_DTO.SRN_Id = 22;
                    SRN_DTO.SRN_Number = PN_DTO.SRN_Number;
                }
                SRN_DAO.SRNumberDB(SRN_DTO);
            }

            GetSRNumber();
            return View(SRN_DTO);
        }
        void GetSRNumber()
        {
            SRN_DTO.SRN_CreatorCode = Convert.ToInt32(UserCode);
            SRN_DTO.SRN_Id = 1;
            DS = SRN_DAO.SRNumberDB(SRN_DTO);

            ViewBag.Method = Help.GetCat(DS.Tables[0]);
            ViewBag.Frequency = Help.GetCat(DS.Tables[1]);
            ViewBag.Prefil = Help.GetCat(DS.Tables[2]);

            if (DS.Tables[3].Rows.Count > 0)
            {
                SRN_DTO.SRN_Number = Convert.ToInt64(DS.Tables[3].Rows[0]["SRN_Number"]);
                SRN_DTO.SRN_Method = DS.Tables[3].Rows[0]["SRN_Method"].ToString();
            }

            SRN_DTO.SRNumberReset = SRN_DL.SRRList(DS.Tables[4]);
            SRN_DTO.SRNumberPrefix = SRN_DL.SRPList(DS.Tables[5]);
            SRN_DTO.SRNumberSuffix = SRN_DL.SRSList(DS.Tables[6]);
        }
    }
}
