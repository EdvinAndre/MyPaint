using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace NewMyPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point Point = new Point();
        SolidColorBrush BrushCl = Brushes.Black;
        LinkedList<Shape> PaintingUndoHistory = new LinkedList<Shape>();
        LinkedList<Shape> PaintingRedoHistory = new LinkedList<Shape>();
        LinkedList<Shape> CurrentLine = new LinkedList<Shape>();
        string FileName;
        Line PrevLine;
        Rectangle PrevRectangle;
        Ellipse PrevCircle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                Point = e.GetPosition(this);
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Point.X > 120 && e.GetPosition(this).X > 120)
            {
                if (Pen.IsChecked == true)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Line line = new Line();

                        line.Stroke = BrushCl;
                        line.StrokeThickness = Convert.ToDouble(((ComboBoxItem)Size.SelectedItem).Tag);
                        line.X1 = point.X;
                        line.Y1 = point.Y;
                        line.X2 = e.GetPosition(this).X;
                        line.Y2 = e.GetPosition(this).Y;

                        Point = e.GetPosition(this);

                        paintCanvas.Children.Add(line);
                        CurrentLine.AddLast(line);
                    }
                    else if (e.LeftButton == MouseButtonState.Released && CurrentLine.Count > 0)
                    {
                        System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                        path.Stroke = BrushCl;
                        path.StrokeThickness = Convert.ToDouble(((ComboBoxItem)Size.SelectedItem).Tag);
                        PathGeometry pathGeometry = new PathGeometry();
                        PathFigure pathFigure = new PathFigure();
                        PathSegmentCollection pathSegments = new PathSegmentCollection();
                        PathFigureCollection pathFigures = new PathFigureCollection();

                        foreach (Line line in CurrentLine)
                        {
                            paintCanvas.Children.Remove(line);
                            if (line.Equals(CurrentLine.First.Value))
                            {
                                pathFigure.StartPoint = new Point(line.X1, line.Y1);
                            }
                            else
                            {
                                LineSegment lineSegment = new LineSegment();
                                lineSegment.Point = new Point(line.X2, line.Y2);
                                pathSegments.Add(lineSegment);
                            }
                        }
                        
                        CurrentLine.Clear();
                        pathFigure.Segments = pathSegments;
                        pathFigures.Add(pathFigure);
                        pathGeometry.Figures = pathFigures;

                        path.Data = pathGeometry;
                        path.Fill = Brushes.Transparent;
                        path.MouseDown += Mouse_DownInsideShape;
                        paintCanvas.Children.Add(path);
                        PaintingUndoHistory.AddLast(path);
                    }
                }
                else if (Line.IsChecked == true)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        paintCanvas.Children.Remove(PrevLine);
                        PaintingUndoHistory.Remove(PrevLine);
                        Line line = new Line();

                        line.Stroke = BrushCl;
                        line.StrokeThickness = Convert.ToDouble(((ComboBoxItem)Size.SelectedItem).Tag);
                        line.X1 = Point.X;
                        line.Y1 = Point.Y;
                        line.X2 = e.GetPosition(this).X;
                        line.Y2 = e.GetPosition(this).Y;

                        paintCanvas.Children.Add(line);
                        PaintingUndoHistory.AddLast(line);
                        PrevLine = line;
                    }
                    else if (e.LeftButton == MouseButtonState.Released)
                    {
                        PrevLine = null;
                    }
                }
                else if (Rectangle.IsChecked == true)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        paintCanvas.Children.Remove(PrevRectangle);
                        PaintingUndoHistory.Remove(PrevRectangle);
                        Rectangle rectangle = new Rectangle();

                        rectangle.Stroke = BrushCl;
                        rectangle.StrokeThickness = Convert.ToDouble(((ComboBoxItem)Size.SelectedItem).Tag);
                        rectangle.Width = Math.Abs(Point.X - e.GetPosition(this).X);
                        rectangle.Height = Math.Abs(Point.Y - e.GetPosition(this).Y);
                        rectangle.Margin = new Thickness(Point.X, Point.Y, 0, 0);
                        rectangle.Fill = Brushes.Transparent;
                        rectangle.MouseDown += Mouse_DownInsideShape;

                        paintCanvas.Children.Add(rectangle);
                        PaintingUndoHistory.AddLast(rectangle);
                        PrevRectangle = rectangle;
                    }
                    else if (e.LeftButton == MouseButtonState.Released)
                    {
                        PrevRectangle = null;
                    }
                }
                else if (Circle.IsChecked == true)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        paintCanvas.Children.Remove(PrevCircle);
                        PaintingUndoHistory.Remove(PrevCircle);
                        Ellipse circle = new Ellipse();

                        circle.Stroke = BrushCl;
                        circle.StrokeThickness = Convert.ToDouble(((ComboBoxItem)Size.SelectedItem).Tag);
                        circle.Width = Math.Abs(Point.X - e.GetPosition(this).X);
                        circle.Height = Math.Abs(Point.Y - e.GetPosition(this).Y);
                        circle.Margin = new Thickness(Point.X, Point.Y, 0, 0);
                        circle.Fill = Brushes.Transparent;
                        circle.MouseDown += Mouse_DownInsideShape;

                        paintCanvas.Children.Add(circle);
                        PaintingUndoHistory.AddLast(circle);
                        PrevCircle = circle;
                    }
                    else if (e.LeftButton == MouseButtonState.Released)
                    {
                        PrevCircle = null;
                    }
                }
                else if (Eraser.IsChecked == true)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Line line = new Line();

                        line.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        line.StrokeThickness = 8;
                        line.X1 = Point.X;
                        line.Y1 = Point.Y;
                        line.X2 = e.GetPosition(this).X;
                        line.Y2 = e.GetPosition(this).Y;

                        Point = e.GetPosition(this);

                        paintCanvas.Children.Add(line);
                    }
                }
            }
        }

        private void UndoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            paintCanvas.Children.Remove(PaintingUndoHistory.Last.Value);
            PaintingRedoHistory.AddLast(PaintingUndoHistory.Last.Value);
            PaintingUndoHistory.RemoveLast();
        }

        private void RedoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            paintCanvas.Children.Add(PaintingRedoHistory.Last.Value);
            PaintingUndoHistory.AddLast(PaintingRedoHistory.Last.Value);
            PaintingRedoHistory.RemoveLast();
        }

        private void CanExecuteUndoCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (PaintingUndoHistory.Count > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void CanExecuteRedoCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (PaintingRedoHistory.Count > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void paintCanvas_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                Uri filePath = new Uri(files[0]);
                ImagePreviewer.Source = new BitmapImage(filePath);
            }
        }

        private void Change_Color(object sender, RoutedEventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();

            if (colordialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                colorSelector.Background = new SolidColorBrush(Color.FromArgb(colordialog.Color.A, colordialog.Color.R, colordialog.Color.G, colordialog.Color.B));
                BrushCl = new SolidColorBrush(Color.FromArgb(colordialog.Color.A, colordialog.Color.R, colordialog.Color.G, colordialog.Color.B));
            }
        }

        private void Open_File(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Multiselect = false;
            Nullable<bool> result = openFile.ShowDialog();

            if (result == true)
            {
                FileName = openFile.FileName;
                Image imageFile = new Image();
                imageFile.Margin = new Thickness(120, 0, 0, 0);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = new Uri(FileName);
                bitmapImage.EndInit();

                imageFile.Source = bitmapImage;
                paintCanvas.Children.Add(imageFile);
            }
        }

        private void Save_FileAs(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFile = new Microsoft.Win32.SaveFileDialog();
            saveFile.FileName = "Nameless";
            saveFile.DefaultExt = ".png";
            saveFile.Filter = "PNG File (*.png)|*.png|Bitmap (*.bmp)|*.bmp";
            Nullable<bool> result = saveFile.ShowDialog();

            if (result == true)
            {
                FileName = saveFile.FileName;
                var extension = System.IO.Path.GetExtension(FileName);
                BitmapEncoder bmpEncoder;
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)paintCanvas.RenderSize.Width, (int)paintCanvas.RenderSize.Height, 96, 96, PixelFormats.Default);
                CroppedBitmap crop = new CroppedBitmap(rtb, new Int32Rect(120, 0, (int)paintCanvas.RenderSize.Width - 120, (int)paintCanvas.RenderSize.Height));

                rtb.Render(paintCanvas);

                switch (extension.ToLower())
                {
                    case ".bmp":
                        // Encode as BMP
                        bmpEncoder = new BmpBitmapEncoder();
                        bmpEncoder.Frames.Add(BitmapFrame.Create(crop));
                        break;
                    default:
                        // Encode as PNG
                        bmpEncoder = new PngBitmapEncoder();
                        bmpEncoder.Frames.Add(BitmapFrame.Create(crop));
                        break;
                }

                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    bmpEncoder.Save(stream);
                    stream.Dispose();
                    stream.Close();
                }
            }
        }

        private void Save_File(object sender, RoutedEventArgs e)
        {
            BitmapEncoder bmpEncoder;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)paintCanvas.RenderSize.Width, (int)paintCanvas.RenderSize.Height, 96, 96, PixelFormats.Default);
            CroppedBitmap crop = new CroppedBitmap(rtb, new Int32Rect(120, 0, (int)paintCanvas.RenderSize.Width - 120, (int)paintCanvas.RenderSize.Height));
            var extension = System.IO.Path.GetExtension(filename);

            rtb.Render(paintCanvas);

            switch (extension.ToLower())
            {
                case ".bmp":
                    // Encode as BMP
                    bmpEncoder = new BmpBitmapEncoder();
                    bmpEncoder.Frames.Add(BitmapFrame.Create(crop));
                    break;
                default:
                    // Encode as PNG
                    bmpEncoder = new PngBitmapEncoder();
                    bmpEncoder.Frames.Add(BitmapFrame.Create(crop));
                    break;
            }

            if (File.Exists(FileName))
                File.Delete(FileName);

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                bmpEncoder.Save(stream);
            }
        }

        private void Mouse_DownInsideShape(object sender, MouseButtonEventArgs e)
        {
            if (Bucket.IsChecked == true)
            {
                Shape source = e.Source as Shape;

                if (source != null)
                {
                    source.Fill = BrushCl;
                }
            }
        }
    }
}
