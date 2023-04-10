using System.Text.RegularExpressions;
using Flurl.Util;
using GitLabApiClient;

namespace Andtech.Ticket
{

	public struct User
	{
		public int? Id { get; set; }
		public string? Name { get; set; }
		public string? DisplayName { get; set; }
	}

	public class Repository
	{
		public User User { get; private set; }
		public int? ProjectID { get; private set; }
		public string ProjectUrl { get; private set; }
		public Host Host { get; private set; }
		public GitLabClient Client { get; set; }
		public bool IsConfigured
		{
			get
			{
				return User.Id >= 0
					&& !string.IsNullOrEmpty(User.Name)
					&& !string.IsNullOrEmpty(User.DisplayName)
					&& ProjectID >= 0
					&& !string.IsNullOrEmpty(ProjectUrl);
			}
		}

		private Dictionary<string, User> usersCache = new Dictionary<string, User>(1);

		public static async Task<Repository> LoadAsync(Config config, bool fetchMissingData = false)
		{
			var remoteUrl = GetRemoteUrl();
            var pathWithNamespace = ParseProjectPathWithNamespace(remoteUrl);
            var host = config.hosts
				.First(x => remoteUrl.Contains(x.hostname));
			var gitlabUrl = "https://" + host.hostname;
			var client = new GitLabClient(gitlabUrl, host.access_token);

			var repository = new Repository()
			{
				Host = host,
				Client = client,
			};

			repository.User = new User()
			{
				Id = repository.GetConfigInt("ticket.userid"),
				Name = repository.GetConfigString("ticket.username"),
				DisplayName = repository.GetConfigString("ticket.displayname"),
			};
			repository.ProjectID = repository.GetConfigInt("ticket.projectid");
            repository.ProjectUrl = repository.GetConfigString("ticket.projecturl");

            if (fetchMissingData)
			{
				var apiSession = await repository.Client.Users.GetCurrentSessionAsync();

				repository.User = new User()
				{
					Id = apiSession.Id,
					Name = apiSession.Username,
					DisplayName = apiSession.Name,
				};

                repository.ProjectID = await repository.Client.FindProjectIDAsync(repository.Host.access_token, pathWithNamespace);
				repository.ProjectUrl = Path.Combine($"https://{repository.Host.hostname}", pathWithNamespace);
			}

			if (!string.IsNullOrEmpty(repository.User.Name))
			{
				repository.usersCache.Add(repository.User.Name, repository.User);
				repository.usersCache.Add("me", repository.User);
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

		public async Task<User> GetUserAsync(string username)
		{
			if (usersCache.TryGetValue(username, out var user))
			{
				return user;
			}

			var u = await Client.Users.GetAsync(username);
			user = new User()
			{
				Id = u.Id,
				Name = u.Username,
				DisplayName = u.Name,
			};
			usersCache.Add(username, user);

			return user;
		}
	}
}
