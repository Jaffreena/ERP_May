using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class COA_Group_DTO
    {
        public Int64 LedgerGroupNumber { get; set; }

        [Display(Name = "Ledger Group")]
        [Required(ErrorMessage = "Ledger Group is Required")]
        [MaxLength(50, ErrorMessage = "Ledger Group be longer than 50 characters.")]
        public String? LedgerGroup { get; set; }

        [Display(Name = "Under")]
        public String? UnderLGroup { get; set; }

        [Display(Name = "Nature of COA Group")]
        [Required(ErrorMessage = "Nature of COA Group is Required")]
        public String? GroupNature { get; set; }

        public String? DeleteNumbers { get; set; }

        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }

        public void Reset()
        {
            this.LedgerGroupNumber = 0;
            this.LedgerGroup = string.Empty;
            this.UnderLGroup = string.Empty;
            this.GroupNature = string.Empty; 
            this.DeleteNumbers = string.Empty;
        }

    }
}
