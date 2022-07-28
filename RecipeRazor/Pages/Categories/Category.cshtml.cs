using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
namespace RecipeRazor.Pages.Categories;

public class CategoryModel : PageModel
{
    [TempData]
    public string? ActionResult { get; set; }
    [BindProperty]
    [Required]
    [Display(Name = "Category Name")]
    public string CategoryAdded { get; set; } = string.Empty;
    [FromRoute(Name = "category")]
    public string CategoryDeleted { get; set; } = string.Empty;
    [FromRoute(Name = "category")]
    [Display(Name = "Old Category Name")]
    public string OldCategory { get; set; } = string.Empty;
    [BindProperty]
    [Required]
    [Display(Name = "New Category Name")]
    public string NewCategory { get; set; } = string.Empty;
    public List<string> CategoryList { get; set; } = new();

    private readonly IHttpClientFactory _httpClientFactory;
    public CategoryModel(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;

    // Get categories
    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
            List<string>? categories = await httpClient.GetFromJsonAsync<List<string>>("categories");
            if (categories != null)
                CategoryList = categories;
            return Page();
        }
        catch (Exception)
        {
            ActionResult = "Something went wrong, Try again later";
            return RedirectToPage("/Index");
        }
    }

    // Add a category
    public async Task<IActionResult> OnPostAdd()
    {
        if (!ModelState.IsValid)
            return Page();
        try
        {
            var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
            var response = await httpClient.PostAsJsonAsync("categories?category=" + CategoryAdded, CategoryAdded);
            response.EnsureSuccessStatusCode();
            ActionResult = "Created successfully";
        }
        catch (Exception)
        {
            ActionResult = "Something went wrong, try again";
        }
        return RedirectToPage("./Index");
    }

    // Edit a Category 
    public async Task<IActionResult> OnPostEdit()
    {
        if (!ModelState.IsValid)
            return Page();
        try
        {
            var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
            var response = await httpClient.PutAsync($"categories?oldcategory={OldCategory}&editedcategory={NewCategory}", null);
            response.EnsureSuccessStatusCode();
            ActionResult = "Created successfully";
        }
        catch (Exception)
        {
            ActionResult = "Something went wrong, try again";
        }
        return RedirectToPage("./Index");
    }

    // Delete a Category
    public async Task<IActionResult> OnPostDelete()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
            var response = await httpClient.DeleteAsync("categories?category=" + CategoryDeleted);
            response.EnsureSuccessStatusCode();
            ActionResult = "Created successfully";
        }
        catch (Exception)
        {
            ActionResult = "Something went wrong, try again";
        }
        return RedirectToPage("./Index");
    }

}
