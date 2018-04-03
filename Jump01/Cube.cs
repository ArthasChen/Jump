using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump01
{
    public  class Cube
    {
        private float[] position = new float[2];
        private float[] dpositon = new float[2];
        private float sidelength;
        private int refreshdirection;
        private int refreshdistance;
        public float dx;//刷新cube在x轴上的增量
        public float dy;//刷新cube在y轴上的增量
        private float Dx;//刷新cube在x轴上的坐标，坐标有正负，这里的Dx不仅由dx决定，还由refreshdirection决定，Dx是原cube中心x坐标加上（refreshdirection * dx）
        private float Dy;//刷新cube在y轴上的坐标，坐标有正负，但是无论新cube是往左上还是右上，y方向上的坐标都在增加，因此Dy是原cube中心y坐标加上dy
        public float MinX;//新cube最左边顶点的x坐标，把它看作能落在方块中的最小x值
        public float MaxX;//新cube最右边顶点的x坐标，把它看作能落在方块中的最大x值
        public float MidX;//新cube中点的x坐标，把它看作分段函数分段的中心，这个值就是cube位置的x值，也就是Position属性的数组中的第0个。
        public float y, x, b;
        private float y0, x0, b0;
        private float y1, x1, b1;
        private float y2, x2, b2;
        private float y3, x3, b3;
        public int k, k0, k1, k2, k3;
        //刷新的位置，由x和y组成，由两位单精度数组表示。这个值是由刷新的相对位置+上一cube的位置得出的
        public float[] Position
        {
            get { return position; }
            set { position = value; }
        }

        //刷新的相对位置，由refreshdirection、 dx和dy组成，由两位单精度数组表示，这个值是由刷新距离得出的，由刷新距离求得在x和y上的相对增量，然后在根据由refreshdirection，得出x的坐标是增加还是减少，然后得到刷新的相对位置
        public float[] DPosition
        {
            get { return dpositon; }
            set { dpositon = value; }

        }

        //cube的边长
        public float SideLength
        {
            get { return sidelength; }
            set { sidelength = value; }
        }

        //cube的刷新方位，1表示朝向右上角，-1表示朝向左上角
        public int RefreshDirection
        {
            get { return refreshdirection; }
            set { refreshdirection = value; }
        }

        //cube的刷新距离，这个距离是指从上一个cube的中心距离这个cube的中心
        //在这个游戏中范围最小是150，最大是300，随机出现，但是最好在优化的时候枚举几个值，只在这几个枚举值里随机更新
        public int RefreshDistance
        {
            get { return refreshdistance; }
            set { refreshdistance = value; }
        }

        //计算cube的位置，和MinX，MidX，MaxX的值
        public float[] ComputCubePositon(int refreshdirection, int refreshdistance, float[] lastCubePositon)
        {
            ComputeParameterOfXY(refreshdirection, lastCubePositon);
            dx = computedx(refreshdistance);
            Dx = computeDx(dx, lastCubePositon);
            Dy = yFROMx(Dx);
            dy = Dy - lastCubePositon[1];

            MinX = (float)(Dx - SideLength * Math.Cos(Math.PI / 6));
            MidX = Dx;
            MaxX = (float)(Dx + SideLength * Math.Cos(Math.PI / 6));

            float[] positionvValue = new float[2] { Dx, Dy };
            return positionvValue;
        }

        //计算穿过两个cube中心的一次函数的参数，斜率k * ((float)Math.Tan(Math.PI / 6))及截距b，这两个参数是全局变量。这里计算好后，后面还能用，且只用在后面的函数
        public void ComputeParameterOfXY(int refreshdirection, float[] lastCubePositon)
        {
            k = refreshdirection;
            b = lastCubePositon[1] - (k * ((float)Math.Tan(Math.PI / 6))) * lastCubePositon[0];
        }

        //穿过两个cube中心的一次函数，给x值，求y值，
        public float yFROMx(float x)
        {
            float yValue;
            yValue = k * ((float)Math.Tan(Math.PI / 6)) * x + b;
            return yValue;
        }

        //计算cube中心坐标Dx，由dx和refreshdirection组成，若向右上角，则Dx是增加的，若向左上角，则Dx是减少的
        public float computeDx(float dx, float[] lastCubePositon)
        {
            float DxValue;
            DxValue = lastCubePositon[0] + k * dx;

            return DxValue;
        }

        //计算cube中心增量dx
        public float computedx(float RefreshDistance)
        {
            float dxValue;
            dxValue = (float)(RefreshDistance * Math.Cos(Math.PI / 6));
            return dxValue;
        }

        //这个方法只是求得4条方程的参数
        public void ComputeParameterOfSafeZone()
        {
            if (RefreshDirection == 1)//当刷新cube在右上方时，求cube四条边的函数方程的参数
            {
                float yf, xf, bf;
                int kf;
                kf = -RefreshDirection;
                xf = Position[0];
                yf = Position[1];
                bf = (float)(yf + Math.Tan(Math.PI / 6) * xf);

                //y0方程斜率和刷新方向不一致，斜率相反，
                //重新求一个经过刷新cube的中心，并且斜率为-1的方程的参数，y0在此函数下面，y0的截距减小
                k0 = -RefreshDirection;
                b0 = bf - SideLength / 2;

                //y1方程斜率和刷新方向一致，斜率不变，y1在经过cube中心函数的上面，截距增加
                k1 = RefreshDirection;
                b1 = b + SideLength / 2;

                //y2方程斜率和刷新方向一致，斜率不变，y2在经过cube中心函数的下面，截距减小
                k2 = RefreshDirection;
                b2 = b - SideLength / 2;

                //y0方程斜率和刷新方向不一致，斜率相反，
                //重新求一个经过刷新cube的中心，并且斜率为-1的方程的参数，y3在此函数上面，y0的截距增加
                k3 = -RefreshDirection;
                b3 = bf + SideLength / 2;
            }
            else
            {
                float yf, xf, bf;
                int kf;
                kf = -RefreshDirection;
                xf = Position[0];
                yf = Position[1];
                bf = (float)(yf - Math.Tan(Math.PI / 6) * xf);

                //y0方程斜率和刷新方向一致，斜率不变，y0在经过cube中心函数的下面，截距减小             
                k0 = RefreshDirection;
                b0 = b - SideLength / 2;

                //y1方程斜率和刷新方向不一致，斜率相反，
                //重新求一个经过刷新cube的中心，并且斜率为1的方程的参数，y1在此函数上面，y1的截距增加
                k1 = -RefreshDirection;
                b1 = bf + SideLength / 2;

                //y2方程斜率和刷新方向不一致，斜率相反，
                //重新求一个经过刷新cube的中心，并且斜率为1的方程的参数，y2在此函数下面，y2的截距减小
                k2 = -RefreshDirection;
                b2 = bf - SideLength / 2;

                //y0方程斜率和刷新方向一致，斜率不变，y3在经过cube中心函数的上面，截距增加         
                k3 = RefreshDirection;
                b3 = b + SideLength / 2;
            }
        }

        //y0函数，给x值，求y值，
        public float y0FROMx0(float x)
        {
            float yValue;
            yValue = k0 * ((float)Math.Tan(Math.PI / 6)) * x + b0;
            return yValue;
        }

        //y1函数，给x值，求y值，
        public float y1FROMx1(float x)
        {
            float yValue;
            yValue = k1 * ((float)Math.Tan(Math.PI / 6)) * x + b1;
            return yValue;
        }

        //y2函数，给x值，求y值，
        public float y2FROMx2(float x)
        {
            float yValue;
            yValue = k2 * ((float)Math.Tan(Math.PI / 6)) * x + b2;
            return yValue;
        }

        //y0函数，给x值，求y值，
        public float y3FROMx3(float x)
        {
            float yValue;
            yValue = k3 * ((float)Math.Tan(Math.PI / 6)) * x + b3;
            return yValue;
        }

    }
}
