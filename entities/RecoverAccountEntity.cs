using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blog.entities.enums;

namespace Blog.entities
{
    [Table("recover_account")]
    public class RecoverAccountEntity
    {
        [Key] public string ApplicationUserId { get; set; } = string.Empty;

        [Required] public string Token { get; set; } = string.Empty;
        [Required] public DateTime ExpireAt { get; set; }
        public DateTime? BlockedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; } = false; 
        public int FailedAttempts { get; set; } = 0;
        public string? RequestIpAddress { get; set; }
        public string? RequestUserAgent { get; set; }

        [Required] public RecoveryMethodEnum Method { get; set; } = RecoveryMethodEnum.Email;

        [Required] public RecoveryReasonEnum Reason { get; set; } = RecoveryReasonEnum.ForgotPassword; 

        [JsonIgnore] public virtual ApplicationUser? User { get; set; }
    }
}