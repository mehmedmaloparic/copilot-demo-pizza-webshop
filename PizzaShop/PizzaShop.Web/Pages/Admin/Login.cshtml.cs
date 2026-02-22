using Microsoft.AspNetCore.Mvc;
using PizzaShop.Web.Services;

namespace PizzaShop.Web.Pages.Admin;

public class LoginModel : PizzaPageModel
{
    private readonly PizzaApiClient _api;

    public LoginModel(PizzaApiClient api, CartService cartService) : base(cartService)
    {
        _api = api;
    }

    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var result = await _api.AdminLoginAsync(Username, Password);
            if (result is { Success: true, Token: not null })
            {
                HttpContext.Session.SetString("admin_token", result.Token);
                return RedirectToPage("/Admin/Orders");
            }
            ErrorMessage = "Invalid username or password.";
        }
        catch
        {
            ErrorMessage = "Could not connect to the API. Is it running?";
        }
        return Page();
    }
}
