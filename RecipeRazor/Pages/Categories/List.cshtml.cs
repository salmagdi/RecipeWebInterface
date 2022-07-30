using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RecipeRazor.Pages.Categories;

    public class ListModel : PageModel
    {

	[TempData]
	public string? ActionResult { get; set; }
	private readonly IHttpClientFactory? _httpClientFactory;
	public ListModel(IHttpClientFactory httpClientFactory) =>
		_httpClientFactory = httpClientFactory;

	public List<string> CategoryList { get; set; } = new();
	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			string baseAddress = httpClient.BaseAddress.ToString();
			List<string> recipes = await httpClient.GetFromJsonAsync<List<string>>($"{baseAddress}category");
			if (recipes != null)
				CategoryList = recipes;
			return Page();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
			return RedirectToPage("/Index");
		}
	}
}
