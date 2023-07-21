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
AnimeCrawler crawler = new();
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
builder.Services.AddSingleton(new MeilisearchClient("https://toonify-meilisearch.j7popa.easypanel.host/",
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
app.MapGet("/testMap", (int[] id) => crawler.MapChunkFromProvider(id));
app.MapControllers();

app.MapGraphQL("/graphql");

app.Run();