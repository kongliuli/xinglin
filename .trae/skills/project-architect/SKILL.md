---
name: project-architect
description: "C#/Python项目架构师技能：负责项目结构生成、目录规范、解决方案文件管理、项目模板初始化、多项目依赖关系配置。使用时需要创建或优化项目结构、配置解决方案文件、设置项目依赖。"
---

# project-architect Skill

为C#/Python项目提供专业的架构设计和项目结构管理能力，确保项目具有良好的可维护性和扩展性。

## When to Use This Skill

Trigger when any of these applies:
- 需要创建新的C#或Python项目结构
- 需要优化现有项目的目录组织
- 需要配置解决方案文件(.sln/.pyproj)
- 需要初始化项目模板
- 需要管理多项目间的依赖关系
- 需要制定项目架构规范

## Not For / Boundaries

- 不负责具体业务逻辑的实现
- 不替代开发人员进行架构决策，仅提供建议和工具支持
- 不处理项目部署和运行环境配置
- 不负责代码实现细节

## Quick Reference

### Common Patterns

**Pattern 1:** 创建C#解决方案和项目结构
```bash
dotnet new sln -n MySolution
dotnet new classlib -n MyProject -o src/MyProject
dotnet sln add src/MyProject/MyProject.csproj
dotnet new xunit -n MyProject.Tests -o tests/MyProject.Tests
dotnet add tests/MyProject.Tests/MyProject.Tests.csproj reference src/MyProject/MyProject.csproj
dotnet sln add tests/MyProject.Tests/MyProject.Tests.csproj
```

**Pattern 2:** 创建Python项目结构
```bash
mkdir -p my_project/src/my_project my_project/tests my_project/docs
cd my_project
python -m venv venv
pip install pytest
cat > setup.py << EOF
from setuptools import setup, find_packages

setup(
    name="my_project",
    version="0.1.0",
    packages=find_packages(where="src"),
    package_dir={"": "src"},
)
EOF
```

**Pattern 3:** 配置多项目依赖关系(C#)
```bash
# 添加项目引用
dotnet add src/ProjectA/ProjectA.csproj reference src/ProjectB/ProjectB.csproj

# 添加NuGet包
dotnet add src/ProjectA/ProjectA.csproj package Newtonsoft.Json
```

**Pattern 4:** Python项目依赖管理
```bash
# 创建requirements.txt
pip freeze > requirements.txt

# 使用Pipenv
pip install pipenv
pipenv install
pipenv install --dev pytest
```

## Examples

### Example 1: 创建C#解决方案结构
- Input: 需要创建一个包含Web API、类库和测试项目的C#解决方案
- Steps:
  1. 创建解决方案文件
  2. 创建Web API项目
  3. 创建类库项目
  4. 创建测试项目
  5. 配置项目间依赖关系
- Expected output / acceptance: 完整的解决方案结构，包含正确的项目引用和依赖关系

### Example 2: 创建Python项目结构
- Input: 需要创建一个符合Python最佳实践的项目结构
- Steps:
  1. 创建标准的Python项目目录结构
  2. 设置虚拟环境
  3. 配置依赖管理
  4. 添加测试框架
- Expected output / acceptance: 完整的Python项目结构，包含src、tests、docs目录和正确的配置文件

## References

- `references/index.md`: 项目架构最佳实践导航
- `references/csharp-project-structure.md`: C#项目结构规范
- `references/python-project-structure.md`: Python项目结构规范
- `references/dependency-management.md`: 项目依赖管理指南

## Maintenance

- Sources: 项目架构最佳实践文档和官方指南
- Last updated: 2026-01-20
- Known limits: 不支持其他编程语言的项目架构设计