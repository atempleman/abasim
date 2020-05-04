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
                scoreComm = homeTeamName + " " + homeScore + " " + awayTeamName + " " + awayScore + " - ";
            } else {
                scoreComm = awayTeamName + " " + awayScore + " " + homeTeamName + " " + homeScore + " - ";
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

        public string GetTwoPointMakeCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes the mid-range jump shot.";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A strong drive to the rim results in 2 points for " + playername + ".";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A contested shot in the paint by " + playername + " is good.";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " An easy drive to the hoop for " + playername + " for an easy 2.";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " with the stepback for 2.";
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

        public string GetThreePointMakeCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " makes the corner three.";
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A spectacular step-back 3 by " + playername + ".";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A contested 3 ball by " + playername + " is good.";
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A wide open 3 from the elbow for " + playername + " is good.";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " hits the 3.";
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

        public string GetOffensiveReboundCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A great offensive rebound is reeled in by " + playername;
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A tipped out offensive rebound is controlled by " + playername;
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " A strong offensive rebound is hauled in by " + playername;
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " gets the offensive rebound";
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The offensive rebound is gathered by " + playername;
                default:
                    return "Error in offensive rebound commentary";
            }
        }

        public string GetDefensiveReboundCommentary(string playername, int time, int quarter, int awayScore, int homeScore, int possession, string awayTeamName, string homeTeamName)
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
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " The defensive rebound is gathered by " + playername;
                case 1:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm + " " + playername + " controls the defensive rebound";
                case 2:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm +" A strong defensive rebound in traffic by " + playername;
                case 3:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm +" The defensive rebound is tipped around and controlled by " + playername;
                case 4:
                    return "Quarter #" + quarter + " - " + minutes + ":" + seconds + " - " + scoreComm +" A strong box out leads to " + playername + " getting the defensive rebound";
                default:
                    return "Error in defensive rebound commentary";
            }
        }
    }
}