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
	public Recipe RecipeEdited { get; set; } = new();
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

	public void OnGet()
	{
	}
	public async Task<IActionResult> OnPostAsync(Guid recipeId)
	{
		

		RecipeEdited.Id = recipeId;
		if (SelectedCategories != null)
			RecipeEdited.Categories = (List<string>)SelectedCategories;
		if (Ingredients != null)
			RecipeEdited.Ingredients = Ingredients.Split(Environment.NewLine).ToList();
		if (Instructions != null)
			RecipeEdited.Instructions = Instructions.Split(Environment.NewLine).ToList();

		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			var response = await httpClient.PutAsJsonAsync("recipes",RecipeEdited);
			response.EnsureSuccessStatusCode();
			ActionResult = "Created successfully";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
		}
		return RedirectToPage("/Index");
	}

	/*public async Task<IActionResult> OnGetAsync(Guid recipeId)
	{
		var httpClient = _httpClientFactory.CreateClient("API");
		string baseAddress = httpClient.BaseAddress.ToString();
		var categoryResponse = await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{baseAddress}category");
		var recipeResponse = await httpClient.GetFromJsonAsync<Recipe>($"{baseAddress}recipes");
		if (categoryResponse != null)
			Categories = categoryResponse;
		if (recipeResponse != null)
			RecipeEdited = recipeResponse;
		if (RecipeEdited == null)
			return NotFound();
		Ingredients = String.Join(Environment.NewLine, RecipeEdited.Ingredients);
		Instructions = String.Join(Environment.NewLine, RecipeEdited.Instructions);
		return Page();
	}

	public async Task<IActionResult> OnPostAsync(Guid recipeId)
	{
		var httpClient = _httpClientFactory.CreateClient("API");
		if (!ModelState.IsValid)
		{
			string baseAddress = httpClient.BaseAddress.ToString();
			var categoryResponse = await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{baseAddress}category");
			var recipeResponse = await httpClient.GetFromJsonAsync<Recipe>($"{baseAddress}recipes");
			if (categoryResponse != null)
				Categories = categoryResponse;
			if (recipeResponse != null)
				RecipeEdited = recipeResponse;
			return Page();
		}

		RecipeEdited.Id = recipeId;
		if (SelectedCategories != null)
			RecipeEdited.Categories = (List<string>)SelectedCategories;
		if (Ingredients != null)
			RecipeEdited.Ingredients = Ingredients.Split(Environment.NewLine).ToList();
		if (Instructions != null)
			RecipeEdited.Instructions = Instructions.Split(Environment.NewLine).ToList();

		try
		{
			var response = await httpClient.PutAsJsonAsync("recipes", RecipeEdited);
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Edited";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
		}
		return RedirectToPage("/Index");
	}*/

}
