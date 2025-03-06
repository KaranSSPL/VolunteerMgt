using Microsoft.AspNetCore.Identity;
using System.Net;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Role;
using VolunteerMgt.Server.Models.Wrapper;
using VolunteerMgt.Server.Persistence;

namespace VolunteerMgt.Server.Services
{
    public class RoleService(
        UserManager<ApplicationUser> _userManager,
        RoleManager<ApplicationRole> _roleManager,
        ILogger<RoleService> _logger,
        ICurrentUserService _currentUserService,
        VolunteerDataContext _dbContext
        ) : IRoleService
    {
        public async Task<Result<List<Role>>> GetRoleAsync()
        {
            try
            {
                var roles = await Task.Run(() => _roleManager.Roles.Select(r => new Role { Id = r.Id, Name = r.Name }).ToList());

                var permissionRoles = _dbContext.PermissionRoles.Where(x => roles.Select(r => r.Id).ToList().Contains(x.RoleId.ToString())).ToList();

                roles.ForEach(role =>
                {
                    var assignedPermissionIds = permissionRoles.Where(pr => pr.RoleId.ToString() == role.Id).Select(pr => pr.PermissionId).ToList();

                    role.Permissions = _dbContext.Permissions.Where(p => assignedPermissionIds.Contains(p.Id)).ToList();
                });

                if (roles == null || !roles.Any())
                {
                    return Result<List<Role>>.Fail("No roles found.", HttpStatusCode.NotFound);
                }

                return Result<List<Role>>.Success(roles, "Roles retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all roles.");
                return Result<List<Role>>.Fail("An internal error occurred.", HttpStatusCode.InternalServerError);
            }
        }
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
        public async Task<Result<AssignUser>> GetUserRolesAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return Result<AssignUser>.Fail("User ID cannot be empty.", HttpStatusCode.BadRequest);

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return Result<AssignUser>.Fail($"User with ID '{userId}' not found.", HttpStatusCode.NotFound);

                var roles = await _userManager.GetRolesAsync(user);
                var assignUser = new AssignUser()
                {
                    UserId = userId.ToString(),
                    RoleIds = roles.ToList()
                };
                return await Result<AssignUser>.SuccessAsync(assignUser, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching roles for user ID '{UserId}'.", userId);
                return Result<AssignUser>.Fail("An internal error occurred while fetching roles.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result<Role>> UpdateRoleAsync(Role updatedRole)
        {
            try
            {
                if (updatedRole == null || string.IsNullOrWhiteSpace(updatedRole.Id) || string.IsNullOrWhiteSpace(updatedRole.Name))
                    return Result<Role>.Fail("Role ID and Name cannot be empty.", HttpStatusCode.BadRequest);

                var existingRole = await _roleManager.FindByIdAsync(updatedRole.Id);
                if (existingRole == null)
                {
                    _logger.LogWarning("Role with ID '{RoleId}' not found.", updatedRole.Id);
                    return Result<Role>.Fail($"Role with ID '{updatedRole.Id}' not found.", HttpStatusCode.NotFound);
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
                    return Result<Role>.Fail("Failed to update the role.", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("Role with ID '{RoleId}' updated successfully by '{ModifiedBy}'.", updatedRole.Id, modifiedBy);
                return await Result<Role>.SuccessAsync(updatedRole, $"Role '{updatedRole.Name}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the role with ID '{RoleId}'.", updatedRole.Id);
                return Result<Role>.Fail("An internal error occurred while updating the role.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> DeleteRoleAsync(Guid roleId)
        {
            try
            {
                if (roleId == Guid.Empty)
                    return Result.Fail("Role ID cannot be empty.", HttpStatusCode.BadRequest);

                var role = await _roleManager.FindByIdAsync(roleId.ToString());
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
        public async Task<Result> RemoveUserRoleAsync(AssignUser assignUser)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assignUser.UserId) || assignUser.RoleIds == null || !assignUser.RoleIds.Any())
                    return Result.Fail("User ID and Role IDs cannot be empty.", HttpStatusCode.BadRequest);

                var user = await _userManager.FindByIdAsync(assignUser.UserId);
                if (user == null)
                    return Result.Fail($"User with ID '{assignUser.UserId}' not found.", HttpStatusCode.NotFound);

                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = new List<string>();

                foreach (var roleId in assignUser.RoleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    if (role != null && currentRoles.Contains(role.Name))
                    {
                        rolesToRemove.Add(role.Name);
                    }
                    else
                    {
                        _logger.LogWarning("Role with ID '{RoleId}' not found or not assigned to the user.", roleId);
                    }
                }

                if (!rolesToRemove.Any())
                    return Result.Fail("No valid roles found to remove.", HttpStatusCode.BadRequest);

                var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to remove roles from user '{UserId}'. Errors: {Errors}",
                                     assignUser.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return Result.Fail("Failed to remove roles from the user.", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("Successfully removed roles from user '{UserId}'. Removed roles: {Roles}",
                                       assignUser.UserId, string.Join(", ", rolesToRemove));
                return await Result.SuccessAsync($"Roles removed successfully from user {assignUser.UserId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing roles from user '{UserId}'.", assignUser.UserId);
                return Result.Fail("An internal error occurred while removing roles.", HttpStatusCode.InternalServerError);
            }
        }
    }
}
