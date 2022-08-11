using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;
namespace RecipeRazor.Pages.Categories;

public class CreateCategoryModel : PageModel
{
	[TempData]
	public string? Message { get; set; }
	[BindProperty]
	[Required]
	[Display(Name = "Category Name")]
	public string AddedCategory { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public CreateCategoryModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			string baseAddress = httpClient.BaseAddress.ToString();
			var response = await httpClient.PostAsJsonAsync($"{baseAddress}category", new Category { CategoryName = AddedCategory },
				new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			response.EnsureSuccessStatusCode();
			Message = "Created successfully";

		}
		catch (Exception)
		{
			Message = "Something went wrong, Try again later";
		}
		return RedirectToPage("./ListCategory");
	}
}