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

        public string GetJumpballCommentary(SimTeamDto team, int quarter, int time, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            string scoreComm = "";
            if (possession == 0) {
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
            }

            string comm = "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The jump ball is controlled by the " + team.TeamName + " " + team.Mascot;
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
                assistComm = " The shot was assisted by " + assistPlayer;

                if (commAsts > 0) {
                    assistComm = assistComm + " (" + commAsts + " asts)";
                }
            }

            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes the mid-range jump shot." + " (" + commPoints + " pts)" + assistComm;
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A strong drive to the rim results in 2 points for " + playername + "." + " (" + commPoints + " pts)" + assistComm;
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A contested shot in the paint by " + playername + " is good." + " (" + commPoints + " pts)" + assistComm;
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " An easy drive to the hoop for " + playername + " for an easy 2." + " (" + commPoints + " pts)" + assistComm;
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " with the stepback for 2." + "(" + commPoints + " pts)" + assistComm;
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " misses the mid-range jumper badly.";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A strong drive to the rim by " + playername + " is no good!";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The deep 2 is missed by " + playername;
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A heavily contested drive is no good by " + playername;
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The shot is up and off the front rim by " + playername;
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
                assistComm = " The shot was assisted by " + assistPlayer;

                if (commAsts > 0) {
                    assistComm = assistComm + " (" + commAsts + " asts)";
                }
            }
            
            switch (choice)
            {
                case 0:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes the corner three." + " (" + commPoints + " pts)" + assistComm;
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A spectacular step-back 3 by " + playername + "." + " (" + commPoints + " pts)" + assistComm;
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A contested 3 ball by " + playername + " is good." + " (" + commPoints + " pts)" + assistComm;
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A wide open 3 from the elbow for " + playername + " is good." + " (" + commPoints + " pts)" + assistComm;
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " hits the 3." + " (" + commPoints + " pts)" + assistComm;
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A great offensive rebound is reeled in by " + playername + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A tipped out offensive rebound is controlled by " + playername + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A strong offensive rebound is hauled in by " + playername + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " gets the offensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The offensive rebound is gathered by " + playername + " (off: " + commOReb + ", def: " + commDReb + ")";
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
                return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The rebound has been tipped out of bounds and possession is with " + homeTeamName;
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
                return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The rebound has been tipped out of bounds and possession is with " + awayTeamName;
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The defensive rebound is gathered by " + playername + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " controls the defensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm +" A strong defensive rebound in traffic by " + playername + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm +" The defensive rebound is tipped around and controlled by " + playername + " (off: " + commOReb + ", def: " + commDReb + ")";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm +" A strong box out leads to " + playername + " getting the defensive rebound" + " (off: " + commOReb + ", def: " + commDReb + ")";
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
                return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The rebound has been tipped out of bounds and possession is with " + homeTeamName;
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
                return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The rebound has been tipped out of bounds and possession is with " + awayTeamName;
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + player + " has blocked " + lostPlayer + " empathically! (" + blockComm + " blks)";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "The shot by " + lostPlayer + " is blocked by " + player + "(" + blockComm + " blks)";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "The shot by " + lostPlayer + " is swatted away by " + player + "(" + blockComm + " blks)";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm  + player + " comes from the weakside to block " + lostPlayer + "'s shot attempt. (" + blockComm + " blks)";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm  + "A strong contest on " + lostPlayer + " sees his shot blocked by " + player + "(" + blockComm + " blks)";;
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + player + " has picked " + lostPlayer + "'s clean!" + " (" + stealComm + "stls)";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "The pass is intercepted by " + player + " (" + stealComm + "stls)";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "The ball is fumbled and taken away by " + player + " (" + stealComm + "stls)";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + lostPlayer + " throws a sloppy pass which is stolen by " + player + " (" + stealComm + "stls)";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "A great steal by " + player + " (" + stealComm + "stls)";
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " has thrown the ball out of bounds" + " (" + toComm + " tovs)";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "That's called travel on " + playername + "." + " (" + toComm + " tovs)";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + "That is an offensive foul against " + playername + "." + " (" + toComm + " tovs)";
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
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + foulingPlayer + " fouls " + playername + " on the drive." + " (" + foulComm + " fls)";
                        case 1:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + foulingPlayer + " is too aggressive on the steal attempt and is called for the foul on " + playername + " (" + foulComm + " fls)";
                        case 2:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " craftily draws the non-shooting foul on " + foulingPlayer + " (" + foulComm + " fls)";
                        default:
                            return "Error in Foul Commentary - Non-shooting foul";
                    }
                case 2:
                    // Shooting Foul - 2
                    switch (choice)
                    {
                        case 0:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A clever pump fake inside by " + playername + " draws the foul on " + foulingPlayer + "." + " (" + foulComm + " fls)";
                        case 1:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " shoots the mid-range jumpshot and is fouled by " + foulingPlayer + ". The shot is no good." + " (" + foulComm + " fls)";
                        case 2:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " pulls up to shoot and is fouled from behind by " + foulingPlayer + ". Two shots coming for " + playername + " (" + foulComm + " fls)";
                        default:
                            return "Error in Foul Commentary - 2 shooting foul";
                    }
                case 3:
                    // Shooting Foul - 3
                    switch (choice)
                    {
                        case 0:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " shoots the the straight on 3 and is fouled by " + foulingPlayer + ". The shot is no good." + " (" + foulComm + " fls)";
                        case 1:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " pulls up to shoot the 3 and is fouled from behind by " + foulingPlayer + ". Three shots coming for " + playername + " (" + foulComm + " fls)";
                        case 2:
                            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " fires up the 3 which is way off, but a foul has been called against " + foulingPlayer + "." + " (" + foulComm + " fls)";
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

            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " makes the free throw" + " (" + commPoints + " pts)";
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

            return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + playername + " misses the free throw";
        }

        public string GetSubCommentary(string outPlayer, string inPlayer, int possession, string awayTeamName, string homeTeamName)
        {
            string subComm = "";
            if (possession == 0) {
                subComm = homeTeamName;
            } else {
                subComm = awayTeamName;
            }

            return "SUB: " + subComm + " make a substitution - " + outPlayer + " is subbed out. " + inPlayer + " is taking his place.";
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