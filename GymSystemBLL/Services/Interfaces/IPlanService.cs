using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.ViewModels.PlanViewModels;

namespace GymSystemBLL.Services.Interfaces
{
    public interface IPlanService
    {
        IEnumerable<PlanViewModel> GetAllPlans();

        PlanViewModel? GetPlanById(int id);

        UpdatePlanViewModel? GetPlanToUpdate(int PlanId);
        bool UpdatePlan(int PlanId, UpdatePlanViewModel updatedPlan);

        bool ToggleStatus(int PlanId);
    }
}
