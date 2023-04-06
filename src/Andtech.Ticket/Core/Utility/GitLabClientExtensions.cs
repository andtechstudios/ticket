using System.Web;
using Flurl;
using Flurl.Http;
using GitLabApiClient;

namespace Andtech.Ticket
{

	public static class GitLabClientExtensions
	{

		public static async Task<int> FindProjectIDAsync(this GitLabClient client, string accessToken, string pathWithNamespace)
		{
			var encodedPath = HttpUtility.UrlEncode(pathWithNamespace);
			var json = await client.HostUrl
				 .AppendPathSegments("projects", encodedPath)
				 .WithOAuthBearerToken(accessToken)
				 .GetJsonAsync();

			var idText = json.id.ToString();

			return int.Parse(idText);
		}
	}
}
