using Microsoft.EntityFrameworkCore;

namespace WebApi.Services;

using AutoMapper;
using BCrypt.Net;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;

public interface IUserService
{
    Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
    Task<IEnumerable<User>> GetAll();
    Task<User> GetById(int id);
    Task RegisterSeller(RegisterRequest model);
    Task RegisterBuyer(RegisterRequest model);
    Task Update(int id, UpdateRequest model);
    Task Delete(int id);
}

public class UserService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    public UserService(
        DataContext context,
        IJwtUtils jwtUtils,
        IMapper mapper)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
    }
    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == model.Username);
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");
        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);
        return response;
    }
    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<User> GetById(int id)
    {
        return await getUser(id);
    }
    public async Task RegisterSeller(RegisterRequest model)
    {
        if (await _context.Users.AnyAsync(x => x.Username == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");
        var user = _mapper.Map<User>(model);
        user.PasswordHash = BCrypt.HashPassword(model.Password);
        user.IsSeller = true;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
    public async Task RegisterBuyer(RegisterRequest model)
    {
        if (await _context.Users.AnyAsync(x => x.Username == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");
        var user = _mapper.Map<User>(model);
        user.PasswordHash = BCrypt.HashPassword(model.Password);
        user.IsSeller = false;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
    public async Task Update(int id, UpdateRequest model)
    {
        var user = await getUser(id);
        if (model.Username != user.Username && _context.Users.Any(x => x.Username == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);
        _mapper.Map(model, user);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    public async Task Delete(int id)
    {
        var user = await getUser(id);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
    private async Task<User> getUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}