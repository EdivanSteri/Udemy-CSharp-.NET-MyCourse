using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCourse.Customizations.Identity;
using MyCourse.Customizations.ModelBinders;
using MyCourse.Models.Entities;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Application.Courses;
using MyCourse.Models.Services.Application.Lessons;
using MyCourse.Models.Services.Infrastructure;
using static MyCourse.Models.Services.Application.Lessons.MemoryCachedLessonService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var builderConfiguration = builder.Configuration;


//ConfigureService

builder.Services.AddResponseCaching();
builder.Services.AddRazorPages();

builder.Services.AddMvc(options => {
    //options.EnableEndpointRouting = false;
    var homeProfile = new CacheProfile();
    //homeProfile.Duration = builder.Configuration.GetValue<int>("ResponseCache:Home:Duration");
    //homeProfile.Location = builderConfiguration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
    //homeProfile.VaryByQueryKeys = new string[] { "page" };
    builderConfiguration.Bind("ResponseCache:Home", homeProfile);
    options.CacheProfiles.Add("Home", homeProfile);

    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());

});
//builder.Services.AddTransient<ICourseService, AdoNetCourseService>();
builder.Services.AddTransient<ICourseService, EfCoreCourseService>();
//builder.Services.AddTransient<ILessonService, AdoNetLessonService>();
builder.Services.AddTransient<ILessonService, EfCoreLessonService>();
//builder.Services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();
builder.Services.AddTransient<ICachedCourseService, MemoryCacheCourseService>();
builder.Services.AddTransient<ICachedLessonService, MemoryCacheLessonService>();
builder.Services.AddSingleton<IImagePersister, MagickNetImagePersister>();
builder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    //CRITERI PASSWORD
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredUniqueChars = 4;
    options.Password.RequireLowercase = true;

    //CONFERMA DELL?ACCOUNT'ACCOUNT
    options.SignIn.RequireConfirmedAccount = true;

    //BLOCCCO DELL'ACCOUNT
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
 .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
 .AddPasswordValidator<CommonPasswordValidator<ApplicationUser>>()
 .AddEntityFrameworkStores<MyCourseDbContext>();
 //.AddUserStore<AdoNetUserStore>();

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
builder.Services.Configure<SmtpOptions>(builderConfiguration.GetSection("Smtp"));





var app = builder.Build();

//Configure Middleware
if (app.Environment.IsDevelopment())
{
     
    app.UseDeveloperExceptionPage();
}
else{
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

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(routeBuilder =>
{
    routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
    routeBuilder.MapRazorPages();
});
//app.UseMvcWithDefaultRoute(); 
/*app.UseMvc(routeBuilder =>
{
    routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
});*/

app.Run();

