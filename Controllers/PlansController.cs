using GymManagement.Data;
using GymManagement.Filters;
using GymManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers;

[ServiceFilter(typeof(AdminOnlyFilter))]
public class PlansController : Controller
{
    private readonly AppDbContext _context;

    public PlansController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var plans = await _context.Plans.ToListAsync();
        return View(plans);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Plan());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Plan plan)
    {
        if (!ModelState.IsValid)
        {
            return View(plan);
        }

        _context.Plans.Add(plan);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Plan created successfully";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        return View(plan);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Plan plan)
    {
        if (id != plan.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(plan);
        }

        _context.Update(plan);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Plan updated successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        plan.IsActive = false;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Plan deactivated successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        plan.IsActive = !plan.IsActive;
        await _context.SaveChangesAsync();

        TempData["Success"] = plan.IsActive ? "Plan activated successfully" : "Plan deactivated successfully";
        return RedirectToAction("Index");
    }
}
