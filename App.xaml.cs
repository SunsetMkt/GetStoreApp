﻿using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Contracts.Services.Web;
using GetStoreApp.Services.App;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Settings;
using GetStoreApp.Services.Shell;
using GetStoreApp.Services.Web;
using GetStoreApp.UI.Controls.About;
using GetStoreApp.UI.Controls.Home;
using GetStoreApp.UI.Controls.Settings;
using GetStoreApp.ViewModels.Controls.About;
using GetStoreApp.ViewModels.Controls.Home;
using GetStoreApp.ViewModels.Controls.Settings;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WinUIEx;

namespace GetStoreApp
{
    public partial class App : Application
    {
        public IHost Host { get; }

        private IActivationService ActivationService { get; }

        public static WindowEx MainWindow { get; } = new MainWindow();

        public static T GetService<T>()
            where T : class
        {
            if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} 需要在App.xaml.cs中的ConfigureServices中注册。");
            }

            return service;
        }

        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.
            CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                // Services
                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IConfigStorageService, ConfigStorageService>();
                services.AddSingleton<IDataBaseService, DataBaseService>();
                services.AddSingleton<IResourceService, ResourceService>();

                services.AddSingleton<IAria2Service, Aria2Service>();
                services.AddSingleton<IDownloadDataService, DownloadDataService>();

                services.AddSingleton<IHistoryDataService, HistoryDataService>();

                services.AddSingleton<IBackdropService, BackdropService>();
                services.AddSingleton<IDownloadOptionsService, DownloadOptionsService>();
                services.AddSingleton<IHistoryItemValueService, HistoryItemValueService>();
                services.AddSingleton<ILanguageService, LanguageService>();
                services.AddSingleton<ILinkFilterService, LinkFilterService>();
                services.AddSingleton<IRegionService, RegionService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<IUseInstructionService, UseInstructionService>();

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();
                services.AddSingleton<IPageService, PageService>();

                services.AddTransient<IWebViewService, WebViewService>();

                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Window
                services.AddTransient<MainWindow>();
                services.AddTransient<MainWindowViewModel>();

                // Pages and ViewModels
                services.AddTransient<AboutPage>();
                services.AddTransient<AboutViewModel>();
                services.AddTransient<DownloadPage>();
                services.AddTransient<DownloadViewModel>();
                services.AddTransient<HistoryPage>();
                services.AddTransient<HistoryViewModel>();
                services.AddTransient<HomePage>();
                services.AddTransient<HomeViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<WebPage>();
                services.AddTransient<WebViewModel>();

                // Controls and ViewModels
                services.AddTransient<HeaderControl>();
                services.AddTransient<HeaderViewModel>();
                services.AddTransient<InstructionsControl>();
                services.AddTransient<InstructionsViewModel>();
                services.AddTransient<PrecautionControl>();
                services.AddTransient<PrecautionViewModel>();
                services.AddTransient<ReferenceControl>();
                services.AddTransient<ReferenceViewModel>();
                services.AddTransient<SettingsHelpControl>();
                services.AddTransient<SettingsHelpViewModel>();

                services.AddTransient<HistoryItemControl>();
                services.AddTransient<HistoryItemViewModel>();
                services.AddTransient<RequestControl>();
                services.AddTransient<RequestViewModel>();
                services.AddTransient<ResultControl>();
                services.AddTransient<ResultViewModel>();
                services.AddTransient<StatusBarControl>();
                services.AddTransient<StatusBarViewModel>();
                services.AddTransient<TitleControl>();
                services.AddTransient<TitleViewModel>();

                services.AddTransient<BackdropControl>();
                services.AddTransient<BackdropViewModel>();
                services.AddTransient<ClearRecordControl>();
                services.AddTransient<ClearRecordViewModel>();
                services.AddTransient<DownloadOptionsControl>();
                services.AddTransient<DownloadOptionsViewModel>();
                services.AddTransient<HistoryItemValueControl>();
                services.AddTransient<HistoryItemValueViewModel>();
                services.AddTransient<LauguageControl>();
                services.AddTransient<LanguageViewModel>();
                services.AddTransient<LinkFilterControl>();
                services.AddTransient<LinkFilterViewModel>();
                services.AddTransient<RegionControl>();
                services.AddTransient<RegionViewModel>();
                services.AddTransient<ThemeControl>();
                services.AddTransient<ThemeViewModel>();
                services.AddTransient<UseInstructionControl>();
                services.AddTransient<UseInstructionViewModel>();
            })
            .Build();

            ActivationService = GetService<IActivationService>();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            await RunSingleInstanceApp();

            await ActivationService.ActivateAsync(args);
        }

        /// <summary>
        /// 应用程序只运行单个实例
        /// </summary>
        private async Task RunSingleInstanceApp()
        {
            // Get the activation args
            var appArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

            // Get or register the main instance
            var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

            // If the main instance isn't this current instance
            if (!mainInstance.IsCurrent)
            {
                // Redirect activation to that instance
                await mainInstance.RedirectActivationToAsync(appArgs);

                // And exit our instance and stop
                Process.GetCurrentProcess().Kill();
                return;
            }
        }
    }
}
