using Library;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class Service : IDb
    {
        private readonly DbContextApi _ctx;

        public Service(DbContextApi ctx)
        {
            _ctx = ctx;
        }


        public async Task<RequestStatus> AddMember(Members member)
        {
            try
            {
                var model = await _ctx.Members.FirstOrDefaultAsync(x => x.Name == member.Name && x.LastName == member.LastName);
                if (model == null)
                {
                    await _ctx.Members.AddAsync(member);
                    await _ctx.SaveChangesAsync();
                    return new RequestStatus(true);
                }
                else
                {
                    return new RequestStatus("Alredy inserted", false);
                }
            }
            catch (Exception e)
            {
                await AddError(e);
                return new RequestStatus(e.InnerException.Message, false);
            }
        }

        public async Task<List<Members>> GetMembers()
        {
            try
            {
                return await _ctx.Members.ToListAsync();
            }
            catch (Exception e)
            {
                await AddError(e);
                return new List<Members>();
            }
        }

        public async Task AddError(Exception e)
        {
            try
            {
                var model = new Error(e);
                Error Error = await _ctx.Errors.FirstOrDefaultAsync(x =>
                    x.InnerMessage != model.InnerMessage &&
                    x.Message != model.Message &&
                    x.MethodName != model.MethodName);

                if (Error == null)
                {
                    await _ctx.AddAsync(model);
                }
                else
                {
                    Error.LastAccrued = DateTime.Now;
                    Error.Increase();
                    _ctx.Entry(Error).State = EntityState.Modified;
                }

                await _ctx.SaveChangesAsync();
            }
            catch (Exception) { }
        }

        public async Task AddError(Error model)
        {
            try
            {
                Error Error = await _ctx.Errors.FirstOrDefaultAsync(x =>
                    x.InnerMessage != model.InnerMessage &&
                    x.Message != model.Message &&
                    x.MethodName != model.MethodName);

                if (Error == null)
                {
                    await _ctx.AddAsync(model);
                }
                else
                {
                    Error.LastAccrued = DateTime.Now;
                    Error.Increase();
                    _ctx.Entry(Error).State = EntityState.Modified;
                }

                await _ctx.SaveChangesAsync();
            }
            catch (Exception) { }
        }

        public async Task<RequestStatus> ChangeStatusAsync(Members member)
        {
            try
            {
                var model = await _ctx.Members.FirstOrDefaultAsync(x => x.Id == member.Id);
                if (model == null)
                {
                    return new RequestStatus("Member neegzistuoja", false);
                }
                else
                {
                    model.Active = member.Active;
                    _ctx.Entry(model).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();

                    return new RequestStatus(true);
                }
            }
            catch (Exception e)
            {
                await AddError(e);
                return new RequestStatus(e.InnerException.Message, false);
            }
        }

        public async Task<RequestStatus> StartedWorkingAsync(Members member, DateTime started)
        {
            try
            {
                var model = await _ctx.Members.Include(x => x.Times).FirstOrDefaultAsync(x => x.Id == member.Id);
                if (model == null)
                {
                    return new RequestStatus("Member neegzistuoja", false);
                }
                else
                {
                    var TimeModel = new Time
                    {
                        Start = started
                    };

                    if (model.Times == null)
                        model.Times = new List<Time>();

                    if (model.Times.Count != 0)
                    {
                        var Temp = model.Times.Last();
                        if (Temp.End.Year == 1)
                        {
                            Temp.End = started;
                            var DiffTimeStam = Temp.End.Subtract(Temp.Start);
                            Temp.Diff = (int)DiffTimeStam.TotalMinutes;
                        }
                    }
                    model.Times.Add(TimeModel);
                    _ctx.Entry(model).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();

                    return new RequestStatus(true);
                }
            }
            catch (Exception e)
            {
                await AddError(e);
                return new RequestStatus(e.InnerException.Message, false);
            }
        }

        public async Task<RequestStatus> StopedWorkingAsync(Members member, DateTime stoped)
        {
            try
            {
                var model = await _ctx.Members.Include(x => x.Times).FirstOrDefaultAsync(x => x.Id == member.Id);
                if (model == null)
                {
                    return new RequestStatus("Member neegzistuoja", false);
                }
                else
                {
                    if (model.Times == null)
                    {
                        return new RequestStatus("Inconsistency Error", false);
                    }
                    Time Temp;
                    if (model.Times.Count > 0)
                    {
                        Temp = model.Times.Last();
                        if (Temp.Start <= stoped && Temp.End.Year == 1)
                        {
                            Temp.End = stoped;
                            var DiffTimeStam = Temp.End.Subtract(Temp.Start);
                            Temp.Diff = (int)DiffTimeStam.TotalMinutes;
                        }
                        else
                        {
                            return new RequestStatus("Inconsistency Error", false);
                        }
                    }
                    else
                    {
                        return new RequestStatus("Inconsistency Error", false);
                    }

                    if (model.Times != null && model.Times.Count > 0)
                    {
                        model.TotalTime = model.Times.Select(x => x.Diff).Sum();
                        model.CurrentDayTime = model.Times.Where(x =>
                            x.Start.Year == DateTime.Now.Year &&
                            x.Start.Day == DateTime.Now.Day &&
                            x.Start.Month == DateTime.Now.Month).Select(x => x.Diff).Sum();
                    }

                    _ctx.Entry(model).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();

                    return new RequestStatus(true);
                }
            }
            catch (Exception e)
            {
                await AddError(e);
                return new RequestStatus(e.InnerException.Message, false);
            }
        }

        public async Task<List<Members>> GetMembersByIdS(List<int> Ids)
        {
            try
            {
                if (Ids.Count > 0)
                {
                    var models = _ctx.Members.Where(x => Ids.Contains(x.Id)).ToList();
                    if (models == null && models.Count == 0)
                        return null;

                    return models;
                }
                return null;
            }
            catch (Exception e)
            {
                await AddError(e);
                return null;
            }
        }
    }


}
