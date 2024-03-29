using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Album.Mail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using razorweb.Models;

namespace razorweb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            var mailSetting =  Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailSetting);
            services.AddSingleton<IEmailSender,SendMailService>();

            services.AddRazorPages();
            //  Connect Database
            services.AddDbContext<MyBlogContext>(options => {
                string connectstring = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectstring);
            });
            // End
            // Dang ky Identity
            /*
            Thêm triển khai EF lưu trữ thông tin về Idetity :AddEntityFrameworkStores
            Thêm Token Provider - nó sử dụng để phát sinh token (reset password, confirm email ...)
            */
            // services.AddDefaultIdentity<AppUser>().AddEntityFrameworkStores<MyBlogContext>()
            // .AddDefaultTokenProviders();
             services.AddIdentity<AppUser,IdentityRole>().AddEntityFrameworkStores<MyBlogContext>()
            .AddDefaultTokenProviders();
            // Cau hinh Identity
            services.Configure<IdentityOptions> (options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
            // IdentityUser user;
            // IdentityDbContext context;
        }
    }
}
/*
CREATE READ Update ,delete (CRUD)
dotnet ef migrations add [name]
dotnet ef database update
dotnet aspnet-codegenerator razorpage -m razorweb.Models.Article -dc razorweb.Models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries
dotnet aspnet-codegenerator identity -dc razorweb.Models.MyBlogContext
*/