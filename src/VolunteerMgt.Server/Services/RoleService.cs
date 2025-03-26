using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models;
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
        public async Task<Result<List<Role>>> GetRolesAsync()
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
        public async Task<Result> AddRoleAsync(Role role)
        {
            try
            {
                if (role == null)
                {
                    return Result.Fail("Role cannot be empty.", HttpStatusCode.BadRequest);
                }

                if (await _roleManager.RoleExistsAsync(role.Name))
                {
                    _logger.LogWarning("Role '{Role}' already exists.", role);
                    return Result.Fail($"Role '{role}' already exists.", HttpStatusCode.Conflict);
                }
                var currentUser = _currentUserService.GetUserEmail();

                var CreatedBy = _userManager.FindByEmailAsync(currentUser!).Result?.Id ?? string.Empty;
                var newRole = new ApplicationRole
                {
                    Name = role.Name,
                    CreatedBy = CreatedBy,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _roleManager.CreateAsync(newRole);

                if (role.Permissions?.Any() == true)
                {
                    var createdRole = await _roleManager.FindByNameAsync(newRole.Name);

                    var Permissios = role.Permissions.Select(x => x.Id).ToList();

                    var permissionRole = Permissios.Select(x => new PermissionRoles
                    {
                        Id = Guid.NewGuid(),
                        RoleId = Guid.Parse(createdRole?.Id!),
                        PermissionId = x
                    }).ToList();

                    await _dbContext.PermissionRoles.AddRangeAsync(permissionRole);
                    await _dbContext.SaveChangesAsync();
                }
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
        public async Task<Result<Role>> GetRolesbyIdAsync(Guid roleId)
        {
            try
            {
                if (roleId == Guid.Empty)
                    return Result<Role>.Fail("Role ID cannot be empty.", HttpStatusCode.BadRequest);

                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role == null)
                    return Result<Role>.Fail($"Role '{roleId}' not found.", HttpStatusCode.NotFound);

                var permissionRoles = await _dbContext.PermissionRoles.Where(x => x.RoleId == roleId).Select(pr => pr.PermissionId).ToListAsync();

                if (!permissionRoles.Any())
                    return Result<Role>.Fail($"No permissions found for role ID '{roleId}'.", HttpStatusCode.NotFound);

                var permissions = await _dbContext.Permissions.Where(p => permissionRoles.Contains(p.Id)).ToListAsync();

                return await Result<Role>.SuccessAsync(new Role
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    Permissions = permissions
                }, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching permissions for role ID '{RoleId}'.", roleId);
                return Result<Role>.Fail("An internal error occurred while fetching permissions.", HttpStatusCode.InternalServerError);
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
                var permissionRole = _dbContext.PermissionRoles.Where(x => x.RoleId == Guid.Parse(role.Id)).ToList();
                _dbContext.PermissionRoles.RemoveRange(permissionRole);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Role with ID '{RoleId}' deleted successfully.", roleId);
                return await Result.SuccessAsync($"Role with ID '{role.Name}' deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the role with ID '{RoleId}'.", roleId);
                return Result.Fail("An internal error occurred while deleting the role.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result> AssignPermissionAsync(PermissionRolesDto permissionRoles)
        {
            try
            {
                if (permissionRoles == null || permissionRoles.PermissionId == Guid.Empty || permissionRoles.RoleId == Guid.Empty)
                    return Result.Fail("Invalid Permission ID or Role ID.", HttpStatusCode.BadRequest);

                var permissions = await _dbContext.Permissions.AnyAsync(p => p.Id == permissionRoles.PermissionId);
                if (!permissions)
                    return Result.Fail($"Permission with ID '{permissionRoles.PermissionId}' not found.", HttpStatusCode.NotFound);

                var roles = await _dbContext.Roles.AnyAsync(r => r.Id == permissionRoles.RoleId.ToString());
                if (!roles)
                    return Result.Fail($"Role with ID '{permissionRoles.RoleId}' not found.", HttpStatusCode.NotFound);

                var existingPermissionRoles = await _dbContext.PermissionRoles
                    .AnyAsync(pr => pr.PermissionId == permissionRoles.PermissionId && pr.RoleId == permissionRoles.RoleId);

                if (existingPermissionRoles)
                {
                    _logger.LogWarning("Permission '{PermissionId}' is already assigned to Role '{RoleId}'.", permissionRoles.PermissionId, permissionRoles.RoleId);
                    return Result.Fail("This permission is already assigned to the specified role.", HttpStatusCode.Conflict);
                }

                var newPermissionRole = new PermissionRoles
                {
                    Id = Guid.NewGuid(),
                    PermissionId = permissionRoles.PermissionId,
                    RoleId = permissionRoles.RoleId
                };

                await _dbContext.PermissionRoles.AddAsync(newPermissionRole);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully assigned Permission '{PermissionId}' to Role '{RoleId}'.", newPermissionRole.PermissionId, newPermissionRole.RoleId);

                return await Result.SuccessAsync("Permission successfully assigned to the role.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning Permission '{PermissionId}' to Role '{RoleId}'.",
                                 permissionRoles?.PermissionId, permissionRoles?.RoleId);
                return Result.Fail("An internal error occurred while assigning the permission.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> RemovePermissionAsync(PermissionRolesDto permissionRoles)
        {
            try
            {
                if (permissionRoles == null || permissionRoles.PermissionId == Guid.Empty || permissionRoles.RoleId == Guid.Empty)
                    return Result.Fail("Invalid Role ID or permissions list.", HttpStatusCode.BadRequest);

                var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == permissionRoles.RoleId.ToString());
                if (!roleExists)
                    return Result.Fail($"Role with ID '{permissionRoles.RoleId.ToString()}' not found.", HttpStatusCode.NotFound);

                var existingPermissionRoles = _dbContext.PermissionRoles.Where(pr => pr.RoleId == permissionRoles.RoleId && pr.PermissionId == permissionRoles.PermissionId);

                _dbContext.PermissionRoles.RemoveRange(existingPermissionRoles);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully Removed permissions for Role '{RoleId}'.", permissionRoles.RoleId.ToString());

                return await Result.SuccessAsync("Permissions Removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing permissions for Role '{RoleId}'.", permissionRoles.RoleId.ToString());
                return Result.Fail("An internal error occurred while removing role permissions.", HttpStatusCode.InternalServerError);
            }
        }

    }
}
