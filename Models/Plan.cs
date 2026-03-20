using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models;

public class Plan
{
    public int Id { get; set; }
    
    [Required, MaxLength(50)]
    public string PlanName { get; set; } = string.Empty;
    
    [Range(1, 120)]
    public int DurationMonths { get; set; }
    
    [Range(0.01, 999999)]
    public decimal Price { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
