using System.Linq;
using Raven.Client.Indexes;
using WebStep.Models;

namespace WebStep.Indexes
{
	public class Global_Search : AbstractMultiMapIndexCreationTask<SearchResult>
	{
		public Global_Search()
		{
			AddMap<Recipe>(recipes =>
			               from recipe in recipes 
						   from c in recipe.Chefs
						   select new { ChefId = c, RecipesId = new[]{recipe.Id}}
			);

			AddMap<Chef>(chefs =>
			             from chef in chefs
						 select new { ChefId = chef.Id, RecipesId = new string[0] }
			);

			Reduce = results =>
			         from searchResult in results
			         group searchResult by searchResult.ChefId
			         into g
			         select new
				         {
							ChefId = g.Key,
							RecipesId = g.SelectMany(x=>x.RecipesId)
				         };

			TransformResults = (database, results) =>
			    from searchResult in results
			    select new
				    {
					    ChefName = database.Load<Chef>(searchResult.ChefId).Name,
					    Recipes = from r in searchResult.RecipesId
					                let recipe = database.Load<Recipe>(r)
					                where recipe != null
					                select new
						                {
							                recipe.Title,
							                Chefs = recipe.Chefs.Select(c => database.Load<Chef>(c).Name)
						                }
				    };

		}
	}
}