using GymSystemBLL.ViewModels.BookingViewModel;
using GymSystemBLL.ViewModels.MembershipViewModels;
using GymSystemBLL.ViewModels.SessionViewModels;

namespace GymSystemBLL.Services.Interfaces
{
    public interface IBookingService
    {
        IEnumerable<SessionViewModel> GetAllSessionsWithTrainerAndCategory();
        IEnumerable<MemberForSessionViewModel> GetAllMembersForSession(int id);
        public bool CreateBooking(CreateBookingViewModel model);
        IEnumerable<MemberForSelectListViewModel> GetMembersForDropdown(int id);
        bool MemberAttended(MemberAttendOrCancelViewModel model);
        bool CancelBooking(MemberAttendOrCancelViewModel model);
    }
}
