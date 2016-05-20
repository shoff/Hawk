using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using Hawk.Core.Utils;
using Hawk.Core.Utils.MVVM;
using Hawk.Core.Utils.Plugins;

namespace Hawk.ETL.Interfaces
{
    [XFrmWorkIgnore]
    public class AbstractProcessMethod : PropertyChangeNotifier, IDataProcess, IDictionarySerializable
    {
        protected string _name;

        public AbstractProcessMethod()
        {
            Name = TypeName;
        }


        [Browsable(false)]
        public IMainFrm MainFrm { get; set; }

        [Browsable(false)]
        public string MainPluginLocation { get; set; }

        [Browsable(false)]
        public PropertyGrid PropertyGrid
        {
            get
            {
                var property = new PropertyGrid();
                property.SelectedObject = this;
                return property;
            }
        }


        /// <summary>
        ///     模块名称
        /// </summary>
        [Category("1.基本信息")]
        [DisplayName("模块名称")]
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }


        [Browsable(false)]
        public bool IsOpen { get; set; }


        [Browsable(false)]
        public IDataManager SysDataManager { get; set; }

        [Browsable(false)]
        public IProcessManager SysProcessManager { get; set; }



        [Browsable(false)]
        public string TypeName
        {
            get
            {
                return

                    AttributeHelper.GetCustomAttribute(GetType()).Name;

            }
        }

        public virtual void ReportFinalResult()
        {
        }

        public virtual void DictDeserialize(IDictionary<string, object> dicts, Scenario scenario = Scenario.Database)
        {



            Name = dicts.Set("Name", Name);
        }

        public virtual FreeDocument DictSerialize(Scenario scenario = Scenario.Database)
        {
            var dict = new FreeDocument
            {
              
                {"Name", Name},
                {"Type", this.GetType().Name},
            };

            return dict;
        }

        public virtual bool Close()
        {
            return true;
        }

        public virtual bool Process()
        {
            return true;
        }


        public virtual bool Init()
        {
            return true;
        }
    }
}