var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();


app.MapGet("/about", () =>
{
    // Read the contents of the HTML file
    string htmlContent = System.IO.File.ReadAllText("about.html");
    // Set the content type to "text/html"
    return Results.Content(htmlContent, "text/html");
});

app.MapGet("/", async () =>
{
    // Read the contents of the HTML file
    string htmlContent = System.IO.File.ReadAllText("index.html");
    // Set the content type to "text/html"
    return Results.Content(htmlContent, "text/html");
});

// Define a method to fetch ISS location
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

// Map route to continuously update ISS position
app.MapGet("/update-iss", async (HttpContext context) =>
{
    string issData = await GetISSLocation();
    if (issData != null)
    {
        // Write ISS location data to the response
        await context.Response.WriteAsync(issData);
    }
    else
    {
        // Handle error
        context.Response.StatusCode = 500; // Internal Server Error
        await context.Response.WriteAsync("Failed to fetch ISS location");
    }
});

app.Run();