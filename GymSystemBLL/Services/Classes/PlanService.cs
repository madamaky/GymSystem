using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels.PlanViewModels;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;

namespace GymSystemBLL.Services.Classes
{
    internal class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<PlanViewModel> GetAllPlans()
        {
            var Plans = _unitOfWork.GetRepository<Plan>().GetAll();

            if (Plans is null || !Plans.Any()) return [];

            return Plans.Select(X => new PlanViewModel
            {
                Id = X.Id,
                Name = X.Name,
                Description = X.Description,
                DurationDays = X.DurationDays,
                Price = X.Price,
                IsActive = X.IsActive
            });
        }

        public PlanViewModel? GetPlanById(int id)
        {
            var Plan = _unitOfWork.GetRepository<Plan>().GetById(id);

            if (Plan is null) return null;

            return new PlanViewModel
            {
                Id = Plan.Id,
                Name = Plan.Name,
                Description = Plan.Description,
                DurationDays = Plan.DurationDays,
                Price = Plan.Price,
                IsActive = Plan.IsActive
            };
        }

        public UpdatePlanViewModel? GetPlanToUpdate(int PlanId)
        {
            var Plan = _unitOfWork.GetRepository<Plan>().GetById(PlanId);

            if (Plan is null || Plan.IsActive == false || HasActiveMembership(PlanId)) return null;

            return new UpdatePlanViewModel()
            {
                Description = Plan.Description,
                DurationDays = Plan.DurationDays,
                Price = Plan.Price,
                PlanName = Plan.Name
            };
        }

        public bool ToggleStatus(int PlanId)
        {
            var Repo = _unitOfWork.GetRepository<Plan>();

            var Plan = Repo.GetById(PlanId);

            if (Plan is null || HasActiveMembership(PlanId)) return false;

            Plan.IsActive = Plan.IsActive == true ? false : true;
            Plan.UpdatedAt = DateTime.Now;

            try
            {
                Repo.Update(Plan);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePlan(int PlanId, UpdatePlanViewModel updatedPlan)
        {
            var Plan = _unitOfWork.GetRepository<Plan>().GetById(PlanId);

            if (Plan is null || HasActiveMembership(PlanId)) return false;

            try
            {
                // Tuples [C# New Feature]
                (Plan.Description, Plan.DurationDays, Plan.Price, Plan.UpdatedAt) = 
                    (updatedPlan.Description, updatedPlan.DurationDays, updatedPlan.Price, DateTime.Now);

                _unitOfWork.GetRepository<Plan>().Update(Plan);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Helper Methods

        private bool HasActiveMembership(int PlanId)
        {
            var ActiveMembership = _unitOfWork.GetRepository<Membership>()
                .GetAll(X => X.PlanId == PlanId && X.Status == "Active");

            return ActiveMembership.Any();
        }

        #endregion
    }
}
