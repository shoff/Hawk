using System.Windows.Controls;
using Hawk.Core.Utils.Plugins;
namespace Hawk.ETL.Controls

{
    using System.ComponentModel;


    /// <summary>
    /// DebugManagerUI.xaml 的交互逻辑
    /// </summary>
    [XFrmWork("调试信息窗口",  "输出调试信息", "")]
    public partial class DebugManagerUI : UserControl, ICustomView
    {
        public DebugManagerUI()
        {
            this.InitializeComponent();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                this.FrmState = FrmState.Large;
            }

        }

        public FrmState FrmState { get; set; }
    }
}