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
        Point currentPoint = new Point();
        //Point currentPoint2 = new Point();
        Line line;
        public BoardControl()
        {
            InitializeComponent();
            btn_eraser.Click += btn_eraser_Click;
            btn_circle.Click += btn_circle_Click;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Pressed)
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
                else
                {
                    paintSurface.Children.Remove(line);
                }
            }
        }

        private void btn_eraser_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
        }

        private void btn_circle_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
            isCircleDrawing = true;
            isRectangleDrawing = false;
            isLineDrawing = false;
        }

        private void btn_pen_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = true;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = false;
        }

        private void btn_rectangle_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = true;
            isLineDrawing = false;
        }

        private void btn_line_Click(object sender, RoutedEventArgs e)
        {
            isDrawing = false;
            isCircleDrawing = false;
            isRectangleDrawing = false;
            isLineDrawing = true;
        }

        private void drawEllipse(double width, double heigth)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = SystemColors.WindowFrameBrush;
            double circleHeigth = (heigth - 77) - (currentPoint.Y - 77);
            double circleWidth = width - currentPoint.X;

            if (circleWidth < 0)
            {
                ellipse.Width = -circleWidth;
                Canvas.SetLeft(ellipse, width);
                Canvas.SetRight(ellipse, currentPoint.X);
            }
            else
            {
                ellipse.Width = circleWidth;
                Canvas.SetLeft(ellipse, currentPoint.X);
                Canvas.SetRight(ellipse, width);
            }

            if(circleHeigth < 0)
            {
                ellipse.Height = -circleHeigth;
                Canvas.SetTop(ellipse, heigth - 77);
                Canvas.SetBottom(ellipse, currentPoint.Y - 77);
            }
            else
            {
                ellipse.Height = circleHeigth;
                Canvas.SetTop(ellipse, currentPoint.Y - 77);
                Canvas.SetBottom(ellipse, heigth - 77);
                
            }
            ellipse.Opacity = 1;

            paintSurface.Children.Add(ellipse);
        }

        private void drawRectangle(double width, double heigth)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Stroke = SystemColors.WindowFrameBrush;
            double rectangleHeigth = (heigth - 77) - (currentPoint.Y - 77);
            double rectangleWidth = width - currentPoint.X;

            if (rectangleWidth < 0)
            {
                rectangle.Width = -rectangleWidth;
                Canvas.SetLeft(rectangle, width);
                Canvas.SetRight(rectangle, currentPoint.X);
            }
            else
            {
                rectangle.Width = rectangleWidth;
                Canvas.SetLeft(rectangle, currentPoint.X);
                Canvas.SetRight(rectangle, width);
            }

            if (rectangleHeigth < 0)
            {
                rectangle.Height = -rectangleHeigth;
                Canvas.SetTop(rectangle, heigth - 77);
                Canvas.SetBottom(rectangle, currentPoint.Y - 77);
            }
            else
            {
                rectangle.Height = rectangleHeigth;
                Canvas.SetTop(rectangle, currentPoint.Y - 77);
                Canvas.SetBottom(rectangle, heigth - 77);

            }

            paintSurface.Children.Add(rectangle);
        }

        private Line drawStraight(double width, double heigth)
        {
            Line line = new Line();

            line.Stroke = SystemColors.WindowFrameBrush;
            line.X1 = currentPoint.X;
            line.Y1 = currentPoint.Y - 77;
            line.X2 = width;
            line.Y2 = heigth - 77;

            return line;
        }
    }
}
