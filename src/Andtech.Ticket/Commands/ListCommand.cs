using System.Text.Json;
using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Projects.Responses;

namespace Andtech.Ticket
{

	[Serializable]
	public class Cache
	{
		public Issue[] issues { get; set; }
		public Label[] labels { get; set; }
	}

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
			var useCache = options.UseCache;
			var cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ticket");

			IEnumerable<Issue> issues = null;
			IEnumerable<Label> labels = null;

			if (options.UseCache)
			{
				try
				{
					var json = File.ReadAllText(cachePath);
					var deserialized = JsonSerializer.Deserialize<Cache>(json);
					issues = deserialized.issues;
					labels = deserialized.labels;
				}
				catch
				{

				}
			}

			if (issues is null)
			{
				var repository = await Session.Instance.GetRepositoryAsync();
				var client = repository.Client;

				var issuesTask = client.Issues.GetAllAsync(repository.ProjectID);
				var labelsTask = client.Projects.GetLabelsAsync(repository.ProjectID);

				await Task.WhenAll(issuesTask, labelsTask);

				issues = issuesTask.Result;
				labels = labelsTask.Result;
			}

			if (issues.Count() == 0)
			{
				Log.WriteLine("No assigned issues");
				return;
			}

			var writer = new IssueWriter(labels);
			writer.DefaultDotSymbol = options.DotSymbol ?? writer.DefaultDotSymbol;
			writer.UseColor = !options.NoColor;

			if (!options.IncludeBacklog)
			{
				issues = issues.Where(x => !x.Labels.Contains("backlog"));
			}

			writer.Print(issues, options.AlignLabels);

			// Write cache
			var cache = new Cache()
			{
				issues = issues.ToArray(),
				labels = labels.ToArray(),
			};

			string jsonString = JsonSerializer.Serialize(cache);
			File.WriteAllText(cachePath, jsonString);
		}
	}
}
