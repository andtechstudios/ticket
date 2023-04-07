using System.Text.RegularExpressions;
using GitLabApiClient;

namespace Andtech.Ticket
{

	public class Repository
	{
		public int? UserID { get; set; }
		public string? UserName { get; set; }
		public int? ProjectID { get; set; }
		public Host Host { get; private set; }
		public GitLabClient Client { get; set; }

		private Dictionary<string, int> usersCache = new Dictionary<string, int>(1);

		public static async Task<Repository> LoadAsync(Config config, bool fetchMissingData = true)
		{
			var url = GetRemoteUrl();
			var host = config.hosts
				.First(x => url.Contains(x.hostname));
			var hostUrl = "https://" + host.hostname;
			var client = new GitLabClient(hostUrl, host.access_token);

			var repository = new Repository()
			{
				Host = host,
				Client = client,
			};

			repository.UserID = repository.GetConfigInt("ticket.userid");
			repository.UserName = repository.GetConfigString("ticket.username");
			repository.ProjectID = repository.GetConfigInt("ticket.projectid");

			if (fetchMissingData)
			{
				var apiSession = await repository.Client.Users.GetCurrentSessionAsync();
				repository.UserID = apiSession.Id;
				repository.UserName = apiSession.Username;
				repository.ProjectID = await repository.GetProjectIDAsync();
			}

			repository.usersCache.Add("me", repository.UserID.Value);
			repository.usersCache.Add(repository.UserName, repository.UserID.Value);

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

		public string GetConfigString(string key)
		{
			return GitWrapper.Git($"config {key}")
				.Trim();
		}

		public int? GetConfigInt(string key)
		{
			var projectIdString = GitWrapper.Git($"config {key}")
				.Trim();

			if (int.TryParse(projectIdString, out var value))
			{
				return value;
			}

			return -1;
		}

		public async Task<int> GetProjectIDAsync()
		{
			var url = GetRemoteUrl();
			var pathWithNamespace = ParseProjectPathWithNamespace(url);
			return await Client.FindProjectIDAsync(Host.access_token, pathWithNamespace);
		}

		public async Task<int> GetUserIdAsync(string username)
		{
			if (usersCache.TryGetValue(username, out var userId))
			{
				return userId;
			}

			var user = await Client.Users.GetAsync(username);
			usersCache.Add(username, user.Id);

			return user.Id;
		}
	}
}
