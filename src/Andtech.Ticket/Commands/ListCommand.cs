using System.Text.Json;
using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Models.Issues.Responses;

namespace Andtech.Ticket
{

	public class ListCommand
	{

		[Verb("list", aliases: new string[] { "ls" }, HelpText = "List issues.", Hidden = true)]
		public class Options : BaseOptions
		{
			[Option("align-labels", HelpText = "Align all label dots when printed.")]
			public bool AlignLabels { get; set; }
			[Option("no-color", HelpText = "Force all dots to be printed white.")]
			public bool NoColor { get; set; }
			[Option("dot-symbol", HelpText = "Use a custom dot symbal.")]
			public string DotSymbol { get; set; }
			[Option('a', "all", HelpText = "Show issues related to all users.")]
			public bool ShowAllUsers { get; set; }
			[Option("backlog", HelpText = "Include issues with label 'backlog'.")]
			public bool IncludeBacklog { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			IEnumerable<Issue> allIssues = null;
			IEnumerable<GitLabApiClient.Models.Projects.Responses.Label> labels = null;

			bool cacheAvailable = TryLoadFromCache();

			bool TryLoadFromCache()
			{
				if (!options.UseCache)
				{
					return false;
				}

				var cache = Cache.Load();
				if (cache is null)
				{
					return false;
				}

				var timespan = DateTime.UtcNow - cache.timestamp;
				if (timespan > TimeSpan.FromHours(3))
				{
					return false;
				}

				allIssues = cache.issues;
				labels = cache.labels;
				return true;
			}

			var repository = await Session.Instance.GetRepositoryAsync(!cacheAvailable);

			if (!cacheAvailable)
			{
				var client = repository.Client;

				var issuesTask = client.Issues.GetAllAsync(repository.ProjectID);
				var labelsTask = client.Projects.GetLabelsAsync(repository.ProjectID);

				await Task.WhenAll(issuesTask, labelsTask);

				allIssues = issuesTask.Result;
				labels = labelsTask.Result;
			}

			if (allIssues.Count() == 0)
			{
				Log.WriteLine("No assigned issues");
				return;
			}

			var writer = new IssueWriter(labels);
			writer.DefaultDotSymbol = options.DotSymbol ?? writer.DefaultDotSymbol;
			writer.UseColor = !options.NoColor;

			// Filter issues
			var issues = allIssues;
			if (!options.ShowAllUsers)
			{
				issues = issues.Where(x => x.Assignee?.Username == repository.User.Name);
			}
			if (!options.IncludeBacklog)
			{
				issues = issues.Where(x => !x.Labels.Contains("backlog"));
			}
			// Apply
			writer.Print(issues, options.AlignLabels);

			if (!cacheAvailable)
			{
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
}
