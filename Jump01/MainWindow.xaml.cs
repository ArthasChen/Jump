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
using System.Data;

namespace Jump01
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {   
        //GameLogic
        public Person player = new Person();
        public Cube nowCube = new Cube();
        public Cube nextCube = new Cube();

        //Draw
        public DataGrid DataGrid0 = new DataGrid();
        public DataGrid DataGrid1 = new DataGrid();
        public DataGrid DataGrid2 = new DataGrid();
        public DataGrid DataGrid3 = new DataGrid();
        public DataTable PlayerData = new DataTable();
        public DataTable NowCubeData = new DataTable();
        public DataTable JudgeData = new DataTable();
        public DataTable NextCubeData = new DataTable();
        public double StepPerFrame = 0;      
        public int iFrames1 = 0;
        public int iFrames2 = 0;
        public int sShort1 = 1;
        public int sShort2 = 1;
        public int sLong1 = 1;
        public int sLong2 = 1;

        //Globl
        public  int nStateIndex = 0;
        public bool bGameOver = false;
        public DateTime PressStartTime = DateTime.MinValue;
        public double dPressTime = 0;
        public double RealTimePressTime = 0;
        public int FrameTotal = 0;
        public double showRealTimeStep = 0;



        public MainWindow()
        {
            InitializeComponent();
            PersonCubeInitialize();
            DrawDataGridTestInitialize();

            CompositionTarget.Rendering += CompositionTarget_Rendering;//主窗口加载完立即进入游戏，简化省略了开始游戏选择窗口。
        }

     
        //******************************* MainLoop *******************************//
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {          
            Logic();
            Draw();
            isGameContinue();        
        }

        public void Logic()
        {           
            RunLogic();
        }

        public void Draw()
        {
            ShowFrameTotal();//显示左上角累计帧数
            newpage();//当游戏结束时，加载的button
            DataGridTest1();//游戏参数更新显示
            TextTestData();//按压时间显示

           // RunDraw();
        }
        public void isGameContinue()
        {
            if (!bGameOver) return;
            CompositionTarget.Rendering -= CompositionTarget_Rendering; 
        }



        //******************************* GameLogic *******************************//
        #region GameLogic

        //初始化小人和2个Cube
        public void PersonCubeInitialize()
        {
            player.Position = 0;
            nowCube.SideLength = 94;//之前是40，39
            
            nextCube.RefreshDistance = 200;//之前是70
            nextCube.SideLength = 94;//之前是40

            nowCube.Position = 0;
            nextCube.Position = nextCube.RefreshDistance;

        }

        //游戏逻辑
        public void RunLogic()
        {
            switch (nStateIndex)
            {
                case 0://空闲状态，等待按压
                    {                    
                        textBlock.Text = "State 0";
                        //计算安全距离，需要4个参数,
                        player.ComputeSafeDistance(
                            player.Position,                                //当前小人位置
                            nowCube.SideLength,                             //当前Cube边长
                            nextCube.RefreshDistance,                       //下一Cube刷新距离
                            nextCube.SideLength);                           //下一Cube边长
                        break;
                    }
                case 1://蓄力状态
                    {
                        textBlock.Text = "State 1";
                        break;
                    }
                case 2://判断状态
                    {
                        textBlock.Text = "State 2";

                        //player.ComputeDistanceOfMove();//根据按键时间换算移动距离
                        player.distanceOfMove = player.PressTimeToDistance(dPressTime);//根据按键时间换算移动距离
                                                                                    
                        int isSafeFlag = player.JudgeSafeSatate(player.distanceOfMove);//判断移动距离与安全距离关系，并根据四种安全关系赋给标志量对应的值，以进入游戏对应的状态索引nStateIndex
                                                                                       //isSafeFlag =0表示跳完还在原方块，=2表示安全跳到了下一方块中，=1表示不安全，跳在原方块和下一方块之间。=3表示不安全，跳过了下一方块
                                                                                       //判断完成后根据状态更新StateIndex,
                        nStateIndex = isSafeFlag;
                        break;
                    }              
                case 30://跳在原Cube的情况
                    {
                        textBlock.Text = "State 30";
                        player.Position = player.Position + player.distanceOfMove;//更新当前person位置
                        nStateIndex = 0;
                        break;
                    }
                case 31://失败，跳在两个Cube之间的情况
                    {
                        textBlock.Text = "State 31 失败，跳在两个Cube之间的情况";
                        bGameOver = true;
                        break;
                    }
                case 32://成功跳到下一Cube的情况
                    {
                        textBlock.Text = "State 32";                       
                        nowCube = nextCube;//此时跳到的Cube对应跳之前的NextCube，因此现在的NowCube就是刚才的NextCube，将NextCube赋值给NowCube
                        nowCube.Position = 0;//重置坐标系，nowCube位置定为坐标原点，下面的位置都是在重置了之后的坐标系下建立的

                        //更新当前person位置,因为之前的player.Position就是在以NowCube中心为坐标原点的坐标系中建立的，所以上面重置了坐标系后，player.Position不会改变
                        player.Position = player.distanceOfMove - (nextCube.RefreshDistance - player.Position);

                        nextCube = new Cube();//更新新的NextCube，有三个主要参数。
                        nextCube.RefreshDistance = 200;
                        nextCube.SideLength = 94;
                        nextCube.Position = nextCube.RefreshDistance;

                        nStateIndex = 0;//重新开始新的游戏循环
                        break;
                    }
                case 33://失败，跳过了下一个Cube的情况
                    {
                        textBlock.Text = "State 33 失败，跳过了下一个Cube的情况";
                        bGameOver = true;
                        break;
                    }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (nStateIndex != 0) return;  //简化版本，强制只在空闲时(0状态)才可以蓄力

            if (e.Key == Key.Space)
            {
                if (e.IsRepeat) return;

                PressStartTime = DateTime.Now;//记录按键按下的时间

                nStateIndex = 1;//第一次按键时，状态就已变为1
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (nStateIndex != 1) return;  //简化版本，强制只在蓄力状态时才可以停止蓄力。
            if (e.Key == Key.Space)
            {
                dPressTime = DateTime.Now.Subtract(PressStartTime).TotalMilliseconds;//此行代码计算按键时长
                

                nStateIndex = 2;
            }
        }

        #endregion

        //******************************* Drwa *******************************//
        #region Draw

        public void DrawDataGridTestInitialize()//初始化测试用的DataGrid
        {           
            //Person属性DataGrid内容
            Grid1.Children.Add(DataGrid0);
            DataGrid0.HorizontalAlignment = HorizontalAlignment.Left;
            DataGrid0.VerticalAlignment = VerticalAlignment.Top;
            DataGrid0.Margin = new Thickness(60, 20, 0, 0);
            DataGrid0.IsReadOnly = true;
            PlayerData.Columns.Add("Person属性");
            PlayerData.Columns.Add("Value");
            PlayerData.Rows.Add("移动距离");
            PlayerData.Rows.Add("当前位置");
            DataGrid0.ItemsSource = PlayerData.DefaultView;

            //当前Cube属性DataGrid内容
            Grid1.Children.Add(DataGrid1);
            DataGrid1.HorizontalAlignment = HorizontalAlignment.Left;
            DataGrid1.VerticalAlignment = VerticalAlignment.Top;
            DataGrid1.Margin = new Thickness(60, 150, 0, 0);
            DataGrid1.IsReadOnly = true;
            NowCubeData.Columns.Add("当前Cube属性");
            NowCubeData.Columns.Add("Value");
            NowCubeData.Rows.Add("当前位置");
            NowCubeData.Rows.Add("边长");
            DataGrid1.ItemsSource = NowCubeData.DefaultView;

            //判定距离DataGrid内容
            Grid1.Children.Add(DataGrid2);
            DataGrid2.HorizontalAlignment = HorizontalAlignment.Left;
            DataGrid2.VerticalAlignment = VerticalAlignment.Top;
            DataGrid2.Margin = new Thickness(260, 20, 0, 0);
            DataGrid2.IsReadOnly = true;
            JudgeData.Columns.Add("判定");
            JudgeData.Columns.Add("范围");
            JudgeData.Rows.Add("在原方块");
            JudgeData.Rows.Add("失败");
            JudgeData.Rows.Add("在下一方块");
            JudgeData.Rows.Add("失败");
            DataGrid2.ItemsSource = JudgeData.DefaultView;

            //下一个Cube属性DataGrid内容
            Grid1.Children.Add(DataGrid3);
            DataGrid3.HorizontalAlignment = HorizontalAlignment.Left;
            DataGrid3.VerticalAlignment = VerticalAlignment.Top;
            DataGrid3.Margin = new Thickness(260, 150, 0, 0);
            DataGrid3.IsReadOnly = true;
            NextCubeData.Columns.Add("下一Cube属性");
            NextCubeData.Columns.Add("Value");
            NextCubeData.Rows.Add("当前位置");
            NextCubeData.Rows.Add("边长");
            DataGrid3.ItemsSource = NextCubeData.DefaultView;
        }

        public void ShowFrameTotal()//显示左上角累计帧数
        {
            labelFrame.Content = FrameTotal.ToString();
            FrameTotal++;
        }

        public void newpage()//游戏结束时，生成的Button
        {
            if (bGameOver==true)
            {
                Button ReStartButton = new Button();
                ReStartButton.Click += ReStartButton_Click;
                
                ReStartButton.Content = "Restart";
                ReStartButton.Width = 75;
                ReStartButton.Height = 20;
                ReStartButton.HorizontalAlignment = HorizontalAlignment.Right;
                ReStartButton.VerticalAlignment = VerticalAlignment.Top;
                ReStartButton.Margin = new Thickness(0, 380, 10, 0);

                Grid1.Children.Add(ReStartButton);

                Button CloseButton = new Button();

                CloseButton.Content = "Close";
                CloseButton.Width = 75;
                CloseButton.Height = 20;
                CloseButton.HorizontalAlignment = HorizontalAlignment.Right;
                CloseButton.VerticalAlignment = VerticalAlignment.Top;
                CloseButton.Margin = new Thickness(0, 380, 95, 0);

                Grid1.Children.Add(CloseButton);
            }
        }

        public void DataGridTest1()
        {
            //改变Datatable里的Person属性，然后更新到DataGrid
            //  PlayerData.Rows[0][1] = player.distanceOfMove;           
            PlayerData.Rows[0][1] = Math.Round( showRealTimeStep,2);//实时更新
            PlayerData.Rows[1][1] = Math.Round(player.Position,2);
            DataGrid0.ItemsSource = PlayerData.DefaultView;

            //改变Datatable里的当前Cube属性，然后更新到DataGrid
            NowCubeData.Rows[0][1] = Math.Round(nowCube.Position,2);
            NowCubeData.Rows[1][1] = Math.Round(nowCube.SideLength,2);
            DataGrid1.ItemsSource = NowCubeData.DefaultView;

            //改变Datatable里的判定距离属性，然后更新到DataGrid
            JudgeData.Rows[0][1] = " < "+ Math.Round(player.FirstSafeDistance,2).ToString();
            JudgeData.Rows[1][1] = Math.Round(player.FirstSafeDistance,2).ToString() + " < " + "   < " + Math.Round(player.SecondSafeDistance,2).ToString();
            JudgeData.Rows[2][1] = Math.Round(player.SecondSafeDistance,2).ToString() +" < "+ "   < "+ Math.Round(player.ThirdSafeDistance,2).ToString();
            JudgeData.Rows[3][1] = " > " + Math.Round(player.ThirdSafeDistance,2).ToString();
            DataGrid2.ItemsSource = JudgeData.DefaultView;

            //改变Datatable里的下一个Cube属性属性，然后更新到DataGrid
            NextCubeData.Rows[0][1] = Math.Round(nextCube.Position,2);
            NextCubeData.Rows[1][1] = Math.Round(nextCube.SideLength,2);
            DataGrid3.ItemsSource = NextCubeData.DefaultView;
        }
        public void TextTestData()
        {
            if (nStateIndex == 1) { RealTimePressTime = DateTime.Now.Subtract(PressStartTime).TotalMilliseconds; }
            showRealTimeStep = player.PressTimeToDistance(RealTimePressTime);
            textBlock1.Text = "RealTimePressTime = " + RealTimePressTime.ToString();
            textBlock3.Text = "PressTime = " + dPressTime.ToString();
            textBlock4.Text = "RealTimeStep = " + showRealTimeStep.ToString();
        }

        public void RunDraw()
        {
            switch (nStateIndex)
            {
                case 0://空闲状态，等待按压
                    {
                        
                        break;
                    }
                case 1://蓄力状态
                    {
                        
                        break;
                    }
                case 2://判断状态
                    {
                        
                        break;
                    }
                case 30://跳在原Cube的情况
                    {
                        DrawSafeShort(player.distanceOfMove);
                        //nStateIndex = 0;
                        break;
                    }
                case 31://失败，跳在两个Cube之间的情况
                    {
                   
                       
                        break;
                    }
                case 32://成功跳到下一Cube的情况
                    {
                        DrawSafeLong(player.distanceOfMove);
                        // nStateIndex = 0;//重新开始新的游戏循环
                        break;
                    }
                case 33://失败，跳过了下一个Cube的情况
                    {
                        
                        break;
                    }
            }
        }

        public void DrawSafeShort(double distanceofmove)
        {

            StepPerFrame = (Math.Sqrt(3.0) * distanceofmove / 2) / 40;
           
            if (iFrames1++ < 40)
            {
                DrawPlayer.SetValue(Canvas.LeftProperty, 100 + StepPerFrame * sShort1++);
                DrawPlayer.SetValue(Canvas.TopProperty, (600 - (Math.Sqrt(3.0) / 3) * StepPerFrame * sShort2++));
            }


        }

        public void DrawSafeLong(double distanceofmove)
        {
            
            StepPerFrame = (Math.Sqrt(3.0)* distanceofmove / 2) / 120;          
            if (iFrames2++ < 120)
            {
                DrawPlayer.SetValue(Canvas.LeftProperty, 100 + (StepPerFrame * sLong1++));
                DrawPlayer.SetValue(Canvas.TopProperty, (600 - (Math.Sqrt(3.0) / 3) * (StepPerFrame * sLong2++)));
            }

        }
        private void ReStartButton_Click(object sender, RoutedEventArgs e)
        {
            //重置游戏
            MessageBox.Show("al");//此行测试
            //PersonCubeInitialize();
            //DrawDataGridTestInitialize();

            //CompositionTarget.Rendering += CompositionTarget_Rendering;//主窗口加载完立即进入游戏，简化省略了开始游戏选择窗口。
        }

        #endregion
    }
}
