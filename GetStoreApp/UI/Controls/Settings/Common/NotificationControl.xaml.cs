﻿using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Common;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    public sealed partial class NotificationControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public NotificationViewModel ViewModel { get; } = IOCHelper.GetService<NotificationViewModel>();

        public NotificationControl()
        {
            InitializeComponent();
        }
    }
}