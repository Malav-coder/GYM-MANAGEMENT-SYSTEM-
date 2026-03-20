using GymManagement.Data;
using GymManagement.Filters;
using GymManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers;

[ServiceFilter(typeof(MemberOnlyFilter))]
public class MemberPortalController : Controller
{
    private readonly AppDbContext _context;

    public MemberPortalController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        
        var user = await _context.Users
            .Include(u => u.Memberships.OrderByDescending(m => m.CreatedAt))
            .ThenInclude(m => m.Plan)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        var activeMembership = user.Memberships.FirstOrDefault(m => m.Status == "Active");

        if (activeMembership == null)
        {
            return RedirectToAction("ChoosePlan");
        }

        var recentPayments = await _context.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();

        var viewModel = new MemberDashboardViewModel
        {
            User = user,
            ActiveMembership = activeMembership,
            DaysRemaining = (activeMembership.EndDate - DateTime.Today).Days,
            RecentPayments = recentPayments
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> ChoosePlan()
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        
        var activeMembership = await _context.Memberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Status == "Active");

        if (activeMembership != null && (activeMembership.EndDate - DateTime.Today).Days > 5)
        {
            TempData["Error"] = "You can only upgrade or change your plan within 5 days of your current plan's expiration.";
            return RedirectToAction("Index");
        }

        var plans = await _context.Plans.Where(p => p.IsActive).ToListAsync();
        return View(plans);
    }

    [HttpGet]
    public async Task<IActionResult> Renew()
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        
        var activeMembership = await _context.Memberships
            .Include(m => m.Plan)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Status == "Active");

        if (activeMembership == null)
        {
            return RedirectToAction("ChoosePlan");
        }

        if ((activeMembership.EndDate - DateTime.Today).Days > 5)
        {
            TempData["Error"] = "You can only renew or upgrade your plan when you have 5 or fewer days remaining.";
            return RedirectToAction("Index");
        }

        var plans = await _context.Plans.Where(p => p.IsActive).ToListAsync();
        
        ViewBag.CurrentMembership = activeMembership;
        return View(plans);
    }

    [HttpGet]
    public async Task<IActionResult> Cancel()
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        
        var activeMembership = await _context.Memberships
            .Include(m => m.Plan)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Status == "Active");

        if (activeMembership == null)
        {
            TempData["Error"] = "No active membership found";
            return RedirectToAction("Index");
        }

        var daysSinceStart = (DateTime.Today - activeMembership.StartDate).Days;
        decimal fee = 0;
        string reason = "No fee — cancelled after 30 days";

        if (daysSinceStart <= 7)
        {
            fee = activeMembership.Plan.Price * 0.50m;
            reason = "50% fee — cancelled within 7 days";
        }
        else if (daysSinceStart <= 30)
        {
            fee = activeMembership.Plan.Price * 0.25m;
            reason = "25% fee — cancelled within 30 days";
        }

        ViewBag.CancellationFee = fee;
        ViewBag.FeeReason = reason;
        ViewBag.DaysSinceStart = daysSinceStart;

        return View(activeMembership);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmCancel()
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        
        var activeMembership = await _context.Memberships
            .Include(m => m.Plan)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Status == "Active");

        if (activeMembership == null)
        {
            TempData["Error"] = "No active membership found";
            return RedirectToAction("Index");
        }

        var daysSinceStart = (DateTime.Today - activeMembership.StartDate).Days;
        decimal fee = 0;

        if (daysSinceStart <= 7)
        {
            fee = activeMembership.Plan.Price * 0.50m;
        }
        else if (daysSinceStart <= 30)
        {
            fee = activeMembership.Plan.Price * 0.25m;
        }

        activeMembership.Status = "Cancelled";
        
        if (fee > 0)
        {
            var payment = new Payment
            {
                UserId = userId,
                MembershipId = activeMembership.Id,
                Amount = fee,
                Type = "CancellationFee",
                PaidAt = DateTime.UtcNow
            };
            _context.Payments.Add(payment);
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Membership cancelled successfully";
        return RedirectToAction("Index");
    }
}
