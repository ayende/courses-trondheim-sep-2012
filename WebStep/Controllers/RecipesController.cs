using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Raven.Client;
using WebStep.Indexes;
using WebStep.Models;
using System.Linq;

namespace WebStep.Controllers
{
	public class RecipesController : RavenController
	{
		public object New(string name)
		{
			var recipe = new Recipe
				{
					Title = name
				};
			Session.Store(recipe);

			return RedirectToAction("List");
		}

		public object Del(int id)
		{
			var entity = Session.Load<Recipe>(id);
			if (entity != null)
				Session.Delete(entity);

			return RedirectToAction("List");
		}

		public ActionResult List()
		{
			RavenQueryStatistics stats;
			var r = Session.Query<Recipe>()
				.Statistics(out stats)
				.Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
				.ToList();

			return Json(new
				{
					stats.IsStale,
					Results = r.Select(x => new { x.Id, x.Title })
				});
		}

		public object Chef(string chef)
		{
			var r = Session.Query<SearchResult, Global_Search>()
					.Include(x=>x.RecipesId)
					.Include(x=>x.ChefId)
				.Where(x => x.ChefId == chef)
				.As<TransformedResult>()
				.ToList();

			return Json(new
				{
					Results = r,
					Session.Advanced.NumberOfRequests
				});
		}
	}
}