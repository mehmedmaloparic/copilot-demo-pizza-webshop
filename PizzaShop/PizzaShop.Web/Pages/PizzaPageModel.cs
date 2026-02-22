using Microsoft.AspNetCore.Mvc.RazorPages;
using PizzaShop.Web.Services;

namespace PizzaShop.Web.Pages;

public abstract class PizzaPageModel : PageModel
{
    protected readonly CartService CartService;

    protected PizzaPageModel(CartService cartService)
    {
        CartService = cartService;
    }

    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        ViewData["CartCount"] = CartService.Count;
        base.OnPageHandlerExecuting(context);
    }
}
