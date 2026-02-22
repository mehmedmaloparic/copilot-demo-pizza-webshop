using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PizzaShop.Web.Services;

namespace PizzaShop.Web.Pages
{
    public class IndexModel : PizzaPageModel
    {
        public IndexModel(CartService cartService) : base(cartService) { }

        public void OnGet()
        {

        }
    }
}
