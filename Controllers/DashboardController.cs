using GymManagement.Data;
using GymManagement.Filters;
using GymManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers;

[ServiceFilter(typeof(AdminOnlyFilter))]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var viewModel = new AdminDashboardViewModel
        {
            TotalMembers = await _context.Users.CountAsync(u => u.Role == "Member"),
            ActiveMemberships = await _context.Memberships.CountAsync(m => m.Status == "Active"),
            ExpiredMemberships = await _context.Memberships.CountAsync(m => m.Status == "Expired"),
            CancelledMemberships = await _context.Memberships.CountAsync(m => m.Status == "Cancelled"),
            TotalRevenue = await _context.Payments
                .Where(p => p.Type == "Subscription" || p.Type == "Renewal")
                .SumAsync(p => p.Amount),
            ExpiringThisWeek = await _context.Memberships
                .Include(m => m.User)
                .Include(m => m.Plan)
                .Where(m => m.Status == "Active" && 
                           m.EndDate >= DateTime.Today && 
                           m.EndDate <= DateTime.Today.AddDays(7))
                .ToListAsync()
        };

        return View(viewModel);
    }
}
