using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;

namespace RecipeRazor.Pages.Categories;

public class CategoryPageModel : PageModel
{
    [TempData]
    public string? Message { get; set; }
    [BindProperty]
    [Required]
    [Display(Name = "Category Name")]
    public string AddedCategory { get; set; }

    [FromRoute(Name = "category")]
    public string DeletedCategory { get; set; } = string.Empty;

    [FromRoute(Name = "category")]
    [Display(Name = "Old Category Name")]
    public string OldCategoryName { get; set; } = string.Empty;
    [BindProperty]
    [Required]
    [Display(Name = "New Category Name")]
    public string NewCategoryName { get; set; } = string.Empty;

    public List<string> CategoryList { get; set; } = new List<string>();

    private readonly IHttpClientFactory _httpClientFactory;
    public CategoryPageModel (IHttpClientFactory? httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<IActionResult> OnGetCategories()
    {
        try
        {
            HttpClient _httpClientFactory.CreateCLient("API");

        }
    }

}

