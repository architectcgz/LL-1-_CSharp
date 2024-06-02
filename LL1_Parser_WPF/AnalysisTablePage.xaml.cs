using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Exp2_WPF
{
    public partial class AnalysisTablePage : Window
    {
        private Dictionary<(char, char), string> analysisTable;
        private string[,] array;

        public AnalysisTablePage(Dictionary<(char, char), string> analysisTable)
        {
            this.analysisTable = analysisTable;
            InitializeComponent();
            PopulateCanvas();
        }


        private void PopulateCanvas()
        {
            // 确定行名（非终结符）和列名（终结符）
            var uniqueTerminals = analysisTable.Keys.Select(k => k.Item2).Distinct().OrderBy(c => c).ToArray();
            var uniqueNonTerminals = analysisTable.Keys.Select(k => k.Item1).Distinct().OrderBy(c => c).ToArray();

            int cellWidth = 70; // 单元格宽度
            int cellHeight = 40; // 单元格高度
            int leftMargin = 60; // 左边距
            int topMargin = 60; // 上边距，为列名留出空间

            // 绘制列名
            for (int col = 0; col < uniqueTerminals.Length; col++)
            {
                var textBlock = new TextBlock
                {
                    Text = uniqueTerminals[col].ToString(),
                    Foreground = Brushes.Black,
                    FontSize = 12,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Width = cellWidth - 10,
                    Height = cellHeight - 10, // 设置列名的高度
                    Margin = new Thickness(col * cellWidth + leftMargin, topMargin - 25, 0, 0) // 调整列名的垂直位置
                };
                TableCanvas.Children.Add(textBlock);
            }

            // 绘制行名
            for (int row = 0; row < uniqueNonTerminals.Length; row++)
            {
                var textBlock = new TextBlock
                {
                    Text = uniqueNonTerminals[row].ToString(),
                    Foreground = Brushes.Black,
                    FontSize = 12,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Width = 20, // 设置行名的宽度
                    Height = cellHeight,
                    Margin = new Thickness(leftMargin - 30, (row + 1) * cellHeight + topMargin, 0, 0) // 调整行名的水平位置
                };
                TableCanvas.Children.Add(textBlock);
            }

            // 绘制表格内容
            for (int row = 0; row < uniqueNonTerminals.Length; row++)
            {
                for (int col = 0; col < uniqueTerminals.Length; col++)
                {
                    var key = (uniqueNonTerminals[row], uniqueTerminals[col]);
                    var value = analysisTable.TryGetValue(key, out var entryValue) ? entryValue : string.Empty;
                    var formattedText = value != string.Empty ? $"{key.Item1}->{value}" : "ERROR";
                    // 创建矩形
                    var rect = new System.Windows.Shapes.Rectangle
                    {
                        Width = cellWidth,
                        Height = cellHeight,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = Brushes.White
                    };

                    // 定位矩形
                    Canvas.SetLeft(rect, col * cellWidth + leftMargin);
                    Canvas.SetTop(rect, row * cellHeight + topMargin + cellHeight); // 留出顶部空间给列名

                    // 创建文本块
                    var textBlock = new TextBlock
                    {
                        Text = formattedText,
                        Foreground = Brushes.Black,
                        FontSize = 12,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center
                    };

                    // 定位文本块
                    Canvas.SetLeft(textBlock, col * cellWidth + leftMargin);
                    Canvas.SetTop(textBlock, row * cellHeight + topMargin + cellHeight); // 留出顶部空间给列名

                    // 将图形添加到Canvas
                    TableCanvas.Children.Add(rect);
                    TableCanvas.Children.Add(textBlock);
                }
            }
        }

        
    }
}
