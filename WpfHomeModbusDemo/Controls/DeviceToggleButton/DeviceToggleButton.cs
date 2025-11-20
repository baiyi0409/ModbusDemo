using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WpfHomeModbusDemo.Controls
{
    public class DeviceToggleButton: ToggleButton
    {
        //创建需要的依赖属性
        //设备名称
        public static readonly DependencyProperty DeviceNameProperty =
            DependencyProperty.Register("DeviceName", typeof(string), typeof(DeviceToggleButton), new
                PropertyMetadata(default(string)));

        //设备类型
        public static readonly DependencyProperty DeviceTypeProperty =
            DependencyProperty.Register("DeviceType", typeof(string), typeof(DeviceToggleButton), new
        PropertyMetadata(default(string)));

        //设备图标
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry), typeof(DeviceToggleButton), new 
                PropertyMetadata(default(Geometry)));

        //设备数值
        public static readonly DependencyProperty DeviceValueProperty =
            DependencyProperty.Register("DeviceValue", typeof(object), typeof(DeviceToggleButton), new
                PropertyMetadata(default(object)));

        public string DeviceName
        {
            get => (string)GetValue(DeviceNameProperty);
            set => SetValue(DeviceNameProperty, value);
        }

        public string DeviceType
        {
            get => (string)GetValue(DeviceTypeProperty);
            set => SetValue(DeviceTypeProperty, value);
        }

        public Geometry Icon
        {
            get => (Geometry)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public object DeviceValue
        {
            get => (object)GetValue(DeviceValueProperty);
            set => SetValue(DeviceValueProperty, value);
        }
    }
}
