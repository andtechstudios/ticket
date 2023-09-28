using Andtech.Ticket.Core;
using CommandLine;

namespace Andtech.Ticket
{

	public class FetchCommand
	{

		[Verb("fetch", HelpText = "Force download the issues cache.")]
		public class Options : BaseOptions
		{
		}

		public static async Task OnParseAsync(Options options)
		{
			var repository = await Session.Instance.GetRepositoryAsync();
			var client = repository.Client;
			var issuesTask = client.Issues.GetAllAsync(repository.ProjectID);
			var labelsTask = client.Projects.GetLabelsAsync(repository.ProjectID);
			await Task.WhenAll(issuesTask, labelsTask);

			var allIssues = issuesTask.Result;
			var labels = labelsTask.Result;

			var cache = new Cache()
			{
				timestamp = DateTime.UtcNow,
				issues = allIssues.ToList(),
				labels = labels.ToList(),
			};

			Cache.Write(cache);
		}
	}
}
