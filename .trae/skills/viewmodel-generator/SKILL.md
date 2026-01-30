---
name: viewmodel-generator
description: "C# MVVM WPF ViewModel生成器技能：负责INotifyPropertyChanged实现、ObservableCollection管理、RelayCommand生成、属性变更通知、ViewModel基类设计。使用时需要实现INotifyPropertyChanged、管理ObservableCollection、生成RelayCommand、处理属性变更通知、设计ViewModel基类。"
---

# viewmodel-generator Skill

为C# MVVM WPF项目提供专业的ViewModel生成和配置能力，确保ViewModel具有规范、一致、高效的实现。

## When to Use This Skill

Trigger when any of these applies:
- 需要实现INotifyPropertyChanged
- 需要管理ObservableCollection
- 需要生成RelayCommand
- 需要处理属性变更通知
- 需要设计ViewModel基类
- 需要优化现有ViewModel实现

## Not For / Boundaries

- 不负责具体业务逻辑的实现
- 不替代开发人员进行ViewModel设计决策
- 不处理ViewModel的深度性能优化（仅设计层面）
- 不负责ViewModel的测试和调试（仅实现）

## Quick Reference

### Common Patterns

**Pattern 1:** ViewModel基类设计
```csharp
// ViewModel基类
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
            return false;

        field = newValue;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

**Pattern 2:** RelayCommand实现
```csharp
// RelayCommand实现
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object parameter)
    {
        _execute();
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

// 带参数的RelayCommand
public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute((T)parameter);
    }

    public void Execute(object parameter)
    {
        _execute((T)parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

**Pattern 3:** ObservableCollection管理
```csharp
// ObservableCollection管理
public class UsersViewModel : ViewModelBase
{
    private ObservableCollection<User> _users;
    public ObservableCollection<User> Users
    {
        get { return _users; }
        set { SetProperty(ref _users, value); }
    }

    private User _selectedUser;
    public User SelectedUser
    {
        get { return _selectedUser; }
        set { SetProperty(ref _selectedUser, value); }
    }

    public ICommand AddUserCommand { get; }
    public ICommand RemoveUserCommand { get; }

    public UsersViewModel()
    {
        Users = new ObservableCollection<User>();
        
        // 订阅集合变更事件
        Users.CollectionChanged += Users_CollectionChanged;
        
        AddUserCommand = new RelayCommand(AddUser);
        RemoveUserCommand = new RelayCommand(RemoveUser, CanRemoveUser);
    }

    private void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // 处理集合变更
        RemoveUserCommand.RaiseCanExecuteChanged();
    }

    private void AddUser()
    {
        var newUser = new User { Name = "New User" };
        Users.Add(newUser);
        SelectedUser = newUser;
    }

    private void RemoveUser()
    {
        if (SelectedUser != null)
        {
            Users.Remove(SelectedUser);
            SelectedUser = Users.FirstOrDefault();
        }
    }

    private bool CanRemoveUser()
    {
        return SelectedUser != null;
    }
}
```

**Pattern 4:** 完整ViewModel示例
```csharp
// 完整ViewModel示例
public class ProductViewModel : ViewModelBase
{
    private string _name;
    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    private decimal _price;
    public decimal Price
    {
        get { return _price; }
        set { SetProperty(ref _price, value); }
    }

    private bool _isAvailable;
    public bool IsAvailable
    {
        get { return _isAvailable; }
        set { SetProperty(ref _isAvailable, value); }
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public ProductViewModel()
    {
        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Save()
    {
        // 保存逻辑
    }

    private bool CanSave()
    {
        return !string.IsNullOrEmpty(Name) && Price > 0;
    }

    private void Cancel()
    {
        // 取消逻辑
    }
}
```

**Pattern 5:** 属性变更通知
```csharp
// 属性变更通知
public class CustomerViewModel : ViewModelBase
{
    private string _firstName;
    public string FirstName
    {
        get { return _firstName; }
        set 
        {
            if (SetProperty(ref _firstName, value))
            {
                // 当FirstName变更时，同时更新FullName
                OnPropertyChanged(nameof(FullName));
            }
        }
    }

    private string _lastName;
    public string LastName
    {
        get { return _lastName; }
        set 
        {
            if (SetProperty(ref _lastName, value))
            {
                // 当LastName变更时，同时更新FullName
                OnPropertyChanged(nameof(FullName));
            }
        }
    }

    public string FullName
    {
        get { return $"{FirstName} {LastName}"; }
    }
}
```

## Examples

### Example 1: 实现ViewModel基类
- Input: 需要创建ViewModel基类
- Steps:
  1. 创建ViewModelBase类
  2. 实现INotifyPropertyChanged接口
  3. 添加OnPropertyChanged方法
  4. 添加SetProperty泛型方法
  5. 测试基类功能
- Expected output / acceptance: ViewModel基类正确实现并可使用

### Example 2: 生成RelayCommand
- Input: 需要为ViewModel生成命令
- Steps:
  1. 创建RelayCommand类
  2. 实现ICommand接口
  3. 添加构造函数和方法
  4. 在ViewModel中使用RelayCommand
  5. 测试命令功能
- Expected output / acceptance: RelayCommand正确生成并可使用

### Example 3: 管理ObservableCollection
- Input: 需要在ViewModel中管理集合
- Steps:
  1. 在ViewModel中添加ObservableCollection属性
  2. 订阅CollectionChanged事件
  3. 实现添加和删除方法
  4. 测试集合管理功能
- Expected output / acceptance: ObservableCollection正确管理并可使用

## References

- `references/index.md`: ViewModel开发最佳实践导航
- `references/viewmodel-base.md`: ViewModel基类设计指南
- `references/relay-command.md`: RelayCommand实现指南
- `references/observable-collection.md`: ObservableCollection管理指南
- `references/property-change.md`: 属性变更通知指南

## Maintenance

- Sources: WPF官方文档和MVVM模式最佳实践
- Last updated: 2026-01-21
- Known limits: 不负责具体业务逻辑的实现