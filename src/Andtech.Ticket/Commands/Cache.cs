using Andtech.Common;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Projects.Responses;
using Newtonsoft.Json;

namespace Andtech.Ticket
{

	public class Cache
	{
		public DateTime timestamp;
		public List<Issue> issues { get; set; }
		public List<Label> labels { get; set; }

		public static Cache Load()
		{
			var hasGitDir = ShellUtility.Find(Environment.CurrentDirectory, ".git", out var gitDir, FindOptions.RecursiveUp);
			if (!hasGitDir)
			{
				return null;
			}

			var cachePath = Path.Combine(gitDir, "ticket-cache.json");

			Cache cache;
			try
			{
				var json = File.ReadAllText(cachePath);
				cache = JsonConvert.DeserializeObject<Cache>(json);
			}
			catch
			{
				return null;
			}

			return cache;
		}

		public static void Write(Cache cache)
		{
			var hasGitDir = ShellUtility.Find(Environment.CurrentDirectory, ".git", out var gitDir, FindOptions.RecursiveUp);
			if (!hasGitDir)
			{
				return;
			}

			var cachePath = Path.Combine(gitDir, "ticket-cache.json");
			string jsonString = JsonConvert.SerializeObject(cache);
			File.WriteAllText(cachePath, jsonString);
		}
	}
}
