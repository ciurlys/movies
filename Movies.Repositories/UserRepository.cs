using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movies.EntityModels;

namespace Movies.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MoviesDataContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public UserRepository(MoviesDataContext db,
			  UserManager<IdentityUser> userManager)
    {
	_db = db;
	_userManager = userManager;
    }

    public async Task<List<string>> GetAllUserIds()
    {
	return await _userManager.Users
	    .Select(u => u.Id)
	    .ToListAsync();
    }
        
}
