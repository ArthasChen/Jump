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
using System.Windows.Shapes;

namespace Jump01
{
    /// <summary>
    /// GameClient.xaml 的交互逻辑
    /// </summary>
    public partial class GameClient : Window
    {
        PageStart PGstart = new PageStart();
        PageGameContent PGgamelogic = new PageGameContent();
        public int frameConuts = 0;
        public GameClient()
        {
            InitializeComponent();
            RunPageStart();
            CompositionTarget.Rendering += CompositionTarget_Rendering2;
        }
        public void RunPageStart()
        {
            this.Content = PGstart;
        }
        private void CompositionTarget_Rendering2(object sender, EventArgs e)
        {
            if (PGstart.isStartGame)
            {
                if (frameConuts++ < 1)
                {

                }
                else
                {
                    frameConuts = 0;
                    PGstart.isStartGame = false;
                    PGgamelogic.Focus();
                    this.Content = PGgamelogic;
                    CompositionTarget.Rendering -= CompositionTarget_Rendering2; ;                  
                }          
            }         
        }

     

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key==Key.Space)
            //{
            //    MessageBox.Show("Window Space is Ok");

            //}
            
        }
    }
}
