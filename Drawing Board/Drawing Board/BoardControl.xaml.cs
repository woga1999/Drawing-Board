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
        Point currentPoint = new Point();
        //Point currentPoint2 = new Point();
        Line line;
        Brush brush = Brushes.Black;
        public BoardControl()
        {
            InitializeComponent();
            btn_eraser.Click += btn_eraser_Click;
            btn_circle.Click += btn_circle_Click;
        }
        
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this);
            }
        }
        
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isDrawing==true )
            {
                line = drawStraight(e.GetPosition(this).X, e.GetPosition(this).Y );

                currentPoint = e.GetPosition(this);
                paintSurface.Children.Add(line);
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
            }
        }
        

        private void btn_eraser_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isPainting = false;
            isErasing = true;
        }

        private void btn_circle_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
            isCircleDrawing = true;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isPainting = false;
            isErasing = false;
        }

        private void btn_pen_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = true;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isPainting = false;
            isErasing = false;
        }

        private void btn_rectangle_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = true;
            isLineDrawing = false;
            isPainting = false;
            isErasing = false;
        }

        private void btn_line_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = true;
            isPainting = false;
            isErasing = false;
        }

        private void btn_paint_Click(object sender, RoutedEventArgs e)
        {
            isPainting = true;
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
            isErasing = false;
        }

        private void drawEllipse(double width, double heigth)
        {
            Ellipse ellipse = new Ellipse();

            ellipse.Stroke = Brushes.Black;
            ellipse.Fill = Brushes.Transparent;

            ellipse.Height = Math.Abs((heigth - 77) - (currentPoint.Y - 77));
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

            if((heigth - 77) - (currentPoint.Y - 77) < 0)
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
            paintSurface.Children.Add(ellipse);
        }

        private void drawRectangle(double width, double heigth)
        {
            Rectangle rectangle = new Rectangle();
            
            rectangle.Stroke = Brushes.Black;
            rectangle.Fill = Brushes.Transparent;

            rectangle.Height = Math.Abs((heigth - 77) - (currentPoint.Y - 77));
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

            if ((heigth - 77) - (currentPoint.Y - 77) < 0)
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
            }

        }

        private Line drawStraight(double width, double heigth)
        {
            Line line = new Line();

            line.Stroke = Brushes.Black;
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

        private void btn_purple_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Purple;
        }
    }
}
