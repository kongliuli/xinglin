using System.ComponentModel;
using Xinglin.Core.Elements;
using Xunit;

namespace Xinglin.Core.Tests;

/// <summary>
/// ElementBase类的单元测试
/// </summary>
public class ElementBaseTests
{
    /// <summary>
    /// 测试ElementBase的基本属性设置和获取
    /// </summary>
    [Fact]
    public void ElementBase_Properties_SetAndGetCorrectly()
    {
        // Arrange
        var element = new TestElement();
        
        // Act
        element.X = 10;
        element.Y = 20;
        element.Width = 100;
        element.Height = 50;
        element.BackgroundColor = "#FF0000";
        element.ForegroundColor = "#00FF00";
        element.FontSize = 14;
        element.FontFamily = "Arial";
        
        // Assert
        Assert.Equal(10, element.X);
        Assert.Equal(20, element.Y);
        Assert.Equal(100, element.Width);
        Assert.Equal(50, element.Height);
        Assert.Equal("#FF0000", element.BackgroundColor);
        Assert.Equal("#00FF00", element.ForegroundColor);
        Assert.Equal(14, element.FontSize);
        Assert.Equal("Arial", element.FontFamily);
    }
    
    /// <summary>
    /// 测试ElementBase的Validate方法
    /// </summary>
    [Fact]
    public void ElementBase_Validate_ReturnsTrueForValidElement()
    {
        // Arrange
        var element = new TestElement
        {
            X = 10,
            Y = 20,
            Width = 100,
            Height = 50
        };
        
        // Act
        var result = element.Validate();
        
        // Assert
        Assert.True(result);
    }
    
    /// <summary>
    /// 测试ElementBase的Validate方法在无效元素时返回false
    /// </summary>
    [Fact]
    public void ElementBase_Validate_ReturnsFalseForInvalidElement()
    {
        // Arrange
        var element = new TestElement
        {
            X = -10, // 无效值：负数X坐标
            Y = 20,
            Width = 100,
            Height = 50
        };
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => element.Validate());
    }
    
    /// <summary>
    /// 测试ElementBase的ValidateBounds方法
    /// </summary>
    [Fact]
    public void ElementBase_ValidateBounds_ReturnsTrueForElementWithinBounds()
    {
        // Arrange
        var element = new TestElement
        {
            X = 10,
            Y = 20,
            Width = 100,
            Height = 50
        };
        
        // Act
        var result = element.ValidateBounds(200, 100);
        
        // Assert
        Assert.True(result);
    }
    
    /// <summary>
    /// 测试ElementBase的ValidateBounds方法在元素超出边界时返回false
    /// </summary>
    [Fact]
    public void ElementBase_ValidateBounds_ReturnsFalseForElementOutOfBounds()
    {
        // Arrange
        var element = new TestElement
        {
            X = 150,
            Y = 60,
            Width = 100,
            Height = 50
        };
        
        // Act
        var result = element.ValidateBounds(200, 100);
        
        // Assert
        Assert.False(result);
    }
    
    /// <summary>
    /// 测试ElementBase的PropertyChanged事件是否正确触发
    /// </summary>
    [Fact]
    public void ElementBase_PropertyChanged_EventIsRaisedWhenPropertyChanges()
    {
        // Arrange
        var element = new TestElement();
        var propertyChangedCount = 0;
        var changedPropertyName = string.Empty;
        
        element.PropertyChanged += (sender, e) =>
        {
            propertyChangedCount++;
            changedPropertyName = e.PropertyName;
        };
        
        // Act
        element.X = 100;
        
        // Assert
        Assert.Equal(1, propertyChangedCount);
        Assert.Equal("X", changedPropertyName);
    }
    
    /// <summary>
    /// 测试ElementBase的PropertyChanged事件在属性值未变化时不触发
    /// </summary>
    [Fact]
    public void ElementBase_PropertyChanged_EventIsNotRaisedWhenPropertyDoesNotChange()
    {
        // Arrange
        var element = new TestElement();
        var propertyChangedCount = 0;
        
        element.PropertyChanged += (sender, e) =>
        {
            propertyChangedCount++;
        };
        
        // Act
        element.X = 0; // X的默认值是0，设置相同的值不应该触发事件
        
        // Assert
        Assert.Equal(0, propertyChangedCount);
    }
    
    /// <summary>
    /// 测试ElementBase的构造函数是否正确初始化默认值
    /// </summary>
    [Fact]
    public void ElementBase_Constructor_InitializesDefaultValues()
    {
        // Arrange & Act
        var element = new TestElement();
        
        // Assert
        Assert.NotNull(element.Id);
        Assert.True(element.IsVisible);
        Assert.Equal(0, element.Rotation);
        Assert.Equal(0, element.ZIndex);
        Assert.Equal("#FFFFFF", element.BackgroundColor);
        Assert.Equal("#000000", element.BorderColor);
        Assert.Equal(0, element.BorderWidth);
        Assert.Equal("Solid", element.BorderStyle);
        Assert.Equal(0, element.CornerRadius);
        Assert.Equal(1, element.Opacity);
        Assert.Equal("#000000", element.ShadowColor);
        Assert.Equal(0, element.ShadowDepth);
        Assert.Equal("Left", element.HorizontalAlignment);
        Assert.Equal("Top", element.VerticalAlignment);
        Assert.False(element.IgnoreGlobalFontSize);
        Assert.Equal("Microsoft YaHei", element.FontFamily);
        Assert.Equal(12, element.FontSize);
        Assert.Equal("Normal", element.FontWeight);
        Assert.Equal("Normal", element.FontStyle);
        Assert.Equal("#000000", element.ForegroundColor);
        Assert.Equal("Left", element.TextAlignment);
    }
    
    /// <summary>
    /// 测试元素的类型属性
    /// </summary>
    [Fact]
    public void ElementBase_Type_ReturnsCorrectTypeName()
    {
        // Arrange & Act
        var element = new TestElement();
        
        // Assert
        Assert.Equal("Test", element.Type);
    }
    
    /// <summary>
    /// 测试元素的ID是否唯一
    /// </summary>
    [Fact]
    public void ElementBase_Id_IsUniqueForEachInstance()
    {
        // Arrange & Act
        var element1 = new TestElement();
        var element2 = new TestElement();
        
        // Assert
        Assert.NotEqual(element1.Id, element2.Id);
    }
}

/// <summary>
/// 用于测试的ElementBase子类
/// </summary>
internal class TestElement : ElementBase
{
    public override string Type => "Test";
}
