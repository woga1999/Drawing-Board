using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Drawing_Board
{
    /// <summary>
    /// BoardControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BoardControl : UserControl
    {
        bool isDrawing = true;
        bool isRectangleDrawing = false;
        bool isCircleDrawing = false;
        bool isLineDrawing = false;
        bool isPainting = false;
        bool isErasing = false;
        bool isSpoiding = false;
        bool isCursorClick = false;
        Point currentPoint = new Point();

        Line line;
        //Rectangle drawnRectangle;
        Brush brush = Brushes.Black;

        // -----------------------------
        AdornerLayer adornerLayer;

        bool isDown;
        bool isDragging;
        bool selected = false;
        UIElement selectedElement = null;
        Point _startPoint;
        private double originalLeft;
        private double originalTop;

        public BoardControl()
        {
            InitializeComponent();
            btn_eraser.Click += btn_eraser_Click;
            btn_circle.Click += btn_circle_Click;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DragFinishedMouseHandler);
            this.MouseMove += new MouseEventHandler(Window_MouseMove);
            this.MouseLeave += new MouseEventHandler(Window_MouseLeave);
        }
        
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this);
                if(isCursorClick == true)
                {
                    if (selected)
                    {
                        selected = false;
                        if (selectedElement != null)
                        {
                            // Remove the adorner from the selected element
                            paintSurface.ReleaseMouseCapture();
                            adornerLayer.Remove(adornerLayer.GetAdorners(selectedElement)[0]);
                            selectedElement = null;
                        }
                    }

                    // If any element except canvas is clicked, 
                    // assign the selected element and add the adorner
                    if (e.Source != paintSurface)
                    {
                        isDown = true;
                        _startPoint = e.GetPosition(paintSurface);

                        selectedElement = e.Source as UIElement;

                        originalLeft = Canvas.GetLeft(selectedElement);
                        originalTop = Canvas.GetTop(selectedElement);

                        adornerLayer = AdornerLayer.GetAdornerLayer(selectedElement);
                        adornerLayer.Add(new ResizeShapes(selectedElement));
                        selected = true;
                        e.Handled = true;
                    }
                }
            }
        }
        
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed )
            {
                if (isDrawing == true || isErasing == true)
                {
                    line = drawStraight(e.GetPosition(this).X, e.GetPosition(this).Y);
                    currentPoint = e.GetPosition(this);
                    paintSurface.Children.Add(line);
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
            if (e.ButtonState == MouseButtonState.Released)
            {
                //currentPoint2 = e.GetPosition(this);
                if (isCircleDrawing == true)
                {
                    drawEllipse(e.GetPosition(this).X, e.GetPosition(this).Y);
                }
                else if(isRectangleDrawing == true)
                {
                    drawRectangle(e.GetPosition(this).X, e.GetPosition(this).Y);
                    
                }
                else if(isLineDrawing == true)
                {
                    line = drawStraight(e.GetPosition(this).X, e.GetPosition(this).Y);
                    paintSurface.Children.Add(line);
                }
                else if(isCursorClick == true)
                {
                    DragFinishedMouseHandler(sender, e);
                }
            }
        }
        
        private void drawEllipse(double width, double heigth)
        {
            Ellipse ellipse = new Ellipse();


            ellipse.Stroke = brush;

            ellipse.Fill = Brushes.Transparent;

            ellipse.Height = Math.Abs(heigth - currentPoint.Y);
            ellipse.Width = Math.Abs(width - currentPoint.X);

            if (width - currentPoint.X < 0)
            {
                Canvas.SetLeft(ellipse, width);
                Canvas.SetRight(ellipse, currentPoint.X);
            }
            else
            {
                Canvas.SetLeft(ellipse, currentPoint.X);
                Canvas.SetRight(ellipse, width);
            }

            if (heigth - currentPoint.Y  < 0)
            {
                Canvas.SetTop(ellipse, heigth - 77);
                Canvas.SetBottom(ellipse, currentPoint.Y - 77);
            }
            else
            {
                Canvas.SetTop(ellipse, currentPoint.Y - 77);
                Canvas.SetBottom(ellipse, heigth - 77);
            }

            ellipse.MouseDown += new MouseButtonEventHandler(Ellipse_OnMouseDown);
            //ellipse.MouseMove += new MouseEventHandler(shape_MouseMove);
            //ellipse.MouseLeftButtonUp += new MouseButtonEventHandler(shape_MouseLeftButtonUp);

            paintSurface.Children.Add(ellipse);
        }

        private void drawRectangle(double width, double heigth)
        {
            Rectangle rectangle = new Rectangle();

            rectangle.Stroke = brush;

            rectangle.Fill = Brushes.Transparent;

            rectangle.Height = Math.Abs(heigth  - currentPoint.Y);
            rectangle.Width = Math.Abs(width - currentPoint.X);

            if (width - currentPoint.X < 0)
            {
                Canvas.SetLeft(rectangle, width);
                Canvas.SetRight(rectangle, currentPoint.X);
            }
            else
            {
                Canvas.SetLeft(rectangle, currentPoint.X);
                Canvas.SetRight(rectangle, width);
            }

            if (heigth- currentPoint.Y  < 0)
            {
                Canvas.SetTop(rectangle, heigth - 77);
                Canvas.SetBottom(rectangle, currentPoint.Y - 77);
            }
            else
            {
                Canvas.SetTop(rectangle, currentPoint.Y - 77);
                Canvas.SetBottom(rectangle, heigth - 77);
            }

            rectangle.MouseDown += new MouseButtonEventHandler(Rectangle_OnMouseDown);
            //rectangle.MouseMove += new MouseEventHandler(shape_MouseMove);
            //rectangle.MouseLeftButtonUp += new MouseButtonEventHandler(shape_MouseLeftButtonUp);

            paintSurface.Children.Add(rectangle);
        }

        private void Ellipse_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Ellipse circle = (Ellipse)e.OriginalSource;

                if (isPainting == true)
                {
                    circle.Fill = brush;
                }
                else if (isErasing == true)
                {
                    paintSurface.Children.Remove(circle);
                }
                else if (isSpoiding == true)
                {
                    brush = circle.Fill;
                    spoideRect.Fill = circle.Fill;
                }
                //else if(isCursorClick == true)
                //{
                //    shape_MouseDown(circle, e);
                //}
            }
        }

        private void Rectangle_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                //UIElement shape = e.Source as UIElement;
                if (isPainting == true)
                {
                    rect.Fill = brush;
                }
                else if(isErasing == true)
                {
                    paintSurface.Children.Remove(rect);
                }
                else if (isSpoiding == true)
                {
                    brush = rect.Fill;
                    spoideRect.Fill = rect.Fill;
                }
                //else if(isCursorClick == true)
                //{
                //    shape_MouseDown(rect, e);
                //}
            }
        }

        //private void shape_MouseDown(UIElement shape, MouseButtonEventArgs e)
        //{
        //    Mouse.Capture(shape);
        //    captured = true;
        //    x_shape = Canvas.GetLeft(shape);
        //    x_canvas = e.GetPosition(paintSurface).X;
        //    y_shape = Canvas.GetTop(shape);
        //    y_canvas = e.GetPosition(paintSurface).Y;
        //    ReleaseMouseCapture();
        //}

        //private void shape_MouseMove(object sender, MouseEventArgs e)
        //{
        //    UIElement source = sender as UIElement;
        //    if (captured)
        //    {
        //        double x = e.GetPosition(paintSurface).X;
        //        double y = e.GetPosition(paintSurface).Y;
        //        x_shape += x - x_canvas;
        //        Canvas.SetLeft(source, x_shape);
        //        x_canvas = x;
        //        y_shape += y - y_canvas;
        //        Canvas.SetTop(source, y_shape);
        //        y_canvas = y;
        //    }
        //}

        //private void shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    Mouse.Capture(null);
        //    ReleaseMouseCapture();
        //    captured = false;
        //}

        private Line drawStraight(double width, double heigth)
        {
            Line line = new Line();

            line.Stroke = brush;
            if(isErasing == true)
            {
                line.StrokeThickness = 15;
                line.Stroke = Brushes.White;
            }
            line.X1 = currentPoint.X;
            line.Y1 = currentPoint.Y - 77;
            line.X2 = width;
            line.Y2 = heigth - 77;

            return line;
        }

        private void btn_black_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Black;
        }

        private void btn_red_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Red;
        }

        private void btn_blue_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Blue;
        }

        private void btn_yellow_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Yellow;
        }

        private void btn_green_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Green;
        }

        private void btn_white_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.White;
        }

        private void btn_eraser_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = Cursors.Arrow;
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isPainting = false;
            isErasing = true;
            isSpoiding = false;
            isCursorClick = false;
        }

        private void btn_circle_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = Cursors.Cross;
            isDrawing = false;
            isCircleDrawing = true;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isPainting = false;
            isErasing = false;
            isSpoiding = false;
            isCursorClick = false;
        }

        private void btn_pen_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = Cursors.Pen;
            isDrawing = true;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isPainting = false;
            isErasing = false;
            isSpoiding = false;
            isCursorClick = false;
        }

        private void btn_rectangle_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = Cursors.Cross;
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = true;
            isLineDrawing = false;
            isPainting = false;
            isErasing = false;
            isSpoiding = false;
            isCursorClick = false;
        }

        private void btn_line_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = Cursors.Cross;
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = true;
            isPainting = false;
            isErasing = false;
            isSpoiding = false;
            isCursorClick = false;
        }

        private void btn_paint_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = Cursors.Arrow;
            isPainting = true;
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isErasing = false;
            isSpoiding = false;
            isCursorClick = false;
        }

        private void btn_spoide_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = Cursors.Arrow;
            isPainting = false;
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isErasing = false;
            isSpoiding = true;
            isCursorClick = false;
        }

        private void btn_cursor_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Cursor = Cursors.Arrow;
            isCursorClick = true;
            isRectangleDrawing = false;
            isPainting = false;
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isErasing = false;
            isSpoiding = false;
        }
        void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        // Handler for drag stopping on user choise
        void DragFinishedMouseHandler(object sender, MouseButtonEventArgs e)
        {
            StopDragging();
            e.Handled = true;
            paintSurface.ReleaseMouseCapture();
        }

        // Method for stopping dragging
        private void StopDragging()
        {
            if (isDown)
            {
                isDown = false;
                isDragging = false;
            }
        }

        // Hanler for providing drag operation with selected element
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                if ((isDragging == false) &&
                    ((Math.Abs(e.GetPosition(paintSurface).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(paintSurface).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    isDragging = true;

                if (isDragging)
                {
                    Point position = Mouse.GetPosition(paintSurface);
                    Canvas.SetTop(selectedElement, position.Y - (_startPoint.Y - originalTop));
                    Canvas.SetLeft(selectedElement, position.X - (_startPoint.X - originalLeft));
                }
            }
        }
        void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selected)
            {
                selected = false;
                if (selectedElement != null)
                {
                    adornerLayer.Remove(adornerLayer.GetAdorners(selectedElement)[0]);
                    selectedElement = null;
                }
            }
            else if(isPainting == true)
            {
                Canvas backGroud = e.OriginalSource as Canvas;
                if(backGroud is Canvas)
                {
                    paintSurface.Background = brush;
                }
            }
        }
    }
}
