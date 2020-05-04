namespace ABASim.api.Controllers
{
    public class Inbounding
    {
        public Inbounding()
        {

        }

        public int GetInboundsResult(int j)
        {
            int result = j;
            if (result < 50)
            {
                return 1;
            }
            else if (result >= 50 && result < 70)
            {
                return 2;
            }
            else if (result >= 70 && result < 87)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
    }
}