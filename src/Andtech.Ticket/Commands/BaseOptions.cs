using CommandLine;

public class BaseOptions
{
	[Option("cache", Default = true, HelpText = "Skip HTTP.")]
	public bool Cache { get; set; }
	[Option("no-cache", HelpText = "Require HTTP.")]
	public bool NoCache { get; set; }

	public bool UseCache => !NoCache && Cache;
}
