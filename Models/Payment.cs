namespace GymManagement.Models;

public class Payment
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int MembershipId { get; set; }
    public Membership Membership { get; set; } = null!;
    
    public decimal Amount { get; set; }
    
    public string Type { get; set; } = string.Empty;
    
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}
