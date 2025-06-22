using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blog.DTOs.Preference
{
    public class CreatePreferenceDTO
    {
        [Required] public long CategoryId { get; set; }
    }
}