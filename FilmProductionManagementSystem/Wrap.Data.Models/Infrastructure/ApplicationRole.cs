namespace Wrap.Data.Models.Infrastructure;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationRole : IdentityRole<Guid>
{
    // [Required] 
    // [DataType(DataType.Text)]
    // [MaxLength(100)]
    // public string Label { get; set; } = null!;
}