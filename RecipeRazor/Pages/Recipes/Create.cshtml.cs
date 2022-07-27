using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;
namespace RecipeRazor.Pages.Recipes;

    public class CreateModel : PageModel
    {

	[TempData]
	public string? ActionResult { get; set; }
	[BindProperty]
	[Required]
	public Recipe Recipe { get; set; } = new();
	public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public IEnumerable<string>? SelectedCategories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public string? Ingredients { get; set; } = string.Empty;
	[BindProperty]
	public string? Instructions { get; set; } = string.Empty;

	private readonly IHttpClientFactory _httpClientFactory;
	public CreateModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;
	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			var response = await httpClient.GetFromJsonAsync<IEnumerable<string>>("categories");
			if (response != null)
				Categories = response;
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
		var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
		try
		{
			var response = await httpClient.GetFromJsonAsync<IEnumerable<string>>("categories");
			if (response != null)
				Categories = response;
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
			return RedirectToPage("./Index");
		}

		if (!ModelState.IsValid)
			return Page();

		Recipe.Id = Guid.Empty;
		if (SelectedCategories != null)
			Recipe.Categories = (List<string>)SelectedCategories;
		if (Ingredients != null)
			Recipe.Ingredients = Ingredients.Split(Environment.NewLine).ToList();
		if (Instructions != null)
			Recipe.Instructions = Instructions.Split(Environment.NewLine).ToList();

		
		try
		{
			var response = await httpClient.PostAsJsonAsync("recipes", Recipe);
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Created";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
		}
		return RedirectToPage("./Index");
	}
}
