using System.Security.Principal;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace WebStep.Indexes
{
	public class AuditListener : IDocumentStoreListener
	{
		public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
		{
			metadata["Updated-By"] = WindowsIdentity.GetCurrent().Name;
			return true;
		}

		public void AfterStore(string key, object entityInstance, RavenJObject metadata)
		{
		}
	}
}