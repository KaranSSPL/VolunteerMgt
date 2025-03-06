using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using VolunteerMgt.Server.Abstraction.Mail;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.ChangePassword;
using VolunteerMgt.Server.Models.Edit;
using VolunteerMgt.Server.Models.ForgotPassword;
using VolunteerMgt.Server.Models.ResetPassword;
using VolunteerMgt.Server.Models.Volunteer;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Services
{
    public class VolunteerService(
        UserManager<ApplicationUser> _userManager,
        ICurrentUserService _currentUserService,
        IHttpContextAccessor _httpContextAccessor,
        IMailService _mailService,
        ILogger<VolunteerService> _logger) : IVolunteerService
    {
        public async Task<Result<List<VolunteerWithId>>> GetVolunteersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                if (!users.Any())
                {
                    return Result<List<VolunteerWithId>>.Fail("No users found.", HttpStatusCode.NotFound);
                }
                var volunteers = new List<VolunteerWithId>();

                foreach (var x in users)
                {
                    var roles = await _userManager.GetRolesAsync(x); // Ensure single-threaded access
                    volunteers.Add(new VolunteerWithId()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Username = x.UserName,
                        PhoneNumber = x.PhoneNumber,
                        Role = string.Join(", ", roles)
                    });
                }
                return await Result<List<VolunteerWithId>>.SuccessAsync(volunteers.ToList(), HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the users.");
                return Result<List<VolunteerWithId>>.Fail("An internal error occurred while fetching the users.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result<VolunteerWithId>> GetVolunteerByIdAsync(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    return Result<VolunteerWithId>.Fail("User not found, Please Login.", HttpStatusCode.BadRequest);
                }
                var user = await _userManager.FindByIdAsync(Id.ToString());

                var volunteer = new VolunteerWithId()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Role = string.Join(", ", await _userManager.GetRolesAsync(user))
                };

                if (user != null)
                {
                    return await Result<VolunteerWithId>.SuccessAsync(volunteer, HttpStatusCode.OK);
                }
                return Result<VolunteerWithId>.Fail("An internal error occurred while finding the user.", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while getting user {Email}");
                return Result<VolunteerWithId>.Fail("An internal error occurred while finding the user.", HttpStatusCode.InternalServerError);
            }
        }
        public async Task<Result<EditVolunteerModel>> UpdateVolunteerAsync(EditVolunteerModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return Result<EditVolunteerModel>.Fail("User not found.", HttpStatusCode.NotFound);
                }

                // Check if the email is changed and already exists
                if (user.Email != model.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        return Result<EditVolunteerModel>.Fail("Email is already registered.", HttpStatusCode.BadRequest);
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
                    return Result<EditVolunteerModel>.Fail(errorMessage, HttpStatusCode.BadRequest);
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

                return await Result<EditVolunteerModel>.SuccessAsync(model, "Successfully Updated User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating user {Email}.", model.Email);
                return Result<EditVolunteerModel>.Fail("An internal error occurred while updating the user.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Result.Fail("User with this email does not exist.", HttpStatusCode.NotFound);
                }

                // Generate reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var scheme = _httpContextAccessor?.HttpContext?.Request.Scheme;
                var host = _httpContextAccessor?.HttpContext?.Request.Host;

                var resetLink = $"{scheme}://{host}/api/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";


                // Send email
                var emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";
                await _mailService.SendEmailAsync(user.Email ?? string.Empty, "Password Reset Request", emailBody);

                return Result.Success("Password reset link has been sent to your email.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while while forgeting the users");
                return Result<List<ApplicationUser>>.Fail("An internal error occurred while forgeting the users.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result> ResetPasswordAsync(string email, string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Invalid reset attempt: Missing email or token.");
                    return Result.Fail("Invalid request. Email and token are required.", HttpStatusCode.BadRequest);
                }
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("Password reset attempt failed: User with email {Email} not found.", email);
                    return Result.Fail("User not found.", HttpStatusCode.NotFound);
                }

                var isTokenValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider,
                    UserManager<ApplicationUser>.ResetPasswordTokenPurpose, token);

                if (!isTokenValid)
                    return Result.Fail("Invalid or expired token.", HttpStatusCode.BadRequest);

                _logger.LogInformation("Password reset request valid for user: {Email}", email);
                return await Result.SuccessAsync("You can reset your password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the password reset for {Email}", email);
                return Result.Fail("An internal error occurred. Please try again later.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result> ResetPasswordPostAsync(ResetPasswordModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Token))
                {
                    _logger.LogWarning("Invalid reset attempt: Missing email or token.");
                    return Result.Fail("Invalid request. Email and token are required.", HttpStatusCode.BadRequest);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _logger.LogWarning("Password reset attempt failed: User with email {Email} not found.", model.Email);
                    return Result.Fail("User not found.", HttpStatusCode.NotFound);
                }

                var resetResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join("; ", resetResult.Errors.Select(e => e.Description));
                    _logger.LogError("Password reset failed for {Email}: {Errors}", model.Email, errors);
                    return Result.Fail($"Failed to reset password: {errors}", HttpStatusCode.BadRequest);
                }
                var emailBody = $"Password reset successfully";
                await _mailService.SendEmailAsync(user.Email ?? string.Empty, "Password Reset", emailBody);

                _logger.LogInformation("Password successfully reset for {Email}", model.Email);
                return await Result.SuccessAsync("Password reset successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the password reset for {Email}", model.Email);
                return Result.Fail("An internal error occurred. Please try again later.", HttpStatusCode.InternalServerError);
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

    }
}
