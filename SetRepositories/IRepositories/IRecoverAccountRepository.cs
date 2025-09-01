using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.DTOs.RecoverAccount;
using Blog.entities;

namespace Blog.SetRepositories.IRepositories
{
    public interface IRecoverAccountRepository
    {
        Task<RecoverAccountEntity> RequestPasswordResetTokenAsync(RequestPasswordResetDto requestDto, string callbackUrl, ApplicationUser user);
        Task<bool> ValidateAndResetPasswordAsync(ResetPasswordDto resetDto, ApplicationUser user,RecoverAccountEntity recoveryEntry);
        Task SendWelcomeEmailAsync(WelcomeEmailDto welcomeDto);
        Task<RecoverAccountEntity?> GetAsync(ApplicationUser user);
    }
}