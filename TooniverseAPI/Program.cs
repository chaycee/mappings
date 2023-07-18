using System.Runtime.InteropServices;
using System.Text;
using Juro.Providers.Anime;
using Meilisearch;
using Newtonsoft.Json;
using TooniverseAPI.Data;
using TooniverseAPI.Database;
using TooniverseAPI.Mappings;
using TooniverseAPI.Mappings.Crawling.Anime;
using TooniverseAPI.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.ForegroundColor = ConsoleColor.DarkRed;
var builder = WebApplication.CreateBuilder(args);
var art = @"⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⣴⣶⣾⣿⣿⣿⣿⣿⡏⡆⠀
⠀⠀⠀⠀⠀⠀⠀⣠⣾⠿⠛⠋⠉⠉⠉⠈⠉⠛⠛⢳⡇⠀
⠀⠀⠀⠀⠀⢀⠞⠋⠀⠀⣷⣤⣀⣀⣀⠀⠀⠀⠀⠸⡇⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⣿⣿⣿⣿⣢⠄⠀⠀⡇⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⣿⣿⣿⣿⣿⣿⡀⠀⠀⡇⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⡇⠀⣀⣇⠀        ▄▄▄▄▄                    ·▄▄▄ ▄· ▄▌
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⣿⣿⣿⣿⣿⣧⣴⣾⣻⡆        •██  ▪     ▪     •█▌██ ▐▄▄·▐█▪██▌
⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣿⣿⣿⣿⣿⣿⣿⣿⣿⣸⣿⡇         ▐█.▪ ▄█▀▄  ▄█▀▄ ▐█ ▐█·██▪ ▐█▌▐█▪
⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⣿⣭⣾⣿⣿⣿⠉⣛⢿⠿⠁         ▐█▌·▐█▌.▐▌▐█▌.▐▌██ ▐█▌██▌. ▐█▀·.
⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⣿⣷⣶⣿⣻⣿⣆⠙⣿⠀⠀         ▀▀▀  ▀█▄▀▪ ▀█▄▀▪▀▀▪▀▀▀▀▀▀   ▀ • 
⠀⠀⠀⠀⠀⠀⠀⢀⣴⣿⣿⣿⣿⡿⣸⣔⣿⣿⡄⣿⠀⠀        
⠀⠀⠀⠀⢀⣠⣶⣿⣿⣿⣿⣿⣿⣧⣼⣿⣿⣿⣿⡏⠀⠀
⠐⠶⠶⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⠇⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠉⠀⠀⠀⠀⠀⠀";

Console.WriteLine(art);
builder.Services.AddSingleton(new MeilisearchClient("http://localhost:7700",
    "mBexx2khL4184VgswA-LOFIJy29cO_Uer24yH5B0dgM"));

builder.Services.AddDbContext<TooniverseContext>();

//builder.Services.AddHostedService<AnimeMappingService>();
// builder.Services.AddHostedService<MeiliSearchSyncService>(); 
// builder.Services.AddHostedService<AnimeRelationService>();
builder.Services.AddOutputCache();
builder.Services.AddGraphQLServer().AddQueryType<Query>().AddProjections().AddFiltering().AddSorting();

builder.Services.AddControllers();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.UseOutputCache();
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/graphql");

app.Run();