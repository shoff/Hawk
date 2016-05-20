﻿namespace Hawk.ETL.Plugins.Transformers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Core.Connectors;
    using Core.Utils;
    using Core.Utils.Plugins;
    using Crawlers;
    using Generators;
    using HtmlAgilityPack;
    using Interfaces;
    using Process;

    [XFrmWork("从爬虫转换", "使用网页采集器获取网页数据，拖入的列需要为超链接")]
    public class CrawlerTF : TransformerBase
    {
        private readonly BuffHelper<HtmlDocument> buffHelper = new BuffHelper<HtmlDocument>(50);
        private readonly IProcessManager processManager;
        private string _crawlerSelector;
        private BfsGE generator;
        private bool isfirst;

        private Regex regex;

        public CrawlerTF()
        {
            this.processManager = MainDescription.MainFrm.PluginDictionary["模块管理"] as IProcessManager;
            //  var defaultcraw = processManager.CurrentProcessCollections.FirstOrDefault(d => d is SmartCrawler);
            this.MaxTryCount = "1";
            //if (defaultcraw != null) CrawlerSelector = defaultcraw.Name;
            this.PropertyChanged += (s, e) => { this.buffHelper.Clear(); };
        }

        [DisplayName("最大重复次数")]
        public string MaxTryCount { get; set; }

        [DisplayName("延时时间")]
        public string DelayTime { get; set; }

        [DisplayName("Post数据")]
        public string PostData { get; set; }

        [DisplayName("爬虫选择")]
        [Description("填写采集器或模块的名称")]
        public string CrawlerSelector
        {
            get { return this._crawlerSelector; }
            set
            {
                if (this._crawlerSelector != value)
                {
                    this.buffHelper.Clear();
                }
                this._crawlerSelector = value;
            }
        }


        [Category("请求队列")]
        [DisplayName("队列生成器")]
        [Description("填写模块的名称")]
        public string GEName { get; set; }

        [Category("请求队列")]
        [DisplayName("过滤规则")]
        public string Prefix { get; set; }

        [Category("请求队列")]
        [DisplayName("启用正则")]
        public bool IsRegex { get; set; }

        private SmartCrawler crawler { get; set; }

        public override bool Init(IEnumerable<IFreeDocument> datas)
        {
            if (this.generator == null)
            {
                var mainstream = this.processManager.CurrentProcessCollections.OfType<SmartETLTool>()
                    .FirstOrDefault(d => d.CurrentETLTools.Contains(this));
                this.generator = mainstream.CurrentETLTools.FirstOrDefault(d => d.Name == this.GEName) as BfsGE;
            }

            this.crawler =
                this.processManager.CurrentProcessCollections.FirstOrDefault(d => d.Name == this.CrawlerSelector) as SmartCrawler;
            if (this.crawler != null)
            {
                this.IsMultiYield = this.crawler.IsMultiData == ListType.List;
            }
            else
            {
                var task = this.processManager.CurrentProject.Tasks.FirstOrDefault(d => d.Name == this.CrawlerSelector);
                if (task == null)
                {
                    return false;
                }
                ControlExtended.UIInvoke(() => { task.Load(false); });
                this.crawler = this.processManager.CurrentProcessCollections.FirstOrDefault(d => d.Name == this.CrawlerSelector) as
                    SmartCrawler;
            }


            this.IsMultiYield = this.crawler.IsMultiData == ListType.List;
            this.isfirst = true;
            this.OneOutput = false;
            if (this.IsRegex)
            {
                this.regex = new Regex(this.Prefix);
            }
            return this.crawler != null && base.Init(datas);
        }


        private List<FreeDocument> GetDatas(IFreeDocument data)
        {
            var p = data[this.Column];
            if (p == null)
            {
                return new List<FreeDocument>();
            }
            var url = p.ToString();
            var bufkey = url;
            var post = data.Query(this.PostData);
            if (this.crawler.Http.Method == MethodType.POST)
            {
                bufkey += post;
            }
            var htmldoc = this.buffHelper.Get(bufkey);
            var docs = new List<FreeDocument>();
            if (htmldoc == null)
            {
                var delay = data.Query(this.DelayTime);
                var delaytime = 0;
                if (delay != null && int.TryParse(delay, out delaytime))
                {
                    if (delaytime != 0)
                    {
                        Thread.Sleep(delaytime);
                    }
                }

                HttpStatusCode code;
                int maxcount = 1;
                int.TryParse(data.Query(this.MaxTryCount), out maxcount);

                int count = 0;
                while (count < maxcount)
                {
                    docs = this.crawler.CrawlData(url, out htmldoc, out code, post);
                    if (HttpHelper.IsSuccess(code))
                    {
                        this.buffHelper.Set(bufkey, htmldoc);
                        break;
                    }
                    count++;
                }
            }
            else
            {
                docs = this.crawler.CrawlData(htmldoc);
            }

            if (this.generator != null)
            {
                var others = htmldoc.DocumentNode.SelectNodes("//@href");

                var r3 = others.Select(d => d.Attributes["href"].Value).ToList();
                IEnumerable<string> r4;

                if (string.IsNullOrEmpty(this.Prefix))
                {
                    r4 = r3;
                }
                else if (this.IsRegex == false)
                {
                    r4 =
                        r3.Where(d => d.StartsWith(this.Prefix)).Where(d => true);
                }
                else
                {
                    r4 = r3.Where(d => this.regex.IsMatch(d));
                }
                foreach (var href in r4)
                {
                    this.generator.InsertQueue(href);
                }
            }
            return docs;
        }

        public override IEnumerable<IFreeDocument> TransformManyData(IEnumerable<IFreeDocument> datas)
        {
            foreach (var data in datas)
            {
                var docs = GetDatas(data);
                foreach (var doc in docs)
                {
                    yield return doc.MergeQuery(data, this.NewColumn);
                }
            }
        }

        //private bool checkautoLogIn(List<FreeDocument> docs)

        //{
        //    if (docs.Count == 0 && isfirst)
        //    {
        //        if (crawler.Documents.Any())
        //        {
        //            crawler.AutoVisit();
        //            return false;
        //        }
        //        if (string.IsNullOrEmpty(crawler.URLFilter) == false &&
        //            crawler.IsRunning == false)
        //            crawler.StartVisit();
        //        return false;
        //    }
        //    if (docs.Count > 0 && crawler.IsRunning)
        //    {
        //        crawler.StopVisit();
        //    }
        //    isfirst = false;
        //    return true;
        //}

        public override object TransformData(IFreeDocument datas)
        {
            var docs = GetDatas(datas);
            if (docs.Count > 0)
            {
                var first = docs.First();
                first.DictCopyTo(datas);
            }

            return null;
        }
    }
}