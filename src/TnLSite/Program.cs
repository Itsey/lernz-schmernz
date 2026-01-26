using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using TnLSite.Data;

namespace TnLSite {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);


            var h = new TCPHandler("127.0.0.1", 9060, true);
            h.SetFormatter(new FlimFlamV4Formatter());
            Bilge.AddHandler(h);
            Bilge.SetConfigurationResolver((a, b) => {
                return System.Diagnostics.SourceLevels.Verbose;
            });

            Bilge b = new Bilge();
            Bilge.Alert.Online("TnL");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<TnLSite.Data.RepositoryBase, TnLSite.Data.FileUserRepository>();
            builder.Services.AddSingleton<TnLSite.Services.IAccountService, TnLSite.Services.AccountService>();
            builder.Services.AddSingleton<TnLSite.Services.ITokenService, TnLSite.Services.InMemoryTokenService>();
            builder.Services.AddSingleton<DynamicTrace>();

            var app = builder.Build();

            RepositoryBase.Initialize(app.Environment.ContentRootPath);


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllers();
            app.MapRazorPages()
                .WithStaticAssets();

            app.Run();
        }
    }
}
