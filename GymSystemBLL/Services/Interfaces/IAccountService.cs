using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.ViewModels.AccountViewModels;
using GymSystemDAL.Entities;

namespace GymSystemBLL.Services.Interfaces
{
    public interface IAccountService
    {
        ApplicationUser? ValidateUser(LoginViewModel loginVM);
    }
}
