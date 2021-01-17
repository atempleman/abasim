using System;
using ABASim.api.Dtos;
using ABASim.api.Models;

namespace ABASim.api.Controllers
{
    public class Commentary
    {
        public Commentary() 
        {

        }

        public string GetGameIntroCommentry(SimTeamDto awayTeam, SimTeamDto homeTeam)
        {
            return "Welcome to today's contest between the visiting " + awayTeam.TeamName + " " + awayTeam.Mascot + " and the home town " + homeTeam.TeamName + " " + homeTeam.Mascot + ". Now let's take a look at the starting lineups. ";
        }

        public string GetStartingLineupsCommentary(Player awayPG, Player awaySG, Player awaySF, Player awayPF, Player awayC)
        {
            return "At Point Guard is " + awayPG.FirstName + " " + awayPG.Surname + ". At the other guard " + awaySG.FirstName + " " + awaySG.Surname + ". " + "At the Small Forward position, " + awaySF.FirstName + " " + awaySF.Surname + ". The Power Forward today is " + awayPF.FirstName + " " + awayPF.Surname + ". And the man in the middle, " + awayC.FirstName + " " + awayC.Surname;
        }

        // public string GetJumpballCommentary(SimTeamDto team, int quarter, int time, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        // {
        //     int minutes = time / 60;
        //     int seconds = time % 60;

        //     string scoreComm = "";
        //     if (possession == 0) {
        //         scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
        //     } else {
        //         scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
        //     }

        //     string comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The jump ball is controlled by the " + team.TeamName + " " + team.Mascot;
        //     return comm;
        // }

        public string GetJumpballCommentary(int quarter, int time, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, string awayJumpPlayer, string homeJumpPlayer, string possessionPlayer)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            string comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + awayJumpPlayer + " vs. " + homeJumpPlayer + " (" + possessionPlayer + ")";
            return comm;
        }

        public string GetPassCommentary(string passer, string receiver, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);
            string comm = "";

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (choice)
            {
                case 0:
                    comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + passer + " passes the ball to " + receiver;
                    break;
                case 1:
                    comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + receiver + " gets the ball from " + passer;
                    break;
                case 2:
                    comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The ball is passed to " + receiver;
                    break;
                case 3:
                    comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A quick pass by " + passer + " to " + receiver;
                    break;
                case 4:
                    comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + receiver + " gets the ball from " + passer;
                    break;
                default:
                    return "Error in pass commentary";
            }
            return comm;
        }
    
        public string GetShotClockTurnoverCommentary(int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 2);
            string comm = "";

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            } else {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            }

            switch (choice)
            {
                case 0:
                    comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "The shot clock goes off and no shot has been attempted, that's a turnover!";
                    break;
                case 1:
                    comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "They don't get the shot off in time, shot clock violation! Great Defence.";
                    break;
                default:
                    throw new System.InvalidOperationException("Error in shot clock violation commentary");
            }
            return comm;
        }

        public string EndOfQuarterCommtary(int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }
            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " And that is the end of the quarter.";
        }

        public string EndGameCommentary(SimTeamDto awayTeam, SimTeamDto homeTeam, int awayScore, int homeScore)
        {
            if (awayScore > homeScore)
            {
                string comm = "The " + awayTeam.TeamName + " " + awayTeam.Mascot + " have defeated the " + homeTeam.TeamName + " " + homeTeam.Mascot + " " + awayScore + " to " + homeScore + ".";
                return comm;
            } else
            {
                string comm = "The " + homeTeam.TeamName + " " + homeTeam.Mascot + " have defeated the " + awayTeam.TeamName + " " + awayTeam.Mascot + " " + homeScore + " to " + awayScore + ".";
                return comm;
            }
        }

        public string GetTwoPointMakeCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int assist, string assistPlayer, int commPoints, int commAsts)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            string assistComm = "";
            if(assist == 1)
            {
                assistComm = " (" + assistPlayer + " assists - " + commAsts + " asts)";
            }

            int dunklayup = random.Next(0, 30);
            string layupdunk = "";
            if (dunklayup > 25)
            {
                layupdunk = "dunk";
            }
            else
            {
                layupdunk = "layup";
            }

            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes two point shot" + " (" + commPoints + " pts)" + assistComm;
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes " + layupdunk + " (" + commPoints + " pts)" + assistComm;
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes step back two point shot" + " (" + commPoints + " pts)" + assistComm;
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes deep two point shot" + " (" + commPoints + " pts)" + assistComm;
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes contested two point shot" + " (" + commPoints + " pts)" + assistComm;
                default:
                    return "Error has occured in the GetTwoPointMakeCommentary";
            }
        }

        public string  GetTwoPointMissCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses driving layup";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses step back two point shot";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses two point shot";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses deep two point shot";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses jumper";
                default:
                    return "Error in Get Missed Two Commentary";
            }
        }

        public string GetThreePointMakeCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int assist, string assistPlayer, int commPoints, int commAsts)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            string assistComm = "";
            if(assist == 1)
            {
                assistComm = " (" + assistPlayer + " assists - " + commAsts + " asts)";
            }
            
            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses three point shot";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses corner three point shot";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses deep three point shot";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses three point shot";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + "misses three point shot";
                default:
                    return "Error has occured in the GetThreePointMakeCommentary";
            }
        }

        public string  GetThreePointMissCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " misses the corner 3.";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A deep three is missed by " + playername;
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A contested staight on three is missed by " + playername;
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A pull-up three clangs of the front of the rim by " + playername;
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A 3 ball from the elbow is missed by " + playername;
                default:
                    return "Error in Get Missed Three Commentary";
            }
        }

        public string GetOffensiveReboundCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int commOReb, int commDReb)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);

            int commTotal = commDReb + commOReb;

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " offensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " offensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " offensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " offensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " offensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                default:
                    return "Error in offensive rebound commentary";
            }
        }

        public string GetOffensiveTeamReboundCommentary(int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int commOReb, int commDReb)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);

            int commTotal = commDReb + commOReb;

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
                return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "  " + homeTeamName + " offensive team rebound";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
                return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "  " + awayTeamName + " offensive team rebound";
            }
        }

        public string GetDefensiveReboundCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int commOReb, int commDReb)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);

            int commTotal = commDReb + commOReb;
            
            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " defensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " defensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " defensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " defensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " defensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                default:
                    return "Error in defensive rebound commentary";
            }
        }

        public string GetDefensiveTeamReboundCommentary(int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int commOReb, int commDReb)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);

            int commTotal = commDReb + commOReb;
            
            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
                return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "  " + homeTeamName + " defensive team rebound";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
                return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "  " + awayTeamName + " defensive team rebound";
            }
        }

        public string BlockCommentary(string player, string lostPlayer, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int blockComm)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);
            
            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + player + " blocks " + lostPlayer + "'s shot (" + blockComm + " blks)";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + player + " blocks " + lostPlayer + "'s shot (" + blockComm + " blks)";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + player + " blocks " + lostPlayer + "'s shot (" + blockComm + " blks)";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + player + " blocks " + lostPlayer + "'s shot (" + blockComm + " blks)";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + player + " blocks " + lostPlayer + "'s shot (" + blockComm + " blks)";
                default:
                    return "Error in Block Commentary";
            }
        }
    
        public string StealCommentary(string player, string lostPlayer, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int stealComm)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 5);
            
            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + lostPlayer + " bad pass" + " (" + player + " steals " + stealComm + "stls)";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + lostPlayer + " lost ball" + " (" + player + " steals " + stealComm + "stls)";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + lostPlayer + " bad pass" + " (" + player + " steals " + stealComm + "stls)";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + lostPlayer + " lost ball" + " (" + player + " steals " + stealComm + "stls)";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + lostPlayer + " bad pass" + " (" + player + " steals " + stealComm + "stls)";
                default:
                    return "Error in Steal Commentary";
            }
        }

        public string TurnoverCommentary(int type, string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int toComm)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 3);
            
            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (type)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " out of bounds bad pass turnover " + " (" + toComm + " tovs)";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " turnover " + " (" + toComm + " tovs)";
                case 2:
                     return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " offensive foul " + " (" + toComm + " tovs)";
                default:
                    return "Error in turnover commentary";
            }
        }

        public string FoulCommentary(string playername, string foulingPlayer, int type, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int foulComm)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            Random random = new Random();
            int choice = random.Next(0, 3);
            
            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            switch (type)
            {
                case 1:
                    // Non-Shooting Foul
                    switch (choice)
                    {
                        case 0:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + foulingPlayer + " personal foul " + " (" + foulComm + " fls)";
                        case 1:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + foulingPlayer + " personal foul " + " (" + foulComm + " fls)";
                        case 2:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + foulingPlayer + " personal foul " + " (" + foulComm + " fls)";
                        default:
                            return "Error in Foul Commentary - Non-shooting foul";
                    }
                case 2:
                    // Shooting Foul - 2
                    switch (choice)
                    {
                        case 0:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + foulingPlayer + " shooting foul" + " (" + foulComm + " fls)";
                        case 1:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + foulingPlayer + " shooting foul" + " (" + foulComm + " fls)";
                        case 2:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + foulingPlayer + " shooting foul" + " (" + foulComm + " fls)";
                        default:
                            return "Error in Foul Commentary - 2 shooting foul";
                    }
                case 3:
                    // Shooting Foul - 3
                    switch (choice)
                    {
                        case 0:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + foulingPlayer + " shooting foul" + " (" + foulComm + " fls)";
                        case 1:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + foulingPlayer + " shooting foul" + " (" + foulComm + " fls)";
                        case 2:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + foulingPlayer + " shooting foul" + " (" + foulComm + " fls)";
                        default:
                            return "Error in Foul Commentary - 3 shooting foul";
                    }
                default:
                    return "Error in the Foul Commentary - Type";
            }
        }

        public string BonusCommentary(int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "This will be a shooting foul due to being in the penalty.";
        }

        public string GetMadeFreeThrowCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName, int commPoints)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " makes free throw" + " (" + commPoints + " pts)";
        }

        public string GetMissedFreeThrowCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " misses free throw";
        }

        public string GetSubCommentary(string outPlayer, string inPlayer, int possession, string awayTeamName, string homeTeamName)
        {
            string subComm = "";
            if (possession == 0) {
                subComm = homeTeamName;
            } else {
                subComm = awayTeamName;
            }
            return subComm = " - " + inPlayer + " enters the game for " + outPlayer;
        }

        public string GetHoldBallCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " holds onto the ball.";
        }

        public string GetInjuryCommentary(string playername, int severity, string injuryType)
        {
            if (severity <= 2) {
                return "INJURY - " + playername + " has picked up a minor injury to his " + injuryType + ". How much will this effect his play?";
            } else {
                return "INJURY - " + playername + " has picked up a significant injury to his " + injuryType + ". He will not be able to continue.";
            }
        }
    }
}