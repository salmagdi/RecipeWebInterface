using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;

namespace RecipeRazor.Pages.Categories;

public class EditModel : PageModel
{

	[TempData]
	public string? Message { get; set; }
	[FromRoute(Name = "category")]
	[Display(Name = "Old AddedCategory Name")]
	public string OldCategory { get; set; } = string.Empty;
	[BindProperty]
	[Required]
	[Display(Name = "New Name")]
	public string NewCategory { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public EditModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public void OnGet()
	{
	}
	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			var response = await httpClient.PutAsync($"category?oldCategory={OldCategory}&editedCategory={NewCategory}", null);
			response.EnsureSuccessStatusCode();
			Message = "Edited successfully";
		}
		catch (Exception)
		{
			Message = "Something went wrong, Try again later";
		}
		return RedirectToPage("./List");
	}
}