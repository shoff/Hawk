using System;
using System.Threading.Tasks;
using Hawk.Core.Utils;
using Hawk.Core.Utils.Plugins;

namespace Hawk.ETL.Interfaces
{
   public static class AppHelper
    {
        public static async Task<T> RunBusyWork<T>(this IMainFrm manager, Func<T> func, string title = "系统正忙",
        string message = "Processing long running task")
        {
            var dock = manager as IDockableManager;
            ControlExtended.UIInvoke(() => dock.SetBusy(true, title, message));

            T item = await Task.Run(func);
            ControlExtended.UIInvoke(() => dock.SetBusy(false));

            return item;
        }

    }
}
