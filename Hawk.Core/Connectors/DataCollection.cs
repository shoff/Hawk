﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Hawk.Core.Utils;
using Hawk.Core.Utils.MVVM;
using Hawk.Core.Utils.Plugins;

namespace Hawk.Core.Connectors
{
    public class DataCollection : PropertyChangeNotifier, IDictionarySerializable
    {
        protected List<IFreeDocument> RealData;


        private string name;

        [DisplayName("来源")]
        public virtual string Source
        {
            get { return "默认来源"; }
        }

        public override string ToString()
        {
            return Name;
        }

        public DataCollection(IEnumerable<IFreeDocument> data)
        {
            TableInfo=new TableInfo();
            RealData = new List<IFreeDocument>(data)
               ;
        }

        public DataCollection()
        {
            TableInfo=new TableInfo();
            RealData = new List<IFreeDocument>();
        }

        [DisplayName("列特性")]
        public TableInfo TableInfo { get; set; }

        [Browsable(false)]
        public virtual IList<IFreeDocument> ComputeData
        {
            get { return RealData; }
        }


        [DisplayName("总数据量")]
        public int Count
        {
            get { return ComputeData.Count; }
        }

        

        [DisplayName("数据描述")]
        public string Description { get; set; }

        [DisplayName("虚拟化数据集")]
        public virtual bool IsVirtual
        {
            get { return false; }
        }

        [DisplayName("数据集名称")]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public FreeDocument DictSerialize(Scenario scenario = Scenario.Database)
        {
            var dict = new FreeDocument();
            dict.Add("Name", Name);
            dict.Add("Count", Count);
            dict.Add("Source", Source);
            return dict;
        }

        public void DictDeserialize(IDictionary<string, object> docu, Scenario scenario = Scenario.Database)
        {
        }

        public DataCollection Clone(bool isdeep)
        {
            var docuts = new List<IFreeDocument>();

            for (int i = 0; i < ComputeData.Count; i ++)
            {
                if (isdeep)
                {
                    var fr = new FreeDocument();
                    ComputeData[i].DictCopyTo(fr);
                    docuts.Add(fr);
                }
                else
                {
                    docuts.Add(ComputeData[i] as IFreeDocument);
                }
            }
            var collection = new DataCollection(docuts);
            collection.Name = Name + '1';

            collection.TableInfo = this.TableInfo.Clone();
            return collection;
        }
    }
}