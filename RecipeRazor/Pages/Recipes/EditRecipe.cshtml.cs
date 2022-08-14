using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;

namespace RecipeRazor.Pages.Recipes;


public class EditRecipeModel : PageModel
{
		[TempData]
		public string? ActionResult { get; set; }
		[BindProperty]
		[Required]
		public Recipe recipe { get; set; } = new();
		public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
		[BindProperty]
		public IEnumerable<string> SelectedCategories { get; set; } = Enumerable.Empty<string>();
		[BindProperty]
		public string Ingredients { get; set; } = string.Empty;
		[BindProperty]
		public string Instructions { get; set; } = string.Empty;
		private readonly IHttpClientFactory _httpClientFactory;

		public EditRecipeModel(IHttpClientFactory httpClientFactory) =>
				_httpClientFactory = httpClientFactory;

		public async Task<IActionResult> OnGetAsync(Guid recipeId)
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			string baseAddress = httpClient.BaseAddress.ToString();
			var categoriesResponse = await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{baseAddress}category");
			if (categoriesResponse != null)
				Categories = categoriesResponse;
			var recipeResponse = await httpClient.GetFromJsonAsync<Recipe>($"{baseAddress}recipes/{recipeId}");
			if (recipeResponse != null)
				recipe = recipeResponse; if (recipe == null)
				return NotFound();

			Ingredients = String.Join(Environment.NewLine, recipe.Ingredients);
			Instructions = String.Join(Environment.NewLine, recipe.Instructions);
			return Page();
		}
		public async Task<IActionResult> OnPostAsync(Guid recipeId)
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			try
			{
				string baseAddress = httpClient.BaseAddress.ToString();
				var response = await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{baseAddress}category", default);
				if (response != null)
					Categories = response;
			}
			catch (Exception)
			{
				ActionResult = "Something went wrong, please try again";
				return RedirectToPage("./ListRecipe");
			}

			recipe.Id = recipeId;
			if (SelectedCategories != null)
				recipe.Categories = (List<string>)SelectedCategories;
			if (Ingredients != null)
				recipe.Ingredients = Ingredients.Split(Environment.NewLine).ToList();
			if (Instructions != null)
				recipe.Instructions = Instructions.Split(Environment.NewLine).ToList();

			try
			{
				var response = await httpClient.PutAsJsonAsync($"recipes/{recipeId}", new Recipe
				{
					Categories = recipe.Categories,
					Id = recipe.Id,
					Ingredients = recipe.Ingredients,
					Instructions = recipe.Instructions,
					Title = recipe.Title
				}
				);
				response.EnsureSuccessStatusCode();
				ActionResult = "Successfully Edited";
			}
			catch (Exception)
			{
				ActionResult = "Something went wrong please try again later";
			}
			return RedirectToPage("./ListRecipe");
		}
	}


