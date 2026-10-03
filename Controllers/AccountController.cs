using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PermitSystem.Web.Entities;

namespace PermitSystem.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly UserManager<ApplicationUser> _users;

    public AccountController(SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> users)
    {
        _signIn = signIn;
        _users = users;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, bool rememberMe = false)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "⚠️ أدخل اسم المستخدم وكلمة المرور";
            return View();
        }

        var user = await _users.FindByNameAsync(username);
        if (user == null || !user.IsActive)
        {
            ViewBag.Error = "⚠️ بيانات الدخول غير صحيحة";
            return View();
        }

        var result = await _signIn.PasswordSignInAsync(user, password, rememberMe, false);
        if (!result.Succeeded)
        {
            ViewBag.Error = "⚠️ بيانات الدخول غير صحيحة";
            return View();
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => Content("غير مصرح لك بالدخول لهذه الصفحة");
}