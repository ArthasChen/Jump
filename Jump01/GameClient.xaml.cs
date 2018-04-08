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
       


        //PageGameContent PGgamelogic = new PageGameContent();//改到了进入游戏界面的前一帧里。假如放在这里，进入此界面游戏中的player就已经下落了
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
                    PGstart.ResetSizeStartButton();
                    PageGameContent PGgamelogic = new PageGameContent();//这是进入游戏界面的前一帧，在这帧中实例化游戏逻辑，以至于进入游戏可以看到小人落下，假如在23行实例化，还没点进入游戏，小人就已经落下了
                    PGgamelogic.gameoverpanel += PGgamelogic_gameoverpanel;              
                    frameConuts = 0;
                    PGstart.isStartGame = false;
                    PGgamelogic.Focus();
                    this.Content = PGgamelogic;
                    CompositionTarget.Rendering -= CompositionTarget_Rendering2;                
                }          
            }         
        }

        private void PGgamelogic_gameoverpanel(int seclectionIndex)
        {
            if (seclectionIndex == 1)//回到主页 PageStart
            {
                //MessageBox.Show("1");
                RunPageStart();
                CompositionTarget.Rendering += CompositionTarget_Rendering2;
            }
            if (seclectionIndex == 2)//重开游戏 PageGameContent
            {
               
                //MessageBox.Show("2");
                PGstart.ResetSizeStartButton();
                PageGameContent PGgamelogic = new PageGameContent();//这是进入游戏界面的前一帧，在这帧中实例化游戏逻辑，以至于进入游戏可以看到小人落下，假如在23行实例化，还没点进入游戏，小人就已经落下了
                PGgamelogic.gameoverpanel += PGgamelogic_gameoverpanel;
                frameConuts = 0;
                PGstart.isStartGame = false;
                PGgamelogic.Focus();
                this.Content = PGgamelogic;
                CompositionTarget.Rendering -= CompositionTarget_Rendering2;
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
