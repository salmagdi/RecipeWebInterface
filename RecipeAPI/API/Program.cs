using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RecipeAPI.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

var recipesList = new List<Recipe>();
var recipePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
string RecipeFile = Path.Combine(recipePath, "Recipes.json");
var categoriesList = new List<string>();
var categoryPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
string CategoryFile = Path.Combine(categoryPath, "Categories.json");

using (StreamReader streamReader = new StreamReader(RecipeFile))
{
	var Data = streamReader.ReadToEnd();
	var Json = JsonConvert.DeserializeObject<List<Recipe>>(Data);
	if (Json != null)
	{
		recipesList = Json;
	}
}

app.MapGet("/recipes", () =>
{
	return Results.Ok(recipesList);
});

app.MapPost("/recipes", (Recipe recipe) =>
{
	recipesList.Add(recipe);
	SaveRecipe();
	return Results.Created($"/recipes/{recipe.Id}", recipe);
});

app.MapDelete("/recipes", (Guid id) =>
{
	if (recipesList.Find(recipe => recipe.Id == id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		SaveRecipe();
		return Results.Ok(recipe);
	}
	return Results.NotFound(); //404 not found
});

app.MapPut("/recipes", (Recipe editedRecipe) =>
{
	if (recipesList.Find(recipe => recipe.Id == editedRecipe.Id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		recipesList.Add(editedRecipe);
		SaveRecipe();
		return Results.NoContent();
	}
	return Results.NotFound();
});

app.MapGet("/category", () =>
{
	return Results.Ok(categoriesList);
});

app.MapPost("/category", ([FromBody] Category category) =>
{
	categoriesList.Add(category.CategoryName);
	SaveCategory();
	return Results.Created($"/recipes/{category}", category);
});

app.MapDelete("/category", (string category) =>
{
	for (int i = 0; i < categoriesList.Count; ++i)
	{
		if (categoriesList[i] == category)
		{
			foreach (Recipe r in recipesList)
			{
				r.Categories.Remove(category);
			}
			categoriesList.Remove(category);
			SaveRecipe();
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
			foreach (var r in recipesList)
			{
				r.Categories.Remove(oldCategory);
				r.Categories.Add(editedCategory);
			}
			SaveRecipe();
			return Results.NoContent();
		}
	}
	return Results.NotFound();
});

void SaveRecipe()
{
	File.WriteAllText(RecipeFile, JsonConvert.SerializeObject(recipesList));
}
 
void SaveCategory()
{
	File.WriteAllText(CategoryFile, JsonConvert.SerializeObject(categoriesList));
}
app.Run();