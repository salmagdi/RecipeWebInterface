using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;

namespace RecipeRazor.Pages.Recipes;
public class RecipesPageModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[BindProperty]
	[Required]
	public Recipe RecipeAdded { get; set; } = new();
	public Recipe RecipeDeleted { get; set; } = new();
	public Recipe RecipeEdited { get; set; } = new();
	public Guid RecipeId { get; set; } = Guid.Empty;
	public List<string> Categories { get; set; } = new List<string>();
	[BindProperty]
	public List<string>? SelectedCategories { get; set; } = new List<string>();
	public List<Recipe> RecipeList { get; set; } = new List<Recipe>();
	[BindProperty]
	public string? Ingredients { get; set; } = string.Empty;
	[BindProperty]
	public string? Instructions { get; set; } = string.Empty;
	public Recipe? RecipeDetails { get; set; }

	private readonly IHttpClientFactory _httpClientFactory;
	public RecipesPageModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	// List Recipes
	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			string baseAddress = httpClient.BaseAddress.ToString();
			List<Recipe> recipes = await httpClient.GetFromJsonAsync<List<Recipe>>($"{baseAddress}recipes");
			if (recipes != null)
				RecipeList = recipes;
			return Page();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
			return RedirectToPage("/Index");
		}
	}
	//List Categories 
	public async Task<IActionResult> OnGetCategoriesAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			string baseAddress = httpClient.BaseAddress.ToString();
			var response = await httpClient.GetFromJsonAsync<List<string>>($"{baseAddress}category");
			if (response != null)
				Categories = response;
			return Page();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
			return RedirectToPage("./RecipesPage");
		}
	}
	//Add a Recipe
	public async Task<IActionResult> OnPostAsync()
	{
		var httpClient = _httpClientFactory.CreateClient("API");
		try
		{
			string baseAddress = httpClient.BaseAddress.ToString();
			var response = await httpClient.GetFromJsonAsync<List<string>>($"{baseAddress}category");
			if (response != null)
				Categories = response;
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
			return RedirectToPage("./RecipesPage");
		}

		if (!ModelState.IsValid)
			return Page();

		RecipeAdded.Id = Guid.NewGuid();
		if (SelectedCategories != null)
			RecipeAdded.Categories = (List<string>)SelectedCategories;
		if (Ingredients != null)
			RecipeAdded.Ingredients = Ingredients.Split(Environment.NewLine).ToList();
		if (Instructions != null)
			RecipeAdded.Instructions = Instructions.Split(Environment.NewLine).ToList();


		try
		{
			string baseAddress = httpClient.BaseAddress.ToString();
			var response = await httpClient.PostAsJsonAsync($"{baseAddress}recipes",
				  new Recipe { Id = RecipeAdded.Id, Title = RecipeAdded.Title, Ingredients = RecipeAdded.Ingredients, Instructions = RecipeAdded.Instructions, Categories = RecipeAdded.Categories }
				 , new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			response.EnsureSuccessStatusCode();
			ActionResult = "Created successfully";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
		}
		return RedirectToPage("./RecipesPage");
	}
	// Delete a Recipe
	public async Task<IActionResult> OnDeleteAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			string baseAddress = httpClient.BaseAddress.ToString();
			var response = await httpClient.DeleteAsync("recipes?id=" + RecipeId);
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Deleted";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, please try again";
		}
		return RedirectToPage("./ListRecipes");
	}

	// Edit a Recipe
	public async Task<IActionResult> OnGetRecipeCategoryAsync(Guid recipeId)
	{
		var httpClient = _httpClientFactory.CreateClient("API");
		string baseAddress = httpClient.BaseAddress.ToString();
		var categoryResponse = await httpClient.GetFromJsonAsync<List<string>>($"{baseAddress}category");
		var recipeResponse = await httpClient.GetFromJsonAsync<Recipe>($"recipes");
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
	public async Task<IActionResult> OnPutAsync(Guid recipeId)
	{
		PopulateRecipeAndCategoriesAsync(recipeId);

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
			var response = await httpClient.PutAsJsonAsync($"{httpClient.BaseAddress.ToString()}recipes/{recipeId}", new Recipe
			{
				Id = RecipeEdited.Id,
				Categories = RecipeEdited.Categories,
				Ingredients = RecipeEdited.Ingredients,
				Instructions = RecipeEdited.Instructions,
				Title = RecipeEdited.Title
			});
			response.EnsureSuccessStatusCode();
			ActionResult = "Edited successfully";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
		}
		return RedirectToPage("./RecipesPage");
	}

   public async Task PopulateRecipeAndCategoriesAsync(Guid recipeId)
	{
		var httpClient = _httpClientFactory.CreateClient("API");
		string baseAddress = httpClient.BaseAddress.ToString();
		var categoriesResponse = await httpClient.GetFromJsonAsync<List<string>>($"{baseAddress}category");
		if (categoriesResponse != null)
			Categories = categoriesResponse;
		var recipeResponse = await httpClient.GetFromJsonAsync<Recipe>($"{baseAddress}recipes/{recipeId}");
		if (recipeResponse != null)
			RecipeEdited = recipeResponse;
	}

	}

