using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Models.Issues.Requests;

namespace Andtech.Ticket
{

	public class CloseCommand
	{

		[Verb("close", HelpText = "Close issues.")]
		public class Options : BaseOptions
		{
			[Value(0, Min = 1, HelpText = "List of issues to close.")]
			public IEnumerable<string> Issues { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			var repository = await Session.Instance.GetRepositoryAsync();
			foreach (var issueString in options.Issues)
			{
				try
				{
					var iidString = issueString.TrimStart('#');
					int iid = int.Parse(iidString);

					var request = new UpdateIssueRequest()
					{
						State = UpdatedIssueState.Close,
					};
					var issue = await repository.Client.Issues.UpdateAsync(repository.ProjectID, iid, request);

					var iidText = Macros.TerminalURL($"#{iid}", issue.WebUrl);
					Log.WriteLine($"Closed issue {iidText} successfully!", ConsoleColor.Green);
				}
				catch
				{
					Log.Error.WriteLine($"Invalid issue: '{issueString}'", ConsoleColor.Red);
				}
			}
		}
	}
}
