using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystemBLL.ViewModels.MemberViewModels
{
    public class MemberToUpdateViewModel
    {
        public string Name { get; set; }
        public string? Photo { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Invalid Email Format!")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email Must Be Between 5 and 100 Chars!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone is Required!")]
        [Phone(ErrorMessage = "Invalid Phone Format!")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "You Must Enter Egyptian Number Format!")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        [Range(1, 9000, ErrorMessage = "Building Number Must Be Greater Than 0")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "Required!")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Street Must Be Between 2 and 30")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City Can Contain Only Letters!")]
        public string City { get; set; } = null!;
    }
}
