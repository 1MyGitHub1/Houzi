using Caliburn.Micro;
using Mass.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monster.AutoSampler.ViewModels.BaseData
{
    [Export]
    public class BaseViewModel : Conductor<object>.Collection.OneActive, IContentService
    {
        /// <summary>
        /// 窗体管理器
        /// </summary>
        protected readonly IWindowManager _windowManager;

        /// <summary>
        /// 事件聚合器
        /// </summary>
        protected readonly IEventAggregator _events;

        /// <summary>
        /// 构造函数
        /// 必须加特性【ImportingConstructor】，这是因为用MEF在创建ViewModel实例时，有[ImportingConstructor]标记的构造函数的参数会自动使用容器内相应的export对象
        /// 但是ImportingConstructor不支持多个构造函数同时有[ImportingConstructor]标记
        /// </summary>
        /// <param name="windowManager">窗体管理器</param>
        [ImportingConstructor]
        public BaseViewModel(IEventAggregator events, IWindowManager windowManager)
        {
            _events = events;
            _windowManager = windowManager;
        }

        #region 方法 获取页面
        /// <summary>
        /// 根据类型获取单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetInstance<T>() => (T)this.GetObject<IContentService>(typeof(T).Name);
        /// <summary>
        /// 根据类型和名称获取实例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual T GetObject<T>(string name = null) => (T)IoC.Get<T>(name);

        /// <summary>
        /// 1、this.OnUIThreadAsync(()=>{ });
        /// 2、new System.Action(() => { }).OnUIThreadAsync();
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        async public virtual Task OnUIThreadAsync(System.Action action)
        {
            await action.OnUIThreadAsync();
        }
        #endregion
    }
}