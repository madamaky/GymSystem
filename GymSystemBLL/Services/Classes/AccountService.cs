using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels.AccountViewModels;
using GymSystemDAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymSystemBLL.Services.Classes
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser? ValidateUser(LoginViewModel loginVM)
        {
            var User = _userManager.FindByEmailAsync(loginVM.Email).Result;
            if (User is null) return null;

            var isPasswordValid = _userManager.CheckPasswordAsync(User, loginVM.Password).Result;

            return isPasswordValid ? User : null;
        }
    }
}
