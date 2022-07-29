﻿using System.ComponentModel.DataAnnotations;
namespace RecipeAPI.Model;

public class Recipe
{
	public Guid Id { get; set; } = Guid.Empty;
	[Required]
	public string Title { get; set; } = string.Empty;
	public List<string> Ingredients { get; set; } = new();
	public List<string> Instructions { get; set; } = new();
	public List<string> Categories { get; set; } = new();
}
