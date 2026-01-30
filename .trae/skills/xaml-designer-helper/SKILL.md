---
name: xaml-designer-helper
description: "C# MVVM WPF XAML设计师助手技能：负责数据模板设计、控件模板定制、样式资源管理、触发器配置、值转换器(IValueConverter)实现。使用时需要设计数据模板、定制控件模板、管理样式资源、配置触发器、实现值转换器。"
---

# xaml-designer-helper Skill

为C# MVVM WPF项目提供专业的XAML设计和配置能力，确保UI具有美观、一致、可维护的实现。

## When to Use This Skill

Trigger when any of these applies:
- 需要设计数据模板
- 需要定制控件模板
- 需要管理样式资源
- 需要配置触发器
- 需要实现值转换器(IValueConverter)
- 需要优化现有XAML实现

## Not For / Boundaries

- 不负责具体业务逻辑的实现
- 不替代开发人员进行UI设计决策
- 不处理XAML的深度性能优化（仅设计层面）
- 不负责XAML的测试和调试（仅实现）

## Quick Reference

### Common Patterns

**Pattern 1:** 数据模板设计
```xml
<!-- 数据模板设计 -->
<DataTemplate x:Key="UserItemTemplate">
    <StackPanel Orientation="Horizontal" Margin="5">
        <Ellipse Width="20" Height="20" Fill="{Binding Status, Converter={StaticResource StatusToColorConverter}}">
            <Ellipse.ToolTip>
                <TextBlock Text="{Binding Status}"/>
            </Ellipse.ToolTip>
        </Ellipse>
        <TextBlock Text="{Binding Name}" Margin="10,0,0,0" FontWeight="Bold"/>
        <TextBlock Text="{Binding Email}" Margin="10,0,0,0" Foreground="Gray"/>
    </StackPanel>
</DataTemplate>

<!-- 使用数据模板 -->
<ListBox ItemsSource="{Binding Users}" ItemTemplate="{StaticResource UserItemTemplate}"/>
```

**Pattern 2:** 控件模板定制
```xml
<!-- 控件模板定制 -->
<ControlTemplate x:Key="CustomButtonTemplate" TargetType="Button">
    <Border x:Name="Border" 
            Background="{TemplateBinding Background}" 
            BorderBrush="{TemplateBinding BorderBrush}" 
            BorderThickness="{TemplateBinding BorderThickness}"
            CornerRadius="4">
        <ContentPresenter x:Name="ContentPresenter" 
                          Content="{TemplateBinding Content}" 
                          ContentTemplate="{TemplateBinding ContentTemplate}"
                          HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                          VerticalAlignment="{TemplateBinding VerticalContentAlignment}"
                          Margin="{TemplateBinding Padding}"/>
    </Border>
    <ControlTemplate.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter TargetName="Border" Property="Background" Value="#E0E0E0"/>
        </Trigger>
        <Trigger Property="IsPressed" Value="True">
            <Setter TargetName="Border" Property="Background" Value="#C0C0C0"/>
            <Setter TargetName="ContentPresenter" Property="Margin" Value="6,6,4,4"/>
        </Trigger>
        <Trigger Property="IsEnabled" Value="False">
            <Setter TargetName="Border" Property="Background" Value="#F0F0F0"/>
            <Setter TargetName="Border" Property="BorderBrush" Value="#E0E0E0"/>
            <Setter Property="Foreground" Value="#A0A0A0"/>
        </Trigger>
    </ControlTemplate.Triggers>
</ControlTemplate>

<!-- 使用控件模板 -->
<Button Template="{StaticResource CustomButtonTemplate}" Content="Click Me" Width="100" Height="30"/>
```

**Pattern 3:** 样式资源管理
```xml
<!-- 样式资源管理 -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- 颜色资源 -->
    <Color x:Key="PrimaryColor">#0066CC</Color>
    <Color x:Key="SecondaryColor">#0099FF</Color>
    <Color x:Key="AccentColor">#FF6600</Color>
    <Color x:Key="TextColor">#333333</Color>
    <Color x:Key="LightTextColor">#666666</Color>
    
    <!-- 画笔资源 -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource PrimaryColor}"/>
    <SolidColorBrush x:Key="SecondaryBrush" Color="{StaticResource SecondaryColor}"/>
    <SolidColorBrush x:Key="AccentBrush" Color="{StaticResource AccentColor}"/>
    <SolidColorBrush x:Key="TextBrush" Color="{StaticResource TextColor}"/>
    <SolidColorBrush x:Key="LightTextBrush" Color="{StaticResource LightTextColor}"/>
    
    <!-- 按钮样式 -->
    <Style x:Key="PrimaryButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="BorderThickness" Value="0"/>
        <Setter Property="Padding" Value="10,5"/>
        <Setter Property="Margin" Value="5"/>
        <Setter Property="FontWeight" Value="Bold"/>
    </Style>
    
    <!-- 文本框样式 -->
    <Style x:Key="DefaultTextBoxStyle" TargetType="TextBox">
        <Setter Property="BorderBrush" Value="{StaticResource SecondaryBrush}"/>
        <Setter Property="BorderThickness" Value="1"/>
        <Setter Property="Padding" Value="5"/>
        <Setter Property="Margin" Value="5"/>
        <Setter Property="FontSize" Value="14"/>
    </Style>
    
</ResourceDictionary>

<!-- 在App.xaml中引用 -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Styles/DefaultStyles.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

**Pattern 4:** 触发器配置
```xml
<!-- 触发器配置 -->
<Style x:Key="StatusTextBlockStyle" TargetType="TextBlock">
    <Setter Property="FontWeight" Value="Normal"/>
    <Setter Property="Foreground" Value="Gray"/>
    
    <!-- 属性触发器 -->
    <Style.Triggers>
        <Trigger Property="Text" Value="Active">
            <Setter Property="Foreground" Value="Green"/>
            <Setter Property="FontWeight" Value="Bold"/>
        </Trigger>
        <Trigger Property="Text" Value="Inactive">
            <Setter Property="Foreground" Value="Red"/>
        </Trigger>
        <Trigger Property="Text" Value="Pending">
            <Setter Property="Foreground" Value="Orange"/>
        </Trigger>
        
        <!-- 数据触发器 -->
        <DataTrigger Binding="{Binding IsAdmin}" Value="True">
            <Setter Property="FontStyle" Value="Italic"/>
        </DataTrigger>
        
        <!-- 多条件触发器 -->
        <MultiTrigger>
            <MultiTrigger.Conditions>
                <Condition Property="Text" Value="Active"/>
                <Condition Binding="{Binding IsAdmin}" Value="True"/>
            </MultiTrigger.Conditions>
            <Setter Property="TextDecorations" Value="Underline"/>
        </MultiTrigger>
    </Style.Triggers>
</Style>
```

**Pattern 5:** 值转换器(IValueConverter)实现
```csharp
// 值转换器实现
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            switch (status)
            {
                case "Active":
                    return Brushes.Green;
                case "Inactive":
                    return Brushes.Red;
                case "Pending":
                    return Brushes.Orange;
                default:
                    return Brushes.Gray;
            }
        }
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// 布尔值反转转换器
public class BooleanInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }
}

// 日期格式化转换器
public class DateToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            string format = parameter as string ?? "dd/MM/yyyy";
            return date.ToString(format);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string dateString)
        {
            DateTime.TryParse(dateString, out DateTime date);
            return date;
        }
        return value;
    }
}
```

## Examples

### Example 1: 设计数据模板
- Input: 需要为用户列表设计数据模板
- Steps:
  1. 创建数据模板
  2. 绑定数据属性
  3. 添加布局和样式
  4. 在控件中使用数据模板
  5. 测试数据模板效果
- Expected output / acceptance: 数据模板正确显示用户数据

### Example 2: 定制控件模板
- Input: 需要定制按钮控件模板
- Steps:
  1. 创建控件模板
  2. 添加视觉元素
  3. 配置触发器
  4. 在按钮中使用模板
  5. 测试控件模板效果
- Expected output / acceptance: 控件模板正确显示和响应

### Example 3: 实现值转换器
- Input: 需要实现状态到颜色的值转换器
- Steps:
  1. 创建值转换器类
  2. 实现IValueConverter接口
  3. 在XAML中注册转换器
  4. 在绑定中使用转换器
  5. 测试转换器效果
- Expected output / acceptance: 值转换器正确转换状态到颜色

## References

- `references/index.md`: XAML设计最佳实践导航
- `references/data-templates.md`: 数据模板设计指南
- `references/control-templates.md`: 控件模板定制指南
- `references/styles.md`: 样式资源管理指南
- `references/triggers.md`: 触发器配置指南
- `references/value-converters.md`: 值转换器实现指南

## Maintenance

- Sources: WPF官方文档和XAML设计最佳实践
- Last updated: 2026-01-21
- Known limits: 不负责具体业务逻辑的实现