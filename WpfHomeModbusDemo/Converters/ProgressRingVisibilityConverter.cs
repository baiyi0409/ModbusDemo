using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfHomeModbusDemo.Converters
{
    public class ProgressRingVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;

            // 尝试转换为 double（覆盖 int, float, decimal, string 等）
            try
            {
                // 使用 invariant culture 避免本地化问题（如逗号小数点）
                var number = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return number > 0 ? Visibility.Visible : Visibility.Hidden;
            }
            catch (FormatException)
            {
                // 无法解析为数字，例如 "abc"
                return Visibility.Hidden;
            }
            catch (InvalidCastException)
            {
                // 类型无法转换，例如传入了一个自定义对象
                return Visibility.Hidden;
            }
            catch (OverflowException)
            {
                // 数值溢出（极少见）
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
