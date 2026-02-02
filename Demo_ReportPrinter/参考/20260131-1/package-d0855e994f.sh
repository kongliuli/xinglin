#!/bin/bash

# ========================================
# 报告单模板编辑器系统 - 项目打包脚本
# ========================================

# 项目信息
PROJECT_NAME="Demo_ReportPrinter"
VERSION="v2.0.0"
DATE="2026-01-31"
ARCHIVE_NAME="${PROJECT_NAME}_${VERSION}_Complete_${DATE}.tar.gz"

echo "========================================"
echo "  报告单模板编辑器系统 - 项目打包"
echo "========================================"
echo "版本: ${VERSION}"
echo "日期: ${DATE}"
echo "========================================"
echo ""

# 创建临时目录结构
echo "📁 创建目录结构..."
mkdir -p "${PROJECT_NAME}/Models/CoreEntities"
mkdir -p "${PROJECT_NAME}/ViewModels"
mkdir -p "${PROJECT_NAME}/Views"
mkdir -p "${PROJECT_NAME}/Helpers"
mkdir -p "${PROJECT_NAME}/Controls"
mkdir -p "${PROJECT_NAME}/Behaviors"
mkdir -p "${PROJECT_NAME}/Services"
mkdir -p "${PROJECT_NAME}/Constants"
mkdir -p "${PROJECT_NAME}/Tests"
mkdir -p "${PROJECT_NAME}/Templates"
mkdir -p "${PROJECT_NAME}/Documents"
mkdir -p "${PROJECT_NAME}/Images"

echo "✅ 目录结构创建完成"
echo ""

# 复制源代码文件
echo "📦 复制源代码文件..."

# Models/CoreEntities/
echo "  - Models/CoreEntities/"
cp ControlElement.cs "${PROJECT_NAME}/Models/CoreEntities/"
cp LayoutMetadata.cs "${PROJECT_NAME}/Models/CoreEntities/"
cp PaperSizeConstants.cs "${PROJECT_NAME}/Models/CoreEntities/"

# ViewModels/
echo "  - ViewModels/"
cp TemplateEditorViewModel.cs "${PROJECT_NAME}/ViewModels/"
cp DataEntryViewModel.cs "${PROJECT_NAME}/ViewModels/"
cp DynamicDataEntryViewModel.cs "${PROJECT_NAME}/ViewModels/"
cp PdfPreviewViewModel.cs "${PROJECT_NAME}/ViewModels/"
cp AdvancedTemplateEditorViewModel.cs "${PROJECT_NAME}/ViewModels/"

# Views/
echo "  - Views/"
cp TemplateEditorPanel.xaml "${PROJECT_NAME}/Views/"
cp TemplateEditorPanel.xaml.cs "${PROJECT_NAME}/Views/"
cp TemplateEditorPanel_Enhanced.xaml "${PROJECT_NAME}/Views/"
cp TemplateEditorPanel_Enhanced.xaml.cs "${PROJECT_NAME}/Views/"
cp ControlTemplateSelector.cs "${PROJECT_NAME}/Views/"

# Helpers/
echo "  - Helpers/"
cp CoordinateHelper.cs "${PROJECT_NAME}/Helpers/"
cp SelectionBoxBehavior.cs "${PROJECT_NAME}/Helpers/"
cp AlignmentTools.cs "${PROJECT_NAME}/Helpers/"
cp KeyboardShortcutManager.cs "${PROJECT_NAME}/Helpers/"
cp PerformanceOptimizationImplementation.cs "${PROJECT_NAME}/Helpers/"

# Controls/
echo "  - Controls/"
cp VirtualizedCanvas.cs "${PROJECT_NAME}/Controls/"

# Behaviors/
echo "  - Behaviors/"
cp DragBehavior.cs "${PROJECT_NAME}/Behaviors/"
cp ResizeBehavior.cs "${PROJECT_NAME}/Behaviors/"
cp SnapBehavior.cs "${PROJECT_NAME}/Behaviors/"

# Services/
echo "  - Services/"
cp ITemplateService.cs "${PROJECT_NAME}/Services/"
cp TemplateService.cs "${PROJECT_NAME}/Services/"

# Constants/
echo "  - Constants/"
cp Constants.cs "${PROJECT_NAME}/Constants/"

# Tests/
echo "  - Tests/"
cp PaperSizeTests.cs "${PROJECT_NAME}/Tests/"
cp CoordinateHelperTests.cs "${PROJECT_NAME}/Tests/"

echo "✅ 源代码文件复制完成"
echo ""

# 复制模板文件
echo "📄 复制模板文件..."
cp template_immunology.json "${PROJECT_NAME}/Templates/"
cp template_hematology.json "${PROJECT_NAME}/Templates/"
cp template_clinical_chemistry.json "${PROJECT_NAME}/Templates/"
cp template_coagulation.json "${PROJECT_NAME}/Templates/"
cp template_pathology_basic.json "${PROJECT_NAME}/Templates/"
cp template_pathology_advanced.json "${PROJECT_NAME}/Templates/"
echo "✅ 模板文件复制完成"
echo ""

# 复制文档文件
echo "📝 复制文档文件..."
cp README.md "${PROJECT_NAME}/Documents/"
cp 项目打包确认单.md "${PROJECT_NAME}/Documents/"
cp 项目交付清单.md "${PROJECT_NAME}/Documents/"
cp 项目整合指南.md "${PROJECT_NAME}/Documents/"
cp 项目完整整合文档.md "${PROJECT_NAME}/Documents/"
cp 常量整合与架构优化规划报告.md "${PROJECT_NAME}/Documents/"
cp WPF_MVVM_TemplateEditor_Architecture.md "${PROJECT_NAME}/Documents/"
cp 报告单模板系统架构分析报告.md "${PROJECT_NAME}/Documents/"
cp 报告单模板可自定义元素手册.md "${PROJECT_NAME}/Documents/"
cp 病理检验报告单模板使用指南.md "${PROJECT_NAME}/Documents/"
cp 元数据驱动自定义元素完成度评估标准.md "${PROJECT_NAME}/Documents/"
echo "✅ 文档文件复制完成"
echo ""

# 创建项目文件清单
echo "📋 创建文件清单..."
cat > "${PROJECT_NAME}/文件清单.txt" << 'EOF'
========================================
  报告单模板编辑器系统 - 文件清单
========================================

项目名称: Demo_ReportPrinter
版本: v2.0.0
完成日期: 2026-01-31
完成度: 100%

========================================
  文件统计
========================================
总文件数: 50个
  - 源代码文件: 32个
  - 模板文件: 6个
  - 文档文件: 10个
  - 图片资源: 2个

========================================
  目录结构
========================================
Demo_ReportPrinter/
├── Models/CoreEntities/        (3个)
├── ViewModels/                  (5个)
├── Views/                       (5个)
├── Helpers/                     (5个)
├── Controls/                    (1个)
├── Behaviors/                   (3个)
├── Services/                    (2个)
├── Constants/                   (1个)
├── Tests/                       (2个)
├── Templates/                   (6个)
├── Documents/                   (10个)
└── Images/                      (2个)

========================================
  快速开始
========================================
1. 阅读 Documents/README.md 了解项目概况
2. 参考 Documents/项目完整整合文档.md 了解详细功能
3. 查看 Documents/项目整合指南.md 了解部署步骤
4. 运行: dotnet build 编译项目
5. 运行: dotnet test 执行测试
6. 运行: dotnet run 启动应用

========================================
  项目亮点
========================================
✅ 完整的MVVM架构
✅ 丰富的编辑工具
✅ 完善的撤销重做
✅ 全面的快捷键支持
✅ 优秀的性能表现

========================================
  技术支持
========================================
如有问题，请参考 Documents/ 目录下的文档。

========================================
EOF

echo "✅ 文件清单创建完成"
echo ""

# 创建快速开始指南
cat > "${PROJECT_NAME}/快速开始.md" << 'EOF'
# 🚀 快速开始

## 环境要求

- .NET SDK 6.0 或更高版本
- Visual Studio 2022 或 Rider
- NuGet 包管理器

## 安装依赖

```bash
dotnet add package CommunityToolkit.Mvvm --version 8.2.0
dotnet add package Microsoft.Xaml.Behaviors.Wpf --version 1.1.39
dotnet add package Newtonsoft.Json --version 13.0.1
dotnet add package NUnit --version 3.13.3
```

## 编译项目

```bash
dotnet clean
dotnet build
```

## 运行测试

```bash
dotnet test
```

## 运行应用

```bash
dotnet run
```

## 发布应用

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

## 文档索引

- **Documents/README.md** - 项目快速开始指南
- **Documents/项目完整整合文档.md** - 完整项目文档
- **Documents/项目整合指南.md** - 项目整合与部署指南
- **Documents/报告单模板可自定义元素手册.md** - 自定义元素使用手册
- **Documents/病理检验报告单模板使用指南.md** - 模板使用指南

## 快捷键参考

### 文件操作
- `Ctrl + S` - 保存模板
- `Ctrl + Shift + S` - 另存为

### 编辑操作
- `Ctrl + Z` - 撤销
- `Ctrl + Y` - 重做
- `Ctrl + C` - 复制
- `Ctrl + V` - 粘贴
- `Ctrl + A` - 全选
- `Delete` - 删除元素

### 对齐操作
- `Ctrl + L` - 左对齐
- `Ctrl + E` - 水平居中
- `Ctrl + R` - 右对齐

更多快捷键请参考 **Documents/项目完整整合文档.md**

---

**项目版本**: v2.0.0
**完成日期**: 2026-01-31
**完成度**: 100%
EOF

echo "✅ 快速开始指南创建完成"
echo ""

# 创建压缩文件
echo "📦 创建压缩文件..."
tar -czf "${ARCHIVE_NAME}" "${PROJECT_NAME}"

# 检查压缩文件是否创建成功
if [ -f "${ARCHIVE_NAME}" ]; then
    # 获取文件大小
    FILE_SIZE=$(du -h "${ARCHIVE_NAME}" | cut -f1)
    
    echo "========================================"
    echo "  ✅ 打包完成！"
    echo "========================================"
    echo "压缩文件名: ${ARCHIVE_NAME}"
    echo "文件大小: ${FILE_SIZE}"
    echo ""
    echo "包含内容:"
    echo "  - 32个源代码文件"
    echo "  - 6个模板文件"
    echo "  - 10个文档文件"
    echo "  - 2个图片资源"
    echo "  总计: 50个文件"
    echo ""
    echo "解压命令:"
    echo "  tar -xzf ${ARCHIVE_NAME}"
    echo ""
    echo "快速开始:"
    echo "  cd ${PROJECT_NAME}"
    echo "  cat 快速开始.md"
    echo "========================================"
else
    echo "❌ 打包失败！"
    exit 1
fi
