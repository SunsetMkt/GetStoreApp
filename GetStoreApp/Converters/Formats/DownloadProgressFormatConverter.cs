﻿using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// 下载进度文字提示转换器
    /// </summary>
    public class DownloadProgressFormatConverter : IValueConverter
    {
        private IResourceService ResourceService = IOCHelper.GetService<IResourceService>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            int result = System.Convert.ToInt32(value);

            return string.Format("{0}{1}", ResourceService.GetLocalized("/Download/Progress"), result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}