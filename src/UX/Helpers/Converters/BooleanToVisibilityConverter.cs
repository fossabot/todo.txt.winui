﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Seemon.Todo.Helpers.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => ((bool)value) ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
