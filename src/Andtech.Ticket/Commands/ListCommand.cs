using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Models.Issues.Requests;

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
		}

		public static async Task OnParseAsync(Options options)
		{
			var repository = await Session.Instance.GetRepositoryAsync();
			var client = repository.Client;

			var apiSession = await client.Users.GetCurrentSessionAsync();
			var issues = await client.Issues.GetAllAsync(repository.ProjectID, options: SelectOnlyMyIssues);
			if (issues.Count == 0)
			{
				Log.WriteLine("No assigned tickets");
			}
			else
			{
				var labels = await client.Projects.GetLabelsAsync(repository.ProjectID);
				var writer = new IssueWriter(labels);
				writer.DefaultDotSymbol = options.DotSymbol ?? writer.DefaultDotSymbol;
				writer.UseColor = !options.NoColor;
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
					assignees.Add(apiSession.Username);
				}

				o.AssigneeUsername = assignees;
			}
		}
	}
}
