using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymSystemPL.Controllers
{
    public class MembershipController(IMembershipService _membershipService) : Controller
    {
        public IActionResult Index()
        {
            var memberships = _membershipService.GetAllMemberships();
            return View(memberships);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateMembershipViewModel model)
        {
            if (ModelState.IsValid)
            {
                var results = _membershipService.CreateMembership(model);

                if (results)
                    TempData["SuccessMessage"] = "Membership created successfully.";
                else
                    TempData["ErrorMessage"] = "An error occurred while creating the membership.";
             
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Please correct the errors in the form.";
            LoadDropdowns();

            return View(model);
        }

        public IActionResult Cancel(int memberId)
        {
            var result = _membershipService.DeleteMembership(memberId);
            if (result)
                TempData["SuccessMessage"] = "Membership deleted successfully.";
            else
                TempData["ErrorMessage"] = "An error occurred while deleting the membership.";

            return RedirectToAction(nameof(Index));
        }



        #region Helper Methods

        public void LoadDropdowns()
        {
            var plans = _membershipService.GetPlansForDropdown();
            var members = _membershipService.GetMembersForDropdown();
            ViewBag.Plans = new SelectList(plans, "Id", "Name");
            ViewBag.Members = new SelectList(members, "Id", "Name");
        }

        #endregion
    }
}
