using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
   

    public class JW_Process_DTO
    {
        public Int64 ProcessNumber { get; set; }

        [Display(Name = "Process Name")]
        [Required(ErrorMessage = "Process Name is Required")]
        [MaxLength(25, ErrorMessage = "Process Name should not be longer than 25 characters.")]
        public String? ProcessName { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Required")]
        [MaxLength(100, ErrorMessage = "Description should not  be longer than 100 characters.")]
        public String? Description { get; set; }

       

        [Display(Name = "Consumption UoM")]
        [Required(ErrorMessage = "Consumption UoM is Required")]
        public String? ConsumptionUoM { get; set; }

        [Display(Name = "Production UoM")]
        [Required(ErrorMessage = "Production UoM is Required")]
        public String? ProductionUoM { get; set; }

        [Display(Name = "Scrap UoM")]
        [Required(ErrorMessage = "Scrap UoM is Required")]
        public String? ScrapUoM { get; set; }


        [Display(Name = "Scrap ItemCode")]
        [Required(ErrorMessage = "Scrap ItemCode is Required")]
        public String? ScrapItemCode { get; set; }

        [Display(Name = "SAC")]
        [Required(ErrorMessage = "SAC")]
        public String? SAC { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.ProcessNumber = 0;
            this.ProcessName = string.Empty;
            this.Description = string.Empty;
            this.ConsumptionUoM = string.Empty;
            this.ScrapUoM = string.Empty;
            this.ScrapItemCode = string.Empty;
            this.SAC = string.Empty;
        }
    }


}
