﻿using Microsoft.UI.Xaml.Data;
using System;
using System.IO;

namespace GetStoreApp.Converters.Checks
{
    /// <summary>
    /// 检测文件是否存在转换器
    /// </summary>
    public class FileExistCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return false;
            }

            string FilePath = System.Convert.ToString(value);
            string param = System.Convert.ToString(parameter);

            if (param is "Reverse")
            {
                return !File.Exists(FilePath);
            }
            else
            {
                return File.Exists(FilePath);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
