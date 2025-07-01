using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SquareGridVisualizer
{
    public partial class MainWindow : Window
    {
        private int gridSize = 2;
        private readonly double cellSize = 40;

        public MainWindow()
        {
            InitializeComponent();
            UpdateNText();
        }

        private void UpdateNText() => NValueText.Text = gridSize.ToString();

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            gridSize++;
            UpdateNText();
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            if (gridSize > 1)
            {
                gridSize--;
                UpdateNText();
            }
        }

        private void DrawGrid_Click(object sender, RoutedEventArgs e)
        {
            GridCanvas.Children.Clear();
            double canvasSize = gridSize * cellSize;
            GridCanvas.Width = canvasSize;
            GridCanvas.Height = canvasSize;

            int delay = 0;
            for (int size = 1; size <= gridSize; size++)
            {
                var color = new SolidColorBrush(Color.FromArgb(
                    (byte)(40 + 200 / size), // Transparency: smaller squares are more visible
                    (byte)(50 + size * 30 % 200),
                    (byte)(100 + size * 20 % 150),
                    (byte)(150 + size * 10 % 100)));

                for (int row = 0; row <= gridSize - size; row++)
                {
                    for (int col = 0; col <= gridSize - size; col++)
                    {
                        double left = col * cellSize;
                        double top = row * cellSize;
                        double side = cellSize * size;

                        var rect = new Rectangle
                        {
                            Width = side,
                            Height = side,
                            Fill = color,
                            Stroke = Brushes.White,
                            StrokeThickness = 1,
                            Opacity = 0
                        };

                        Canvas.SetLeft(rect, left);
                        Canvas.SetTop(rect, top);

                        // Smaller squares come on top
                        Panel.SetZIndex(rect, gridSize - size);

                        GridCanvas.Children.Add(rect);

                        // Fade-in animation
                        var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
                        {
                            BeginTime = TimeSpan.FromMilliseconds(delay)
                        };
                        rect.BeginAnimation(UIElement.OpacityProperty, fade);

                        delay += 15;
                    }
                }
            }

            ResultText.Text = $"Total unique squares: {CountTotalSquares(gridSize)}";
        }


        private int CountTotalSquares(int n)
        {
            int total = 0;
            for (int k = 1; k <= n; k++)
                total += (n - k + 1) * (n - k + 1);
            return total;
        }

        private void ExportPng_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new(
                (int)GridCanvas.Width,
                (int)GridCanvas.Height,
                96d, 96d, PixelFormats.Default);
            rtb.Render(GridCanvas);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = $"Grid_{gridSize}x{gridSize}.png",
                Filter = "PNG Image|*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                using FileStream fs = new FileStream(dialog.FileName, FileMode.Create);
                encoder.Save(fs);
                MessageBox.Show("PNG saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
