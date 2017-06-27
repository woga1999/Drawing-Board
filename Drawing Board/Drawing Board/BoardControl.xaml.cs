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
        Point currentPoint = new Point(); //현재 좌표 구하기 위해 쓰이는 객체
        Line line;
        Brush brush = Brushes.Black; //처음 브러쉬 색을 정해주고 브러쉬 변수명 
        AdornerLayer adornerLayer;
        UIElement selectedElement = null; //그 선택한 도형 정보를 받기 위한 클래스
        Point startPoint; //드래깅 할 때 시작 포인트좌표
        RotateTransform rotation = new RotateTransform();

        bool isDrawing = true; //맨 처음 혹은 펜 그리기 할 때 판단하는 bool
        bool isRectangleDrawing = false; //사각형 그리기 할 지 안 할지 판단
        bool isCircleDrawing = false; //원 그리기 할 지 안 할지 판단
        bool isLineDrawing = false; //직선 그리기 하는지 하지 않는지 판단
        bool isPainting = false; //페인팅 할 것인지 하지 않을 것인지 판단
        bool isErasing = false; //지우개 기능 쓸 것인지 쓰지 않을 것인지 판단
        bool isSpoiding = false; //색 뽑아내기 기능 쓸 것인지 판단
        bool isCursorClick = false; //커서 기능 쓸 것인지 안 쓸 것인지 판단
        bool isDown; //커서가 바탕에 있는지 없는지를 판단하는 변수
        bool isDragging; //drag하는지 하지 않는지를 판단하는 변수
        bool selected = false; //도형을 선택했는 지 하지 않았는지를 판단하는 변수
        
        private double originalLeft; //드래깅 하기 전 왼쪽 좌표 값
        private double originalTop; //드래깅 하기 전 위쪽 좌표 값
        double angle = 0;

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
        
        //canvas 내 지역 마우스 갖다 대서 누를 시 발동하는 이벤트
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this);
                if (isCursorClick == true)
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
                        startPoint = e.GetPosition(paintSurface);

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
            UIElement source = sender as UIElement;
            if (e.LeftButton == MouseButtonState.Pressed )
            {
                if (isDrawing == true || isErasing == true)
                {
                    line = drawStraight(e.GetPosition(this).X, e.GetPosition(this).Y);
                    currentPoint = e.GetPosition(this);
                    paintSurface.Children.Add(line);
                }
                //else if (source is Rectangle)
                //{
                //    Rectangle rect = (Rectangle)source;
                //    rotation.CenterX = Canvas.GetLeft(source) + rect.Width / 2;
                //    rotation.CenterY = Canvas.GetTop(source) + rect.Height / 2;
                //}
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
            if (e.ButtonState == MouseButtonState.Released)
            {
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
                //else if(isCursorClick == true)
                //{
                //    DragFinishedMouseHandler(sender, e);
                //}
            }
        }
        
        //원형 도형 그리는 함수
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
            ellipse.MouseMove += new MouseEventHandler(shape_MouseMove);
            //ellipse.MouseLeftButtonUp += new MouseButtonEventHandler(shape_MouseLeftButtonUp);

            paintSurface.Children.Add(ellipse);
        }

        //사각형 그리는 함수
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

            paintSurface.Children.Add(rectangle);
        }

        //원 누를 시 발동하는 이벤트
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
                    showColor.Fill = circle.Fill;
                }
                //else if(isCursorClick == true)
                //{
                //    shape_MouseDown(circle, e);
                //}
            }
        }

        //사각형 누를 시 발동하는 이벤트
        private void Rectangle_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
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
                    showColor.Fill = rect.Fill;
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
        
        //도형 마우스 갖다 댈 시 발생하느 이벤트 함수
        private void shape_MouseMove(object sender, MouseEventArgs e)
        {
           if (isCursorClick == true)
                {
                    Ellipse ell = (Ellipse)e.OriginalSource;

                    //else if (source is Rectangle)
                    //{
                    //    Rectangle rect = (Rectangle)source;
                    //    rotation.CenterX = Canvas.GetLeft(source) + rect.Width / 2;
                    //    rotation.CenterY = Canvas.GetTop(source) + rect.Height / 2;
                    //}
                    ell.RenderTransform = rotation;
                    rotation.Angle = angle; // yes, Angle is a double
                    angle += 1;
                }
            
        }

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

        //-------색상 버튼, 기능 버튼 선택 시 ---
        private void btn_black_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Black;
            showColor.Fill = brush;
        }

        private void btn_red_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Red;
            showColor.Fill = brush;
        }

        private void btn_blue_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Blue;
            showColor.Fill = brush;
        }

        private void btn_yellow_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Yellow;
            showColor.Fill = brush;
        }

        private void btn_green_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.Green;
            showColor.Fill = brush;
        }

        private void btn_white_Click(object sender, RoutedEventArgs e)
        {
            brush = Brushes.White;
            showColor.Fill = brush;
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

        // 드래깅 하게끔 하는 thumb를 이용하지 못 하는 이벤트 핸들러
        void DragFinishedMouseHandler(object sender, MouseButtonEventArgs e)
        {
            StopDragging();
            e.Handled = true;
            paintSurface.ReleaseMouseCapture();
        }

        // 도형 움직이는 걸 멈추게 하는 함수
        private void StopDragging()
        {
            if (isDown)
            {
                isDown = false;
                isDragging = false;
            }
        }

        //움직일 시 선택한 도형이 움직이게
        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                if ((isDragging == false) &&
                    ((Math.Abs(e.GetPosition(paintSurface).X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(paintSurface).Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    isDragging = true;

                if (isDragging)
                {
                    Point position = Mouse.GetPosition(paintSurface);
                    Canvas.SetTop(selectedElement, position.Y - (startPoint.Y - originalTop));
                    Canvas.SetLeft(selectedElement, position.X - (startPoint.X - originalLeft));
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
    

