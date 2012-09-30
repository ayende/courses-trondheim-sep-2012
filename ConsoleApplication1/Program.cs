using System;
using System.Collections.Generic;
using System.Diagnostics;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Shard;
using Raven.Client.Linq;
using System.Linq;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			var shards = new Dictionary<string, IDocumentStore>
				{
					{ "one", new DocumentStore { Url = "http://localhost:8079", } },
					{ "two", new DocumentStore { Url = "http://localhost:8078", } },
				};

			var shardStrategy = new ShardStrategy(shards)
				.ShardingOn<User>()
				.ShardingOn<Story>(x => x.UserId);

			using (var store = new ShardedDocumentStore(shardStrategy).Initialize())
			{
				//using (var session = store.OpenSession())
				//{
				//	var user = new User { Name = "Ayende" };
				//	session.Store(user);
				//	session.Store(new Story { UserId = user.Id });
				//	session.SaveChanges();
				//}


				using (var session = store.OpenSession())
				{
					var load = session.Query<Story>()
						.Where(x => x.UserId == "two/users/1")
						.ToList();

					Console.WriteLine(load[0].UserId);
				}
			}
		}
	}

	public class User
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class Story
	{
		public string UserId { get; set; }
	}
}
