using System.Collections.Generic;
using System.ComponentModel;
using Hawk.Core.Utils.Plugins;

namespace Hawk.Core.Connectors.Vitural
{
    public class VirtualDataCollection : DataCollection
    {
        protected VirtualizingCollection<IFreeDocument> VirtualData;


        public VirtualDataCollection()
        {
        }

        public VirtualDataCollection(IItemsProvider<IFreeDocument> data,  int pageTimeout = 30000)
            : base()
        {
         
            VirtualData = new VirtualizingCollection<IFreeDocument>(data,pageTimeout);
 
            data.AlreadyGetSize += (s, e) => OnPropertyChanged("Count");
        }

        public override string Source {get{return VirtualData.ItemsProvider.Name; }}

        [DisplayName("Virtualized DataSet")]
        public override bool IsVirtual
        {
            get{return true;}
        }


        public IItemsProvider<IFreeDocument> ItemsProvider {get{return VirtualData.ItemsProvider; }}

        [Browsable(false)]
        public override IList<IFreeDocument> ComputeData
        {
            get { return VirtualData; }
        }
    }


}