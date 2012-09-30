using WebStep.Models;
using Raven.Client;
using System.Linq;

namespace WebStep.Controllers
{
	public class CompaniesController : RavenController
	{
		 public object Search(string q)
		 {
			 var query = Session.Query<Company>("companies")
				.Search(x => x.Name, q);
			 var r = query
				 .ToList();

			if(r.Count == 0)
			{
				var suggest = query.Suggest();
				switch (suggest.Suggestions.Length)
				{
					case 0:
						return Json("Could not find any matches");
					case 1:
						return Search(suggest.Suggestions[0]);
					default:
						return Json(new
							{
								DidYouMean = suggest.Suggestions
							});
				}
			}

			 return Json(new
				 {
					 Results = r.Select(x=>new
						 {
							 x.Name,
							 x.Id,
							 Score = Session.Advanced.GetMetadataFor(x).Value<string>("Temp-Index-Score")
						 })
				 });
		 }
	}
}