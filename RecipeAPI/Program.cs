using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RecipeAPI.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
	options.RoutePrefix = string.Empty;
});

var recipesList = new List<Recipe>();
var jsonRecipePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
string jsonRecipeFile = Path.Combine(Environment.CurrentDirectory, "Data.json");
using(StreamReader streamReader = new StreamReader(jsonRecipeFile))
{
	var Data = streamReader.ReadToEnd();
	var Json = JsonConvert.DeserializeObject<List<Recipe>>(Data);
	if (Json != null)
	{
		recipesList = Json;
	}
}

var categoriesList = new List<string>();
var jsonCategoryPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
string jsoneCategoryFile = Path.Combine(Environment.CurrentDirectory, "CategoriesInfo.json");



using (StreamReader streamReader = new StreamReader(jsoneCategoryFile))
{
	var Data = streamReader.ReadToEnd();
	var Json = JsonConvert.DeserializeObject<List<string>>(Data);
	if (Json != null)
	{
		categoriesList = Json;
	}
}
app.MapGet("/recipes", () =>
{
	return Results.Ok(recipesList);
});

app.MapGet("/recipes/{id}", ([FromRoute(Name = "id")] Guid id) =>
{
	var selectedRecipeIndex = recipesList.FindIndex(x => x.Id == id);
	if (selectedRecipeIndex != -1)
	{
		return Results.Ok(recipesList[selectedRecipeIndex]);
	}
	else
	{
		return Results.NotFound();
	}
});

app.MapPost("/recipes", (Recipe recipe) =>
{
	recipesList.Add(recipe);
	Save();
	return Results.Created($"/recipes/{recipe.Id}", recipe);
});

app.MapDelete("/recipes", (Guid id) =>
{
	if (recipesList.Find(recipe => recipe.Id == id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		Save();
		return Results.Ok(recipe);
	}
	return Results.NotFound(); 
});

app.MapPut("/recipes/{id}", ([FromBody] Recipe editedRecipe) =>
{
	if (recipesList.Find(recipe => recipe.Id == editedRecipe.Id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		recipesList.Add(editedRecipe);
		recipesList = recipesList.OrderBy(o => o.Title).ToList();
		Save();
		return Results.NoContent();
	}
	return Results.NotFound();
});

app.MapGet("/category", () =>
{
	return Results.Ok(categoriesList);
});

app.MapPost("/category", async (Category category) =>
{
	if (category.CategoryName == string.Empty || categoriesList.Contains(category.CategoryName))
	{
		return Results.BadRequest();
	}

	categoriesList.Add(category.CategoryName);
	categoriesList = categoriesList.OrderBy(o => o).ToList();
	Save();
	return Results.Created($"/category/{category}", category);
});

app.MapDelete("/category", (string category) =>
{
	for (int i = 0; i < categoriesList.Count; ++i)
	{
		if (categoriesList[i] == category)
		{
			foreach (Recipe recipe in recipesList)
			{
				recipe.Categories.Remove(category);
			}
			categoriesList.Remove(category);
			Save();
			return Results.Ok(category);
		}
	}
	return Results.NotFound();
});

app.MapPut("/category", (string oldCategory, string editedCategory) =>
{
	for (int i = 0; i < categoriesList.Count; ++i)
	{
		if (categoriesList[i] == oldCategory)
		{
			categoriesList.Remove(oldCategory);
			categoriesList.Add(editedCategory);
			foreach (var recipe in recipesList)
			{
				recipe.Categories.Remove(oldCategory);
				recipe.Categories.Add(editedCategory);
			}
			Save();
			return Results.NoContent();
		}
	}
	return Results.NotFound();
});

void Save()
{
	void Save()
	{
		File.WriteAllText(jsonRecipeFile, JsonConvert.SerializeObject(recipesList));
		File.WriteAllText(jsoneCategoryFile, JsonConvert.SerializeObject(categoriesList));
	}
}
 

app.Run();