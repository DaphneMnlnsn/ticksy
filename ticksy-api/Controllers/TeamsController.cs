using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticksy_api.Data;
using ticksy_api.Models;

[Route("api/[controller]")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notificationService;

    public TeamsController(AppDbContext context, NotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (await _context.Teams.AnyAsync(t => t.TeamName.ToLower() == dto.TeamName.ToLower() && t.DeletedAt == null))
            return BadRequest("A team with this name already exists.");
        
        var team = new Team
        {
            TeamName =  dto.TeamName,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Teams.Add(team);

        var membersToAdd = new List<TeamMember>
        {
            new TeamMember
            {
                Team = team,
                UserId = userId,
                TeamRole = TeamMember.TeamRoleEnum.Leader,
                JoinedAt = DateTime.UtcNow
            }
        };

        var validUserIds = await _context.Users
            .Where(u => dto.MemberIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();

        foreach (var memberId in validUserIds.Distinct())
        {
            if (memberId == userId) continue;

            membersToAdd.Add(new TeamMember
            {
                Team = team,
                UserId = memberId,
                TeamRole = TeamMember.TeamRoleEnum.Member,
                JoinedAt = DateTime.UtcNow
            });
        }

        _context.TeamMembers.AddRange(membersToAdd);
        await _context.SaveChangesAsync();

        var creator = await _context.Users.FindAsync(userId);

        foreach (var member in membersToAdd)
        {
            if (member.UserId == userId) continue;

            await _notificationService.CreateAsync(
                member.UserId,
                Notification.NotifType.AssignedToTeam,
                $"{creator.FirstName} added you to team '{team.TeamName}'.",
                false
            );
        }

        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, new {
            team.Id,
            team.TeamName,
        });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTeams()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var teams = await _context.TeamMembers
            .Where(tm => tm.UserId == userId && tm.Team.DeletedAt == null)
            .Select(tm => new TeamListDto
            {
                Id = tm.Team.Id,
                TeamName = tm.Team.TeamName
            })
            .ToListAsync();

        return Ok(teams);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeam(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isLeader = await _context.TeamMembers
            .AnyAsync(tm => tm.TeamId == id 
                        && tm.UserId == userId 
                        && tm.TeamRole == TeamMember.TeamRoleEnum.Leader);

        var isMember = await _context.TeamMembers
            .AnyAsync(tm => tm.TeamId == id && tm.UserId == userId);

        if (!isMember)
            return Forbid("You are not a member of this team.");

        var team = await _context.Teams
            .Include(t => t.TeamMembers)
            .ThenInclude(tm => tm.User)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (team == null) return NotFound();

        if (team.DeletedAt != null)
            return NotFound("This team has been deleted.");

        var members = await _context.TeamMembers
            .Select(t => new
            {
                t.UserId,
                t.User.FirstName,
                t.User.MiddleName,
                t.User.LastName,
                Role = t.TeamRole.ToString(),
                t.JoinedAt
            })
            .ToListAsync();

        var result = new TeamDetailsDto
        {
            Id = team.Id,
            TeamName = team.TeamName,
            CreatedBy = team.CreatedBy,
            Members = members.Select(tm => new TeamMemberDto
            {
                UserId = tm.UserId,
                FullName = string.Join(" ", new[]
                {
                    tm.FirstName,
                    tm.MiddleName,
                    tm.LastName
                }.Where(x => !string.IsNullOrWhiteSpace(x))),
                Role = tm.Role,
                JoinedAt = tm.JoinedAt
            }).ToList()
        };

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{teamId}/invite")]
    public async Task<IActionResult> CreateInvite(int teamId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isLeader = await _context.TeamMembers
            .AnyAsync(tm => tm.TeamId == teamId 
                        && tm.UserId == userId 
                        && tm.TeamRole == TeamMember.TeamRoleEnum.Leader);

        if (!isLeader)
            return Forbid();

        var token = Guid.NewGuid().ToString();

        var invite = new TeamInvite
        {
            TeamId = teamId,
            Token = token,
            CreatedBy = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _context.Add(invite);
        await _context.SaveChangesAsync();

        var link = $"https://ticksy-frontend.com/join/{token}";

        return Ok(new { link });
    }

    [Authorize]
    [HttpPost("accept-invite")]
    public async Task<IActionResult> AcceptInvite(TeamAcceptInviteDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var invite = await _context.TeamInvites
            .FirstOrDefaultAsync(i => i.Token == dto.Token);

        if (invite == null || invite.ExpiresAt < DateTime.UtcNow)
            return BadRequest("Invalid or expired invite.");

        _context.TeamMembers.Add(new TeamMember
        {
            TeamId = invite.TeamId,
            UserId = userId,
            TeamRole = TeamMember.TeamRoleEnum.Member,
            JoinedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok("Joined team!");
    }

    [Authorize]
    [HttpPost("{id}/add-members")]
    public async Task<IActionResult> AddMembers(int id, [FromBody] TeamAddMembersDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isLeader = await _context.TeamMembers
            .AnyAsync(tm => tm.TeamId == id 
                        && tm.UserId == userId 
                        && tm.TeamRole == TeamMember.TeamRoleEnum.Leader);

        if (!isLeader)
            return Forbid("Only team leaders can add members.");

        var existingMembers = await _context.TeamMembers
            .Where(tm => tm.TeamId == id)
            .Select(tm => tm.UserId)
            .ToListAsync();

        var membersToAdd = new List<TeamMember>();

        foreach (var memberId in dto.MemberIds.Distinct())
        {
            if (existingMembers.Contains(memberId)) continue;

            var userExists = await _context.Users.AnyAsync(u => u.Id == memberId);
            if (!userExists) continue;

            membersToAdd.Add(new TeamMember
            {
                TeamId = id,
                UserId = memberId,
                TeamRole = TeamMember.TeamRoleEnum.Member,
                JoinedAt = DateTime.UtcNow
            });
        }

        if (!membersToAdd.Any())
            return BadRequest("No valid members to add.");

        _context.TeamMembers.AddRange(membersToAdd);
        await _context.SaveChangesAsync();

        var leader = await _context.Users.FindAsync(userId);
        var team = await _context.Teams.FindAsync(id);

        foreach (var member in membersToAdd)
        {
            await _notificationService.CreateAsync(
                member.UserId,
                Notification.NotifType.AssignedToTeam,
                $"{leader.FirstName} added you to team '{team.TeamName}'.",
                false
            );
        }

        return Ok(new { message = "Members added successfully.", added = membersToAdd.Count });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(int id, [FromBody] TeamUpdateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isLeader = await _context.TeamMembers
            .AnyAsync(tm => tm.TeamId == id 
                        && tm.UserId == userId 
                        && tm.TeamRole == TeamMember.TeamRoleEnum.Leader);

        if (!isLeader)
            return Forbid("Only team leaders can perform this action.");

        var team = await _context.Teams.FindAsync(id);
        if (team == null) return NotFound();

        if (team.DeletedAt != null)
            return BadRequest("Cannot update a deleted team.");

        if(!string.IsNullOrWhiteSpace(dto.TeamName)) team.TeamName = dto.TeamName;

        team.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Team updated successfully."});
    }

    [Authorize]
    [HttpDelete("{teamId}/members/{memberId}")]
    public async Task<IActionResult> RemoveMember(int teamId, int memberId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isLeader = await _context.TeamMembers
            .AnyAsync(tm => tm.TeamId == teamId 
                        && tm.UserId == userId 
                        && tm.TeamRole == TeamMember.TeamRoleEnum.Leader);

        if (!isLeader)
            return Forbid("Only team leaders can remove members.");

        if (memberId == userId)
            return BadRequest("Leaders cannot remove themselves.");

        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == memberId);

        if (member == null)
            return NotFound("Member not found in this team.");

        _context.TeamMembers.Remove(member);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Member removed successfully." });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isLeader = await _context.TeamMembers
            .AnyAsync(tm => tm.TeamId == id 
                        && tm.UserId == userId 
                        && tm.TeamRole == TeamMember.TeamRoleEnum.Leader);

        if (!isLeader)
            return Forbid("Only team leaders can perform this action.");

        var team = await _context.Teams.FindAsync(id);
        if (team == null) return NotFound();

        if (team.DeletedAt != null)
            return BadRequest("Team is already deleted.");

        team.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Team deleted successfully."});
    }
}