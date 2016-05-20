using System.Windows.Controls;
using System.Windows.Input;
using Hawk.Core.Utils.Plugins;

namespace Hawk.ETL.Controls
{
    using System.ComponentModel;

    /// <summary>
    /// DataProcessManagerUI.xaml 的交互逻辑
    /// </summary>
    [XFrmWork("模块管理", "替换模块管理界面")]
    public partial class DataProcessManagerUI : UserControl, ICustomView
    {
        public DataProcessManagerUI()
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