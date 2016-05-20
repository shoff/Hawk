using System.Collections.ObjectModel;
using System.Windows.Input;
using Hawk.Core.Utils.MVVM;

namespace Hawk.Core.Utils.Plugins
{
    [XFrmWork("调试信息窗口",  "输出调试信息", "")]
    public class DebugManager : AbstractPlugIn, IView, IMainFrmMenu
    {
        #region Properties
         
        public IAction BindingCommands {get{return null; }} 

        public FrmState FrmState {get{return FrmState.Buttom; }}

        public object UserControl {get{return null; }}

        #endregion
    }
}