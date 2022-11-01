﻿using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    public sealed partial class AppExitControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public AppExitViewModel ViewModel { get; } = IOCHelper.GetService<AppExitViewModel>();

        public AppExitControl()
        {
            InitializeComponent();
        }
    }
}