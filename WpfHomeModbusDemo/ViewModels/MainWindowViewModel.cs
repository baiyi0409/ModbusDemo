using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Wpf.Ui.Controls;

namespace WpfHomeModbusDemo.ViewModels
{

    public partial class MainWindowViewModel: ObservableObject
    {
        public MainWindowViewModel()
        {
            //获取串口列表
            portNameList = new ObservableCollection<string>(SerialPort.GetPortNames());
            //初始化波特率 数据位 校验位 停止位 列表
            BaudRateList = new ObservableCollection<int> { 4800, 9600, 19200, 38400, 57600, 115200 };
            DataBitsList = new ObservableCollection<int> { 7, 8 };
            ParityList = new ObservableCollection<Parity>(Enum.GetValues(typeof(Parity)).Cast<Parity>());
            StopBitsList = new ObservableCollection<StopBits>(Enum.GetValues(typeof(StopBits)).Cast<StopBits>());
            //设置串口默认值
            SelectedBaudRate = 9600;
            SelectedDataBits = 8;
            SelectedParity = Parity.None;
            SelectedStopBits = StopBits.One;
        }

        //串口列表
        [ObservableProperty]
        private ObservableCollection<string> portNameList;
        //波特率列表
        [ObservableProperty]
        private ObservableCollection<int> baudRateList;
        //数据位列表
        [ObservableProperty]
        private ObservableCollection<int> dataBitsList;
        [ObservableProperty]
        private ObservableCollection<Parity> parityList;
        [ObservableProperty]
        private ObservableCollection<StopBits> stopBitsList;
        //串口属性
        [ObservableProperty]
        private string selectedPort;
        [ObservableProperty]
        private int selectedBaudRate;
        [ObservableProperty]
        private int selectedDataBits;
        [ObservableProperty]
        private Parity selectedParity;
        [ObservableProperty]
        private StopBits selectedStopBits;

        [ObservableProperty]
        private bool isConnected;

        private SerialPort serialPort;
        private IModbusSerialMaster master;

        //连接串口
        [RelayCommand]
        private void Connect()
        {
            if (IsConnected == true)
            {
                Open();

                StartReading();
            }
            else
            {
                Close();
            }
        }

        //打开连接
        private bool Open() 
        {
            try
            {
                serialPort = new SerialPort();
                serialPort.PortName = SelectedPort;
                serialPort.BaudRate = SelectedBaudRate;
                serialPort.DataBits = SelectedDataBits;
                serialPort.Parity = SelectedParity;
                serialPort.StopBits = SelectedStopBits;

                serialPort.Open();
                master = ModbusSerialMaster.CreateRtu(serialPort);
                master.Transport.ReadTimeout = 2000; //读超时
                master.Transport.Retries = 3;  //重试次数

                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "提示",
                    Content = "串口打开成功",
                };
                uiMessageBox.ShowDialogAsync(true);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "错误",
                    Content = "无法访问串口，可能串口被其他程序占用",
                };
                uiMessageBox.ShowDialogAsync(false);
                return false;
            }
            catch (IOException ex)
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "错误",
                    Content = "串口连接错误: " + ex.Message,
                };
                uiMessageBox.ShowDialogAsync(false);
                return false;
            }
            catch (Exception ex)
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "错误",
                    Content = "打开串口时发生未知错误: " + ex.Message
                };
                uiMessageBox.ShowDialogAsync(false);
                return false;
            }
        }
        //关闭连接
        private void Close()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "提示",
                        Content = "串口已关闭！"
                    };
                    uiMessageBox.ShowDialogAsync(false);
                }
            }
            catch (Exception ex)
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "错误",
                    Content = "关闭串口时发生错误: " + ex.Message
                };
                uiMessageBox.ShowDialogAsync(false);
            }
        }
        //检查连接
        private void CheckConnection()
        {
            if (master == null || !IsConnected)
            {
                throw new InvalidOperationException("串口未打开或已断开，请检查连接！");
            }
        }
        //数据读取
        private async void StartReading()
        {
            while (true)
            {
                // 如果没有连接，则停止循环
                if (!IsConnected)
                {
                    break;
                }

                Read(); // 执行读取数据

                await Task.Delay(1000); // 每隔1秒读取一次
            }
        }

        //主厅灯光是否开启
        [ObservableProperty]
        private bool mainLightIsChecked;
        [ObservableProperty]
        private bool deskLampIsChecked;
        [ObservableProperty]
        private bool tvIsChecked;
        [ObservableProperty]
        private bool pS5IsChecked;


        //主厅灯光
        [ObservableProperty]
        private string mainLight;
        private void Read()
        {
            if (master == null)
            {
                //var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                //{
                //    Title = "提示",
                //    Content = "串口未打开或已断开，请检查连接！",
                //};
                //uiMessageBox.ShowDialogAsync(false);
                return;
            }

            //客厅部分
            byte slaveAddress = 1;      //从站地址
            ushort startAddress = 0;    //起始寄存器地址
            ushort numberOfPoints = 4;  //读取寄存器数量

            // 读取从站信息
            CheckConnection();
            // 01 输入状态
            try
            {
                bool[] LivingRoom = master.ReadCoils(slaveAddress, startAddress, numberOfPoints);
                //主厅灯
                MainLightIsChecked = LivingRoom[0];
                //氛围灯
                DeskLampIsChecked = LivingRoom[1];
                //电视
                TvIsChecked = LivingRoom[2];
                //PS5
                PS5IsChecked = LivingRoom[3];
            }
            catch (Modbus.SlaveException ex)
            {
                HandleModbusException(ex);
            }


            // 04 输入寄存器
            try
            {
                //04
                ushort[] register = master.ReadInputRegisters(slaveAddress, startAddress, numberOfPoints);
                MainLight = register[0]!.ToString();
            }
            catch (Modbus.SlaveException ex)
            {
                HandleModbusException(ex);
            }
        }

        // 处理 Modbus 错误
        private void HandleModbusException(Modbus.SlaveException ex)
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "提示",
                Content = "未知错误",
            };

            switch (ex.SlaveExceptionCode)
            {
                case 1:
                    uiMessageBox.Content = "异常代码 1: 非法功能码！";
                    uiMessageBox.ShowDialogAsync(false);
                    break;
                case 2:
                    uiMessageBox.Content = "异常代码 2: 非法数据地址！";
                    uiMessageBox.ShowDialogAsync(false);
                    break;
                case 3:
                    uiMessageBox.Content = "异常代码 3: 非法数据值！";
                    uiMessageBox.ShowDialogAsync(false);
                    break;
                case 4:
                    uiMessageBox.Content = "异常代码 4: 从设备故障！";
                    uiMessageBox.ShowDialogAsync(false);
                    break;
                case 5:
                    uiMessageBox.Content = "异常代码 5: 硬件故障！";
                    uiMessageBox.ShowDialogAsync(false);
                    break;
                case 6:
                    uiMessageBox.Content = "异常代码 6: 从设备忙！";
                    uiMessageBox.ShowDialogAsync(false);
                    break;
                case 7:
                    uiMessageBox.Content = "异常代码 7: 内存错误！";
                    uiMessageBox.ShowDialogAsync(false);
                    break;
                default:
                    uiMessageBox.Content = "未知异常代码: " + ex.SlaveExceptionCode;
                    uiMessageBox.ShowDialogAsync();
                    break;
            }

            Close();
        }
    }
}
