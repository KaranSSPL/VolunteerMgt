using Microsoft.AspNetCore.Identity;
using System.Net;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Role;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Services
{
    public class RoleService(
        UserManager<ApplicationUser> _userManager,
        RoleManager<ApplicationRole> _roleManager,
        ILogger<RoleService> _logger,
        ICurrentUserService _currentUserService
        ) : IRoleService
    {
        public async Task<Result> AddRoleAsync(string role)
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                {
                    return Result.Fail("Role Name cannot be empty.", HttpStatusCode.BadRequest);
                }

                if (await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogWarning("Role '{Role}' already exists.", role);
                    return Result.Fail($"Role '{role}' already exists.", HttpStatusCode.BadRequest);
                }
                var currentUser = _currentUserService.GetUserEmail();

                var CreatedBy = _userManager.FindByEmailAsync(currentUser!).Result?.Id ?? string.Empty;
                var newRole = new ApplicationRole
                {
                    Name = role,
                    CreatedBy = CreatedBy,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _roleManager.CreateAsync(newRole);

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create role '{Role}'. Errors: {Errors}",
                                    role, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return Result.Fail("Failed to create the role.", HttpStatusCode.InternalServerError);
                }
                return await Result.SuccessAsync("Successfully Created User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating Role.");
                return Result.Fail("An internal error occurred while creating the Role.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> DeleteRoleAsync(string roleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return Result.Fail("Role ID cannot be empty.", HttpStatusCode.BadRequest);

                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    _logger.LogWarning("Role with ID '{RoleId}' not found.", roleId);
                    return Result.Fail($"Role with ID '{roleId}' not found.", HttpStatusCode.NotFound);
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to delete role with ID '{RoleId}'. Errors: {Errors}",
                                    roleId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return Result.Fail("Failed to delete the role.", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("Role with ID '{RoleId}' deleted successfully.", roleId);
                return await Result.SuccessAsync($"Role with ID '{role.Name}' deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the role with ID '{RoleId}'.", roleId);
                return Result.Fail("An internal error occurred while deleting the role.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> UpdateRoleAsync(Role updatedRole)
        {
            try
            {
                if (updatedRole == null || string.IsNullOrWhiteSpace(updatedRole.Id) || string.IsNullOrWhiteSpace(updatedRole.Name))
                    return Result.Fail("Role ID and Name cannot be empty.", HttpStatusCode.BadRequest);

                var existingRole = await _roleManager.FindByIdAsync(updatedRole.Id);
                if (existingRole == null)
                {
                    _logger.LogWarning("Role with ID '{RoleId}' not found.", updatedRole.Id);
                    return Result.Fail($"Role with ID '{updatedRole.Id}' not found.", HttpStatusCode.NotFound);
                }
                var currentUser = _currentUserService.GetUserEmail();
                var modifiedBy = _userManager.FindByEmailAsync(currentUser!).Result?.Id ?? string.Empty;

                existingRole.Name = updatedRole.Name;

                if (existingRole is ApplicationRole appRole)
                {
                    appRole.ModifiedBy = modifiedBy;
                    appRole.ModifiedDate = DateTime.Now;
                }

                var result = await _roleManager.UpdateAsync(existingRole);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update role with ID '{RoleId}'. Errors: {Errors}",
                                     updatedRole.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return Result.Fail("Failed to update the role.", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("Role with ID '{RoleId}' updated successfully by '{ModifiedBy}'.", updatedRole.Id, modifiedBy);
                return await Result.SuccessAsync($"Role '{updatedRole.Name}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the role with ID '{RoleId}'.", updatedRole.Id);
                return Result.Fail("An internal error occurred while updating the role.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> AssignUserRoleAsync(AssignUser assignUser)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assignUser.UserId) || assignUser.RoleIds == null || !assignUser.RoleIds.Any())
                    return Result.Fail("User ID and Role IDs cannot be empty.", HttpStatusCode.BadRequest);

                var user = await _userManager.FindByIdAsync(assignUser.UserId);
                if (user == null)
                    return Result.Fail($"User with ID '{assignUser.UserId}' not found.", HttpStatusCode.NotFound);

                var roleNames = new List<string>();

                foreach (var roleId in assignUser.RoleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    if (role != null)
                    {
                        roleNames.Add(role.Name);
                    }
                    else
                    {
                        _logger.LogWarning("Role with ID '{RoleId}' not found.", roleId);
                    }
                }

                if (!roleNames.Any())
                    return Result.Fail("No valid roles found to assign.", HttpStatusCode.BadRequest);

                var result = await _userManager.AddToRolesAsync(user, roleNames);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to assign roles to user '{UserId}'. Errors: {Errors}",
                                     assignUser.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return Result.Fail("Failed to assign roles to the user.", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("Roles assigned to user '{UserId}' successfully: {Roles}",
                                       assignUser.UserId, string.Join(", ", roleNames));
                return await Result.SuccessAsync($"Roles assigned successfully to user {assignUser.UserId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning roles to user '{UserId}'", assignUser.UserId);
                return Result.Fail("An internal error occurred while assigning roles.", HttpStatusCode.InternalServerError);
            }
        }

    }
}
