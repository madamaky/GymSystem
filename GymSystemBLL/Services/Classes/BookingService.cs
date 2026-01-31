using AutoMapper;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels.BookingViewModel;
using GymSystemBLL.ViewModels.MembershipViewModels;
using GymSystemBLL.ViewModels.SessionViewModels;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;

namespace GymSystemBLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<SessionViewModel> GetAllSessionsWithTrainerAndCategory()
        {
            var sessions = _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory();
            var sessionViewModels = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in sessionViewModels)
                session.AvailableSlot = session.Capacity - _unitOfWork.SessionRepository.GetCountOfBookedSlot(session.Id);

            return sessionViewModels;
        }

        public IEnumerable<MemberForSessionViewModel> GetAllMembersForSession(int id)
        {
            var membersForSession = _unitOfWork.BookingRepository.GetSessionsById(id);
            return _mapper.Map<IEnumerable<MemberForSessionViewModel>>(membersForSession);
        }

        public bool CreateBooking(CreateBookingViewModel model)
        {
            var session = _unitOfWork.SessionRepository.GetById(model.SessionId);
            if (session is null || session.StartDate <= DateTime.UtcNow)
                return false;

            var activeMembershipForMemeber = _unitOfWork.MembershipRepository.GetFirstOrDefault(m =>
                m.Status.ToLower() == "active" &&
                m.MemberId == model.MemberId);
            if (activeMembershipForMemeber is null)
                return false;

            var bookedSlots = _unitOfWork.SessionRepository.GetCountOfBookedSlot(model.SessionId);
            var availableSlots = session.Capacity - bookedSlots;
            if (availableSlots == 0)
                return false;

            var booking = _mapper.Map<MemberSession>(model);
            booking.IsAttended = false;

            _unitOfWork.BookingRepository.Add(booking);
            return _unitOfWork.SaveChanges() > 0;
        }

        public bool MemberAttended(MemberAttendOrCancelViewModel model)
        {
            var memberSession = _unitOfWork.GetRepository<MemberSession>()
                .GetAll(X => X.MemberId == model.MemberId && X.SessionId == model.SessionId)
                .FirstOrDefault();
            if (memberSession is null) 
                return false;

            memberSession.IsAttended = true;
            memberSession.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<MemberSession>().Update(memberSession);
            return _unitOfWork.SaveChanges() > 0;
        }

        public bool CancelBooking(MemberAttendOrCancelViewModel model)
        {
            var session = _unitOfWork.SessionRepository.GetById(model.SessionId);
            if (session is null || session.StartDate <= DateTime.Now) 
                return false;

            var Booking = _unitOfWork.BookingRepository
                .GetAll(X => X.MemberId == model.MemberId && X.SessionId == model.SessionId)
                .FirstOrDefault();
            if (Booking is null) 
                return false;

            _unitOfWork.BookingRepository.Delete(Booking);
            return _unitOfWork.SaveChanges() > 0;
        }
        
        #region Helper Methods

        public IEnumerable<MemberForSelectListViewModel> GetMembersForDropdown(int id)
        {
            var bookedMemberIds = _unitOfWork.BookingRepository.GetAll(s => s.SessionId == id )
                .Select(ms => ms.MemberId)
                .ToList();

            var membersAvailableToBook = _unitOfWork.GetRepository<Member>().GetAll(m => !bookedMemberIds.Contains(m.Id));
            return _mapper.Map<IEnumerable<MemberForSelectListViewModel>>(membersAvailableToBook);
        }

        #endregion
    }
}
