using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeRazor.Models;

namespace RecipeRazor.Pages.Recipes;

    public class DeleteRecipeModel : PageModel
    {
	[TempData]
	public string? ActionResult { get; set; }
	public Guid RecipeId { get; set; } = Guid.Empty;
	public Recipe recipe { get; set; } = new();
	private readonly IHttpClientFactory _httpClientFactory;

	public DeleteRecipeModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public async Task<IActionResult> OnGet(Guid recipeId)
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			var response = await httpClient.GetFromJsonAsync<Recipe>($"{httpClient.BaseAddress.ToString()}recipes/{recipeId}");
			if (response == null)
				return NotFound();
			recipe = response;
			return Page();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong please try again later";
			return RedirectToPage("./ListRecipe");
		}
	}

	public async Task<IActionResult> OnPostAsync(Guid recipeId)
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			var response = await httpClient.DeleteAsync($"recipes/{recipeId}");
			//var response = await httpClient.DeleteAsync("recipes?recipeId=" + recipeId);
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Deleted";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong please try again later";
		}
		return RedirectToPage("./ListRecipe");
	}
}
