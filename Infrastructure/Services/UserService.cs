using ArduinoAttendance.Domain.Entities;
using ArduinoAttendance.DTOs.User;
using ArduinoAttendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArduinoAttendance.Infrastructure.Services
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<User> CreateUserAsync(UserRequest userRequest)
        {
            var user = new User
            {
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Email = userRequest.Email,
                PhoneNumber = userRequest.PhoneNumber,
                Address = userRequest.Address,
                Nic = userRequest.Nic,
                RFIDTag = userRequest.RFIDTag,
                Role = userRequest.Role,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                isActive = true
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(Guid id, User updatedUser)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return null;

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Address = updatedUser.Address;
            user.Nic = updatedUser.Nic;
            user.RFIDTag = updatedUser.RFIDTag;
            user.Role = updatedUser.Role;
            user.LastModified = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
