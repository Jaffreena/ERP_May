using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class Currency_DTO
    {
        public Int64 CurrencyNumber { get; set; }

        [Display(Name = "Currency Code")]
        [Required(ErrorMessage = "Currency Code is Required")]
        [MaxLength(10, ErrorMessage = "Currency Code be longer than 10 characters.")]
        public String? CurrencyCode { get; set; }

        [Display(Name = "Formal Name")]
        [Required(ErrorMessage = "Formal Name is Required")]
        [MaxLength(30, ErrorMessage = "Formal Name be longer than 30 characters.")]
        public String? FormalName { get; set; }

        [Display(Name = "Symbol")]
        [MaxLength(3, ErrorMessage = "Symbol be longer than 3 characters.")]
        public String? Symbol { get; set; }

        [Display(Name = "Number of decimal places")]
        [Required(ErrorMessage = "Number of decimal places is Required")]
        [Range(0, 9, ErrorMessage = "Number of decimal places be longer than 10")]
        [MaxLength(1, ErrorMessage = "Number of decimal places be longer than 1 characters.")]
        [MinLength(1, ErrorMessage = "Number of decimal places be more than 1 characters.")]
        public String? DecimalPlaces { get; set; }

        [Display(Name = "Name of Decimal Portion")]
        [Required(ErrorMessage = "Name of Decimal Portion is Required")]
        [MaxLength(15, ErrorMessage = "Name of Decimal Portion be longer than 15 characters.")]
        public String? DecimalPortionName { get; set; }

        [Display(Name = "Currency Location")]
        [Required(ErrorMessage = "Currency Location is Required")]
        public String? CurrencyLocation { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.CurrencyNumber = 0;
            this.CurrencyCode = string.Empty;
            this.FormalName = string.Empty;
            this.Symbol = string.Empty;
            this.DecimalPlaces = string.Empty;
            this.DecimalPortionName = string.Empty;
            this.CurrencyLocation = "0";
            this.DeleteNumbers = string.Empty;
        }
    }


    public class CurrencyLocation_DTO
    {
        public Int64 Number { get; set; }
        public String? Location { get; set; }
        public Boolean Checked { get; set; }
    }
}
