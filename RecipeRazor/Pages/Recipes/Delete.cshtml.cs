using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;
namespace RecipeRazor.Pages.Recipes;

public class DeleteModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	public Guid RecipeId { get; set; } = Guid.Empty;
	public Recipe Recipe { get; set; } = new();
	private readonly IHttpClientFactory _httpClientFactory;

	public DeleteModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public async Task<IActionResult> OnGet()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			var response = await httpClient.GetFromJsonAsync<Recipe>($"recipes/{RecipeId}");
			if (response == null)
				return NotFound();
			Recipe = response;
			return Page();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
			return RedirectToPage("./Index");
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			var response = await httpClient.DeleteAsync("recipes?id=" + RecipeId);
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Deleted";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try agai";
		}
		return RedirectToPage("./Index");
	}
}
