using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;
namespace RecipeRazor.Pages.Categories;

public class CreateModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[BindProperty]
	[Required]
	[Display(Name = "Category Name")]
	public string Category { get; set; } = string.Empty;
	[BindProperty]
	public List<string> Categories { get; set; } = new();
	private readonly IHttpClientFactory _httpClientFactory;

	public CreateModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			string baseAddress = httpClient.BaseAddress.ToString();
			var response = await httpClient.PostAsJsonAsync($"{baseAddress}category",
				new Category { CategoryName = Category }, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			response.EnsureSuccessStatusCode();
			ActionResult = "Created successfully";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
		}
		return RedirectToPage("./List");
	}

}