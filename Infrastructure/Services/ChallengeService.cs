using System.Net;
using Domain.Dtos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ChallengeService : IChallengeService
{
    private readonly DataContext _context;

    public ChallengeService(DataContext context)
    {
        _context = context;
    }

    public async Task<Response<List<GetChallengeDto>>> GetChallenges()
    {
        var locations = await _context.Challanges.Select(l=> new GetChallengeDto()
        {
            Id = l.Id,
            Title = l.Title,
            Description = l.Description,
        }).ToListAsync();
        return new Response<List<GetChallengeDto>>(locations);
    }
    
    public async Task<Response<List<GetChallengeWithGroupsDto>>> GetChallengeWithGroups()
    {
        var challanges = await (
            from ch in _context.Challanges
            select new GetChallengeWithGroupsDto()
            {
                Description = ch.Description,
                Id = ch.Id,
                Title = ch.Title,
                Groups = (
                    from gr in _context.Groups
                    where gr.ChallangeId == ch.Id
                    select new GetGroupDto()
                    {
                        Id = gr.Id,
                        ChallangeId = gr.ChallangeId,
                        CreatedAt = gr.CreatedAt,
                        GroupNick = gr.GroupNick,
                        NeededMember = gr.NeededMember,
                        TeamSlogan = gr.TeamSlogan

                    }
                ).ToList()
            }
        ).ToListAsync();

        return new Response<List<GetChallengeWithGroupsDto>>(challanges);
    }


    //add location 
    public async Task<Response<AddChallengeDto>> AddChallenge(AddChallengeDto model)
    {
        try
        {
            var group = new Challange()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
            };
            await _context.Challanges.AddAsync(group);
            await _context.SaveChangesAsync();
            model.Id = group.Id;
            return new Response<AddChallengeDto>(model);
        }
        catch (System.Exception ex)
        {
            return new Response<AddChallengeDto>(System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<Response<GetChallengeDto>> GetChallengeById(int id)
    {
        var find = await _context.Challanges.FindAsync(id);
        if (find == null) return new Response<GetChallengeDto>(HttpStatusCode.NotFound, "");
        var group = new GetChallengeDto()
        {
            Id = find.Id,
            Title = find.Title,
            Description = find.Description
        };
        return new Response<GetChallengeDto>(group);
    }

    //add location 
    public async Task<Response<AddChallengeDto>> UpdateChallenge(AddChallengeDto groupDto)
    {
        try
        {
            var finds = await _context.Challanges.FindAsync(groupDto.Id);
            if (finds == null) return new Response<AddChallengeDto>(System.Net.HttpStatusCode.NotFound, "");

            // if location is found
            finds.Title = groupDto.Title;
            finds.Id = groupDto.Id;
            finds.Description = groupDto.Description; 
   
            await _context.SaveChangesAsync();
            return new Response<AddChallengeDto>(groupDto);
        }
        catch (System.Exception ex)
        {
            return new Response<AddChallengeDto>(System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    //add location 
    public async Task<Response<string>> DeleteChallenge(int id)
    {
        try
        {
            var find = await _context.Challanges.FindAsync(id);
            if (find == null) return new Response<string>(System.Net.HttpStatusCode.NotFound, "");

            _context.Challanges.Remove(find);
            await _context.SaveChangesAsync();
            return new Response<string>("removed successfully");
        }
        catch (System.Exception ex)
        {
            return new Response<string>(System.Net.HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}