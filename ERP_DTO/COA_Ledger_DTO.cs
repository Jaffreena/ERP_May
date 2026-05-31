using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class COA_Ledger_DTO
    {
        public Int64 LedgerNumber { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        [MaxLength(6, ErrorMessage = "Ledger Account be longer than 6 characters.")]
        public String? LedgerAccount { get; set; }

        [Display(Name = "Ledger Name")]
        [Required(ErrorMessage = "Ledger Name is Required")]
        [MaxLength(50, ErrorMessage = "Ledger Name be longer than 50 characters.")]
        public String? LedgerName { get; set; }

        [Display(Name = "Ledger Group")]
        [Required(ErrorMessage = "Ledger Group is Required")]
        public String? LedgerGroup { get; set; }

        public String? DeleteNumbers { get; set; }

        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }

        public void Reset()
        {
            this.LedgerNumber = 0;
            this.LedgerAccount = string.Empty;
            this.LedgerName = string.Empty;
            this.LedgerGroup = string.Empty;
            this.DeleteNumbers = string.Empty;
        }
    }
}
