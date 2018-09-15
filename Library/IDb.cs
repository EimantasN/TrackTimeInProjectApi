using Library.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public interface IDb
    {
        Task AddError(Exception e);
        Task<RequestStatus> AddMember(Members member);
        Task<List<Members>> GetMembers();
        Task<RequestStatus> ChangeStatusAsync(Members member);
        Task<RequestStatus> StartedWorkingAsync(Members member, DateTime started);
        Task<RequestStatus> StopedWorkingAsync(Members member, DateTime stoped);
    }
}
