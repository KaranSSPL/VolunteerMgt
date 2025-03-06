using Microsoft.AspNetCore.Identity;
using System.Net;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Register;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Services
{
    public class UserService(
        RoleManager<ApplicationRole> _roleManager,
        UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager,
        ICurrentUserService _currentUserService,
        ILogger<UserService> _logger
        ) : IUserService
    {
        public async Task<Result<RegisterModel>> AddUserAsync(RegisterModel model)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    return Result<RegisterModel>.Fail("Email is already registered.", HttpStatusCode.BadRequest);
                }
                var currentUser = _currentUserService.GetUserEmail();
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CreatedDate = DateTime.Now,
                    CreatedBy = (currentUser != null) ? _userManager.FindByEmailAsync(currentUser).Result?.Id : model.Username
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errorMessage = result.Errors.Select(e => e.Description).ToList();
                    return Result<RegisterModel>.Fail(errorMessage, HttpStatusCode.BadRequest);
                }
                var roleExists = await _roleManager.RoleExistsAsync(model.Role);

                if (!roleExists)
                {
                    _logger.LogWarning("Role '{Role}' does not exist.", model.Role);
                    return Result<RegisterModel>.Fail($"Role '{model.Role}' does not exist.", HttpStatusCode.BadRequest);
                }

                await _userManager.AddToRoleAsync(user, model.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return await Result<RegisterModel>.SuccessAsync(model, "Successfully Created User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating user {Email}.", model.Email);
                return Result<RegisterModel>.Fail("An internal error occurred while creating the user.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> DeleteUserAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("Delete user attempt failed: User ID is required.");
                    return Result.Fail("Invalid request. User ID is required.", HttpStatusCode.BadRequest);
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Delete user attempt failed: User with ID {UserId} not found.", id);
                    return Result.Fail("User not found.", HttpStatusCode.NotFound);
                }

                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    var errors = string.Join("; ", deleteResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to delete user {UserId}: {Errors}", id, errors);
                    return Result.Fail($"Failed to delete user: {errors}", HttpStatusCode.BadRequest);
                }

                _logger.LogInformation("User with ID {UserId} successfully deleted.", id);
                return await Result.SuccessAsync("User deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user {UserId}", id);
                return Result.Fail("An internal error occurred. Please try again later.", HttpStatusCode.InternalServerError);
            }
        }
    }
}
