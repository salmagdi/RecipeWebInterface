using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;


namespace RecipeRazor.Pages.Categories;

    public class EditModel : PageModel
    {
	[TempData]
	public string? ActionResult { get; set; }
	[FromRoute(Name = "category")]
	[Display(Name = "Old Category Name")]
	public string OldCategory { get; set; } = string.Empty;
	[BindProperty]
	[Required]
	[Display(Name = "New Category Name")]
	public string NewCategory { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public EditModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public void OnGet()
	{
	}
	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();
		try
		{
			var httpClient = _httpClientFactory.CreateClient("API");
			var response = await httpClient.PutAsync($"category?oldCategory={OldCategory}&editedCategory={NewCategory}", null);
			response.EnsureSuccessStatusCode();
			ActionResult = "Edited successfully";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
		}
		return RedirectToPage("./List");
	}

}
