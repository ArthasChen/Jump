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
        public Cube Cube0 = new Cube();
        public Cube Cube1 = new Cube();

        public Cube NowCube = new Cube();
        public Cube NextCube = new Cube();

        //第几关，跳跃了几个Cube
        public int CountOfLevel = 0;

        //Draw

        //第一版时候测试用的，现在暂时不用，以后测试时再修改
        public DataGrid DataGrid0 = new DataGrid();
        public DataGrid DataGrid1 = new DataGrid();
        public DataGrid DataGrid2 = new DataGrid();
        public DataGrid DataGrid3 = new DataGrid();
        public DataTable PlayerData = new DataTable();
        public DataTable NowCubeData = new DataTable();
        public DataTable JudgeData = new DataTable();
        public DataTable NextCubeData = new DataTable();


        public bool RunDrawState130EndFlag = false;
        public bool RunDrawState131EndFlag = false;
        public bool RunDrawState132EndFlag = false;
        public bool RunDrawState133EndFlag = false;
        public bool RunDrawState1320EndFlag = false;
        public bool RunDrawStateNegative11EndFlag=false;

        //游戏完善后可以删掉，计算播放跳跃成功的动画，没什么意义
        public int CountOfAnimationFrame = 0;
       
        //设定短动画的绘图帧数，以多少帧来运行短动画
        public int shortAnimationFrames = 15;//Ver1=30
        //短动画已绘画帧数计数器
        public int shortAnimationFrameCounts = 0;
        //设定长动画的绘图帧数，以多少帧来运行长动画
        public int longAnimationFrames = 30;//Ver1=60
        //长动画已绘画帧数计数器
        public int longAnimationFrameCounts = 0;
        //设定生成新cube以及新cube的移动动画的帧数
        public int CreatNewCubeFrames = 24;
        //设定生成新cube以及新cube的移动动画的帧数计数器
        public int CreatNewCubeFrameCounts = 0;
        //设定player下落动画总帧数
        public int playerFallAnimationFrames = 24;
        //设定player下落动画计数器
        public int playerFallAnimationFramesCounts = 0;



        //压缩的X Y 坐标
        public double scaleX=1;
        public double scaleY=1;
        //小人的头每帧的下降值
        public double HeadMoveDownDy;
        //压缩计时器
        public int PersonCompressionCount = 0;
        public int CubeCompressionCount = 0;
   
        //cube压缩时每一帧的压缩Y值
        public double dyOfcCubeCompression = 1;
        public double YofCubeCompressionMoveDown;

        //cube回复动画帧数计数器
        public double CubeRestoreCounts = 0;
        //cube回复动画重复标志
        public bool RepeatFlag = false;

        //绘制动画的cube控件类
        UCcubeCompression UCcubecom0 = new UCcubeCompression();
        UCcubeCompression UCcubecom1 = new UCcubeCompression();

        UCcubeCompression UcNowCubeCom = new UCcubeCompression();
        UCcubeCompression UcNextCubeCom = new UCcubeCompression();

        //Globl

        //屏幕左上角的帧数计数器
        public int FrameTotal = 0;
        public bool bGameOver = false;

        //游戏状态索引
        public int nStateIndex = -1;
        //绘画状态索引
        public int drawStateIndex = 10;
        //镜头状态索引
        public int backgroundMoveIndex = 0;
        //Cube下落弹性反弹动画状态索引
        public int cubeFallIndex = 0;

        public DateTime PressStartTime = DateTime.MinValue;
        public float dPressTime = 0;
        public float RealTimePressTime = 0;
        public float showRealTimeStep = 0;

        public Random rn = new Random();

        //背景移动时候，记录的上一次位置
        private double LastLeftValue;
        private double LastTopValue;
        //动画结束后，镜头移动，采用多少帧来完成移动
        public int CameraMoveFrames = 24;
        //镜头移动计数器
        public int icamera = 0;


        public MainWindow()
        {
            InitializeComponent();
            LogicPersonCubeInitialize();
            DrawAnimationInitialize();
            //DrawDataGridTestInitialize();//Datagrid控件，用来显示各种状态，暂时不删，后面测试需要时可以用

            CompositionTarget.Rendering += CompositionTarget_Rendering;//主窗口加载完立即进入游戏，简化省略了开始游戏选择窗口。
        }


        //******************************* MainLoop *******************************//
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Logic();
            isGameContinue();
            if (bGameOver)  return;
            Draw();
            CameraPosition();
        }

        public void Logic()
        {
            RunLogic();
        }

        public void Draw()
        {
            ShowFrameTotal();//显示左上角累计帧数       
            //DataGridTest1();//游戏参数更新显示
            TextTestData();//按压时间显示

            RunDraw();
            RunCubeFall();
        }
        public void isGameContinue()
        {
            if (!bGameOver) return;
            GameOverInterface();//当游戏结束时，生成结束页面
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }



        //******************************* GameLogic *******************************//
        #region GameLogic

        //初始化小人和2个Cube,并赋给2个cube详细的参数，因为在Cube类中这些参数是根据前一个cube的属性计算得出的，而这两个cube就是最初的两个，所以在这里直接初始化这些需要计算的参数
        public void LogicPersonCubeInitialize()
        {
            player.Position = new float[2] { 100f, 300f };
            //初始化跳跃距离中心的高度
            player.dhight = 160;

            Cube0.Position = new float[2] { 100f, 300f };
            Cube0.SideLength = 100f;//边长单位为float，以后优化的时候看能不能改为整数

            //下面的这一段，将来是跳跃成功后，应该自动改变的
            Cube1.RefreshDirection = 1;//1表示生成的cube在上一cube右上角
            Cube1.RefreshDistance = 200;
            Cube1.SideLength = 100f;//
            Cube1.Position = Cube1.ComputCubePositon(Cube1.RefreshDirection, Cube1.RefreshDistance, Cube0.Position);
            player.MoveDirection = Cube1.RefreshDirection;

            //初始化第一个cube的范围。为了测试。
            Cube0.RefreshDirection = 1;
            Cube0.k = 1;
            Cube0.b = 92.265f;
            Cube0.SideLength = 100f;
            Cube0.MinX = (float)(100 - Cube0.SideLength * Math.Cos(Math.PI / 6));
            Cube0.MidX = Cube0.Position[0];
            Cube0.MaxX = (float)(100 + Cube0.SideLength * Math.Cos(Math.PI / 6));

            NowCube = Cube0;
            NextCube = Cube1;

            //用于移动背景
            UpdataBackgroundLastPosition();//用于canmera
        }

        //游戏逻辑
        public void RunLogic()
        {
            switch (nStateIndex)
            {
                case -1://小人下落状态,只在游戏开始时运行一次,但是需要运行好几帧，此后循环只从nStateIndex=0开始
                    {
                        drawStateIndex = -11;
                        //当最后一帧运行动画时，进入if，因此在倒数第二针帧的动画结束时，RunDrawStateNegative11EndFlag赋值true，以至于最后一帧时进入此判断
                        if (RunDrawStateNegative11EndFlag)
                        {
                            RunDrawStateNegative11EndFlag = false;
                            nStateIndex = 0;
                        }
                        break;
                    }
                case 0://空闲状态，等待按压
                    {
                        textBlockbb.Text = "State 0 " + $"第{CountOfLevel}个方块" + " counts= " + CountOfAnimationFrame.ToString();
                        ScoreNumber.Content = CountOfLevel.ToString();

                        //计算安全区,计算新的nowcube和nextcube表面的四条函数方程
                        NowCube.ComputeParameterOfSafeZone();
                        NextCube.ComputeParameterOfSafeZone();
                        player.MoveDirection = NextCube.RefreshDirection;
                        drawStateIndex = 10;
                        break;
                    }
                case 1://蓄力状态
                    {
                        textBlockbb.Text = "State 1";
                        drawStateIndex = 11;
                        break;
                    }
                case 2://判断状态
                    {
                        textBlockbb.Text = "State 2";                      

                        //根据按键时间换算移动距离
                        player.distanceOfMove = player.PressTimeToDistance(dPressTime);

                        //计算出小人跳跃后落点的坐标。
                        player.FallOffPosition = player.ComputeFallOutPositon(player.distanceOfMove, player.Position, NextCube.Position, NextCube.RefreshDirection);

                        //计算小人跳跃函数的参数
                        player.parameterXYofJump(player.Position, NextCube.Position);

                        int isSafeFlag = player.JudgeSafeSatate(player.FallOffPosition, NowCube, NextCube);
                        nStateIndex = isSafeFlag;
                        drawStateIndex = 12;
                        break;
                    }
                case 30://跳在原Cube的情况
                    {
                        textBlockbb.Text = "State 30";
                        drawStateIndex = 130;

                        //只有当动画结束才执行的操作
                        if (RunDrawState130EndFlag)
                        {
                            //当RunDrawState130EndFlag=true进入此后，立刻置false,不然动画只能执行一次。
                            RunDrawState130EndFlag = false;

                            //当动画结束后，更新person现在的坐标，因为person跳跃的动画需要用到起跳的位置，所以只能在跳跃完成后更新person现在的坐标
                            player.Position = player.FallOffPosition;
                            //当动画结束后，进入0状态等待命令
                            nStateIndex = 0;
                            drawStateIndex = 10;
                        }
                        break;
                    }
                case 31://失败，跳在两个Cube之间的情况
                    {
                        textBlockbb.Text = "State 31 失败，跳在两个Cube之间的情况";
                        drawStateIndex = 131;

                        //只有当动画结束才执行的操作
                        if (RunDrawState131EndFlag)
                        {
                            //当RunDrawState130EndFlag=true进入此后，立刻置false,不然动画只能执行一次。
                            RunDrawState131EndFlag = false;

                            bGameOver = true;
                            
                            
                        }

                        break;
                    }
                case 32://成功跳到下一Cube的情况
                    {
                        textBlockbb.Text = "State 32";
                        drawStateIndex = 132;
                        CountOfAnimationFrame += 1;//第几个播放动画的帧数

                        if (RunDrawState132EndFlag)
                        {
                            //当RunDrawState130EndFlag=true进入此后，立刻置false,不然动画只能执行一次。
                            RunDrawState132EndFlag = false;

                            //当动画结束后，更新person现在的坐标，因为person跳跃的动画需要用到起跳的位置，所以只能在跳跃完成后更新person现在的坐标
                            player.Position = player.FallOffPosition;

                            //跳跃成功，之前的nextcube要就是现在的nowcube，所以要赋值一次，之前的nextcube重新初始化，当成下一个cube
                            NowCube = NextCube;

                            NextCube = new Cube();
                            NextCube.RefreshDistance = 200;//这个距离应该是随机出现的，现在暂时固定
                            NextCube.SideLength = 100;

                            Random rng = new Random();
                            int s = rng.Next(2);
                            if (s == 0) NextCube.RefreshDirection = -1;
                            else NextCube.RefreshDirection = 1;//这个方向应该是在-1和1中随机出现的，现在暂时固定


                            //计算新出现的cube的位置
                            NextCube.Position = NextCube.ComputCubePositon(NextCube.RefreshDirection, NextCube.RefreshDistance, NowCube.Position);

                            //当动画结束后，进入0状态等待命令,重新开始新的游戏循环
                            nStateIndex = 0;
                            drawStateIndex = 1320;
                            cubeFallIndex = 1;



                            //跳到了第几个方块
                            CountOfLevel++;
                        }
                        break;
                    }
                case 33://失败，跳过了下一个Cube的情况
                    {
                        textBlockbb.Text = "State 33 失败，跳过了下一个Cube的情况";
                        drawStateIndex = 133;
                        if (RunDrawState133EndFlag)
                        {
                            //当RunDrawState130EndFlag=true进入此后，立刻置false,不然动画只能执行一次。
                            RunDrawState133EndFlag = false;
                            bGameOver = true;
                        }
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
                dPressTime = (float)DateTime.Now.Subtract(PressStartTime).TotalMilliseconds;//此行代码计算按键时长


                nStateIndex = 2;
            }
        }

        #endregion

        //******************************* Drwa *******************************//
        #region Draw

        //初始化
        public void DrawAnimationInitialize()
        {        
            UcNowCubeCom = UCcubecomZero;
            UcNextCubeCom = UCcubecomOne;
        }

        //初始化测试用的DataGrid
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

        public void GameOverInterface()//当游戏结束时，生成结束页面
        {
            //游戏Page中的记分板隐藏
            ScoreNumber.Visibility = Visibility.Hidden;
            //游戏结束的页面出现
            GameOverCanvas.Visibility = Visibility.Visible;
            //将游戏分数显示在游戏结束的页面中，赋值给
            LastScoreNumber.Content = CountOfLevel;

        }

       
        public void TextTestData()
        {
            if (nStateIndex == 1) { RealTimePressTime = (float)DateTime.Now.Subtract(PressStartTime).TotalMilliseconds; }
            showRealTimeStep = player.PressTimeToDistance(RealTimePressTime);
            textBlock1.Text = "RealTimePressTime = " + RealTimePressTime.ToString();
            textBlock3.Text = "PressTime = " + dPressTime.ToString();
            textBlock4.Text = "RealTimeStep = " + showRealTimeStep.ToString();
        }
            
        //绘画状态的内容
        public void RunDraw()
        {
            switch (drawStateIndex)
            {
                case -11://小人下落状态,只在游戏开始时运行一次,但是需要运行好几帧，此后循环只从nStateIndex=0开始
                    {                       
                        //初始化时，小人下落动画
                        PersonFallAnimation();
                        break;
                    }
                case 10://空闲状态，等待按压
                    {
                        break;
                    }
                case 11://蓄力状态
                    {
                        //小人的压缩状态，小人的高度变短，身体变粗
                        CompressionAnimation();
                        //方块的压缩状态
                        CompressionOfCube();
                        //小人的位置随着方块的压缩而下降，必须在 CompressionOfCube()后面运行此方法，因为需要用到CompressionOfCube()算完的参数：方块每帧下降的dy就是小人每帧下降的dy
                        PersonMoveDownAnimationWithCubeCompression();
                        break;
                    }
                case 12://判断状态
                    {
                        //小人的尺寸恢复原状态，并且重置压缩状态改变了的参数
                        RelaxAndResetParameterOfPersonCompression();
                        //Cube尺寸恢复原状态，并且重置压缩状态改变了的参数
                      //  RelaxAndResetParameterOfCubeCompression();
                        //小人下移后恢复原来的状态，
                        RelaxAndResetParameterOfPersonMoveDown();
                        break;
                    }
                case 130://跳在原Cube的情况
                    {
                        //方块弹性形变后恢复的效果。无论小人是哪种情况，只要跳起，cube都有这个效果。
                        CubeElasticRestore();
                        // SuccessShortAnimation(player);
                        SuccessShortAnimationWithJump(player);
                        break;
                    }
                case 131://失败，跳在两个Cube之间的情况
                    {
                        //方块弹性形变后恢复的效果。无论小人是哪种情况，只要跳起，cube都有这个效果。
                        CubeElasticRestore();
                        // FailShortAnimation(player);
                        FailShortAnimationWithJump(player);
                        break;
                    }
                case 132://成功跳到下一Cube的动画
                    {
                        //方块弹性形变后恢复的效果。无论小人是哪种情况，只要跳起，cube都有这个效果。
                        CubeElasticRestore();
                        // SuccessLongAnimation(player);
                        SuccessLongAnimationWithJump(player);
                        break;
                    }
                case 1320://开始生成新的cube,在成功跳跃到下一cube后出现
                    {
                        ShowNewCube();
                        break;
                    }

                case 133://失败，跳过了下一个Cube的情况
                    {
                        //方块弹性形变后恢复的效果。无论小人是哪种情况，只要跳起，cube都有这个效果。
                        CubeElasticRestore();
                        FailLongAnimationWithJump(player);
                        break;
                    }
            }
        }

        //小人下落动画
        public void PersonFallAnimation()
        {
            double x = (1d / playerFallAnimationFrames) * (playerFallAnimationFramesCounts + 1);
            double yvalue = 250 * BounceEaseOutFucntion(x);
           // Double Y = (250d / playerFallAnimationFrames) * (playerFallAnimationFramesCounts+1);//test用的，每帧线性下落距离
            Layer_Person.SetValue(Canvas.TopProperty, 506d + yvalue);
            playerFallAnimationFramesCounts++;
            if (playerFallAnimationFramesCounts== playerFallAnimationFrames-1)
            {
                RunDrawStateNegative11EndFlag = true;
            }
            if (playerFallAnimationFramesCounts == playerFallAnimationFrames )
            {
                playerFallAnimationFramesCounts = 0;
            }
        }

        //cube出现后的下落反弹动画。本来应该在RunDraw里，但是写在里面会出现一些时间上的问题。
        /*跳一跳原版里，当小人成功跳到cube的一瞬间，出现下一cube，此时cube在高处，经过几帧的动画后，下一个cube下落反弹最终触地，与此同时，镜头开始移动，最终在下一cube稳定落地时镜头也移动结束。
        但是，当小人当小人成功跳到cube的一瞬间，就可以开始蓄力下一次跳跃了。也就是说在此时下一个cube还没落地，镜头也没完全移动结束，但是就可以开始蓄力了。为了实现这个机制，不得不把cube下落反
        弹动画独立出来，暂时想到的是这个方法，以后再优化*/
        public void RunCubeFall()
        {
            switch (cubeFallIndex)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        if (CreatNewCubeFrameCounts < CreatNewCubeFrames)
                        {
                            double x =(1d/ CreatNewCubeFrames)* (CreatNewCubeFrameCounts+1);
                            double yvalue = 250 * BounceEaseOutFucntion(x);
                            //double y = (NowCube.Position[1] + NextCube.dy + 250 - (250d / CreatNewCubeFrames) * (CreatNewCubeFrameCounts + 1));//线性下落test
                            double y = (NowCube.Position[1] + NextCube.dy + (250-yvalue));
                            UcNextCubeCom.SetValue(Canvas.BottomProperty, y);
                            CreatNewCubeFrameCounts++;

                        }
                        if (CreatNewCubeFrameCounts == CreatNewCubeFrames)
                        {
                            CreatNewCubeFrameCounts = 0;
                            cubeFallIndex = 0;
                        }
                        break;
                    }
            }
        }

        //cube下落反弹函数
        public double BounceEaseOutFucntion(double p)
        {
            if (p < 4 / 11.0)
            {
                return (121 * p * p) / 16.0;
            }
            else if (p < 8 / 11.0)
            {
                return (363 / 40.0 * p * p) - (99 / 10.0 * p) + 17 / 5.0;
            }
            else if (p < 9 / 10.0)
            {
                return (4356 / 361.0 * p * p) - (35442 / 1805.0 * p) + 16061 / 1805.0;
            }
            else
            {
                return (54 / 5.0 * p * p) - (513 / 25.0 * p) + 268 / 25.0;
            }
        }

        //小人的压缩动画 和 跟随压缩的方块移动的动画
        //这个方法里，有3个固定量暂时未设置成参数，分别是完成全部压缩动画的帧数90，小人身体每次沿x、y方向改变的0.006，0.004值，还有19.68376558
        public void CompressionAnimation()
        {
            if (PersonCompressionCount >= 90) return;

            ScaleTransform compression = new ScaleTransform();
            TranslateTransform movedown = new TranslateTransform();
            //小人宽度每次压缩的倍数，小人长度每次压缩的倍数
            //压缩时，每次在Y方向减少0.004倍，也就是每次压缩都在初始的基础上*（1-n*（0.004））
            scaleX += 0.006;
            scaleY -= 0.004;

            //将压缩的x和y值赋给缩放变换的属性
            compression.ScaleX = scaleX;
            compression.ScaleY = scaleY;

            //小人的头每帧向下移动的距离,这里的19.68376558是由小人压缩减少的总长度计算得出的，小人压缩减少的高度，就是头部下降的高度，处于总共压缩过程的帧数，就是每一帧下降的高度
            double dyPerFrame = 19.68376558 / 90;
            HeadMoveDownDy = dyPerFrame * PersonCompressionCount;

            movedown.Y = HeadMoveDownDy;

            UCplayer.head.RenderTransform = movedown;
            UCplayer.body.RenderTransform = compression;

            PersonCompressionCount++;        
        }

        //重置小人压缩后的恢复动画，重置压缩参数
        public void RelaxAndResetParameterOfPersonCompression()
        {
            PersonCompressionCount = 0;

            ScaleTransform compression = new ScaleTransform();
            TranslateTransform movedown = new TranslateTransform();

            scaleX = 1;
            scaleY = 1;


            compression.ScaleX = scaleX;
            compression.ScaleY = scaleY;

            HeadMoveDownDy = 0;
            movedown.Y = HeadMoveDownDy;

            UCplayer.head.RenderTransform = movedown;
            UCplayer.body.RenderTransform = compression;


        }


        //小人跟随cube压缩往下移动
        public void PersonMoveDownAnimationWithCubeCompression()
        {
            if (CubeCompressionCount >= 90) return;

            Layer_Person.SetValue(Canvas.LeftProperty, 0d);
            Layer_Person.SetValue(Canvas.TopProperty, 756d + YofCubeCompressionMoveDown);
        }

        //重置小人跟随cube压缩往下移动的参数
        public void RelaxAndResetParameterOfPersonMoveDown()
        {
            Layer_Person.SetValue(Canvas.LeftProperty, 0d);
            Layer_Person.SetValue(Canvas.TopProperty, 756d );
        }


        //cube的压缩动画
        public void CompressionOfCube()
        {
            //这个标志指示着cube复原时候到达某一帧数后做什么事情
            RepeatFlag = false;

            if (CubeCompressionCount >= 90) return;

            //在代码中初始化cube的transform参数
            Point LeftBottomPoint = new Point(0.5, 1);
            Point RightBottomPoint = new Point(0, 1);
            UcNowCubeCom.leftbottom.RenderTransformOrigin = LeftBottomPoint;
            UcNowCubeCom.rightbottom.RenderTransformOrigin = RightBottomPoint;

            //配置左侧的方块变形
            SkewTransform LeftSkew = new SkewTransform();
            LeftSkew.AngleY = 30;
            TranslateTransform lefttranslat = new TranslateTransform();
            lefttranslat.Y = 25.1147;

            //配置右侧的方块变形
            SkewTransform RightSkew = new SkewTransform();
            RightSkew.AngleY = -30;
            TranslateTransform righttranslat = new TranslateTransform();
            righttranslat.Y = 99.5929;

            //配置上面的菱形每帧下移的位移dy
            

            if (CubeCompressionCount<90)
            {

                ScaleTransform compressionOfCube = new ScaleTransform();
                dyOfcCubeCompression -= 0.0055;
                compressionOfCube.ScaleY = dyOfcCubeCompression;

                //配置左侧的方块变形
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(compressionOfCube);
                transformGroup.Children.Add(LeftSkew);
                transformGroup.Children.Add(lefttranslat);

                UcNowCubeCom.leftbottom.RenderTransform = transformGroup;

                //配置右侧的方块变形
                var transformGroupRight = new TransformGroup();
                transformGroupRight.Children.Add(compressionOfCube);
                transformGroupRight.Children.Add(RightSkew);
                transformGroupRight.Children.Add(righttranslat);

                UcNowCubeCom.rightbottom.RenderTransform = transformGroupRight;

                //配置上面的菱形每帧下移的位移dy，50是侧边平行四边形的高，用高减去压缩后最低点的高，然后除以90帧
                double dy = (50 - 50 * (1 - 90 * 0.0055)) / 90;

                YofCubeCompressionMoveDown = (dy * CubeCompressionCount)+1;

                TranslateTransform moveTop = new TranslateTransform();
                moveTop.Y = YofCubeCompressionMoveDown;

                UcNowCubeCom.topface.RenderTransform = moveTop;

                CubeCompressionCount++;
            }
        }

        //重置cube的压缩动画,最初的版本使用，候来来被下面的CubeElasticRestore方法取代。
        public void RelaxAndResetParameterOfCubeCompression()
        {
            CubeCompressionCount = 0;

            //在代码中初始化cube的transform参数
            Point LeftBottomPoint = new Point(0.5, 1);
            Point RightBottomPoint = new Point(0, 1);
            UcNowCubeCom.leftbottom.RenderTransformOrigin = LeftBottomPoint;
            UcNowCubeCom.rightbottom.RenderTransformOrigin = RightBottomPoint;

            //
            ScaleTransform compressionOfCube = new ScaleTransform();
            dyOfcCubeCompression =1;
            compressionOfCube.ScaleY = dyOfcCubeCompression;

            //配置左侧的方块变形
            SkewTransform LeftSkew = new SkewTransform();
            LeftSkew.AngleY = 30;
            TranslateTransform lefttranslat = new TranslateTransform();
            lefttranslat.Y = 25.1147;

            //配置右侧的方块变形
            SkewTransform RightSkew = new SkewTransform();
            RightSkew.AngleY = -30;
            TranslateTransform righttranslat = new TranslateTransform();
            righttranslat.Y = 99.5929;

            //配置左侧的方块变形
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(compressionOfCube);
            transformGroup.Children.Add(LeftSkew);
            transformGroup.Children.Add(lefttranslat);

            UcNowCubeCom.leftbottom.RenderTransform = transformGroup;

            //配置右侧的方块变形
            var transformGroupRight = new TransformGroup();
            transformGroupRight.Children.Add(compressionOfCube);
            transformGroupRight.Children.Add(RightSkew);
            transformGroupRight.Children.Add(righttranslat);

            UcNowCubeCom.rightbottom.RenderTransform = transformGroupRight;

            //配置上面的菱形每帧下移的位移dy
            double dy = 0;
            YofCubeCompressionMoveDown = dy * CubeCompressionCount;

            TranslateTransform moveTop = new TranslateTransform();
            moveTop.Y = YofCubeCompressionMoveDown;

            UcNowCubeCom.topface.RenderTransform = moveTop;
        }


        //方块弹性形变后恢复的效果。无论小人是哪种情况，只要跳起，cube都有这个效果。
        //定为15帧内完成。短动画为15帧，刚好在它之内可以完成，长动画30帧，存在重复的现象，加入了标志位避免重复
        public void CubeElasticRestore()
        {
            //这个标志指示着cube复原时候超过15帧就不进行下面的操作，因为下面的操作本来就在15帧运行OK，只不过有的动画需要30帧来演示。假如不设置这个判断，又开始cube还原了
            if (RepeatFlag) return;
            CubeCompressionCount = 0;

            //在代码中初始化cube的transform参数
            Point LeftBottomPoint = new Point(0.5, 1);
            Point RightBottomPoint = new Point(0, 1);
            UcNowCubeCom.leftbottom.RenderTransformOrigin = LeftBottomPoint;
            UcNowCubeCom.rightbottom.RenderTransformOrigin = RightBottomPoint;

           
            if (CubeRestoreCounts < 15)
            {
                //配置上面的菱形每帧弹性还原的位移dy,15帧内运行完弹性动画
                double t = (1d / 15) * CubeRestoreCounts++;
                TranslateTransform moveTop = new TranslateTransform();
                moveTop.Y = 25*(1- ElasticFucntion(t));
                UcNowCubeCom.topface.RenderTransform = moveTop;

                //配置两侧方块每帧弹性还原的百分比
                ScaleTransform compressionOfCube = new ScaleTransform();
                dyOfcCubeCompression = (50- 25 * (1 - ElasticFucntion(t))) /50;
                compressionOfCube.ScaleY = dyOfcCubeCompression;

                //配置左侧的方块还原
                SkewTransform LeftSkew = new SkewTransform();
                LeftSkew.AngleY = 30;
                TranslateTransform lefttranslat = new TranslateTransform();
                lefttranslat.Y = 25.1147;

                //配置右侧的方块还原
                SkewTransform RightSkew = new SkewTransform();
                RightSkew.AngleY = -30;
                TranslateTransform righttranslat = new TranslateTransform();
                righttranslat.Y = 99.5929;

                //配置左侧的方块还原
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(compressionOfCube);
                transformGroup.Children.Add(LeftSkew);
                transformGroup.Children.Add(lefttranslat);

                UcNowCubeCom.leftbottom.RenderTransform = transformGroup;

                //配置右侧的方块还原
                var transformGroupRight = new TransformGroup();
                transformGroupRight.Children.Add(compressionOfCube);
                transformGroupRight.Children.Add(RightSkew);
                transformGroupRight.Children.Add(righttranslat);

                UcNowCubeCom.rightbottom.RenderTransform = transformGroupRight;

            }
            //还原动画的最后一帧，将CubeRestoreCounts重置为0。为了避免长动画时继续进入上面的循环，加了一个标志位，RepeatFlag
            if (CubeRestoreCounts == 14)
            {
                CubeRestoreCounts = 0;
                RepeatFlag = true;
            }         
        }

        public double ElasticFucntion(double t)
        {
            double yVlaue;
            //y = sin(-13pi/2*(x + 1))*pow(2, -10x) + 1
            //yVlaue = (((-1d) / 2) * Math.Pow(Math.E, ((-1) * 6 * t))) * ((-1) * 2 * Math.Pow(Math.E, 6 * t) + Math.Sin(12 * t) + 2 * Math.Cos(12 * t)); //可以用.cs给的
            yVlaue = Math.Sin((-13 * (Math.PI) / 2) * (t + 1)) * Math.Pow(2, -10 * t) + 1;
            return yVlaue;
        }
        #region 两个短动画，一个成功一个失败，共用一套 帧数计数器 和 总帧数，因为两个动画同时只执行一个，执行完了重置，因此暂时觉得不会冲突
        //跳跃成功的短动画，跳在自己方块上的动画
        public void SuccessShortAnimation(Person player)
        {
            double dxPerFrame, dyPerFrame;
            //每一帧在x方向上的移动，每一帧在y方向上的移动
            dxPerFrame = (double)(player.dx * player.MoveDirection / shortAnimationFrames);
            dyPerFrame = (double)(player.dy / shortAnimationFrames);

            //当短动画帧计数器小于给定的短动画帧数，绘画，当计数器等于给定的短动画帧数，结束动画，游戏逻辑进入下一状态
            if (shortAnimationFrameCounts < shortAnimationFrames)
            {
                UCplayer.SetValue(Canvas.LeftProperty, player.Position[0] + dxPerFrame * (shortAnimationFrameCounts + 1));
                UCplayer.SetValue(Canvas.BottomProperty, player.Position[1] + dyPerFrame * (shortAnimationFrameCounts + 1));

                shortAnimationFrameCounts++;
                if (shortAnimationFrameCounts == shortAnimationFrames)//这个的意思是当执行完stateindex=30的动画后，才执行索引
                {
                    shortAnimationFrameCounts = 0;
                    RunDrawState130EndFlag = true;
                    // drawStateIndex = 10;
                }
            }
        }

        public void SuccessShortAnimationWithJump(Person player)
        {
            double dxPerFrame;
            float floatdxPerFrame;

            //每一帧在x方向上的移动，每一帧在y方向上的移动
            dxPerFrame = (double)(player.dx * player.MoveDirection / shortAnimationFrames);
            floatdxPerFrame = player.dx * player.MoveDirection / shortAnimationFrames;

            //当短动画帧计数器小于给定的短动画帧数，绘画，当计数器等于给定的短动画帧数，结束动画，游戏逻辑进入下一状态
            if (shortAnimationFrameCounts < shortAnimationFrames)
            {
                double x = player.Position[0] + dxPerFrame * (shortAnimationFrameCounts + 1);
                float fx=player.Position[0] + floatdxPerFrame * (shortAnimationFrameCounts + 1);
                double y = (double)player.XYofJump(fx);
                UCplayer.SetValue(Canvas.LeftProperty, x);
                UCplayer.SetValue(Canvas.BottomProperty, y);

                shortAnimationFrameCounts++;
                if (shortAnimationFrameCounts == shortAnimationFrames)//这个的意思是当执行完stateindex=30的动画后，才执行索引
                {
                    shortAnimationFrameCounts = 0;
                    RunDrawState130EndFlag = true;
                    // drawStateIndex = 10;
                }
            }
        }

        //跳跃失败的动画，跳在了两方块之间
        public void FailShortAnimation(Person player)
        {
            double dxPerFrame, dyPerFrame;
            //每一帧在x方向上的移动，每一帧在y方向上的移动
            dxPerFrame = (double)(player.dx * player.MoveDirection / shortAnimationFrames);
            dyPerFrame = (double)(player.dy / shortAnimationFrames);

            //当短动画帧计数器小于给定的短动画帧数，绘画，当计数器等于给定的短动画帧数，结束动画，游戏逻辑进入下一状态
            if (shortAnimationFrameCounts < shortAnimationFrames)
            {
                UCplayer.SetValue(Canvas.LeftProperty, player.Position[0] + dxPerFrame * (shortAnimationFrameCounts + 1));
                UCplayer.SetValue(Canvas.BottomProperty, player.Position[1] + dyPerFrame * (shortAnimationFrameCounts + 1));

                shortAnimationFrameCounts++;
                if (shortAnimationFrameCounts == shortAnimationFrames)
                {
                    shortAnimationFrameCounts = 0;
                    RunDrawState131EndFlag = true;
                }
            }

        }

        public void FailShortAnimationWithJump(Person player)
        {
            double dxPerFrame;
            float floatdxPerFrame;

            //每一帧在x方向上的移动，每一帧在y方向上的移动
            dxPerFrame = (double)(player.dx * player.MoveDirection / shortAnimationFrames);
            floatdxPerFrame = player.dx * player.MoveDirection / shortAnimationFrames;

            //当短动画帧计数器小于给定的短动画帧数，绘画，当计数器等于给定的短动画帧数，结束动画，游戏逻辑进入下一状态
            if (shortAnimationFrameCounts < shortAnimationFrames)
            {
                double x = player.Position[0] + dxPerFrame * (shortAnimationFrameCounts + 1);
                float fx = player.Position[0] + floatdxPerFrame * (shortAnimationFrameCounts + 1);
                double y = (double)player.XYofJump(fx);
                UCplayer.SetValue(Canvas.LeftProperty, x);
                UCplayer.SetValue(Canvas.BottomProperty, y);

                shortAnimationFrameCounts++;
                if (shortAnimationFrameCounts == shortAnimationFrames)//这个的意思是当执行完stateindex=30的动画后，才执行索引
                {
                    shortAnimationFrameCounts = 0;
                    RunDrawState131EndFlag = true;
                    // drawStateIndex = 10;
                }
            }
        }
        #endregion

        #region 两个长动画，一个成功一个失败，共用一套 帧数计数器 和 总帧数，因为两个动画同时只执行一个，执行完了重置，因此暂时觉得不会冲突
        public void SuccessLongAnimation(Person player)
        {
            double dxPerFrame, dyPerFrame;
            //每一帧在x方向上的移动，每一帧在y方向上的移动
            dxPerFrame = (double)(player.dx * player.MoveDirection / longAnimationFrames);
            dyPerFrame = (double)(player.dy / longAnimationFrames);

            if (longAnimationFrameCounts < longAnimationFrames)
            {
                UCplayer.SetValue(Canvas.LeftProperty, player.Position[0] + dxPerFrame * (longAnimationFrameCounts + 1));
                UCplayer.SetValue(Canvas.BottomProperty, player.Position[1] + dyPerFrame * (longAnimationFrameCounts + 1));

                longAnimationFrameCounts++;
                if (longAnimationFrameCounts == longAnimationFrames)
                {
                    longAnimationFrameCounts = 0;
                    RunDrawState132EndFlag = true;
                }
            }
        }

        public void SuccessLongAnimationWithJump(Person player)
        {
            double dxPerFrame;
            float floatdxPerFrame;
            //每一帧在x方向上的移动的相对位移，相对值，有正负，正为跳跃方向为1，x坐标增加，负为跳跃方向为-1，x坐标减少
            dxPerFrame = (double)(player.dx * player.MoveDirection / longAnimationFrames);
            //用来计算跳跃时每一帧在x方向上的相对位移，相对值，值与上一个一样，不过采用float型表示，用来带入方程
            floatdxPerFrame = player.dx * player.MoveDirection / longAnimationFrames; ;

            if (longAnimationFrameCounts < longAnimationFrames)
            {
                //在跳跃工过程中，每一帧的x坐标，这是个绝对坐标中的坐标值,两个是一样的值，一个是double，一个是float，float是要带入一元二次方程的
                double x = player.Position[0] + dxPerFrame * (longAnimationFrameCounts + 1);
                float fx= player.Position[0] + floatdxPerFrame * (longAnimationFrameCounts + 1);
                //在跳跃工过程中，每一帧的y坐标，这是个绝对坐标中的坐标值，把x坐标带入一元二次方程中得出的y
                double y = (double)player.XYofJump(fx);

                UCplayer.SetValue(Canvas.LeftProperty, x);//这里设置canvas的值是double型的，x是double型的
                UCplayer.SetValue(Canvas.BottomProperty, y);

                longAnimationFrameCounts++;
                if (longAnimationFrameCounts == longAnimationFrames)
                {
                    longAnimationFrameCounts = 0;
                    RunDrawState132EndFlag = true;
                }
            }
        }

        public void FailLongAnimation(Person player)
        {
            double dxPerFrame, dyPerFrame;
            //每一帧在x方向上的移动，每一帧在y方向上的移动
            dxPerFrame = (double)(player.dx * player.MoveDirection / longAnimationFrames);
            dyPerFrame = (double)(player.dy / longAnimationFrames);

            if (longAnimationFrameCounts < longAnimationFrames)
            {
                UCplayer.SetValue(Canvas.LeftProperty, player.Position[0] + dxPerFrame * (longAnimationFrameCounts + 1));
                UCplayer.SetValue(Canvas.BottomProperty, player.Position[1] + dyPerFrame * (longAnimationFrameCounts + 1));

                longAnimationFrameCounts++;
                if (longAnimationFrameCounts == longAnimationFrames)
                {
                    longAnimationFrameCounts = 0;
                    RunDrawState133EndFlag = true;
                }
            }
        }

        public void FailLongAnimationWithJump(Person player)
        {
            double dxPerFrame;
            float floatdxPerFrame;
            //每一帧在x方向上的移动，每一帧在y方向上的移动
            dxPerFrame = (double)(player.dx * player.MoveDirection / longAnimationFrames);
            floatdxPerFrame = player.dx * player.MoveDirection / longAnimationFrames;

            if (longAnimationFrameCounts < longAnimationFrames)
            {
                //在跳跃工过程中，每一帧的x坐标，这是个绝对坐标中的坐标值,两个是一样的值，一个是double，一个是float，float是要带入一元二次方程的
                double x = player.Position[0] + dxPerFrame * (longAnimationFrameCounts + 1);
                float fx = player.Position[0] + floatdxPerFrame * (longAnimationFrameCounts + 1);
                //在跳跃工过程中，每一帧的y坐标，这是个绝对坐标中的坐标值，把x坐标带入一元二次方程中得出的y
                double y = (double)player.XYofJump(fx);

                UCplayer.SetValue(Canvas.LeftProperty, x);
                UCplayer.SetValue(Canvas.BottomProperty, y);

                longAnimationFrameCounts++;
                if (longAnimationFrameCounts == longAnimationFrames)
                {
                    longAnimationFrameCounts = 0;
                    RunDrawState133EndFlag = true;
                }
            }
        }
        #endregion

        //跳跃成功后生成新cube,并改变镜头位置状态backgroundMoveIndex = 1，下个循环开始背景移动
        public void ShowNewCube()
        {
            //UcNowCubeCom = UCcubecom1;

            // UCcube1 UCcube0 = new UCcube1();
            UCcubeCompression UCcubecom1 = new UCcubeCompression();

            //Layer_Cube.Children.Add(UCcube0);
            //UCcube0.SetValue(Canvas.LeftProperty, (double)(NowCube.Position[0] + (NextCube.k * NextCube.dx)));
            //UCcube0.SetValue(Canvas.BottomProperty, (double)(NowCube.Position[1] + NextCube.dy));
            Layer_Cube.Children.Add(UCcubecom1);
            UCcubecom1.SetValue(Canvas.LeftProperty, (double)(NowCube.Position[0] + (NextCube.k * NextCube.dx)));
            UCcubecom1.SetValue(Canvas.BottomProperty, (double)(NowCube.Position[1] + NextCube.dy));

            //更新，现在的UcNowCubeCom是刚才的UcNextCubeCom，现在的UcNextCubeCom是新生成的UCcubecom1
            UcNowCubeCom = UcNextCubeCom;
            UcNextCubeCom = UCcubecom1;
            backgroundMoveIndex = 1;
        }

        //镜头位置状态
        public void CameraPosition()
        {
            switch (backgroundMoveIndex)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        MoveBackground();
                        UpdataBackgroundLastPosition();
                        //测试用的，在窗口title上显示移动时候计数
                        Title = icamera++.ToString();

                        if (icamera == CameraMoveFrames)
                        {
                            icamera = 0;
                            backgroundMoveIndex = 0;
                        }
                        break;
                    }
            }
        }

        //为了实现移动镜头的效果，开始移动背景
        public void MoveBackground()
        {
            if (NowCube.RefreshDirection == NextCube.RefreshDirection)
            {
                FamilyCanvas.SetValue(Canvas.LeftProperty, LastLeftValue - NextCube.k * (NextCube.dx / CameraMoveFrames));
                FamilyCanvas.SetValue(Canvas.TopProperty, LastTopValue + (NextCube.dy / CameraMoveFrames));
            }
            if (NowCube.RefreshDirection != NextCube.RefreshDirection)
            {
                FamilyCanvas.SetValue(Canvas.TopProperty, LastTopValue + (NextCube.dy / CameraMoveFrames));
            }
        }

        //更新记录每次背景移动后背景Canvas的Position
        public void UpdataBackgroundLastPosition()
        {
            LastLeftValue = (double)FamilyCanvas.GetValue(Canvas.LeftProperty);
            LastTopValue = (double)FamilyCanvas.GetValue(Canvas.TopProperty);
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

        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("re");
        }

        private void ReStartButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void homeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
