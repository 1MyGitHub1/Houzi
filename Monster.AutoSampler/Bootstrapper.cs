using Caliburn.Micro;
using LabTech.Common;
using LabTech.UITheme.Enums;
using LabTech.UITheme.Helper;
using Mass.Common;
using Mass.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Monster.AutoSampler
{
    public class Bootstrapper : BootstrapperBase
    {
        #region 构造函数
        [ImportingConstructor]
        public Bootstrapper()
        {
            this.InstallExceptionHandledEvents();
            Initialize();
        }
        #endregion

        #region 私有字段
        /// <summary>
        /// 管理非单一实例模式的组合容器，Caliburn.Micro默认使用的IOC容器【SimpleContainer】
        /// </summary>
        private SimpleContainer multipleContainer;

        /// <summary>
        /// 管理单一实例的组合容器，MEF组合容器【CompositionContainer】
        /// </summary>
        private CompositionContainer singleContainer;
        #endregion

        #region 重构用于配置IOC容器的方法 --> Configure
        /// <summary>
        /// 用于配置IOC容器
        /// 使用CM自带的IOC容器（SimpleContainer）或者使用MEF的组合容器（CompositionContainer）来组合部件实现IOC
        /// MEF是一个.net的插件框架，也可以作为一个依赖注入容器（IOC）使用
        /// 三种发现部件（Part）的方式，如下：
        /// 1.AssemblyCatalog 在当前程序集发现部件【AssemblySource.Instance.Select(x => new AssemblyCatalog(x))】
        /// 2.DirectoryCatalog 在指定的目录发现部件【new DirectoryCatalog("C:\\Users\\v-rizhou\\SimpleCalculator\\Extensions")】
        /// 3.DeploymentCatalog 在指定的XAP文件中发现部件（用于silverlight）
        /// </summary>
        protected override void Configure()
        {
            //新建一个窗口管理器添加到IOC中
            var winMananger = new BaseWindowManager();
            //如果要使用弱事件就需要将这个添加IOC容器中
            var eventAggregator = new EventAggregator();

            #region 注册单例对象模式
            //1.查询需要组合的部件（当前程序集的所有部件）
            IEnumerable<ComposablePartCatalog> assemblyCatalogs = AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>();

            //2.使用AggregateCatalog来把程序集部件聚合到一起
            var catalog = new AggregateCatalog(assemblyCatalogs);

            //把从指定path发现的部件也添加到目录中
            //catalog.Catalogs.Add(new DirectoryCatalog("指定的路径path"));

            //3.使用AssemblyCatalog创建组合容器【聚合完部件之后，要把它们放到CompositionContainer容器中】
            singleContainer = new CompositionContainer(catalog);

            //4.调用组合容器的组合部件方法ComposeParts【ComposeParts可以把容器中的部件组合到一起】,与下面方式等价
            //container.ComposeParts(this, new WindowManager(),new EventAggregator());

            //4.也可以调用组合容器的组合对象的方法Compose【如果需要支持移除或者添加部件这就需要批处理（CompositionBatch），变化执行后，容器将自动地触发重合】
            //4.1 创建组合对象【组合容器“加工工厂”】
            var batch = new CompositionBatch();
            //4.2.向CompositionBatch对象中添加指定的部件
            batch.AddExportedValue<IWindowManager>(winMananger);//被组装对象加入批处理
            batch.AddExportedValue<IEventAggregator>(eventAggregator);
            //可以在任何地方通过IOC使用compositionContainer了
            batch.AddExportedValue(singleContainer);
            batch.AddExportedValue(catalog);

            //4.3.在容器中添加或移除指定的CompositionBatch中的部件并执行组合。
            singleContainer.Compose(batch);

            singleContainer.ComposeParts(singleContainer.GetExportedValues<ViewModels.BaseData.IContentService>());

            //ViewLocator.NameTransformer.AddRule(@"(?<nsbefore>([A-Za-z_\u4e00-\u9fa5]\w*\.)*)(?<subns>ViewModels\.)(?<nsafter>([A-Za-z_\u4e00-\u9fa5]\w*\.)*)(?<basename>[A-Za-z_\u4e00-\u9fa5]\w*)(?<suffix>)$", @"${nsbefore}Views.${nsafter}${basename}");
            #endregion

            #region 多例对象模式
            multipleContainer = new SimpleContainer();
            multipleContainer.Instance<IWindowManager>(winMananger);
            multipleContainer.Instance<IEventAggregator>(eventAggregator);
            multipleContainer.Instance(multipleContainer);
            multipleContainer.PerRequest<ViewModels.MessagePageViewModel>();
            #endregion
        }
        #endregion

        #region 重构程序进入的方法 --> OnStartup
        /// <summary>
        /// 程序进入
        /// 重写此操作以添加要在应用程序启动后执行的自定义行为（Override this to add custom behavior to execute after the application starts.）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            if (Win32Helper.IsApplicationRunning())
            {
                //程序已启动，不要试图启动多个应用程序。
                // 退出当前实例程序
                Environment.Exit(0);
            }
            bool rlt = true;
            Enum_Languages currentLangName = Enum_Languages.English;
            Enum_ThemeColor currentSkinThemeName = Enum_ThemeColor.Green;
            List<string> languageList = new List<string>
            {
                "/Mass.Language;component/"
            };
            try
            {
                string runLanguage = ConfigurationHelper.ReadAppSettings("LanguageName");
                if (!string.IsNullOrEmpty(runLanguage) && Enum.TryParse(runLanguage, out Enum_Languages language))
                {
                    currentLangName = language;
                }
                ResourceDictionaryHelper.SwitchLanguage(currentLangName, languageList);

                string skinThemeName = ConfigurationHelper.ReadAppSettings("SkinThemeName");
                if (!string.IsNullOrEmpty(skinThemeName) && Enum.TryParse(skinThemeName, out Enum_ThemeColor themeColor))
                {
                    currentSkinThemeName = themeColor;
                }
                ResourceDictionaryHelper.SwitchSkinTheme(currentSkinThemeName);
            }
            catch (Exception ex)
            {
                MainLogHelper.Instance.Error("OnStartup 异常", ex);
                rlt = false;
            }
            finally
            {
                if (rlt == false)
                {
                    //语言
                    currentLangName = Enum_Languages.Chinese;
                    ResourceDictionaryHelper.SwitchLanguage(currentLangName, languageList);
                    ConfigurationHelper.UpdateAppSettings("LanguageName", currentLangName.ToString());

                    //主题色
                    currentSkinThemeName = Enum_ThemeColor.Green;
                    ResourceDictionaryHelper.SwitchSkinTheme(currentSkinThemeName);
                    ConfigurationHelper.UpdateAppSettings("SkinThemeName", currentSkinThemeName.ToString());
                }
            }
            DisplayRootViewFor<ViewModels.ShellPageViewModel>();
        }
        #endregion

        #region 重构用于程序退出的方法 --> OnExit
        /// <summary>
        /// 程序退出
        /// 重写此操作可在退出时添加自定义行为(Override this to add custom behavior on exit.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnExit(object sender, EventArgs e)
        {
            this.ExitProgram();
            new System.Action(() =>
            {
                Application.Current.Shutdown();
                System.Environment.Exit(System.Environment.ExitCode);
            }).OnUIThread();
        }
        #endregion

        #region 重构获取某一特定类型的所有实例的方法 --> GetAllInstances
        /// <summary>
        /// IOC容器获取实例的方法
        /// 获取某一特定类型的所有实例
        /// 重写此操作以提供特定于 IoC 的实现（Override this to provide an IoC specific implementation.）
        /// </summary>
        /// <param name="service">查找的实例类型</param>
        /// <returns>符合实例类型的所有实例</returns>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            if (IsInstanceType(service))
                return singleContainer.GetExportedValues<object>(AttributedModelServices.GetContractName(service));
            else
                return multipleContainer.GetAllInstances(service);
        }
        #endregion

        #region 重构根据Key或名称获取实例的方法 --> GetInstance
        /// <summary>
        /// 根据传过来的Key或名称得到实例
        /// 重写此操作以提供特定于 IoC 的实现（Override this to provide an IoC specific implementation.）
        /// </summary>
        /// <param name="service">查找的实例类型</param>
        /// <param name="key">查找的Key或名称</param>
        /// <returns>返回的实例</returns>
        protected override object GetInstance(Type service, string key)
        {
            var contract = AttributedModelServices.GetContractName(service);
            var exports = singleContainer.GetExportedValues<object>(contract);
            if (exports.Any())
            {
                //如果为单例模式则使用compositionContainer容器
                if (IsInstanceType(service))
                {
                    if (key.IsNotNullOrEmpty())
                        return exports.FirstOrDefault(p => p.GetType().Name == key);
                    else
                        return exports.First();
                }
                else
                {
                    //如果为非单例模式则使用multipleContainer容器
                    return multipleContainer.GetInstance(service, key);
                }
            }
            else
            {
                throw new Exception(string.Format("找不到{0}实例", contract));
            }
        }
        #endregion

        #region 重构向IOC容器注入实例的方法 --> BuildUp
        /// <summary>
        /// IOC容器注入实例的方法
        /// 将实例传送给IOC容器使依赖关系注入
        /// </summary>
        /// <param name="instance"></param>
        protected override void BuildUp(object instance)
        {
            singleContainer.SatisfyImportsOnce(instance);
        }
        #endregion

        #region 异常处理
        /// <summary>
        /// 安装异常处理事件
        /// </summary>
        private void InstallExceptionHandledEvents()
        {
            //避免事件多次注册
            UninstallExceptionHandledEvents();
            //Task异常处理
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //UI线程异常处理
            Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            //非UI线程异常处理
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// 卸载异常处理事件
        /// </summary>
        private void UninstallExceptionHandledEvents()
        {
            //Task异常处理
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            //UI线程异常处理
            Application.Current.DispatcherUnhandledException -= Application_DispatcherUnhandledException;
            //非UI线程异常处理
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// Task异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            this.OnUnhandledException(sender, e.Exception, "UnobservedTaskException");
        }

        /// <summary>
        /// UI线程异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // 避免再次进入 CurrentDomain_UnhandledException
            e.Handled = true;
            this.OnUnhandledException(sender, e.Exception, "DispatcherUnhandledException");
        }

        /// <summary>
        /// 非UI线程异常处理
        /// 能捕获 所有线程（Task 除外及Application_DispatcherUnhandledException 的 e.Handled没有处理的） 抛出的未处理异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.OnUnhandledException(sender, e.ExceptionObject as Exception, "UnhandledException");
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="type">
        /// 异常类型：
        /// DispatcherUnhandledException: UI线程出现的异常
        /// UnobservedTaskException: 仅能捕获 Task 中抛出的未处理异常
        /// UnhandledException: 所有线程（Task 除外） 抛出的未处理异常
        /// </param>
        private void OnUnhandledException(object sender, Exception e, string type)
        {
            MainLogHelper.Instance.Error("OnUnhandledException 异常", e);
            //程序运行错误，即将关闭。请与技术支持人员联系！
            IoC.Get<ViewModels.MessagePageViewModel>()?.ShowDialog("Message_Error1000".GetWord(), "MessageTitle_Error".GetWord(), false, Enum_MessageType.Error);
            if (Application.Current.MainWindow != null)
            {
                //释放资源
                //OnExit(null, null);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 是否为单例类型
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private bool IsInstanceType(Type service)
        {
            return service == typeof(ViewModels.BaseData.IContentService)
                || service == typeof(IWindowManager)
                || service == typeof(IEventAggregator)
                || service == typeof(ViewModels.ShellPageViewModel);
        }

        /// <summary>
        /// 退出应用程序
        /// </summary>
        private void ExitProgram()
        {
            MainLogHelper.Instance.Info("开始退出应用程序");
            // 释放数据结构体
            //Mass.Models.DataBaseInit.DBDispose();
            MainLogHelper.Instance.Debug("数据库释放完毕");
            MainLogHelper.Instance.Info("应用程序已正常退出");
        }
        #endregion
    }
}