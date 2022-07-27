using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;

namespace RecipeRazor.Pages.Recipes;

public class EditModel :PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[BindProperty]
	[Required]
	public Recipe Recipe { get; set; } = new();
	public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public IEnumerable<string> SelectedCategories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public string Ingredients { get; set; } = string.Empty;
	[BindProperty]
	public string Instructions { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public EditModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public async Task<IActionResult> OnGetAsync(Guid recipeId)
	{
		var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
		var categoryResponse = await httpClient.GetFromJsonAsync<IEnumerable<string>>("categories");
		var recipeResponse = await httpClient.GetFromJsonAsync<Recipe>($"recipes/{recipeId}");
		if (categoryResponse != null)
			Categories = categoryResponse;
		if (recipeResponse != null)
			Recipe = recipeResponse;
		if (Recipe == null)
			return NotFound();
		Ingredients = String.Join(Environment.NewLine, Recipe.Ingredients);
		Instructions = String.Join(Environment.NewLine, Recipe.Instructions);
		return Page();
	}

	public async Task<IActionResult> OnPostAsync(Guid recipeId)
	{
		var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
		if (!ModelState.IsValid)
		{
			var categoryResponse = await httpClient.GetFromJsonAsync<IEnumerable<string>>("categories");
			var recipeResponse = await httpClient.GetFromJsonAsync<Recipe>($"recipes/{recipeId}");
			if (categoryResponse != null)
				Categories = categoryResponse;
			if (recipeResponse != null)
				Recipe = recipeResponse;
			return Page();
		}

		Recipe.Id = recipeId;
		if (SelectedCategories != null)
			Recipe.Categories = (List<string>)SelectedCategories;
		if (Ingredients != null)
			Recipe.Ingredients = Ingredients.Split(Environment.NewLine).ToList();
		if (Instructions != null)
			Recipe.Instructions = Instructions.Split(Environment.NewLine).ToList();

		try
		{
			var response = await httpClient.PutAsJsonAsync("recipes", Recipe);
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Edited";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
		}
		return RedirectToPage("./Index");
	}

}
