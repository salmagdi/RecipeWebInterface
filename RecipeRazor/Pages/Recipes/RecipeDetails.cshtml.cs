using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;

namespace RecipeRazor.Pages.Recipes;

public class RecipeDetailsModel : PageModel
{



	[TempData]
	public string? Message { get; set; }

	public Recipe recipe { get; set; } = new();
	public IEnumerable<string> DetailedIngredients { get; set; } = new List<string>();
	public IEnumerable<string> DetailedInstructions { get; set; } = new List<string>();

	private readonly IHttpClientFactory _httpClientFactory;
	public RecipeDetailsModel(IHttpClientFactory httpClientFactory) =>
		_httpClientFactory = httpClientFactory;
	public async Task<IActionResult> OnGet(Guid recipeId)
	{
		try
		{
			HttpClient httpClient = _httpClientFactory.CreateClient("API");
			string baseAddress = httpClient.BaseAddress.ToString();
			var response = await httpClient.GetFromJsonAsync<Recipe>($"{baseAddress}recipes/{recipeId}");
			if (response != null)
			{
				recipe = response;
				DetailedIngredients = recipe.Ingredients.ToList();
				DetailedInstructions = recipe.Instructions.ToList();
				return Page();
			}
			else
			{
				return NotFound();
			}
		}
		catch (Exception)
		{
			Message = "something went wrong";
		}
		return RedirectToPage("/Index");

	}
}