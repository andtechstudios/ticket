using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;

namespace Andtech.Ticket
{

	public class ListCommand
	{

		[Verb("list",  aliases: new string[] { "ls" }, HelpText = "List issues.", Hidden = true)]
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
			var repository = await Session.Instance.GetRepositoryAsync();
			var client = repository.Client;

			var issuesTask = client.Issues.GetAllAsync(repository.ProjectID, options: SelectOnlyMyIssues);
			var labelsTask = client.Projects.GetLabelsAsync(repository.ProjectID);

			await Task.WhenAll(issuesTask, labelsTask);

			IEnumerable<Issue> issues = issuesTask.Result;
			if (issues.Count() == 0)
			{
				Log.WriteLine("No assigned issues");
			}
			else
			{
				var labels = labelsTask.Result;
				var writer = new IssueWriter(labels);
				writer.DefaultDotSymbol = options.DotSymbol ?? writer.DefaultDotSymbol;
				writer.UseColor = !options.NoColor;

				if (!options.IncludeBacklog)
				{
					issues = issues.Where(x => !x.Labels.Contains("backlog"));
				}

				writer.Print(issues, options.AlignLabels);
			}

			void SelectOnlyMyIssues(IssuesQueryOptions o)
			{
				var assignees = new List<string>(1);
				if (options.ShowAllUsers)
				{
				}
				else
				{
					assignees.Add(repository.User.Name);
				}

				o.AssigneeUsername = assignees;
			}
		}
	}
}
