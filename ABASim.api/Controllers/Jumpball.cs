namespace ABASim.api.Controllers
{
    public class Jumpball
    {
        public Jumpball() 
        {

        }

        public int GetJumpPlayer(int random)
        {
            int result = random;
            if (result < 60)
            {
                return 1;
            }
            else if (result >= 60 && result < 77)
            {
                return 2;
            }
            else if (result >= 77 && result < 87)
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