using System.Windows.Controls;
using Hawk.Core.Utils.Plugins;

namespace Hawk.ETL.Controls
{
    using System.ComponentModel;

    /// <summary>
    /// TaskManagerView.xaml 的交互逻辑
    /// </summary>
  [XFrmWork("任务管理视图" )]
    public partial class TaskManagerView : UserControl,ICustomView
    {
        public TaskManagerView()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                this.FrmState = FrmState.Buttom;
            }

        }

        public FrmState FrmState { get; set; }
    }
}
