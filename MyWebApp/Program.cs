var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();


app.MapGet("/about", () =>
{
    string htmlContent = System.IO.File.ReadAllText("wwwroot/about.html");
    return Results.Content(htmlContent, "text/html");
});

app.MapGet("/", async () =>
{
    string htmlContent = System.IO.File.ReadAllText("wwwroot/index.html");
    return Results.Content(htmlContent, "text/html");
});

// Defining a method to fetch ISS location from API.
async Task<string> GetISSLocation()
{
    using var client = new HttpClient();
    var response = await client.GetAsync("http://api.open-notify.org/iss-now.json");
    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    else
    {
        return "{}";
    }
}

// Mapping route to continuously update ISS position.
app.MapGet("/update-iss", async (HttpContext context) =>
{
    string issData = await GetISSLocation();
    if (issData != null)
    {
        await context.Response.WriteAsync(issData);
    }
    else
    {
        // Error handling.
        context.Response.StatusCode = 500; 
        await context.Response.WriteAsync("Failed to fetch ISS location");
    }
});

app.Run();