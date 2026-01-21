using CommunityToolkit.Mvvm.ComponentModel;

namespace ReportTemplateEditor.Designer.ViewModels
{
    /// <summary>
    /// ViewModel基类，提供属性变更通知功能
    /// </summary>
    /// <remarks>
    /// 所有ViewModel都应继承此类，以获得属性变更通知能力
    /// 使用CommunityToolkit.Mvvm的ObservableObject作为基类
    /// </remarks>
    public partial class ViewModelBase : ObservableObject
    {
        /// <summary>
        /// 设置属性值并触发属性变更通知
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="field">属性字段的引用</param>
        /// <param name="newValue">新的属性值</param>
        /// <param name="propertyName">属性名称（可选，自动获取）</param>
        /// <returns>如果属性值发生变化返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// private string _name;
        /// public string Name
        /// {
        ///     get => _name;
        ///     set => SetProperty(ref _name, value);
        /// }
        /// </code>
        /// </example>
        protected new bool SetProperty<T>(ref T field, T newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            return base.SetProperty(ref field, newValue, propertyName);
        }

        /// <summary>
        /// 手动触发属性变更通知
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        /// <example>
        /// <code>
        /// public string FullName => $"{FirstName} {LastName}";
        /// 
        /// public string FirstName
        /// {
        ///     get => _firstName;
        ///     set
        ///     {
        ///         if (SetProperty(ref _firstName, value))
        ///         {
        ///             OnPropertyChanged(nameof(FullName));
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
        }
    }
}
