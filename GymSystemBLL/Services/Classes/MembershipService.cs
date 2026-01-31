using AutoMapper;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels.MembershipViewModels;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;

namespace GymSystemBLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<MembershipViewModel> GetAllMemberships()
        {
            var memberships = _unitOfWork.MembershipRepository.GetAllMembershipsWithMembersAndPlans(m => m.Status.ToLower() == "active");
            var membershipVM = _mapper.Map<IEnumerable<MembershipViewModel>>(memberships);
            return membershipVM;
        }

        public bool CreateMembership(CreateMembershipViewModel model)
        {
            if (!IsMemberExists(model.MemberId) || !IsPlanExists(model.PlanId) || HasActiveMembership(model.MemberId))
                return false;

            var membershipToCreate = _mapper.Map<Membership>(model);
            var plan = _unitOfWork.GetRepository<Plan>().GetById(model.PlanId);
            membershipToCreate.EndDate = DateTime.UtcNow.AddDays(plan!.DurationDays);

            _unitOfWork.MembershipRepository.Add(membershipToCreate);
            return _unitOfWork.SaveChanges() > 0;
        }

        public IEnumerable<PlanForSelectListViewModel> GetPlansForDropdown()
        {
            var plans = _unitOfWork.GetRepository<Plan>().GetAll(p => p.IsActive);
            var plansSelectList = _mapper.Map<IEnumerable<PlanForSelectListViewModel>>(plans);
            return plansSelectList;
        }

        public IEnumerable<MemberForSelectListViewModel> GetMembersForDropdown()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll();
            var membersSelectList = _mapper.Map<IEnumerable<MemberForSelectListViewModel>>(members);
            return membersSelectList;
        }

        public bool DeleteMembership(int memberId)
        {
            var membershipToDelete = _unitOfWork.MembershipRepository.GetFirstOrDefault(m => m.MemberId == memberId && m.Status.ToLower() == "active");
            if (membershipToDelete is null)
                return false;

            _unitOfWork.MembershipRepository.Delete(membershipToDelete);
            return _unitOfWork.SaveChanges() > 0;
        }



        #region Helper Methods

        private bool IsMemberExists(int memberId)
            => _unitOfWork.GetRepository<Member>().GetById(memberId) is not null;
        private bool IsPlanExists(int planId)
            => _unitOfWork.GetRepository<Plan>().GetById(planId) is not null;

        private bool HasActiveMembership(int memberId)
            => _unitOfWork.MembershipRepository
                .GetAllMembershipsWithMembersAndPlans(m => m.MemberId == memberId && m.Status.ToLower() == "active")
                .Any();

        #endregion
    }
}
