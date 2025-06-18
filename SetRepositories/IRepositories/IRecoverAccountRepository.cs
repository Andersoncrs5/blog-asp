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
        Task<RecoverAccountEntity> RequestPasswordResetTokenAsync(RequestPasswordResetDto requestDto, string callbackUrl);
        Task<bool> ValidateAndResetPasswordAsync(ResetPasswordDto resetDto);
        Task SendWelcomeEmailAsync(WelcomeEmailDto welcomeDto);
    }
}