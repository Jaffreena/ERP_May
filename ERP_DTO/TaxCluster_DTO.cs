using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class TaxCluster_DTO
    {
        public Int64 TaxClusterNumber { get; set; }
        public String? GST_Category { get; set; }
        public String? GST_Type { get; set; }
        public String? TaxCluster { get; set; }
        public String? ClusterDescription { get; set; }

        public Int32 TaxIndex { get; set; }
        public Int64 TaxClusterDetailsNumber { get; set; }
        public Int64 TaxElement { get; set; }
        public Int64? ChargeableBasis { get; set; }
        public String? CalculationFactors { get; set; }


        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.TaxClusterNumber = 0;
            this.GST_Category = "0";
            this.ClusterDescription = string.Empty;
            this.GST_Type = "0";
            this.ChargeableBasis = 0;
            this.TaxCluster = string.Empty;
            this.CalculationFactors = string.Empty;
            this.TaxClusterDetailsNumber = 0;
            this.TaxElement = 0;
        }
    }
    public class TaxClusterHead_DTO
    {
        public Int64 TaxClusterNumber { get; set; }

        [Display(Name = "Tax Cluster")]
        [Required(ErrorMessage = "Tax Cluster is Required")]
        [MaxLength(25, ErrorMessage = "Tax Cluster be longer than 25 characters.")]
        public String? TaxCluster { get; set; }


        [Display(Name = "Tax Description")]
        [Required(ErrorMessage = "Tax Description is Required")]
        [MaxLength(50, ErrorMessage = "Tax Description be longer than 50 characters.")]
        public String? ClusterDescription { get; set; }


        [Display(Name = "GST Tax Category")]
        [Required(ErrorMessage = "GST Tax Category is Required")]
        public String? GST_Category { get; set; }


        [Display(Name = "GST Tax Type")]
        [Required(ErrorMessage = "GST Tax Type is Required")]
        public String? GST_Type { get; set; }

        public List<TaxClusterDetail_DTO>? TaxElement { get; set; }
        public PaginatedList_DTO<TaxCluster_DTO>? TaxCluster_List { get; set; }


        public Boolean IsDeleted { get; set; }

        public void Reset()
        {
            this.TaxClusterNumber = 0;
            this.TaxCluster = string.Empty;
            this.ClusterDescription = string.Empty;
            this.GST_Category = "0";
            this.GST_Type = "0";
            this.TaxElement = null;
            this.TaxCluster_List = null;
        }
    }
    public class TaxClusterDetail_DTO
    {
        public Int64 TaxIndex { get; set; }
        public Int64 TaxClusterDetailsNumber { get; set; }

        [Display(Name = "Tax Element")]
        [Required(ErrorMessage = "Tax Element is Required")]
        public String? TaxElement { get; set; }

        public String? TaxElementDescription { get; set; }


        [Display(Name = "ChargeableBasis")]
        [Required(ErrorMessage = "ChargeableBasis is Required")]
        public String? ChargeableBasis { get; set; }

        [Display(Name = "Calculation Factors")]
        public String? CalculationFactors { get; set; }

        public Boolean IsDeleted { get; set; }

        public void Reset()
        {
            this.TaxClusterDetailsNumber = 0;
            this.TaxElement = "0";
            this.TaxElementDescription = string.Empty;
            this.ChargeableBasis = "0";
            this.CalculationFactors = string.Empty;
        }
    }
}
