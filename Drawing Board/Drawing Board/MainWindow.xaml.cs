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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        BoardControl boardControl = new BoardControl();
        ScaleTransform scale = new ScaleTransform();//윈도우 크기 바뀔 때 필요한 객체
        double orginalWidth, originalHeight;

        public MainWindow()
        {
            InitializeComponent();
            MainGrid.Children.Add(boardControl);
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }
        void Window_SizeChanged(object sender, SizeChangedEventArgs e)//윈도우 사이즈에 따라 컨트롤 크기도 바뀌기 위해 쓰였다
        {
            ChangeSize(e.NewSize.Width, e.NewSize.Height);
        }

        void Window_Loaded(object sender, RoutedEventArgs e) //윈도우 창 실행되면 실행
        {
            orginalWidth = this.Width;
            originalHeight = this.Height;

            if (this.WindowState == WindowState.Maximized)
            {
                ChangeSize(this.ActualWidth, this.ActualHeight);
            }

            this.SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
        }

        private void ChangeSize(double width, double height)//창 크기에 따라 변화
        {
            scale.ScaleX = width / orginalWidth;
            scale.ScaleY = height / originalHeight;

            FrameworkElement rootElement = this.Content as FrameworkElement;

            rootElement.LayoutTransform = scale;
        }
    }
}
