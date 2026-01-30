---
name: documentation-engineer
description: "C#/Python文档工程师技能：负责XML/Docstring注释生成、README编写、API文档维护、CHANGELOG更新、接口说明文档自动化。使用时需要生成代码注释、创建项目文档、维护API文档、更新变更日志。"
---

# documentation-engineer Skill

为C#/Python项目提供专业的文档编写和维护能力，确保项目具有清晰、完整、最新的文档。

## When to Use This Skill

Trigger when any of these applies:
- 需要为代码生成XML或Docstring注释
- 需要编写项目README文件
- 需要维护API文档
- 需要更新CHANGELOG文件
- 需要自动化生成接口说明文档
- 需要优化现有文档结构

## Not For / Boundaries

- 不负责编写技术实现细节之外的业务文档
- 不替代开发人员验证文档的技术准确性
- 不处理文档的翻译工作
- 不负责文档的排版和设计（仅内容）

## Quick Reference

### Common Patterns

**Pattern 1:** C# XML注释示例
```csharp
/// <summary>
/// 计算两个整数的和
/// </summary>
/// <param name="a">第一个整数</param>
/// <param name="b">第二个整数</param>
/// <returns>两个整数的和</returns>
/// <exception cref="ArgumentNullException">当参数为null时抛出</exception>
/// <example>
/// <code>
/// var result = Calculator.Add(1, 2);
/// // result = 3
/// </code>
/// </example>
public int Add(int a, int b)
{
    return a + b;
}
```

**Pattern 2:** Python Docstring注释示例
```python
def add(a, b):
    """
    计算两个整数的和

    Args:
        a (int): 第一个整数
        b (int): 第二个整数

    Returns:
        int: 两个整数的和

    Raises:
        TypeError: 当参数类型不是整数时抛出

    Examples:
        >>> add(1, 2)
        3
        >>> add(0, 0)
        0
    """
    if not isinstance(a, int) or not isinstance(b, int):
        raise TypeError("参数必须是整数")
    return a + b
```

**Pattern 3:** README文件模板
```markdown
# 项目名称

项目的简短描述，说明项目的主要功能和用途。

## 功能特性

- 特性1：描述
- 特性2：描述
- 特性3：描述

## 快速开始

### 安装

```bash
# 安装依赖
pip install -r requirements.txt  # Python
# 或
dotnet restore  # C#
```

### 使用示例

```python
# Python示例
from package import module

result = module.function()
print(result)
```

```csharp
// C#示例
using Package;

var result = Module.Function();
Console.WriteLine(result);
```

## 文档

- [API文档](docs/api.md)
- [使用指南](docs/guide.md)
- [贡献指南](CONTRIBUTING.md)

## 贡献

欢迎提交Issue和Pull Request！

## 许可证

MIT License
```

**Pattern 4:** CHANGELOG文件模板
```markdown
# CHANGELOG

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2023-12-31
### Added
- 初始版本发布
- 实现了核心功能A
- 实现了核心功能B

### Changed
- 优化了性能

### Fixed
- 修复了已知bug

## [0.1.0] - 2023-11-15
### Added
- 项目初始化
- 实现了基本功能
```

## Examples

### Example 1: 为C#代码生成XML注释
- Input: 需要为一个C#类库生成完整的XML注释
- Steps:
  1. 分析代码结构和公共API
  2. 为每个类、方法、属性添加XML注释
  3. 确保包含summary、param、returns、exception、example等标签
  4. 配置项目生成XML文档文件
- Expected output / acceptance: 所有公共API都有完整的XML注释，可生成API文档

### Example 2: 为Python项目创建README
- Input: 需要为一个Python项目创建README文件
- Steps:
  1. 收集项目信息（功能、安装、使用方法等）
  2. 按照标准模板组织内容
  3. 添加代码示例和文档链接
  4. 确保内容清晰、结构合理
- Expected output / acceptance: 完整的README文件，包含所有必要信息

### Example 3: 更新项目CHANGELOG
- Input: 需要为项目更新CHANGELOG文件
- Steps:
  1. 查看最新的git提交记录
  2. 按照Keep a Changelog格式组织变更
  3. 分类整理Added、Changed、Fixed等部分
  4. 添加版本号和发布日期
- Expected output / acceptance: 最新的CHANGELOG文件，反映项目的所有变更

## References

- `references/index.md`: 文档编写最佳实践导航
- `references/csharp-xml-comments.md`: C# XML注释指南
- `references/python-docstring.md`: Python Docstring指南
- `references/readme-template.md`: README模板
- `references/changelog-template.md`: CHANGELOG模板

## Maintenance

- Sources: 官方文档编写指南和行业最佳实践
- Last updated: 2026-01-20
- Known limits: 不支持复杂的文档生成系统配置