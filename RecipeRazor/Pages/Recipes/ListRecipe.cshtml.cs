using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecipeRazor.Models;


namespace RecipeRazor.Pages.Recipes;

public class ListModel : PageModel
{

	[TempData]
public string? ActionResult { get; set; }
private readonly IHttpClientFactory _httpClientFactory;
public ListModel(IHttpClientFactory httpClientFactory) =>
	_httpClientFactory = httpClientFactory;

public List<Recipe> RecipeList { get; set; } = new();
public async Task<IActionResult> OnGet()
{
	try
	{
		HttpClient httpClient = _httpClientFactory.CreateClient("API");
		string baseAddress = httpClient.BaseAddress.ToString();
		var response = await httpClient.GetFromJsonAsync<List<Recipe>>($"{baseAddress}recipes", default);
		if (response != null)
			RecipeList = response;
		return Page();
	}
	catch (Exception)
	{
		ActionResult = "something went wrong";
	}
	return RedirectToPage("/Index");

}
}
