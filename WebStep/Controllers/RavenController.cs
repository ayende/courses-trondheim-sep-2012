using System.Net;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using WebStep.Indexes;

namespace WebStep.Controllers
{
	public class RavenController : Controller
	{
		protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return base.Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
		}

		private static IDocumentStore documentStore;

		public static IDocumentStore DocumentStore
		{
			get
			{
				if (documentStore == null)
				{
					lock (typeof(RavenController))
					{
						if (documentStore != null)
							return documentStore;
						var store  = new DocumentStore
						{
							ConnectionStringName = "RavenDB",
						};

						store.RegisterListener(new AuditListener());

						documentStore = store.Initialize();
						IndexCreation.CreateIndexesAsync(typeof(RavenController).Assembly, documentStore);
					}
				}
				return documentStore;
			}
		}

		public new IDocumentSession Session { get; set; }

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			Session = DocumentStore.OpenSession();
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			using (Session)
			{
				if (Session == null)
					return;
				if (filterContext.Exception != null)
					return;
				Session.SaveChanges();
			}
		}
	}
}