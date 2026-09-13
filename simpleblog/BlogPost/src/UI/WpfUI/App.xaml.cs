using Application.Interface;
using Application.UseCase;
using Infrastructure.Configurations;
using Infrastructure.Repo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using WpfUI.ViewModel;

namespace WpfUI
{

    public partial class App : System.Windows.Application
    {
        private IHost? _host;
        
        protected override async void OnStartup(StartupEventArgs e)
        {
            var culture = new CultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            
            // Imposta la lingua per i controlli WPF
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

            _host = Host.CreateDefaultBuilder()
                        .ConfigureAppConfiguration((context, config) =>
                        { 
                            config.SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        })
                        .ConfigureServices((context, services) =>
                        {
                            services.Configure<FirebaseSettings>(
                            context.Configuration.GetSection("Firebase"));

                            services.AddSingleton<IBlogRepository>(sp =>
                            {
                                var settings = sp.GetRequiredService<IOptions<FirebaseSettings>>().Value;
                                return new BlogPostFirebaseRepo(settings.DatabaseUrl);
                            });

                            services.AddSingleton<IBlogService, BlogService>();

                            services.AddSingleton<MainViewModel>();
                            services.AddSingleton<MainWindow>();

                        })
                        .Build();

            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if(_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }
    }

}
