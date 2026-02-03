using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.Services.Validation;
using Demo_ReportPrinter.ViewModels.Base;

namespace Demo_ReportPrinter.ViewModels
{
    /// <summary>
    /// 数据录入面板ViewModel
    /// </summary>
    public partial class DataEntryViewModel : ViewModelBase
    {
        private readonly ISharedDataService _sharedDataService;
        private readonly IValidationService _validationService;

        [ObservableProperty]
        private string _userName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private DateTime _birthDate = DateTime.Now;

        [ObservableProperty]
        private string _department;

        [ObservableProperty]
        private string _errorMessage;

        public ObservableCollection<string> Departments { get; set; }

        public DataEntryViewModel()
        {
            _sharedDataService = Demo_ReportPrinter.Services.DI.ServiceLocator.Instance.GetService<ISharedDataService>();
            _validationService = Demo_ReportPrinter.Services.DI.ServiceLocator.Instance.GetService<IValidationService>();
            Departments = new ObservableCollection<string>
            {
                "技术部",
                "市场部",
                "销售部",
                "人力资源部",
                "财务部"
            };

            // 注册数据变更监听
            RegisterDataChangeHandlers();
        }

        public DataEntryViewModel(
            ISharedDataService sharedDataService,
            IValidationService validationService)
        {
            _sharedDataService = sharedDataService;
            _validationService = validationService;
            Departments = new ObservableCollection<string>
            {
                "技术部",
                "市场部",
                "销售部",
                "人力资源部",
                "财务部"
            };

            // 注册数据变更监听
            RegisterDataChangeHandlers();
        }

        partial void OnUserNameChanged(string value)
        {
            _sharedDataService.UpdateUserData("UserName", value);
        }

        partial void OnEmailChanged(string value)
        {
            _sharedDataService.UpdateUserData("Email", value);
        }

        partial void OnBirthDateChanged(DateTime value)
        {
            _sharedDataService.UpdateUserData("BirthDate", value);
        }

        partial void OnDepartmentChanged(string value)
        {
            _sharedDataService.UpdateUserData("Department", value);
        }

        private void RegisterDataChangeHandlers()
        {
            // 监听数据变更消息
            RegisterMessageHandler<Services.Shared.DataChangedMessage>((message) =>
            {
                HandleDataChange(message.Key, message.Value);
            });
        }

        private void HandleDataChange(string key, object value)
        {
            // 根据key更新对应的属性
            switch (key)
            {
                case "UserName":
                    UserName = value?.ToString();
                    break;
                case "Email":
                    Email = value?.ToString();
                    break;
                case "BirthDate":
                    if (value is DateTime dateValue)
                    {
                        BirthDate = dateValue;
                    }
                    break;
                case "Department":
                    Department = value?.ToString();
                    break;
            }
        }

        [RelayCommand]
        private async Task SaveDataAsync()
        {
            // 验证数据
            if (ValidateData())
            {
                try
                {
                    // 保存数据逻辑
                    await Task.CompletedTask;
                    // 发送保存成功消息
                    _sharedDataService.BroadcastDataChange("DataSaved", true);
                    ErrorMessage = "数据保存成功";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"保存数据失败: {ex.Message}";
                    _sharedDataService.BroadcastDataChange("Error", ex.Message);
                }
            }
        }

        [RelayCommand]
        private void ResetData()
        {
            // 重置数据逻辑
            UserName = string.Empty;
            Email = string.Empty;
            BirthDate = DateTime.Now;
            Department = string.Empty;
            ErrorMessage = string.Empty;
            _sharedDataService.ClearUserData();
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        private bool ValidateData()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(UserName))
            {
                errors.Add("用户名不能为空");
            }

            if (string.IsNullOrEmpty(Email))
            {
                errors.Add("邮箱不能为空");
            }
            else if (!IsValidEmail(Email))
            {
                errors.Add("邮箱格式不正确");
            }

            if (string.IsNullOrEmpty(Department))
            {
                errors.Add("部门不能为空");
            }

            if (errors.Any())
            {
                ErrorMessage = string.Join("\n", errors);
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 验证邮箱格式
        /// </summary>
        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            return emailRegex.IsMatch(email);
        }
    }
}