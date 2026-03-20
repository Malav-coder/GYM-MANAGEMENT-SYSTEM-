using GymManagement.Data;
using GymManagement.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers;

[ServiceFilter(typeof(AdminOnlyFilter))]
public class MembersController : Controller
{
    private readonly AppDbContext _context;

    public MembersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Users
            .Include(u => u.Memberships.OrderByDescending(m => m.CreatedAt).Take(1))
            .ThenInclude(m => m.Plan)
            .Where(u => u.Role == "Member");

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => 
                u.Name.Contains(search) || 
                u.Email.Contains(search) || 
                u.Phone.Contains(search));
        }

        var members = await query.ToListAsync();
        ViewBag.Search = search;
        return View(members);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _context.Users
            .Include(u => u.Memberships.OrderByDescending(m => m.CreatedAt))
            .ThenInclude(m => m.Plan)
            .Include(u => u.Payments.OrderByDescending(p => p.PaidAt))
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Freeze(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsFrozen = true;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Member account frozen successfully";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Unfreeze(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsFrozen = false;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Member account unfrozen successfully";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> WaiveCancellationFee(int membershipId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.MembershipId == membershipId && p.Type == "CancellationFee");

        if (payment == null)
        {
            TempData["Error"] = "No cancellation fee found for this membership";
            return RedirectToAction("Index");
        }

        var userId = payment.UserId;
        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cancellation fee waived successfully";
        return RedirectToAction("Details", new { id = userId });
    }
}
