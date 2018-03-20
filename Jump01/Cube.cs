using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump01
{
    public  class Cube:ICreatRandomNumber
    {
        public double position;
        public double sidelength;
        public int refreshdistance;
        public double minsafedistance;
        public double maxsafedistance;
        public double Position
        {
            get { return position; }
            set { position = value; }
        }
        public double SideLength
        {
            get { return sidelength; }
            set { sidelength = value; }
        }
        public int RefreshDistance
        {
            get { return refreshdistance; }
            set { refreshdistance = value; }
        }
        public int RefreshDirection { get; set; }

        public int CreatRandomSideLengthForCube()
        {
            return CreatRandomSideLength();
        }
        public int CreatRandomDistance()
        {
            Random rnd = new Random();
            return rnd.Next(60, 80);          
        }

        public int CreatRandomSideLength()
        {
            Random rnd = new Random();
            return rnd.Next(30, 40);
        }

    }
}
