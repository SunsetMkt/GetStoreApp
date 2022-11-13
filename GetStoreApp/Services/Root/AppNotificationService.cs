﻿using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.AppNotifications;
using System;
using System.Web;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public class AppNotificationService : IAppNotificationService
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public INotificationService NotificationService { get; } = ContainerHelper.GetInstance<INotificationService>();

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        ~AppNotificationService()
        {
            Unregister();
        }

        public void Initialize()
        {
            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

            AppNotificationManager.Default.Register();
        }

        /// <summary>
        /// 处理应用通知后的响应事件
        /// </summary>
        public void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            HandleAppNotification(args);
        }

        /// <summary>
        /// 处理应用通知
        /// </summary>
        public void HandleAppNotification(AppNotificationActivatedEventArgs args)
        {
            dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
            {
                // 先设置应用窗口的显示方式
                WindowHelper.ShowAppWindow();

                switch (HttpUtility.ParseQueryString(args.Argument)["action"])
                {
                    case "OpenApp":
                        {
                            break;
                        }
                    case "CheckNetWorkConnection":
                        {
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                            break;
                        }
                    case "DownloadingNow":
                        {
                            if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
                            }
                            break;
                        }
                    case "NotDownload":
                        {
                            if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
                            }
                            break;
                        }
                    case "ViewDownloadPage":
                        {
                            if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
                            }
                            break;
                        }
                    case "DownloadCompleted":
                        {
                            if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            });
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        public void Show(string notificationKey, params string[] notificationContent)
        {
            if (!NotificationService.AppNotification)
            {
                return;
            }

            AppNotification notification;

            switch (notificationKey)
            {
                case "DownloadAborted":
                    {
                        if (notificationContent.Length == 0)
                        {
                            return;
                        }

                        // 有任务处于正在下载状态时被迫中断显示相应的通知
                        if (notificationContent[0] == "Downloading")
                        {
                            notification = new AppNotification(ResourceService.GetLocalized("/Notification/DownloadingNowOfflineMode"));
                            AppNotificationManager.Default.Show(notification);
                        }

                        // 没有任务下载时显示相应的通知
                        else if (notificationContent[0] == "NotDownload")
                        {
                            notification = new AppNotification(ResourceService.GetLocalized("/Notification/NotDownloadOfflineMode"));
                            AppNotificationManager.Default.Show(notification);
                        }
                        break;
                    }

                // 所有任务下载完成时显示通知
                case "DownloadCompleted":
                    {
                        notification = new AppNotification(ResourceService.GetLocalized("/Notification/DownloadCompleted"));
                        AppNotificationManager.Default.Show(notification);
                        break;
                    }

                // 安装应用显示相应的通知
                case "InstallApp":
                    {
                        if (notificationContent.Length == 0)
                        {
                            return;
                        }

                        // 成功安装应用通知
                        if (notificationContent[0] == "Successfully")
                        {
                            notification = new AppNotification(string.Format(ResourceService.GetLocalized("/Notification/InstallSuccessfully"), notificationContent[1]));
                            AppNotificationManager.Default.Show(notification);
                        }
                        else if (notificationContent[0] == "Error")
                        {
                            notification = new AppNotification(string.Format(ResourceService.GetLocalized("/Notification/InstallError"), notificationContent[1], notificationContent[2]));
                            AppNotificationManager.Default.Show(notification);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 注销通知服务
        /// </summary>
        public void Unregister()
        {
            AppNotificationManager.Default.NotificationInvoked -= OnNotificationInvoked;
            AppNotificationManager.Default.Unregister();
        }
    }
}
