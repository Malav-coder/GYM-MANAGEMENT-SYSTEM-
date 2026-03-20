namespace GymManagement.Models;

public class AdminDashboardViewModel
{
    public int TotalMembers { get; set; }
    public int ActiveMemberships { get; set; }
    public int ExpiredMemberships { get; set; }
    public int CancelledMemberships { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<Membership> ExpiringThisWeek { get; set; } = new();
}
