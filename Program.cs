using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Application;
using MyCourse.Models.Services.Infrastructure;



WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var builderConfiguration = builder.Configuration;


//ConfigureService

builder.Services.AddResponseCaching();

builder.Services.AddMvc(options => {
    //options.EnableEndpointRouting = false;
    var homeProfile = new CacheProfile();
    //homeProfile.Duration = builder.Configuration.GetValue<int>("ResponseCache:Home:Duration");
    //homeProfile.Location = builderConfiguration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
    //homeProfile.VaryByQueryKeys = new string[] { "page" };
    builderConfiguration.Bind("ResponseCache:Home", homeProfile);
    options.CacheProfiles.Add("Home", homeProfile);

});
//builder.Services.AddTransient<ICourseService, AdoNetCourseService>();
builder.Services.AddTransient<ICourseService, EfCoreCourseService>();
builder.Services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();
builder.Services.AddTransient<ICachedCourseService, MemoryCacheCourseService>();
builder.Services.AddSingleton<IImagePersister, MagickNetImagePersister>();

//builder.Services.AddScoped<MyCourseDbContext>();
//builder.Services.AddDbContext<MyCourseDbContext>();
builder.Services.AddDbContextPool<MyCourseDbContext>(optionsBuilder =>
{
    string connectionString = "Data Source=Data/MyCourse.db";
    optionsBuilder.UseSqlite(connectionString);
});

//options
builder.Services.Configure<ConnectionStringsOptions>(builderConfiguration.GetSection("ConnectionStrings"));
builder.Services.Configure<CoursesOptions>(builderConfiguration.GetSection("Courses"));
builder.Services.Configure<MemoryCacheOptions>(builderConfiguration.GetSection("MemoryCache"));
builder.Services.Configure<KestrelServerOptions>(builderConfiguration.GetSection("Kestrel"));





var app = builder.Build();

//Configure Middleware
if (app.Environment.IsDevelopment())
{ 
    app.UseDeveloperExceptionPage();
}else{
    app.UseExceptionHandler("/Error");
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}


app.UseStaticFiles();

//Nel caso volessi impostare una Culture specifica...
/*var appCulture = CultureInfo.InvariantCulture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(appCulture),
    SupportedCultures = new[] { appCulture }
});*/

app.UseRouting();


app.UseResponseCaching();

app.UseEndpoints(routeBuilder =>
{
    routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
});
//app.UseMvcWithDefaultRoute(); 
/*app.UseMvc(routeBuilder =>
{
    routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
});*/

app.Run();

