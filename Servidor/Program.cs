using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseWebSockets();

app.Map(pattern: "/", requestDelegate: async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
    else
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        while (true)
        {
            var data = Encoding.ASCII.GetBytes($"{DateTime.Now}");

            await webSocket.SendAsync(
                data,
                WebSocketMessageType.Text,
                endOfMessage: true,
                CancellationToken.None
                );
            await Task.Delay(1000);
        }
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
