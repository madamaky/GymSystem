using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.Services.AttachmentService;
using GymSystemBLL.Services.Interfaces;
using GymSystemBLL.ViewModels.MemberViewModels;
using GymSystemDAL.Entities;
using GymSystemDAL.Repositories.Interfaces;

namespace GymSystemBLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _attachmentService = attachmentService;
        }

        public bool CreateMembers(CreateMemberViewModel createMember)
        {
            try
            {
                // Check if Phone or Email Are Unique
                if (IsEmailExist(createMember.Email) || IsPhoneExist(createMember.Phone)) return false;

                var PhotoName = _attachmentService.Upload("members", createMember.PhotoFile);
                if (string.IsNullOrEmpty(PhotoName)) return false;

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
                member.Photo = PhotoName;

                _unitOfWork.GetRepository<Member>().Add(member);

                var isCreated = _unitOfWork.SaveChanges() > 0;
                if (!isCreated)
                {
                    _attachmentService.Delete(PhotoName, "members");
                    return false;
                }
                else
                {
                    return isCreated;
                }
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

            var Members = _unitOfWork.GetRepository<Member>().GetAll();
            if (Members is null || !Members.Any()) return [];

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

            var Member = _unitOfWork.GetRepository<Member>().GetById(MemberId);
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

            var ActiveMemberShip = _unitOfWork.GetRepository<Membership>().GetAll(X => X.MemberId == MemberId && X.Status == "Active")
                .FirstOrDefault();

            if(ActiveMemberShip is not null) // StartDate, EndDate
            {
                ViewModel.MembershipStartDate = ActiveMemberShip.CreatedAt.ToShortDateString();
                ViewModel.MembershipEndDate = ActiveMemberShip.EndDate.ToShortDateString();

                // Plans
                var Plan = _unitOfWork.GetRepository<Plan>().GetById(ActiveMemberShip.PlanId);
                ViewModel.PlanName = Plan?.Name;
            }

            return ViewModel;
        }

        public HealthViewModel? GetMemberHealthRecordDetails(int MemberId)
        {
            var MemberHealthRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(MemberId);

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
            var Member = _unitOfWork.GetRepository<Member>().GetById(MemberId);
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
            var MemberRepo = _unitOfWork.GetRepository<Member>();
            var MemberSessionRepo = _unitOfWork.GetRepository<MemberSession>();
            var MembershipRepo = _unitOfWork.GetRepository<Membership>();

            var Member = MemberRepo.GetById(MemberId);
            if (Member is null) return false;

            // Check If Member Has Active Sessions or Not
            //var HasActiveMemberSessions =
            //    MemberSessionRepo.GetAll(X => X.MemberId == MemberId && X.Session.StartDate > DateTime.Now).Any();

            // Get All SessionsIDs
            var SessionIDs = _unitOfWork.GetRepository<MemberSession>()
                .GetAll(X => X.MemberId == MemberId).Select(X => X.SessionId);

            var HasActiveSession = _unitOfWork.GetRepository<Session>()
                .GetAll(X => SessionIDs.Contains(X.Id) && X.StartDate > DateTime.Now).Any();

            if (HasActiveSession) return false;

            // Remove
            // Handle to Cascade Action in CODE
            var Membership = MembershipRepo.GetAll(X => X.MemberId == MemberId);

            try
            {
                if (Membership.Any())
                {
                    foreach (var member in Membership)
                    {
                        MembershipRepo.Delete(member);
                    }
                }

                MemberRepo.Delete(Member);

                var isDeleted = _unitOfWork.SaveChanges() > 0;
                if (isDeleted)
                    _attachmentService.Delete(Member.Photo, "members");
                return isDeleted;
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
                var MemberRepo = _unitOfWork.GetRepository<Member>();

                //if (IsEmailExist(updatedMember.Email) || IsPhoneExist(updatedMember.Phone)) return false;

                var emailExists = _unitOfWork.GetRepository<Member>()
                    .GetAll(X => X.Email == updatedMember.Email && X.Id != id);
                var phoneExists = _unitOfWork.GetRepository<Member>()
                    .GetAll(X => X.Phone == updatedMember.Phone && X.Id != id);

                if (emailExists.Any() || phoneExists.Any()) return false;

                var Member = MemberRepo.GetById(id);
                if (Member is null) return false;

                Member.Email = updatedMember.Email;
                Member.Phone = updatedMember.Phone;
                Member.Address.BuildingNumber = updatedMember.BuildingNumber;
                Member.Address.Street = updatedMember.Street;
                Member.Address.City = updatedMember.City;
                Member.UpdatedAt = DateTime.Now;

                MemberRepo.Update(Member);

                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Helper Methods

        private bool IsEmailExist(string email)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(X => X.Email == email).Any();
        }
        private bool IsPhoneExist(string phone)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(X => X.Phone == phone).Any();
        }

        #endregion
    }
}
