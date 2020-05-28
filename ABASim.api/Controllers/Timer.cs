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
            return random.Next(1, 6);
            // int result = random.Next(0, 101);

            // if (result <= 10) {
            //     return 1;
            // } else if (result > 10 && result <= 25) {
            //     return 2;
            // } else if (result > 25 && result <= 55) {
            //     return 3;
            // } else if (result > 55 && result <= 80) {
            //     return 4;
            // } else if (result > 80 && result <= 90) {
            //     return 5;
            // } else if (result > 90 && result <= 95) {
            //     return 6;
            // } else {
            //     return 7;
            // }
        }

        public int GetTwoShotTime()
        {
            Random random = new Random();
            return random.Next(1, 6);
            // int result = random.Next(0, 101);

            // if (result <= 10) {
            //     return 1;
            // } else if (result > 10 && result <= 30) {
            //     return 2;
            // } else if (result > 30 && result <= 70) {
            //     return 3;
            // } else if (result > 70 && result <= 90) {
            //     return 4;
            // } else if (result > 90 && result <= 100) {
            //     return 5;
            // } else {
            //     return 6;
            // }
        }

        public int GetThreeShotTime()
        {
            Random random = new Random();
            return random.Next(1, 6);
            // int result = random.Next(0, 101);

            // if (result <= 10) {
            //     return 1;
            // } else if (result > 10 && result <= 30) {
            //     return 2;
            // } else if (result > 30 && result <= 70) {
            //     return 3;
            // } else if (result > 70 && result <= 90) {
            //     return 4;
            // } else if (result > 90 && result <= 100) {
            //     return 5;
            // } else {
            //     return 6;
            // }
        }
    }
}