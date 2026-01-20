using System;
using System.Collections.Generic;
using System.Linq;
using ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Core.Services
{
    public class TableCalculationService
    {
        public void CalculateColumnWidths(TableElement tableElement, double mmToPixel)
        {
            if (tableElement.ColumnWidths == null)
            {
                tableElement.ColumnWidths = new List<double>();
            }

            for (int col = 0; col < tableElement.Columns; col++)
            {
                double columnWidth = tableElement.ColumnWidths.Count > col && tableElement.ColumnWidths[col] > 0
                    ? tableElement.ColumnWidths[col]
                    : (tableElement.Width - tableElement.CellSpacing * (tableElement.Columns + 1)) / tableElement.Columns;

                while (tableElement.ColumnWidths.Count <= col)
                {
                    tableElement.ColumnWidths.Add(20);
                }

                tableElement.ColumnWidths[col] = columnWidth;
            }
        }

        public void AutoCalculateColumnWidths(TableElement tableElement)
        {
            if (tableElement.ColumnWidths == null)
            {
                tableElement.ColumnWidths = new List<double>();
            }

            for (int col = 0; col < tableElement.Columns; col++)
            {
                double maxContentWidth = 0;

                for (int row = 0; row < tableElement.Rows; row++)
                {
                    var cell = tableElement.Cells?.FirstOrDefault(c => c.RowIndex == row && c.ColumnIndex == col);
                    if (cell != null)
                    {
                        double contentWidth = MeasureCellContentWidth(cell);
                        if (contentWidth > maxContentWidth)
                        {
                            maxContentWidth = contentWidth;
                        }
                    }
                }

                double columnWidth = Math.Max(maxContentWidth, 10);

                while (tableElement.ColumnWidths.Count <= col)
                {
                    tableElement.ColumnWidths.Add(20);
                }

                tableElement.ColumnWidths[col] = columnWidth;
            }
        }

        public void CalculateRowHeights(TableElement tableElement)
        {
            if (tableElement.RowHeights == null)
            {
                tableElement.RowHeights = new List<double>();
            }

            for (int row = 0; row < tableElement.Rows; row++)
            {
                double rowHeight = tableElement.RowHeights.Count > row && tableElement.RowHeights[row] > 0
                    ? tableElement.RowHeights[row]
                    : CalculateRowHeight(row, tableElement);

                while (tableElement.RowHeights.Count <= row)
                {
                    tableElement.RowHeights.Add(20);
                }

                tableElement.RowHeights[row] = rowHeight;
            }
        }

        private double CalculateRowHeight(int row, TableElement tableElement)
        {
            double maxHeight = 0;

            for (int col = 0; col < tableElement.Columns; col++)
            {
                var cellData = tableElement.Cells?.FirstOrDefault(c => c.RowIndex == row && c.ColumnIndex == col);
                if (cellData != null)
                {
                    double contentHeight = MeasureCellContentHeight(cellData);
                    double cellPadding = tableElement.CellPadding * 2;
                    double cellHeight = contentHeight + cellPadding;
                    if (cellHeight > maxHeight)
                    {
                        maxHeight = cellHeight;
                    }
                }
            }

            return Math.Max(maxHeight, 10);
        }

        public double MeasureCellContentWidth(TableCell cell)
        {
            if (string.IsNullOrEmpty(cell.Content))
                return 10;

            double charWidth = cell.FontSize * 0.6;
            double contentWidth = cell.Content.Length * charWidth;
            double cellPadding = 5;

            return contentWidth + cellPadding * 2;
        }

        public double MeasureCellContentHeight(TableCell cell)
        {
            if (string.IsNullOrEmpty(cell.Content))
                return cell.FontSize;

            double lineHeight = cell.FontSize * 1.2;
            int lineCount = cell.Content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            double contentHeight = lineCount * lineHeight;

            return contentHeight;
        }

        public void ApplyColumnWidths(TableElement tableElement, double mmToPixel)
        {
            CalculateColumnWidths(tableElement, mmToPixel);
        }

        public void ApplyRowHeights(TableElement tableElement, double mmToPixel)
        {
            CalculateRowHeights(tableElement);
        }
    }
}
