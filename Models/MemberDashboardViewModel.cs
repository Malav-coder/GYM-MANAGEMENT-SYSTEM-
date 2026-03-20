namespace GymManagement.Models;

public class MemberDashboardViewModel
{
    public User User { get; set; } = null!;
    public Membership? ActiveMembership { get; set; }
    public int DaysRemaining { get; set; }
    public List<Payment> RecentPayments { get; set; } = new();
}
