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
			public string AssigneeName { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			var repository = await Session.Instance.GetRepositoryAsync();

			var iidString = options.IssueId.TrimStart('#');
			int iid = int.Parse(iidString);
			var assigneeIds = new List<int>(1)
			{
				await repository.GetUserIdAsync(options.AssigneeName)
			};

			var request = new UpdateIssueRequest()
			{
				Assignees = assigneeIds,
			};
			var issue = await repository.Client.Issues.UpdateAsync(repository.ProjectID, iid, request);

			var iidText = Macros.TerminalURL($"#{iid}", issue.WebUrl);
			Log.WriteLine($"Assigned issue {iidText} to {options.AssigneeName}!", ConsoleColor.Green);
		}
	}
}
