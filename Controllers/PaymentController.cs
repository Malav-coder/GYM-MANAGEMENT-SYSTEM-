using GymManagement.Data;
using GymManagement.Filters;
using GymManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers;

[ServiceFilter(typeof(MemberOnlyFilter))]
public class PaymentController : Controller
{
    private readonly AppDbContext _context;

    public PaymentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(int planId, string paymentType, int? membershipId)
    {
        var plan = await _context.Plans.FindAsync(planId);
        if (plan == null)
        {
            return NotFound();
        }

        var viewModel = new PaymentViewModel
        {
            PlanId = plan.Id,
            PlanName = plan.PlanName,
            Amount = plan.Price,
            PaymentType = paymentType,
            MembershipId = membershipId
        };

        if (paymentType == "Renewal" && membershipId.HasValue)
        {
            var oldMembership = await _context.Memberships.FindAsync(membershipId.Value);
            if (oldMembership != null)
            {
                var daysRemaining = (oldMembership.EndDate - DateTime.Today).Days;
                if (daysRemaining > 0)
                {
                    decimal discount = 200m;
                    viewModel.Amount = Math.Max(0, plan.Price - discount);
                    ViewBag.Discount = discount;
                }
            }
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(PaymentViewModel model)
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var plan = await _context.Plans.FindAsync(model.PlanId);

        if (plan == null)
        {
            return NotFound();
        }

        decimal finalAmount = plan.Price;

        if (model.PaymentType == "Renewal" && model.MembershipId.HasValue)
        {
            var oldMembership = await _context.Memberships.FindAsync(model.MembershipId.Value);
            if (oldMembership != null)
            {
                oldMembership.Status = "Expired";
                
                var daysRemaining = (oldMembership.EndDate - DateTime.Today).Days;
                if (daysRemaining > 0)
                {
                    finalAmount = Math.Max(0, plan.Price - 200m);
                }
            }
        }

        var newMembership = new Membership
        {
            UserId = userId,
            PlanId = plan.Id,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddMonths(plan.DurationMonths),
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        _context.Memberships.Add(newMembership);
        await _context.SaveChangesAsync();

        var payment = new Payment
        {
            UserId = userId,
            MembershipId = newMembership.Id,
            Amount = finalAmount,
            Type = model.PaymentType,
            PaidAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        TempData["PlanName"] = plan.PlanName;
        TempData["StartDate"] = newMembership.StartDate.ToString("dd MMM yyyy");
        TempData["EndDate"] = newMembership.EndDate.ToString("dd MMM yyyy");
        TempData["Amount"] = finalAmount.ToString("N2");

        return RedirectToAction("Success");
    }

    [HttpGet]
    public IActionResult Success()
    {
        ViewBag.PlanName = TempData["PlanName"];
        ViewBag.StartDate = TempData["StartDate"];
        ViewBag.EndDate = TempData["EndDate"];
        ViewBag.Amount = TempData["Amount"];

        return View();
    }
}
