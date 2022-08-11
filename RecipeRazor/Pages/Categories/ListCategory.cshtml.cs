using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeRazor.Models;
namespace RecipeRazor.Pages.Categories
{
    public class ListCategoryModel : PageModel
    {
        public string? Message { get; set; }
        private readonly IHttpClientFactory? _httpClientFactory;
        public ListCategoryModel(IHttpClientFactory? httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<string> CategoryList { get; set; } = new List<string>();
        public async Task<IActionResult> OnGet()
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("API");
                string baseAddress = httpClient.BaseAddress.ToString();
                var response = await httpClient.GetFromJsonAsync<List<string>>($"{baseAddress}category", default);
                if (response != null)
                {
                    CategoryList = response;
                }
                return Page();
            }
            catch (Exception)
            {
                Message = "something went wrong";
                return RedirectToPage("/Index");
            }
        }
    }
}