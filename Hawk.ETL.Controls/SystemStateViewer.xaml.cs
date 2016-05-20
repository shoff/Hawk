using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hawk.Core.Utils.Plugins;

namespace Hawk.ETL.Controls
{
    using System.ComponentModel;

    /// <summary>
    /// SystemStateViewer.xaml 的交互逻辑
    /// </summary>
    [XFrmWork("系统状态视图" )]
    public partial class SystemStateViewer : UserControl, ICustomView
    {
        public SystemStateViewer()
        {
            this.InitializeComponent();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                this.FrmState = FrmState.Middle;
            }

        }

        public FrmState FrmState { get; set; }

        private void ListBox_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var attr = this.dataListBox.SelectedItem;
                
                if (attr == null)
                {
                    return;
                }

                var data = new DataObject(typeof(IDictionarySerializable), attr);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        private void processListBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var attr = this.processListBox.SelectedItem as IProcess;
                if (attr == null)
                {
                    return;
                }

                var data = new DataObject(typeof(IDictionarySerializable), attr);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }
    }
}