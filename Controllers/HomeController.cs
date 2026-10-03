using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermitSystem.Web.Data;
using PermitSystem.Web.Entities;

namespace PermitSystem.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public HomeController(AppDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var roles = await _users.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault() ?? "Requester";

        var lookups = await _db.LookupCategories
            .Include(c => c.Items)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

        var screens = await _db.Screens
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();

        var settings = await _db.SystemSettings
            .Where(s => s.Key.StartsWith("System."))
            .ToDictionaryAsync(x => x.Key, x => x.Value);

        var themeKey = settings.GetValueOrDefault("System.CurrentTheme") ?? "default";
        var theme = await _db.Themes.FirstOrDefaultAsync(t => t.Key == themeKey)
                    ?? await _db.Themes.FirstOrDefaultAsync(t => t.IsDefault)
                    ?? new Theme();

        var branches = await _db.Branches.Where(b => b.IsActive).ToListAsync();
        var buildings = await _db.Buildings.Where(b => b.IsActive).ToListAsync();

        var lookupsMap = new Dictionary<string, object>();
        foreach (var c in lookups)
        {
            var items = c.Items.Where(i => i.IsActive).OrderBy(i => i.SortOrder).ToList();
            var grouped = items.Where(i => i.GroupKey != null).ToList();
            if (grouped.Any())
            {
                lookupsMap[c.Key] = grouped
                    .GroupBy(i => i.GroupKey!)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Value).ToList());
            }
            else
            {
                lookupsMap[c.Key] = items.Select(i => i.Value).ToList();
            }
        }

        ViewBag.SystemName = settings.GetValueOrDefault("System.Name") ?? "نظام تصاريح الدخول";
        ViewBag.SystemLogo = settings.GetValueOrDefault("System.Logo");
        ViewBag.Theme = new
        {
            Key = theme.Key,
            Gd = theme.ColorDark,
            Gm = theme.ColorPrimary,
            Gmi = theme.ColorPrimaryLight,
            Gp = theme.ColorPale,
            Glight = theme.ColorLight,
            Au = theme.ColorGold,
            Aul = theme.ColorGoldLight,
            Aub = theme.ColorGoldBg
        };
        ViewBag.CurrentUser = new
        {
            Id = user.Id,
            Name = user.FullName,
            Role = roleName,
            Avatar = user.Avatar
        };
        ViewBag.Screens = screens.Select(s => new
        {
            s.Key, s.Title, s.Icon, s.GroupName, s.Category, s.SortOrder
        }).ToList();
        ViewBag.Lookups = lookupsMap;
        ViewBag.Branches = branches.Select(b => new { b.Id, b.Name, b.Code, b.City }).ToList();
        ViewBag.Buildings = buildings.Select(b => new { b.Id, b.Name, b.Code, b.BranchId, b.Floors }).ToList();

        return View();
    }

    [AllowAnonymous]
    public IActionResult Error() => View();
}