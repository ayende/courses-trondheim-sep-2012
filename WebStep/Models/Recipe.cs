using System.Collections.Generic;

namespace WebStep.Models
{
	public class Chef
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class SearchResult
	{
		public string ChefId { get; set; }
		public string[] RecipesId { get; set; }
	}

	public class TransformedResult
	{
		public string ChefName { get; set; }
		public TransformedRecipe[] Recipes { get; set; }

		public class TransformedRecipe
		{
			public string Title;
			public string[] Chefs;
		}
	}

	public class Recipe
	{
		public string Title { get; set; }
		public string[] Chefs { get; set; }
		public List<Ingredient> Ingredients { get; set; }

		public string Id { get; set; }

		public Recipe()
		{
			Ingredients = new List<Ingredient>();
		}
	}

	public class Ingredient
	{
		public string Name { get; set; }
		public decimal Amount { get; set; }
		public string Unit { get; set; }
	}
}