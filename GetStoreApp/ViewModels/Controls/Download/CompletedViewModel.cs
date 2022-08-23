﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public class CompletedViewModel : ObservableRecipient
    {
        private IDownloadDataService DownloadDataService { get; } = IOCHelper.GetService<IDownloadDataService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private StorageFolder DownloadFolder { get; }

        public ObservableCollection<DownloadModel> CompletedDataList { get; } = new ObservableCollection<DownloadModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        private bool _isCompletedEmpty = true;

        public bool IsCompletedEmpty
        {
            get { return _isCompletedEmpty; }

            set { SetProperty(ref _isCompletedEmpty, value); }
        }

        public IAsyncRelayCommand OpenFolderCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        });

        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            await SelectNoneAsync();
            IsSelectMode = true;
        });

        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            foreach (DownloadModel downloadItem in CompletedDataList)
            {
                downloadItem.IsSelected = true;
            }
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(SelectNoneAsync);

        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand(DeleteAsync);

        public IAsyncRelayCommand DeleteWithFileCommand { get; }

        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand DeleteTaskCommand { get; }

        public CompletedViewModel()
        {
            DownloadFolder = DownloadOptionsService.DownloadFolder;

            Messenger.Register<CompletedViewModel, PivotSelectionMessage>(this, async (completedViewModel, pivotSelectionMessage) =>
            {
                // 切换到已完成页面时，更新当前页面的数据
                if (pivotSelectionMessage.Value == 1)
                {
                }

                // 从下载页面离开时，关闭所有事件。并注销所有消息服务
                else if (pivotSelectionMessage.Value == -1)
                {
                    Messenger.UnregisterAll(this);
                }
                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        private async Task DeleteAsync()
        {
            List<DownloadModel> SelectedCompletedDataList = CompletedDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedCompletedDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog().ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                //await DownloadDataService.DeleteDownloadDataAsync(SelectedCompletedDataList);

                // 如果有正在下载的服务，从下载列表中删除
                //if (DownloadingList.Any())
                //{
                //    foreach (DownloadModel downloadItem in DownloadingList)
                //    {
                //        //Aria2Service.DeleteSelectedAsync();
                //    }
                //}
                await GetDownloadDataListAsync();
            }
        }

        /// <summary>
        /// 从数据库中加载已下载完成的数据
        /// </summary>
        private async Task GetDownloadDataListAsync()
        {
            Tuple<List<DownloadModel>, bool> DownloadData = await DownloadDataService.QueryAllDownloadDataAsync();

            List<DownloadModel> DownloadRawList = DownloadData.Item1;

            IsCompletedEmpty = DownloadData.Item2;

            try
            {
                ConvertRawListToDisplayList(ref DownloadRawList);
            }
            catch (Exception)
            {
                ConvertRawListToDisplayList(ref DownloadRawList);
            }
        }

        /// <summary>
        /// 将原始数据转换为在UI界面上呈现出来的数据
        /// </summary>
        private void ConvertRawListToDisplayList(ref List<DownloadModel> downloadRawList)
        {
            CompletedDataList.Clear();

            foreach (DownloadModel downloadRawData in downloadRawList)
            {
                CompletedDataList.Add(downloadRawData);
            }
        }

        /// <summary>
        /// 点击全部不选按钮时，让复选框的下载记录全部不选
        /// </summary>
        private async Task SelectNoneAsync()
        {
            foreach (DownloadModel downloadItem in CompletedDataList)
            {
                downloadItem.IsSelected = false;
            }

            await Task.CompletedTask;
        }
    }
}