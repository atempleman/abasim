using System;

namespace ABASim.api.Controllers
{
    public class Timer
    {
        public Timer()
        {

        }

        public int GetPassTime()
        {
            Random random = new Random();
            int result = random.Next(0, 101);

            if (result <= 20) {
                return 1;
            } else if (result > 20 && result <= 40) {
                return 2;
            } else if (result > 40 && result <= 70) {
                return 3;
            } else if (result > 70 && result <= 85) {
                return 4;
            } else if (result > 85 && result <= 95) {
                return 5;
            } else if (result > 95 && result <= 100) {
                return 6;
            }

            return 2;
        }
    }
}