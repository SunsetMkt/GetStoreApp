﻿using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class DownloadCreateNotification : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DownloadCreateViewModel ViewModel { get; } = IOCHelper.GetService<DownloadCreateViewModel>();

        public object[] Notification { get; }

        public DownloadCreateNotification(object[] notification)
        {
            Notification = notification;
            InitializeComponent();
            ViewModel.Initialize(Convert.ToBoolean(notification[0]), Convert.ToBoolean(notification[1]));
        }

        public void CreateSelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            if (Notification.Length > 2)
            {
                CreateSelectedSuccess.Text = string.Format(ResourceService.GetLocalized("/Notification/DownloadSelectedCreateSuccessfully"), Notification[2]);
            }
        }

        public void CreateSelectedFailedLoaded(object sender, RoutedEventArgs args)
        {
            if (Notification.Length > 2)
            {
                CreateSelectedFailed.Text = string.Format(ResourceService.GetLocalized("/Notification/DownloadSelectedCreateFailed"), Notification[2]);
            }
        }

        public bool ControlLoad(bool createState, bool isMultiSelected, int visibilityFlag)
        {
            if (visibilityFlag == 1 && createState && !isMultiSelected)
            {
                return true;
            }
            else if (visibilityFlag == 2 && (!createState && !isMultiSelected))
            {
                return true;
            }
            else if (visibilityFlag == 3 && (createState && isMultiSelected))
            {
                return true;
            }
            else if (visibilityFlag == 4 && (!createState && isMultiSelected))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}