using ClosedXML.Excel;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DL;
using ERP_DTO;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Map;
using SelectPdf;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace ERP.Controllers.Procurement
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class PurchaseOrderController : Controller
    {
        CultureInfo India = new CultureInfo("hi-IN");
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        Procurement_DL P_DL = new Procurement_DL();

        //PO
        PurchaseOrderHead_DTO POH_DTO = new PurchaseOrderHead_DTO();
        PurchaseOrder_DAO PO_DAO = new PurchaseOrder_DAO();
        PurchaseOrder_DTO PO_DTO = new PurchaseOrder_DTO();
        List<PurchaseOrder_DTO> PO_List = new List<PurchaseOrder_DTO>();


        //PO NUMBERING
        PONumber_DTO PON_DTO = new PONumber_DTO();
        PONumber_DAO PON_DAO = new PONumber_DAO();
        Numbering_DL PON_DL = new Numbering_DL();
        List<PONumberReset_DTO> POR_List = new List<PONumberReset_DTO>();
        List<PONumberPrefix_DTO> POP_List = new List<PONumberPrefix_DTO>();
        List<PONumberSuffix_DTO> POS_List = new List<PONumberSuffix_DTO>();


        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        DataSet DS = new DataSet();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;






        //Purchase Order summary
        [Route("procurement/transactions/purchase-order-register-summary")]
        public IActionResult PurchaseOrderRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            PO_List = POSummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseOrder_DTO>.CreateAsync(PO_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("procurement/transactions/purchase-order-register-summary")]
        [HttpPost]
        public IActionResult PurchaseOrderRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? PO_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            if (Mode == "DeleteAll")
            {
                PO_DTO.DeleteNumbers = DeleteNumbers;
                PO_DTO.Id = 31;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            }
            else if (Mode == "Ascii" || Mode == "Excel" || Mode == "PDF")
            {
                List<Int64> AllowedPoNumbers = new List<Int64>();
                if (DeleteNumber.Length > 0)
                {
                    try
                    {
                        AllowedPoNumbers = DeleteNumber
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Select(Int64.Parse)
                            .ToList();
                    }
                    catch
                    {
                    }
                }
                HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

                PO_DTO.Id = 21;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                if (Mode == "Ascii")
                {
                    List<PurchaseOrderAscii> PO_List = P_DL.POAscii(DS.Tables[0]);

                    var Key = PO_List;
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;

                    var HeaderRow = typeof(PurchaseOrderAscii)
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(prop => prop.Name != nameof(PurchaseOrderAscii.POH_Number))
                            .Select(prop =>
                                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                             )
                            .ToList();

                    var AsciiData = new StringBuilder();
                    AsciiData.AppendLine(string.Join("\t", HeaderRow));



                    PropertyInfo[] PropertiesToInclude = typeof(PurchaseOrderAscii)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.Name != nameof(PurchaseOrderAscii.POH_Number))
                        .ToArray();

                    foreach (var item in Key)
                    {
                        var rowValues = PropertiesToInclude
                            .Select(prop => prop.GetValue(item)?.ToString() ?? "")
                            .ToList();

                        var escapedRowValues = rowValues
                            .Select(val => val.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))
                            .ToList();

                        AsciiData.AppendLine(string.Join("\t", escapedRowValues));

                        if (Decimal.TryParse(item.TotalQty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                        {
                            TotalQtySum += QtyValue;
                        }
                        if (Decimal.TryParse(item.MaterialValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                        {
                            TotalMaterialValueSum += MaterialValue;
                        }
                        if (Decimal.TryParse(item.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                        {
                            TotalItemMiscExpenseSum += ItemMiscValue;
                        }
                        if (Decimal.TryParse(item.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                        {
                            TotalHeadMiscExpenseSum += HeadMiscValue;
                        }
                    }

                    List<String> FooterCells = new List<String>();
                    Boolean TotalLabelAdded = false;
                    foreach (var prop in PropertiesToInclude)
                    {
                        string FooterCellValue = "";

                        if (!TotalLabelAdded && prop.Name != nameof(PurchaseOrderAscii.TotalQty) &&
                                                 prop.Name != nameof(PurchaseOrderAscii.MaterialValue) &&
                                                 prop.Name != nameof(PurchaseOrderAscii.ItemMiscExpense) &&
                                                 prop.Name != nameof(PurchaseOrderAscii.HeadMiscExpense))
                        {
                            if (FooterCells.Count == 0)
                            {
                                FooterCellValue = "Total:";
                                TotalLabelAdded = true;
                            }
                        }


                        switch (prop.Name)
                        {
                            case nameof(PurchaseOrderAscii.TotalQty):
                                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseOrderAscii.MaterialValue):
                                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseOrderAscii.ItemMiscExpense):
                                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseOrderAscii.HeadMiscExpense):
                                FooterCellValue = TotalHeadMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                        }

                        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                    }

                    AsciiData.AppendLine(string.Join("\t", FooterCells));

                    String FileName = "PO-download";
                    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                    var contentType = "text/plain";
                    var fileDownloadName = $"{FileName}.txt";
                    return File(fileBytes, contentType, fileDownloadName);

                }
                else if (Mode == "Excel")
                {
                    PO_List = P_DL.POList(DS.Tables[0]);

                    var Key = PO_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String FileName = "PO-download";
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add(SummaryDownload(Key.ToList()), "Sheet1");

                        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Style.Font.Bold = true;
                        ws.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);

                            var content = stream.ToArray();
                            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var fileDownloadName = $"{FileName}.xlsx";

                            return File(content, contentType, fileDownloadName);
                        }
                    }
                }
                else if (Mode == "PDF")
                {
                    PO_List = P_DL.POList(DS.Tables[0]);

                    var Key = PO_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String PDFDownload = $@"<html>
                        <head></head>
                        <body>
                            <style>
                                html,body{{
                                    font-family: ""Noto Sans"", sans-serif;
                                    width: 100%;
                                    margin: 0;
                                    background-color: #fff;
                                }}

                                .table {{
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-size: 0.6rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                    .table th {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    background-color: #000;
                                        color:#fff;
                                    }}

                                    .table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    }}

                                    .table th {{
                                        font-weight: bold;
                                    }}

                                    .table tr td {{
                                        height: 25px;
                                    }}
                            </style>";

                    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Import Order</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Material Value</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th></tr>";

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;

                    if (Key.ToList().Count > 0)
                    {
                        foreach (var Row in Key.ToList())
                        {
                            String Import = "";
                            if (Row.POH_ImportOrder == 1)
                            {
                                Import = "Yes";
                            }
                            else
                            {
                                Import = "No";
                            }

                            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.MaterialValue));
                            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscExpense));
                            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscExpense));
                            PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.POH_Date).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.POH_OrderNo}</td>
                                <td>{Row.POH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.POH_Currency_Number}</td>
                                <td style='width:30px;text-align:center'>{Import}</td>
                                <td>{Row.POH_MS_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.TotalItem}</td>
                                <td style='width:30px;text-align:center'>{Row.TotalQty}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                </tr>";


                            if (Decimal.TryParse(Row.TotalQty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                            {
                                TotalQtySum += QtyValue;
                            }
                            if (Decimal.TryParse(Row.MaterialValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                            {
                                TotalMaterialValueSum += MaterialValue;
                            }
                            if (Decimal.TryParse(Row.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                            {
                                TotalItemMiscExpenseSum += ItemMiscValue;
                            }
                            if (Decimal.TryParse(Row.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                            {
                                TotalHeadMiscExpenseSum += HeadMiscValue;
                            }
                        }
                    }

                    PDFDownload += $@"<tr>
                                <td>Total</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                </tr>";


                    PDFDownload += $@"</table></body></html>";

                    HtmlToPdf converter = new HtmlToPdf();
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                    converter.Options.MarginLeft = 10;
                    converter.Options.MarginRight = 10;
                    converter.Options.MarginTop = 10;
                    converter.Options.MarginBottom = 10;

                    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                    MemoryStream memoryStream = new MemoryStream();
                    doc.Save(memoryStream);
                    doc.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "PO_Download.pdf");
                }
            }
            else if (Mode == "View")
            {
                return RedirectToAction("PurchaseOrderPreview", new { PO_No = PO_No });
            }
            else if (Mode == "Edit")
            {
                return RedirectToAction("PurchaseOrderEdit", new { PO_No = PO_No });
            }

            PO_List = POSummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseOrder_DTO>.CreateAsync(PO_List, DPageNumber ?? 1, DPageSize));
        }
        DataTable SummaryDownload(List<PurchaseOrder_DTO> PO_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "purchase-order-summary";

            TableDown.Clear();
            TableDown.Columns.Add("Date");
            TableDown.Columns.Add("Order Number");
            TableDown.Columns.Add("Vendor Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Import Order");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("No. of Item");
            TableDown.Columns.Add("Qty");
            TableDown.Columns.Add("Material Value");
            TableDown.Columns.Add("Item Misc Expense Value");
            TableDown.Columns.Add("Head Misc Expense Value");


            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;

            foreach (var Product in PO_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["Date"] = Product.POH_Date;
                NewRow["Order Number"] = Product.POH_OrderNo;
                NewRow["Vendor Name"] = Product.POH_Vendor_Number;
                NewRow["Currency"] = Product.POH_Currency_Number;
                NewRow["Import Order"] = (Product.POH_ImportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Material Segregation"] = Product.POH_MS_Number;
                NewRow["No. of Item"] = Product.TotalItem;
                NewRow["Qty"] = Product.TotalQty;
                NewRow["Material Value"] = Product.MaterialValue;
                NewRow["Item Misc Expense Value"] = Product.ItemMiscExpense;
                NewRow["Head Misc Expense Value"] = Product.HeadMiscExpense;

                TableDown.Rows.Add(NewRow);


                if (Decimal.TryParse(Product.TotalQty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.MaterialValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscExpenseSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                {
                    TotalHeadMiscExpenseSum += HeadMiscValue;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["Date"] = "Total";
            NewRows["Order Number"] = "";
            NewRows["Vendor Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Import Order"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["No. of Item"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Material Value"] = TotalMaterialValueSum;
            NewRows["Item Misc Expense Value"] = TotalItemMiscExpenseSum;
            NewRows["Head Misc Expense Value"] = TotalHeadMiscExpenseSum;
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }
        List<PurchaseOrder_DTO> POSummaryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            PO_DTO.Id = 21;
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            PO_List = P_DL.POList(DS.Tables[0]);

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

            ViewData["CurrentSort"] = SortOrder;
            ViewData["KeySort"] = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["CurrentFilter"] = Search;

            var Key = PO_List.OrderByDescending(Cs => Cs.POH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.POH_Date.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.POH_OrderNo.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.POH_Vendor_Number.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.POH_Currency_Number.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.POH_MS_Number.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.TotalItem.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.TotalQty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.POH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.POH_Date)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.POH_Date)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.POH_Number);
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

            ViewBag.TotalItem = Key.Sum(item => Convert.ToDouble(item.TotalItem));
            ViewBag.TotalQty = Key.Sum(item => Convert.ToDouble(item.TotalQty));
            ViewBag.TotalMaterial = Key.Sum(item => Convert.ToDouble(item.POH_MaterialValue));
            ViewBag.TotalItemMisc = Key.Sum(item => Convert.ToDouble(item.POH_ItemMiscExpense));
            ViewBag.TotalHeadMisc = Key.Sum(item => Convert.ToDouble(item.POH_HeadMiscExpense));
            ViewBag.TotalOrderValue = Key.Sum(item => Convert.ToDouble(item.POH_OrderValue));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }

        [Route("procurement/transactions/purchase-order-register-summary/print")]
        public String POSummaryPrint(String Search, String SelectedItem, bool AllItem)
        {
            List<Int64> AllowedPoNumbers = new List<Int64>();
            if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                try
                {
                    AllowedPoNumbers = SelectedItem.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Select(Int64.Parse)
                        .ToList();
                }
                catch
                {
                }
            }
            HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

            if (Search == null)
            {
                Search = "";
            }
            PO_DTO.Id = 21;
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            PO_List = P_DL.POList(DS.Tables[0]);

            var Key = PO_List.OrderByDescending(Cs => Cs.POH_Number);

            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                     K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                     K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                     K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                     K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                     K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                     K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                     K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                     K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                     K.HeadMiscExpense!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.POH_Number);
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POH_Number)).OrderByDescending(K => K.POH_Number);
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                     K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                     K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                     K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                     K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                     K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                     K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                     K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                     K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                     K.HeadMiscExpense!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.POH_Number);
                }
            }

            String PDFDownload = $@"<html>
                        <head></head>
                        <body>
                            <style>
                                html,body{{
                                    font-family: ""Noto Sans"", sans-serif;
                                    width: 100%;
                                    margin: 0;
                                    background-color: #fff;
                                }}

                                .table {{
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-size: 0.6rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                    .table th {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    background-color: #000;
                                        color:#fff;
                                    }}

                                    .table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    }}

                                    .table th {{
                                        font-weight: bold;
                                    }}

                                    .table tr td {{
                                        height: 25px;
                                    }}
                            </style>";

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Import Order</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Material Value</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th></tr>";

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;

            if (Key.ToList().Count > 0)
            {
                foreach (var Row in Key.ToList())
                {
                    String Import = "";
                    if (Row.POH_ImportOrder == 1)
                    {
                        Import = "Yes";
                    }
                    else
                    {
                        Import = "No";
                    }

                    String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.MaterialValue));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscExpense));
                    String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscExpense));
                    PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.POH_Date).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.POH_OrderNo}</td>
                                <td>{Row.POH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.POH_Currency_Number}</td>
                                <td style='width:30px;text-align:center'>{Import}</td>
                                <td>{Row.POH_MS_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.TotalItem}</td>
                                <td style='width:30px;text-align:center'>{Row.TotalQty}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                </tr>";


                    if (Decimal.TryParse(Row.TotalQty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.MaterialValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalMaterialValueSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscExpenseSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                    {
                        TotalHeadMiscExpenseSum += HeadMiscValue;
                    }
                }
            }


            PDFDownload += $@"<tr>
                                <td>Total</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                </tr>";

            PDFDownload += $@"</table></body></html>";

            return PDFDownload;
        }






        //Purchase Preview
        [Route("procurement/transactions/purchase-order/{PO_No}/view")]
        public IActionResult PurchaseOrderPreview(String? PO_No)
        {
            String Active = GetPOPreview(PO_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseOrderRegisterSummary");
            }
            ViewBag.PO_No = PO_No;

            PurchaseGetData();
            GetPOEditData(PO_No);
            return View(POH_DTO);
        }

        [Route("procurement/transactions/purchase-order/{PO_No}/view")]
        [HttpPost]
        public IActionResult PurchaseOrderPreview(String Mode, String? PO_No)
        {
            if (Mode == "PDF")
            {
                PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                PO_DTO.Id = 41;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                String Total = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[0].Rows[0]["TotalQty"]));

                String Item = "";
                if (DS.Tables[1].Rows.Count > 0)
                {
                    for (Int32 i = 0; i < DS.Tables[1].Rows.Count; i++)
                    {
                        String UnitPrice = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["POI_UnitPrice"]));
                        String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["POI_Amount"]));
                        Item += $@"<tr>
                                <td colspan='2'>{DS.Tables[1].Rows[i]["POI_ItemCode"]}</td>
                                <td colspan='3'>{DS.Tables[1].Rows[i]["POI_Description"]}</td>
                                <td class='align-center'>{DS.Tables[1].Rows[i]["POI_UoM"]}</td>
                                <td colspan='2' class='align-center'>{DS.Tables[1].Rows[i]["POI_Qty"]}</td>
                                <td colspan='2' class='align-right'>{UnitPrice}</td>
                                <td colspan='2' class='align-right'>{Amount}</td>
                                </tr>";
                    }

                    Int32 Count = 10 - Convert.ToInt32(DS.Tables[1].Rows.Count);
                    if (Count > 0)
                    {
                        for (Int32 i = 0; i < Count; i++)
                        {
                            Item += $@"<tr>
                                    <td colspan='2'></td>
                                    <td colspan='3'></td>
                                    <td class='align-center'></td>
                                    <td colspan='2' class='align-center'></td>
                                    <td colspan='2' class='align-right'></td>
                                    <td colspan='2' class='align-right'></td>
                                    </tr>";
                        }
                    }
                }

                String PDFDownload = $@"<html>
                        <head>
                            <title>Part Label</title>
                        </head>
                        <body>
                            <style>
                                html,body{{
                                    font-size: 1rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                .po-container {{
                                    max-width: 100%;
                                    margin: auto;
                                    background-color: #fff;
                                    padding: 20px;
                                    height: 100%;
                                    font-size: 1rem;
                                }}

                                .po-table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}

                                    .po-table th, .po-table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                        line-height: 1.4;
                                    }}

                                    .po-table th {{
                                        font-weight: bold;
                                    }}

                                    .po-table tr td {{
                                        height: 29px;
                                    }}

                                .header-main {{
                                    background-color: #1E8449;
                                    color: white;
                                    font-weight: bold;
                                    text-align: center;
                                }}

                                .header-section {{
                                    background-color: #2ECC71;
                                    color: white;
                                    font-weight: bold;
                                    text-align: center;
                                }}

                                .header-sub {{
                                    background-color: #ABEBC6;
                                    color: #333;
                                    font-weight: bold;
                                }}

                                .align-right {{
                                    text-align: right;
                                }}

                                .align-center {{
                                    text-align: center;
                                }}

                                .company-title {{
                                    font-size: 1.4rem;
                                    font-weight: bold;
                                    text-align: center;
                                    vertical-align: middle;
                                }}

                                .po-title {{
                                    font-size: 1.2rem;
                                    font-weight: bold;
                                    text-align: center;
                                    vertical-align: middle;
                                }}

                                .placeholder {{
                                    color: #777;
                                    font-style: italic;
                                }}

                                .total-row td {{
                                    background-color: #2ECC71;
                                    color: white;
                                    font-weight: bold;
                                }}

                                .no-border {{
                                    border: none !important;
                                }}

                                .signature-area td {{
                                    border: none;
                                    padding-top: 15px;
                                    padding-bottom: 5px;
                                }}

                                a.details-link {{
                                    color: #0000EE;
                                    text-decoration: underline;
                                }}
                            </style>
                            <table class='po-table' style='background-color: #fff;color:#000'>
                                <tbody>
                                    <tr>
                                        <td colspan='2' style='vertical-align: middle; text-align: center;'>[LOGO]</td>
                                        <td colspan='10' class='company-title'>ABC PRIVATE LIMITED</td>
                                    </tr>
                                    <tr class='header-main'>
                                        <td colspan='4' class='po-title'>PURCHASE ORDER</td>
                                        <td class='align-right' style='vertical-align: middle;'>PO #</td>
                                        <td colspan='3' style='vertical-align: middle;'>{DS.Tables[0].Rows[0]["POH_OrderNo"]}</td>
                                        <td></td>
                                        <td class='align-right' style='vertical-align: middle;'>Date</td>
                                        <td colspan='2' style='vertical-align: middle;'>{DS.Tables[0].Rows[0]["POH_Date"]}</td>
                                    </tr>
                                    <tr class='header-section'>
                                        <td colspan='6'>VENDOR</td>
                                        <td colspan='6'>SHIP TO</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["VendorName"]}</td>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["VendorName"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["Address"]}</td>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["Address"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["City"]}, {DS.Tables[2].Rows[0]["State"]}, {DS.Tables[2].Rows[0]["Pincode"]}</td>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["City"]}, {DS.Tables[2].Rows[0]["State"]}, {DS.Tables[2].Rows[0]["Pincode"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>GST: {DS.Tables[2].Rows[0]["GSTIN"]}</td>
                                        <td colspan='6'>GST: {DS.Tables[2].Rows[0]["GSTIN"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>PAN: {DS.Tables[2].Rows[0]["PAN"]}</td>
                                        <td colspan='6'>PAN: {DS.Tables[2].Rows[0]["PAN"]}</td>
                                    </tr>
                                    <tr class='header-section'>
                                        <th>Currency</th>
                                        <th colspan='3'>Payment Terms</th>
                                        <th colspan='2'>Method of payment</th>
                                        <th colspan='4'>Delivery terms</th>
                                        <th colspan='2'>Mode of delivery</th>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["POH_PaymentTerms"]}</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["POH_PaymentTerms"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["POH_MOP"]}</td>
                                        <td colspan='4'>{DS.Tables[0].Rows[0]["POH_DeliveryTerms"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["POH_MOD"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'><a href='#' class='details-link'>Inspection Details</a></td>
                                        <td colspan='6'><a href='#' class='details-link'>Technical delivery conditions</a></td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>{DS.Tables[0].Rows[0]["POH_Inspection"]}</td>
                                        <td colspan='6'>{DS.Tables[0].Rows[0]["POH_TDC"]}</td>
                                    </tr>
                                    <tr class='header-section'>
                                        <th colspan='2'>Item Code</th>
                                        <th colspan='3'>Description</th>
                                        <th class='align-center'>Uom</th>
                                        <th colspan='2' class='align-center'>Qty</th>
                                        <th colspan='2' class='align-right'>Unit Price</th>
                                        <th colspan='2' class='align-right'>Value</th>
                                    </tr>";
                PDFDownload += Item;
                PDFDownload += $@"<tr class='total-row'>
                                        <td colspan='6' class='align-right'>Total</td>
                                        <td colspan='2' class='align-center'>{DS.Tables[0].Rows[0]["TotalQty"]}</td>
                                        <td colspan='2'></td>
                                    <td colspan='2' class='align-right'>{Total}</td>
                                    </tr>
                                    <tr>
                                        <td class='no-border' style='font-weight: bold;'>Tax</td>
                                        <td colspan='5' class='no-border'></td>
                                        <td colspan='6' class='no-border'></td>
                                    </tr>
                                    <tr>
                                        <td class='no-border' style='font-weight: bold;'>Remarks</td>
                                        <td colspan='11' class='no-border'></td>
                                    </tr>
                                    <tr class='signature-area'> <td colspan='12'> </td> </tr>
                                    <tr class='signature-area'> <td colspan='12'> </td> </tr>
                                    <tr class='signature-area'>
                                        <td colspan='8'></td>
                                        <td colspan='4' class='align-center'>for ABC Private Limited</td>
                                    </tr>
                                    <tr class='signature-area'> <td colspan='12'> </td> </tr>
                                    <tr class='signature-area'>
                                        <td colspan='8'></td>
                                        <td colspan='4' class='align-center' style='border-top: 1px solid #000; padding-top: 5px;'>Authorized Signatory</td>
                                    </tr>
                                </tbody>
                            </table>
                        </body>
                    </html>";

                HtmlToPdf converter = new HtmlToPdf();
                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                //converter.Options.PdfPageCustomSize = new System.Drawing.SizeF(420,160);
                converter.Options.MarginLeft = 0;
                converter.Options.MarginRight = 0;
                converter.Options.MarginTop = 0;
                converter.Options.MarginBottom = 0;

                PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                MemoryStream memoryStream = new MemoryStream();
                doc.Save(memoryStream);
                doc.Close();

                ModelState.Clear();
                return File(memoryStream.ToArray(), "application/pdf", "Print_Download.pdf");
            }
            else if (Mode == "Ascii")
            {
                PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                PO_DTO.Id = 41;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                String Total = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[0].Rows[0]["TotalQty"]));

                String Item = "";
                if (DS.Tables[1].Rows.Count > 0)
                {
                    for (Int32 i = 0; i < DS.Tables[1].Rows.Count; i++)
                    {
                        String UnitPrice = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["POI_UnitPrice"]));
                        String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["POI_Amount"]));
                        Item += $@"<tr>
                                <td>{DS.Tables[1].Rows[i]["POI_ItemCode"]}</td>
                                <td>{DS.Tables[1].Rows[i]["POI_Description"]}</td>
                                <td>{DS.Tables[1].Rows[i]["POI_UoM"]}</td>
                                <td>{DS.Tables[1].Rows[i]["POI_Qty"]}</td>
                                <td>{UnitPrice}</td>
                                <td>{Amount}</td>
                                </tr>";
                    }

                    Int32 Count = 10 - Convert.ToInt32(DS.Tables[1].Rows.Count);
                    if (Count > 0)
                    {
                        for (Int32 i = 0; i < Count; i++)
                        {
                            Item += $@"<tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    </tr>";
                        }
                    }
                }

                string htmlString = $@"
                            <table>
                                <tbody>
                                    <tr>
                                        <td>[LOGO]</td>
                                        <td>ABC PRIVATE LIMITED</td>
                                    </tr>
                                    <tr>
                                        <td class='po-title'>PURCHASE ORDER</td>
                                        <td>PO #</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_OrderNo"]}</td>
                                        <td></td>
                                        <td>Date</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_Date"]}</td>
                                    </tr>
                                    <tr>
                                        <td>VENDOR</td>
                                        <td>SHIP TO</td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[2].Rows[0]["VendorName"]}</td>
                                        <td>{DS.Tables[2].Rows[0]["VendorName"]}</td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[2].Rows[0]["Address"]}</td>
                                        <td>{DS.Tables[2].Rows[0]["Address"]}</td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[2].Rows[0]["City"]}, {DS.Tables[2].Rows[0]["State"]}, {DS.Tables[2].Rows[0]["Pincode"]}</td>
                                        <td>{DS.Tables[2].Rows[0]["City"]}, {DS.Tables[2].Rows[0]["State"]}, {DS.Tables[2].Rows[0]["Pincode"]}</td>
                                    </tr>
                                    <tr>
                                        <td>GST: {DS.Tables[2].Rows[0]["GSTIN"]}</td>
                                        <td>GST: {DS.Tables[2].Rows[0]["GSTIN"]}</td>
                                    </tr>
                                    <tr>
                                        <td>PAN: {DS.Tables[2].Rows[0]["PAN"]}</td>
                                        <td>PAN: {DS.Tables[2].Rows[0]["PAN"]}</td>
                                    </tr>
                                    <tr>
                                        <th>Currency</th>
                                        <th>Payment Terms</th>
                                        <th>Method of payment</th>
                                        <th>Delivery terms</th>
                                        <th>Mode of delivery</th>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["POH_PaymentTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_PaymentTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_MOP"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_DeliveryTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_MOD"]}</td>
                                    </tr>
                                    <tr>
                                        <td><a href='#'>Inspection Details</a></td>
                                        <td><a href='#'>Technical delivery conditions</a></td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["POH_Inspection"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_TDC"]}</td>
                                    </tr>
                                    <tr>
                                        <th>Item Code</th>
                                        <th>Description</th>
                                        <th>Uom</th>
                                        <th>Qty</th>
                                        <th>Unit Price</th>
                                        <th>Value</th>
                                    </tr>";
                htmlString += Item;
                htmlString += $@"<tr>
                                        <td>Total</td>
                                        <td>{DS.Tables[0].Rows[0]["TotalQty"]}</td>
                                        <td></td>
                                    <td>{Total}</td>
                                    </tr>
                                    <tr>
                                        <td>Tax</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>Remarks</td>
                                        <td></td>
                                    </tr>
                                    <tr> <td> </td> </tr>
                                    <tr> <td> </td> </tr>
                                    <tr>
                                        <td></td>
                                        <td>for ABC Private Limited</td>
                                    </tr>
                                    <tr> <td> </td> </tr>
                                    <tr>
                                        <td></td>
                                        <td>Authorized Signatory</td>
                                    </tr>
                                </tbody>
                            </table>";

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlString);

                var asciiBuilder = new StringBuilder();

                foreach (var row in doc.DocumentNode.SelectNodes("//tr"))
                {
                    foreach (var cell in row.SelectNodes("th|td"))
                    {
                        //var colspan = cell.GetAttributeValue("colspan", 1);
                        asciiBuilder.Append(cell.InnerText);
                        //for (int i = 1; i < colspan; i++)
                        //{
                        //    asciiBuilder.Append("\t");
                        //}
                        asciiBuilder.Append("\t");
                    }
                    asciiBuilder.AppendLine();
                }

                byte[] fileBytes = Encoding.UTF8.GetBytes(asciiBuilder.ToString());
                string contentType = "text/plain";
                string fileDownloadName = "product-download.txt";

                return File(fileBytes, contentType, fileDownloadName);
            }
            else if (Mode == "Excel")
            {
                PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                PO_DTO.Id = 41;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                String Total = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[0].Rows[0]["TotalQty"]));

                String Item = "";
                if (DS.Tables[1].Rows.Count > 0)
                {
                    for (Int32 i = 0; i < DS.Tables[1].Rows.Count; i++)
                    {
                        String UnitPrice = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["POI_UnitPrice"]));
                        String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["POI_Amount"]));
                        Item += $@"<tr>
                                <td>{DS.Tables[1].Rows[i]["POI_ItemCode"]}</td>
                                <td>{DS.Tables[1].Rows[i]["POI_Description"]}</td>
                                <td>{DS.Tables[1].Rows[i]["POI_UoM"]}</td>
                                <td>{DS.Tables[1].Rows[i]["POI_Qty"]}</td>
                                <td>{UnitPrice}</td>
                                <td>{Amount}</td>
                                </tr>";
                    }

                    Int32 Count = 10 - Convert.ToInt32(DS.Tables[1].Rows.Count);
                    if (Count > 0)
                    {
                        for (Int32 i = 0; i < Count; i++)
                        {
                            Item += $@"<tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    </tr>";
                        }
                    }
                }

                string htmlString = $@"
                            <table>
                                <tbody>
                                    <tr>
                                        <td>[LOGO]</td>
                                        <td>ABC PRIVATE LIMITED</td>
                                    </tr>
                                    <tr>
                                        <td class='po-title'>PURCHASE ORDER</td>
                                        <td>PO #</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_OrderNo"]}</td>
                                        <td></td>
                                        <td>Date</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_Date"]}</td>
                                    </tr>
                                    <tr>
                                        <td>VENDOR</td>
                                        <td>SHIP TO</td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[2].Rows[0]["VendorName"]}</td>
                                        <td>{DS.Tables[2].Rows[0]["VendorName"]}</td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[2].Rows[0]["Address"]}</td>
                                        <td>{DS.Tables[2].Rows[0]["Address"]}</td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[2].Rows[0]["City"]}, {DS.Tables[2].Rows[0]["State"]}, {DS.Tables[2].Rows[0]["Pincode"]}</td>
                                        <td>{DS.Tables[2].Rows[0]["City"]}, {DS.Tables[2].Rows[0]["State"]}, {DS.Tables[2].Rows[0]["Pincode"]}</td>
                                    </tr>
                                    <tr>
                                        <td>GST: {DS.Tables[2].Rows[0]["GSTIN"]}</td>
                                        <td>GST: {DS.Tables[2].Rows[0]["GSTIN"]}</td>
                                    </tr>
                                    <tr>
                                        <td>PAN: {DS.Tables[2].Rows[0]["PAN"]}</td>
                                        <td>PAN: {DS.Tables[2].Rows[0]["PAN"]}</td>
                                    </tr>
                                    <tr>
                                        <th>Currency</th>
                                        <th>Payment Terms</th>
                                        <th>Method of payment</th>
                                        <th>Delivery terms</th>
                                        <th>Mode of delivery</th>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["POH_PaymentTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_PaymentTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_MOP"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_DeliveryTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_MOD"]}</td>
                                    </tr>
                                    <tr>
                                        <td><a href='#'>Inspection Details</a></td>
                                        <td><a href='#'>Technical delivery conditions</a></td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["POH_Inspection"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["POH_TDC"]}</td>
                                    </tr>
                                    <tr>
                                        <th>Item Code</th>
                                        <th>Description</th>
                                        <th>Uom</th>
                                        <th>Qty</th>
                                        <th>Unit Price</th>
                                        <th>Value</th>
                                    </tr>";
                htmlString += Item;
                htmlString += $@"<tr>
                                        <td>Total</td>
                                        <td>{DS.Tables[0].Rows[0]["TotalQty"]}</td>
                                        <td></td>
                                    <td>{Total}</td>
                                    </tr>
                                    <tr>
                                        <td>Tax</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>Remarks</td>
                                        <td></td>
                                    </tr>
                                    <tr> <td> </td> </tr>
                                    <tr> <td> </td> </tr>
                                    <tr>
                                        <td></td>
                                        <td>for ABC Private Limited</td>
                                    </tr>
                                    <tr> <td> </td> </tr>
                                    <tr>
                                        <td></td>
                                        <td>Authorized Signatory</td>
                                    </tr>
                                </tbody>
                            </table>";

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlString);

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Sheet1");

                    var rows = doc.DocumentNode.SelectNodes("//tr");
                    if (rows != null)
                    {
                        int rowIndex = 1;
                        foreach (var row in rows)
                        {
                            int colIndex = 1;
                            foreach (var cell in row.SelectNodes("td|th"))
                            {
                                ws.Cell(rowIndex, colIndex).Value = cell.InnerText;
                                colIndex++;
                            }
                            rowIndex++;
                        }
                    }

                    ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Style.Font.Bold = true;
                    ws.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        var content = stream.ToArray();
                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileDownloadName = "purchase-order-download.xlsx";

                        return File(content, contentType, fileDownloadName);
                    }
                }

            }
            else if (Mode == "Delete")
            {
                PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                PO_DTO.Id = 32;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);
                return RedirectToAction("PurchaseOrderRegisterSummary");
            }
            String Active = GetPOPreview(PO_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseOrderRegisterSummary");
            }
            ViewBag.PO_No = PO_No;

            PurchaseGetData();
            GetPOEditData(PO_No);
            return View(POH_DTO);
        }
        String GetPOPreview(String PO_No)
        {
            PO_DTO.POH_Number = Convert.ToInt64(PO_No);
            PO_DTO.Id = 41;
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                ViewBag.HeadPreview = P_DL.POHeadList(DS.Tables[0]).FirstOrDefault();
                ViewBag.ItemPreview = P_DL.POItemList(DS.Tables[1]);
                ViewBag.VendorPreview = P_DL.VendorList(DS.Tables[2]).FirstOrDefault();
                return "1";
            }
            else
            {
                return "0";
            }
        }






        //Purchase Order Detailed
        [Route("procurement/transactions/purchase-order-register-detailed")]
        public IActionResult PurchaseOrderRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            PO_List = PODetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseOrder_DTO>.CreateAsync(PO_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("procurement/transactions/purchase-order-register-detailed")]
        [HttpPost]
        public IActionResult PurchaseOrderRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? PO_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            if (Mode == "Ascii" || Mode == "Excel" || Mode == "PDF")
            {
                List<Int64> AllowedPoNumbers = new List<Int64>();
                if (DeleteNumber.Length > 0)
                {
                    try
                    {
                        AllowedPoNumbers = DeleteNumber
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Select(Int64.Parse)
                            .ToList();
                    }
                    catch
                    {
                    }
                }
                HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

                PO_DTO.Id = 22;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                if (Mode == "Ascii")
                {
                    List<PurchaseOrderDetailAscii> PO_List = P_DL.PODetailAscii(DS.Tables[0]);

                    var Key = PO_List;
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UnitPrice!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UnitPrice!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;

                    var HeaderRow = typeof(PurchaseOrderDetailAscii)
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(prop => prop.Name != nameof(PurchaseOrderDetailAscii.POH_Number))
                            .Select(prop =>
                                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                             )
                            .ToList();

                    var AsciiData = new StringBuilder();
                    AsciiData.AppendLine(string.Join("\t", HeaderRow));


                    PropertyInfo[] PropertiesToInclude = typeof(PurchaseOrderDetailAscii)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.Name != nameof(PurchaseOrderDetailAscii.POH_Number))
                        .ToArray();

                    foreach (var item in Key)
                    {
                        var rowValues = PropertiesToInclude
                            .Select(prop => prop.GetValue(item)?.ToString() ?? "")
                            .ToList();

                        var escapedRowValues = rowValues
                            .Select(val => val.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))
                            .ToList();

                        AsciiData.AppendLine(string.Join("\t", escapedRowValues));

                        if (Decimal.TryParse(item.POI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                        {
                            TotalQtySum += QtyValue;
                        }
                        if (Decimal.TryParse(item.POI_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                        {
                            TotalMaterialValueSum += MaterialValue;
                        }
                        if (Decimal.TryParse(item.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                        {
                            TotalItemMiscExpenseSum += ItemMiscValue;
                        }
                        if (Decimal.TryParse(item.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                        {
                            TotalHeadMiscExpenseSum += HeadMiscValue;
                        }
                    }

                    List<String> FooterCells = new List<String>();
                    Boolean TotalLabelAdded = false;
                    foreach (var prop in PropertiesToInclude)
                    {
                        string FooterCellValue = "";

                        if (!TotalLabelAdded && prop.Name != nameof(PurchaseOrderDetailAscii.POI_Qty) &&
                                                 prop.Name != nameof(PurchaseOrderDetailAscii.POI_Amount) &&
                                                 prop.Name != nameof(PurchaseOrderDetailAscii.ItemMiscExpense) &&
                                                 prop.Name != nameof(PurchaseOrderDetailAscii.HeadMiscExpense))
                        {
                            if (FooterCells.Count == 0)
                            {
                                FooterCellValue = "Total:";
                                TotalLabelAdded = true;
                            }
                        }

                        switch (prop.Name)
                        {
                            case nameof(PurchaseOrderDetailAscii.POI_Qty):
                                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseOrderDetailAscii.POI_Amount):
                                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseOrderDetailAscii.ItemMiscExpense):
                                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseOrderDetailAscii.HeadMiscExpense):
                                FooterCellValue = TotalHeadMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                        }

                        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                    }

                    AsciiData.AppendLine(string.Join("\t", FooterCells));

                    String FileName = "PO-download";
                    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                    var contentType = "text/plain";
                    var fileDownloadName = $"{FileName}.txt";
                    return File(fileBytes, contentType, fileDownloadName);

                }
                else if (Mode == "Excel")
                {
                    PO_List = P_DL.PODetailList(DS.Tables[0]);

                    var Key = PO_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POI_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String FileName = "PO-download";
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add(PODetailsDownload(Key.ToList()), "Sheet1");

                        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Style.Font.Bold = true;
                        ws.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);

                            var content = stream.ToArray();
                            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var fileDownloadName = $"{FileName}.xlsx";

                            return File(content, contentType, fileDownloadName);
                        }
                    }
                }
                else if (Mode == "PDF")
                {
                    PO_List = P_DL.PODetailList(DS.Tables[0]);

                    var Key = PO_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POI_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String PDFDownload = $@"<html>
                        <head></head>
                        <body>
                            <style>
                                html,body{{
                                    font-family: ""Noto Sans"", sans-serif;
                                    width: 100%;
                                    margin: 0;
                                    background-color: #fff;
                                }}

                                .table {{
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-size: 0.5rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                    .table th {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    background-color: #000;
                                        color:#fff;
                                    }}

                                    .table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    }}

                                    .table th {{
                                        font-weight: bold;
                                    }}

                                    .table tr td {{
                                        height: 25px;
                                    }}
                            </style>";

                    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Import Order</th><th>Material Segregation</th><th>Item Group</th><th>Item Code</th><th>Item Description</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:30px;text-align:center'>Unit Price</th><th style='width:70px'>Material Value</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:70px'>Delivery Date</th></tr>";

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;

                    if (Key.ToList().Count > 0)
                    {
                        foreach (var Row in Key.ToList())
                        {
                            String Import = "";
                            if (Row.POH_ImportOrder == 1)
                            {
                                Import = "Yes";
                            }
                            else
                            {
                                Import = "No";
                            }

                            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.POI_Amount));
                            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscExpense));
                            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscExpense));
                            PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.POH_Date).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.POH_OrderNo}</td>
                                <td>{Row.POH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.POH_Currency_Number}</td>
                                <td style='width:30px;text-align:center'>{Import}</td>
                                <td>{Row.POH_MS_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_Item_Group}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_Item_Code}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_Item_Description}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_UoM}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_Qty}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_UnitPrice}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                <td style='width:70px;text-align:right'>{Row.POI_DeliveryDate}</td>
                                </tr>";


                            if (Decimal.TryParse(Row.POI_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                            {
                                TotalQtySum += QtyValue;
                            }
                            if (Decimal.TryParse(Row.POI_Amount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                            {
                                TotalMaterialValueSum += MaterialValue;
                            }
                            if (Decimal.TryParse(Row.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                            {
                                TotalItemMiscExpenseSum += ItemMiscValue;
                            }
                            if (Decimal.TryParse(Row.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                            {
                                TotalHeadMiscExpenseSum += HeadMiscValue;
                            }
                        }
                    }

                    PDFDownload += $@"<tr>
                                <td>Total</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td></td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                <td></td>
                                </tr>";


                    PDFDownload += $@"</table></body></html>";

                    HtmlToPdf converter = new HtmlToPdf();
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                    converter.Options.MarginLeft = 10;
                    converter.Options.MarginRight = 10;
                    converter.Options.MarginTop = 10;
                    converter.Options.MarginBottom = 10;

                    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                    MemoryStream memoryStream = new MemoryStream();
                    doc.Save(memoryStream);
                    doc.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "Print_Download.pdf");
                }
            }
            else if (Mode == "View")
            {
                PO_DTO.POH_Number = Convert.ToInt32(PO_No);
                PO_DTO.Id = 42;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PurchaseOrderPreview", new { PO_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }
            else if (Mode == "Edit")
            {
                PO_DTO.POH_Number = Convert.ToInt32(PO_No);
                PO_DTO.Id = 42;
                PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PurchaseOrderEdit", new { PO_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }

            PO_List = PODetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseOrder_DTO>.CreateAsync(PO_List, DPageNumber ?? 1, DPageSize));
        }
        List<PurchaseOrder_DTO> PODetailedGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            PO_DTO.Id = 22;
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            PO_List = P_DL.PODetailList(DS.Tables[0]);

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

            ViewData["CurrentSort"] = SortOrder;
            ViewData["KeySort"] = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["CurrentFilter"] = Search;

            var Key = PO_List.OrderByDescending(Cs => Cs.POH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.POH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.POH_Date)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.POH_Date)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.POH_Number);
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

            ViewBag.SumQty = Key.Sum(item => Convert.ToDouble(item.POI_Qty));
            ViewBag.SumMaterial = Key.Sum(item => Convert.ToDouble(item.POI_Amount));
            ViewBag.SumItemMisc = Key.Sum(item => Convert.ToDouble(item.POI_ExpenseValue));
            ViewBag.SumHeadMisc = Key.Sum(item => Convert.ToDouble(item.HeadMiscExpense));
            ViewBag.SumTotalValue = Key.Sum(item => Convert.ToDouble(item.POH_TotalValue));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        DataTable PODetailsDownload(List<PurchaseOrder_DTO> PO_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "purchase-order-detailed";

            TableDown.Clear();
            TableDown.Columns.Add("Date");
            TableDown.Columns.Add("Order Number");
            TableDown.Columns.Add("Vendor Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Import Order");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("Item Group");
            TableDown.Columns.Add("Item Code");
            TableDown.Columns.Add("Item Description");
            TableDown.Columns.Add("UoM");
            TableDown.Columns.Add("Qty");
            TableDown.Columns.Add("Unit Price");
            TableDown.Columns.Add("Material Value");
            TableDown.Columns.Add("Item Misc Expense Value");
            TableDown.Columns.Add("Head Misc Expense Value");
            TableDown.Columns.Add("Delivery Date");


            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;

            foreach (var Product in PO_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["Date"] = Product.POH_Date;
                NewRow["Order Number"] = Product.POH_OrderNo;
                NewRow["Vendor Name"] = Product.POH_Vendor_Number;
                NewRow["Currency"] = Product.POH_Currency_Number;
                NewRow["Import Order"] = (Product.POH_ImportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Material Segregation"] = Product.POH_MS_Number;
                NewRow["Item Group"] = Product.POI_Item_Group;
                NewRow["Item Code"] = Product.POI_Item_Code;
                NewRow["Item Description"] = Product.POI_Item_Description;
                NewRow["UoM"] = Product.POI_UoM;
                NewRow["Qty"] = Product.POI_Qty;
                NewRow["Unit Price"] = Product.POI_UnitPrice;
                NewRow["Material Value"] = Product.POI_Amount;
                NewRow["Item Misc Expense Value"] = Product.ItemMiscExpense;
                NewRow["Head Misc Expense Value"] = Product.HeadMiscExpense;
                NewRow["Delivery Date"] = Product.POI_DeliveryDate;

                TableDown.Rows.Add(NewRow);

                if (Decimal.TryParse(Product.POI_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.POI_Amount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscExpenseSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                {
                    TotalHeadMiscExpenseSum += HeadMiscValue;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["Date"] = "Total";
            NewRows["Order Number"] = "";
            NewRows["Vendor Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Import Order"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["Item Group"] = "";
            NewRows["Item Code"] = "";
            NewRows["Item Description"] = "";
            NewRows["UoM"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Unit Price"] = "";
            NewRows["Material Value"] = TotalMaterialValueSum;
            NewRows["Item Misc Expense Value"] = TotalItemMiscExpenseSum;
            NewRows["Head Misc Expense Value"] = TotalHeadMiscExpenseSum;
            NewRows["Delivery Date"] = "";
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }

        [Route("procurement/transactions/purchase-order-register-detailed/print")]
        public String PODetailedPrint(String Search, String SelectedItem, bool AllItem)
        {
            List<Int64> AllowedPoNumbers = new List<Int64>();
            if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                try
                {
                    AllowedPoNumbers = SelectedItem.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Select(Int64.Parse)
                        .ToList();
                }
                catch
                {
                }
            }
            HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

            if (Search == null)
            {
                Search = "";
            }
            PO_DTO.Id = 22;
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            PO_List = P_DL.PODetailList(DS.Tables[0]);

            var Key = PO_List.ToList();

            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POI_Number)).ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Item_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                }
            }

            String PDFDownload = $@"<html>
                        <head></head>
                        <body>
                            <style>
                                html,body{{
                                    font-family: ""Noto Sans"", sans-serif;
                                    width: 100%;
                                    margin: 0;
                                    background-color: #fff;
                                }}

                                .table {{
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-size: 0.5rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                    .table th {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    background-color: #000;
                                        color:#fff;
                                    }}

                                    .table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    }}

                                    .table th {{
                                        font-weight: bold;
                                    }}

                                    .table tr td {{
                                        height: 25px;
                                    }}
                            </style>";

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Import Order</th><th>Material Segregation</th><th>Item Group</th><th>Item Code</th><th>Item Description</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:30px;text-align:center'>Unit Price</th><th style='width:70px'>Material Value</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:70px'>Delivery Date</th></tr>";

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;

            if (Key.ToList().Count > 0)
            {
                foreach (var Row in Key.ToList())
                {
                    String Import = "";
                    if (Row.POH_ImportOrder == 1)
                    {
                        Import = "Yes";
                    }
                    else
                    {
                        Import = "No";
                    }

                    String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.POI_Amount));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscExpense));
                    String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscExpense));

                    PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.POH_Date).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.POH_OrderNo}</td>
                                <td>{Row.POH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.POH_Currency_Number}</td>
                                <td style='width:30px;text-align:center'>{Import}</td>
                                <td>{Row.POH_MS_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_Item_Code}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_Item_Group}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_Item_Description}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_UoM}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_Qty}</td>
                                <td style='width:30px;text-align:center'>{Row.POI_UnitPrice}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                <td style='width:70px;text-align:right'>{Row.POI_DeliveryDate}</td>
                                </tr>";


                    if (Decimal.TryParse(Row.POI_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.POI_Amount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalMaterialValueSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscExpenseSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                    {
                        TotalHeadMiscExpenseSum += HeadMiscValue;
                    }
                }
            }

            PDFDownload += $@"<tr>
                                <td>Total</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td></td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                <td></td>
                                </tr>";


            PDFDownload += $@"</table></body></html>";

            return PDFDownload;
        }






        //Purchase Order Create
        [Route("procurement/transactions/purchase-order/create")]
        public IActionResult PurchaseOrderCreate()
        {
            PurchaseOrderHead_DTO PH_DTO = new PurchaseOrderHead_DTO();
            PH_DTO.POH_Date = DateTime.Now.ToString("dd-MMM-yy");
            PH_DTO.POH_OrderNo = OnPurchaseNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            PurchaseGetData();
            return View(PH_DTO);
        }
        [HttpPost]
        [Route("procurement/transactions/purchase-order/create")]
        public IActionResult PurchaseOrderCreate(PurchaseOrderHead_DTO P_DTO, String? Mode)
        {
            var Original_PIH_DTO = Help.JsonClone(P_DTO);

            bool IsValid = false;
            PurchaseOrderHead_DTO P_Head_DTO = new PurchaseOrderHead_DTO();

            List<PurchaseOrderItem_DTO>? Item_DTO = new List<PurchaseOrderItem_DTO>();
            List<PurchaseOrderExpense_DTO>? Expense_DTO = new List<PurchaseOrderExpense_DTO>();
            List<PurchaseOrderIExpense_DTO>? ItemExpense_DTO = new List<PurchaseOrderIExpense_DTO>();

            P_Head_DTO = P_DTO;

            if (P_DTO.PurchaseItems != null)
                Item_DTO = P_DTO.PurchaseItems!.Where(K => K.IsDeleted == "false").ToList();

            if (P_DTO.Expenses != null)
                Expense_DTO = P_DTO.Expenses!.Where(K => K.IsDeleted == "false").ToList();

            if (P_DTO.ItemExpenses != null)
                ItemExpense_DTO = P_DTO.ItemExpenses!.Where(K => K.IsDeleted == "false").ToList();

            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.POI_MS_Number) != Convert.ToInt64(P_DTO.POH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.POI_Qty) == 0 || Convert.ToDouble(x.POI_UnitPrice) == 0 || Convert.ToDouble(x.POI_Amount) == 0);

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
                else if (Item_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(P_DTO.POH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(P_DTO.POH_ImportOrder) ? 2 : 1) != Convert.ToInt32(P_DTO.POH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import Order and Vendor not match";
                }
                else
                {
                    ModelState.Clear();
                    P_Head_DTO.PurchaseItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            try
                            {
                                String POHOrderNoOld = P_DTO.POH_OrderNo;
                                String POHOrderNoNew = OnPurchaseNumber(Convert.ToInt32(Convert.ToDateTime(P_DTO.POH_Date).ToString("yyyyMMdd")));

                                PO_DTO.POH_OrderNo = POHOrderNoNew;
                                PO_DTO.POH_Date = Convert.ToString(Convert.ToDateTime(P_DTO.POH_Date).ToString("yyyyMMdd"));
                                PO_DTO.POH_Vendor_Number = Convert.ToString(P_DTO.POH_Vendor_Number);
                                PO_DTO.POH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(P_DTO.POH_ImportOrder) ? 1 : 0);
                                PO_DTO.POH_Currency_Number = Convert.ToString(P_DTO.POH_Currency_Number);
                                PO_DTO.POH_MS_Number = Convert.ToString(P_DTO.POH_MS_Number);
                                PO_DTO.POH_PaymentTerms = P_DTO.POH_PaymentTerms;
                                PO_DTO.POH_MOP = P_DTO.POH_MOP;
                                PO_DTO.POH_DeliveryTerms = P_DTO.POH_DeliveryTerms;
                                PO_DTO.POH_MOD = P_DTO.POH_MOD;
                                PO_DTO.POH_Tax = P_DTO.POH_Tax;
                                PO_DTO.POH_Inspection = P_DTO.POH_Inspection;
                                PO_DTO.POH_TDC = P_DTO.POH_TDC;
                                PO_DTO.POH_ExchangeRate = Convert.ToDouble(P_DTO.POH_ExchangeRate) == 0 ? "1" : P_DTO.POH_ExchangeRate;
                                PO_DTO.POH_Remarks = P_DTO.POH_Remarks;
                                PO_DTO.POH_MaterialValue = Convert.ToDouble(P_DTO.POH_MaterialValue).ToString();
                                PO_DTO.POH_ItemMiscExpense = Convert.ToDouble(P_DTO.POH_ItemMiscExpense).ToString();
                                PO_DTO.POH_HeadMiscExpense = Convert.ToDouble(P_DTO.POH_HeadMiscExpense).ToString();
                                PO_DTO.POH_OrderValue = Convert.ToDouble(P_DTO.POH_OrderValue).ToString();
                                PO_DTO.Id = 11;
                                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                                OnPurchaseNumberGen(Convert.ToInt32(Convert.ToDateTime(P_DTO.POH_Date).ToString("yyyyMMdd")));

                                if (DS.Tables[0].Rows.Count > 0)
                                {
                                    PO_DTO.POH_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                    foreach (var Item in Item_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        PO_DTO.POI_Item_Number = Convert.ToInt64(Item.POI_Item_Number);
                                        PO_DTO.POI_UoM_Number = Convert.ToInt64(Item.POI_UoM_Number);
                                        PO_DTO.POI_Qty = Convert.ToDouble(Item.POI_Qty);
                                        PO_DTO.POI_UnitPrice = Convert.ToDouble(Item.POI_UnitPrice);
                                        PO_DTO.POI_Amount = Convert.ToDouble(Item.POI_Amount);
                                        PO_DTO.POI_ExpenseValue = Convert.ToDouble(Item.POI_ExpenseValue);
                                        PO_DTO.POI_DeliveryDate = Convert.ToDateTime(Item.POI_DeliveryDate).ToString("yyyyMMdd");
                                        PO_DTO.Id = 12;
                                        D = PO_DAO.PurchaseOrderDB(PO_DTO);

                                        var ItemExpense = ItemExpense_DTO.Where(x => (x.POI_EXP_Item_Number == Item.POI_Item_Number) && (x.POI_EXP_Item_Index == Item.POI_Index));

                                        foreach (var ItemExp in ItemExpense)
                                        {
                                            PO_DTO.POI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            PO_DTO.POI_Item_Number = Convert.ToInt64(ItemExp.POI_EXP_Item_Number);
                                            PO_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.POI_EXP_Expense_Number);
                                            PO_DTO.EXP_Remarks = ItemExp.POI_EXP_Remarks;
                                            PO_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.POI_EXP_Occurrence_Number);
                                            PO_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.POI_EXP_CM_Number);
                                            PO_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.POI_EXP_ExpenseBase);
                                            PO_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.POI_EXP_ExpenseValue);
                                            PO_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.POI_EXP_Allocate_Number);
                                            PO_DTO.EXP_LA_Number = Convert.ToInt64(ItemExp.POI_EXP_LA_Number);
                                            PO_DTO.Id = 14;
                                            PO_DAO.PurchaseOrderDB(PO_DTO);
                                        }
                                    }
                                    foreach (var Exp in Expense_DTO)
                                    {
                                        PO_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.POH_EXP_Expense_Number);
                                        PO_DTO.EXP_Remarks = Exp.POH_EXP_Remarks;
                                        PO_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.POH_EXP_Occurrence_Number);
                                        PO_DTO.EXP_CM_Number = Convert.ToInt64(Exp.POH_EXP_CM_Number);
                                        PO_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.POH_EXP_ExpenseBase);
                                        PO_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.POH_EXP_ExpenseValue);
                                        PO_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.POH_EXP_Allocate_Number);
                                        PO_DTO.EXP_LA_Number = Convert.ToInt64(Exp.POH_EXP_LA_Number);
                                        PO_DTO.Id = 13;
                                        PO_DAO.PurchaseOrderDB(PO_DTO);
                                    }
                                }

                                transaction.Complete();

                                P_Head_DTO.Reset();
                                Expense_DTO = null;
                                Item_DTO = null;
                                ItemExpense_DTO = null;
                                P_DTO.Reset();
                                Original_PIH_DTO = Help.JsonClone(P_DTO);

                                if (POHOrderNoOld != POHOrderNoNew)
                                {
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = "Purchase Order number " + POHOrderNoOld + " used by another user. Next number will be allotted to you.";
                                }
                                return RedirectToAction("PurchaseOrderCreate");
                            }
                            catch (Exception ex)
                            {
                                transaction.Dispose();
                                ViewBag.ErrorCode = 2;
                                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
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

            PurchaseGetData();
            return View(Original_PIH_DTO);
        }

        [Route("procurement/transactions/purchase-order/item")]
        public IActionResult PurchaseItem(String? ItemCode, String MS)
        {
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PO_DTO.POI_Item_Code = Convert.ToString(ItemCode).Trim();
            PO_DTO.POH_MS_Number = Convert.ToString(MS).Trim();
            PO_DTO.Id = 2;
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            var Item = P_DL.IList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("procurement/transactions/purchase-order/uom")]
        public String PurchaseUoM(String? UoM)
        {
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PO_DTO.POI_UoM_Number = Convert.ToInt32(UoM);
            PO_DTO.Id = 4;
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("procurement/transactions/purchase-order/expense/des")]
        public IActionResult PurchaseExpensiveDes(String? Title)
        {
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PO_DTO.EXP_Expense_Number = Convert.ToInt32(Title);
            PO_DTO.Id = 3;
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            var Expensive = P_DL.ExList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("procurement/transactions/purchase-order/vendor")]
        public IActionResult PurchaseVendor(String? Vendor, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (Import == null)
            {
                Import = "";
            }
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PO_DTO.POH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PO_DTO.POI_Item_Code = Convert.ToString(Vendor).Trim();
            PO_DTO.Id = 5;
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            var Ven = P_DL.VList(DS.Tables[0]);
            return Json(Ven);
        }

        void PurchaseGetData()
        {
            PO_DTO.POH_Date = Convert.ToString(DateTime.Now.ToString("yyyyMMdd"));
            PO_DTO.Id = 1;
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);

            ViewBag.ExpenseCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.Currency = Help.GetCat(DS.Tables[4]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[5]);
            ViewBag.Vendor = Help.GetCat(DS.Tables[6]);
            ViewBag.UoM = Help.GetCat(DS.Tables[7]);
            ViewBag.LedgerAccount = Help.GetCat(DS.Tables[8]);
        }
        void OnPurchaseNumberGen(Int32 PODate)
        {
            DataSet DS1 = new DataSet();
            PON_DTO.PON_Date = PODate.ToString();
            PON_DTO.Id = 101;
            DS1 = PON_DAO.PONumberDB(PON_DTO);
            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["PON_Method"].ToString());
                DateTime PO_Date = Convert.ToDateTime(DateTime.ParseExact(PODate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["StartingNumber"].ToString());

                        PON_DTO.PON_Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["Number"].ToString());
                        PON_DTO.PON_StartingNumber = Convert.ToString(Convert.ToInt32(Number + 1));
                        PON_DTO.Id = 103;
                        PON_DAO.PONumberDB(PON_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;
                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(DS1.Tables[2].Rows[0]["POR_Date"].ToString());
                            Start = Convert.ToInt32(DS1.Tables[2].Rows[0]["POR_StartingNumber"].ToString());
                            Frequency = Convert.ToInt32(DS1.Tables[2].Rows[0]["POR_Frequency"].ToString());
                        }

                        if (Frequency == 4)
                        {
                            if (Date.Month == PO_Date.Month)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PO_Date.Year, PO_Date.Month, 1);
                                EndDate = new DateTime(PO_Date.Year, PO_Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                        }
                        else if (Frequency == 5)
                        {
                            if (Date.Month == PO_Date.Month && Date.Year == PO_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else if (Date.Month != PO_Date.Month && Date.Year == PO_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PO_Date.Year, Date.Month, 1);
                                EndDate = new DateTime(PO_Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                        }

                        PON_DTO.PON_Number = Convert.ToInt32(DS1.Tables[2].Rows[0]["POR_Number"].ToString());
                        PON_DTO.PON_StartingNumber = Convert.ToString(Start);
                        PON_DTO.PON_Date = Convert.ToString(StartDate.ToString("yyyyMMdd"));
                        PON_DTO.PON_Method = Convert.ToString(EndDate.ToString("yyyyMMdd"));
                        PON_DTO.Id = 102;
                        PON_DAO.PONumberDB(PON_DTO);
                    }
                }
            }
        }


        [Route("procurement/transactions/purchase-order/numbering")]
        public String OnPurchaseNumber(Int32 PODate)
        {
            PO_DTO.POH_Date = Convert.ToString(PODate);
            PO_DTO.Id = 0;
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);

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
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["PON_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["POP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["POS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["POR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["POR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["POR_Frequency"].ToString());

                            //if (RFre == 4)
                            //{
                            Number = OrNum + 1;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                            //}
                            //else if (RFre == 5)
                            //{
                            //    Number = OrNum + 1;
                            //    if (RZero == 2)
                            //    {
                            //        Prefil = "D" + RDigit;
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["POR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["POR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["POR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["POR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["POR_Frequency"].ToString());

                            //if (RFre == 4)
                            //{
                            Number = RNumber;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                            //}
                            //else if (RFre == 5)
                            //{
                            //Number = RNumber;
                            //if (RZero == 2)
                            //{
                            //    Prefil = "D" + RDigit;
                            //}
                            //}
                        }
                    }
                    OrderNum = Prefix + "" + Number.ToString(Prefil) + "" + Surfix;

                    return OrderNum;
                }
            }
            else
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = "PO number Not assigned for Given date";
            }
            return "";
        }







        //Purchase Order Edit
        [Route("procurement/transactions/purchase-order/{PO_No}/edit")]
        public IActionResult PurchaseOrderEdit(String? PO_No)
        {
            PurchaseGetData();
            String Active = GetPOEditData(PO_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseOrderRegisterSummary");
            }

            return View(POH_DTO);
        }

        [Route("procurement/transactions/purchase-order/{PO_No}/edit")]
        [HttpPost]
        public IActionResult PurchaseOrderEdit(PurchaseOrderHead_DTO P_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? PO_No)
        {
            var Original_PIH_DTO = Help.JsonClone(P_DTO);

            bool IsValid = false;
            PurchaseOrderHead_DTO P_Head_DTO = new PurchaseOrderHead_DTO();

            List<PurchaseOrderItem_DTO>? Item_DTO = new List<PurchaseOrderItem_DTO>();
            List<PurchaseOrderExpense_DTO>? Expense_DTO = new List<PurchaseOrderExpense_DTO>();
            List<PurchaseOrderIExpense_DTO>? ItemExpense_DTO = new List<PurchaseOrderIExpense_DTO>();

            P_Head_DTO = P_DTO;

            if (P_DTO.PurchaseItems != null)
                Item_DTO = P_DTO.PurchaseItems!.Where(K => K.IsDeleted == "false").ToList();

            if (P_DTO.Expenses != null)
                Expense_DTO = P_DTO.Expenses!.Where(K => K.IsDeleted == "false").ToList();

            if (P_DTO.ItemExpenses != null)
                ItemExpense_DTO = P_DTO.ItemExpenses!.Where(K => K.IsDeleted == "false").ToList();

            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.POI_MS_Number) != Convert.ToInt64(P_DTO.POH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.POI_Qty) == 0 || Convert.ToDouble(x.POI_UnitPrice) == 0 || Convert.ToDouble(x.POI_Amount) == 0);

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
                else if (Item_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(P_DTO.POH_ImportOrder) ? 2 : 1) != Convert.ToInt32(P_DTO.POH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();
                    P_Head_DTO.PurchaseItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            try
                            {
                                PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                                PO_DTO.POH_OrderNo = P_DTO.POH_OrderNo;
                                PO_DTO.POH_Date = Convert.ToString(Convert.ToDateTime(P_DTO.POH_Date).ToString("yyyyMMdd"));
                                PO_DTO.POH_Vendor_Number = Convert.ToString(P_DTO.POH_Vendor_Number);
                                PO_DTO.POH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(P_DTO.POH_ImportOrder) ? 1 : 0);
                                PO_DTO.POH_Currency_Number = Convert.ToString(P_DTO.POH_Currency_Number);
                                PO_DTO.POH_MS_Number = Convert.ToString(P_DTO.POH_MS_Number);
                                PO_DTO.POH_PaymentTerms = P_DTO.POH_PaymentTerms;
                                PO_DTO.POH_MOP = P_DTO.POH_MOP;
                                PO_DTO.POH_DeliveryTerms = P_DTO.POH_DeliveryTerms;
                                PO_DTO.POH_MOD = P_DTO.POH_MOD;
                                PO_DTO.POH_Tax = P_DTO.POH_Tax;
                                PO_DTO.POH_Inspection = P_DTO.POH_Inspection;
                                PO_DTO.POH_TDC = P_DTO.POH_TDC;
                                PO_DTO.POH_ExchangeRate = Convert.ToDouble(P_DTO.POH_ExchangeRate) == 0 ? "1" : P_DTO.POH_ExchangeRate;
                                PO_DTO.POH_Remarks = P_DTO.POH_Remarks;
                                PO_DTO.POH_MaterialValue = Convert.ToDouble(P_DTO.POH_MaterialValue).ToString();
                                PO_DTO.POH_ItemMiscExpense = Convert.ToDouble(P_DTO.POH_ItemMiscExpense).ToString();
                                PO_DTO.POH_HeadMiscExpense = Convert.ToDouble(P_DTO.POH_HeadMiscExpense).ToString();
                                PO_DTO.POH_OrderValue = Convert.ToDouble(P_DTO.POH_OrderValue).ToString();
                                PO_DTO.Id = 101;
                                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                                String ItemDTO = string.Join(", ", Item_DTO.Where(x => Convert.ToInt64(x.POI_Number) != 0).Select(x => x.POI_Number));
                                String ItemExpDTO = string.Join(", ", ItemExpense_DTO.Where(x => Convert.ToInt64(x.POI_EXP_Number) != 0).Select(x => x.POI_EXP_Number));
                                String ExpDTO = string.Join(", ", Expense_DTO.Where(x => Convert.ToInt64(x.POH_EXP_Number) != 0).Select(x => x.POH_EXP_Number));

                                PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                                PO_DTO.DeleteNumbers = Convert.ToString(ItemDTO);
                                PO_DTO.Id = 102;
                                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                                PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                                PO_DTO.DeleteNumbers = Convert.ToString(ExpDTO);
                                PO_DTO.Id = 103;
                                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                                PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                                PO_DTO.DeleteNumbers = Convert.ToString(ItemExpDTO);
                                PO_DTO.Id = 104;
                                DS = PO_DAO.PurchaseOrderDB(PO_DTO);

                                foreach (var Item in Item_DTO)
                                {
                                    DataSet D = new DataSet();
                                    PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                                    PO_DTO.POI_Item_Number = Convert.ToInt64(Item.POI_Item_Number);
                                    PO_DTO.POI_UoM_Number = Convert.ToInt64(Item.POI_UoM_Number);
                                    PO_DTO.POI_Qty = Convert.ToDouble(Item.POI_Qty);
                                    PO_DTO.POI_UnitPrice = Convert.ToDouble(Item.POI_UnitPrice);
                                    PO_DTO.POI_Amount = Convert.ToDouble(Item.POI_Amount);
                                    PO_DTO.POI_ExpenseValue = Convert.ToDouble(Item.POI_ExpenseValue);
                                    PO_DTO.POI_DeliveryDate = Convert.ToDateTime(Item.POI_DeliveryDate).ToString("yyyyMMdd");

                                    if (Item.POI_Number == 0)
                                    {
                                        PO_DTO.Id = 12;
                                        D = PO_DAO.PurchaseOrderDB(PO_DTO);

                                        var ItemExpense = ItemExpense_DTO.Where(x => (x.POI_EXP_Item_Number == Item.POI_Item_Number) && (x.POI_EXP_Item_Index == Item.POI_Index));

                                        foreach (var ItemExp in ItemExpense)
                                        {
                                            PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                                            PO_DTO.POI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            PO_DTO.POI_Item_Number = Convert.ToInt64(ItemExp.POI_EXP_Item_Number);
                                            PO_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.POI_EXP_Expense_Number);
                                            PO_DTO.EXP_Remarks = ItemExp.POI_EXP_Remarks;
                                            PO_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.POI_EXP_Occurrence_Number);
                                            PO_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.POI_EXP_CM_Number);
                                            PO_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.POI_EXP_ExpenseBase);
                                            PO_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.POI_EXP_ExpenseValue);
                                            PO_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.POI_EXP_Allocate_Number);
                                            PO_DTO.EXP_LA_Number = Convert.ToInt64(ItemExp.POI_EXP_LA_Number);
                                            if (ItemExp.POI_EXP_Number == 0)
                                            {
                                                PO_DTO.Id = 14;
                                            }
                                            else
                                            {
                                                PO_DTO.EXP_Number = Convert.ToInt64(ItemExp.POI_EXP_Number);
                                                PO_DTO.Id = 106;
                                            }

                                            PO_DAO.PurchaseOrderDB(PO_DTO);
                                        }
                                    }
                                    else
                                    {
                                        PO_DTO.POI_Number = Convert.ToInt64(Item.POI_Number);
                                        PO_DTO.Id = 105;
                                        D = PO_DAO.PurchaseOrderDB(PO_DTO);

                                        var ItemExpense = ItemExpense_DTO.Where(x => (x.POI_EXP_POI_Number == Item.POI_Number));

                                        foreach (var ItemExp in ItemExpense)
                                        {
                                            PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                                            PO_DTO.POI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            PO_DTO.POI_Item_Number = Convert.ToInt64(ItemExp.POI_EXP_Item_Number);
                                            PO_DTO.EXP_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            PO_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.POI_EXP_Expense_Number);
                                            PO_DTO.EXP_Remarks = ItemExp.POI_EXP_Remarks;
                                            PO_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.POI_EXP_Occurrence_Number);
                                            PO_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.POI_EXP_CM_Number);
                                            PO_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.POI_EXP_ExpenseBase);
                                            PO_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.POI_EXP_ExpenseValue);
                                            PO_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.POI_EXP_Allocate_Number);
                                            PO_DTO.EXP_LA_Number = Convert.ToInt64(ItemExp.POI_EXP_LA_Number);
                                            if (ItemExp.POI_EXP_Number == 0)
                                            {
                                                PO_DTO.Id = 14;
                                            }
                                            else
                                            {
                                                PO_DTO.EXP_Number = Convert.ToInt64(ItemExp.POI_EXP_Number);
                                                PO_DTO.Id = 106;
                                            }
                                            PO_DAO.PurchaseOrderDB(PO_DTO);
                                        }
                                    }

                                }

                                foreach (var Exp in Expense_DTO)
                                {
                                    PO_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.POH_EXP_Expense_Number);
                                    PO_DTO.EXP_Remarks = Exp.POH_EXP_Remarks;
                                    PO_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.POH_EXP_Occurrence_Number);
                                    PO_DTO.EXP_CM_Number = Convert.ToInt64(Exp.POH_EXP_CM_Number);
                                    PO_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.POH_EXP_ExpenseBase);
                                    PO_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.POH_EXP_ExpenseValue);
                                    PO_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.POH_EXP_Allocate_Number);
                                    PO_DTO.EXP_LA_Number = Convert.ToInt64(Exp.POH_EXP_LA_Number);
                                    if (Exp.POH_EXP_Number == 0)
                                    {
                                        PO_DTO.Id = 13;
                                    }
                                    else
                                    {
                                        PO_DTO.POH_Number = Convert.ToInt64(PO_No);
                                        PO_DTO.EXP_Number = Convert.ToInt64(Exp.POH_EXP_Number);
                                        PO_DTO.Id = 107;
                                    }
                                    PO_DAO.PurchaseOrderDB(PO_DTO);
                                }

                                transaction.Complete();

                                return RedirectToAction("PurchaseOrderRegisterSummary");

                                //P_Head_DTO.Reset();
                                //Expense_DTO = null;
                                //Item_DTO = null;
                                //ItemExpense_DTO = null;
                                //P_DTO.Reset();

                                //P_Head_DTO.POH_Date = DateTime.Now.ToString("dd-MMM-yy");
                            }
                            catch (Exception ex)
                            {
                                transaction.Dispose();
                                ViewBag.ErrorCode = 2;
                                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
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


            PurchaseGetData();
            return View(Original_PIH_DTO);
        }

        [Route("procurement/transactions/purchase-order/{PO_No}/item")]
        public IActionResult PurchaseOrderItem(String? ItemCode, String MS)
        {
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PO_DTO.POI_Item_Code = Convert.ToString(ItemCode).Trim();
            PO_DTO.POH_MS_Number = Convert.ToString(MS).Trim();
            PO_DTO.Id = 2;
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            var Item = P_DL.IList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("procurement/transactions/purchase-order/{PO_No}/uom")]
        public String PurchaseOrderUoM(String? UoM)
        {
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PO_DTO.POI_UoM_Number = Convert.ToInt32(UoM);
            PO_DTO.Id = 4;
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("procurement/transactions/purchase-order/{PO_No}/expense/des")]
        public IActionResult PurchaseOrderExpensiveDes(String? Title)
        {
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PO_DTO.EXP_Expense_Number = Convert.ToInt32(Title);
            PO_DTO.Id = 3;
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            var Expensive = P_DL.ExList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("procurement/transactions/purchase-order/{PO_No}/vendor")]
        public IActionResult PurchaseOrderVendor(String? Vendor, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (Import == null)
            {
                Import = "";
            }
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PO_DTO.POH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PO_DTO.POI_Item_Code = Convert.ToString(Vendor).Trim();
            PO_DTO.Id = 5;
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            var Ven = P_DL.VList(DS.Tables[0]);
            return Json(Ven);
        }
        String GetPOEditData(String PO_No)
        {
            PO_DTO.POH_Number = Convert.ToInt64(PO_No);
            PO_DTO.Id = 51;
            PO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PO_DAO.PurchaseOrderDB(PO_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                POH_DTO = P_DL.POHeadEditList(DS.Tables[0]).FirstOrDefault();
                POH_DTO.PurchaseItems = P_DL.POItemEditList(DS.Tables[1]);
                POH_DTO.Expenses = P_DL.POExpenseEditList(DS.Tables[2]);
                POH_DTO.ItemExpenses = P_DL.POIExpenseEditList(DS.Tables[3]);
                return "1";
            }
            else
            {
                return "0";
            }
        }



        //Purchase Order Numbering
        [Route("procurement/setup/purchase-order-numbering")]
        public IActionResult PONumbering()
        {
            GetPONumber();
            return View(PON_DTO);
        }

        [Route("procurement/setup/purchase-order-numbering")]
        [HttpPost]
        public IActionResult PONumbering(PONumber_DTO PN_DTO)
        {
                bool IsValid = false;
                PONumber_DTO P_Head_DTO = new PONumber_DTO();

                List<PONumberReset_DTO>? Reset_DTO = new List<PONumberReset_DTO>();
                List<PONumberPrefix_DTO>? Prefix_DTO = new List<PONumberPrefix_DTO>();
                List<PONumberSuffix_DTO>? Suffix_DTO = new List<PONumberSuffix_DTO>();

                P_Head_DTO = PON_DTO;

                if (PN_DTO.PONumberReset != null)
                    Reset_DTO = PN_DTO.PONumberReset!.Where(K => !K.POR_IsDeleted).ToList();

                if (PN_DTO.PONumberPrefix != null)
                    Prefix_DTO = PN_DTO.PONumberPrefix!.Where(K => !K.POP_IsDeleted).ToList();

                if (PN_DTO.PONumberSuffix != null)
                    Suffix_DTO = PN_DTO.PONumberSuffix!.Where(K => !K.POS_IsDeleted).ToList();

                if (PN_DTO.PON_Method == "2")
                {
                    String ResetDTO = string.Join(", ", Reset_DTO.Where(x => Convert.ToInt64(x.POR_Number) != 0).Select(x => x.POR_Number));
                    String PrefixDTO = string.Join(", ", Prefix_DTO.Where(x => Convert.ToInt64(x.POP_Number) != 0).Select(x => x.POP_Number));
                    String SuffixDTO = string.Join(", ", Suffix_DTO.Where(x => Convert.ToInt64(x.POS_Number) != 0).Select(x => x.POS_Number));


                    PON_DTO.CreatorCode = Convert.ToInt32(UserCode);
                    PON_DTO.DeleteNumbers = Convert.ToString(ResetDTO);
                    PON_DTO.Id = 31;
                    PON_DAO.PONumberDB(PON_DTO);

                    PON_DTO.DeleteNumbers = Convert.ToString(PrefixDTO);
                    PON_DTO.Id = 32;
                    PON_DAO.PONumberDB(PON_DTO);

                    PON_DTO.DeleteNumbers = Convert.ToString(SuffixDTO);
                    PON_DTO.Id = 33;
                    PON_DAO.PONumberDB(PON_DTO);

                    PON_DTO.PON_Method = PN_DTO.PON_Method;
                    if (PN_DTO.PON_Number == 0)
                    {
                        PON_DTO.Id = 11;
                    }
                    else
                    {
                        PON_DTO.Id = 41;
                        PON_DTO.PON_Number = PN_DTO.PON_Number;
                    }
                    PON_DAO.PONumberDB(PON_DTO);

                    foreach (var Reset in Reset_DTO)
                    {
                        PON_DTO.PON_Date = Convert.ToString(Convert.ToDateTime(Reset.POR_Date).ToString("yyyyMMdd"));
                        PON_DTO.PON_StartingNumber = Convert.ToInt32(Reset.POR_StartingNumber).ToString();
                        PON_DTO.PON_NumberofDigits = Convert.ToInt32(Reset.POR_NumberofDigits).ToString();
                        PON_DTO.PON_PrefilZero = Convert.ToInt64(Reset.POR_PrefilZero).ToString();
                        PON_DTO.PON_Frequency = Convert.ToInt64(Reset.POR_Frequency).ToString();
                        if (Reset.POR_Number == 0)
                        {
                            PON_DTO.Id = 12;
                        }
                        else
                        {
                            PON_DTO.Id = 42;
                            PON_DTO.PON_Number = Reset.POR_Number;
                        }
                        PON_DAO.PONumberDB(PON_DTO);
                    }

                    foreach (var Prefix in Prefix_DTO)
                    {
                        PON_DTO.PON_Date = Convert.ToString(Convert.ToDateTime(Prefix.POP_Date).ToString("yyyyMMdd"));
                        PON_DTO.PON_Particulars = Convert.ToString(Prefix.POP_Particulars);
                        if (Prefix.POP_Number == 0)
                        {
                            PON_DTO.Id = 13;
                        }
                        else
                        {
                            PON_DTO.Id = 43;
                            PON_DTO.PON_Number = Prefix.POP_Number;
                        }
                        PON_DAO.PONumberDB(PON_DTO);
                    }

                    foreach (var Suffix in Suffix_DTO)
                    {
                        PON_DTO.PON_Date = Convert.ToString(Convert.ToDateTime(Suffix.POS_Date).ToString("yyyyMMdd"));
                        PON_DTO.PON_Particulars = Convert.ToString(Suffix.POS_Particulars);
                        if (Suffix.POS_Number == 0)
                        {
                            PON_DTO.Id = 14;
                        }
                        else
                        {
                            PON_DTO.Id = 44;
                            PON_DTO.PON_Number = Suffix.POS_Number;
                        }
                        PON_DAO.PONumberDB(PON_DTO);
                    }
                    PON_DTO.Reset();
                    Reset_DTO = null;
                    Prefix_DTO = null;
                    Suffix_DTO = null;
                    ModelState.Clear();
                }
                else if (PN_DTO.PON_Method == "3")
                {
                    PON_DTO.PON_Method = PN_DTO.PON_Method;
                    if (PN_DTO.PON_Number == 0)
                    {
                        PON_DTO.Id = 21;
                    }
                    else
                    {
                        PON_DTO.Id = 22;
                        PON_DTO.PON_Number = PN_DTO.PON_Number;
                    }
                    PON_DAO.PONumberDB(PON_DTO);
                }

                GetPONumber();
                return View(PON_DTO);
        }
        void GetPONumber()
        {
            PON_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PON_DTO.Id = 1;
            DS = PON_DAO.PONumberDB(PON_DTO);

            ViewBag.Method = Help.GetCat(DS.Tables[0]);
            ViewBag.Frequency = Help.GetCat(DS.Tables[1]);
            ViewBag.Prefil = Help.GetCat(DS.Tables[2]);

            if (DS.Tables[3].Rows.Count > 0)
            {
                PON_DTO.PON_Number = Convert.ToInt64(DS.Tables[3].Rows[0]["PON_Number"]);
                PON_DTO.PON_Method = DS.Tables[3].Rows[0]["PON_Method"].ToString();
            }

            PON_DTO.PONumberReset = PON_DL.PORList(DS.Tables[4]);
            PON_DTO.PONumberPrefix = PON_DL.POPList(DS.Tables[5]);
            PON_DTO.PONumberSuffix = PON_DL.POSList(DS.Tables[6]);
        }
    
    
    }
}
