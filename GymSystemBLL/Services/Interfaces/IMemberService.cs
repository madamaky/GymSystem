using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystemBLL.ViewModels;

namespace GymSystemBLL.Services.Interfaces
{
    internal interface IMemberService
    {
        IEnumerable<MemberViewModel> GetAllMembers();

        bool CreateMembers(CreateMemberViewModel createMember);

        MemberViewModel? GetMemberDetails(int MemberId);

        // Get HealthRecord
        HealthViewModel? GetMemberHealthRecordDetails(int MemberId);

        // GetMemberId To Update View
        MemberToUpdateViewModel? GetMemberToUpdate(int MemberId);
        // Apply Update
        bool UpdateMemberDetails(int id, MemberToUpdateViewModel updatedMember);

        // Remove
        bool RemoveMember(int MemberId);
    }
}
