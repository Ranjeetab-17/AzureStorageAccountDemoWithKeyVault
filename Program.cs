using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL").Value;

builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultURL),
    new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blobs}/{action=Index}/{id?}");

app.Run();
