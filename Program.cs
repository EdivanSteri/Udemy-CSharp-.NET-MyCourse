using MyCourse.Models.Services.Application;

var builder = WebApplication.CreateBuilder(args);

//ConfigureService
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddTransient<ICourseService, CourseService>();

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

