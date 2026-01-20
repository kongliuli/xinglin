using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models.Commands;
using ReportTemplateEditor.Designer.Constants;

namespace ReportTemplateEditor.Designer.Services
{
    public class PropertyPanelManager
    {
        private readonly Action<CommandBase> _executeCommand;
        private readonly Action<string> _showStatus;

        public PropertyPanelManager(Action<CommandBase> executeCommand, Action<string> showStatus)
        {
            _executeCommand = executeCommand ?? throw new ArgumentNullException(nameof(executeCommand));
            _showStatus = showStatus ?? throw new ArgumentNullException(nameof(showStatus));
        }

        public void UpdatePropertyPanel(ElementBase element, Control propertyPanel)
        {
            if (element == null || propertyPanel == null)
                return;

            var selectionInfoText = propertyPanel.FindName("selectionInfoText") as TextBlock;
            if (selectionInfoText != null)
            {
                selectionInfoText.Text = $"{element.Type} 元素";
            }

            EnableCommonProperties(propertyPanel, true);
            UpdateCommonProperties(element, propertyPanel);

            switch (element.Type)
            {
                case "Text":
                    UpdateTextElementProperties((TextElement)element, propertyPanel);
                    break;
                case "Label":
                    UpdateLabelElementProperties((LabelElement)element, propertyPanel);
                    break;
                case "Image":
                    UpdateImageElementProperties((ImageElement)element, propertyPanel);
                    break;
                case "Table":
                    UpdateTableElementProperties((TableElement)element, propertyPanel);
                    break;
                case "Barcode":
                    UpdateBarcodeElementProperties((BarcodeElement)element, propertyPanel);
                    break;
                case "Signature":
                    UpdateSignatureElementProperties((SignatureElement)element, propertyPanel);
                    break;
                case "AutoNumber":
                    UpdateAutoNumberElementProperties((AutoNumberElement)element, propertyPanel);
                    break;
                case "LabelInputBox":
                    UpdateLabelInputBoxProperties((LabelInputBoxElement)element, propertyPanel);
                    break;
                default:
                    DisableTextProperties(propertyPanel);
                    break;
            }
        }

        private void EnableCommonProperties(Control propertyPanel, bool enable)
        {
            var posXTextBox = propertyPanel.FindName(PropertyPanelControlNames.PosXTextBox) as TextBox;
            var posYTextBox = propertyPanel.FindName(PropertyPanelControlNames.PosYTextBox) as TextBox;
            var widthTextBox = propertyPanel.FindName(PropertyPanelControlNames.WidthTextBox) as TextBox;
            var heightTextBox = propertyPanel.FindName(PropertyPanelControlNames.HeightTextBox) as TextBox;
            var visibleCheckBox = propertyPanel.FindName(PropertyPanelControlNames.VisibleCheckBox) as CheckBox;
            var rotationTextBox = propertyPanel.FindName(PropertyPanelControlNames.RotationTextBox) as TextBox;
            var zIndexTextBox = propertyPanel.FindName(PropertyPanelControlNames.ZIndexTextBox) as TextBox;
            var opacitySlider = propertyPanel.FindName(PropertyPanelControlNames.OpacitySlider) as Slider;
            var backgroundColorTextBox = propertyPanel.FindName(PropertyPanelControlNames.BackgroundColorTextBox) as TextBox;
            var borderColorTextBox = propertyPanel.FindName(PropertyPanelControlNames.BorderColorTextBox) as TextBox;
            var borderWidthTextBox = propertyPanel.FindName(PropertyPanelControlNames.BorderWidthTextBox) as TextBox;
            var borderStyleComboBox = propertyPanel.FindName(PropertyPanelControlNames.BorderStyleComboBox) as ComboBox;
            var cornerRadiusTextBox = propertyPanel.FindName(PropertyPanelControlNames.CornerRadiusTextBox) as TextBox;
            var shadowColorTextBox = propertyPanel.FindName(PropertyPanelControlNames.ShadowColorTextBox) as TextBox;
            var shadowDepthTextBox = propertyPanel.FindName(PropertyPanelControlNames.ShadowDepthTextBox) as TextBox;
            var textAlignmentComboBox = propertyPanel.FindName(PropertyPanelControlNames.TextAlignmentComboBox) as ComboBox;
            var verticalAlignmentComboBox = propertyPanel.FindName(PropertyPanelControlNames.VerticalAlignmentComboBox) as ComboBox;
            var foregroundColorTextBox = propertyPanel.FindName(PropertyPanelControlNames.ForegroundColorTextBox) as TextBox;
            var textBackgroundColorTextBox = propertyPanel.FindName(PropertyPanelControlNames.TextBackgroundColorTextBox) as TextBox;

            if (posXTextBox != null) posXTextBox.IsEnabled = enable;
            if (posYTextBox != null) posYTextBox.IsEnabled = enable;
            if (widthTextBox != null) widthTextBox.IsEnabled = enable;
            if (heightTextBox != null) heightTextBox.IsEnabled = enable;
            if (visibleCheckBox != null) visibleCheckBox.IsEnabled = enable;
            if (rotationTextBox != null) rotationTextBox.IsEnabled = enable;
            if (zIndexTextBox != null) zIndexTextBox.IsEnabled = enable;
            if (opacitySlider != null) opacitySlider.IsEnabled = enable;
            if (backgroundColorTextBox != null) backgroundColorTextBox.IsEnabled = enable;
            if (borderColorTextBox != null) borderColorTextBox.IsEnabled = enable;
            if (borderWidthTextBox != null) borderWidthTextBox.IsEnabled = enable;
            if (borderStyleComboBox != null) borderStyleComboBox.IsEnabled = enable;
            if (cornerRadiusTextBox != null) cornerRadiusTextBox.IsEnabled = enable;
            if (shadowColorTextBox != null) shadowColorTextBox.IsEnabled = enable;
            if (shadowDepthTextBox != null) shadowDepthTextBox.IsEnabled = enable;
            if (textAlignmentComboBox != null) textAlignmentComboBox.IsEnabled = enable;
            if (verticalAlignmentComboBox != null) verticalAlignmentComboBox.IsEnabled = enable;
            if (foregroundColorTextBox != null) foregroundColorTextBox.IsEnabled = enable;
            if (textBackgroundColorTextBox != null) textBackgroundColorTextBox.IsEnabled = enable;
        }

        private void UpdateCommonProperties(ElementBase element, Control propertyPanel)
        {
            var posXTextBox = propertyPanel.FindName("posXTextBox") as TextBox;
            var posYTextBox = propertyPanel.FindName("posYTextBox") as TextBox;
            var widthTextBox = propertyPanel.FindName("widthTextBox") as TextBox;
            var heightTextBox = propertyPanel.FindName("heightTextBox") as TextBox;
            var visibleCheckBox = propertyPanel.FindName("visibleCheckBox") as CheckBox;
            var rotationTextBox = propertyPanel.FindName("rotationTextBox") as TextBox;
            var zIndexTextBox = propertyPanel.FindName("zIndexTextBox") as TextBox;
            var opacitySlider = propertyPanel.FindName("opacitySlider") as Slider;
            var backgroundColorTextBox = propertyPanel.FindName("backgroundColorTextBox") as TextBox;
            var borderColorTextBox = propertyPanel.FindName("borderColorTextBox") as TextBox;
            var borderWidthTextBox = propertyPanel.FindName("borderWidthTextBox") as TextBox;
            var borderStyleComboBox = propertyPanel.FindName("borderStyleComboBox") as ComboBox;
            var cornerRadiusTextBox = propertyPanel.FindName("cornerRadiusTextBox") as TextBox;
            var shadowColorTextBox = propertyPanel.FindName("shadowColorTextBox") as TextBox;
            var shadowDepthTextBox = propertyPanel.FindName("shadowDepthTextBox") as TextBox;

            if (posXTextBox != null) posXTextBox.Text = element.X.ToString();
            if (posYTextBox != null) posYTextBox.Text = element.Y.ToString();
            if (widthTextBox != null) widthTextBox.Text = element.Width.ToString();
            if (heightTextBox != null) heightTextBox.Text = element.Height.ToString();
            if (visibleCheckBox != null) visibleCheckBox.IsChecked = element.IsVisible;
            if (rotationTextBox != null) rotationTextBox.Text = element.Rotation.ToString();
            if (zIndexTextBox != null) zIndexTextBox.Text = element.ZIndex.ToString();
            if (opacitySlider != null) opacitySlider.Value = element.Opacity;
            if (backgroundColorTextBox != null) backgroundColorTextBox.Text = element.BackgroundColor;
            if (borderColorTextBox != null) borderColorTextBox.Text = element.BorderColor;
            if (borderWidthTextBox != null) borderWidthTextBox.Text = element.BorderWidth.ToString();
            if (borderStyleComboBox != null) borderStyleComboBox.Text = element.BorderStyle;
            if (cornerRadiusTextBox != null) cornerRadiusTextBox.Text = element.CornerRadius.ToString();
            if (shadowColorTextBox != null) shadowColorTextBox.Text = element.ShadowColor;
            if (shadowDepthTextBox != null) shadowDepthTextBox.Text = element.ShadowDepth.ToString();
        }

        private void UpdateTextElementProperties(TextElement textElement, Control propertyPanel)
        {
            EnableTextProperties(propertyPanel, true);
            EnableDataBindingProperties(propertyPanel, true);

            var textContentTextBox = propertyPanel.FindName("textContentTextBox") as TextBox;
            var fontFamilyComboBox = propertyPanel.FindName("fontFamilyComboBox") as ComboBox;
            var fontSizeTextBox = propertyPanel.FindName("fontSizeTextBox") as TextBox;
            var fontWeightComboBox = propertyPanel.FindName("fontWeightComboBox") as ComboBox;
            var fontStyleComboBox = propertyPanel.FindName("fontStyleComboBox") as ComboBox;
            var textAlignmentComboBox = propertyPanel.FindName("textAlignmentComboBox") as ComboBox;
            var verticalAlignmentComboBox = propertyPanel.FindName("verticalAlignmentComboBox") as ComboBox;
            var foregroundColorTextBox = propertyPanel.FindName("foregroundColorTextBox") as TextBox;
            var textBackgroundColorTextBox = propertyPanel.FindName("textBackgroundColorTextBox") as TextBox;
            var dataPathTextBox = propertyPanel.FindName("dataPathTextBox") as TextBox;
            var formatStringTextBox = propertyPanel.FindName("formatStringTextBox") as TextBox;

            if (textContentTextBox != null) textContentTextBox.Text = textElement.Text;
            if (fontFamilyComboBox != null) fontFamilyComboBox.Text = textElement.FontFamily;
            if (fontSizeTextBox != null) fontSizeTextBox.Text = textElement.FontSize.ToString();
            if (fontWeightComboBox != null) fontWeightComboBox.Text = textElement.FontWeight;
            if (fontStyleComboBox != null) fontStyleComboBox.Text = textElement.FontStyle;

            if (textAlignmentComboBox != null)
            {
                textAlignmentComboBox.SelectedValuePath = "Tag";
                textAlignmentComboBox.SelectedValue = textElement.TextAlignment;
            }

            if (verticalAlignmentComboBox != null)
            {
                verticalAlignmentComboBox.SelectedValuePath = "Tag";
                verticalAlignmentComboBox.SelectedValue = textElement.VerticalAlignment;
            }

            if (foregroundColorTextBox != null) foregroundColorTextBox.Text = textElement.ForegroundColor;
            if (textBackgroundColorTextBox != null) textBackgroundColorTextBox.Text = textElement.BackgroundColor;
            if (dataPathTextBox != null) dataPathTextBox.Text = textElement.DataBindingPath;
            if (formatStringTextBox != null) formatStringTextBox.Text = textElement.FormatString;
        }

        private void UpdateLabelElementProperties(LabelElement labelElement, Control propertyPanel)
        {
            EnableTextProperties(propertyPanel, true);
            EnableDataBindingProperties(propertyPanel, false);

            var textContentTextBox = propertyPanel.FindName("textContentTextBox") as TextBox;
            var fontFamilyComboBox = propertyPanel.FindName("fontFamilyComboBox") as ComboBox;
            var fontSizeTextBox = propertyPanel.FindName("fontSizeTextBox") as TextBox;
            var fontWeightComboBox = propertyPanel.FindName("fontWeightComboBox") as ComboBox;
            var fontStyleComboBox = propertyPanel.FindName("fontStyleComboBox") as ComboBox;
            var textAlignmentComboBox = propertyPanel.FindName("textAlignmentComboBox") as ComboBox;
            var verticalAlignmentComboBox = propertyPanel.FindName("verticalAlignmentComboBox") as ComboBox;
            var foregroundColorTextBox = propertyPanel.FindName("foregroundColorTextBox") as TextBox;
            var textBackgroundColorTextBox = propertyPanel.FindName("textBackgroundColorTextBox") as TextBox;

            if (textContentTextBox != null) textContentTextBox.Text = labelElement.Text;
            if (fontFamilyComboBox != null) fontFamilyComboBox.Text = labelElement.FontFamily;
            if (fontSizeTextBox != null) fontSizeTextBox.Text = labelElement.FontSize.ToString();
            if (fontWeightComboBox != null) fontWeightComboBox.Text = labelElement.FontWeight;
            if (fontStyleComboBox != null) fontStyleComboBox.Text = labelElement.FontStyle;

            if (textAlignmentComboBox != null)
            {
                textAlignmentComboBox.SelectedValuePath = "Tag";
                textAlignmentComboBox.SelectedValue = labelElement.TextAlignment;
            }

            if (verticalAlignmentComboBox != null)
            {
                verticalAlignmentComboBox.SelectedValuePath = "Tag";
                verticalAlignmentComboBox.SelectedValue = labelElement.VerticalAlignment;
            }

            if (foregroundColorTextBox != null) foregroundColorTextBox.Text = labelElement.ForegroundColor;
            if (textBackgroundColorTextBox != null) textBackgroundColorTextBox.Text = labelElement.BackgroundColor;
        }

        private void UpdateImageElementProperties(ImageElement imageElement, Control propertyPanel)
        {
            DisableTextProperties(propertyPanel);
            EnableDataBindingProperties(propertyPanel, false);

            var imagePathTextBox = propertyPanel.FindName("imagePathTextBox") as TextBox;
            var stretchComboBox = propertyPanel.FindName("stretchComboBox") as ComboBox;
            var opacitySlider = propertyPanel.FindName("opacitySlider") as Slider;

            if (imagePathTextBox != null) imagePathTextBox.Text = imageElement.ImagePath ?? string.Empty;
            if (stretchComboBox != null) stretchComboBox.SelectedValue = imageElement.Stretch;
            if (opacitySlider != null) opacitySlider.Value = imageElement.Opacity;
        }

        private void UpdateTableElementProperties(TableElement tableElement, Control propertyPanel)
        {
            DisableTextProperties(propertyPanel);
            EnableDataBindingProperties(propertyPanel, false);

            var rowsTextBox = propertyPanel.FindName("rowsTextBox") as TextBox;
            var columnsTextBox = propertyPanel.FindName("columnsTextBox") as TextBox;
            var cellSpacingTextBox = propertyPanel.FindName("cellSpacingTextBox") as TextBox;
            var cellPaddingTextBox = propertyPanel.FindName("cellPaddingTextBox") as TextBox;

            if (rowsTextBox != null) rowsTextBox.Text = tableElement.Rows.ToString();
            if (columnsTextBox != null) columnsTextBox.Text = tableElement.Columns.ToString();
            if (cellSpacingTextBox != null) cellSpacingTextBox.Text = tableElement.CellSpacing.ToString();
            if (cellPaddingTextBox != null) cellPaddingTextBox.Text = tableElement.CellPadding.ToString();
        }

        private void UpdateBarcodeElementProperties(BarcodeElement barcodeElement, Control propertyPanel)
        {
            DisableTextProperties(propertyPanel);
            EnableDataBindingProperties(propertyPanel, true);
        }

        private void UpdateSignatureElementProperties(SignatureElement signatureElement, Control propertyPanel)
        {
            DisableTextProperties(propertyPanel);
            EnableDataBindingProperties(propertyPanel, false);
        }

        private void UpdateAutoNumberElementProperties(AutoNumberElement autoNumberElement, Control propertyPanel)
        {
            EnableTextProperties(propertyPanel, true);
            EnableDataBindingProperties(propertyPanel, false);
        }

        private void UpdateLabelInputBoxProperties(LabelInputBoxElement labelInputBoxElement, Control propertyPanel)
        {
            EnableCommonProperties(propertyPanel, false);
            DisableTextProperties(propertyPanel);
            EnableDataBindingProperties(propertyPanel, false);

            var txtLabelText = propertyPanel.FindName("txtLabelText") as TextBox;
            var cmbLabelPosition = propertyPanel.FindName("cmbLabelPosition") as ComboBox;
            var txtLabelFontSize = propertyPanel.FindName("txtLabelFontSize") as TextBox;
            var cmbLabelFontWeight = propertyPanel.FindName("cmbLabelFontWeight") as ComboBox;
            var txtLabelForegroundColor = propertyPanel.FindName("txtLabelForegroundColor") as TextBox;
            var txtLabelBackgroundColor = propertyPanel.FindName("txtLabelBackgroundColor") as TextBox;
            var txtInputWidth = propertyPanel.FindName("txtInputWidth") as TextBox;
            var txtInputHeight = propertyPanel.FindName("txtInputHeight") as TextBox;
            var txtDefaultValue = propertyPanel.FindName("txtDefaultValue") as TextBox;
            var txtPlaceholder = propertyPanel.FindName("txtPlaceholder") as TextBox;
            var txtInputBackgroundColor = propertyPanel.FindName("txtInputBackgroundColor") as TextBox;
            var txtInputBorderColor = propertyPanel.FindName("txtInputBorderColor") as TextBox;
            var txtInputBorderWidth = propertyPanel.FindName("txtInputBorderWidth") as TextBox;

            if (txtLabelText != null) txtLabelText.Text = labelInputBoxElement.LabelText;

            if (cmbLabelPosition != null)
            {
                var positionItem = cmbLabelPosition.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => (string)item.Tag == labelInputBoxElement.LabelPosition);
                if (positionItem != null)
                {
                    cmbLabelPosition.SelectedItem = positionItem;
                }
            }

            if (txtLabelFontSize != null) txtLabelFontSize.Text = labelInputBoxElement.LabelFontSize.ToString();

            if (cmbLabelFontWeight != null)
            {
                var fontWeightItem = cmbLabelFontWeight.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => (string)item.Tag == labelInputBoxElement.LabelFontWeight);
                if (fontWeightItem != null)
                {
                    cmbLabelFontWeight.SelectedItem = fontWeightItem;
                }
            }

            if (txtLabelForegroundColor != null) txtLabelForegroundColor.Text = labelInputBoxElement.LabelForegroundColor;
            if (txtLabelBackgroundColor != null) txtLabelBackgroundColor.Text = labelInputBoxElement.LabelBackgroundColor;
            if (txtInputWidth != null) txtInputWidth.Text = labelInputBoxElement.InputWidth.ToString();
            if (txtInputHeight != null) txtInputHeight.Text = labelInputBoxElement.InputHeight.ToString();
            if (txtDefaultValue != null) txtDefaultValue.Text = labelInputBoxElement.DefaultValue;
            if (txtPlaceholder != null) txtPlaceholder.Text = labelInputBoxElement.Placeholder;
            if (txtInputBackgroundColor != null) txtInputBackgroundColor.Text = labelInputBoxElement.InputBackgroundColor;
            if (txtInputBorderColor != null) txtInputBorderColor.Text = labelInputBoxElement.InputBorderColor;
            if (txtInputBorderWidth != null) txtInputBorderWidth.Text = labelInputBoxElement.InputBorderWidth.ToString();
        }

        private void EnableTextProperties(Control propertyPanel, bool enable)
        {
            var textContentTextBox = propertyPanel.FindName("textContentTextBox") as TextBox;
            var fontFamilyComboBox = propertyPanel.FindName("fontFamilyComboBox") as ComboBox;
            var fontSizeTextBox = propertyPanel.FindName("fontSizeTextBox") as TextBox;
            var fontWeightComboBox = propertyPanel.FindName("fontWeightComboBox") as ComboBox;
            var fontStyleComboBox = propertyPanel.FindName("fontStyleComboBox") as ComboBox;
            var textAlignmentComboBox = propertyPanel.FindName("textAlignmentComboBox") as ComboBox;
            var verticalAlignmentComboBox = propertyPanel.FindName("verticalAlignmentComboBox") as ComboBox;
            var foregroundColorTextBox = propertyPanel.FindName("foregroundColorTextBox") as TextBox;
            var textBackgroundColorTextBox = propertyPanel.FindName("textBackgroundColorTextBox") as TextBox;

            if (textContentTextBox != null) textContentTextBox.IsEnabled = enable;
            if (fontFamilyComboBox != null) fontFamilyComboBox.IsEnabled = enable;
            if (fontSizeTextBox != null) fontSizeTextBox.IsEnabled = enable;
            if (fontWeightComboBox != null) fontWeightComboBox.IsEnabled = enable;
            if (fontStyleComboBox != null) fontStyleComboBox.IsEnabled = enable;
            if (textAlignmentComboBox != null) textAlignmentComboBox.IsEnabled = enable;
            if (verticalAlignmentComboBox != null) verticalAlignmentComboBox.IsEnabled = enable;
            if (foregroundColorTextBox != null) foregroundColorTextBox.IsEnabled = enable;
            if (textBackgroundColorTextBox != null) textBackgroundColorTextBox.IsEnabled = enable;
        }

        private void DisableTextProperties(Control propertyPanel)
        {
            EnableTextProperties(propertyPanel, false);
        }

        private void EnableDataBindingProperties(Control propertyPanel, bool enable)
        {
            var dataPathTextBox = propertyPanel.FindName("dataPathTextBox") as TextBox;
            var formatStringTextBox = propertyPanel.FindName("formatStringTextBox") as TextBox;

            if (dataPathTextBox != null) dataPathTextBox.IsEnabled = enable;
            if (formatStringTextBox != null) formatStringTextBox.IsEnabled = enable;
        }

        public void HandlePropertyChange(ElementBase element, string propertyName, object value)
        {
            if (element == null)
                return;

            try
            {
                var command = new ModifyElementPropertyCommand(element, propertyName, value);
                _executeCommand?.Invoke(command);
            }
            catch (Exception ex)
            {
                _showStatus?.Invoke($"属性修改失败: {ex.Message}");
            }
        }
    }
}
