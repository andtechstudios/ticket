using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Internal.Paths;
using GitLabApiClient.Models.Issues.Requests;

namespace Andtech.Ticket
{

	public class AssignCommand
	{

		[Verb("assign", HelpText = "Assign issues.")]
		public class Options : BaseOptions
		{
			[Value(0, HelpText = "Iid of the issue.")]
			public string IssueId { get; set; }
			[Value(1, HelpText = "Username of the new assignee.")]
			public string AssigneeName { get; set; } = "me";
		}

		public static async Task OnParseAsync(Options options)
		{
			var repository = await Session.Instance.GetRepositoryAsync();

			int iid = Macros.ParseIssue(options.IssueId);
			var user = await repository.GetUserAsync(options.AssigneeName);
			var assigneeIds = new List<int>(1)
			{
				user.Id.Value,
			};

			var request = new UpdateIssueRequest()
			{
				Assignees = assigneeIds,
			};
			var issue = await repository.Client.Issues.UpdateAsync(repository.ProjectID, iid, request);

			var iidText = Macros.TerminalLink($"#{iid}", issue.WebUrl);
			Log.WriteLine($"Assigned issue {iidText} to @{user.Name}!", ConsoleColor.Green);
		}
	}
}
