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
var categoriesList = new List<string>();
var jsonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
string jsonFile = Path.Combine(Environment.CurrentDirectory, "RecipeJson.json");


var jsonPathCategory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
string jsonFileCategory = Path.Combine(Environment.CurrentDirectory, "CategoryJson.json");

using (StreamReader r = new StreamReader(jsonFile))
{
	var Data = r.ReadToEnd();
	var Json = JsonConvert.DeserializeObject<List<Recipe>>(Data);
	if (Json != null)
	{
		recipesList = Json;
	}
}


using (StreamReader C = new StreamReader(jsonFileCategory))
{
	var Data = C.ReadToEnd();
	var Json = JsonConvert.DeserializeObject<List<string>>(Data);
	if (Json != null)
	{
		categoriesList = Json;
	}
}

app.MapGet("/recipes", () =>
{
	recipesList = recipesList.OrderBy(o => o.Title).ToList();
	return Results.Ok(recipesList);
});

app.MapGet("/recipes/{id}", ([FromRoute(Name = "id")] Guid id) =>
{
	for (int i = 0; i < recipesList.Count(); ++i)
	{
		if (recipesList[i].Id == id)
		{
			return Results.Ok(recipesList[i]);
		}
	}
	return Results.NotFound();
});

app.MapPost("/recipes", ([FromBody] Recipe recipe) =>
{
	recipesList.Add(recipe);
	recipesList = recipesList.OrderBy(o => o.Title).ToList();
	Save();
	return Results.Created($"/recipes/{recipe.Id}", recipe);
});

app.MapDelete("/recipes/{id}", ([FromRoute(Name = "id")] Guid id) =>
{
	if (recipesList.Find(recipe => recipe.Id == id) is Recipe recipe)
	{
		recipesList.Remove(recipe);
		Save();
		return Results.Ok(recipe);
	}
	return Results.NotFound(); //404 not found
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


app.MapPost("/category", ([FromBody] Category category) =>
{
	categoriesList.Add(category.CategoryName);
	Save();
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
			foreach (var r in recipesList)
			{
				r.Categories.Remove(oldCategory);
				r.Categories.Add(editedCategory);
			}
			Save();
			return Results.NoContent();
		}
	}
	return Results.NotFound();
});
void Save()
{
	File.WriteAllText(jsonFile, JsonConvert.SerializeObject(recipesList));
	File.WriteAllText(jsonFileCategory, JsonConvert.SerializeObject(categoriesList));
}
app.Run(); //