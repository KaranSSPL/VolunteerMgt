using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using VolunteerMgt.Server.Abstraction.Mail;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.PasswordModel;
using VolunteerMgt.Server.Models.User;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Services
{
    public class UserService(
        UserManager<ApplicationUser> _userManager,
        ICurrentUserService _currentUserService,
        IMailService _mailService,
        RoleManager<ApplicationRole> _roleManager,
        SignInManager<ApplicationUser> _signInManager,
        ILogger<UserService> _logger) : IUserService
    {
        public async Task<Result<List<UserWithId>>> GetUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                if (!users.Any())
                {
                    return Result<List<UserWithId>>.Fail("No users found.", HttpStatusCode.NotFound);
                }
                var userWithId = new List<UserWithId>();

                foreach (var x in users)
                {
                    var roles = await _userManager.GetRolesAsync(x); // Ensure single-threaded access
                    userWithId.Add(new UserWithId()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Username = x.UserName,
                        PhoneNumber = x.PhoneNumber,
                        Role = string.Join(", ", roles)
                    });
                }
                return await Result<List<UserWithId>>.SuccessAsync(userWithId.ToList(), HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the users.");
                return Result<List<UserWithId>>.Fail("An internal error occurred while fetching the users.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result<UserWithId>> GetUserByIdAsync(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    return Result<UserWithId>.Fail("User not found, Please Login.", HttpStatusCode.BadRequest);
                }
                var user = await _userManager.FindByIdAsync(Id.ToString());

                var userWithid = new UserWithId()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Role = string.Join(", ", await _userManager.GetRolesAsync(user))
                };

                if (user != null)
                {
                    return await Result<UserWithId>.SuccessAsync(userWithid, HttpStatusCode.OK);
                }
                return Result<UserWithId>.Fail("An internal error occurred while finding the user.", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while getting user {Email}");
                return Result<UserWithId>.Fail("An internal error occurred while finding the user.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result<EditUserModel>> UpdateUserAsync(EditUserModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return Result<EditUserModel>.Fail("User not found.", HttpStatusCode.NotFound);
                }

                // Check if the email is changed and already exists
                if (user.Email != model.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        return Result<EditUserModel>.Fail("Email is already registered.", HttpStatusCode.BadRequest);
                    }
                }
                if (user.UserName != model.Username)
                {
                    var existingUser = await _userManager.FindByNameAsync(model.Username);
                    if (existingUser != null)
                    {
                        return Result<EditUserModel>.Fail("Username is already registered.", HttpStatusCode.BadRequest);
                    }
                }

                var currentUser = _currentUserService.GetUserName();

                user.UserName = model.Username;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = currentUser ?? model.Username;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errorMessage = result.Errors.Select(e => e.Description).ToList();
                    return Result<EditUserModel>.Fail(errorMessage, HttpStatusCode.BadRequest);
                }

                if (!string.IsNullOrEmpty(model.Role))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (!currentRoles.Contains(model.Role))
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                }

                return await Result<EditUserModel>.SuccessAsync(model, "Successfully Updated User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating user {Email}.", model.Email);
                return Result<EditUserModel>.Fail("An internal error occurred while updating the user.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.OldPassword) || string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    _logger.LogWarning("Invalid change password attempt: Missing required fields.");
                    return Result.Fail("Invalid request. Email, old password, and new password are required.", HttpStatusCode.BadRequest);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _logger.LogWarning("Change password attempt failed: User with email {Email} not found.", model.Email);
                    return Result.Fail("User not found.", HttpStatusCode.NotFound);
                }

                // Verify the old password before proceeding
                var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                if (!isOldPasswordValid)
                {
                    _logger.LogWarning("Change password attempt failed: Incorrect old password for {Email}.", model.Email);
                    return Result.Fail("Incorrect old password.", HttpStatusCode.Unauthorized);
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    var errors = string.Join("; ", changePasswordResult.Errors.Select(e => e.Description));
                    _logger.LogError("Password change failed for {Email}: {Errors}", model.Email, errors);
                    return Result.Fail($"Failed to change password: {errors}", HttpStatusCode.BadRequest);
                }
                var emailBody = $"Password successfully changed";
                await _mailService.SendEmailAsync(user.Email ?? string.Empty, "Password Change", emailBody);

                _logger.LogInformation("Password successfully changed for {Email}", model.Email);
                return await Result.SuccessAsync("Password changed successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the password change for {Email}", model.Email);
                return Result.Fail("An internal error occurred. Please try again later.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result<RegisterUserModel>> AddUserAsync(RegisterUserModel model)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    return Result<RegisterUserModel>.Fail("Email is already registered.", HttpStatusCode.BadRequest);
                }
                if (await _userManager.FindByNameAsync(model.Username) != null)
                {
                    return Result<RegisterUserModel>.Fail("Username is already registered.", HttpStatusCode.BadRequest);
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
                    return Result<RegisterUserModel>.Fail(errorMessage, HttpStatusCode.BadRequest);
                }
                var roleExists = await _roleManager.RoleExistsAsync(model.Role);

                if (!roleExists)
                {
                    _logger.LogWarning("Role '{Role}' does not exist.", model.Role);
                    return Result<RegisterUserModel>.Fail($"Role '{model.Role}' does not exist.", HttpStatusCode.BadRequest);
                }

                await _userManager.AddToRoleAsync(user, model.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return await Result<RegisterUserModel>.SuccessAsync(model, "Successfully Created User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating user {Email}.", model.Email);
                return Result<RegisterUserModel>.Fail("An internal error occurred while creating the user.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result<UserRoleMapping>> GetUserRolesAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return Result<UserRoleMapping>.Fail("User ID cannot be empty.", HttpStatusCode.BadRequest);

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return Result<UserRoleMapping>.Fail($"User with ID '{userId}' not found.", HttpStatusCode.NotFound);

                var roles = await _userManager.GetRolesAsync(user);
                var assignUser = new UserRoleMapping()
                {
                    UserId = userId.ToString(),
                    RoleId = roles.First()
                };
                return await Result<UserRoleMapping>.SuccessAsync(assignUser, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching roles for user ID '{UserId}'.", userId);
                return Result<UserRoleMapping>.Fail("An internal error occurred while fetching roles.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> AssignRoleAsync(UserRoleMapping userRole)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userRole.UserId) || string.IsNullOrWhiteSpace(userRole.RoleId))
                    return Result.Fail("User ID and Role IDs cannot be empty.", HttpStatusCode.BadRequest);

                var user = await _userManager.FindByIdAsync(userRole.UserId);
                if (user == null)
                    return Result.Fail($"User with ID '{userRole.UserId}' not found.", HttpStatusCode.NotFound);

                var role = await _roleManager.FindByIdAsync(userRole.RoleId);

                if (role == null)
                    return Result.Fail("Role not found.", HttpStatusCode.BadRequest);

                var currentRoles = await _userManager.GetRolesAsync(user);

                if (currentRoles.Contains(role.Name!))
                    return Result.Fail($"User '{userRole.UserId}' already has the role '{role.Name}'.", HttpStatusCode.Conflict);

                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        return Result.Fail("Failed to remove existing roles.", HttpStatusCode.InternalServerError);
                }

                var result = await _userManager.AddToRolesAsync(user, new List<string> { role.Name! });
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to assign role to user '{UserId}'. Errors: {Errors}", userRole.UserId,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return Result.Fail("Failed to assign role to the user.", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("Role assigned to user '{UserId}' successfully: {Roles}", userRole.UserId, role.Name);
                return await Result.SuccessAsync($"Role assigned successfully to user {userRole.UserId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning role to user '{UserId}'", userRole.UserId);
                return Result.Fail("An internal error occurred while assigning role.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> RemoveRoleAsync(UserRoleMapping userRole)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userRole.UserId) || userRole.RoleId == null || !userRole.RoleId.Any())
                    return Result.Fail("User ID and Role IDs cannot be empty.", HttpStatusCode.BadRequest);

                var user = await _userManager.FindByIdAsync(userRole.UserId);
                if (user == null)
                    return Result.Fail($"User with ID '{userRole.UserId}' not found.", HttpStatusCode.NotFound);

                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = new List<string>();

                var role = await _roleManager.FindByIdAsync(userRole.RoleId);

                if (role != null && currentRoles.Contains(role.Name))
                {
                    rolesToRemove.Add(role.Name);
                }
                else
                {
                    _logger.LogWarning("Role with ID '{RoleId}' not found or not assigned to the user.", userRole.RoleId);
                }

                if (!rolesToRemove.Any())
                    return Result.Fail("No valid role found to remove.", HttpStatusCode.BadRequest);

                var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to remove role from user '{UserId}'. Errors: {Errors}",
                                     userRole.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return Result.Fail("Failed to remove roles from the user.", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("Successfully removed role from user '{UserId}'. Removed roles: {Roles}",
                                       userRole.UserId, string.Join(", ", rolesToRemove));
                return await Result.SuccessAsync($"Roles removed successfully from user {userRole.UserId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing role from user '{UserId}'.", userRole.UserId);
                return Result.Fail("An internal error occurred while removing roles.", HttpStatusCode.InternalServerError);
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
