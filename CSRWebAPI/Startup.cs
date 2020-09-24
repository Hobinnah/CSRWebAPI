using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CSRWebAPI.Models;
using CSRWebAPI.Repositories;
using CSRWebAPI.Repositories.DTO;
using CSRWebAPI.Repositories.Identity;
using CSRWebAPI.Repositories.Implementations;
using CSRWebAPI.Repositories.Interfaces;
using CSRWebAPI.Repositories.Models;
using CSRWebAPI.Services.Implementations;
using CSRWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CSRWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static readonly string _defaultCorsPolicyName = "http://localhost:4200";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setupAction => { setupAction.ReturnHttpNotAcceptable = true; }).AddXmlDataContractSerializerFormatters();
            services.AddMemoryCache();
            services.AddAutoMapper(typeof(Startup));

            // ========== Connection String ============
            services.AddDbContextPool<CSRDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            #region ===== DI =====
            services.AddTransient<IBankRepository, BankRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<IShareholderAddressRepository, ShareholderAddressRepository>();
            services.AddTransient<IShareholderBankRepository, ShareholderBankRepository>();
            services.AddTransient<IShareholderRepository, ShareholderRepository>();
            services.AddTransient<IStateRepository, StateRepository>();

            services.AddTransient<IBankService, BankService>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<IShareholderAddressService, ShareholderAddressService>();
            services.AddTransient<IShareholderBankService, ShareholderBankService>();
            services.AddTransient<IShareholderService, ShareholderService>();
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<IStateService, StateService>();

            #endregion

            #region ===== Add Identity ========

            services.AddIdentity<User, Role>().AddEntityFrameworkStores<CSRDBContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._!+";
                options.User.RequireUniqueEmail = true;
            });

            #endregion


            #region ===== Add Cookies =====
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            #endregion

            #region ===== JWT configuration =====
            /*
             This is used to provide user authentication before access to the API can be granted.
             However, on this occation, I didnt implement the login aspect because of limited avilable time.
             */
            JwtSettings settings = GetJwtSettings();

            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(settings.Lifespan)
                };
            });
            services.AddSingleton<JwtSettings>();
            #endregion

            #region ===== CORS configuration =====

            
            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                      builder.SetIsOriginAllowed((host) => true).AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "http://appsvr.westeurope.cloudapp.azure.com/CSRAPI").AllowCredentials().Build();
                });

            });

           
            #endregion

            #region ===== Swagger generator =====
           //Register the Swagger generator, defining 1 or more Swagger documents
           services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CSR Web API",
                    Description = "CSR Web API Suits, an ASP.NET Core Web API",
                    TermsOfService = new Uri("https://cardinalstoneregistrars.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "",
                        Email = string.Empty,
                        Url = new Uri("https://cardinalstoneregistrars.com/csr/spboyer"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://cardinalstoneregistrars.com/csr/license"),
                    }
                });
            });

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {

            app.UseCors(_defaultCorsPolicyName);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {

                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault occurred. Please try again later.");
                    });

                });
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            // http://localhost:51742/swagger/index.html
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "CSR Core WebAPI");
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Uploads")),
                RequestPath = new PathString("/Uploads")
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Uploads")),
                RequestPath = new PathString("/Uploads")
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("EnableCORS");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
            // Seeding the database with data.
            InitializeResources(serviceProvider).Wait();

        }

        public JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings();

            settings.Key = Configuration["JwtSettings:Key"];
            settings.Issuer = Configuration["JwtSettings:Issuer"];
            settings.Audience = Configuration["JwtSettings:Audience"];
            settings.Lifespan = Convert.ToInt32(Configuration["JwtSettings:Lifespan"]);

            return settings;
        }

        private async Task InitializeResources(IServiceProvider serviceProvider)
        {

            try
            {
                var bankRepository = serviceProvider.GetRequiredService<IBankRepository>();
                var countryRepository = serviceProvider.GetRequiredService<ICountryRepository>();
                var companyRepository = serviceProvider.GetRequiredService<ICompanyRepository>();
                var stateRepository = serviceProvider.GetRequiredService<IStateRepository>();

                #region ==================== Create Banks ====================

                Bank bank = new Bank();

                // Access Bank Plc
                bank = await bankRepository.Single(x => x.Name.ToLower() == "access bank plc");
                if (bank == null || bank.BankID == null || bank.BankID == 0)
                {
                    bank = new Bank();
                    bank.Name = string.Format("Access Bank Plc");
                    await bankRepository.Create(bank);
                    await bankRepository.Save();
                    bank = null;
                }

                // Firstbank Of Nigeria Ltd
                bank = await bankRepository.Single(x => x.Name.ToLower() == "firstbank of nigeria ltd");
                if (bank == null || bank.BankID == null || bank.BankID == 0)
                {
                    bank = new Bank();
                    bank.Name = string.Format("Firstbank Of Nigeria Ltd");
                    await bankRepository.Create(bank);
                    await bankRepository.Save();
                    bank = null;
                }

                #endregion

                #region ==================== Create Country ====================

                Country country = new Country();

                // Nigeria
                country = await countryRepository.Single(x => x.Name.ToLower() == "nigeria");
                if (country == null || country.CountryID == null || country.CountryID == 0)
                {
                    country = new Country();
                    country.Name = string.Format("Nigeria");
                    await countryRepository.Create(country);
                    await countryRepository.Save();
                    country = null;
                }

                #endregion

                #region ==================== Create Company ====================

                Company company = new Company();

                // ACORN PET. PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "ACORN PET. PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("ACORN PET. PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // AFRIK PHARMACEUTICALS PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "AFRIK PHARMACEUTICALS PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("AFRIK PHARMACEUTICALS PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // AG MORTGAGE BANK PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "AG MORTGAGE BANK PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("AG MORTGAGE BANK PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // AG LEVENTISPLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "AG LEVENTIS PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("AG LEVENTIS PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // ARBICO NIGERIA PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "ARBICO NIGERIA PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("ARBICO NIGERIA PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // BANKERS WAREHOUSE PLC 
                company = await companyRepository.Single(x => x.Name.ToUpper() == "BANKERS WAREHOUSE PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("BANKERS WAREHOUSE PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // BETA GLASS PLC 
                company = await companyRepository.Single(x => x.Name.ToUpper() == "BETA GLASS PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("BETA GLASS PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // CAPITAL HOTELS PLC 
                company = await companyRepository.Single(x => x.Name.ToUpper() == "CAPITAL HOTELS PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("CAPITAL HOTELS PLC ");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // ELLAH LAKES PLC 
                company = await companyRepository.Single(x => x.Name.ToUpper() == "ELLAH LAKES PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("ELLAH LAKES PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // EVANS MEDICALS PLC 
                company = await companyRepository.Single(x => x.Name.ToUpper() == "EVANS MEDICALS PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("EVANS MEDICALS PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // FCMB BOND 1
                company = await companyRepository.Single(x => x.Name.ToUpper() == "FCMB BOND 1");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("FCMB BOND 1");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // FCMB BOND 2
                company = await companyRepository.Single(x => x.Name.ToUpper() == "FCMB BOND 2");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("FCMB BOND 2");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // FCMB BOND 3
                company = await companyRepository.Single(x => x.Name.ToUpper() == "FCMB BOND 3");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("FCMB BOND 3");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // FCMB GROUP PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "FCMB GROUP PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("FCMB GROUP PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // FIDSON BOND
                company = await companyRepository.Single(x => x.Name.ToUpper() == "FIDSON BOND");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("FIDSON BOND");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // G. CAPPA PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "G. CAPPA PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("G. CAPPA PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // GUINEA INSURANCE PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "GUINEA INSURANCE PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("GUINEA INSURANCE PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // JOS INT. BREWERIES PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "JOS INT. BREWERIES PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("JOS INT. BREWERIES PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LAFARGE AFRICA PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LAFARGE AFRICA PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LAFARGE AFRICA PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LAFARGE BOND 1
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LAFARGE BOND 1");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LAFARGE BOND 1");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LAFARGE BOND 2
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LAFARGE BOND 2");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LAFARGE BOND 2");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LAPO MICROFINANCE BANK
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LAPO MICROFINANCE BANK");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LAPO MICROFINANCE BANK");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LAW UNION & ROCK INS. PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LAW UNION & ROCK INS. PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LAW UNION & ROCK INS. PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LEGACY EQUITY FUND
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LEGACY EQUITY FUND");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LEGACY EQUITY FUND");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LEGACY DEBT FUND
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LEGACY DEBT FUND");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LEGACY DEBT FUND");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LEGACY USD BOND FUND
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LEGACY USD BOND FUND");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LEGACY USD BOND FUND");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // LIVESTOCK FEEDS PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "LIVESTOCK FEEDS PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("LIVESTOCK FEEDS PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // MORISON INDUSTRIES PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "MORISON INDUSTRIES PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("MORISON INDUSTRIES PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // NAHCO BOND
                company = await companyRepository.Single(x => x.Name.ToUpper() == "NAHCO BOND");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("NAHCO BOND");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // NAHCO AVIANCE PLC 
                company = await companyRepository.Single(x => x.Name.ToUpper() == "NAHCO AVIANCE PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("NAHCO AVIANCE PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // NEWPAK PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "NEWPAK PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("NEWPAK PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // N.G.C PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "N.G.C PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("N.G.C PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // NPF MICROFINANCE BANK PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "NPF MICROFINANCE BANK PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("NPF MICROFINANCE BANK PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // OKOMU OIL PALM PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "OKOMU OIL PALM PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("OKOMU OIL PALM PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // PREMIER PAINTS PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "PREMIER PAINTS PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("PREMIER PAINTS PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // ROYAL EXCHANGE PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "ROYAL EXCHANGE PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("ROYAL EXCHANGE PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // SKYE BANK PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "SKYE BANK PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("SKYE BANK PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // TOTAL NIGERIA PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "TOTAL NIGERIA PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("TOTAL NIGERIA PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // TRANS-NATIONWIDE EXP. PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "TRANS-NATIONWIDE EXP. PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("TRANS-NATIONWIDE EXP. PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // UBN PROPERTY COMPANY PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "UBN PROPERTY COMPANY PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("UBN PROPERTY COMPANY PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // UNION BANK OF NIGERIA PLC
                company = await companyRepository.Single(x => x.Name.ToUpper() == "UNION BANK OF NIGERIA PLC");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("UNION BANK OF NIGERIA PLC");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }

                // WOMEN INVESTMENT FUND
                company = await companyRepository.Single(x => x.Name.ToUpper() == "WOMEN INVESTMENT FUND");
                if (company == null || company.CompanyID == null || company.CompanyID == 0)
                {
                    company = new Company();
                    company.Name = string.Format("WOMEN INVESTMENT FUND");
                    await companyRepository.Create(company);
                    await companyRepository.Save();
                    company = null;
                }
                #endregion

                #region ==================== Create States ====================

                State state = new State();

                // Lagos
                state = await stateRepository.Single(x => x.Name.ToLower() == "lagos");
                if (state == null || state.StateID == null || state.StateID == 0)
                {
                    state = new State();
                    state.Name = string.Format("Lagos");
                    await stateRepository.Create(state);
                    await stateRepository.Save();
                    state = null;
                }

                // Ogun
                state = await stateRepository.Single(x => x.Name.ToLower() == "ogun");
                if (state == null || state.StateID == null || state.StateID == 0)
                {
                    state = new State();
                    state.Name = string.Format("Ogun");
                    await stateRepository.Create(state);
                    await stateRepository.Save();
                    state = null;
                }

                #endregion
            }
            catch { }

        }
    }
}
