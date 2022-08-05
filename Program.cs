using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Application;
using MyCourse.Models.Services.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var builderConfiguration = builder.Configuration;

//ConfigureService
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
//builder.Services.AddTransient<ICourseService, AdoNetCourseService>();
builder.Services.AddTransient<ICourseService, EfCoreCourseService>();
builder.Services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();

//builder.Services.AddScoped<MyCourseDbContext>();
//builder.Services.AddDbContext<MyCourseDbContext>();
builder.Services.AddDbContextPool<MyCourseDbContext>(optionsBuilder =>
{
    string configuration = builderConfiguration.GetSection(key:"ConnectionStrings").GetValue<string>("Default");
    optionsBuilder.UseSqlite(configuration);
});

//Options
builder.Services.Configure<ConnectionStringsOptions>(builderConfiguration.GetSection(key: "ConnectionStrings"));
builder.Services.Configure<CoursesOptions>(builderConfiguration.GetSection(key: "Courses"));


var app = builder.Build();

//Configure Middleware
if (app.Environment.IsDevelopment())
{ 
    app.UseDeveloperExceptionPage();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}


app.UseStaticFiles();
app.UseRouting();

//app.UseMvcWithDefaultRoute(); 
app.UseMvc(routeBuilder =>
{
    routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

