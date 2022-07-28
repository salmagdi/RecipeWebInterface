using FluentValidation;
using System.ComponentModel.DataAnnotations;
namespace RecipeRazor.Models;

public class Recipe
{
    public Guid Id { get; set; } = Guid.Empty;
    [Required]
    public string Title { get; set; } = string.Empty;
    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
    public List<string> Categories { get; set; } = new();

    public class RecipeValidator : AbstractValidator<Recipe>
    {
        public RecipeValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Title).NotNull();
            RuleFor(x => x.Ingredients).NotNull().NotEmpty();
            RuleFor(x => x.Instructions).NotNull().NotEmpty();
            RuleFor(x => x.Categories).NotNull().NotEmpty();

        }
    }
}
