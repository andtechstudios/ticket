﻿using Andtech.Ticket;
using Andtech.Ticket.Core;
using CommandLine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var result = Parser.Default.ParseArguments<InitCommand.Options, ListCommand.Options, CreateCommand.Options>(args);
await result.WithParsedAsync<BaseOptions>(PreParse);
await result
	.WithParsedAsync<InitCommand.Options>(InitCommand.OnParseAsync);
await result
	.WithParsedAsync<ListCommand.Options>(ListCommand.OnParseAsync);
await result
	.WithParsedAsync<CreateCommand.Options>(CreateCommand.OnParseAsync);

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
