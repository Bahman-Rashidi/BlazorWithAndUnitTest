using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Yami33.Components;
using Yami33.Services;
using Yami33.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRadzenComponents();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



builder.Services.AddHttpClient();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>()
);
///builder.Services.AddAuthorizationCore();

builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();

//builder.Services.AddScoped<CustomAuthStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>(
//    sp => sp.GetRequiredService<CustomAuthStateProvider>()
//);
//builder.Services.AddAuthorizationCore();




#if DEBUG
builder.Services.AddScoped<IAuthService, MockAuthService>();
builder.Services.AddScoped<IProductRepository, MockProductRepository>();
#else
builder.Services.AddScoped<IAuthService, AuthService>();

#endif




builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
