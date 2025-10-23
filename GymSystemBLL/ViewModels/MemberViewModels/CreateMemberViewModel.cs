using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemDAL.Entities.Enums;

namespace GymSystemBLL.ViewModels.MemberViewModels
{
    public class CreateMemberViewModel
    {
        [Required(ErrorMessage = "Name is Required!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name Must Be Between 2 and 50 Chars!")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name Can Contain Only Letters!")]
        public string Name { get; set; } = null!;

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
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Required!")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Range(1, 9000, ErrorMessage = "Building Number Must Be Greater Than 0")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "Required!")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Street Must Be Between 2 and 30")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City Can Contain Only Letters!")]
        public string City { get; set; } = null!;

        // HealthViewModel
        public HealthViewModel HealthViewModel { get; set; } = null!;
    }
}
