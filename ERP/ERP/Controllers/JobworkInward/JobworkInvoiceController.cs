using ERP.Models;
using ERP_DAO.JobInwardTransaction;
using ERP_DTO.JobInwardTransaction;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ERP.Controllers.JobworkInward
{
    public class JobworkInvoiceController : Controller
    {
        Help Help = new Help();
        public IActionResult Create()
        {
            GetJobworkInvoiceData();
            return View();
        }

        public void GetJobworkInvoiceData()
        {
            JobworkInvoiceCreate_DTO DN_DTO = new JobworkInvoiceCreate_DTO();
            JobworkInvoice_DAO DN_DAO = new JobworkInvoice_DAO();
            DN_DTO.Header.JISVIH_InvoiceDate = DateTime.Now;
            DN_DTO.Header.JW_Inv_Id = 1;
            DataSet DS = new DataSet();
            DS = DN_DAO.JobworkInvoice(DN_DTO);
            ViewBag.Currency = Help.GetCat(DS.Tables[4]);         
            ViewBag.UoM = Help.GetCat(DS.Tables[5]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[7]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[11]);
            ViewBag.Process = Help.GetCat(DS.Tables[12]);

        }
        }
}
