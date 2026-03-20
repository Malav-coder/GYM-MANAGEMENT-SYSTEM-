namespace GymManagement.Models;

public class Membership
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int PlanId { get; set; }
    public Plan Plan { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
