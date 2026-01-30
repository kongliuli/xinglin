---
name: code-style-reviewer
description: "C#/Python代码规范审查员技能：负责编码规范检查、命名约定验证、格式化配置(EditorConfig)、代码风格一致性、静态代码分析规则。使用时需要检查代码规范、配置格式化规则、确保代码风格一致性。"
---

# code-style-reviewer Skill

为C#/Python项目提供专业的代码规范审查和格式检查能力，确保项目代码符合行业最佳实践和团队约定。

## When to Use This Skill

Trigger when any of these applies:
- 需要检查代码是否符合编码规范
- 需要验证命名约定是否正确
- 需要配置EditorConfig等格式化规则
- 需要确保代码风格一致性
- 需要配置静态代码分析规则
- 需要审查现有代码的规范问题

## Not For / Boundaries

- 不负责修复代码逻辑错误
- 不替代开发人员进行代码实现
- 不处理项目架构设计问题
- 不提供业务逻辑的优化建议

## Quick Reference

### Common Patterns

**Pattern 1:** 创建EditorConfig配置文件
```ini
# EditorConfig is awesome: https://EditorConfig.org

# top-most EditorConfig file
root = true

# Unix-style newlines with a newline ending every file
[*]
end_of_line = lf
insert_final_newline = true

# Matches multiple files with brace expansion notation
# Set default charset
[*.{cs,py}]
charset = utf-8

# 4 space indentation
[*.cs]
indent_style = space
indent_size = 4

# Python specific settings
[*.py]
indent_style = space
indent_size = 4

# Tab indentation (no size specified)
[Makefile]
indent_style = tab
```

**Pattern 2:** C#静态代码分析配置
```xml
<!-- 在.csproj文件中添加 -->
<Project>
  <PropertyGroup>
    <AnalysisLevel>latest</AnalysisLevel>
    <Nullable>enable</Nullable>
    <WarningsAsErrors>CS8600;CS8602;CS8603</WarningsAsErrors>
  </PropertyGroup>
</Project>
```

**Pattern 3:** Python代码风格检查 (Flake8)
```bash
# 安装Flake8
pip install flake8

# 运行代码风格检查
flake8 .

# 创建Flake8配置文件
cat > .flake8 << EOF
[flake8]
max-line-length = 88
exclude = .git,__pycache__,build,dist
EOF
```

**Pattern 4:** C#代码格式化 (dotnet format)
```bash
# 安装dotnet format
.dotnet tool install -g dotnet-format

# 运行代码格式化
dotnet format

# 检查格式问题但不修复
dotnet format --check
```

## Examples

### Example 1: 配置C#项目的代码规范
- Input: 需要为C#项目配置代码规范和静态分析规则
- Steps:
  1. 创建EditorConfig文件
  2. 在.csproj中配置静态分析级别
  3. 设置警告为错误的规则
  4. 运行dotnet format检查格式
- Expected output / acceptance: 项目具有一致的代码格式和严格的静态分析规则

### Example 2: 配置Python项目的代码风格
- Input: 需要为Python项目配置代码风格和检查工具
- Steps:
  1. 创建EditorConfig文件
  2. 安装并配置Flake8
  3. 配置Black代码格式化工具
  4. 运行检查验证代码风格
- Expected output / acceptance: 项目代码符合PEP 8规范，具有一致的格式

## References

- `references/index.md`: 代码规范最佳实践导航
- `references/csharp-coding-standards.md`: C#编码规范
- `references/python-coding-standards.md`: Python编码规范
- `references/editorconfig-guide.md`: EditorConfig配置指南

## Maintenance

- Sources: 官方编码规范文档和行业最佳实践
- Last updated: 2026-01-20
- Known limits: 不支持其他编程语言的代码规范检查