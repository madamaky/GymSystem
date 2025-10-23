using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystemBLL.ViewModels.MemberViewModels
{
    public class HealthViewModel
    {
        [Required(ErrorMessage = "Required!")]
        [Range(1, 300, ErrorMessage = "Height Must Be Greater Than 0")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Range(1, 500, ErrorMessage = "Weight Must Be Greater Than 0 and Less Than 500")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Required!")]
        [StringLength(3, ErrorMessage = "Blood Type Must Be 3 Chars or Less")]
        public string BloodType { get; set; }

        public string? Note { get; set; }
    }
}
