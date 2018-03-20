using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump01
{
    public class CreatRandomNumber
    {

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
