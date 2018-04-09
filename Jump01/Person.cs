using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump01
{
    public class Person
    {                    
        private float[] position;
        private float[] falloffposition;
        private float distanaceofmove;
        private int directionofmove;
        public float dx;//跳跃后在x轴上的增量
        public float dy;//跳跃后在cube在y轴上的增量
        private float Dx;//跳跃后在cube在x轴上的坐标，坐标有正负，这里的Dx不仅由dx决定，还由refreshdirection决定，Dx是原person位置x坐标加上（refreshdirection * dx）
        private float Dy;//跳跃后在cube在y轴上的坐标，坐标只能为正且增加，因为无论新cube是往左上还是右上，y方向上的坐标都在增加，因此Dy是原cube中心y坐标加上dy
        private float y, x;
        private int k;
        private float a, b, c;
        public  float dhight;
        private float Theta;

        public float[] Position
        {
            get { return position; }
            set { position = value; }
        }
        public float[] FallOffPosition
        {
            get { return falloffposition; }
            set { falloffposition = value; }
        }
        public float distanceOfMove
        {
            get { return distanaceofmove; }
            set { distanaceofmove = value; }
        }

        public int MoveDirection
        {
            get { return directionofmove; }
            set { directionofmove = value; }
        }

        public int JudgeSafeSatate(float[] FallOffPosition, Cube nowCube, Cube nextCube)
        {
            float Xp = FallOffPosition[0];
            float Yp = FallOffPosition[1];

            if (nextCube.RefreshDirection == 1)
            {
                if (Xp >= nowCube.MinX && Xp <= nowCube.MidX)
                {
                    if (Yp >= nowCube.y0FROMx0(Xp) && Yp <= nowCube.y1FROMx1(Xp)) return 30;
                    else return 31;
                }
                if (Xp > nowCube.MidX && Xp <= nowCube.MaxX)
                {
                    if (Yp >= nowCube.y2FROMx2(Xp) && Yp <= nowCube.y3FROMx3(Xp)) return 30;
                    else return 31;
                }

                if (Xp > nowCube.MaxX && Xp < nextCube.MinX) return 31;
                if (Xp >= nextCube.MinX && Xp <= nextCube.MidX)
                {
                    if (Yp >= nextCube.y0FROMx0(Xp) && Yp <= nextCube.y1FROMx1(Xp)) return 32;
                    else return 31;
                }
                if (Xp > nextCube.MidX && Xp <= nextCube.MaxX)
                {
                    if (Yp >= nextCube.y2FROMx2(Xp) && Yp <= nextCube.y3FROMx3(Xp)) return 32;
                    return 33;
                }
                if (Xp > nextCube.MaxX) return 33;
            }
            if (nextCube.RefreshDirection == -1)
            {
                if (Xp >= nowCube.MinX && Xp <= nowCube.MidX)
                {
                    if (Yp >= nowCube.y0FROMx0(Xp) && Yp <= nowCube.y1FROMx1(Xp)) return 30;
                    else return 31;
                }
                if (Xp > nowCube.MidX && Xp <= nowCube.MaxX)
                {
                    if (Yp >= nowCube.y2FROMx2(Xp) && Yp <= nowCube.y3FROMx3(Xp)) return 30;
                    else return 31;
                }

                if (Xp > nextCube.MaxX && Xp < nowCube.MinX) return 31;
                if (Xp >= nextCube.MidX && Xp <= nextCube.MaxX)
                {
                    if (Yp >= nextCube.y2FROMx2(Xp) && Yp <= nextCube.y3FROMx3(Xp)) return 32;
                    else return 31;
                }
                if (Xp >= nextCube.MinX && Xp < nextCube.MidX)
                {
                    if (Yp >= nextCube.y0FROMx0(Xp) && Yp <= nextCube.y1FROMx1(Xp)) return 32;
                    else return 33;
                }
                if (Xp < nextCube.MinX) return 33;
            }

            return 39;//还有情况没有考虑到，坐标以及方程之间的判断出错。        
        }



        public float[] ComputeFallOutPositon(float distanceofmove, float[] playerposition, float[] nextcubeposition, int nextcuberefreshdirection)
        {
            //算出通过小人起跳位置和下一cube中心的直线与X轴之间的角度Theta
            ComputerTheta(playerposition, nextcubeposition);
            //通过小人跳跃距离和Theta算出调完后在x轴上的增量
            dx = dxOfDistanceOfmove(distanceofmove);
            //通过小人跳跃的方向算出小人跳跃完后在x轴上的坐标，因为方向不同x坐标可能是增加或者减少
            Dx = playerposition[0] + (nextcuberefreshdirection * dx);
            //将Dx带入，起跳位置和cube中心的方程，得到小人下落的y坐标
            Dy = XYofPersonToCube(Dx, playerposition, nextcubeposition);
            dy = Dy - playerposition[1];
            FallOffPosition = new float[2] { Dx, Dy };
            return FallOffPosition;
        }

        public float dxOfDistanceOfmove(float distanceofmove)
        {
            dx = (float)(distanceofmove * Math.Cos(Theta));
            return dx;
        }

        public void ComputerTheta(float[] playerposition, float[] nextcubeposition)
        {
            float DX, DY;
            DY = Math.Abs(nextcubeposition[1] - playerposition[1]);
            DX = Math.Abs(nextcubeposition[0] - playerposition[0]);
            Theta = (float)(Math.Atan(DY / DX));
        }

        public float XYofPersonToCube(float x, float[] playerposition, float[] nextcubeposition)
        {
            float yValue;
            yValue = ((nextcubeposition[1] - playerposition[1]) / (nextcubeposition[0] - playerposition[0])) * (x - playerposition[0]) + playerposition[1];
            return yValue;
        }
        public float PressTimeToDistance(float PressTime)
        {
            float k = (200F) / 1000f;//之前方块边长是30，所以用一半15除以1000ms，现在边长是94，因此是94的一半除以1s
            distanaceofmove = PressTime * k;
            return distanaceofmove;
        }

        public void parameterXYofJump(float[] playerposition, float[] nextcubeposition)
        {
            float x1;
            float y1;
            float x2;
            float y2;
            float x3;
            float y3;
            x1 = Position[0];
            y1 = Position[1];
            x2 = FallOffPosition[0];
            y2 = FallOffPosition[1];
            x3 = x1 + (dx * MoveDirection / 2);
            y3 = XYofPersonToCube(x3, playerposition, nextcubeposition)+dhight;

            a = ((x1 - x2) * (y2 - y3)) / (((x2 * x2 - x3 * x3) * (x1 - x2)) - ((x1 * x1 - x2 * x2) * (x2 - x3)));
            b = ((y1 - y2) - (a * (x1 * x1 - x2 * x2))) / (x1 - x2);
            c = y1 - a * x1 * x1 - b * x1;
        }

        public float XYofJump(float x)
        {
            float yValue;
            yValue = a * x * x + b * x + c;
            return yValue;
        }

    }
}
