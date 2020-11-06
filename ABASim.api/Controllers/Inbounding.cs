using ABASim.api.Models;

namespace ABASim.api.Controllers
{
    public class Inbounding
    {
        public Inbounding()
        {

        }

        public int GetInboundsResult(PlayerRating pg, PlayerRating sg, PlayerRating sf, PlayerRating pf, PlayerRating c, int j)
        {
            int result = j;

            if (result < pg.UsageRating) {
                return 1;
            } else if (result >= pg.UsageRating && result < pg.UsageRating + sg.UsageRating) {
                return 2;
            } else if (result >= pg.UsageRating + sg.UsageRating && result < pg.UsageRating + sg.UsageRating + sf.UsageRating) {
                return 3;
            } else if (result >= pg.UsageRating + sg.UsageRating + sf.UsageRating && result < pg.UsageRating + sg.UsageRating + sf.UsageRating + pf.UsageRating) {
                return 4;
            } else {
                return 5;
            }
        }
    }
}