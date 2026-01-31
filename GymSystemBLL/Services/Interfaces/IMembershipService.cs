using GymSystemBLL.ViewModels.MembershipViewModels;

namespace GymSystemBLL.Services.Interfaces
{
    public interface IMembershipService
    {
        IEnumerable<MembershipViewModel> GetAllMemberships();
        IEnumerable<PlanForSelectListViewModel> GetPlansForDropdown();
        IEnumerable<MemberForSelectListViewModel> GetMembersForDropdown();
        bool CreateMembership(CreateMembershipViewModel model);
        bool DeleteMembership(int memberId);
    }
}
