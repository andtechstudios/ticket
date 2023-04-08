using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Models.Issues.Requests;

namespace Andtech.Ticket
{

	public class UnassignCommand
	{

		[Verb("unassign", HelpText = "Unassign issues.")]
		public class Options : BaseOptions
		{
			[Value(0, HelpText = "Iid of the issue.")]
			public string IssueId { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			var repository = await Session.Instance.GetRepositoryAsync();

			var iidString = options.IssueId.TrimStart('#');
			int iid = int.Parse(iidString);
			var assigneeIds = new List<int>()
			{
				0,
			};

			var request = new UpdateIssueRequest()
			{
				Assignees = assigneeIds,
			};
			var issue = await repository.Client.Issues.UpdateAsync(repository.ProjectID, iid, request);

			var iidText = Macros.TerminalURL($"#{iid}", issue.WebUrl);
			Log.WriteLine($"Cleared assignees from issue {iidText}!", ConsoleColor.Green);
		}
	}
}
