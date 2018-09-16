using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace TrackTimeInProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IDb _ctx;

        public MembersController(IDb service)
        {
            _ctx = service;
        }

        // GET api/members
        [Route("Get/")]
        [HttpGet]
        public async Task<List<Members>> Get()
        {
            try
            {
                return await _ctx.GetMembers();
            }
            catch (Exception e)
            {
                await _ctx.AddError(e);
                return new List<Members>();
            }
        }

        // GET api/members
        [Route("GetById/")]
        [HttpGet]
        public async Task<List<Members>> GetById(List<int> Ids)
        {
            try
            {
                return await _ctx.GetMembersByIdS(Ids);
            }
            catch (Exception e)
            {
                await _ctx.AddError(e);
                return new List<Members>();
            }
        }

        // GET api/members
        [Route("AddError/")]
        [HttpGet]
        public async Task AddError([FromBody] Error error)
        {
            try
            {
                await _ctx.AddError(error);
            }
            catch (Exception e)
            {
                await _ctx.AddError(e);
            }
        }

        // POST api/AddMember
        [Route("AddMember/")]
        [HttpPost]
        public async Task<RequestStatus> AddMember([FromBody] Members member)
        {
            try
            {
                return await _ctx.AddMember(member);
            }
            catch (Exception e)
            {
                await _ctx.AddError(e);
                return new RequestStatus(e.Message, false);
            }
        }

        // POST api/AddMember
        [Route("ChangeStatus/")]
        [HttpPost]
        public async Task<RequestStatus> ChangeStatus([FromBody] Members member)
        {
            try
            {
                return await _ctx.ChangeStatusAsync(member);
            }
            catch (Exception e)
            {
                await _ctx.AddError(e);
                return new RequestStatus(e.Message, false);
            }
        }

        // POST api/AddMember
        [Route("MemberStarted/{started}")]
        [HttpPost]
        public async Task<RequestStatus> StartedWorking([FromBody] Members member, DateTime started)
        {
            try
            {
                return await _ctx.StartedWorkingAsync(member, started);
            }
            catch (Exception e)
            {
                await _ctx.AddError(e);
                return new RequestStatus(e.Message, false);
            }
        }

        [Route("MemberStoped/{stoped}")]
        [HttpPost]
        public async Task<RequestStatus> StopedWorking([FromBody] Members member, DateTime stoped)
        {
            try
            {
                return await _ctx.StopedWorkingAsync(member, stoped);
            }
            catch (Exception e)
            {
                await _ctx.AddError(e);
                return new RequestStatus(e.InnerException.Message, false);
            }
        }
    }
}
