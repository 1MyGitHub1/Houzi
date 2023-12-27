using bbv.Common.EventBroker;
using bbv.Common.EventBroker.Handlers;
using Caliburn.Micro;
using DeviceInterface;
using LabTech.Common;
using Mass.Common;
using Mass.Common.Enums;
using Monster.AutoSampler.ViewModels.BaseData;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Monster.AutoSampler.ViewModels
{
    [Export(typeof(IContentService))]
    public class DeviceModuleViewModel : BaseViewModel
    {
        private EventBroker _EeventBroker = new EventBroker();
        private CompositionContainer _CompositionContainer;

        #region 构造函数
        [ImportingConstructor]
        public DeviceModuleViewModel(IEventAggregator events, IWindowManager windowManager) : base(events, windowManager)
        {
            DisplayName = "子页面（Sub Page）模块";
            _EeventBroker.Register(this);
            InitPlugin();
            LoadPlugs();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 插件资源集合
        /// </summary>
        [ImportMany]
        public IEnumerable<IQuickSamplerPlug> DeviceList
        {
            get => this._deviceList;
            set => this.Set(ref this._deviceList, value);
        }
        private IEnumerable<IQuickSamplerPlug> _deviceList;

        public Page DevicePage
        {
            get => this._devicePage;
            set => this.Set(ref _devicePage, value);
        }
        private Page _devicePage;
        #endregion

        /// <summary>
        /// 装载插件
        /// </summary>
        private void InitPlugin()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SamplerPlugins");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //设置目录，让引擎能自动去发现新的扩展
                AggregateCatalog catalog = new AggregateCatalog();
                DirectoryCatalog plugDir = new DirectoryCatalog(path);
                catalog.Catalogs.Add(plugDir);
                //创建一个容器，相当于是生产车间
                _CompositionContainer = new CompositionContainer(catalog);

                //调用车间的ComposeParts把各个部件组合到一起

                this._CompositionContainer.ComposeParts(this);//这里只需要传入当前应用程序实例就可以了，其它部分会自动发现并组装
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
            catch (Exception e)
            {
                MainLogHelper.Instance.Error("DeviceModuleViewModel [InitPlugin]：", e);
                this.GetObject<MessagePageViewModel>().ShowDialog("MessageContent_Error".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            }
        }

        /// <summary>
        /// 加载插件资源
        /// </summary>
        private void LoadPlugs()
        {
            foreach (IQuickSamplerPlug plugin in DeviceList)
            {
                _EeventBroker.Register(plugin);
                if (plugin is Page devicePage)
                {
                    DevicePage = devicePage;
                }
                if (plugin != null)
                {
                    //是否为连接HiMass状态
                    plugin.ConnectionInit(false);
                }
            }
        }

        public void InitData()
        {

            //AutoSamplerModule_MassSendSamDataEvent(this, new AutoSamplerEventArgs()
            //{

            //    SamInfoList = new List<AutoSampler_SamInfo>()
            //      {
            //          new AutoSampler_SamInfo()
            //          {
            //              OperationMode = EnumSamOperationMode.Add,
            //              SamID = Guid.Empty,
            //              SamName = "aaa"
            //          },
            //          //new AutoSampler_SamInfo()
            //          //{
            //          //    OperationMode = EnumSamOperationMode.Update,
            //          //    SamID = Guid.Empty,
            //          //    SamName = "bbb",
            //          //    OverWash = 300
            //          //},

            //      }



            //});

            //AutoSamplerModule_MassSendSamDataEvent(this, new AutoSamplerEventArgs()
            // {

            //     SamInfoList = new List<AutoSampler_SamInfo>()
            //      {
            //          new AutoSampler_SamInfo()
            //          {
            //              OperationMode = EnumSamOperationMode.Update,
            //              SamID = Guid.NewGuid(),
            //              Ov
            //              SamName = "aaa"
            //          },
            //          //new AutoSampler_SamInfo()
            //          //{
            //          //    OperationMode = EnumSamOperationMode.Delete,
            //          //    SamID = Guid.NewGuid(),
            //          //    SamName = "bbb"
            //          //}
            //      }

            // });
            AutoSamplerModule_MassSendObjectEvent(this, new ObjectEventArgs()
            {
                Parameter = true,
                MessParamType = EnumMessParamType.UseAutoSampler
            });
        }

        [EventSubscription("topic://AutoSampler_AutoSamplerSendSamData", typeof(Publisher))]
        public void SubscribeAutoSamplerSendSamDataEvent(object sender, AutoSamplerEventArgs msgArg)
        {
            try
            {
                if (msgArg != null && msgArg.SamInfoList != null)
                {
                    for (int i = 0; i < msgArg.SamInfoList.Count; i++)
                    {
                        AutoSampler_SamInfo samInfo = msgArg.SamInfoList[i];
                        //Trace.WriteLine(samInfo.SamName + "[" + samInfo.SamID + "]" + samInfo.Location + samInfo.OperationMode + samInfo.IsAnalyze + samInfo.AnalysisType
                        //   +samInfo.NextSamID);
                    }
                }
            }
            catch (Exception e)
            {
                MainLogHelper.Instance.Error("AutoSamplerModule [SubscribeAutoSamSendSyncDataEvent]：", e);
            }
        }

        [EventSubscription("topic://AutoSampler_AutoSamplerSendObjectData", typeof(Publisher))]
        public void SubscribeAutoSamplerSendObjectDataEvent(object sender, ObjectEventArgs msgArg)
        {
            try
            {
                if (msgArg != null)
                {
                    //更新是否使用自动进样器的状态消息
                    if (msgArg.MessParamType == EnumMessParamType.UseAutoSampler && msgArg.Parameter is bool state)
                    {
                        bool a = state;
                        //Trace.WriteLine(state);
                    }
                }
            }
            catch (Exception e)
            {
                MainLogHelper.Instance.Error("AutoSamplerModule [SubscribeUseAutoSamStateEvent]：", e);
            }
        }

        #region MassSendObjectEvent
        [EventPublication("topic://AutoSampler_MassSendObjectData")]
        public event EventHandler<ObjectEventArgs> MassSendObjectEvent;

        /// <summary>
        /// 将数据更新到自动进样器（包含一些命令，如：停止，暂停，执行进样等）
        /// </summary>
        /// <param name="msgArg">消息参数</param>
        public void PublisherMassSendObjectEvent(ObjectEventArgs msgArg)
        {
            MassSendObjectEvent?.Invoke(this, msgArg);
        }

        private void AutoSamplerModule_MassSendObjectEvent(object sender, ObjectEventArgs e)
        {
            PublisherMassSendObjectEvent(e);
        }
        #endregion

        #region MassSendSamDataEvent
        [EventPublication("topic://AutoSampler_MassSendSamData")]
        public event EventHandler<AutoSamplerEventArgs> MassSendSamDataEvent;

        /// <summary>
        /// 将数据更新到自动进样器（包含一些命令，如：停止，暂停，执行进样等）
        /// </summary>
        /// <param name="msgArg">消息参数</param>
        public void PublisherMassSendSamDataEvent(AutoSamplerEventArgs msgArg)
        {
            MassSendSamDataEvent?.Invoke(this, msgArg);
        }

        private void AutoSamplerModule_MassSendSamDataEvent(object sender, AutoSamplerEventArgs e)
        {
            PublisherMassSendSamDataEvent(e);
        }
        #endregion
    }
}