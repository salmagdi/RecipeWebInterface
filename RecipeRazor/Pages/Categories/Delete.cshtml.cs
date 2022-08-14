using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace RecipeRazor.Pages.Categories;

public class DeleteModel : PageModel
{
	[TempData]
	public string? Message { get; set; }
	[FromRoute(Name = "category")]
	public string DeletedCategory { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public DeleteModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public void OnGet()
	{
	}
	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			var response = await httpClient.DeleteAsync("category?category=" + DeletedCategory);
			response.EnsureSuccessStatusCode();
			Message = "Deleted successfully";
		}
		catch (Exception)
		{
			Message = "Something went wrong, Try again later";
		}
		return RedirectToPage("./List");
	}

}
