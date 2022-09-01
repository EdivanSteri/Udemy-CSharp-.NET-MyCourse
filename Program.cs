using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using MyCourse.Customizations.Identity;
using MyCourse.Customizations.ModelBinders;
using MyCourse.Models.Authorization;
using MyCourse.Models.Entities;
using MyCourse.Models.Enums;
using MyCourse.Models.Options;
using MyCourse.Models.Services.Application.Courses;
using MyCourse.Models.Services.Application.Lessons;
using MyCourse.Models.Services.Infrastructure;
using Stripe;
using static MyCourse.Models.Services.Application.Lessons.MemoryCachedLessonService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var builderConfiguration = builder.Configuration;

// Servizi di pagamento: Paypal o Stripe?
builder.Services.AddTransient<IPaymentGateway, PaypalPaymentGateway>();
// builder.Services.AddTransient<IPaymentGateway, StripePaymentGateway>();

builder.Services.AddReCaptcha(builderConfiguration.GetSection("ReCaptcha"));
builder.Services.AddResponseCaching();

builder.Services.AddMvc(options =>{
    CacheProfile homeProfile = new();
    //homeProfile.Duration = Configuration.GetValue<int>("ResponseCache:Home:Duration");
    //homeProfile.Location = Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
    //homeProfile.VaryByQueryKeys = new string[] { "page" };
    builderConfiguration.Bind("ResponseCache:Home", homeProfile);
    options.CacheProfiles.Add("Home", homeProfile);

    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());


    AuthorizationPolicyBuilder policyBuilder = new();
    AuthorizationPolicy policy = policyBuilder.RequireAuthenticatedUser().Build();
    AuthorizeFilter filter = new(policy);
    options.Filters.Add(filter);

});

builder.Services.AddRazorPages(options => {
options.Conventions.AllowAnonymousToPage("/Privacy");
});

var identityBuilder = builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Criteri di validazione della password
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 4;

    // Conferma dell'account
    options.SignIn.RequireConfirmedAccount = true;

    // Blocco dell'account
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

})
  .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
  .AddPasswordValidator<CommonPasswordValidator<ApplicationUser>>();
  /*.AddRoles<IdentityRole>()
  .AddRoleManager<RoleManager<IdentityRole>>();*/

//Usiamo ADO.NET o Entity Framework Core per l'accesso ai dati?
var persistence = Persistence.EfCore;
switch (persistence){
    case Persistence.AdoNet:
            builder.Services.AddTransient<ICourseService, AdoNetCourseService>();
            builder.Services.AddTransient<ILessonService, AdoNetLessonService>();
            builder.Services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();

    //Imposta l'AdoNetUserStore come servizio di persistenza per Identity
    identityBuilder.AddUserStore<AdoNetUserStore>();

    break;

    case Persistence.EfCore:

    //Imposta il MyCourseDbContext come servizio di persistenza per Identity
    identityBuilder.AddEntityFrameworkStores<MyCourseDbContext>();

        builder.Services.AddTransient<ICourseService, EfCoreCourseService>();
        builder.Services.AddTransient<ILessonService, EfCoreLessonService>();

        // Usando AddDbContextPool, il DbContext verrà implicitamente registrato con il ciclo di vita Scoped
        builder.Services.AddDbContextPool<MyCourseDbContext>(optionsBuilder => {
        string connectionString = builderConfiguration.GetSection("ConnectionStrings").GetValue<string>("Database");
        optionsBuilder.UseSqlite(connectionString, options =>
        {
            // Abilito il connection resiliency (tuttavia non è supportato dal provider di Sqlite perché non è soggetto a errori transienti)
            // Info su: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            // options.EnableRetryOnFailure(3);
        });
    });
    break;
}

builder.Services.AddTransient<ICachedCourseService, MemoryCacheCourseService>();
builder.Services.AddTransient<ICachedLessonService, MemoryCacheLessonService>();
builder.Services.AddSingleton<IImagePersister, MagickNetImagePersister>();
builder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();
builder.Services.AddSingleton<IEmailClient, MailKitEmailSender>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, MultiAuthorizationPolicyProvider>();
builder.Services.AddSingleton<ITransactionLogger, LocalTransactionLogger>();



builder.Services.AddScoped<IAuthorizationHandler, CourseAuthorRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CourseLimitRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CourseSubscriberRequirementHandler>();

// Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(nameof(Policy.CourseAuthor), builder =>
    {
        builder.Requirements.Add(new CourseAuthorRequirement());
    });

    options.AddPolicy(nameof(Policy.CourseSubscriber), builder =>
    {
        builder.Requirements.Add(new CourseSubscriberRequirement());
    });

    options.AddPolicy(nameof(Policy.CourseLimit), builder =>
    {
        builder.Requirements.Add(new CourseLimitRequirement(limit: 5));
    });

});



// Options
builder.Services.Configure<CoursesOptions>(builderConfiguration.GetSection("Courses"));
builder.Services.Configure<ConnectionStringsOptions>(builderConfiguration.GetSection("ConnectionStrings"));
builder.Services.Configure<MemoryCacheOptions>(builderConfiguration.GetSection("MemoryCache"));
builder.Services.Configure<KestrelServerOptions>(builderConfiguration.GetSection("Kestrel"));
builder.Services.Configure<SmtpOptions>(builderConfiguration.GetSection("Smtp"));
builder.Services.Configure<UsersOptions>(builderConfiguration.GetSection("Users"));
builder.Services.Configure<PaypalOptions>(builderConfiguration.GetSection("Paypal"));
builder.Services.Configure<StripeOptions>(builderConfiguration.GetSection("Stripe"));


var app = builder.Build();

//if (env.IsDevelopment())
if (app.Environment.IsDevelopment()){
    app.UseDeveloperExceptionPage();
}
else{
    // app.UseExceptionHandler("/Error");
    // Breaking change .NET 5: https://docs.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/5.0/middleware-exception-handler-throws-original-exception
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandlingPath = "/Error",
        AllowStatusCode404Response = true
    });
}

app.UseStaticFiles();

//Nel caso volessi impostare una Culture specifica...
/*var appCulture = CultureInfo.InvariantCulture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(appCulture),
    SupportedCultures = new[] { appCulture }
});*/

//EndpointRoutingMiddleware
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

//EndpointMiddleware
app.UseEndpoints(routeBuilder => {
    routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
    routeBuilder.MapRazorPages().RequireAuthorization();
});


app.Run();

