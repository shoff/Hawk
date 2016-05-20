namespace Hawk.ETL.Managements
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using Core.Connectors;
    using Core.Utils;
    using Core.Utils.MVVM;
    using Core.Utils.Plugins;

    public class ProjectItem : PropertyChangeNotifier, IDictionarySerializable
    {
        [DisplayName("保存路径")]
        public string SavePath { get; set; }

        [DisplayName("名称")]
        public string Name { get; set; }

        [DisplayName("描述")]
        public string Description { get; set; }

        [DisplayName("版本")]
        public int Version { get; set; }


        public virtual FreeDocument DictSerialize(Scenario scenario = Scenario.Database)
        {
            var dict = new FreeDocument();
            dict.Add("Name", this.Name);
            dict.Add("Description", this.Description);
            dict.Add("Version", this.Version);
            dict.Add("SavePath", this.SavePath);
            return dict;
        }

        public virtual void DictDeserialize(IDictionary<string, object> docu, Scenario scenario = Scenario.Database)
        {
            this.Name = docu.Set("Name", this.Name);
            this.Description = docu.Set("Description", this.Description);
            this.Version = docu.Set("Version", this.Version);
            this.SavePath = docu.Set("SavePath", this.SavePath);
        }

        public static Project LoadProject(string path)
        {
            if (File.Exists(path) == false)
            {
                throw new Exception("当前工程文件的路径不存在，生成新工程");
            }
            var xml = new FileConnectorXML {FileName = path};
            var r = xml.ReadFile().FirstOrDefault();
            var project = new Project();

            if (r != null)
            {
                project.DictDeserialize(r.DictSerialize());
            }
            return project;
        }
    }

    /// <summary>
    ///     项目信息
    /// </summary>
    public class Project : ProjectItem
    {
        public Project()
        {
            this.Tasks = new ObservableCollection<ProcessTask>();
            this.DBConnections = new ObservableCollection<IDataBaseConnector>();
        }


        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }

        [DisplayName("数据库连接")]
        public ObservableCollection<IDataBaseConnector> DBConnections { get; set; }


        /// <summary>
        ///     在工程中保存的所有任务
        /// </summary>
        [DisplayName("任务列表")]
        public ObservableCollection<ProcessTask> Tasks { get; set; }

        public void Save()
        {
            var connector = new FileConnectorXML();


            if (this.SavePath != null && File.Exists(this.SavePath))
            {
                connector.FileName = this.SavePath;
            }
            else
            {
                var result = connector.CheckFilePath(FileOperate.Save);
                if (result == false)
                {
                    return;
                }
                this.SavePath = connector.FileName;
            }

            connector.WriteAll(
                new List<IFreeDocument> {DictSerialize()}
                );
        }

        public static Project Load(string path = null)
        {
            var connector = new FileConnectorXML();
            connector.FileName = path;
            if (connector.FileName == null)
            {
                var result = connector.CheckFilePath(FileOperate.Read);
                if (result == false)
                {
                    return null;
                }
            }
            var proj = connector.ReadFile().FirstOrDefault();

            if (proj == null)
            {
                return null;
            }
            var proj2 = new Project();
            proj.DictCopyTo(proj2);
            proj2.SavePath = connector.FileName;

            return proj2;
        }

        public override FreeDocument DictSerialize(Scenario scenario = Scenario.Database)
        {
            var dict = base.DictSerialize();

            dict.Children = this.Tasks.Select(d => d.DictSerialize()).ToList();
            var connecots = new FreeDocument
            {
                Children = this.DBConnections.Select(d => (d as IDictionarySerializable).DictSerialize()).ToList()
            };
            ;
            dict.Add("DBConnections", connecots);
            return dict;
        }

        public override void DictDeserialize(IDictionary<string, object> docu, Scenario scenario = Scenario.Database)
        {
            base.DictDeserialize(docu);
            var doc = docu as FreeDocument;

            if (doc.Children != null)
            {
                var items = doc.Children;

                foreach (var item in items)
                {
                    var proces = new ProcessTask();
                    proces.Project = this;
                    proces.DictDeserialize(item);

                    this.Tasks.Add(proces);
                }
            }

            if (docu["DBConnections"] != null)
            {
                var items = docu["DBConnections"] as FreeDocument;

                if (items.Children == null)
                {
                    return;
                }
                foreach (var item in items.Children)
                {
                    var type = item["TypeName"].ToString();
                    var conn = PluginProvider.GetObjectByType<IDataBaseConnector>(type) as DBConnectorBase;
                    if (conn == null)
                    {
                        continue;
                    }
                    conn.DictDeserialize(item);

                    this.DBConnections.Add(conn);
                }
            }
        }
    }
}