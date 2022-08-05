using Microsoft.EntityFrameworkCore;
using MyCourse.Models.Services.Application;
using MyCourse.Models.Services.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//ConfigureService
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
//builder.Services.AddTransient<ICourseService, AdoNetCourseService>();
builder.Services.AddTransient<ICourseService, EfCoreCourseService>();
builder.Services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();

//builder.Services.AddScoped<MyCourseDbContext>();
//builder.Services.AddDbContext<MyCourseDbContext>();
builder.Services.AddDbContextPool<MyCourseDbContext>(optionsBuilder =>
{
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    optionsBuilder.UseSqlite("data Source=Data/MyCourse.db");
});

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

