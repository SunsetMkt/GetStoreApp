﻿using GetStoreAppWebView.Extensions.DataType.Enums;
using Windows.Storage;

namespace GetStoreAppWebView.Services.Root
{
    /// <summary>
    /// 存储运行结果服务
    /// </summary>
    public static class ResultService
    {
        private const string result = "Result";

        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer resultContainer;

        /// <summary>
        /// 初始化结果记录存储服务
        /// </summary>
        public static void Initialize()
        {
            resultContainer = localSettingsContainer.CreateContainer(result, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 保存结果存储信息
        /// </summary>
        public static void SaveResult(StorageDataKind dataKind, string value)
        {
            resultContainer.Values[nameof(StorageDataKind)] = dataKind.ToString();
            resultContainer.Values[dataKind.ToString()] = value;
        }
    }
}
