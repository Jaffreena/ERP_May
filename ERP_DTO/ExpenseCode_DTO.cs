using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class ExpenseCode_DTO
    {
        public Int64 ExpenseCodeNumber { get; set; }


        [Display(Name = "Expense Code")]
        [Required(ErrorMessage = "Expense Code is Required")]
        [MaxLength(30, ErrorMessage = "Expense Code be longer than 30 characters.")]
        public String? ExpenseCode { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Required")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        public String? EC_Description { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? LedgerAccount { get; set; }

        [Display(Name = "SAC")]
        [Required(ErrorMessage = "SAC is Required")]
        public String? EC_SAC_Number { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.ExpenseCodeNumber = 0;
            this.ExpenseCode = string.Empty;
            this.EC_Description = string.Empty;
            this.LedgerAccount = string.Empty;
            this.EC_SAC_Number = string.Empty;
        }
    }
}
