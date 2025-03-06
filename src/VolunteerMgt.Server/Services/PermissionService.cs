using Microsoft.EntityFrameworkCore;
using System.Net;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Wrapper;
using VolunteerMgt.Server.Persistence;

namespace VolunteerMgt.Server.Services
{
    public class PermissionService(
        VolunteerDataContext _dbContext,
        ILogger<PermissionService> _logger
        ) : IPermissionService
    {
        public async Task<Result<List<Permission>>> GetPermissionAsync()
        {
            try
            {
                var permissions = await _dbContext.Permissions.ToListAsync();

                if (!permissions.Any())
                {
                    _logger.LogWarning("No permissions found in the database.");
                    return Result<List<Permission>>.Fail("No permissions found.", HttpStatusCode.NotFound);
                }

                _logger.LogInformation("Successfully retrieved {Count} permissions.", permissions.Count);
                return Result<List<Permission>>.Success(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving permissions.");
                return Result<List<Permission>>.Fail("An internal error occurred while fetching permissions.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result<Permission>> GetPermissionByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return Result<Permission>.Fail("Invalid Permission ID.", HttpStatusCode.BadRequest);

                var permission = await _dbContext.Permissions.FindAsync(id);

                if (permission == null)
                {
                    _logger.LogWarning("No permission found in the database.");
                    return Result<Permission>.Fail("No permission found.", HttpStatusCode.NotFound);
                }

                _logger.LogInformation("Successfully retrieved permission.");
                return Result<Permission>.Success(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving permission.");
                return Result<Permission>.Fail("An internal error occurred while fetching permission.", HttpStatusCode.InternalServerError);
            }
        }
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

    }
}
