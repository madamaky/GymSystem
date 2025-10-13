using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;

namespace GymSystemBLL.Services.Classes
{
    internal class MemberService : IMemberService
    {
        #region Fields

        private readonly IGenericRepository<Member> _memberRepoitory;
        private readonly IGenericRepository<Membership> _membershipRepoitory;
        private readonly IPlanRepository _planRepository;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepository;
        private readonly IGenericRepository<MemberSession> _membersessionRepository;

        #endregion

        public MemberService
            (IGenericRepository<Member> memberRepoitory,
             IGenericRepository<Membership> membershipRepoitory,
             IPlanRepository planRepository,
             IGenericRepository<HealthRecord> healthRecordRepository,
             IGenericRepository<MemberSession> membersessionRepository)
        {
            _memberRepoitory = memberRepoitory;
            _membershipRepoitory = membershipRepoitory;
            _planRepository = planRepository;
            _healthRecordRepository = healthRecordRepository;
            _membersessionRepository = membersessionRepository;
        }

        public bool CreateMembers(CreateMemberViewModel createMember)
        {
            try
            {
                // Check if Phone or Email Are Unique
                if (IsEmailExist(createMember.Email) || IsPhoneExist(createMember.Phone)) return false;

                var member = new Member()
                {
                    Name = createMember.Name,
                    Email = createMember.Email,
                    Phone = createMember.Phone,
                    Gender = createMember.Gender,
                    DateOfBirth = createMember.DateOfBirth,
                    Address = new Address()
                    {
                        BuildingNumber = createMember.BuildingNumber,
                        Street = createMember.Street,
                        City = createMember.City
                    },
                    HealthRecord = new HealthRecord()
                    {
                        Height = createMember.HealthViewModel.Height,
                        Weight = createMember.HealthViewModel.Weight,
                        BloodType = createMember.HealthViewModel.BloodType,
                        Note = createMember.HealthViewModel.Note
                    }
                };

                return _memberRepoitory.Add(member) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            #region First Way Of Mapping

            ////var Members = _memberRepoitory.GetAll() ?? [];

            //var Members = _memberRepoitory.GetAll();
            //if (Members is null || Members.Any()) return [];

            //var MemberViewModels = new List<MemberViewModel>();

            //foreach (var Member in Members)
            //{
            //    var memberViewModel = new MemberViewModel()
            //    {
            //        Id = Member.Id,
            //        Name = Member.Name,
            //        Email = Member.Email,
            //        Phone = Member.Phone,
            //        Gender = Member.Gender.ToString()
            //    };
            //    MemberViewModels.Add(memberViewModel);
            //}

            //return MemberViewModels;

            #endregion

            var Members = _memberRepoitory.GetAll();
            if (Members is null || Members.Any()) return [];

            var MemberViewModels = Members.Select(X => new MemberViewModel
            {
                Id = X.Id,
                Name = X.Name,
                Email = X.Email,
                Phone = X.Phone,
                Photo = X.Photo,
                Gender = X.Gender.ToString()
            });

            return MemberViewModels;
        }

        public MemberViewModel? GetMemberDetails(int MemberId)
        {
            // IPlanRepository
            // Inject For PlanRepo && Membership Repo

            var Member = _memberRepoitory.GetById(MemberId);
            if (Member == null) return null;

            var ViewModel = new MemberViewModel()
            {
                Name = Member.Name,
                Email = Member.Email,
                Phone = Member.Phone,
                Photo = Member.Photo,
                Gender = Member.Gender.ToString(),
                DateOfBirth = Member.DateOfBirth.ToShortDateString(),
                Address = $"{Member.Address.BuildingNumber} - {Member.Address.Street} - {Member.Address.City}"
            };

            var ActiveMemberShip = _membershipRepoitory.GetAll(X => X.MemberId == MemberId && X.Status == "Active")
                .FirstOrDefault();

            if(ActiveMemberShip is not null) // StartDate, EndDate
            {
                ViewModel.MembershipStartDate = ActiveMemberShip.CreatedAt.ToShortDateString();
                ViewModel.MembershipEndDate = ActiveMemberShip.EndDate.ToShortDateString();

                // Plans
                var Plan = _planRepository.GetById(ActiveMemberShip.PlanId);
                ViewModel.PlanName = Plan?.Name;
            }

            return ViewModel;
        }

        public HealthViewModel? GetMemberHealthRecordDetails(int MemberId)
        {
            var MemberHealthRecord = _healthRecordRepository.GetById(MemberId);

            if (MemberHealthRecord is null) return null;

            return new HealthViewModel()
            {
                BloodType = MemberHealthRecord.BloodType,
                Height = MemberHealthRecord.Height,
                Weight = MemberHealthRecord.Weight,
                Note = MemberHealthRecord.Note
            };
        }

        public MemberToUpdateViewModel? GetMemberToUpdate(int MemberId)
        {
            var Member = _memberRepoitory.GetById(MemberId);
            if (Member is null) return null;

            return new MemberToUpdateViewModel()
            {
                Email = Member.Email,
                Phone = Member.Phone,
                Photo = Member.Photo,
                Name = Member.Name,
                BuildingNumber = Member.Address.BuildingNumber,
                City = Member.Address.City,
                Street = Member.Address.Street
            };
        }

        public bool RemoveMember(int MemberId)
        {
            var Member = _memberRepoitory.GetById(MemberId);
            if (Member is null) return false;

            // Check If Member Has Active Sessions or Not
            var HasActiveMemberSessions = 
                _membersessionRepository.GetAll(X => X.MemberId == MemberId && X.Session.StartDate > DateTime.Now).Any();

            if (HasActiveMemberSessions) return false;

            // Remove
            // Handle to Cascade Action in CODE
            var Membership = _membershipRepoitory.GetAll(X => X.MemberId == MemberId);

            try
            {
                if (Membership.Any())
                {
                    foreach (var member in Membership)
                    {
                        _membershipRepoitory.Delete(member);
                    }
                }

                return _memberRepoitory.Delete(Member) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateMemberDetails(int id, MemberToUpdateViewModel updatedMember)
        {
            try
            {
                if (IsEmailExist(updatedMember.Email) || IsPhoneExist(updatedMember.Phone)) return false;

                var Member = _memberRepoitory.GetById(id);
                if (Member is null) return false;

                Member.Email = updatedMember.Email;
                Member.Phone = updatedMember.Phone;
                Member.Address.BuildingNumber = updatedMember.BuildingNumber;
                Member.Address.Street = updatedMember.Street;
                Member.Address.City = updatedMember.City;
                Member.UpdatedAt = DateTime.Now;
                return _memberRepoitory.Update(Member) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Helper Methods

        private bool IsEmailExist(string email)
        {
            return _memberRepoitory.GetAll(X => X.Email == email).Any();
        }
        private bool IsPhoneExist(string phone)
        {
            return _memberRepoitory.GetAll(X => X.Phone == phone).Any();
        }

        #endregion
    }
}
