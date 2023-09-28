using Andtech.Common;
using Andtech.Ticket;
using Andtech.Ticket.Core;
using CommandLine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var result = Parser.Default.ParseArguments<
	FetchCommand.Options	,
	InitCommand.Options,
	ListCommand.Options,
	CreateCommand.Options,
	AssignCommand.Options,
	UnassignCommand.Options,
	CloseCommand.Options,
	ViewCommand.Options
>(args);

await result.WithParsedAsync<BaseOptions>(PreParse);
try
{
    await result
        .WithParsedAsync<InitCommand.Options>(InitCommand.OnParseAsync);
    await result
        .WithParsedAsync<ListCommand.Options>(ListCommand.OnParseAsync);
    await result
        .WithParsedAsync<CreateCommand.Options>(CreateCommand.OnParseAsync);
    await result
        .WithParsedAsync<AssignCommand.Options>(AssignCommand.OnParseAsync);
    await result
        .WithParsedAsync<UnassignCommand.Options>(UnassignCommand.OnParseAsync);
    await result
        .WithParsedAsync<CloseCommand.Options>(CloseCommand.OnParseAsync);
    await result
        .WithParsedAsync<ViewCommand.Options>(ViewCommand.OnParseAsync);
}
catch (TicketConfigurationException)
{
	Log.Error.WriteLine("Error reading ticket repository. Did you forget to run 'ticket init'?", ConsoleColor.Red);
}

static async Task PreParse(BaseOptions options)
{
	var configPath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
		".config",
		"ticket.yml"
	);
	var text = File.ReadAllText(configPath);
	var deserializer = new DeserializerBuilder()
	 .WithNamingConvention(UnderscoredNamingConvention.Instance)
	 .Build();
	var config = deserializer.Deserialize<Config>(text);

	var session = new Session()
	{
		Config = config,
	};
	Session.Instance = session;
}
