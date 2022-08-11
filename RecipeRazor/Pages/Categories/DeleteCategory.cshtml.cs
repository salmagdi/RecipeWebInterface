using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace RecipeRazor.Pages.Categories;

public class DeleteCategoryModel : PageModel
{
	[TempData]
	public string? Message { get; set; }
	[FromRoute(Name = "category")]
	public string DeletedCategory { get; set; } = String.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public DeleteCategoryModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public void OnGet()
	{
	}
	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			var response = await httpClient.DeleteAsync("category?category=" + DeletedCategory);
			response.EnsureSuccessStatusCode();
			Message = "Deleted successfully";
		}
		catch (Exception)
		{
			Message = "Something went wrong, Try again later";
		}
		return RedirectToPage("./ListCategory");
	}

}

