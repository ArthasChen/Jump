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

namespace Jump01
{
    /// <summary>
    /// PageStart.xaml 的交互逻辑
    /// </summary>
    public partial class PageStart : Page
    {
        public bool isStartGame = false;

        ScaleTransform change = new ScaleTransform();
        double x = 0.8;
        double y = 0.8;

        public PageStart()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point d = new Point(0.5, 0.5);
            PageStartButton.RenderTransformOrigin = d;
            change.ScaleX = x;
            change.ScaleY = y;

            PageStartButton.RenderTransform = change;

            isStartGame = true;
     
        }
    }
}
