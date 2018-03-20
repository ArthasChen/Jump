using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump01
{
    public class Person
    {
        public bool isstand;
        public double position;
        public double distanaceofmove;
        public double firstsafedistance;
        public double secondsafedistance;
        public double thirdsafedistance;
        public bool isStand
        {
            get { return isstand; }
            set { isstand = value; }
        }
        public double Position
        {
            get { return position; }
            set { position = value; }
        }
        public double distanceOfMove
        {
            get { return distanaceofmove; }
            set { distanaceofmove = value; }
        }
        public double FirstSafeDistance
        {
            get { return firstsafedistance; }
            set { firstsafedistance = value; }
        }
        public double SecondSafeDistance
        {
            get { return secondsafedistance; }
            set { secondsafedistance = value; }
        }
        public double ThirdSafeDistance
        {
            get { return thirdsafedistance; }
            set { thirdsafedistance = value; }
        }
        public int MoveDirection { get; set; }

        public void ComputeSafeDistance(double PersonPosition,double NowCubeLength, double RefreshDistance, double NextCubeLength)
        //此处参数为double。但是Cube的位置和长度都为int。因此在使用时，应该把Cube的两个参数转换为double
        {
            FirstSafeDistance = (NowCubeLength / 2) - PersonPosition;
            SecondSafeDistance = RefreshDistance - PersonPosition - (NextCubeLength / 2);
            ThirdSafeDistance = RefreshDistance - PersonPosition + (NextCubeLength / 2);
        }
        public void ComputeMinSafeDistance()
        {

        }
        public void ComputeMaxSafeDistance()
        {

        }
        public int JudgeSafeSatate(double  distanceofmove)
        {          
            if (distanceofmove < FirstSafeDistance) return 30;
            else if (distanceofmove < SecondSafeDistance) return 31;
            else if (distanceofmove < ThirdSafeDistance) return 32;
            else  return 33;

            //以下代码为第一次测试用，数值全为ms时间，未转化为长度
            //if (PressTime < 1000) return 0;
            //else if (PressTime < 2000) return 1;
            //else if (PressTime < 3000) return 2;
            //else  return 3;
        }
        public void ComputeDistanceOfMove()
        {

        }//暂时没用到，应该是被下面的代替了，暂时留着
        public double PressTimeToDistance(double PressTime)
        {
            double k = (47d) / 1000d;//之前方块边长是30，所以用一半15除以1000ms，现在边长是94，因此是94的一半除以1s
            distanaceofmove = PressTime * k;
            return distanaceofmove;
        }



    }
}
