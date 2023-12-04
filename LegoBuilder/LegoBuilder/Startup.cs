using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using LegoBuilder.Security;
using LegoBuilder.Models;
using LegoBuilder.Services.Models;
using System;
using LegoBuilder.Services;
using LegoBuilder.SqlDaos;

namespace LegoBuilder
{
    public class Startup
    {
        // stores the configuration from I don't know where
        public IConfiguration Configuration { get; } 
        // this is called by program CS though I don't know where it gets the configuration variable from
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // not sure about IServicesCollection, it's from the dependencyinjection namespace but when I tried
        // it solo, it didn't pull in the methods in the interface and I don't know where that code lives
        //      it doesn't say anything about adding the json in here?
        //      but it is added somewhere in Iconfiguration
        // apparently dependency injection is native to asp.net, and so we can pass configuration value into controllers 
        //      Tom said this is where stuff is made to be put on a shelf to be used in controllers
        // in here is where we add the dependency injections we want

        // dependency injection is where you have an interface in a constructor for whatever you want to 
        //      from reddit:
        //      dependency injection is when you pass things to a class through its constructor
        //      you don't have to instantiate those things in the class before making that class, that's done here
        // dependently inject (if that's a term...)
        // then elsewhere all the items that the interface can work with are built
        //      the "services.AddSingleton" bit checks for an interfaceTypeIWant,
        //      checks the constructors of those objects using it
        //      then will go out and build the objects required for those constructors
        // this is the elsewhere, they're all built and added as singletons to this services variable
        // where that lives... and how it can be in the parameter... I don't know
        //      this doesn't say anything about the builder stuff
        /*
        That's what constructors are for.
        It lets us easily see everything a class needs.
        We don't get surprised by additional dependencies halfway into a class.
        It's easy to swap out what we give the class with something else.
        Because of that, we make it easier to unit-test our class.
        This style forces us to write code that is 'loosely-coupled'.
        When code is loosely-coupled, changing one part of our program is less likely to break other parts.
        When code is loosely-coupled, you can easily understand it one piece at a time. You don't need to keep it all in your head at once.
         */
        public void ConfigureServices(IServiceCollection services)
        {
            // add services for the controllers ???
            services.AddControllers();
             

            // somehow calls the connection string from the API settings for the database
            // configuration is the instantiated object referring to the json
            string connectionString = Configuration.GetConnectionString("Project");

            // based on the key I supplied it (yay me!)
            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes(Configuration["JwtSecret"]);
            // this creates ??? the jwt token passed back
            // presumably don't do anything with sub
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[JwtRegisteredClaimNames.Sub] = "sub";
            // use of a lambda function to add... authentication and the bearer information???
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    NameClaimType = "name"
                };
            });
            // adds cross origin resource sharing ???
            services.AddCors();

            // this is where I create my daos for use and add it to the overall services package
            // Dependency Injection configuration
            services.AddSingleton<ITokenGenerator>(sp => new JwtGenerator(Configuration["JwtSecret"]));
            services.AddSingleton<IPasswordHasher>(sp => new PasswordHasher());

            // it's Configuration that holds the appsettings json stuff, you call it like a dictionary
            services.AddTransient<IRebrickableAPI>(sp => new RebrickableAPI(Configuration["RebrickableAPIURL"], Configuration["APIKey"]));
            //      Don't know of an easier way to generate teh authenticated Api Service aside from making the rebrickable api in it
            services.AddTransient<IAuthenticatedApiServiceBase>(sp => new AuthenticatedRebrickableApiServiceBase(new RebrickableAPI(Configuration["RebrickableAPIURL"], Configuration["APIKey"])));
            
            services.AddTransient<IAuthenticatedApiServiceById>(sp => new AuthenticatedRebrickableApiServiceById(new RebrickableAPI(Configuration["RebrickableAPIURL"], Configuration["APIKey"])));
            services.AddTransient<IAuthenticatedApiServiceAll>(sp => new AuthenticatedRebrickableApiServiceAll(new RebrickableAPI(Configuration["RebrickableAPIURL"], Configuration["APIKey"])));

            // lego url to add to the pantry
            // singleton, since apparently you use a single httpclicent for all requests
            services.AddSingleton<ILegoUrlGetter>(sp => new LegoUrlGetter(Configuration["LegoUrl"]));

            // if the json has grouped information in an object, you have to serialize it into a class
            // instantiate the class
            // then bind the section to object
            // then use that in a singleton to make it available for use
            //IRebrickableAPI rebrickableAPI = new RebrickableAPI();
            //Configuration.GetSection("RebrickableApi").Bind(rebrickableAPI);
            //services.AddSingleton<IRebrickableAPI>(rebrickableAPI);

            services.AddTransient(sp => new BaseSqlDao(connectionString));
            services.AddTransient<IUserSqlDao>(sp => new UserSqlDao(connectionString));
            services.AddTransient<IColourSqlDao>(sp => new ColourSqlDao(connectionString));
            services.AddTransient<IPartCatIdSqlDao>(sp => new PartCatIdSqlDao(connectionString));
            services.AddTransient<IPartSqlDao>(sp => new PartSqlDao(connectionString));
            services.AddTransient<ISetSqlDao>(sp => new SetSqlDao(connectionString));
            services.AddTransient<IThemeSqlDao>(sp => new ThemeSqlDao(connectionString));
            services.AddTransient<ISetPartsSqlDao>(sp => new SetPartsSqlDao(connectionString));
            services.AddTransient<IUserSetsSqlDao>(sp => new UserSetsSqlDao(connectionString));

            // adds swagger and the use of a bearer token to access my api
            // Swagger Documentation
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = Configuration["APIVersion"],
                    Title = "LegoBuilder API",
                    Description = "For supporting aseelt's 'What Can I Build?' side project"
                });
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer token is required for access.",
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                s.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });
            });

        }

        // builds the swagger stuff
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "13.1026");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
