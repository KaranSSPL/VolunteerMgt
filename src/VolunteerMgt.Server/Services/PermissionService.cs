using Microsoft.EntityFrameworkCore;
using System.Net;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Permission;
using VolunteerMgt.Server.Models.Wrapper;
using VolunteerMgt.Server.Persistence;

namespace VolunteerMgt.Server.Services
{
    public class PermissionService(
        VolunteerDataContext _dbContext,
        ILogger<PermissionService> _logger
        ) : IPermissionService
    {
        public async Task<Result> AddPermissionAsync(Permission permission)
        {
            try
            {
                if (permission == null || string.IsNullOrWhiteSpace(permission.Name))
                    return Result.Fail("Permission name cannot be empty.", HttpStatusCode.BadRequest);

                var existingPermission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Name.ToLower() == permission.Name.ToLower());

                if (existingPermission != null)
                {
                    _logger.LogWarning("Permission '{PermissionName}' already exists.", permission.Name);
                    return Result.Fail($"Permission '{permission.Name}' already exists.", HttpStatusCode.Conflict);
                }

                var newPermission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = permission.Name.Trim()
                };

                await _dbContext.Permissions.AddAsync(newPermission);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully added permission '{PermissionName}' with ID '{PermissionId}'.",
                                        newPermission.Name, newPermission.Id);

                return await Result.SuccessAsync($"Permission '{newPermission.Name}' added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding permission '{PermissionName}'.", permission?.Name);
                return Result.Fail("An internal error occurred while adding the permission.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result<Permission>> UpdatePermissionAsync(Permission permission)
        {
            try
            {
                if (permission == null || permission.Id == Guid.Empty || string.IsNullOrWhiteSpace(permission.Name))
                    return Result<Permission>.Fail("Invalid permission data.", HttpStatusCode.BadRequest);

                var existingPermission = await _dbContext.Permissions.FindAsync(permission.Id);
                if (existingPermission == null)
                    return Result<Permission>.Fail($"Permission with ID '{permission.Id}' not found.", HttpStatusCode.NotFound);

                var duplicatePermission = await _dbContext.Permissions
                    .AnyAsync(p => p.Id != permission.Id && p.Name.ToLower() == permission.Name.ToLower());

                if (duplicatePermission)
                {
                    _logger.LogWarning("Permission name '{PermissionName}' is already in use by another record.", permission.Name);
                    return Result<Permission>.Fail($"Permission '{permission.Name}' already exists.", HttpStatusCode.Conflict);
                }
                existingPermission.Name = permission.Name.Trim();

                _dbContext.Permissions.Update(existingPermission);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully updated permission '{PermissionName}' with ID '{PermissionId}'.",
                                       existingPermission.Name, existingPermission.Id);

                return await Result<Permission>.SuccessAsync(existingPermission, "Permission updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating permission '{PermissionName}'.", permission?.Name);
                return Result<Permission>.Fail("An internal error occurred while updating the permission.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result> DeletePermissionAsync(Guid permissionId)
        {
            try
            {
                if (permissionId == Guid.Empty)
                    return Result.Fail("Invalid Permission ID.", HttpStatusCode.BadRequest);

                var permission = await _dbContext.Permissions.FindAsync(permissionId);
                if (permission == null)
                    return Result.Fail($"Permission with ID '{permissionId}' not found.", HttpStatusCode.NotFound);

                var isAssigned = await _dbContext.PermissionRoles.AnyAsync(pr => pr.PermissionId == permissionId);
                if (isAssigned)
                    return Result.Fail("Cannot delete permission because it is assigned to one or more roles.", HttpStatusCode.Conflict);

                _dbContext.Permissions.Remove(permission);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully removed permission '{PermissionName}' with ID '{PermissionId}'.",
                                        permission.Name, permission.Id);

                return await Result.SuccessAsync($"Permission '{permission.Name}' removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing permission with ID '{PermissionId}'.", permissionId);
                return Result.Fail("An internal error occurred while removing the permission.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result> AssignPermissionRoleAsync(PermissionRolesModel permissionRoles)
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
        public async Task<Result> RemovePermissionRoleAsync(PermissionRolesModel permissionRoles)
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
