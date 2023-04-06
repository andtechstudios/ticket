using GitLabApiClient;
using System.Text.RegularExpressions;

namespace Andtech.Ticket
{

	public class Repository
	{
		public Host Host { get; private set; }
		public int ProjectID { get; set; }
		public int UserID { get; set; }
		public bool IsGitConfigurationCorrect { get; set; }
		public GitLabClient Client { get; set; }

		public static async Task<Repository> LoadAsync(Config config, bool fetchMissingData = true)
		{
			var url = GetRemoteUrl();
			var host = config.hosts
				.First(x => url.Contains(x.hostname));
			var hostUrl = "https://" + host.hostname;
			var client = new GitLabClient(hostUrl, host.accessToken);

			var repository = new Repository()
			{
				Host = host,
				Client = client,
			};

			repository.UserID = repository.GetConfigInt("ticket.userid");
			repository.ProjectID = repository.GetConfigInt("ticket.projectid");
			if (fetchMissingData && repository.ProjectID < 0)
			{
				repository.ProjectID = await repository.GetProjectIDAsync(client, host.accessToken);
			}
			if (fetchMissingData && repository.UserID < 0)
			{
				repository.UserID = await repository.GetUserIDAsync(client, host.accessToken);
			}

			return repository;
		}

		public static string GetRemoteUrl()
		{
			return GitWrapper.Git("ls-remote --get-url origin").Trim();
		}

		public static string ParseProjectPathWithNamespace(string url)
		{
			var match = Regex.Match(url, @":(?<path>.+)$");
			if (match.Success)
			{
				var path = match.Groups["path"].Value;
				path = path.Replace(".git", string.Empty);

				return path;
			}

			return null;
		}

		public async Task<int> GetProjectIDAsync(GitLabClient client, string accessToken)
		{
			var url = GetRemoteUrl();
			var pathWithNamespace = ParseProjectPathWithNamespace(url);
			return await client.FindProjectIDAsync(accessToken, pathWithNamespace);
		}

		public async Task<int> GetUserIDAsync(GitLabClient client, string accessToken)
		{
			var apiSession = await client.Users.GetCurrentSessionAsync();
			return apiSession.Id;
		}

		public string GetConfigString(string key)
		{
			return GitWrapper.Git($"config {key}")
				.Trim();
		}

		public int GetConfigInt(string key)
		{
			var projectIdString = GitWrapper.Git($"config {key}")
				.Trim();

			if (int.TryParse(projectIdString, out var value))
			{
				return value;
			}

			return -1;
		}
	}
}
