using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Context;
using Blog.DTOs.RecoverAccount;
using Blog.entities;
using Blog.entities.enums;
using Blog.SetRepositories.IRepositories;
using Blog.SetServices.IServices;
using Blog.utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.SetRepositories.Repositories
{
    public class RecoverAccountRepository: IRecoverAccountRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public RecoverAccountRepository(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<RecoverAccountEntity?> GetAsync(ApplicationUser user)
        {
            RecoverAccountEntity? recoveryEntry = await _context.RecoverAccountEntities
                .FirstOrDefaultAsync(ra => ra.ApplicationUserId == user.Id);

            if (recoveryEntry == null) 
                return null;

            return recoveryEntry;
        }

        public async Task<RecoverAccountEntity> RequestPasswordResetTokenAsync(RequestPasswordResetDto requestDto, string callbackUrl, ApplicationUser user)
        {
            if (!new EmailAddressAttribute().IsValid(requestDto.Email))
                throw new ArgumentNullException();

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            RecoverAccountEntity? existingRecovery = await _context.RecoverAccountEntities
                .FirstOrDefaultAsync(ra => ra.ApplicationUserId == user.Id);

            if (existingRecovery != null)
            {
                existingRecovery.Token = token;
                existingRecovery.ExpireAt = DateTime.UtcNow.AddHours(24); 
                existingRecovery.CreatedAt = DateTime.UtcNow;
                existingRecovery.IsUsed = false; 
                existingRecovery.FailedAttempts = 0; 
                existingRecovery.BlockedAt = null; 
                existingRecovery.RequestIpAddress = requestDto.RequestIpAddress; 
                existingRecovery.RequestUserAgent = requestDto.RequestUserAgent; 
                existingRecovery.Method = RecoveryMethodEnum.Email; 
                existingRecovery.Reason = RecoveryReasonEnum.ForgotPassword; 

                _context.RecoverAccountEntities.Update(existingRecovery);
            }
            else
            {
                existingRecovery = new RecoverAccountEntity
                {
                    ApplicationUserId = user.Id,
                    Token = token,
                    ExpireAt = DateTime.UtcNow.AddHours(24), 
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false,
                    FailedAttempts = 0,
                    BlockedAt = null,
                    RequestIpAddress = requestDto.RequestIpAddress, 
                    RequestUserAgent = requestDto.RequestUserAgent,
                    Method = RecoveryMethodEnum.Email,
                    Reason = RecoveryReasonEnum.ForgotPassword
                };
                await _context.RecoverAccountEntities.AddAsync(existingRecovery);
            }

            await _context.SaveChangesAsync();

            await _emailService.SendPasswordResetEmailAsync(user.Email!, user.UserName!, token, callbackUrl); // user.Email and user.UserName are not null if user is found

            return existingRecovery;
        }

        public async Task<bool> ValidateAndResetPasswordAsync(
            ResetPasswordDto resetDto, 
            ApplicationUser user,
            RecoverAccountEntity recoveryEntry
            )
        {
            if (recoveryEntry.BlockedAt != null && recoveryEntry.BlockedAt > DateTime.UtcNow)
            {
                recoveryEntry.FailedAttempts++;
                _context.RecoverAccountEntities.Update(recoveryEntry);
                await _context.SaveChangesAsync();
                throw new ResponseException("This recovery token is temporarily blocked due to too many failed attempts. Please try again later.", 403);
            }

            if (recoveryEntry.IsUsed)
            {
                recoveryEntry.FailedAttempts++; 
                _context.RecoverAccountEntities.Update(recoveryEntry);
                await _context.SaveChangesAsync();
                throw new ResponseException("This password reset token has already been used.", 400);
            }

            if (recoveryEntry.ExpireAt < DateTime.UtcNow)
            {
                recoveryEntry.FailedAttempts++; 
                _context.RecoverAccountEntities.Update(recoveryEntry);
                await _context.SaveChangesAsync();
                throw new ResponseException("This password reset token has expired.", 400);
            }

            if (recoveryEntry.Token != resetDto.Token)
            {
                recoveryEntry.FailedAttempts++;
                
                if (recoveryEntry.FailedAttempts >= 5)
                {
                    recoveryEntry.BlockedAt = DateTime.UtcNow.AddMinutes(30);
                    throw new ResponseException("Too many failed attempts. This token is temporarily blocked.", 403);
                }
                _context.RecoverAccountEntities.Update(recoveryEntry);
                await _context.SaveChangesAsync();
                throw new ResponseException("Invalid password reset token.", 400);
            }

            if (resetDto.NewPassword != resetDto.ConfirmPassword)
                throw new ArgumentException("New password and confirmation password do not match.");

            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetDto.Token, resetDto.NewPassword);

            if (result.Succeeded)
            {
                recoveryEntry.IsUsed = true;
                _context.RecoverAccountEntities.Update(recoveryEntry);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                string errors = string.Join(" ", result.Errors.Select(e => e.Description));
                recoveryEntry.FailedAttempts++;
                _context.RecoverAccountEntities.Update(recoveryEntry);
                await _context.SaveChangesAsync();
                throw new Exception($"Password reset failed: {errors}");
            }
        }

        public async Task SendWelcomeEmailAsync(WelcomeEmailDto welcomeDto)
        {
            if (!new EmailAddressAttribute().IsValid(welcomeDto.Email))
                throw new ResponseException("Invalid email format for welcome email.", 400);

            try
            {
                await _emailService.SendWelcomeEmailAsync(welcomeDto.Email, welcomeDto.Username);
            }
            catch (Exception ex)
            {
                throw new ResponseException($"Failed to send welcome email. Please contact support. Error: {ex.Message}", 500);
            }
        }
    }
}