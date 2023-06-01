using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<GremlinClient>(
                (serviceProvider) =>
                {
                    var gremlinServer = new GremlinServer(
                        hostname: "localhost",
                        port: 8182,
                        enableSsl: false,
                        username: null,
                        password: null
                    );
                    var connectionPoolSettings = new ConnectionPoolSettings
                    {
                        MaxInProcessPerConnection = 32,
                        PoolSize = 4,
                        ReconnectionAttempts = 4,
                        ReconnectionBaseDelay = TimeSpan.FromSeconds(1)
                    };
                    return new GremlinClient(
                        gremlinServer: gremlinServer,
                        connectionPoolSettings: connectionPoolSettings
                    );
                }
            );
builder.Services.AddSingleton<GraphTraversalSource>(
                (serviceProvider) =>
                {
                    GremlinClient gremlinClient = serviceProvider.GetService<GremlinClient>();
                    var driverRemoteConnection = new DriverRemoteConnection(gremlinClient, "g");
                    return AnonymousTraversalSource.Traversal().WithRemote(driverRemoteConnection);
                }
            );
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHsts();
app.Run();
