﻿using Microsoft.UI.Xaml;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    public class ResultModel : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// 在多选模式下，该行信息是否被选择的标志
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        public string FileLink
        {
            get { return (string)GetValue(FileLinkProperty); }
            set { SetValue(FileLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileLinkProperty =
            DependencyProperty.Register("FileLink", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        public string FileLinkExpireTime
        {
            get { return (string)GetValue(FileLinkExpireTimeProperty); }
            set { SetValue(FileLinkExpireTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileLinkExpireTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileLinkExpireTimeProperty =
            DependencyProperty.Register("FileLinkExpireTime", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        public string FileSHA1
        {
            get { return (string)GetValue(FileSHA1Property); }
            set { SetValue(FileSHA1Property, value); }
        }

        // Using a DependencyProperty as the backing store for FileSHA1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileSHA1Property =
            DependencyProperty.Register("FileSHA1", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        public string FileSize
        {
            get { return (string)GetValue(FileSizeProperty); }
            set { SetValue(FileSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileSizeProperty =
            DependencyProperty.Register("FileSize", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}