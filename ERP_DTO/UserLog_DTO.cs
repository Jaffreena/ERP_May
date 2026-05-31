using System.ComponentModel.DataAnnotations;

namespace ERP_DTO
{
    public class UserLog_DTO
    {
        [Key]
        public Int32 Number { get; set; }

        [Display(Name ="Username")]
        [Required(ErrorMessage = "Username is Required")]
        public String? Username { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is Required")]
        [MaxLength(32, ErrorMessage = "Password be longer than 32 characters.")]
        public String? Password { get; set; }

        public Int32 Theme { get; set; }
        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }
    }
}
