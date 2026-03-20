using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models;

public class User
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required, MaxLength(15)]
    public string Phone { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public string Gender { get; set; } = "Male";
    
    public string Role { get; set; } = "Member";
    
    public bool IsFrozen { get; set; } = false;
    
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
