using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Data;
using ABASim.api.Dtos;
using ABASim.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABASim.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameEngineController : ControllerBase
    {
        private readonly IGameEngineRepository _repo;
        List<string> commentaryData = new List<string>();
        Commentary comm = new Commentary();
        Timer _timer = new Timer();
        static Random _random = new Random();

        int _teamPossession, _playerPossession, _initialPossession;

        int _quarter, _time, _awayScore, _homeScore, _shotClock;

        int subError = 0;

        SimGameDto _game;

        SimTeamDto _awayTeam, _homeTeam;

        List<Roster> _awayRoster = new List<Roster>();
        List<Roster> _homeRoster = new List<Roster>();

        List<DepthChart> _awayDepth = new List<DepthChart>();
        List<DepthChart> _homeDepth = new List<DepthChart>();

        List<Player> _awayPlayers = new List<Player>();
        List<Player> _homePlayers = new List<Player>();

        List<PlayerRating> _awayRatings = new List<PlayerRating>();
        List<PlayerRating> _homeRatings = new List<PlayerRating>();

        List<PlayerTendancy> _awayTendancies = new List<PlayerTendancy>();
        List<PlayerTendancy> _homeTendancies = new List<PlayerTendancy>();

        List<StaminaTrack> _awayStaminas = new List<StaminaTrack>();
        List<StaminaTrack> _homeStaminas = new List<StaminaTrack>();

        List<BoxScore> _awayBoxScores = new List<BoxScore>();
        List<BoxScore> _homeBoxScores = new List<BoxScore>();

        PlayerRating homePGRatings, homeSGRatings, homeSFRatings, homePFRatings, homeCRatings;
        PlayerRating awayPGRatings, awaySGRatings, awaySFRatings, awayPFRatings, awayCRatings;
        PlayerTendancy homePGTendancy, homeSGTendancy, homeSFTendancy, homePFTendancy, homeCTendancy;
        PlayerTendancy awayPGTendancy, awaySGTendancy, awaySFTendancy, awayPFTendancy, awayCTendancy;

        Player homePG, homeSG, homeSF, homePF, homeC;
        Player awayPG, awaySG, awaySF, awayPF, awayC;
        Player _playerPassed;
        PlayerRating _playerRatingPassed;

        int _homeFoulBonus = 0;
        int _awayFoulBonus = 0;
        int _awayFinal2Bonus = 0;
        int _homeFinal2Bonus = 0;

        int foulCounter = 0;
        int blockCounter = 0;
        int stealCounter = 0;
        int turnoverCounter = 0;
        int shotClockCounter = 0;
        int assistCounter = 0;
        int assistCounterChance = 0;
        int twosTaken = 0;
        int threesTaken = 0;
        int timeCounter = 0;

        SubTracker _awaySubTracker = new SubTracker();

        SubTracker _homeSubTracker = new SubTracker();

        List<int> fouledOutPlayers = new List<int>();

        int _endGameShotClockBonus = 0;
        int _endGameTwoPointAddition = 0;
        int _endGameThreePointAddition = 0;
        int _endGameFoulAddition = 0;
        int _endGameStealAddition = 0;
        int _endGameResultIncrease = 0;


        List<PlayByPlay> _playByPlays = new List<PlayByPlay>();
        int _ordering = 0;
        int _playNumber = 0;

        List<InjurySeverity> _injurySeverities = new List<InjurySeverity>();
        List<InjuryType> _injuryTypes = new List<InjuryType>();
        List<InjuryDto> _awayInjuries = new List<InjuryDto>();
        List<InjuryDto> _homeInjuries = new List<InjuryDto>();

        List<CoachSetting> _homeSettings = new List<CoachSetting>();
        List<CoachSetting> _awaySettings = new List<CoachSetting>();
        int awayGoToOne;
        int awayGoToTwo;
        int awayGoToThree;
        int homeGoToOne;
        int homeGoToTwo;
        int homeGoToThree;

        List<int> homeStarterIds = new List<int>();
        List<int> awayStarterIds = new List<int>();

        TeamStrategy _homeStrategy = new TeamStrategy();
        TeamStrategy _awayStrategy = new TeamStrategy();

        // Strategy variables
        int _homeFoulsDrawnStrategy = 0;
        int _awayFoulsDrawnStrategy = 0;
        int _homeTwoTendancyStrategy = 0;
        int _awayTwoTendancyStrategy = 0;
        int _homeThreeTendancyStrategy = 0;
        int _awayThreeTendancyStrategy = 0;
        int _homeTwoPercentageStrategy = 0;
        int _awayTwoPercentageStrategy = 0;
        int _homeThreePercentageStrategy = 0;
        int _awayThreePercentageStrategy = 0;
        int _homeTurnoversStrategy = 0;
        int _awayTurnoversStrategy = 0;
        int _homeStealStrategy = 0;
        int _awayStealStrategy = 0;
        int _homeORebStrategy = 0;
        int _awayORebStrategy = 0;
        int _homeDRebStrategy = 0;
        int _awayDRebStrategy = 0;
        int _homeDRPMStrategy = 0;
        int _awayDRPMStrategy = 0;
        int _homeSpeedStrategy = 0;
        int _awaySpeedStrategy = 0;
        int _homeORPMStrategy = 0;
        int _awayORPMStrategy = 0;
        int _homeAssistStrategy = 0;
        int _awayAssistStrategy = 0;
        int _homeStaminaStrategy = 0;
        int _awayStaminaStrategy = 0;
        int _homeBlockStrategy = 0;
        int _awayBlockStrategy = 0;


        public GameEngineController(IGameEngineRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> StartGame(SimGameDto game)
        {
            _game = game;
            _awayScore = 0;
            _homeScore = 0;
            _quarter = 1;
            _time = 720;
            _shotClock = 24;

            // Setup the Game
            var result = await SetupTeams();
            var result1 = await SetupRosters();
            var result2 = await SetupDepthCharts();
            var result3 = await GetPlayerDetails();
            var result4 = await GetPlayerInjuries();
            var result5 = await GetCoachSettings();
            var result6 = await GetTeamStrategies();

            SetStartingLineups();
            awayStarterIds.Add(awayPG.Id);
            awayStarterIds.Add(awaySG.Id);
            awayStarterIds.Add(awaySF.Id);
            awayStarterIds.Add(awayPF.Id);
            awayStarterIds.Add(awayC.Id);
            homeStarterIds.Add(homePG.Id);
            homeStarterIds.Add(homeSG.Id);
            homeStarterIds.Add(homeSF.Id);
            homeStarterIds.Add(homePF.Id);
            homeStarterIds.Add(homeC.Id);

            // commentaryData.Add(comm.GetGameIntroCommentry(_awayTeam, _homeTeam)); // Need a way to block this out when run games for real
            // commentaryData.Add(comm.GetStartingLineupsCommentary(awayPG, awaySG, awaySF, awayPF, awayC));
            // commentaryData.Add(comm.GetStartingLineupsCommentary(homePG, homeSG, homeSF, homePF, homeC));
            // commentaryData.Add("It's now time for the opening tip");

            PlayByPlayTracker(comm.GetGameIntroCommentry(_awayTeam, _homeTeam), 0);
            PlayByPlayTracker(comm.GetStartingLineupsCommentary(awayPG, awaySG, awaySF, awayPF, awayC), 0);
            PlayByPlayTracker(comm.GetStartingLineupsCommentary(homePG, homeSG, homeSF, homePF, homeC), 0);
            PlayByPlayTracker("It's now time for the opening tip", 1);

            Jumpball();
            _initialPossession = _teamPossession;

            // 1st Quarter
            RunQuarter();

            // 2nd Quarter
            RunQuarter();

            // 3rd Quarter
            // SetStartingLineups();
            // EndGameSubCheck(0);
            SetStartingLineups();

            RunQuarter();

            // 4th Quarter
            RunQuarter();

            // Overtime
            while (_awayScore == _homeScore)
            {
                RunOvertime();
            }

            return Ok(true);
        }

        [HttpPost("startGame")]
        public async Task<IActionResult> StartTestGame(SimGameDto game)
        {
            await StartGame(game);

            // Will need to update the play by play saving here TODO
            bool savedPBPs = await _repo.SavePlayByPlays(_playByPlays);

            // Now we have a game id, now to save the away box scores
            bool saved = await _repo.SaveTeamsBoxScore(_game.GameId, _awayBoxScores);

            // Now we have a game id, now to save the home box scores
            saved = await _repo.SaveTeamsBoxScore(_game.GameId, _homeBoxScores);

            // Need to save the game in the database

            return Ok(commentaryData);
        }

        [HttpPost("startPreseasonGame")]
        public async Task<IActionResult> StartPreseasonGame(SimGameDto game)
        {
            await StartGame(game);

            // Will need to update the play by play saving here TODO
            bool savedPBPs = await _repo.SavePlayByPlays(_playByPlays);

            // Now we have a game id, now to save the away box scores
            bool saved = await _repo.SaveTeamsBoxScore(_game.GameId, _awayBoxScores);

            // Now we have a game id, now to save the home box scores
            saved = await _repo.SaveTeamsBoxScore(_game.GameId, _homeBoxScores);

            // Need to save the game in the database
            // Need to save the game in the database
            int winningTeamId = 0;
            if (_awayScore > _homeScore)
            {
                winningTeamId = _awayTeam.Id;
            }
            else
            {
                winningTeamId = _homeTeam.Id;
            }
            bool savedGame = await _repo.SavePreseasonResult(_awayScore, _homeScore, winningTeamId, game.GameId);

            return Ok(true);
        }

        [HttpPost("startPlayoffGame")]
        public async Task<IActionResult> StartPlayoffGame(SimGameDto game)
        {
            // Need to get the player injuries
            await StartGame(game);

            // Will need to update the play by play saving here
            bool savedPBPs = await _repo.SavePlayByPlaysPlayoffs(_playByPlays);

            // Now we have a game id, now to save the away box scores
            bool saved = await _repo.SaveTeamsBoxScorePlayoffs(_game.GameId, _awayBoxScores);

            // Now we have a game id, now to save the home box scores
            saved = await _repo.SaveTeamsBoxScorePlayoffs(_game.GameId, _homeBoxScores);

            // Need to save the game in the database
            int winningTeamId = 0;
            int losingTeamId = 0;
            if (_awayScore > _homeScore)
            {
                winningTeamId = _awayTeam.Id;
                losingTeamId = _homeTeam.Id;
            }
            else
            {
                winningTeamId = _homeTeam.Id;
                losingTeamId = _awayTeam.Id;
            }
            bool savedGame = await _repo.SavePlayoffResult(_awayScore, _homeScore, winningTeamId, game.GameId, losingTeamId);

            List<PlayerInjury> playerInjuries = new List<PlayerInjury>();
            // end of game injuries
            foreach (var injury in _homeInjuries)
            {
                PlayerInjury pi = new PlayerInjury();
                if (injury.StartQuarterImpact == 1 && injury.StartTimeImpact == 720 && injury.EndQuarterImpact == 4 && injury.EndTimeImpact == 0)
                {
                    // Player is pre-existing injured
                }
                else
                {
                    if (injury.Severity == 1)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                        pi.CurrentlyInjured = 0;
                    }
                    else if (injury.Severity == 2)
                    {
                        int tm = _random.Next(1, 1000);
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                        pi.CurrentlyInjured = 0;
                    }
                    else if (injury.Severity == 3)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                        pi.CurrentlyInjured = 0;
                    }
                    else if (injury.Severity == 4)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                        pi.CurrentlyInjured = 0;
                    }
                    else if (injury.Severity == 5)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                        pi.CurrentlyInjured = 0;
                    }
                    playerInjuries.Add(pi);
                }
            }

            foreach (var injury in _awayInjuries)
            {
                PlayerInjury pi = new PlayerInjury();
                if (injury.Severity == 1)
                {
                    pi.PlayerId = injury.PlayerId;
                    pi.Severity = injury.Severity;
                    pi.StartDay = 0;
                    pi.EndDay = 0;
                    pi.TimeMissed = 0;
                    pi.Type = injury.InjuryTypeName;
                    pi.CurrentlyInjured = 0;
                }
                else if (injury.Severity == 2)
                {
                    // int tm = _random.Next(1, 1000);
                    // int daysMissed = 0;
                    // if (tm >= 900 && tm < 950)
                    // {
                    //     daysMissed = 1;
                    // }
                    // else if (tm >= 950)
                    // {
                    //     daysMissed = 2;
                    // }

                    pi.PlayerId = injury.PlayerId;
                    pi.Severity = injury.Severity;
                    pi.StartDay = 0;
                    pi.EndDay = 0;
                    pi.TimeMissed = 0;
                    pi.Type = injury.InjuryTypeName;
                    pi.CurrentlyInjured = 0;
                }
                else if (injury.Severity == 3)
                {
                    // int tm = _random.Next(3, 21);
                    // int daysMissed = tm;

                    pi.PlayerId = injury.PlayerId;
                    pi.Severity = injury.Severity;
                    pi.StartDay = 0;
                    pi.EndDay = 0;
                    pi.TimeMissed = 0;
                    pi.Type = injury.InjuryTypeName;
                    pi.CurrentlyInjured = 0;
                }
                else if (injury.Severity == 4)
                {
                    // int tm = _random.Next(21, 50);
                    // int daysMissed = tm;

                    pi.PlayerId = injury.PlayerId;
                    pi.Severity = injury.Severity;
                    pi.StartDay = 0;
                    pi.EndDay = 0;
                    pi.TimeMissed = 0;
                    pi.Type = injury.InjuryTypeName;
                    pi.CurrentlyInjured = 0;
                }
                else if (injury.Severity == 5)
                {
                    // int tm = _random.Next(51, 180);
                    // int daysMissed = tm;

                    pi.PlayerId = injury.PlayerId;
                    pi.Severity = injury.Severity;
                    pi.StartDay = 0;
                    pi.EndDay = 0;
                    pi.TimeMissed = 0;
                    pi.Type = injury.InjuryTypeName;
                    pi.CurrentlyInjured = 0;
                }
                playerInjuries.Add(pi);
            }
            // Need to save all records now
            await _repo.SaveInjury(playerInjuries);

            return Ok(true);
        }

        [HttpPost("startSeasonGame")]
        public async Task<IActionResult> StartSeasonGame(SimGameDto game)
        {
            await StartGame(game);

            // Will need to update the play by play saving here TODO
            bool savedPBPs = await _repo.SavePlayByPlays(_playByPlays);

            // Now we have a game id, now to save the away box scores
            bool saved = await _repo.SaveTeamsBoxScore(_game.GameId, _awayBoxScores);

            // Now we have a game id, now to save the home box scores
            saved = await _repo.SaveTeamsBoxScore(_game.GameId, _homeBoxScores);

            // Need to save the game in the database
            int winningTeamId = 0;
            int losingTeamId = 0;
            if (_awayScore > _homeScore)
            {
                winningTeamId = _awayTeam.Id;
                losingTeamId = _homeTeam.Id;
            }
            else
            {
                winningTeamId = _homeTeam.Id;
                losingTeamId = _awayTeam.Id;
            }
            bool savedGame = await _repo.SaveSeasonResult(_awayScore, _homeScore, winningTeamId, game.GameId, losingTeamId);

            List<PlayerInjury> playerInjuries = new List<PlayerInjury>();
            // end of game injuries
            foreach (var injury in _homeInjuries)
            {
                PlayerInjury pi = new PlayerInjury();
                if (injury.StartQuarterImpact == 1 && injury.StartTimeImpact == 720 && injury.EndQuarterImpact == 4 && injury.EndTimeImpact == 0)
                {
                    // Player is pre-existing injured
                }
                else
                {
                    if (injury.Severity == 1)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    else if (injury.Severity == 2)
                    {
                        int tm = _random.Next(1, 1000);
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    else if (injury.Severity == 3)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    else if (injury.Severity == 4)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    else if (injury.Severity == 5)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    playerInjuries.Add(pi);
                }
            }

            foreach (var injury in _awayInjuries)
            {
                PlayerInjury pi = new PlayerInjury();
                if (injury.StartQuarterImpact == 1 && injury.StartTimeImpact == 720 && injury.EndQuarterImpact == 4 && injury.EndTimeImpact == 0)
                {
                    // Player is pre-existing injured
                }
                else
                {
                    if (injury.Severity == 1)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    else if (injury.Severity == 2)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    else if (injury.Severity == 3)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    else if (injury.Severity == 4)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    else if (injury.Severity == 5)
                    {
                        pi.PlayerId = injury.PlayerId;
                        pi.Severity = injury.Severity;
                        pi.StartDay = 0;
                        pi.EndDay = 0;
                        pi.TimeMissed = 0;
                        pi.Type = injury.InjuryTypeName;
                    }
                    playerInjuries.Add(pi);
                }
            }
            // Need to save all records now
            await _repo.SaveInjury(playerInjuries);

            // Now need to update the awards
            List<BoxScore> fullBS = new List<BoxScore>();
            fullBS.AddRange(_homeBoxScores);
            fullBS.AddRange(_awayBoxScores);
            await _repo.MvpVotes(fullBS);
            await _repo.SixthManVotes(fullBS, homeStarterIds, awayStarterIds);
            await _repo.DpoyVotes(fullBS);

            return Ok(true);
        }

        public async Task<IActionResult> SetupTeams()
        {
            Team at = await _repo.GetTeam(_game.AwayId);
            Team ht = await _repo.GetTeam(_game.HomeId);

            _awayTeam = new SimTeamDto
            {
                Id = at.Id,
                TeamName = at.Teamname,
                ShortCode = at.ShortCode,
                Mascot = at.Mascot
            };

            _homeTeam = new SimTeamDto
            {
                Id = ht.Id,
                TeamName = ht.Teamname,
                ShortCode = ht.ShortCode,
                Mascot = ht.Mascot
            };

            _awaySubTracker.CurrentPG = 1;
            _awaySubTracker.CurrentSG = 1;
            _awaySubTracker.CurrentSF = 1;
            _awaySubTracker.CurrentPF = 1;
            _awaySubTracker.CurrentC = 1;
            _awaySubTracker.QuarterSub = 0;

            _homeSubTracker.CurrentPG = 1;
            _homeSubTracker.CurrentSG = 1;
            _homeSubTracker.CurrentSF = 1;
            _homeSubTracker.CurrentPF = 1;
            _homeSubTracker.CurrentC = 1;
            _homeSubTracker.QuarterSub = 0;

            return Ok(true);
        }

        public async Task<IActionResult> SetupRosters()
        {
            var ar = await _repo.GetRoster(_awayTeam.Id);
            var hr = await _repo.GetRoster(_homeTeam.Id);
            _awayRoster = (List<Roster>)ar;
            _homeRoster = (List<Roster>)hr;

            return Ok(true);
        }

        public async Task<IActionResult> SetupDepthCharts()
        {
            var adc = await _repo.GetDepthChart(_awayTeam.Id);
            var hdc = await _repo.GetDepthChart(_homeTeam.Id);
            _awayDepth = (List<DepthChart>)adc;
            _homeDepth = (List<DepthChart>)hdc;

            return Ok(true);
        }

        public async Task<IActionResult> GetTeamStrategies()
        {
            _homeStrategy = await _repo.GetTeamStrategies(_homeTeam.Id);
            _awayStrategy = await _repo.GetTeamStrategies(_awayTeam.Id);

            if (_homeStrategy != null)
            {
                // Set offensive settings
                switch (_homeStrategy.OffensiveStrategyId)
                {
                    case 2:
                        _homeFoulsDrawnStrategy = _homeFoulsDrawnStrategy + 10;
                        _homeTwoTendancyStrategy = _homeTwoTendancyStrategy + 30;
                        _homeThreeTendancyStrategy = _homeThreeTendancyStrategy + -30;
                        _homeTurnoversStrategy = _homeTurnoversStrategy + 3;
                        _awayStealStrategy = _awayStealStrategy + 5;
                        break;
                    case 4:
                        _homeORebStrategy = _homeORebStrategy + 10;
                        _homeDRPMStrategy = _homeDRPMStrategy + -50;
                        break;
                    case 7:
                        _homeSpeedStrategy = _homeSpeedStrategy + 25;
                        _homeORPMStrategy = _homeORPMStrategy + 30;
                        _awayORPMStrategy = _awayORPMStrategy + 10;
                        _homeTurnoversStrategy = _homeTurnoversStrategy + 2;
                        _awayStealStrategy = _awayStealStrategy + 2;
                        break;
                    case 5:
                        _homeTwoPercentageStrategy = _homeTwoPercentageStrategy - 20;
                        _homeSpeedStrategy = _homeSpeedStrategy + 20;
                        _homeThreePercentageStrategy = _homeThreePercentageStrategy - 10;
                        _homeAssistStrategy = _homeAssistStrategy + 15;
                        break;
                    case 6:
                        _homeAssistStrategy = _homeAssistStrategy - 10;
                        _homeTwoTendancyStrategy = _homeTwoTendancyStrategy + 20;
                        _homeThreeTendancyStrategy = _homeThreeTendancyStrategy + 10;
                        _homeFoulsDrawnStrategy = _homeFoulsDrawnStrategy + 5;
                        break;
                    default:
                        break;
                }

                switch (_homeStrategy.DefensiveStrategyId)
                {
                    case 2:
                        _homeDRebStrategy = _homeDRebStrategy + -10;
                        _awayTwoPercentageStrategy = _awayTwoPercentageStrategy + -30;
                        _awayThreePercentageStrategy = _awayThreePercentageStrategy + 10;
                        _awayORebStrategy = _awayORebStrategy + 10;
                        break;
                    case 3:
                        _homeStealStrategy = _homeStealStrategy + 5;
                        _awayFoulsDrawnStrategy = _awayFoulsDrawnStrategy + 15;
                        break;
                    case 4:
                        _homeDRebStrategy = _homeDRebStrategy + 50;
                        _awaySpeedStrategy = _awaySpeedStrategy - 20;
                        break;
                    case 5:
                        _homeSpeedStrategy = _homeSpeedStrategy - 35;
                        _homeDRebStrategy = _homeDRebStrategy + 30;
                        _homeStaminaStrategy = _homeStaminaStrategy + 20;
                        _awayStaminaStrategy = _awayStaminaStrategy + 20;
                        break;
                    case 6:
                        _awayTwoPercentageStrategy = _awayTwoPercentageStrategy + 20;
                        _awayThreePercentageStrategy = _awayThreePercentageStrategy + 20;
                        _homeBlockStrategy = _homeBlockStrategy + 10;
                        _awayFoulsDrawnStrategy = _awayFoulsDrawnStrategy + 15;
                        break;
                    default:
                        break;
                }
            }

            if (_awayStrategy != null)
            {
                switch (_awayStrategy.OffensiveStrategyId)
                {
                    case 2:
                        _awayFoulsDrawnStrategy = _awayFoulsDrawnStrategy + 10;
                        _awayTwoTendancyStrategy = _awayTwoTendancyStrategy + 30;
                        _awayThreeTendancyStrategy = _awayThreeTendancyStrategy + -30;
                        _awayTurnoversStrategy = _awayTurnoversStrategy + 3;
                        _homeStealStrategy = _homeStealStrategy + 5;
                        break;
                    case 4:
                        _awayORebStrategy = _awayORebStrategy + 10;
                        _awayDRPMStrategy = _awayDRPMStrategy + -50;
                        break;
                    case 7:
                        _awaySpeedStrategy = _awaySpeedStrategy + 25;
                        _awayORPMStrategy = _awayORPMStrategy + 30;
                        _homeORPMStrategy = _homeORPMStrategy + 10;
                        _awayTurnoversStrategy = _awayTurnoversStrategy + 2;
                        _homeStealStrategy = _homeStealStrategy + 2;
                        break;
                    case 5:
                        _awayTwoPercentageStrategy = _awayTwoPercentageStrategy - 20;
                        _awaySpeedStrategy = _awaySpeedStrategy + 20;
                        _awayThreePercentageStrategy = _awayThreePercentageStrategy - 10;
                        _awayAssistStrategy = _awayAssistStrategy + 15;
                        break;
                    case 6:
                        _awayAssistStrategy = _awayAssistStrategy - 10;
                        _awayTwoTendancyStrategy = _awayTwoTendancyStrategy + 20;
                        _awayThreeTendancyStrategy = _awayThreeTendancyStrategy + 10;
                        _awayFoulsDrawnStrategy = _awayFoulsDrawnStrategy + 5;
                        break;
                    default:
                        break;
                }

                switch (_awayStrategy.DefensiveStrategyId)
                {
                    case 2:
                        _awayDRebStrategy = _awayDRebStrategy + -10;
                        _homeTwoPercentageStrategy = _homeTwoPercentageStrategy + -30;
                        _homeThreePercentageStrategy = _homeThreePercentageStrategy + 10;
                        _homeORebStrategy = _homeORebStrategy + 10;
                        break;
                    case 3:
                        _awayStealStrategy = _awayStealStrategy + 5;
                        _homeFoulsDrawnStrategy = _homeFoulsDrawnStrategy + 15;
                        break;
                    case 4:
                        _awayDRebStrategy = _awayDRebStrategy + 50;
                        _homeSpeedStrategy = _homeSpeedStrategy - 20;
                        break;
                    case 5:
                        _homeSpeedStrategy = _homeSpeedStrategy - 35;
                        _homeDRebStrategy = _homeDRebStrategy + 30;
                        _homeStaminaStrategy = _homeStaminaStrategy + 20;
                        _awayStaminaStrategy = _awayStaminaStrategy + 20;
                        break;
                    case 6:
                        _homeTwoPercentageStrategy = _homeTwoPercentageStrategy + 20;
                        _homeThreePercentageStrategy = _homeThreePercentageStrategy + 20;
                        _awayBlockStrategy = _awayBlockStrategy + 10;
                        _homeFoulsDrawnStrategy = _homeFoulsDrawnStrategy + 15;
                        break;
                    default:
                        break;
                }
            }

            return Ok(true);
        }

        public async Task<IActionResult> GetCoachSettings()
        {
            var settings = await _repo.GetCoachSettings(_homeTeam.Id);
            _homeSettings = (List<CoachSetting>)settings;

            settings = await _repo.GetCoachSettings(_awayTeam.Id);
            _awaySettings = (List<CoachSetting>)settings;

            // Now need to setup the variables for go to players
            if (_homeSettings.Count != 0)
            {
                homeGoToOne = _homeSettings[0].GoToPlayerOne;
                homeGoToTwo = _homeSettings[0].GoToPlayerTwo;
                homeGoToThree = _homeSettings[0].GoToPlayerThree;
            }

            if (_awaySettings.Count != 0)
            {
                awayGoToOne = _awaySettings[0].GoToPlayerOne;
                awayGoToTwo = _awaySettings[0].GoToPlayerTwo;
                awayGoToThree = _awaySettings[0].GoToPlayerThree;
            }
            return Ok(true);
        }

        public async Task<IActionResult> GetPlayerInjuries()
        {
            for (int i = 0; i < _awayRoster.Count; i++)
            {
                var injury = await _repo.GetPlayerInjury(_awayRoster[i].PlayerId);
                if (injury != null)
                {
                    InjuryDto dto = new InjuryDto
                    {
                        Id = injury.Id,
                        InjuryTypeName = injury.Type,
                        Severity = injury.Severity,
                        PlayerId = injury.PlayerId,
                        Impact = 2,
                        StaminaImpact = 0,
                        StartQuarterImpact = 1,
                        EndQuarterImpact = 4,
                        StartTimeImpact = 720,
                        EndTimeImpact = 0
                    };
                    _awayInjuries.Add(dto);
                }
            }

            for (int i = 0; i < _homeRoster.Count; i++)
            {
                var injury = await _repo.GetPlayerInjury(_homeRoster[i].PlayerId);
                if (injury != null)
                {
                    InjuryDto dto = new InjuryDto
                    {
                        Id = injury.Id,
                        InjuryTypeName = injury.Type,
                        Severity = injury.Severity,
                        PlayerId = injury.PlayerId,
                        Impact = 2,
                        StaminaImpact = 0,
                        StartQuarterImpact = 1,
                        EndQuarterImpact = 4,
                        StartTimeImpact = 720,
                        EndTimeImpact = 0
                    };
                    _homeInjuries.Add(dto);
                }
            }
            return Ok(true);
        }

        public async Task<IActionResult> GetPlayerDetails()
        {
            // Need to get the players and ratings for each member of the roster
            for (int i = 0; i < _awayRoster.Count; i++)
            {
                // Player
                var player = await _repo.GetPlayer(_awayRoster[i].PlayerId);
                _awayPlayers.Add(player);

                // Player Rating
                var playerRating = await _repo.GetPlayerRating(_awayRoster[i].PlayerId);
                _awayRatings.Add(playerRating);

                var playerTendancy = await _repo.GetPlayerTendancy(_awayRoster[i].PlayerId);
                _awayTendancies.Add(playerTendancy);

                // Set up the BoxScore
                BoxScore box = new BoxScore
                {
                    Id = player.Id,
                    ScheduleId = 0,
                    TeamId = _awayRoster[i].TeamId,
                    FirstName = player.FirstName,
                    LastName = player.Surname,
                    Minutes = 0,
                    Points = 0,
                    Rebounds = 0,
                    Assists = 0,
                    Steals = 0,
                    Blocks = 0,
                    BlockedAttempts = 0,
                    FGM = 0,
                    FGA = 0,
                    ThreeFGA = 0,
                    ThreeFGM = 0,
                    FTM = 0,
                    FTA = 0,
                    ORebs = 0,
                    DRebs = 0,
                    Fouls = 0,
                    PlusMinus = 0
                };
                _awayBoxScores.Add(box);

                // Set up the Stamina object
                StaminaTrack staminaTracking = new StaminaTrack
                {
                    PlayerId = _awayRoster[i].PlayerId,
                    OnOff = 0,
                    StaminaTime = 0,
                    StaminaValue = 0
                };
                _awayStaminas.Add(staminaTracking);
            }

            for (int i = 0; i < _homeRoster.Count; i++)
            {
                // Player
                var player = await _repo.GetPlayer(_homeRoster[i].PlayerId);
                _homePlayers.Add(player);

                // Player Rating
                var playerRating = await _repo.GetPlayerRating(_homeRoster[i].PlayerId);
                _homeRatings.Add(playerRating);

                var playerTendancy = await _repo.GetPlayerTendancy(_homeRoster[i].PlayerId);
                _homeTendancies.Add(playerTendancy);

                // Set up the BoxScore
                BoxScore box = new BoxScore
                {
                    Id = player.Id,
                    ScheduleId = 0,
                    TeamId = _homeRoster[i].TeamId,
                    FirstName = player.FirstName,
                    LastName = player.Surname,
                    Minutes = 0,
                    Points = 0,
                    Rebounds = 0,
                    Assists = 0,
                    Steals = 0,
                    Blocks = 0,
                    BlockedAttempts = 0,
                    FGM = 0,
                    FGA = 0,
                    ThreeFGA = 0,
                    ThreeFGM = 0,
                    FTM = 0,
                    FTA = 0,
                    ORebs = 0,
                    DRebs = 0,
                    Fouls = 0,
                    PlusMinus = 0
                };
                _homeBoxScores.Add(box);

                // Set up the Stamina object
                StaminaTrack staminaTracking = new StaminaTrack
                {
                    PlayerId = _homeRoster[i].PlayerId,
                    OnOff = 0,
                    StaminaTime = 0,
                    StaminaValue = 0
                };
                _homeStaminas.Add(staminaTracking);
            }
            return Ok(true);
        }

        public void SetStartingLineups()
        {

            var ad = _awayDepth.FindAll(x => x.Depth == 1);
            var ad2 = _awayDepth.FindAll(x => x.Depth == 2);
            var ad3 = _awayDepth.FindAll(x => x.Depth == 3);
            var hd = _homeDepth.FindAll(x => x.Depth == 1);
            var hd2 = _homeDepth.FindAll(x => x.Depth == 2);
            var hd3 = _homeDepth.FindAll(x => x.Depth == 3);

            for (int i = 0; i < ad.Count; i++)
            {
                int index;
                int onCourt = 0;
                bool injured = false;
                bool canPlay = true;

                switch (ad[i].Position)
                {
                    case 1:
                        onCourt = CheckIfPlayerIsOnCourt(1, ad[i].PlayerId);
                        injured = _awayInjuries.Exists(x => x.PlayerId == ad[i].PlayerId);
                        if (injured)
                        {
                            var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }

                        if (canPlay && onCourt == 0)
                        {
                            awayPG = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                            awayPGRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                            awayPGTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);
                        }
                        else
                        {
                            // Now need to check the next tier
                            // ad2 = _awayDepth.FindAll(x => x.Depth == 2);
                            onCourt = CheckIfPlayerIsOnCourt(1, ad2[i].PlayerId);
                            injured = _awayInjuries.Exists(x => x.PlayerId == ad2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }

                            if (canPlay && onCourt == 0)
                            {
                                awayPG = _awayPlayers.Find(x => x.Id == ad2[i].PlayerId);
                                awayPGRatings = _awayRatings.Find(x => x.PlayerId == ad2[i].PlayerId);
                                awayPGTendancy = _awayTendancies.Find(x => x.PlayerId == ad2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(1, ad3[i].PlayerId);
                                injured = _awayInjuries.Exists(x => x.PlayerId == ad3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    awayPG = _awayPlayers.Find(x => x.Id == ad3[i].PlayerId);
                                    awayPGRatings = _awayRatings.Find(x => x.PlayerId == ad3[i].PlayerId);
                                    awayPGTendancy = _awayTendancies.Find(x => x.PlayerId == ad3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    awayPG = FindPlayerToBeSubbedMandatory(0, 1, 1);
                                    awayPGRatings = _awayRatings.Find(x => x.PlayerId == awayPG.Id);
                                    awayPGTendancy = _awayTendancies.Find(x => x.PlayerId == awayPG.Id);
                                }
                            }
                        }

                        StaminaTrack awatSTPG = _awayStaminas.Find(x => x.PlayerId == awayPG.Id);
                        awatSTPG.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awatSTPG.PlayerId);
                        _awayStaminas[index] = awatSTPG;
                        break;
                    case 2:
                        onCourt = CheckIfPlayerIsOnCourt(1, ad[i].PlayerId);
                        injured = _awayInjuries.Exists(x => x.PlayerId == ad[i].PlayerId);
                        if (injured)
                        {
                            var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            awaySG = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                            awaySGRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                            awaySGTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(1, ad2[i].PlayerId);
                            injured = _awayInjuries.Exists(x => x.PlayerId == ad2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                awaySG = _awayPlayers.Find(x => x.Id == ad2[i].PlayerId);
                                awaySGRatings = _awayRatings.Find(x => x.PlayerId == ad2[i].PlayerId);
                                awaySGTendancy = _awayTendancies.Find(x => x.PlayerId == ad2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(1, ad3[i].PlayerId);
                                injured = _awayInjuries.Exists(x => x.PlayerId == ad3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    awaySG = _awayPlayers.Find(x => x.Id == ad3[i].PlayerId);
                                    awaySGRatings = _awayRatings.Find(x => x.PlayerId == ad3[i].PlayerId);
                                    awaySGTendancy = _awayTendancies.Find(x => x.PlayerId == ad3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    awaySG = FindPlayerToBeSubbedMandatory(0, 1, 2);
                                    awaySGRatings = _awayRatings.Find(x => x.PlayerId == awaySG.Id);
                                    awaySGTendancy = _awayTendancies.Find(x => x.PlayerId == awaySG.Id);
                                }
                            }
                        }

                        StaminaTrack awaySTSG = _awayStaminas.Find(x => x.PlayerId == awaySG.Id);
                        awaySTSG.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awaySTSG.PlayerId);
                        _awayStaminas[index] = awaySTSG;
                        break;
                    case 3:
                        onCourt = CheckIfPlayerIsOnCourt(1, ad[i].PlayerId);
                        injured = _awayInjuries.Exists(x => x.PlayerId == ad[i].PlayerId);
                        if (injured)
                        {
                            var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            awaySF = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                            awaySFRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                            awaySFTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(1, ad2[i].PlayerId);
                            injured = _awayInjuries.Exists(x => x.PlayerId == ad2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                awaySF = _awayPlayers.Find(x => x.Id == ad2[i].PlayerId);
                                awaySFRatings = _awayRatings.Find(x => x.PlayerId == ad2[i].PlayerId);
                                awaySFTendancy = _awayTendancies.Find(x => x.PlayerId == ad2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(1, ad3[i].PlayerId);
                                injured = _awayInjuries.Exists(x => x.PlayerId == ad3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    awaySF = _awayPlayers.Find(x => x.Id == ad3[i].PlayerId);
                                    awaySFRatings = _awayRatings.Find(x => x.PlayerId == ad3[i].PlayerId);
                                    awaySFTendancy = _awayTendancies.Find(x => x.PlayerId == ad3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    awaySF = FindPlayerToBeSubbedMandatory(0, 1, 3);
                                    awaySFRatings = _awayRatings.Find(x => x.PlayerId == awaySF.Id);
                                    awaySFTendancy = _awayTendancies.Find(x => x.PlayerId == awaySF.Id);
                                }
                            }

                        }

                        StaminaTrack awaySTSF = _awayStaminas.Find(x => x.PlayerId == awaySF.Id);
                        awaySTSF.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awaySTSF.PlayerId);
                        _awayStaminas[index] = awaySTSF;
                        break;
                    case 4:
                        onCourt = CheckIfPlayerIsOnCourt(1, ad[i].PlayerId);
                        injured = _awayInjuries.Exists(x => x.PlayerId == ad[i].PlayerId);
                        if (injured)
                        {
                            var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            awayPF = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                            awayPFRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                            awayPFTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(1, ad2[i].PlayerId);
                            injured = _awayInjuries.Exists(x => x.PlayerId == ad2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                awayPF = _awayPlayers.Find(x => x.Id == ad2[i].PlayerId);
                                awayPFRatings = _awayRatings.Find(x => x.PlayerId == ad2[i].PlayerId);
                                awayPFTendancy = _awayTendancies.Find(x => x.PlayerId == ad2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(1, ad3[i].PlayerId);
                                injured = _awayInjuries.Exists(x => x.PlayerId == ad3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    awayPF = _awayPlayers.Find(x => x.Id == ad3[i].PlayerId);
                                    awayPFRatings = _awayRatings.Find(x => x.PlayerId == ad3[i].PlayerId);
                                    awayPFTendancy = _awayTendancies.Find(x => x.PlayerId == ad3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    awayPF = FindPlayerToBeSubbedMandatory(0, 1, 4);
                                    awayPFRatings = _awayRatings.Find(x => x.PlayerId == awayPF.Id);
                                    awayPFTendancy = _awayTendancies.Find(x => x.PlayerId == awayPF.Id);
                                }
                            }

                        }

                        StaminaTrack awaySTPF = _awayStaminas.Find(x => x.PlayerId == awayPF.Id);
                        awaySTPF.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awaySTPF.PlayerId);
                        _awayStaminas[index] = awaySTPF;
                        break;
                    case 5:
                        onCourt = CheckIfPlayerIsOnCourt(1, ad[i].PlayerId);
                        injured = _awayInjuries.Exists(x => x.PlayerId == ad[i].PlayerId);
                        if (injured)
                        {
                            var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            awayC = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                            awayCRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                            awayCTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(1, ad2[i].PlayerId);
                            injured = _awayInjuries.Exists(x => x.PlayerId == ad2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                awayC = _awayPlayers.Find(x => x.Id == ad2[i].PlayerId);
                                awayCRatings = _awayRatings.Find(x => x.PlayerId == ad2[i].PlayerId);
                                awayCTendancy = _awayTendancies.Find(x => x.PlayerId == ad2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(1, ad3[i].PlayerId);
                                injured = _awayInjuries.Exists(x => x.PlayerId == ad3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _awayInjuries.FirstOrDefault(x => x.PlayerId == ad3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    awayC = _awayPlayers.Find(x => x.Id == ad3[i].PlayerId);
                                    awayCRatings = _awayRatings.Find(x => x.PlayerId == ad3[i].PlayerId);
                                    awayCTendancy = _awayTendancies.Find(x => x.PlayerId == ad3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    awayC = FindPlayerToBeSubbedMandatory(0, 1, 5);
                                    awayCRatings = _awayRatings.Find(x => x.PlayerId == awayC.Id);
                                    awayCTendancy = _awayTendancies.Find(x => x.PlayerId == awayC.Id);
                                }
                            }

                        }

                        StaminaTrack awaySTC = _awayStaminas.Find(x => x.PlayerId == awayC.Id);
                        awaySTC.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awaySTC.PlayerId);
                        _awayStaminas[index] = awaySTC;
                        break;
                    default:
                        break;
                }
            }

            for (int i = 0; i < hd.Count; i++)
            {
                int index;
                int onCourt = 0;
                bool injured = false;
                bool canPlay = true;

                switch (hd[i].Position)
                {
                    case 1:
                        onCourt = CheckIfPlayerIsOnCourt(0, hd[i].PlayerId);
                        injured = _homeInjuries.Exists(x => x.PlayerId == hd[i].PlayerId);
                        if (injured)
                        {
                            var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            homePG = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                            homePGRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                            homePGTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(0, hd2[i].PlayerId);
                            injured = _homeInjuries.Exists(x => x.PlayerId == hd2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                homePG = _homePlayers.Find(x => x.Id == hd2[i].PlayerId);
                                homePGRatings = _homeRatings.Find(x => x.PlayerId == hd2[i].PlayerId);
                                homePGTendancy = _homeTendancies.Find(x => x.PlayerId == hd2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(0, hd3[i].PlayerId);
                                injured = _homeInjuries.Exists(x => x.PlayerId == hd3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    homePG = _homePlayers.Find(x => x.Id == hd3[i].PlayerId);
                                    homePGRatings = _homeRatings.Find(x => x.PlayerId == hd3[i].PlayerId);
                                    homePGTendancy = _homeTendancies.Find(x => x.PlayerId == hd3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    homePG = FindPlayerToBeSubbedMandatory(0, 0, 1);
                                    homePGRatings = _homeRatings.Find(x => x.PlayerId == homePG.Id);
                                    homePGTendancy = _homeTendancies.Find(x => x.PlayerId == homePG.Id);
                                }
                            }

                        }

                        StaminaTrack homeSTPG = _homeStaminas.Find(x => x.PlayerId == homePG.Id);
                        homeSTPG.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTPG.PlayerId);
                        _homeStaminas[index] = homeSTPG;
                        break;
                    case 2:
                        onCourt = CheckIfPlayerIsOnCourt(0, hd[i].PlayerId);
                        injured = _homeInjuries.Exists(x => x.PlayerId == hd[i].PlayerId);
                        if (injured)
                        {
                            var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            homeSG = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                            homeSGRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                            homeSGTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(0, hd2[i].PlayerId);
                            injured = _homeInjuries.Exists(x => x.PlayerId == hd2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                homeSG = _homePlayers.Find(x => x.Id == hd2[i].PlayerId);
                                homeSGRatings = _homeRatings.Find(x => x.PlayerId == hd2[i].PlayerId);
                                homeSGTendancy = _homeTendancies.Find(x => x.PlayerId == hd2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(0, hd3[i].PlayerId);
                                injured = _homeInjuries.Exists(x => x.PlayerId == hd3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    homeSG = _homePlayers.Find(x => x.Id == hd3[i].PlayerId);
                                    homeSGRatings = _homeRatings.Find(x => x.PlayerId == hd3[i].PlayerId);
                                    homeSGTendancy = _homeTendancies.Find(x => x.PlayerId == hd3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    homeSG = FindPlayerToBeSubbedMandatory(0, 0, 2);
                                    homeSGRatings = _homeRatings.Find(x => x.PlayerId == homeSG.Id);
                                    homeSGTendancy = _homeTendancies.Find(x => x.PlayerId == homeSG.Id);
                                }
                            }

                        }

                        StaminaTrack homeSTSG = _homeStaminas.Find(x => x.PlayerId == homeSG.Id);
                        homeSTSG.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTSG.PlayerId);
                        _homeStaminas[index] = homeSTSG;
                        break;
                    case 3:
                        onCourt = CheckIfPlayerIsOnCourt(0, hd[i].PlayerId);
                        injured = _homeInjuries.Exists(x => x.PlayerId == hd[i].PlayerId);
                        if (injured)
                        {
                            var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            homeSF = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                            homeSFRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                            homeSFTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(0, hd2[i].PlayerId);
                            injured = _homeInjuries.Exists(x => x.PlayerId == hd2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                homeSF = _homePlayers.Find(x => x.Id == hd2[i].PlayerId);
                                homeSFRatings = _homeRatings.Find(x => x.PlayerId == hd2[i].PlayerId);
                                homeSFTendancy = _homeTendancies.Find(x => x.PlayerId == hd2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(0, hd3[i].PlayerId);
                                injured = _homeInjuries.Exists(x => x.PlayerId == hd3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    homeSF = _homePlayers.Find(x => x.Id == hd3[i].PlayerId);
                                    homeSFRatings = _homeRatings.Find(x => x.PlayerId == hd3[i].PlayerId);
                                    homeSFTendancy = _homeTendancies.Find(x => x.PlayerId == hd3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    homeSF = FindPlayerToBeSubbedMandatory(0, 0, 3);
                                    homeSFRatings = _homeRatings.Find(x => x.PlayerId == homeSF.Id);
                                    homeSFTendancy = _homeTendancies.Find(x => x.PlayerId == homeSF.Id);
                                }
                            }

                        }

                        StaminaTrack homeSTSF = _homeStaminas.Find(x => x.PlayerId == homeSF.Id);
                        homeSTSF.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTSF.PlayerId);
                        _homeStaminas[index] = homeSTSF;
                        break;
                    case 4:
                        onCourt = CheckIfPlayerIsOnCourt(0, hd[i].PlayerId);
                        injured = _homeInjuries.Exists(x => x.PlayerId == hd[i].PlayerId);
                        if (injured)
                        {
                            var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            homePF = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                            homePFRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                            homePFTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(0, hd2[i].PlayerId);
                            injured = _homeInjuries.Exists(x => x.PlayerId == hd2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                homePF = _homePlayers.Find(x => x.Id == hd2[i].PlayerId);
                                homePFRatings = _homeRatings.Find(x => x.PlayerId == hd2[i].PlayerId);
                                homePFTendancy = _homeTendancies.Find(x => x.PlayerId == hd2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(0, hd3[i].PlayerId);
                                injured = _homeInjuries.Exists(x => x.PlayerId == hd3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    homePF = _homePlayers.Find(x => x.Id == hd3[i].PlayerId);
                                    homePFRatings = _homeRatings.Find(x => x.PlayerId == hd3[i].PlayerId);
                                    homePFTendancy = _homeTendancies.Find(x => x.PlayerId == hd3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    homePF = FindPlayerToBeSubbedMandatory(0, 0, 4);
                                    homePFRatings = _homeRatings.Find(x => x.PlayerId == homePF.Id);
                                    homePFTendancy = _homeTendancies.Find(x => x.PlayerId == homePF.Id);
                                }
                            }

                        }

                        StaminaTrack homeSTPF = _homeStaminas.Find(x => x.PlayerId == homePF.Id);
                        homeSTPF.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTPF.PlayerId);
                        _homeStaminas[index] = homeSTPF;
                        break;
                    case 5:
                        onCourt = CheckIfPlayerIsOnCourt(0, hd[i].PlayerId);
                        injured = _homeInjuries.Exists(x => x.PlayerId == hd[i].PlayerId);
                        if (injured)
                        {
                            var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd[i].PlayerId);
                            if (injury.Severity > 2)
                            {
                                canPlay = false;
                            }
                        }
                        if (canPlay && onCourt == 0)
                        {
                            homeC = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                            homeCRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                            homeCTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);
                        }
                        else
                        {
                            onCourt = CheckIfPlayerIsOnCourt(0, hd2[i].PlayerId);
                            injured = _homeInjuries.Exists(x => x.PlayerId == hd2[i].PlayerId);
                            if (injured)
                            {
                                var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd2[i].PlayerId);
                                if (injury.Severity > 2)
                                {
                                    canPlay = false;
                                }
                            }
                            if (canPlay && onCourt == 0)
                            {
                                homeC = _homePlayers.Find(x => x.Id == hd2[i].PlayerId);
                                homeCRatings = _homeRatings.Find(x => x.PlayerId == hd2[i].PlayerId);
                                homeCTendancy = _homeTendancies.Find(x => x.PlayerId == hd2[i].PlayerId);
                            }
                            else
                            {
                                onCourt = CheckIfPlayerIsOnCourt(0, hd3[i].PlayerId);
                                injured = _homeInjuries.Exists(x => x.PlayerId == hd3[i].PlayerId);
                                if (injured)
                                {
                                    var injury = _homeInjuries.FirstOrDefault(x => x.PlayerId == hd3[i].PlayerId);
                                    if (injury.Severity > 2)
                                    {
                                        canPlay = false;
                                    }
                                }

                                if (canPlay && onCourt == 0)
                                {
                                    homeC = _homePlayers.Find(x => x.Id == hd3[i].PlayerId);
                                    homeCRatings = _homeRatings.Find(x => x.PlayerId == hd3[i].PlayerId);
                                    homeCTendancy = _homeTendancies.Find(x => x.PlayerId == hd3[i].PlayerId);
                                }
                                else
                                {
                                    // Need to find anyone who can play
                                    homeC = FindPlayerToBeSubbedMandatory(0, 0, 5);
                                    homeCRatings = _homeRatings.Find(x => x.PlayerId == homeC.Id);
                                    homeCTendancy = _homeTendancies.Find(x => x.PlayerId == homeC.Id);
                                }
                            }

                        }

                        StaminaTrack homeSTC = _homeStaminas.Find(x => x.PlayerId == homeC.Id);
                        homeSTC.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTC.PlayerId);
                        _homeStaminas[index] = homeSTC;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Jumpball()
        {
            SimTeamDto winningTeam;
            int result = _random.Next(1, 101);

            if (result % 2 == 0)
            {
                // Home Team wins the jumpball
                _teamPossession = 0;
                winningTeam = _homeTeam;
            }
            else
            {
                // Away Team wins the jumpball
                _teamPossession = 1;
                winningTeam = _awayTeam;
            }

            int j = _random.Next(1, 101);

            _playerPossession = new Jumpball().GetJumpPlayer(j);
            string currentPlayerName = GetCurrentPlayerFullName();

            // Need to do the commentary
            // commentaryData.Add(comm.GetJumpballCommentary(winningTeam, _quarter, _time, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            // PlayByPlayTracker(comm.GetJumpballCommentary(winningTeam, _quarter, _time, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

            string awayJump = awayC.FirstName + " " + awayC.Surname;
            string homeJump = homeC.FirstName + " " + homeC.Surname;
            commentaryData.Add(comm.GetJumpballCommentary(_quarter, _time, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, awayJump, homeJump, currentPlayerName));
            PlayByPlayTracker(comm.GetJumpballCommentary(_quarter, _time, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, awayJump, homeJump, currentPlayerName), 0);

            int timeValue = _random.Next(1, 5);

            // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
            if (timeValue > _time || timeValue > _shotClock)
            {
                // This action causes time to expire
                if (timeValue > _time)
                {
                    timeValue = _time;
                }
                else if (timeValue > _shotClock)
                {
                    timeValue = _shotClock;
                }
            }

            _shotClock = _shotClock - timeValue;
            _time = _time - timeValue;
            timeCounter = timeCounter + timeValue;
            StaminaUpdates(timeValue);
            UpdateTimeInBoxScore(timeValue);
        }

        public void RunQuarter()
        {
            _homeFoulBonus = 0;
            _awayFoulBonus = 0;
            _awayFinal2Bonus = 0;
            _homeFinal2Bonus = 0;

            while (_time > 0)
            {
                if (_shotClock > 0)
                {
                    int decision = GetPlayerDecision();

                    switch (decision)
                    {
                        case 1:
                            // Player has passed the ball
                            Pass();
                            break;
                        case 2:
                            TwoPointShot();
                            break;
                        case 3:
                            ThreePointShot();
                            break;
                        case 4:
                            // player has been fouled
                            if (_time > 0)
                                PlayerFouled();
                            break;
                        case 5:
                            PlayerTurnover();

                            if (_time > 0)
                                Inbounds();
                            break;
                        case 6:
                            // Ball has been stolen - already handled
                            break;
                        case 7:
                            // Player has held onto the ball
                            _time = _time - 1;
                            timeCounter = timeCounter + 1;
                            StaminaUpdates(1);
                            UpdateTimeInBoxScores(1);
                            // commentaryData.Add(comm.GetHoldBallCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                            // PlayByPlayTracker(comm.GetHoldBallCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ShotClockViolation();
                }
            }
            EndOfQuarter();
        }

        public void RunOvertime()
        {
            _homeFoulBonus = 0;
            _awayFoulBonus = 0;
            _homeFinal2Bonus = 0;
            _awayFinal2Bonus = 0;
            _time = 300;
            _quarter++;

            while (_time > 0)
            {
                if (_shotClock > 0)
                {
                    int decision = GetPlayerDecision();

                    switch (decision)
                    {
                        case 1:
                            // Player has passed the ball
                            Pass();
                            break;
                        case 2:
                            TwoPointShot();
                            break;
                        case 3:
                            ThreePointShot();
                            break;
                        case 4:
                            // player has been fouled
                            if (_time > 0)
                                PlayerFouled();
                            break;
                        case 5:
                            PlayerTurnover();

                            if (_time > 0)
                                Inbounds();
                            break;
                        case 6:
                            // Ball has been stolen - already handled
                            break;
                        case 7:
                            // Player has held onto the ball
                            _time = _time - 1;
                            timeCounter = timeCounter + 1;
                            StaminaUpdates(1);
                            UpdateTimeInBoxScores(1);
                            commentaryData.Add(comm.GetHoldBallCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                            PlayByPlayTracker(comm.GetHoldBallCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    ShotClockViolation();
                }
            }
            EndOfQuarter();
        }

        // Player Decisions
        public int GetPlayerDecision()
        {
            int decision = 0;
            int shotClockBonus = 0;

            int foulsDrawnStrategy = 0;
            int twoTendancyStrategy = 0;
            int threeTendancyStrategy = 0;
            int turnoverStrategy = 0;

            if (_teamPossession == 0)
            {
                foulsDrawnStrategy = _homeFoulsDrawnStrategy;
                twoTendancyStrategy = _homeTwoTendancyStrategy;
                threeTendancyStrategy = _homeThreeTendancyStrategy;
                turnoverStrategy = _homeTurnoversStrategy;
            }
            else
            {
                foulsDrawnStrategy = _awayFoulsDrawnStrategy;
                twoTendancyStrategy = _awayTwoTendancyStrategy;
                threeTendancyStrategy = _awayThreeTendancyStrategy;
                turnoverStrategy = _awayTurnoversStrategy;
            }

            if (_quarter > 3 && _time <= 48)
            {
                _endGameFoulAddition = 0;
                _endGameResultIncrease = 0;
                _endGameShotClockBonus = 0;
                _endGameStealAddition = 0;
                _endGameThreePointAddition = 0;
                _endGameTwoPointAddition = 0;

                EndGameShootBonus();
            }
            else
            {
                _endGameFoulAddition = 0;
                _endGameResultIncrease = 0;
                _endGameShotClockBonus = 0;
                _endGameStealAddition = 0;
                _endGameThreePointAddition = 0;
                _endGameTwoPointAddition = 0;

                shotClockBonus = ShotClockShootBonus();
            }

            // Need to check whether a steal has occurred
            int isSteal = StealCheck();

            if (isSteal == 0)
            {
                // Now need to get the current players tendancies
                PlayerTendancy tendancy = GetCurrentPlayersTendancies();

                if (tendancy.PlayerId == 203)
                {
                    string s = "";
                }

                int foulBonusValue = 0;
                if (_quarter > 3 && _time <= 48 && _endGameFoulAddition != 0)
                {
                    foulBonusValue = _endGameFoulAddition;
                }
                else
                {
                    foulBonusValue = (int)(tendancy.FouledTendancy * 0.1);
                }

                int threeTendancy = tendancy.ThreePointTendancy + threeTendancyStrategy;
                int twoTendancy = tendancy.TwoPointTendancy + twoTendancyStrategy;
                int passTendancy = tendancy.PassTendancy;
                if (_quarter > 3 && _time <= 48 && _endGameThreePointAddition != 0)
                {
                    threeTendancy = threeTendancy + _endGameThreePointAddition;

                    if (twoTendancy < _endGameThreePointAddition)
                    {
                        twoTendancy = 0;
                        int remainder = _endGameThreePointAddition - threeTendancy;
                        passTendancy = tendancy.PassTendancy - remainder;
                    }
                    else
                    {
                        twoTendancy = twoTendancy - _endGameThreePointAddition;
                    }
                }

                if (_quarter > 3 && _time <= 48 && _endGameTwoPointAddition != 0)
                {
                    twoTendancy = twoTendancy + _endGameTwoPointAddition;

                    if (tendancy.PassTendancy < _endGameTwoPointAddition)
                    {
                        passTendancy = 0;
                        int remainder = _endGameTwoPointAddition - tendancy.PassTendancy;
                        twoTendancy = twoTendancy - remainder;
                    }
                    else
                    {
                        passTendancy = tendancy.PassTendancy - _endGameTwoPointAddition;
                    }
                }

                int fouledTendancy = tendancy.FouledTendancy + foulsDrawnStrategy;
                int turnoverTendancy = tendancy.TurnoverTendancy + turnoverStrategy;

                int twoSection = twoTendancy;
                int threeSection = (twoTendancy + threeTendancy);
                int foulSection = (twoTendancy + threeTendancy + fouledTendancy + foulBonusValue);
                int turnoverSection = (twoTendancy + threeTendancy + fouledTendancy + foulBonusValue + turnoverTendancy);

                if (shotClockBonus > 0 || _endGameShotClockBonus > 0)
                {
                    if (_quarter > 3 && _time <= 48)
                    {
                        shotClockBonus = _endGameShotClockBonus;
                    }
                    // Then we have some bonus to work out
                    int total = twoTendancy + threeTendancy + fouledTendancy + foulBonusValue + turnoverTendancy;

                    double twoUpgrade = (double)twoTendancy / total;
                    double threeUpgrade = (double)threeTendancy / total;
                    double foulUpgrade = (double)(fouledTendancy + foulBonusValue) / total;
                    double turnoverUpgrade = (double)turnoverTendancy / total;

                    int twoPointBonus = (int)(shotClockBonus * twoUpgrade);
                    int threePointBonus = (int)(shotClockBonus * threeUpgrade);
                    int foulBonus = (int)(shotClockBonus * foulUpgrade);
                    int turnoverBonus = (int)(shotClockBonus * turnoverUpgrade);

                    twoSection = twoTendancy + twoPointBonus;
                    threeSection = threeTendancy + threePointBonus + twoSection;
                    foulSection = fouledTendancy + foulBonusValue + foulBonus + threeSection;
                    turnoverSection = turnoverTendancy + turnoverBonus + foulSection;
                }
                else
                {
                    if (shotClockBonus != 0 || _endGameShotClockBonus != 0)
                    {
                        if (_quarter > 3 && _time <= 48)
                        {
                            shotClockBonus = _endGameShotClockBonus;
                        }

                        int total = twoTendancy + threeTendancy;

                        double twoUpgrade = (double)twoTendancy / total;
                        double threeUpgrade = (double)threeTendancy / total;

                        int twoPointBonus = (int)(shotClockBonus * twoUpgrade);
                        int threePointBonus = (int)(shotClockBonus * threeUpgrade);

                        twoSection = twoTendancy + twoPointBonus;
                        threeSection = threeTendancy + threePointBonus + twoSection;
                        foulSection = fouledTendancy + foulBonusValue + threeSection;
                        turnoverSection = turnoverTendancy + foulSection;
                    }
                }

                int value = 1000 + foulBonusValue;
                if (turnoverSection > 1000)
                {
                    value = turnoverSection + 10;
                }
                int result = _random.Next(1, value + 1);

                if (result <= twoSection)
                {
                    // Shooting a 2
                    decision = 2;
                }
                else if (result > twoSection && result <= threeSection)
                {
                    // Shooting a 3
                    decision = 3;
                }
                else if (result > threeSection && result <= foulSection)
                {
                    // The player has been fouled potentially
                    int check = _random.Next(1, 101);
                    if (check < 70)
                    {
                        // The player has been fouled
                        decision = 4;
                    }
                    else
                    {
                        decision = 7; // Player has had to hold onto the ball
                    }
                }
                else if (result > foulSection && result <= turnoverSection)
                {
                    // The player has potentially fouled
                    int check = _random.Next(1, 101);
                    if (check < 35)
                    {
                        // The player has turned the ball over
                        decision = 5;
                    }
                    else
                    {
                        decision = 7; // Player has had to hold onto the ball
                    }
                }
                else
                {
                    // The player has passed the ball
                    decision = 1;
                }
            }
            else
            {
                // The ball has been stolen
                stealCounter++;
                decision = 6;
            }
            return decision;
        }

        // Game Actions
        public void Pass()
        {
            int totalUsage = 0;
            string receiver = "";
            string passer = "";

            int homePGUsageRating = 0;
            int homeSGUsageRating = 0;
            int homeSFUsageRating = 0;
            int homePFUsageRating = 0;
            int homeCUsageRating = 0;
            int awayPGUsageRating = 0;
            int awaySGUsageRating = 0;
            int awaySFUsageRating = 0;
            int awayPFUsageRating = 0;
            int awayCUsageRating = 0;

            int pgBonus = 0;
            int sgBonus = 0;
            int sfBonus = 0;
            int pfBonus = 0;
            int cBonus = 0;

            // Check 
            if (_teamPossession == 0)
            {
                // Need to check the go to player bonus
                if (homePG.Id == _homeSettings[0].GoToPlayerOne)
                {
                    pgBonus = 30;
                }
                else if (homePG.Id == _homeSettings[0].GoToPlayerTwo)
                {
                    pgBonus = 20;
                }
                else if (homePG.Id == _homeSettings[0].GoToPlayerThree)
                {
                    pgBonus = 10;
                }

                if (homeSG.Id == _homeSettings[0].GoToPlayerOne)
                {
                    sgBonus = 30;
                }
                else if (homeSG.Id == _homeSettings[0].GoToPlayerTwo)
                {
                    sgBonus = 20;
                }
                else if (homeSG.Id == _homeSettings[0].GoToPlayerThree)
                {
                    sgBonus = 10;
                }

                if (homeSF.Id == _homeSettings[0].GoToPlayerOne)
                {
                    sfBonus = 30;
                }
                else if (homeSF.Id == _homeSettings[0].GoToPlayerTwo)
                {
                    sfBonus = 20;
                }
                else if (homeSF.Id == _homeSettings[0].GoToPlayerThree)
                {
                    sfBonus = 10;
                }

                if (homePF.Id == _homeSettings[0].GoToPlayerOne)
                {
                    pfBonus = 30;
                }
                else if (homePF.Id == _homeSettings[0].GoToPlayerTwo)
                {
                    pfBonus = 20;
                }
                else if (homePF.Id == _homeSettings[0].GoToPlayerThree)
                {
                    pfBonus = 10;
                }

                if (homeC.Id == _homeSettings[0].GoToPlayerOne)
                {
                    cBonus = 30;
                }
                else if (homeC.Id == _homeSettings[0].GoToPlayerTwo)
                {
                    cBonus = 20;
                }
                else if (homeC.Id == _homeSettings[0].GoToPlayerThree)
                {
                    cBonus = 10;
                }

                homePGUsageRating = homePGRatings.UsageRating + pgBonus;
                homeSGUsageRating = homeSGRatings.UsageRating + sgBonus;
                homeSFUsageRating = homeSFRatings.UsageRating + sfBonus;
                homePFUsageRating = homePFRatings.UsageRating + pfBonus;
                homeCUsageRating = homeCRatings.UsageRating + cBonus;

                switch (_playerPossession)
                {
                    case 1:
                        totalUsage = homeSGUsageRating + homeSFUsageRating + homePFUsageRating + homeCUsageRating;
                        break;
                    case 2:
                        totalUsage = homePGUsageRating + homeSFUsageRating + homePFUsageRating + homeCUsageRating;
                        break;
                    case 3:
                        totalUsage = homePGUsageRating + homeSGUsageRating + homePFUsageRating + homeCUsageRating;
                        break;
                    case 4:
                        totalUsage = homePGUsageRating + homeSGUsageRating + homeSFUsageRating + homeCUsageRating;
                        break;
                    case 5:
                        totalUsage = homePGUsageRating + homeSFUsageRating + homePFUsageRating + homeSGUsageRating;
                        break;
                    default:
                        break;
                }

                switch (_playerPossession)
                {
                    case 1:
                        _playerPassed = homePG;
                        _playerRatingPassed = homePGRatings;
                        break;
                    case 2:
                        _playerPassed = homeSG;
                        _playerRatingPassed = homeSGRatings;
                        break;
                    case 3:
                        _playerPassed = homeSF;
                        _playerRatingPassed = homeSFRatings;
                        break;
                    case 4:
                        _playerPassed = homePF;
                        _playerRatingPassed = homePFRatings;
                        break;
                    case 5:
                        _playerPassed = homeC;
                        _playerRatingPassed = homeCRatings;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Need to check the go to player bonus
                if (awayPG.Id == _awaySettings[0].GoToPlayerOne)
                {
                    pgBonus = 30;
                }
                else if (awayPG.Id == _awaySettings[0].GoToPlayerTwo)
                {
                    pgBonus = 20;
                }
                else if (awayPG.Id == _awaySettings[0].GoToPlayerThree)
                {
                    pgBonus = 10;
                }

                if (awaySG.Id == _awaySettings[0].GoToPlayerOne)
                {
                    sgBonus = 30;
                }
                else if (awaySG.Id == _awaySettings[0].GoToPlayerTwo)
                {
                    sgBonus = 20;
                }
                else if (awaySG.Id == _awaySettings[0].GoToPlayerThree)
                {
                    sgBonus = 10;
                }

                if (awaySF.Id == _awaySettings[0].GoToPlayerOne)
                {
                    sfBonus = 30;
                }
                else if (awaySF.Id == _awaySettings[0].GoToPlayerTwo)
                {
                    sfBonus = 20;
                }
                else if (awaySF.Id == _awaySettings[0].GoToPlayerThree)
                {
                    sfBonus = 10;
                }

                if (awayPF.Id == _awaySettings[0].GoToPlayerOne)
                {
                    pfBonus = 30;
                }
                else if (awayPF.Id == _awaySettings[0].GoToPlayerTwo)
                {
                    pfBonus = 20;
                }
                else if (awayPF.Id == _awaySettings[0].GoToPlayerThree)
                {
                    pfBonus = 10;
                }

                if (awayC.Id == _awaySettings[0].GoToPlayerOne)
                {
                    cBonus = 30;
                }
                else if (awayC.Id == _awaySettings[0].GoToPlayerTwo)
                {
                    cBonus = 20;
                }
                else if (awayC.Id == _awaySettings[0].GoToPlayerThree)
                {
                    cBonus = 10;
                }

                awayPGUsageRating = awayPGRatings.UsageRating + pgBonus;
                awaySGUsageRating = awaySGRatings.UsageRating + sgBonus;
                awaySFUsageRating = awaySFRatings.UsageRating + sfBonus;
                awayPFUsageRating = awayPFRatings.UsageRating + pfBonus;
                awayCUsageRating = awayCRatings.UsageRating + cBonus;

                switch (_playerPossession)
                {
                    case 1:
                        totalUsage = awaySGUsageRating + awaySFUsageRating + awayPFUsageRating + awayCUsageRating;
                        break;
                    case 2:
                        totalUsage = awayPGUsageRating + awaySFUsageRating + awayPFUsageRating + awayCUsageRating;
                        break;
                    case 3:
                        totalUsage = awayPGUsageRating + awaySGUsageRating + awayPFUsageRating + awayCUsageRating;
                        break;
                    case 4:
                        totalUsage = awayPGUsageRating + awaySGUsageRating + awaySFUsageRating + awayCUsageRating;
                        break;
                    case 5:
                        totalUsage = awayPGUsageRating + awaySGUsageRating + awaySFUsageRating + awayPFUsageRating;
                        break;
                    default:
                        break;
                }

                switch (_playerPossession)
                {
                    case 1:
                        _playerPassed = awayPG;
                        _playerRatingPassed = awayPGRatings;
                        break;
                    case 2:
                        _playerPassed = awaySG;
                        _playerRatingPassed = awaySGRatings;
                        break;
                    case 3:
                        _playerPassed = awaySF;
                        _playerRatingPassed = awaySFRatings;
                        break;
                    case 4:
                        _playerPassed = awayPF;
                        _playerRatingPassed = awayPFRatings;
                        break;
                    case 5:
                        _playerPassed = awayC;
                        _playerRatingPassed = awayCRatings;
                        break;
                    default:
                        break;
                }
            }

            // Now need to generate the result of who receives the ball
            int result = _random.Next(1, totalUsage + 1);

            if (_teamPossession == 0)
            {
                switch (_playerPossession)
                {
                    case 1:
                        if (result < homeSGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            // Home SG receives the ball
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homeSGUsageRating && result < (homeSGUsageRating + homeSFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homeSGUsageRating + homeSFUsageRating) && result < (homeSGUsageRating + homeSFUsageRating + homePFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homeSGUsageRating + homeSFUsageRating + homePFUsageRating) && result < (homeSGUsageRating + homeSFUsageRating + homePFUsageRating + homeCUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 2:
                        if (result < homePGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            // Home PG receives the ball
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homePGUsageRating && result < (homePGUsageRating + homeSFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGUsageRating + homeSFUsageRating) && result < (homePGUsageRating + homeSFUsageRating + homePFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGUsageRating + homeSFUsageRating + homePFUsageRating) && result < (homePGUsageRating + homeSFUsageRating + homePFUsageRating + homeCUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 3:
                        if (result < homePGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            // Home PG receives the ball
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homePGUsageRating && result < (homePGUsageRating + homeSGUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGUsageRating + homeSGUsageRating) && result < (homePGUsageRating + homeSGUsageRating + homePFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGUsageRating + homeSGUsageRating + homePFUsageRating) && result < (homePGUsageRating + homeSGUsageRating + homePFUsageRating + homeCUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 4:
                        if (result < homePGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            // Home SG receives the ball
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homePGUsageRating && result < (homePGUsageRating + homeSGUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGUsageRating + homeSGUsageRating) && result < (homePGUsageRating + homeSGUsageRating + homeSFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGUsageRating + homeSGUsageRating + homeSFUsageRating) && result < (homePGUsageRating + homeSGUsageRating + homeSFUsageRating + homeCUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 5:
                        if (result < homePGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homePGUsageRating && result < (homePGUsageRating + homeSGUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGUsageRating + homeSGUsageRating) && result < (homePGUsageRating + homeSGUsageRating + homeSFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGUsageRating + homeSGUsageRating + homeSFUsageRating) && result < (homePGUsageRating + homeSGUsageRating + homeSFUsageRating + homePFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (_playerPossession)
                {
                    case 1:
                        if (result < awayPGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awaySGUsageRating && result < (awaySGUsageRating + awaySFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awaySGUsageRating + awaySFUsageRating) && result < (awaySGUsageRating + awaySFUsageRating + awayPFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awaySGUsageRating + awaySFUsageRating + awayPFUsageRating) && result < (awaySGUsageRating + awaySFUsageRating + awayPFUsageRating + awayCUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 2:
                        if (result < awayPGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awayPGUsageRating && result < (awayPGUsageRating + awaySFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGUsageRating + awaySFUsageRating) & result < (awayPGUsageRating + awaySFUsageRating + awayPFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGUsageRating + awaySFUsageRating + awayPFUsageRating) & result < (awayPGUsageRating + awaySFUsageRating + awayPFUsageRating + awayCUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 3:
                        if (result < awayPGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awayPGUsageRating && result < (awayPGUsageRating + awaySGUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGUsageRating + awaySGUsageRating) & result < (awayPGUsageRating + awaySGUsageRating + awayPFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGUsageRating + awaySGUsageRating + awayPFUsageRating) && result < (awayPGUsageRating + awaySGUsageRating + awayPFUsageRating + awayCUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 4:
                        if (result < awayPGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awayPGUsageRating && result < (awayPGUsageRating + awaySGUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGUsageRating + awaySGUsageRating) && result < (awayPGUsageRating + awaySGUsageRating + awaySFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGUsageRating + awaySGUsageRating + awaySFUsageRating) && result < (awayPGUsageRating + awaySGUsageRating + awaySFUsageRating + awayCUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 5:
                        if (result < awayPGUsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awayPGUsageRating && result < (awayPGUsageRating + awaySGUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGUsageRating + awaySGUsageRating) && result < (awayPGUsageRating + awaySGUsageRating + awaySFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGUsageRating + awaySGUsageRating + awaySFUsageRating) && result < (awayPGUsageRating + awaySGUsageRating + awaySFUsageRating + awayPFUsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    default:
                        break;
                }
            }

            // Now need the timer - to determine how long the play to the pass took
            int timeValue = _timer.GetPassTime();

            // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
            if (timeValue > _time || timeValue > _shotClock)
            {
                // This action causes time to expire somewhere
                if (timeValue > _time)
                {
                    timeValue = _time;
                    _shotClock = _shotClock - timeValue;
                    _time = _time - timeValue;
                    timeCounter = timeCounter + timeValue;

                    // UPDATE THE TIME IN THE BOX SCORE OBJECTS
                    UpdateTimeInBoxScores(timeValue);

                    // UPDATE THE STAMINA
                    StaminaUpdates(timeValue);
                }
                else if (timeValue > _shotClock)
                {
                    timeValue = _shotClock;
                    _shotClock = _shotClock - timeValue;
                    _time = _time - timeValue;
                    timeCounter = timeCounter + timeValue;

                    // UPDATE THE TIME IN THE BOX SCORE OBJECTS
                    UpdateTimeInBoxScores(timeValue);

                    // UPDATE THE STAMINA
                    StaminaUpdates(timeValue);

                    // Call ShotClockViolation
                    ShotClockViolation();
                }
            }
            else
            {
                _shotClock = _shotClock - timeValue;
                _time = _time - timeValue;
                timeCounter = timeCounter + timeValue;

                // UPDATE THE TIME IN THE BOX SCORE OBJECTS
                UpdateTimeInBoxScores(timeValue);

                // UPDATE THE STAMINA
                StaminaUpdates(timeValue);

                // COMMENTARY
                // commentaryData.Add(comm.GetPassCommentary(passer, receiver, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                // PlayByPlayTracker(comm.GetPassCommentary(passer, receiver, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                // Console.WriteLine(comm.GetPassCommentary(passer, receiver, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            }
        }

        public void TwoPointShot()
        {
            twosTaken++;
            int possibleAssist = 0;
            string assistingPlayingName = "";
            if (_playerPassed != null)
            {
                assistingPlayingName = _playerPassed.FirstName + " " + _playerPassed.Surname;
            }

            // Need to see if the shot was blocked
            int blockResult = BlockCheck();

            if (blockResult == 0)
            {
                PlayerRating currentRating = GetCurrentPlayersRatings();
                int result = _random.Next(1, 1001);

                int twoPercStrategy = 0;
                if (_teamPossession == 0)
                {
                    if (_homeTwoPercentageStrategy != 0)
                    {
                        twoPercStrategy = _homeTwoPercentageStrategy * -1;
                    }
                }
                else
                {
                    if (_awayTwoPercentageStrategy != 0)
                    {
                        twoPercStrategy = _awayTwoPercentageStrategy * -1;
                    }
                }

                int twoRating = StaminaEffect(currentRating.PlayerId, _teamPossession, currentRating.TwoRating);
                twoRating = twoRating + twoPercStrategy;

                // Need to Apply ORPM to result - effecting quality of the shot
                int orpmValue = GetOrpmValue(_teamPossession);

                // Need to Apply DRPM to result
                int defence = -1;
                if (_teamPossession == 0)
                {
                    defence = 1;
                }
                else
                {
                    defence = 0;
                }
                int drpmValue = GetDrpmValue(defence);

                result = result - ((orpmValue - drpmValue) / 2);
                result = result + _endGameResultIncrease;

                result = result - 20;

                // Need to determine whether an assist chance has been created
                if (_playerRatingPassed != null)
                {
                    int assistRating = (_playerRatingPassed.AssitRating * 5);

                    int assistStrategy = 0;
                    if (_teamPossession == 0)
                    {
                        assistStrategy = _homeAssistStrategy;
                    }
                    else
                    {
                        assistStrategy = _awayAssistStrategy;
                    }
                    assistRating = assistRating + assistStrategy;

                    int assistResult = _random.Next(0, 1000);

                    if (assistResult <= assistRating)
                    {
                        assistCounterChance++;
                        possibleAssist = 1;
                        // update the result
                        result = result - 30;
                    }
                    else
                    {
                        possibleAssist = 0;
                    }
                }

                // Home shooting bonus
                if (_teamPossession == 0)
                {
                    double value = currentRating.TwoRating * 0.025;
                    int bonus = (int)value;
                    twoRating = twoRating + bonus;
                }

                if (twoRating >= result)
                {
                    // Show was made
                    int timeValue = _timer.GetTwoShotTime();

                    // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
                    if (timeValue > _time || timeValue > _shotClock)
                    {
                        // This action causes time to expire
                        if (timeValue > _time)
                        {
                            timeValue = _time;
                        }
                        else if (timeValue > _shotClock)
                        {
                            timeValue = _shotClock;
                        }
                    }

                    _shotClock = 24;
                    _time = _time - timeValue;
                    timeCounter = timeCounter + timeValue;
                    StaminaUpdates(timeValue);
                    UpdateTimeInBoxScore(timeValue);

                    int commPoints = 0;
                    int commAsts = 0;

                    // Update Score
                    if (_teamPossession == 0)
                    {
                        _homeScore = _homeScore + 2;
                        UpdatePlusMinusBoxScore(2);

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.FGM++;
                        temp.Points = temp.Points + 2;
                        int index = _homeBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _homeBoxScores[index] = temp;

                        commPoints = temp.Points;

                        if (possibleAssist == 1)
                        {
                            // Update the Box Score
                            BoxScore tempAst = _homeBoxScores.Find(x => x.Id == _playerPassed.Id);
                            tempAst.Assists++;
                            int indexAst = _homeBoxScores.FindIndex(x => x.Id == _playerPassed.Id);
                            _homeBoxScores[indexAst] = tempAst;
                            commAsts = tempAst.Assists;
                        }
                    }
                    else
                    {
                        _awayScore = _awayScore + 2;
                        UpdatePlusMinusBoxScore(2);

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.FGM++;
                        temp.Points = temp.Points + 2;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _awayBoxScores[index] = temp;

                        commPoints = temp.Points;

                        if (possibleAssist == 1)
                        {
                            // Update the Box Score
                            BoxScore tempAst = _awayBoxScores.Find(x => x.Id == _playerPassed.Id);
                            tempAst.Assists++;
                            int indexAst = _awayBoxScores.FindIndex(x => x.Id == _playerPassed.Id);
                            _awayBoxScores[indexAst] = tempAst;
                            commAsts = tempAst.Assists;
                        }
                    }

                    // Comm
                    commentaryData.Add(comm.GetTwoPointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName, commPoints, commAsts));
                    // Console.WriteLine(comm.GetTwoPointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName));
                    PlayByPlayTracker(comm.GetTwoPointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName, commPoints, commAsts), 0);

                    if (_teamPossession == 0)
                    {
                        _teamPossession = 1;
                    }
                    else
                    {
                        _teamPossession = 0;
                    }

                    Inbounds();
                }
                else
                {
                    // Shot missed
                    int timeValue = _timer.GetTwoShotTime();

                    // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
                    if (timeValue > _time || timeValue > _shotClock)
                    {
                        // This action causes time to expire
                        if (timeValue > _time)
                        {
                            timeValue = _time;
                        }
                        else if (timeValue > _shotClock)
                        {
                            timeValue = _shotClock;
                        }
                    }

                    _time = _time - timeValue;
                    timeCounter = timeCounter + timeValue;
                    StaminaUpdates(timeValue);
                    UpdateTimeInBoxScore(timeValue);

                    if (_teamPossession == 0)
                    {
                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _homeBoxScores[index] = temp;
                    }
                    else
                    {
                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _awayBoxScores[index] = temp;
                    }

                    // Commentary
                    commentaryData.Add(comm.GetTwoPointMissCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetTwoPointMissCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetTwoPointMissCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

                    // Go to the Rebound
                    Rebound();
                }
            }
            else
            {
                // Shot has been blocked
                blockCounter++;
                // TODO - need to handle the fact the shot clock would not reset on offensive rebound here
                Rebound();
            }
        }

        public void ThreePointShot()
        {
            threesTaken++;
            int possibleAssist = 0;
            string assistingPlayingName = "";
            if (_playerPassed != null)
            {
                assistingPlayingName = _playerPassed.FirstName + " " + _playerPassed.Surname;
            }

            // Need to see if the shot was blocked
            int blockResult = BlockCheck();

            if (blockResult == 0)
            {
                PlayerRating currentRating = GetCurrentPlayersRatings();
                int result = _random.Next(1, 1001);

                int threePercStrategy = 0;
                if (_teamPossession == 0)
                {
                    if (_homeThreePercentageStrategy != 0)
                    {
                        threePercStrategy = _homeThreePercentageStrategy * -1;
                    }
                }
                else
                {
                    if (_awayThreePercentageStrategy != 0)
                    {
                        threePercStrategy = _awayThreePercentageStrategy * -1;
                    }
                }

                int threeRating = StaminaEffect(currentRating.PlayerId, _teamPossession, currentRating.ThreeRating);
                threeRating = threeRating + threePercStrategy;

                // Need to Apply ORPM to result - effecting quality of the shot
                int orpmValue = GetOrpmValue(_teamPossession);

                // Need to Apply DRPM to result
                int defence = -1;
                if (_teamPossession == 0)
                {
                    defence = 1;
                }
                else
                {
                    defence = 0;
                }
                int drpmValue = GetDrpmValue(defence);

                result = result - ((orpmValue - drpmValue) / 2);
                result = result + _endGameResultIncrease;
                //result = result - 10;

                // Need to determine whether an assist chance has been created
                if (_playerRatingPassed != null)
                {
                    int assistRating = (_playerRatingPassed.AssitRating * 5); // Factor applied to increase the low Assist to Pass rate for low pass counts in sim

                    int assistStrategy = 0;
                    if (_teamPossession == 0)
                    {
                        assistStrategy = _homeAssistStrategy;
                    }
                    else
                    {
                        assistStrategy = _awayAssistStrategy;
                    }
                    assistRating = assistRating + assistStrategy;

                    int assistResult = _random.Next(0, 1000);

                    if (assistResult <= assistRating)
                    {
                        assistCounterChance++;
                        possibleAssist = 1;
                        // update the result
                        result = result - 20;
                    }
                    else
                    {
                        possibleAssist = 0;
                    }
                }

                // Home shooting bonus
                if (_teamPossession == 0)
                {
                    double value = currentRating.TwoRating * 0.025;
                    int bonus = (int)value;
                    threeRating = threeRating + bonus;
                }

                if (threeRating >= result)
                {
                    // Show was made
                    int timeValue = _timer.GetThreeShotTime();

                    // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
                    if (timeValue > _time || timeValue > _shotClock)
                    {
                        // This action causes time to expire
                        if (timeValue > _time)
                        {
                            timeValue = _time;
                        }
                        else if (timeValue > _shotClock)
                        {
                            timeValue = _shotClock;
                        }
                    }

                    _shotClock = 24;
                    _time = _time - timeValue;
                    timeCounter = timeCounter + timeValue;

                    StaminaUpdates(timeValue);
                    UpdateTimeInBoxScore(timeValue);

                    int commPoints = 0;
                    int commAsts = 0;

                    // Update Score
                    if (_teamPossession == 0)
                    {
                        _homeScore = _homeScore + 3;
                        UpdatePlusMinusBoxScore(3);

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.FGM++;
                        temp.ThreeFGA++;
                        temp.ThreeFGM++;
                        temp.Points = temp.Points + 3;
                        int index = _homeBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _homeBoxScores[index] = temp;
                        commPoints = temp.Points;

                        if (possibleAssist == 1)
                        {
                            assistCounter++;

                            // Update the Box Score
                            BoxScore temp2 = _homeBoxScores.Find(x => x.Id == _playerPassed.Id);
                            temp2.Assists++;
                            int index2 = _homeBoxScores.FindIndex(x => x.Id == _playerPassed.Id);
                            _homeBoxScores[index2] = temp2;
                            commAsts = temp.Assists;
                        }

                    }
                    else
                    {
                        _awayScore = _awayScore + 3;
                        UpdatePlusMinusBoxScore(3);

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.FGM++;
                        temp.ThreeFGA++;
                        temp.ThreeFGM++;
                        temp.Points = temp.Points + 3;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _awayBoxScores[index] = temp;
                        commPoints = temp.Points;

                        if (possibleAssist == 1)
                        {
                            assistCounter++;

                            // Update the Box Score
                            BoxScore temp2 = _awayBoxScores.Find(x => x.Id == _playerPassed.Id);
                            temp2.Assists++;
                            int index2 = _awayBoxScores.FindIndex(x => x.Id == _playerPassed.Id);
                            _awayBoxScores[index2] = temp2;
                            commAsts = temp.Assists;
                        }
                    }

                    // Comm
                    commentaryData.Add(comm.GetThreePointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName, commPoints, commAsts));
                    // Console.WriteLine(comm.GetThreePointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName));
                    PlayByPlayTracker(comm.GetThreePointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName, commPoints, commAsts), 0);

                    if (_teamPossession == 0)
                    {
                        _teamPossession = 1;
                    }
                    else
                    {
                        _teamPossession = 0;
                    }

                    Inbounds();
                }
                else
                {
                    // Shot missed
                    int timeValue = _timer.GetThreeShotTime();

                    // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
                    if (timeValue > _time || timeValue > _shotClock)
                    {
                        // This action causes time to expire
                        if (timeValue > _time)
                        {
                            timeValue = _time;
                        }
                        else if (timeValue > _shotClock)
                        {
                            timeValue = _shotClock;
                        }
                    }

                    _time = _time - timeValue;
                    timeCounter = timeCounter + timeValue;

                    StaminaUpdates(timeValue);
                    UpdateTimeInBoxScore(timeValue);

                    if (_teamPossession == 0)
                    {
                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.ThreeFGA++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _homeBoxScores[index] = temp;
                    }
                    else
                    {
                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.ThreeFGA++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _awayBoxScores[index] = temp;
                    }

                    // Commentary
                    commentaryData.Add(comm.GetThreePointMissCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetThreePointMissCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetThreePointMissCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

                    // Go to the Rebound
                    Rebound();
                }
            }
            else
            {
                // Shot has been blocked
                blockCounter++;
                // TODO - need to handle the fact the shot clock would not reset on offensive rebound here
                Rebound();
            }
        }

        public int BlockCheck()
        {
            PlayerRating currentRating = GetCurrentPlayersRatings();
            int blockStrategy = 0;
            int blockComm = 0;

            if (_teamPossession == 0)
            {
                blockStrategy = _awayBlockStrategy;

                // Away team is trying to block the shot
                List<PlayerRating> awayRatings = new List<PlayerRating>();
                awayRatings.Add(awayPGRatings);
                awayRatings.Add(awaySGRatings);
                awayRatings.Add(awaySFRatings);
                awayRatings.Add(awayPFRatings);
                awayRatings.Add(awayCRatings);

                List<PlayerRating> awayRatingsSorted = new List<PlayerRating>();
                awayRatingsSorted = awayRatings.OrderByDescending(x => x.BlockRating).ToList();

                for (int i = 0; i < awayRatingsSorted.Count; i++)
                {
                    PlayerRating checking = awayRatingsSorted[i];
                    int blockRating = checking.BlockRating + blockStrategy;
                    int rating = StaminaEffect(checking.PlayerId, 1, blockRating);
                    int result = _random.Next(1, 1451);

                    if (result <= rating)
                    {
                        // Update the timer
                        int timeValue = _random.Next(2, 6);

                        // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
                        if (timeValue > _time || timeValue > _shotClock)
                        {
                            // This action causes time to expire
                            if (timeValue > _time)
                            {
                                timeValue = _time;
                            }
                            else if (timeValue > _shotClock)
                            {
                                timeValue = _shotClock;
                            }
                        }

                        // Updates
                        // Get correct display of time
                        _time = _time - timeValue;
                        timeCounter = timeCounter + timeValue;
                        _shotClock = _shotClock - timeValue;

                        UpdateTimeInBoxScore(timeValue);
                        StaminaUpdates(timeValue);

                        // Box Score - both block and the shooter
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.BlockedAttempts++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _homeBoxScores[index] = temp;

                        BoxScore temp2 = _awayBoxScores.Find(x => x.Id == checking.PlayerId);
                        temp2.Blocks++;
                        int index2 = _awayBoxScores.FindIndex(x => x.Id == checking.PlayerId);
                        _awayBoxScores[index2] = temp2;
                        blockComm = temp2.Blocks;

                        // Commentary
                        commentaryData.Add(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, blockComm));
                        // Console.WriteLine(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, blockComm), 0);

                        return 1;
                    }
                }
            }
            else
            {
                blockStrategy = _homeBlockStrategy;

                // Home team is trying to block the shot
                List<PlayerRating> homeRatings = new List<PlayerRating>();
                homeRatings.Add(homePGRatings);
                homeRatings.Add(homeSGRatings);
                homeRatings.Add(homeSFRatings);
                homeRatings.Add(homePFRatings);
                homeRatings.Add(homeCRatings);

                List<PlayerRating> homeRatingsSorted = new List<PlayerRating>();
                homeRatingsSorted = homeRatings.OrderByDescending(x => x.BlockRating).ToList();

                for (int i = 0; i < homeRatingsSorted.Count; i++)
                {
                    PlayerRating checking = homeRatingsSorted[i];
                    int blockRating = checking.BlockRating + blockStrategy;
                    int rating = StaminaEffect(checking.PlayerId, 0, blockRating);
                    int result = _random.Next(1, 1451);

                    if (result <= rating)
                    {
                        // Update the timer
                        int timeValue = _random.Next(2, 6);

                        // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
                        if (timeValue > _time || timeValue > _shotClock)
                        {
                            // This action causes time to expire
                            if (timeValue > _time)
                            {
                                timeValue = _time;
                            }
                            else if (timeValue > _shotClock)
                            {
                                timeValue = _shotClock;
                            }
                        }

                        // Updates
                        // Get correct display of time
                        _time = _time - timeValue;
                        _shotClock = _shotClock - timeValue;
                        timeCounter = timeCounter + timeValue;

                        UpdateTimeInBoxScore(timeValue);
                        StaminaUpdates(timeValue);

                        // Box Score - both block and the shooter
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.BlockedAttempts++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _awayBoxScores[index] = temp;

                        BoxScore temp2 = _homeBoxScores.Find(x => x.Id == checking.PlayerId);
                        temp2.Blocks++;
                        int index2 = _homeBoxScores.FindIndex(x => x.Id == checking.PlayerId);
                        _homeBoxScores[index2] = temp2;
                        blockComm = temp2.Blocks;

                        // Commentary
                        commentaryData.Add(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, blockComm));
                        // Console.WriteLine(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, blockComm), 0);

                        return 1;
                    }
                }
            }
            return 0;
        }

        public void Rebound()
        {
            _playerRatingPassed = null;
            _playerPassed = null;

            int offensiveRate = 0;
            int defensiveRate = 0;

            int offRebStrategy = 0;
            int defRebStrategy = 0;

            // Update the timer
            int timeValue = _random.Next(2, 6);

            // Need to check if the time has not gone past 0 and update the time value appropriately
            if (timeValue > _time || timeValue > _shotClock)
            {
                // This action causes time to expire
                if (timeValue > _time)
                {
                    timeValue = _time;
                }
                else if (timeValue > _shotClock)
                {
                    timeValue = _shotClock;
                }
            }

            // Updates
            // Get correct display of time
            _time = _time - timeValue;
            _shotClock = _shotClock - timeValue;
            timeCounter = timeCounter + timeValue;

            UpdateTimeInBoxScore(timeValue);
            StaminaUpdates(timeValue);

            if (_teamPossession == 0)
            {
                offRebStrategy = _homeORebStrategy;
                defRebStrategy = _awayDRebStrategy;

                int homePGRebound = StaminaEffect(homePGRatings.PlayerId, 0, homePGRatings.ORebRating) + offRebStrategy;
                int homeSGRebound = StaminaEffect(homeSGRatings.PlayerId, 0, homeSGRatings.ORebRating) + offRebStrategy;
                int homeSFRebound = StaminaEffect(homeSFRatings.PlayerId, 0, homeSFRatings.ORebRating) + offRebStrategy;
                int homePFRebound = StaminaEffect(homePFRatings.PlayerId, 0, homePFRatings.ORebRating) + offRebStrategy;
                int homeCRebound = StaminaEffect(homeCRatings.PlayerId, 0, homeCRatings.ORebRating) + offRebStrategy;

                int awayPGRebound = StaminaEffect(awayPGRatings.PlayerId, 1, awayPGRatings.DRebRating) + defRebStrategy;
                int awaySGRebound = StaminaEffect(awaySGRatings.PlayerId, 1, awaySGRatings.DRebRating) + defRebStrategy;
                int awaySFRebound = StaminaEffect(awaySFRatings.PlayerId, 1, awaySFRatings.DRebRating) + defRebStrategy;
                int awayPFRebound = StaminaEffect(awayPFRatings.PlayerId, 1, awayPFRatings.DRebRating) + defRebStrategy;
                int awayCRebound = StaminaEffect(awayCRatings.PlayerId, 1, awayCRatings.DRebRating) + defRebStrategy;

                offensiveRate = homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound + 10;
                defensiveRate = awayPGRebound + awaySGRebound + awaySFRebound + awaySFRebound + awayCRebound + 20;

                // home team shot
                int randValue = offensiveRate + defensiveRate;
                int result = _random.Next(1, randValue + 1);

                // Firstly determine if it is offensive or defensive
                if (result < offensiveRate)
                {
                    // Offensive Rebound
                    _shotClock = 14;

                    int commOReb = 0;
                    int commDReb = 0;

                    if (result < homePGRebound)
                    {
                        _playerPossession = 1;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homePG.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homePG.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= homePGRebound && result < (homePGRebound + homeSGRebound))
                    {
                        _playerPossession = 2;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeSG.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeSG.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (homePGRebound + homeSGRebound) && result < (homePGRebound + homeSGRebound + homeSFRebound))
                    {
                        _playerPossession = 3;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeSF.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeSF.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (homePGRebound + homeSGRebound + homeSFRebound) && result < (homePGRebound + homeSGRebound + homeSFRebound + homePFRebound))
                    {
                        _playerPossession = 4;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homePF.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homePF.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (homePGRebound + homeSGRebound + homeSFRebound + homePFRebound) && result < (homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound))
                    {
                        _playerPossession = 5;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeC.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound) && result < (homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound + 10))
                    {
                        _playerPossession = -1;
                    }

                    if (_playerPossession != -1)
                    {
                        // Commentary for Offensive Rebound
                        commentaryData.Add(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb));
                        // Console.WriteLine(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb), 0);
                    }
                    else
                    {
                        // Commentary for Offensive Rebound
                        commentaryData.Add(comm.GetOffensiveTeamReboundCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb));
                        // Console.WriteLine(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetOffensiveTeamReboundCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb), 0);
                        SubstitutionCheck(0);
                        Inbounds();
                    }

                }
                else
                {
                    // Defensive Rebound
                    _shotClock = 24;
                    int commOReb = 0;
                    int commDReb = 0;

                    if (result < (offensiveRate + awayPGRebound))
                    {
                        _teamPossession = 1;
                        _playerPossession = 1;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayPG.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayPG.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + awayPGRebound) && result < (offensiveRate + awayPGRebound + awaySGRebound))
                    {
                        _teamPossession = 1;
                        _playerPossession = 2;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awaySG.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awaySG.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + awayPGRebound + awaySGRebound) && result < (offensiveRate + awayPGRebound + awaySGRebound + awaySFRebound))
                    {
                        _teamPossession = 1;
                        _playerPossession = 3;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awaySF.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awaySF.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + awayPGRebound + awaySGRebound + awaySFRebound) && result < (offensiveRate + awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound))
                    {
                        _teamPossession = 1;
                        _playerPossession = 4;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayPF.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayPF.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound) && result < (offensiveRate + awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound + awayCRebound))
                    {
                        _teamPossession = 1;
                        _playerPossession = 5;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayC.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound + awayCRebound) && result < (offensiveRate + awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound + awayCRebound + 20))
                    {
                        _playerPossession = -1;
                    }

                    if (_playerPossession != -1)
                    {
                        // Display Defensive Rebound Commentary
                        commentaryData.Add(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb));
                        // Console.WriteLine(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb), 0);
                    }
                    else
                    {
                        // Display Defensive Rebound Commentary
                        commentaryData.Add(comm.GetDefensiveTeamReboundCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb));
                        // Console.WriteLine(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetDefensiveTeamReboundCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb), 0);
                        SubstitutionCheck(0);
                        Inbounds();
                    }
                }
            }
            else
            {
                offRebStrategy = _awayORebStrategy;
                defRebStrategy = _homeDRebStrategy;

                int homePGRebound = StaminaEffect(homePGRatings.PlayerId, 0, homePGRatings.DRebRating) + defRebStrategy;
                int homeSGRebound = StaminaEffect(homeSGRatings.PlayerId, 0, homeSGRatings.DRebRating) + defRebStrategy;
                int homeSFRebound = StaminaEffect(homeSFRatings.PlayerId, 0, homeSFRatings.DRebRating) + defRebStrategy;
                int homePFRebound = StaminaEffect(homePFRatings.PlayerId, 0, homePFRatings.DRebRating) + defRebStrategy;
                int homeCRebound = StaminaEffect(homeCRatings.PlayerId, 0, homeCRatings.DRebRating) + defRebStrategy;

                int awayPGRebound = StaminaEffect(awayPGRatings.PlayerId, 1, awayPGRatings.ORebRating) + offRebStrategy;
                int awaySGRebound = StaminaEffect(awaySGRatings.PlayerId, 1, awaySGRatings.ORebRating) + offRebStrategy;
                int awaySFRebound = StaminaEffect(awaySFRatings.PlayerId, 1, awaySFRatings.ORebRating) + offRebStrategy;
                int awayPFRebound = StaminaEffect(awayPFRatings.PlayerId, 1, awayPFRatings.ORebRating) + offRebStrategy;
                int awayCRebound = StaminaEffect(awayCRatings.PlayerId, 1, awayCRatings.ORebRating) + offRebStrategy;

                defensiveRate = homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound + 20;
                offensiveRate = awayPGRebound + awaySGRebound + awaySFRebound + awaySFRebound + awayCRebound + 10;

                // home team shot
                int randValue = offensiveRate + defensiveRate;
                int result = _random.Next(1, randValue + 1);

                int commOReb = 0;
                int commDReb = 0;

                // Firstly determine if it is offensive or defensive
                if (result < offensiveRate)
                {
                    // Offensive Rebound
                    _shotClock = 14;

                    if (result < awayPGRebound)
                    {
                        _playerPossession = 1;
                        _shotClock = 14;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayPG.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayPG.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= awayPGRebound && result < (awayPGRebound + awaySGRebound))
                    {
                        _playerPossession = 2;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awaySG.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awaySG.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (awayPGRebound + awaySGRebound) && result < (awayPGRebound + awaySGRebound + awaySFRebound))
                    {
                        _playerPossession = 3;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awaySF.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awaySF.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (awayPGRebound + awaySGRebound + awaySFRebound) && result < (awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound))
                    {
                        _playerPossession = 4;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayPF.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayPF.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound) && result < (awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound + awayCRebound))
                    {
                        _playerPossession = 5;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayC.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
                        _awayBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound + awayCRebound) && result < (awayPGRebound + awaySGRebound + awaySFRebound + awayPFRebound + awayCRebound + 10))
                    {
                        _playerPossession = -1;
                    }

                    if (_playerPossession != -1)
                    {
                        // Commentary for Offensive Rebound
                        commentaryData.Add(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb));
                        // Console.WriteLine(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb), 0);
                    }
                    else
                    {
                        // Commentary for Offensive Rebound
                        commentaryData.Add(comm.GetOffensiveTeamReboundCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb));
                        // Console.WriteLine(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetOffensiveTeamReboundCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb), 0);
                        SubstitutionCheck(0);
                        Inbounds();
                    }
                }
                else
                {
                    // Defensive Rebound
                    _shotClock = 24;

                    if (result < (offensiveRate + homePGRebound))
                    {
                        _teamPossession = 0;
                        _playerPossession = 1;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homePG.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homePG.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + homePGRebound) && result < (offensiveRate + homePGRebound + homeSGRebound))
                    {
                        _teamPossession = 0;
                        _playerPossession = 2;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeSG.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeSG.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + homePGRebound + homeSGRebound) && result < (offensiveRate + homePGRebound + homeSGRebound + homeSFRebound))
                    {
                        _teamPossession = 0;
                        _playerPossession = 3;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeSF.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeSF.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + homePGRebound + homeSGRebound + homeSFRebound) && result < (offensiveRate + homePGRebound + homeSGRebound + homeSFRebound + homePFRebound))
                    {
                        _teamPossession = 0;
                        _playerPossession = 4;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homePF.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homePF.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + homePGRebound + homeSGRebound + homeSFRebound + homePFRebound) && result < (offensiveRate + homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound))
                    {
                        _teamPossession = 0;
                        _playerPossession = 5;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeC.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
                        _homeBoxScores[index] = temp;
                        commOReb = temp.ORebs;
                        commDReb = temp.DRebs;
                    }
                    else if (result >= (offensiveRate + homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound) && result < (offensiveRate + homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound + 20))
                    {
                        _playerPossession = -1;
                    }

                    if (_playerPossession != -1)
                    {
                        // Display Defensive Rebound Commentary
                        commentaryData.Add(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb));
                        // Console.WriteLine(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb), 0);
                    }
                    else
                    {
                        // Display Defensive Rebound Commentary
                        commentaryData.Add(comm.GetDefensiveTeamReboundCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb));
                        // Console.WriteLine(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetDefensiveTeamReboundCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commOReb, commDReb), 0);
                        SubstitutionCheck(0);
                        Inbounds();
                    }
                }
            }
        }

        public int StealCheck()
        {
            PlayerRating currentRating = GetCurrentPlayersRatings();
            int stealBonus = 0;
            int stealStrategy = 0;
            int stealComm = 0;

            if (_endGameStealAddition > 0)
            {
                stealBonus = _endGameStealAddition;
            }

            if (_teamPossession == 0)
            {
                // int stealingTeam = 1;
                stealStrategy = _awayStealStrategy;

                // Away team is trying to steal the ball
                List<PlayerRating> awayRatings = new List<PlayerRating>();
                awayRatings.Add(awayPGRatings);
                awayRatings.Add(awaySGRatings);
                awayRatings.Add(awaySFRatings);
                awayRatings.Add(awayPFRatings);
                awayRatings.Add(awayCRatings);

                List<PlayerRating> awayRatingsSorted = new List<PlayerRating>();
                awayRatingsSorted = awayRatings.OrderByDescending(x => x.StealRating).ToList();

                for (int i = 0; i < awayRatingsSorted.Count; i++)
                {
                    PlayerRating checking = awayRatingsSorted[i];
                    int stealRating = checking.StealRating + stealStrategy;
                    int rating = StaminaEffect(checking.PlayerId, 1, stealRating);
                    int result = _random.Next(1, (4151 - stealBonus)); // This is times 5 to account for all 5 players pn the court

                    if (result <= rating)
                    {
                        // Update the timer
                        int timeValue = _random.Next(1, 6);

                        // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
                        if (timeValue > _time || timeValue > _shotClock)
                        {
                            // This action causes time to expire
                            if (timeValue > _time)
                            {
                                timeValue = _time;
                            }
                            else if (timeValue > _shotClock)
                            {
                                timeValue = _shotClock;
                            }
                        }

                        // Updates
                        // Get correct display of time
                        _time = _time - timeValue;
                        _shotClock = _shotClock - timeValue;
                        timeCounter = timeCounter + timeValue;

                        UpdateTimeInBoxScore(timeValue);
                        StaminaUpdates(timeValue);

                        // Box Score - both block and the shooter
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.Turnovers++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _homeBoxScores[index] = temp;

                        BoxScore temp2 = _awayBoxScores.Find(x => x.Id == checking.PlayerId);
                        temp2.Steals++;
                        int index2 = _awayBoxScores.FindIndex(x => x.Id == checking.PlayerId);
                        _awayBoxScores[index2] = temp2;
                        stealComm = temp2.Steals;

                        // Commentary
                        commentaryData.Add(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, stealComm));
                        // Console.WriteLine(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, stealComm), 0);

                        _teamPossession = 1;
                        _playerPossession = _random.Next(1, 6); // not sure how to make this correct

                        _playerPassed = null;
                        _playerRatingPassed = null;

                        return 1;
                    }
                }
            }
            else
            {
                stealStrategy = _homeStealStrategy;

                // Home team is trying to steal the shot
                List<PlayerRating> homeRatings = new List<PlayerRating>();
                homeRatings.Add(homePGRatings);
                homeRatings.Add(homeSGRatings);
                homeRatings.Add(homeSFRatings);
                homeRatings.Add(homePFRatings);
                homeRatings.Add(homeCRatings);

                List<PlayerRating> homeRatingsSorted = new List<PlayerRating>();
                homeRatingsSorted = homeRatings.OrderByDescending(x => x.StealRating).ToList();

                for (int i = 0; i < homeRatingsSorted.Count; i++)
                {
                    PlayerRating checking = homeRatingsSorted[i];
                    int stealRating = checking.StealRating + stealStrategy;
                    int rating = StaminaEffect(checking.PlayerId, 0, stealRating);
                    int result = _random.Next(1, (4151 - stealBonus));

                    if (result <= rating)
                    {
                        // Update the timer
                        int timeValue = _random.Next(1, 6);

                        // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
                        if (timeValue > _time || timeValue > _shotClock)
                        {
                            // This action causes time to expire
                            if (timeValue > _time)
                            {
                                timeValue = _time;
                            }
                            else if (timeValue > _shotClock)
                            {
                                timeValue = _shotClock;
                            }
                        }

                        // Updates
                        // Get correct display of time
                        _time = _time - timeValue;
                        _shotClock = _shotClock - timeValue;
                        timeCounter = timeCounter + timeValue;

                        UpdateTimeInBoxScore(timeValue);
                        StaminaUpdates(timeValue);

                        // Box Score - both block and the shooter
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.Turnovers++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _awayBoxScores[index] = temp;

                        BoxScore temp2 = _homeBoxScores.Find(x => x.Id == checking.PlayerId);
                        temp2.Steals++;
                        int index2 = _homeBoxScores.FindIndex(x => x.Id == checking.PlayerId);
                        _homeBoxScores[index2] = temp2;
                        stealComm = temp2.Steals;

                        // Commentary
                        commentaryData.Add(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, stealComm));
                        // Console.WriteLine(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, stealComm), 0);

                        _teamPossession = 0;
                        _playerPossession = _random.Next(1, 6);

                        _playerPassed = null;
                        _playerRatingPassed = null;

                        return 1;
                    }
                }
            }
            return 0;
        }

        public void PlayerFouled()
        {
            // Setting up the variables
            _playerRatingPassed = null;
            _playerPassed = null;

            // Setup fouling objects
            Player playerFouling = new Player();
            int isFreeThrows = 0;
            int teamWhichFouled = 2;
            int numberOfShots = 0;

            int foulComm = 0;

            // Set the time
            int timeValue = _random.Next(1, 4);

            // Now need to work out how to determine what time is sent - in case this triggers a shot clock or end of quarter action
            if (timeValue > _time || timeValue > _shotClock)
            {
                // This action causes time to expire
                if (timeValue > _time)
                {
                    timeValue = _time;
                }
                else if (timeValue > _shotClock)
                {
                    timeValue = _shotClock;
                }
            }

            // Update what goes game wide
            _time = _time - timeValue;
            _shotClock = _shotClock - timeValue;
            timeCounter = timeCounter + timeValue;

            StaminaUpdates(timeValue);
            UpdateTimeInBoxScore(timeValue);

            if (_shotClock < 14)
            {
                _shotClock = 14;
            }

            // Need to update the player fouling boxscore
            int fouler = PlayerWhoFouled();
            playerFouling = UpdateFouler(fouler);
            string fouling = playerFouling.FirstName + " " + playerFouling.Surname;
            BoxScore bs = new BoxScore();
            if (_teamPossession == 0)
            {
                bs = _awayBoxScores.Find(x => x.FirstName == playerFouling.FirstName && x.LastName == playerFouling.Surname);
                foulComm = bs.Fouls;
            }
            else
            {
                bs = _homeBoxScores.Find(x => x.FirstName == playerFouling.FirstName && x.LastName == playerFouling.Surname);
                foulComm = bs.Fouls;
            }

            int result = _random.Next(1, 20);

            if (result < 13)
            {
                // Non-Shooting Foul
                if (_teamPossession == 0)
                {
                    // Home team has been fouled
                    // Commentary
                    commentaryData.Add(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, foulComm));
                    PlayByPlayTracker(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, foulComm), 1);

                    if (_time <= 120)
                    {
                        _homeFinal2Bonus++;
                    }
                    _homeFoulBonus++;
                    teamWhichFouled = 1;

                    if (_homeFoulBonus > 4 || _homeFinal2Bonus > 1)
                    {
                        // Commentary
                        commentaryData.Add(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                        isFreeThrows = 1;
                        numberOfShots = 2;
                    }
                }
                else
                {
                    // Away Team has been fouled
                    // Commentary
                    commentaryData.Add(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, foulComm));
                    PlayByPlayTracker(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, foulComm), 1);

                    if (_time <= 120)
                    {
                        _awayFinal2Bonus++;
                    }
                    _awayFoulBonus++;
                    teamWhichFouled = 0;

                    if (_awayFoulBonus > 4 || _awayFinal2Bonus > 1)
                    {
                        // Commentary
                        commentaryData.Add(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                        isFreeThrows = 1;
                        numberOfShots = 2;
                    }
                }
            }
            else
            {
                // Shooting Foul
                if (_teamPossession == 0)
                {
                    if (_time <= 120)
                    {
                        _homeFinal2Bonus++;
                    }
                    _homeFoulBonus++;
                }
                else
                {
                    if (_time <= 120)
                    {
                        _awayFinal2Bonus++;
                    }
                    _awayFoulBonus++;
                }

                // Need to determine if it is a 3 or 2
                int shots = _random.Next(1, 11);
                if (shots <= 8)
                {
                    // Commentary
                    commentaryData.Add(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 2, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, foulComm));
                    PlayByPlayTracker(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 2, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, foulComm), 0);

                    // Todo Injury Check
                    // InjuryCheck();
                    // InjuryStaminaChecks();

                    // Need to check if there are any subs to be made
                    // SubCheckFT();
                    // CheckForSubs(1);

                    numberOfShots = 2;
                    isFreeThrows = 1;
                }
                else
                {
                    commentaryData.Add(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 3, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, foulComm));
                    PlayByPlayTracker(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 3, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, foulComm), 1);

                    // InjuryCheck();
                    // Need to check if there are any subs to be made
                    // SubCheckFT();
                    // CheckForSubs(0);

                    numberOfShots = 3;
                    isFreeThrows = 1;
                }
            }

            // // Foul Trouble Check for subs
            // int sub = FoulTroubleCheck(teamWhichFouled, fouler);

            // if (sub == 1)
            // {
            //     string outPlayer = GetPlayerFullNameForPositionForFouler(teamWhichFouled, GetPlayerIdForPosition(teamWhichFouled, fouler));
            //     // Player needs to be subbed out due to foul trouble
            //     Substitution(teamWhichFouled, fouler);

            //     if (subError == 0)
            //     {
            //         // Now need to sort out the sub commentary
            //         string inPlayer = GetPlayerFullNameForPositionForFouler(teamWhichFouled, GetPlayerIdForPosition(teamWhichFouled, fouler));

            //         int teamToDisplay = 0;
            //         if (teamWhichFouled == 1)
            //         {
            //             teamToDisplay = 1;
            //         }

            //         commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, teamToDisplay, _awayTeam.Mascot, _homeTeam.Mascot));
            //         PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, teamToDisplay, _awayTeam.Mascot, _homeTeam.Mascot), 1);
            //     }


            // }

            // Check if it is free throws
            if (isFreeThrows == 1)
            {
                InjuryCheck();
                InjuryStaminaChecks();
                // Need to check if there are any subs to be made
                // SubCheckFT();
                // CheckForSubs(1);
                SubstitutionCheck(1);

                int ftResult = FreeThrows(numberOfShots);
                if (ftResult == 0)
                {
                    // last shot was made
                    if (_teamPossession == 0)
                    {
                        _teamPossession = 1;
                    }
                    else
                    {
                        _teamPossession = 0;
                    }
                    Inbounds();
                }
                else
                {
                    Rebound();
                }
            }
            else
            {
                // Need to check for subs
                InjuryCheck();
                InjuryStaminaChecks();
                // SubCheck();
                // CheckForSubs(0);
                SubstitutionCheck(0);
                Inbounds();
            }
        }

        public int PlayerWhoFouled()
        {
            if (_teamPossession == 0)
            {
                // Then it was away team who fouled
                int awayPGFoulRating = awayPGRatings.FoulingRating;
                int awaySGFoulRating = awaySGRatings.FoulingRating;
                int awaySFFoulRating = awaySFRatings.FoulingRating;
                int awayPFFoulRating = awayPFRatings.FoulingRating;
                int awayCFoulRating = awayCRatings.FoulingRating;

                int totalRating = awayPGFoulRating + awaySGFoulRating + awaySFFoulRating + awayPFFoulRating + awayCFoulRating;

                int fouler = _random.Next(1, totalRating + 1);

                if (fouler < awayPGFoulRating)
                {
                    return 1;
                }
                else if (fouler >= awayPGFoulRating && fouler < (awayPGFoulRating + awaySGFoulRating))
                {
                    return 2;
                }
                else if (fouler >= (awayPGFoulRating + awaySGFoulRating) && fouler < (awayPGFoulRating + awaySGFoulRating + awaySFFoulRating))
                {
                    return 3;
                }
                else if (fouler >= (awayPGFoulRating + awaySGFoulRating + awaySFFoulRating) && fouler < (awayPGFoulRating + awaySGFoulRating + awaySFFoulRating + awayPFFoulRating))
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }
            else
            {
                // Then it was home team who fouled
                int homePGFoulRating = homePGRatings.FoulingRating;
                int homeSGFoulRating = homeSGRatings.FoulingRating;
                int homeSFFoulRating = homeSFRatings.FoulingRating;
                int homePFFoulRating = homePFRatings.FoulingRating;
                int homeCFoulRating = homeCRatings.FoulingRating;

                int totalRating = homePGFoulRating + homeSGFoulRating + homeSFFoulRating + homePFFoulRating + homeCFoulRating;

                int fouler = _random.Next(1, totalRating + 1);

                if (fouler < homePGFoulRating)
                {
                    return 1;
                }
                else if (fouler >= homePGFoulRating && fouler < (homePGFoulRating + homeSGFoulRating))
                {
                    return 2;
                }
                else if (fouler >= (homePGFoulRating + homeSGFoulRating) && fouler < (homePGFoulRating + homeSGFoulRating + homeSFFoulRating))
                {
                    return 3;
                }
                else if (fouler >= (homePGFoulRating + homeSGFoulRating + homeSFFoulRating) && fouler < (homePGFoulRating + homeSGFoulRating + homeSFFoulRating + homePFFoulRating))
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }
        }

        public int FreeThrows(int shots)
        {
            int lastShot = 0;
            PlayerRating currentPlayerRating = GetCurrentPlayersRatings();
            _playerRatingPassed = null;
            _playerPassed = null;

            for (int i = 0; i < shots; i++)
            {
                int result = _random.Next(1, 1001);
                int ftRating = StaminaEffect(currentPlayerRating.PlayerId, _teamPossession, currentPlayerRating.FTRating) + 20;

                if (result <= ftRating)
                {
                    // Free throw is made
                    int commPoints = 0;
                    // box score
                    if (_teamPossession == 0)
                    {
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == currentPlayerRating.PlayerId);
                        temp.Points++;
                        temp.FTM++;
                        temp.FTA++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == currentPlayerRating.PlayerId);
                        _homeBoxScores[index] = temp;
                        _homeScore++;
                        commPoints = temp.Points;
                    }
                    else
                    {
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentPlayerRating.PlayerId);
                        temp.Points++;
                        temp.FTM++;
                        temp.FTA++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentPlayerRating.PlayerId);
                        _awayBoxScores[index] = temp;
                        _awayScore++;
                        commPoints = temp.Points;
                    }
                    UpdatePlusMinusBoxScore(1);

                    // commentary
                    commentaryData.Add(comm.GetMadeFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commPoints));
                    // Console.WriteLine(comm.GetMadeFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetMadeFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, commPoints), 1);
                }
                else
                {
                    // Free throw is missed
                    if (shots - 1 == i)
                    {
                        lastShot = 1;
                    }

                    // commentary
                    commentaryData.Add(comm.GetMissedFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetMissedFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetMissedFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);

                    // box score
                    if (_teamPossession == 0)
                    {
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == currentPlayerRating.PlayerId);
                        temp.FTA++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == currentPlayerRating.PlayerId);
                        _homeBoxScores[index] = temp;
                    }
                    else
                    {
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentPlayerRating.PlayerId);
                        temp.FTA++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentPlayerRating.PlayerId);
                        _awayBoxScores[index] = temp;
                    }
                }
            }
            _shotClock = 24;
            return lastShot;
        }

        public void PlayerTurnover()
        {
            turnoverCounter++;
            _playerRatingPassed = null;
            _playerPassed = null;

            int toComm = 0;

            // Update the timer
            int timeValue = _random.Next(1, 6);

            // Need to check if the time has not gone past 0 and update the time value appropriately
            if (timeValue > _time || timeValue > _shotClock)
            {
                // This action causes time to expire
                if (timeValue > _time)
                {
                    timeValue = _time;
                }
                else if (timeValue > _shotClock)
                {
                    timeValue = _shotClock;
                }
            }

            // Updates
            // Get correct display of time
            _time = _time - timeValue;
            _shotClock = _shotClock - timeValue;
            timeCounter = timeCounter + timeValue;

            UpdateTimeInBoxScore(timeValue);
            StaminaUpdates(timeValue);

            // Player has committed a turnover
            int result = _random.Next(1, 51);
            PlayerRating current = GetCurrentPlayersRatings();

            int turnoverType = -1;
            if (result <= 35)
            {
                turnoverType = 0;
            }
            else if (result <= 48)
            {
                turnoverType = 1;
            }
            else
            {
                turnoverType = 2;
            }

            if (_teamPossession == 0)
            {
                BoxScore temp = _homeBoxScores.Find(x => x.Id == current.PlayerId);
                // Need to check if it was Offensive Foul
                if (turnoverType == 2)
                {
                    temp.Fouls++;

                    // Need to identify the current position
                    int fouler = CheckCurrentPosition(current.PlayerId);

                    // // Foul Trouble Check for subs
                    // int sub = FoulTroubleCheck(0, fouler);

                    // if (sub == 1)
                    // {
                    //     string outPlayer = GetPlayerFullNameForPositionForFouler(0, GetPlayerIdForPosition(0, fouler));
                    //     // Player needs to be subbed out due to foul trouble
                    //     Substitution(1, fouler);

                    //     if (subError == 0)
                    //     {
                    //         // Now need to sort out the sub commentary
                    //         string inPlayer = GetPlayerFullNameForPositionForFouler(0, GetPlayerIdForPosition(0, fouler));

                    //         commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                    //         // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                    //         PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    //     }


                    // }
                    // else if (sub == 6) {
                    //     // player has fouled out
                    //     int pid = GetPlayerIdForPosition(1, fouler);
                    //     fouledOutPlayers.Add(pid);

                    //     // call substituion
                    //     Substitution(1, fouler);
                    // }
                }
                temp.Turnovers++;
                int index = _homeBoxScores.FindIndex(x => x.Id == current.PlayerId);
                _homeBoxScores[index] = temp;
                toComm = temp.Turnovers;

                // commentary
                commentaryData.Add(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, toComm));
                // Console.WriteLine(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                PlayByPlayTracker(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, toComm), 1);

                // change possession
                _teamPossession = 1;
            }
            else
            {
                BoxScore temp = _awayBoxScores.Find(x => x.Id == current.PlayerId);
                // Need to check if it was Offensive Foul
                if (turnoverType == 2)
                {
                    temp.Fouls++;

                    // Need to identify the current position
                    int fouler = CheckCurrentPosition(current.PlayerId);

                    // // Foul Trouble Check for subs
                    // int sub = FoulTroubleCheck(1, fouler);

                    // if (sub == 1)
                    // {
                    //     string outPlayer = GetPlayerFullNameForPositionForFouler(1, GetPlayerIdForPosition(1, fouler));

                    //     // Player needs to be subbed out due to foul trouble
                    //     Substitution(1, fouler);

                    //     if (subError == 0)
                    //     {
                    //         // Now need to sort out the sub commentary
                    //         string inPlayer = GetPlayerFullNameForPositionForFouler(1, GetPlayerIdForPosition(1, fouler));

                    //         commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                    //         // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                    //         PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    //     }


                    // }
                    // else if (sub == 6) {
                    //     // player has fouled out
                    //     int pid = GetPlayerIdForPosition(1, fouler);
                    //     fouledOutPlayers.Add(pid);

                    //     // call substituion
                    //     Substitution(1, fouler);
                    // }
                }
                temp.Turnovers++;
                int index = _awayBoxScores.FindIndex(x => x.Id == current.PlayerId);
                _awayBoxScores[index] = temp;
                toComm = temp.Turnovers;

                // commentary
                commentaryData.Add(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, toComm));
                // Console.WriteLine(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                PlayByPlayTracker(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, toComm), 1);

                // change possession
                _teamPossession = 0;
            }
            _shotClock = 24;
            InjuryCheck();
            // SubCheck();
            // CheckForSubs(0);
            SubstitutionCheck(0);
        }

        public void ShotClockViolation()
        {
            shotClockCounter++;
            _playerRatingPassed = null;
            _playerPassed = null;

            _shotClock = 24;

            if (_teamPossession == 0)
            {
                _teamPossession = 1;
            }
            else
            {
                _teamPossession = 0;
            }

            // Need to add the commentary here
            commentaryData.Add(comm.GetShotClockTurnoverCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            // Console.WriteLine(comm.GetShotClockTurnoverCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            PlayByPlayTracker(comm.GetShotClockTurnoverCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);

            // Now it would go to check for SUBS
            InjuryCheck();
            InjuryStaminaChecks();
            // SubCheck();
            // CheckForSubs(0);
            SubstitutionCheck(0);

            // Inbounds the ball to continue on
            Inbounds();
        }

        public void EndGameDefenceAndFouls()
        {
            int diff = 0;
            if (_teamPossession == 0)
            {
                // Away team is on defence
                diff = _homeScore - _awayScore;

                if (diff <= 7 && diff >= 0)
                {
                    // Filtering out blowouts and if the team is winning
                    if (_time < (_shotClock + 4) && (diff >= 4 && diff <= 7))
                    {
                        // increased steal chance
                        _endGameStealAddition = 400;
                        // much increased in fouls
                        _endGameFoulAddition = 1500;
                    }
                    else if (_time < _shotClock && (diff >= 1 && diff <= 7))
                    {
                        // increased steal chance
                        _endGameStealAddition = 400;
                        // much increased in fouls
                        _endGameFoulAddition = 1560;
                    }
                    else if (_time > _shotClock && _time <= 40 && (diff >= 4 && diff <= 7))
                    {
                        // increased steal chance
                        _endGameStealAddition = 400;
                        // much increased in fouls
                        _endGameFoulAddition = 1500;
                    }
                }
            }
            else
            {
                diff = _awayScore - _homeScore;

                if (diff <= 7 && diff >= 0)
                {
                    // Filtering out blowouts and if the team is winning
                    if (_time < (_shotClock + 4) && (diff >= 4 && diff <= 7))
                    {
                        // increased steal chance
                        _endGameStealAddition = 400;
                        // much increased in fouls
                        _endGameFoulAddition = 1500;
                    }
                    else if (_time < _shotClock && (diff >= 1 && diff <= 7))
                    {
                        // increased steal chance
                        _endGameStealAddition = 400;
                        // much increased in fouls
                        _endGameFoulAddition = 1500;
                    }
                    else if (_time > _shotClock && _time <= 40 && (diff >= 4 && diff <= 7))
                    {
                        // increased steal chance
                        _endGameStealAddition = 400;
                        // much increased in fouls
                        _endGameFoulAddition = 1500;
                    }
                }
            }
        }

        public void EndGameShootBonus()
        {
            int diff = 0;
            EndGameDefenceAndFouls();

            if (_teamPossession == 0)
            {
                diff = _awayScore - _homeScore;

                if (diff < 0)
                {
                    // home team is winning
                    if (_time > 16 || _shotClock > 16)
                    {
                        _endGameShotClockBonus = -500;
                    }
                    else if (_time > 12 || _shotClock > 12)
                    {
                        _endGameShotClockBonus = -300;
                    }
                    else if (_time > 8 || _shotClock > 8)
                    {
                        _endGameShotClockBonus = 400;
                    }
                    else if (_time > 4 || _shotClock > 4)
                    {
                        _endGameShotClockBonus = 750;
                    }
                    else
                    {
                        _endGameShotClockBonus = 1000;
                    }
                }
                else if (diff > 0)
                {
                    // home team is losing
                    // losing margins
                    if (diff <= 10 && diff > 7)
                    {
                        _endGameThreePointAddition = (int)(GetCurrentPlayersTendancies().ThreePointTendancy * 0.50);
                        if (_time > 16 || _shotClock > 16)
                        {
                            _endGameShotClockBonus = 200;
                        }
                        else if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = 400;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 500;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 600;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 750;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 9000;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 1000;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1100;
                        }
                    }
                    else if (diff == 5 || diff == 6 || diff == 7)
                    {
                        // Shooting liklihood is increase significantly and 3's are increased most
                        // team will shoot quicker
                        _endGameThreePointAddition = (int)(GetCurrentPlayersTendancies().ThreePointTendancy * 0.33);
                        if (_time > 16 || _shotClock > 16)
                        {
                            _endGameShotClockBonus = 150;
                        }
                        else if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = 200;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 250;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 300;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 500;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 600;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 800;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1000;
                        }
                    }
                    else if (diff == 4)
                    {
                        // team will shoot quicker
                        if (_time > 16 || _shotClock > 16)
                        {
                            _endGameShotClockBonus = 150;
                        }
                        else if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = 200;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 250;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 300;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 500;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 750;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 900;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1250;
                        }

                        // random between 5 and 10% added to shot result
                        _endGameResultIncrease = (_random.Next(20, 51));
                    }
                    else if (diff == 3)
                    {
                        // 33% increase of shooting the 3 ball
                        _endGameThreePointAddition = (int)(GetCurrentPlayersTendancies().ThreePointTendancy * 0.33);
                        // increase is shot overall
                        if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = 100;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 150;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 250;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 300;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 700;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 850;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1250;
                        }
                    }
                    else if (diff <= 2)
                    {
                        // normal tendancies
                        // increase shot bonus as time runs out
                        if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = -0;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 0;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 50;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 200;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 600;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 900;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1250;
                        }
                    }
                }
            }
            else
            {
                diff = _homeScore - _awayScore;

                if (diff < 0)
                {
                    // away team is losing
                    if (_time > 16 || _shotClock > 16)
                    {
                        _endGameShotClockBonus = -500;
                    }
                    else if (_time > 12 || _shotClock > 12)
                    {
                        _endGameShotClockBonus = -300;
                    }
                    else if (_time > 8 || _shotClock > 8)
                    {
                        _endGameShotClockBonus = 400;
                    }
                    else if (_time > 4 || _shotClock > 4)
                    {
                        _endGameShotClockBonus = 750;
                    }
                    else
                    {
                        _endGameShotClockBonus = 1000;
                    }
                }
                else if (diff > 0)
                {
                    // away team is winning
                    // losing margins
                    if (diff <= 10 && diff > 7)
                    {
                        _endGameThreePointAddition = (int)(GetCurrentPlayersTendancies().ThreePointTendancy * 0.50);
                        if (_time > 16 || _shotClock > 16)
                        {
                            _endGameShotClockBonus = 200;
                        }
                        else if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = 400;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 500;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 600;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 750;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 9000;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 1000;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1100;
                        }
                    }
                    else if (diff == 5 || diff == 6 || diff == 7)
                    {
                        _endGameThreePointAddition = (int)(GetCurrentPlayersTendancies().ThreePointTendancy * 0.33);
                        // Shooting liklihood is increase significantly and 3's are increased most
                        // team will shoot quicker
                        if (_time > 16 || _shotClock > 16)
                        {
                            _endGameShotClockBonus = 150;
                        }
                        else if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = 200;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 250;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 300;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 500;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 600;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 800;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1000;
                        }
                    }
                    else if (diff == 4)
                    {
                        // team will shoot quicker
                        if (_time > 16 || _shotClock > 16)
                        {
                            _endGameShotClockBonus = 150;
                        }
                        else if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = 200;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 250;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 300;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 500;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 750;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 900;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1250;
                        }

                        // random between 5 and 10% added to shot result
                        _endGameResultIncrease = (_random.Next(20, 51));
                    }
                    else if (diff == 3)
                    {
                        // 33% increase of shooting the 3 ball
                        _endGameThreePointAddition = (int)(GetCurrentPlayersTendancies().ThreePointTendancy * 0.33);
                        // increase is shot overall
                        if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = 100;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 150;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 250;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 300;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 700;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 850;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1250;
                        }
                    }
                    else if (diff <= 2)
                    {
                        // normal tendancies
                        // increase shot bonus as time runs out
                        if (_time > 14 || _shotClock > 14)
                        {
                            _endGameShotClockBonus = -0;
                        }
                        else if (_time > 12 || _shotClock > 12)
                        {
                            _endGameShotClockBonus = 0;
                        }
                        else if (_time > 10 || _shotClock > 10)
                        {
                            _endGameShotClockBonus = 50;
                        }
                        else if (_time > 8 || _shotClock > 8)
                        {
                            _endGameShotClockBonus = 200;
                        }
                        else if (_time > 6 || _shotClock > 6)
                        {
                            _endGameShotClockBonus = 600;
                        }
                        else if (_time > 4 || _shotClock > 4)
                        {
                            _endGameShotClockBonus = 900;
                        }
                        else
                        {
                            _endGameShotClockBonus = 1250;
                        }
                    }
                }
            }
        }

        public int ShotClockShootBonus()
        {
            int speedStrategy = 0;

            if (_teamPossession == 0)
            {
                speedStrategy = _homeSpeedStrategy * 1;
            }
            else
            {
                speedStrategy = _awaySpeedStrategy * 1;
            }

            if (_shotClock > 22)
            {
                return -200 + speedStrategy;
            }
            else if (_shotClock > 20)
            {
                return -100 + speedStrategy;
            }
            else if (_shotClock > 18)
            {
                return -25 + speedStrategy;
            }
            else if (_shotClock > 14)
            {
                return 35 + speedStrategy;
            }
            else if (_shotClock > 12)
            {
                return 80 + speedStrategy;
            }
            else if (_shotClock > 10)
            {
                return 140 + speedStrategy;
            }
            else if (_shotClock > 8)
            {
                return 250 + speedStrategy;
            }
            else if (_shotClock > 6)
            {
                return 300 + speedStrategy;
            }
            else if (_shotClock > 4)
            {
                return 400 + speedStrategy;
            }
            else
            {
                return 500 + speedStrategy;
            }
        }

        public void EndOfQuarter()
        {
            commentaryData.Add(comm.EndOfQuarterCommtary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            // Console.WriteLine(comm.EndOfQuarterCommtary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            PlayByPlayTracker(comm.EndOfQuarterCommtary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);

            _playerPassed = null;
            _playerRatingPassed = null;

            switch (_quarter)
            {
                case 1:
                    StaminaQuarterEffects(_quarter);
                    _quarter = 2;
                    _time = 720;
                    break;
                case 2:
                    StaminaQuarterEffects(_quarter);
                    _quarter = 3;
                    _time = 720;
                    break;
                case 3:
                    StaminaQuarterEffects(_quarter);
                    _quarter = 4;
                    _time = 720;
                    break;
                case 4:
                    if (_awayScore == _homeScore)
                    {
                        StaminaQuarterEffects(_quarter);
                    }
                    else
                    {
                        // End of Game commentary
                        commentaryData.Add(comm.EndGameCommentary(_awayTeam, _homeTeam, _awayScore, _homeScore));
                        // Console.WriteLine(comm.EndGameCommentary(_awayTeam, _homeTeam, _awayScore, _homeScore));
                        PlayByPlayTracker(comm.EndGameCommentary(_awayTeam, _homeTeam, _awayScore, _homeScore), 0);
                    }
                    break;
                default:
                    break;
            }

        }

        public void Inbounds()
        {
            // Update the timer
            int timeValue = _random.Next(0, 6);

            // Need to check if the time has not gone past 0 and update the time value appropriately
            if (timeValue > _time || timeValue > _shotClock)
            {
                // This action causes time to expire
                if (timeValue > _time)
                {
                    timeValue = _time;
                }
                else if (timeValue > _shotClock)
                {
                    timeValue = _shotClock;
                }
            }

            // Updates
            // Get correct display of time
            _time = _time - timeValue;
            _shotClock = _shotClock - timeValue;
            timeCounter = timeCounter + timeValue;

            UpdateTimeInBoxScore(timeValue);
            StaminaUpdates(timeValue);

            int value = 0;
            if (_teamPossession == 0)
            {
                int total = homePGRatings.UsageRating + homeSGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating + homeCRatings.UsageRating;
                int result = _random.Next(total);

                Inbounding inbound = new Inbounding();
                value = inbound.GetInboundsResult(homePGRatings, homeSGRatings, homeSFRatings, homePFRatings, homeCRatings, result);

            }
            else
            {
                int total = awayPGRatings.UsageRating + awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating + awayCRatings.UsageRating;
                int result = _random.Next(total);
                Inbounding inbound = new Inbounding();
                value = inbound.GetInboundsResult(awayPGRatings, awaySGRatings, awaySFRatings, awayPFRatings, awayCRatings, result);
            }

            _playerPossession = value;
            int passerSet = 0;

            // Need to update the player passed and the player passed rating
            if (_teamPossession == 0)
            {
                while (passerSet == 0)
                {
                    int passer = _random.Next(1, 6);
                    if (passer != _playerPossession)
                    {
                        // Now set the value
                        switch (passer)
                        {
                            case 1:
                                _playerPassed = homePG;
                                _playerRatingPassed = homePGRatings;
                                break;
                            case 2:
                                _playerPassed = homeSG;
                                _playerRatingPassed = homeSGRatings;
                                break;
                            case 3:
                                _playerPassed = homeSF;
                                _playerRatingPassed = homeSFRatings;
                                break;
                            case 4:
                                _playerPassed = homePF;
                                _playerRatingPassed = homePFRatings;
                                break;
                            case 5:
                                _playerPassed = homeC;
                                _playerRatingPassed = homeCRatings;
                                break;
                            default:
                                break;
                        }
                        passerSet = 1;
                    }
                }
            }
            else
            {
                while (passerSet == 0)
                {
                    int passer = _random.Next(1, 6);
                    if (passer != _playerPossession)
                    {
                        // Now set the value
                        switch (passer)
                        {
                            case 1:
                                _playerPassed = awayPG;
                                _playerRatingPassed = awayPGRatings;
                                break;
                            case 2:
                                _playerPassed = awaySG;
                                _playerRatingPassed = awaySGRatings;
                                break;
                            case 3:
                                _playerPassed = awaySF;
                                _playerRatingPassed = awaySFRatings;
                                break;
                            case 4:
                                _playerPassed = awayPF;
                                _playerRatingPassed = awayPFRatings;
                                break;
                            case 5:
                                _playerPassed = awayC;
                                _playerRatingPassed = awayCRatings;
                                break;
                            default:
                                break;
                        }
                        passerSet = 1;
                    }
                }
            }
        }

        /* REFACTORED */
        public int GetOrpmValue(int team)
        {
            int orpmStrategy = 0;

            if (team == 0)
            {
                orpmStrategy = _homeORPMStrategy;

                int pgVal = (homePGRatings.ORPMRating / 10) + orpmStrategy;
                int sgVal = (homeSGRatings.ORPMRating / 10) + orpmStrategy;
                int sfVal = (homeSFRatings.ORPMRating / 10) + orpmStrategy;
                int pfVal = (homePFRatings.ORPMRating / 10) + orpmStrategy;
                int cVal = (homeCRatings.ORPMRating / 10) + orpmStrategy;

                int total = pgVal + sgVal + sfVal + pfVal + cVal;
                int valueToReturn = total / 5;
                return valueToReturn;
            }
            else
            {
                orpmStrategy = _awayDRPMStrategy;

                int pgVal = (awayPGRatings.ORPMRating / 10) + orpmStrategy;
                int sgVal = (awaySGRatings.ORPMRating / 10) + orpmStrategy;
                int sfVal = (awaySFRatings.ORPMRating / 10) + orpmStrategy;
                int pfVal = (awayPFRatings.ORPMRating / 10) + orpmStrategy;
                int cVal = (awayCRatings.ORPMRating / 10) + orpmStrategy;

                int total = pgVal + sgVal + sfVal + pfVal + cVal;
                int valueToReturn = total / 5;
                return valueToReturn;
            }
        }

        /* REFACTORED */
        public int GetDrpmValue(int team)
        {
            int drpmStrategy = 0;

            if (team == 0)
            {
                drpmStrategy = _homeDRPMStrategy;

                int pgVal = (homePGRatings.DRPMRating / 10) + drpmStrategy;
                int sgVal = (homeSGRatings.DRPMRating / 10) + drpmStrategy;
                int sfVal = (homeSFRatings.DRPMRating / 10) + drpmStrategy;
                int pfVal = (homePFRatings.DRPMRating / 10) + drpmStrategy;
                int cVal = (homeCRatings.DRPMRating / 10) + drpmStrategy;

                int total = pgVal + sgVal + sfVal + pfVal + cVal;
                int valueToReturn = total / 5;
                return valueToReturn;
            }
            else
            {
                drpmStrategy = _awayDRPMStrategy;

                int pgVal = (awayPGRatings.DRPMRating / 10) + drpmStrategy;
                int sgVal = (awaySGRatings.DRPMRating / 10) + drpmStrategy;
                int sfVal = (awaySFRatings.DRPMRating / 10) + drpmStrategy;
                int pfVal = (awayPFRatings.DRPMRating / 10) + drpmStrategy;
                int cVal = (awayCRatings.DRPMRating / 10) + drpmStrategy;

                int total = pgVal + sgVal + sfVal + pfVal + cVal;
                int valueToReturn = total / 5;
                return valueToReturn;
            }
        }

        // SUBS
        /* REFACTORED */
        public int FoulTroubleCheck(int team, int player)
        {
            int subOut = 0;
            int fouls = 0;

            if (team == 0)
            {
                BoxScore temp = _homeBoxScores.Find(x => x.Id == player);
                fouls = temp.Fouls;
            }
            else
            {
                BoxScore temp = _awayBoxScores.Find(x => x.Id == player);
                fouls = temp.Fouls;
            }

            switch (_quarter)
            {
                case 1:
                    if (fouls >= 2)
                    {
                        subOut = 1;
                    }
                    break;
                case 2:
                    if (fouls >= 3)
                    {
                        subOut = 1;
                    }
                    break;
                case 3:
                    if (fouls >= 4)
                    {
                        subOut = 1;
                    }
                    break;
                default:
                    subOut = 0;
                    break;
            }
            return subOut;
        }

        // public void CheckForSubs(int ftPlayerId)
        // {
        //     if ((_quarter > 3 && _time <= 240) || (_quarter > 4))
        //     {
        //         // End Game Subs
        //         SubToStarters();
        //     }
        //     else
        //     {
        //         // Normal non-end game subs
        //         // For each teamm - Home and Away
        //         // Home
        //         for (int i = 1; i < 6; i++)
        //         {
        //             int result = 0;
        //             if (ftPlayerId == 1)
        //             {
        //                 result = CheckIfShooter(0, i, ftPlayerId);
        //             }

        //             if (result != 1)
        //             {
        //                 Player p = new Player();
        //                 switch (i)
        //                 {
        //                     case 1:
        //                         p = homePG;
        //                         break;
        //                     case 2:
        //                         p = homeSG;
        //                         break;
        //                     case 3:
        //                         p = homeSF;
        //                         break;
        //                     case 4:
        //                         p = homePF;
        //                         break;
        //                     case 5:
        //                         p = homeC;
        //                         break;
        //                     default:
        //                         break;
        //                 }

        //                 result = CheckSubEligility(p, 0);
        //                 if (result == 0)
        //                 {
        //                     // Passed the first check, now for fatigue
        //                     result = CheckPlayerFatigue(0, i, p.Id);
        //                 }

        //                 if (result == 1)
        //                 {
        //                     // The player is to be subbed off
        //                     Substitution(0, i);
        //                 }
        //             }
        //         }

        //         // Away
        //         for (int j = 1; j < 6; j++)
        //         {
        //             int result = 0;
        //             if (ftPlayerId == 1)
        //             {
        //                 result = CheckIfShooter(1, j, ftPlayerId);
        //             }

        //             if (result != 1)
        //             {
        //                 Player p = new Player();
        //                 switch (j)
        //                 {
        //                     case 1:
        //                         p = awayPG;
        //                         break;
        //                     case 2:
        //                         p = awaySG;
        //                         break;
        //                     case 3:
        //                         p = awaySF;
        //                         break;
        //                     case 4:
        //                         p = awayPF;
        //                         break;
        //                     case 5:
        //                         p = awayC;
        //                         break;
        //                     default:
        //                         break;
        //                 }

        //                 result = CheckSubEligility(p, 1);   // TODO fix up the Player and team
        //                 if (result == 0)
        //                 {
        //                     // Passed the first check, now for fatigue
        //                     result = CheckPlayerFatigue(1, j, p.Id);
        //                 }

        //                 if (result == 1)
        //                 {
        //                     // The player is to be subbed off
        //                     Substitution(1, j);
        //                 }
        //             }
        //         }
        //     }
        // }

        public int CheckIfShooter(int team, int position, int playerId)
        {
            int currentPosPlayerId = 0;
            if (playerId == 0)
            {
                return 0;
            }
            else
            {
                if (team == 0)
                {
                    switch (position)
                    {
                        case 1:
                            currentPosPlayerId = homePG.Id;
                            break;
                        case 2:
                            currentPosPlayerId = homeSG.Id;
                            break;
                        case 3:
                            currentPosPlayerId = homeSF.Id;
                            break;
                        case 4:
                            currentPosPlayerId = homePF.Id;
                            break;
                        case 5:
                            currentPosPlayerId = homeC.Id;
                            break;
                        default:
                            break;
                    }

                    if (currentPosPlayerId == playerId)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    switch (position)
                    {
                        case 1:
                            currentPosPlayerId = awayPG.Id;
                            break;
                        case 2:
                            currentPosPlayerId = awaySG.Id;
                            break;
                        case 3:
                            currentPosPlayerId = awaySF.Id;
                            break;
                        case 4:
                            currentPosPlayerId = awayPF.Id;
                            break;
                        case 5:
                            currentPosPlayerId = awayC.Id;
                            break;
                        default:
                            break;
                    }

                    if (currentPosPlayerId == playerId)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public int CheckSubEligility(Player player, int team)
        {
            int currentInjured = 0;
            int currentFouledOut = 0;

            InjuryDto injuryCheck = null;
            BoxScore bs = null;
            if (team == 0)
            {
                injuryCheck = _homeInjuries.Find(x => x.PlayerId == player.Id);
                bs = _homeBoxScores.FirstOrDefault(x => x.Id == player.Id);
            }
            else
            {
                injuryCheck = _awayInjuries.Find(x => x.PlayerId == player.Id);
                bs = _awayBoxScores.FirstOrDefault(x => x.Id == player.Id);
            }

            if (injuryCheck != null)
            {
                if (injuryCheck.Severity > 2)
                {
                    currentInjured = 1;
                }
            }

            if (bs.Fouls >= 6)
            {
                currentFouledOut = 1;
            }

            int pid = GetPlayerPositionForPlayerId(player.Id, team);
            int currentFoulTrouble = FoulTroubleCheck(team, pid);

            // Now need to check to ensure that the current player is not injured or fouled out or in foul trouble
            if (currentFoulTrouble == 1 || currentFouledOut == 1 || currentInjured == 1)
            {
                // The player cannot be kept on the court and must be subbed off (can change the play through foul trouble option)
                return 1;
            }
            return 0;
        }

        public int CheckPlayerFatigue(int team, int position, int playerId)
        {
            List<StaminaTrack> staminas = new List<StaminaTrack>();

            if (playerId != 0)
            {
                if (team == 0)
                {
                    staminas = _homeStaminas;
                }
                else
                {
                    staminas = _awayStaminas;
                }
            }
            var st = staminas.FirstOrDefault(x => x.PlayerId == playerId);
            if ((st != null) && (st.StaminaValue > 700 && st.OnOff == 1))
            {
                // Player should be subbed out
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // public void Substitution(int team, int position)
        // {
        //     Player current = new Player();
        //     Player checkingPlayer = new Player();
        //     List<DepthChart> filterDepth = new List<DepthChart>();
        //     List<Player> teamPlayers = new List<Player>();
        //     int playerSet = 0;

        //     if (team == 0)
        //     {
        //         List<DepthChart> depth = _homeDepth.OrderBy(x => x.Depth).ToList();
        //         filterDepth = depth.Where(x => x.Position == position).ToList();
        //         teamPlayers = _homePlayers;
        //         switch (position)
        //         {
        //             case 1:
        //                 current = homePG;
        //                 break;
        //             case 2:
        //                 current = homeSG;
        //                 break;
        //             case 3:
        //                 current = homeSF;
        //                 break;
        //             case 4:
        //                 current = homePF;
        //                 break;
        //             case 5:
        //                 current = homeC;
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        //     else
        //     {
        //         List<DepthChart> depth = _awayDepth.OrderBy(x => x.Depth).ToList();
        //         filterDepth = depth.Where(x => x.Position == position).ToList();
        //         teamPlayers = _awayPlayers;
        //         switch (position)
        //         {
        //             case 1:
        //                 current = awayPG;
        //                 break;
        //             case 2:
        //                 current = awaySG;
        //                 break;
        //             case 3:
        //                 current = awaySF;
        //                 break;
        //             case 4:
        //                 current = awayPF;
        //                 break;
        //             case 5:
        //                 current = awayC;
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }

        //     for (int i = 0; i < filterDepth.Count; i++)
        //     {
        //         DepthChart dc = filterDepth[i];

        //         if (dc.PlayerId != 0)
        //         {
        //             checkingPlayer = teamPlayers.FirstOrDefault(x => x.Id == dc.PlayerId);

        //             int onCourt = CheckIfPlayerIsOnCourt(team, checkingPlayer.Id);
        //             if (onCourt != 1)
        //             {
        //                 int result = CheckSubEligility(checkingPlayer, team);
        //                 if (result == 0)
        //                 {
        //                     // Passed the first check, now for fatigue
        //                     result = CheckPlayerFatigue(team, position, checkingPlayer.Id);

        //                     if (result == 1)
        //                     {
        //                         // The player is to be subbed on 
        //                         SubPlayer(team, position, checkingPlayer);

        //                         playerSet = 1;

        //                         // Add the commentary here
        //                         string outPlayer = current.FirstName + " " + current.Surname;
        //                         string inPlayer = checkingPlayer.FirstName + " " + checkingPlayer.Surname;

        //                         if (team == 0)
        //                         {
        //                             commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
        //                             PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
        //                         }
        //                         else
        //                         {
        //                             commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
        //                             PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //     }

        //     if (playerSet == 0)
        //     {
        //         // Player has not yet been subbed - need to check NoPlayersToSubIn
        //         NoPlayersToSubIn(team, position);
        //     }
        // }

        public int CheckIfPlayerIsOnCourt(int team, int playerId)
        {
            List<int> onCourtIds = new List<int>();
            if (team == 0)
            {
                if (homePG != null)
                {
                    onCourtIds.Add(homePG.Id);
                }

                if (homeSG != null)
                {
                    onCourtIds.Add(homeSG.Id);
                }

                if (homeSF != null)
                {
                    onCourtIds.Add(homeSF.Id);
                }

                if (homePF != null)
                {
                    onCourtIds.Add(homePF.Id);
                }

                if (homeC != null)
                {
                    onCourtIds.Add(homeC.Id);
                }
            }
            else
            {
                if (awayPG != null)
                {
                    onCourtIds.Add(awayPG.Id);
                }

                if (awaySG != null)
                {
                    onCourtIds.Add(awaySG.Id);
                }

                if (awaySF != null)
                {
                    onCourtIds.Add(awaySF.Id);
                }

                if (awayPF != null)
                {
                    onCourtIds.Add(awayPF.Id);
                }

                if (awayC != null)
                {
                    onCourtIds.Add(awayC.Id);
                }
            }
            var exists = onCourtIds.Contains(playerId);
            if (exists)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void SubPlayer(int team, int position, Player player)
        {
            if (team == 0)
            {
                // Home
                if (position == 1)
                {
                    Player current = homePG;
                    homePG = player;
                    homePGRatings = _homeRatings.Find(x => x.PlayerId == player.Id);
                    homePGTendancy = _homeTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _homeStaminas[index] = stOff;

                    StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePG.Id);
                    stOn.OnOff = 1;
                    index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _homeStaminas[index] = stOn;
                }
                else if (position == 2)
                {
                    Player current = homeSG;
                    homeSG = player;
                    homeSGRatings = _homeRatings.Find(x => x.PlayerId == player.Id);
                    homeSGTendancy = _homeTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _homeStaminas[index] = stOff;

                    StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSG.Id);
                    stOn.OnOff = 1;
                    index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _homeStaminas[index] = stOn;
                }
                else if (position == 3)
                {
                    Player current = homeSF;
                    homeSF = player;
                    homeSFRatings = _homeRatings.Find(x => x.PlayerId == player.Id);
                    homeSFTendancy = _homeTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _homeStaminas[index] = stOff;

                    StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSF.Id);
                    stOn.OnOff = 1;
                    index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _homeStaminas[index] = stOn;
                }
                else if (position == 4)
                {
                    Player current = homePF;
                    homePF = player;
                    homePFRatings = _homeRatings.Find(x => x.PlayerId == player.Id);
                    homePFTendancy = _homeTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _homeStaminas[index] = stOff;

                    StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePF.Id);
                    stOn.OnOff = 1;
                    index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _homeStaminas[index] = stOn;
                }
                else if (position == 5)
                {
                    Player current = homeC;
                    homeC = player;
                    homeCRatings = _homeRatings.Find(x => x.PlayerId == player.Id);
                    homeCTendancy = _homeTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _homeStaminas[index] = stOff;

                    StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeC.Id);
                    stOn.OnOff = 1;
                    index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _homeStaminas[index] = stOn;
                }
            }
            else
            {
                // Away
                if (position == 1)
                {
                    Player current = awayPG;
                    awayPG = player;
                    awayPGRatings = _awayRatings.Find(x => x.PlayerId == player.Id);
                    awayPGTendancy = _awayTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _awayStaminas[index] = stOff;

                    StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayPG.Id);
                    stOn.OnOff = 1;
                    index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _awayStaminas[index] = stOn;
                }
                else if (position == 2)
                {
                    Player current = awaySG;
                    awaySG = player;
                    awaySGRatings = _awayRatings.Find(x => x.PlayerId == player.Id);
                    awaySGTendancy = _awayTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _awayStaminas[index] = stOff;

                    StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awaySG.Id);
                    stOn.OnOff = 1;
                    index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _awayStaminas[index] = stOn;
                }
                else if (position == 3)
                {
                    Player current = awaySF;
                    awaySF = player;
                    awaySFRatings = _awayRatings.Find(x => x.PlayerId == player.Id);
                    awaySFTendancy = _awayTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _awayStaminas[index] = stOff;

                    StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awaySF.Id);
                    stOn.OnOff = 1;
                    index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _awayStaminas[index] = stOn;
                }
                else if (position == 4)
                {
                    Player current = awayPF;
                    awayPF = player;
                    awayPFRatings = _awayRatings.Find(x => x.PlayerId == player.Id);
                    awayPFTendancy = _awayTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _awayStaminas[index] = stOff;

                    StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayPF.Id);
                    stOn.OnOff = 1;
                    index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _awayStaminas[index] = stOn;
                }
                else if (position == 5)
                {
                    Player current = awayC;
                    awayC = player;
                    awayCRatings = _awayRatings.Find(x => x.PlayerId == player.Id);
                    awayCTendancy = _awayTendancies.Find(x => x.PlayerId == player.Id);

                    // Need to update the stamina track objects for on and off court
                    StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                    stOff.OnOff = 0;
                    int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                    _awayStaminas[index] = stOff;

                    StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayC.Id);
                    stOn.OnOff = 1;
                    index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                    _awayStaminas[index] = stOn;
                }
            }
        }

        public void NoPlayersToSubIn(int team, int position)
        {
            List<Player> players = new List<Player>();
            Player current = new Player();
            Player newPlayer = new Player();
            List<DepthChart> dc = new List<DepthChart>();
            List<StaminaTrack> st = new List<StaminaTrack>();
            int playerSubbed = 0;

            // Now need to decide who comes on
            if (team == 0)
            {
                dc = _homeDepth;
                st = _homeStaminas;

                // Home team
                switch (position)
                {
                    case 1:
                        current = homePG;
                        players = _homePlayers.FindAll(x => x.PGPosition == 1);
                        break;
                    case 2:
                        current = homeSG;
                        players = _homePlayers.FindAll(x => x.SGPosition == 1);
                        break;
                    case 3:
                        current = homeSF;
                        players = _homePlayers.FindAll(x => x.SFPosition == 1);
                        break;
                    case 4:
                        current = homePF;
                        players = _homePlayers.FindAll(x => x.PFPosition == 1);
                        break;
                    case 5:
                        current = homeC;
                        players = _homePlayers.FindAll(x => x.CPosition == 1);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                dc = _awayDepth;
                st = _awayStaminas;

                // Away team
                switch (position)
                {
                    case 1:
                        current = awayPG;
                        players = _awayPlayers.FindAll(x => x.PGPosition == 1);
                        break;
                    case 2:
                        current = awaySG;
                        players = _awayPlayers.FindAll(x => x.SGPosition == 1);
                        break;
                    case 3:
                        current = awaySF;
                        players = _awayPlayers.FindAll(x => x.SFPosition == 1);
                        break;
                    case 4:
                        current = awayPF;
                        players = _awayPlayers.FindAll(x => x.PFPosition == 1);
                        break;
                    case 5:
                        current = awayC;
                        players = _awayPlayers.FindAll(x => x.CPosition == 1);
                        break;
                    default:
                        break;
                }
            }

            // Firstly we want to check the deptch chart again
            var options = dc.FindAll(x => x.Position == position);

            foreach (var option in options)
            {
                int playerId = option.PlayerId;
                var player = players.FirstOrDefault(x => x.Id == playerId);

                int result = 1;
                int onCourt = 1;
                if (player != null)
                {
                    result = CheckSubEligility(player, team);
                    onCourt = CheckIfPlayerIsOnCourt(team, player.Id);
                }

                if (result == 0 && onCourt == 0)
                {
                    // The player can be checked for the new fatigue
                    var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                    if ((st != null) && (stam.StaminaValue > 500))
                    {
                        // Player should be subbed on
                        // Need action to make the sub
                        SubPlayer(team, position, player);
                        newPlayer = player;
                        playerSubbed = 1;
                        break;
                    }
                }
            }

            if (playerSubbed == 0)
            {
                foreach (var player in players)
                {
                    // Now need to check if the current player is NOT in the positions DC
                    var res = dc.FirstOrDefault(x => x.Position == position && x.PlayerId == player.Id);

                    if (res != null)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            if ((st != null) && (stam.StaminaValue > 500))
                            {
                                // Need action to make the sub
                                SubPlayer(team, position, player);
                                newPlayer = player;
                                playerSubbed = 1;
                                break;
                            }
                        }
                    }
                }
            }

            if (playerSubbed == 0)
            {
                // Need to go to fallback options
                if (team == 0)
                {
                    switch (position)
                    {
                        case 1:
                            players = _homePlayers.FindAll(x => x.SGPosition == 1);
                            break;
                        case 2:
                            players = _homePlayers.FindAll(x => x.SFPosition == 1);
                            break;
                        case 3:
                            players = _homePlayers.FindAll(x => x.SGPosition == 1);
                            break;
                        case 4:
                            players = _homePlayers.FindAll(x => x.SFPosition == 1);
                            break;
                        case 5:
                            players = _homePlayers.FindAll(x => x.PFPosition == 1);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (position)
                    {
                        case 1:
                            players = _awayPlayers.FindAll(x => x.SGPosition == 1);
                            break;
                        case 2:
                            players = _awayPlayers.FindAll(x => x.SFPosition == 1);
                            break;
                        case 3:
                            players = _awayPlayers.FindAll(x => x.SGPosition == 1);
                            break;
                        case 4:
                            players = _awayPlayers.FindAll(x => x.SFPosition == 1);
                            break;
                        case 5:
                            players = _awayPlayers.FindAll(x => x.PFPosition == 1);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (playerSubbed == 0)
            {
                foreach (var player in players)
                {
                    var res = dc.FirstOrDefault(x => x.Position == position && x.PlayerId == player.Id);

                    if (res != null)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            if ((st != null) && (stam.StaminaValue > 500))
                            {
                                // Need action to make the sub
                                SubPlayer(team, position, player);
                                newPlayer = player;
                                playerSubbed = 1;
                                break;
                            }
                        }
                    }
                }
            }
            if (playerSubbed == 0)
            {
                // Need to go to fallback options
                if (team == 0)
                {
                    switch (position)
                    {
                        case 1:
                            players = _homePlayers.FindAll(x => x.SFPosition == 1);
                            break;
                        case 2:
                            players = _homePlayers.FindAll(x => x.PGPosition == 1);
                            break;
                        case 3:
                            players = _homePlayers.FindAll(x => x.PFPosition == 1);
                            break;
                        case 4:
                            players = _homePlayers.FindAll(x => x.CPosition == 1);
                            break;
                        case 5:
                            players = _homePlayers.FindAll(x => x.SFPosition == 1);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (position)
                    {
                        case 1:
                            players = _awayPlayers.FindAll(x => x.SFPosition == 1);
                            break;
                        case 2:
                            players = _awayPlayers.FindAll(x => x.PGPosition == 1);
                            break;
                        case 3:
                            players = _awayPlayers.FindAll(x => x.PFPosition == 1);
                            break;
                        case 4:
                            players = _awayPlayers.FindAll(x => x.CPosition == 1);
                            break;
                        case 5:
                            players = _awayPlayers.FindAll(x => x.SFPosition == 1);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (playerSubbed == 0)
            {
                foreach (var player in players)
                {
                    var res = dc.FirstOrDefault(x => x.Position == position && x.PlayerId == player.Id);

                    if (res != null)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            if ((st != null) && (stam.StaminaValue > 500))
                            {
                                // Need action to make the sub
                                SubPlayer(team, position, player);
                                newPlayer = player;
                                playerSubbed = 1;
                                break;
                            }
                        }
                    }
                }
            }
            if (playerSubbed == 0)
            {
                // Need to put another call in here to go to another sub check
                FinalSubCheck(team, position);
            }
            else
            {
                // Add the commentary here
                string outPlayer = current.FirstName + " " + current.Surname;
                string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;

                if (team == 0)
                {
                    commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                }
                else
                {
                    commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                }
            }
        }

        public void FinalSubCheck(int team, int position)
        {
            List<DepthChart> dc = new List<DepthChart>();
            List<Player> players = new List<Player>();
            Player current = new Player();
            Player newPlayer = new Player();
            List<StaminaTrack> st = new List<StaminaTrack>();
            int playerSubbed = 0;

            if (team == 0)
            {
                // Home
                dc = _homeDepth;
                players = _homePlayers;
                st = _homeStaminas;

                switch (position)
                {
                    case 1:
                        current = homePG;
                        break;
                    case 2:
                        current = homeSG;
                        break;
                    case 3:
                        current = homeSF;
                        break;
                    case 4:
                        current = homePF;
                        break;
                    case 5:
                        current = homeC;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Away
                dc = _awayDepth;
                players = _awayPlayers;
                st = _awayStaminas;

                switch (position)
                {
                    case 1:
                        current = awayPG;
                        break;
                    case 2:
                        current = awaySG;
                        break;
                    case 3:
                        current = awaySF;
                        break;
                    case 4:
                        current = awayPF;
                        break;
                    case 5:
                        current = awayC;
                        break;
                    default:
                        break;
                }
            }

            // Now need to work through the options
            List<Player> pgPlayers = players.FindAll(x => x.PGPosition == 1);
            List<Player> sgPlayers = players.FindAll(x => x.SGPosition == 1);
            List<Player> sfPlayers = players.FindAll(x => x.SFPosition == 1);
            List<Player> pfPlayers = players.FindAll(x => x.PFPosition == 1);
            List<Player> cPlayers = players.FindAll(x => x.CPosition == 1);

            if (position == 1)
            {
                // Now go through a specific order
                // PG
                foreach (var player in pgPlayers)
                {
                    int result = CheckSubEligility(player, team);
                    int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                    if (result == 0 && onCourt == 0) // can sub the player on
                    {
                        // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                        // if ((st != null) && (stam.StaminaValue > 500))
                        // {
                        // Need action to make the sub
                        SubPlayer(team, position, player);
                        newPlayer = player;
                        playerSubbed = 1;
                        break;
                        // }
                    }
                }

                // SG
                if (playerSubbed == 0)
                {
                    foreach (var player in sgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // SF
                if (playerSubbed == 0)
                {
                    foreach (var player in sfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }


                // PF
                if (playerSubbed == 0)
                {
                    foreach (var player in pfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // C    
                if (playerSubbed == 0)
                {
                    foreach (var player in cPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

            }
            else if (position == 2)
            {
                // SG
                if (playerSubbed == 0)
                {
                    foreach (var player in sgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // SF
                if (playerSubbed == 0)
                {
                    foreach (var player in sfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // PG
                if (playerSubbed == 0)
                {
                    foreach (var player in pgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // PF
                if (playerSubbed == 0)
                {
                    foreach (var player in pfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // C
                if (playerSubbed == 0)
                {
                    foreach (var player in cPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

            }
            else if (position == 3)
            {
                // SF
                if (playerSubbed == 0)
                {
                    foreach (var player in sfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // SG
                if (playerSubbed == 0)
                {
                    foreach (var player in sgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // PF
                if (playerSubbed == 0)
                {
                    foreach (var player in pfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // PG
                if (playerSubbed == 0)
                {
                    foreach (var player in pgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // C
                if (playerSubbed == 0)
                {
                    foreach (var player in cPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }
            }
            else if (position == 4)
            {
                // PF
                if (playerSubbed == 0)
                {
                    foreach (var player in pfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // C
                if (playerSubbed == 0)
                {
                    foreach (var player in cPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // SF
                if (playerSubbed == 0)
                {
                    foreach (var player in sfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // SG
                if (playerSubbed == 0)
                {
                    foreach (var player in sgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // PG
                if (playerSubbed == 0)
                {
                    foreach (var player in pgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }
            }
            else if (position == 5)
            {
                // C
                if (playerSubbed == 0)
                {
                    foreach (var player in cPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // PF
                if (playerSubbed == 0)
                {
                    foreach (var player in pfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // SF
                if (playerSubbed == 0)
                {
                    foreach (var player in sfPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // SG
                if (playerSubbed == 0)
                {
                    foreach (var player in sgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }

                // PG
                if (playerSubbed == 0)
                {
                    foreach (var player in pgPlayers)
                    {
                        int result = CheckSubEligility(player, team);
                        int onCourt = CheckIfPlayerIsOnCourt(team, player.Id);

                        if (result == 0 && onCourt == 0) // can sub the player on
                        {
                            // var stam = st.FirstOrDefault(x => x.PlayerId == player.Id);
                            // if ((st != null) && (stam.StaminaValue > 500))
                            // {
                            // Need action to make the sub
                            SubPlayer(team, position, player);
                            newPlayer = player;
                            playerSubbed = 1;
                            break;
                            // }
                        }
                    }
                }
            }

            if (playerSubbed == 0)
            {
                throw new Exception("No player to sub on");
            }
            else
            {
                // Need to add commentary here
                string outPlayer = current.FirstName + " " + current.Surname;
                string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;

                if (team == 0)
                {
                    commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                }
                else
                {
                    commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                }
            }
        }

        public void SubToStarters()
        {
            Player current = new Player();
            Player newPlayer = new Player();
            int playerSubbed = 0;

            // Home team first
            for (int i = 1; i < 6; i++)
            {
                switch (i)
                {
                    case 1:
                        current = homePG;
                        break;
                    case 2:
                        current = homeSG;
                        break;
                    case 3:
                        current = homeSF;
                        break;
                    case 4:
                        current = homePF;
                        break;
                    case 5:
                        current = homeC;
                        break;
                    default:
                        break;
                }

                var dc = _homeDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == i);
                var result = _homeInjuries.Find(x => x.PlayerId == dc.PlayerId);
                var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);


                if ((result == null || result.Severity < 3) && dc.PlayerId != current.Id && bs.Fouls < 6)
                {
                    // Get the player to pass in
                    var player = _homePlayers.FirstOrDefault(x => x.Id == dc.PlayerId);

                    int onCourt = CheckIfPlayerIsOnCourt(0, player.Id);

                    if (onCourt == 0)
                    {
                    SubPlayer(0, i, player);
                    newPlayer = player;
                    playerSubbed = 1;

                    // Commentary
                    string outPlayer = current.FirstName + " " + current.Surname;
                    string inPlayer = player.FirstName + " " + player.Surname;

                    commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    }
                }
                else if (dc.PlayerId == current.Id)
                {
                    // Player is already on the court - nothing is required
                }
                else
                {
                    // So what if the starter cannot play?
                    var dc2 = _homeDepth.FirstOrDefault(x => x.Depth == 2 && x.Position == i);
                    var result2 = _homeInjuries.Find(x => x.PlayerId == dc2.PlayerId);
                    var bs2 = _homeBoxScores.FirstOrDefault(x => x.Id == dc2.PlayerId);

                    if ((result2 == null || result2.Severity < 3) && dc2.PlayerId != current.Id && bs2.Fouls < 6)
                    {
                        // Get the player to pass in
                        var player = _homePlayers.FirstOrDefault(x => x.Id == dc2.PlayerId);
                        int onCourt = CheckIfPlayerIsOnCourt(0, player.Id);

                        if (onCourt == 0)
                        {
                            SubPlayer(0, i, player);
                        newPlayer = player;
                        playerSubbed = 1;

                        // Commentary
                        string outPlayer = current.FirstName + " " + current.Surname;
                        string inPlayer = player.FirstName + " " + player.Surname;

                        commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                        }
                    }
                    else if (dc2.PlayerId == current.Id)
                    {
                        // Player is already on the court - nothing is required
                    }
                    else
                    {
                        // So what if the starter cannot play?
                        var dc3 = _homeDepth.FirstOrDefault(x => x.Depth == 3 && x.Position == i);
                        var result3 = _homeInjuries.Find(x => x.PlayerId == dc3.PlayerId);
                        var bs3 = _homeBoxScores.FirstOrDefault(x => x.Id == dc3.PlayerId);

                        if ((result3 == null || result3.Severity < 3) && dc3.PlayerId != current.Id && bs3.Fouls < 6)
                        {
                            // Get the player to pass in
                            var player = _homePlayers.FirstOrDefault(x => x.Id == dc3.PlayerId);
                            int onCourt = CheckIfPlayerIsOnCourt(0, player.Id);

                            if (onCourt == 0)
                            {
                                SubPlayer(0, i, player);
                            newPlayer = player;
                            playerSubbed = 1;

                            // Commentary
                            string outPlayer = current.FirstName + " " + current.Surname;
                            string inPlayer = player.FirstName + " " + player.Surname;

                            commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                            PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                            }
                        }
                        else if (dc3.PlayerId == current.Id)
                        {
                            // Player is already on the court - nothing is required
                        }
                        else
                        {
                            NoPlayersToSubIn(0, i);
                        }
                    }
                }
            }

            // Away team second
            for (int j = 1; j < 6; j++)
            {
                switch (j)
                {
                    case 1:
                        current = awayPG;
                        break;
                    case 2:
                        current = awaySG;
                        break;
                    case 3:
                        current = awaySF;
                        break;
                    case 4:
                        current = awayPF;
                        break;
                    case 5:
                        current = awayC;
                        break;
                    default:
                        break;
                }

                var dc = _awayDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == j);
                var result = _awayInjuries.Find(x => x.PlayerId == dc.PlayerId);
                var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                if ((result == null || result.Severity < 3) && dc.PlayerId != current.Id && bs.Fouls < 6)
                {
                    // Get the player to pass in
                    var player = _awayPlayers.FirstOrDefault(x => x.Id == dc.PlayerId);
                    int onCourt = CheckIfPlayerIsOnCourt(1, player.Id);

                    if (onCourt == 0)
                    {
                    SubPlayer(1, j, player);
                    newPlayer = player;
                    playerSubbed = 1;

                    // Commentary
                    string outPlayer = current.FirstName + " " + current.Surname;
                    string inPlayer = player.FirstName + " " + player.Surname;

                    commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    }
                }
                else if (dc.PlayerId == current.Id)
                {
                    // Player is already on the court - nothing is required
                }
                else
                {
                    // So what if the starter cannot play?
                    var dc2 = _awayDepth.FirstOrDefault(x => x.Depth == 2 && x.Position == j);
                    var result2 = _awayInjuries.Find(x => x.PlayerId == dc2.PlayerId);
                    var bs2 = _awayBoxScores.FirstOrDefault(x => x.Id == dc2.PlayerId);

                    if ((result2 == null || result2.Severity < 3) && dc2.PlayerId != current.Id && bs2.Fouls < 6)
                    {
                        // Get the player to pass in
                        var player = _awayPlayers.FirstOrDefault(x => x.Id == dc2.PlayerId);
                        int onCourt = CheckIfPlayerIsOnCourt(0, player.Id);

                        if (onCourt == 0)
                        {
                        SubPlayer(1, j, player);
                        newPlayer = player;
                        playerSubbed = 1;

                        // Commentary
                        string outPlayer = current.FirstName + " " + current.Surname;
                        string inPlayer = player.FirstName + " " + player.Surname;

                        commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                        }
                    }
                    else if (dc2.PlayerId == current.Id)
                    {
                        // Player is already on the court - nothing is required
                    }
                    else
                    {
                        // So what if the starter cannot play?
                        var dc3 = _awayDepth.FirstOrDefault(x => x.Depth == 3 && x.Position == j);
                        var result3 = _awayInjuries.Find(x => x.PlayerId == dc3.PlayerId);
                        var bs3 = _awayBoxScores.FirstOrDefault(x => x.Id == dc3.PlayerId);

                        if ((result3 == null || result3.Severity < 3) && dc3.PlayerId != current.Id && bs3.Fouls < 6)
                        {
                            // Get the player to pass in
                            var player = _awayPlayers.FirstOrDefault(x => x.Id == dc3.PlayerId);
                            int onCourt = CheckIfPlayerIsOnCourt(0, player.Id);

                            if (onCourt == 0)
                            {
                            SubPlayer(1, j, player);
                            newPlayer = player;
                            playerSubbed = 1;

                            // Commentary
                            string outPlayer = current.FirstName + " " + current.Surname;
                            string inPlayer = player.FirstName + " " + player.Surname;

                            commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                            PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                            }
                        }
                        else if (dc3.PlayerId == current.Id)
                        {
                            // Player is already on the court - nothing is required
                        }
                        else
                        {
                            NoPlayersToSubIn(1, j);
                        }
                    }
                }
            }
        }


        public int ScoreDiffCheck()
        {
            return _awayScore - _homeScore;
        }

        // STATS & TRACKING
        /* REFACTORED */
        public void UpdateTimeInBoxScores(int time)
        {
            int minutes;
            int index;

            // Home PG
            BoxScore homePGBS = _homeBoxScores.Find(x => x.Id == homePG.Id);
            minutes = homePGBS.Minutes;
            homePGBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homePG.Id);
            _homeBoxScores[index] = homePGBS;

            // Home SG
            BoxScore homeSGBS = _homeBoxScores.Find(x => x.Id == homeSG.Id);
            minutes = homeSGBS.Minutes;
            homeSGBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homeSG.Id);
            _homeBoxScores[index] = homeSGBS;

            // Home SF
            BoxScore homeSFBS = _homeBoxScores.Find(x => x.Id == homeSF.Id);
            minutes = homeSFBS.Minutes;
            homeSFBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homeSF.Id);
            _homeBoxScores[index] = homeSFBS;

            // Home PF
            BoxScore homePFBS = _homeBoxScores.Find(x => x.Id == homePF.Id);
            minutes = homePFBS.Minutes;
            homePFBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homePF.Id);
            _homeBoxScores[index] = homePFBS;

            // Home C
            BoxScore homeCBS = _homeBoxScores.Find(x => x.Id == homeC.Id);
            minutes = homeCBS.Minutes;
            homeCBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
            _homeBoxScores[index] = homeCBS;

            // Away PG
            BoxScore awayPGBS = _awayBoxScores.Find(x => x.Id == awayPG.Id);
            minutes = awayPGBS.Minutes;
            awayPGBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awayPG.Id);
            _awayBoxScores[index] = awayPGBS;

            // Away SG
            BoxScore awaySGBS = _awayBoxScores.Find(x => x.Id == awaySG.Id);
            minutes = awaySGBS.Minutes;
            awaySGBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awaySG.Id);
            _awayBoxScores[index] = awaySGBS;

            // Away SF
            BoxScore awaySFBS = _awayBoxScores.Find(x => x.Id == awaySF.Id);
            minutes = awaySFBS.Minutes;
            awaySFBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awaySF.Id);
            _awayBoxScores[index] = awaySFBS;

            // Away PF
            BoxScore awayPFBS = _awayBoxScores.Find(x => x.Id == awayPF.Id);
            minutes = awayPFBS.Minutes;
            awayPFBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awayPF.Id);
            _awayBoxScores[index] = awayPFBS;

            // Away C
            BoxScore awayCBS = _awayBoxScores.Find(x => x.Id == awayC.Id);
            minutes = awayCBS.Minutes;
            awayCBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
            _awayBoxScores[index] = awayCBS;
        }

        /* REFACTORED */
        public void StaminaUpdates(int time)
        {
            int awayStaminaStrategy = 120 - (60 - _awayStaminaStrategy);
            int homeStaminaStrategy = 120 - (60 - _homeStaminaStrategy);

            // Away Team
            for (int a = 0; a < _awayStaminas.Count; a++)
            {
                StaminaTrack st = _awayStaminas[a];

                if (st.OnOff == 1)
                {
                    // player is on the court
                    // Get the players stamina rating
                    PlayerRating current = _awayRatings.Find(x => x.PlayerId == st.PlayerId);

                    // Determine how much of the rating gets applied for the time passed
                    decimal percantageToApply = (decimal)time / 60;
                    decimal staminaUpdateAmount = current.StaminaRating * percantageToApply;
                    // Update the players stamina rating
                    decimal currentStamina = st.StaminaValue;
                    st.StaminaValue = currentStamina + staminaUpdateAmount;

                    // Add the StaminaTrack record back into the awayStaminas list
                    int index = _awayStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                    _awayStaminas[index] = st;
                }
                else
                {
                    // player is off the court
                    // Do this check as if the player has not stamina effect that can not receive any more
                    if (st.StaminaValue > 0)
                    {
                        // Get the players stamina rating
                        PlayerRating current = _awayRatings.Find(x => x.PlayerId == st.PlayerId);

                        // Determine how much of the rating gets applied for the time passed
                        decimal percantageToApply = (decimal)time / awayStaminaStrategy; // making this have half the effect of playing
                        decimal staminaUpdateAmount = current.StaminaRating * percantageToApply;

                        // Update the players stamina rating
                        decimal currentStamina = st.StaminaValue;
                        st.StaminaValue = currentStamina - staminaUpdateAmount;

                        // Add the StaminaTrack record back into the awayStaminas list
                        int index = _awayStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                        _awayStaminas[index] = st;
                    }
                }
            }

            // Home Team
            for (int h = 0; h < _homeStaminas.Count; h++)
            {
                StaminaTrack st = _homeStaminas[h];

                if (st.OnOff == 1)
                {
                    // player is on the court
                    // Get the players stamina rating
                    PlayerRating current = _homeRatings.Find(x => x.PlayerId == st.PlayerId);

                    // Determine how much of the rating gets applied for the time passed
                    decimal percantageToApply = (decimal)time / 60;
                    decimal staminaUpdateAmount = current.StaminaRating * percantageToApply;

                    // Update the players stamina rating
                    decimal currentStamina = st.StaminaValue;
                    st.StaminaValue = currentStamina + staminaUpdateAmount;

                    // Add the StaminaTrack record back into the awayStaminas list
                    int index = _homeStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                    _homeStaminas[index] = st;
                }
                else
                {
                    // player is off the court
                    // Do this check as if the player has not stamina effect that can not receive any more
                    if (st.StaminaValue > 0)
                    {
                        // Get the players stamina rating
                        PlayerRating current = _homeRatings.Find(x => x.PlayerId == st.PlayerId);

                        // Determine how much of the rating gets applied for the time passed
                        decimal percantageToApply = (decimal)time / homeStaminaStrategy; // making this have half the effect of playing
                        decimal staminaUpdateAmount = current.StaminaRating * percantageToApply;

                        // Update the players stamina rating
                        decimal currentStamina = st.StaminaValue;
                        st.StaminaValue = currentStamina - staminaUpdateAmount;

                        // Add the StaminaTrack record back into the awayStaminas list
                        int index = _homeStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                        _homeStaminas[index] = st;
                    }
                }
            }
        }

        public int StaminaEffect(int playerid, int team, int rating)
        {
            if (team == 0)
            {
                // Home
                StaminaTrack st = _homeStaminas.Find(x => x.PlayerId == playerid);
                decimal effect = st.StaminaValue / 90;
                int value = (int)effect;

                if (value == 0)
                {
                    return rating;
                }
                else
                {
                    double multiplier = (double)value / 100;
                    double newRating = (double)rating * multiplier;
                    int returnValue = rating - (int)newRating;
                    return returnValue;
                }
            }
            else
            {
                // Away
                StaminaTrack st = _awayStaminas.Find(x => x.PlayerId == playerid);
                int effect = (int)st.StaminaValue / 90;
                int value = (int)effect;

                if (value == 0)
                {
                    return rating;
                }
                else
                {
                    double multiplier = (double)value / 100;
                    double newRating = (double)rating * multiplier;
                    int returnValue = rating - (int)newRating;
                    return returnValue;
                }
            }
        }

        public void StaminaQuarterEffects(int quarter)
        {
            decimal value = 0;
            if (quarter == 1 || quarter == 3)
            {
                value = 1;
            }
            else if (quarter == 2)
            {
                value = (decimal)2.5;
            }

            // Away Team
            for (int a = 0; a < _awayStaminas.Count; a++)
            {
                StaminaTrack st = _awayStaminas[a];

                // Get the players stamina rating
                PlayerRating current = _awayRatings.Find(x => x.PlayerId == st.PlayerId);

                // Determine how much of the rating gets applied for the time passed
                decimal percantageToApply = value;
                decimal staminaUpdateAmount = current.StaminaRating * percantageToApply;

                // Update the players stamina rating
                decimal currentStamina = st.StaminaValue;
                st.StaminaValue = currentStamina - staminaUpdateAmount;

                // Add the StaminaTrack record back into the awayStaminas list
                int index = _awayStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                _awayStaminas[index] = st;
            }

            // Home Team
            for (int h = 0; h < _homeStaminas.Count; h++)
            {
                StaminaTrack st = _homeStaminas[h];

                // Get the players stamina rating
                PlayerRating current = _homeRatings.Find(x => x.PlayerId == st.PlayerId);

                // Determine how much of the rating gets applied for the time passed
                decimal percantageToApply = value;
                decimal staminaUpdateAmount = current.StaminaRating * percantageToApply;

                // Update the players stamina rating
                decimal currentStamina = st.StaminaValue;
                st.StaminaValue = currentStamina - staminaUpdateAmount;

                // Add the StaminaTrack record back into the awayStaminas list
                int index = _homeStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                _homeStaminas[index] = st;
            }
        }

        public void InjuryCheck()
        {
            // Go through Away players first
            List<Player> currentAwayPlayers = new List<Player>();
            currentAwayPlayers.Add(awayPG);
            currentAwayPlayers.Add(awaySG);
            currentAwayPlayers.Add(awaySF);
            currentAwayPlayers.Add(awayPF);
            currentAwayPlayers.Add(awayC);

            // Now go through home players
            List<Player> currentHomePlayers = new List<Player>();
            currentHomePlayers.Add(homePG);
            currentHomePlayers.Add(homeSG);
            currentHomePlayers.Add(homeSF);
            currentHomePlayers.Add(homePF);
            currentHomePlayers.Add(homeC);

            foreach (var player in currentAwayPlayers)
            {
                int result = _random.Next(1, 6001);

                if (result > 5990)
                {
                    // Player has been injured and needs to be actioned accordingly
                    // Injury severity
                    int severity = InjurySeverityValue();

                    List<InjuryType> injuryTypes = _repo.GetInjuryTypesForSeverity(severity);
                    int count = injuryTypes.Count;
                    int injuryNumber = _random.Next(0, count);
                    InjuryType injury = injuryTypes[injuryNumber];

                    int impact = 0;
                    if (severity >= 3)
                    {
                        impact = 1;
                    }

                    int staminaImpact = 0;
                    int timeLength = 0;
                    int endQuarter = _quarter;

                    if (severity <= 2)
                    {
                        staminaImpact = (severity * 100);
                        timeLength = _random.Next(200, 1200);

                        if (timeLength > _time)
                        {
                            endQuarter = endQuarter++;

                            int temp = timeLength - _time;
                            timeLength = timeLength - temp;
                        }
                        else
                        {
                            timeLength = _time - timeLength;
                        }
                    }

                    // Populate the injury DTO
                    InjuryDto injuryDto = new InjuryDto
                    {
                        Id = 0,
                        InjuryTypeName = injury.Type,
                        Severity = severity,
                        PlayerId = player.Id,
                        Impact = impact,
                        StaminaImpact = staminaImpact,
                        StartQuarterImpact = _quarter,
                        EndQuarterImpact = endQuarter,
                        StartTimeImpact = _time,
                        EndTimeImpact = timeLength,
                    };

                    _awayInjuries.Add(injuryDto);

                    // Next item of the injury process

                    // Need to add the commentary
                    string playerName = player.FirstName + " " + player.Surname;
                    commentaryData.Add(comm.GetInjuryCommentary(playerName, severity, injury.Type));
                    PlayByPlayTracker(comm.GetInjuryCommentary(playerName, severity, injury.Type), 0);

                    // The injury sub will be included in sub check and sub free throw checks
                }
            }

            foreach (var player in currentHomePlayers)
            {
                int result = _random.Next(1, 6001);


                if (result > 5990)
                {
                    // Player has been injured and needs to be actioned accordingly
                    // Injury severity
                    int severity = InjurySeverityValue();

                    List<InjuryType> injuryTypes = _repo.GetInjuryTypesForSeverity(severity);
                    int count = injuryTypes.Count;
                    int injuryNumber = _random.Next(0, count);
                    InjuryType injury = injuryTypes[injuryNumber];

                    int impact = 0;
                    if (severity >= 3)
                    {
                        impact = 1;
                    }

                    int staminaImpact = 0;
                    int timeLength = 0;
                    int endQuarter = _quarter;

                    if (severity <= 2)
                    {
                        staminaImpact = (severity * 100);
                        timeLength = _random.Next(200, 1200);

                        if (timeLength > _time)
                        {
                            endQuarter = endQuarter++;

                            int temp = timeLength - _time;
                            timeLength = timeLength - temp;
                        }
                        else
                        {
                            timeLength = _time - timeLength;
                        }
                    }

                    // Populate the injury DTO
                    InjuryDto injuryDto = new InjuryDto
                    {
                        Id = 0,
                        InjuryTypeName = injury.Type,
                        Severity = severity,
                        PlayerId = player.Id,
                        Impact = impact,
                        StaminaImpact = staminaImpact,
                        StartQuarterImpact = _quarter,
                        EndQuarterImpact = endQuarter,
                        StartTimeImpact = _time,
                        EndTimeImpact = timeLength,
                    };

                    _homeInjuries.Add(injuryDto);

                    // Next item of the injury process

                    // Need to add the commentary
                    string playerName = player.FirstName + " " + player.Surname;
                    commentaryData.Add(comm.GetInjuryCommentary(playerName, severity, injury.Type));
                    PlayByPlayTracker(comm.GetInjuryCommentary(playerName, severity, injury.Type), 0);

                    // The injury sub will be included in sub check and sub free throw checks
                }
            }
        }

        public void InjuryStaminaChecks()
        {
            // Home - only need to check if sev 1 or 2
            var homeInjuries = _homeInjuries.FindAll(x => x.Severity == 1 || x.Severity == 2);
            foreach (var injury in homeInjuries)
            {
                // Firstly, check to see if the injury is still active
                if (injury.Impact == 0)
                {
                    // Need to check to see if the injury is to be applied
                    // Injury is still in play
                    // Same quarter injury
                    if (injury.StartQuarterImpact <= _quarter)
                    {
                        if (injury.StartQuarterImpact == _quarter && injury.StartTimeImpact <= _time)
                        {
                            StaminaTrack st = _homeStaminas.Find(x => x.PlayerId == injury.PlayerId);

                            // Update the players stamina rating
                            decimal currentStamina = st.StaminaValue;
                            st.StaminaValue = currentStamina + injury.StaminaImpact;

                            // Add the StaminaTrack record back into the awayStaminas list
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                            _homeStaminas[index] = st;
                            injury.Impact = 1;
                        }
                        else if (injury.StartQuarterImpact < _quarter)
                        {
                            StaminaTrack st = _homeStaminas.Find(x => x.PlayerId == injury.PlayerId);

                            // Update the players stamina rating
                            decimal currentStamina = st.StaminaValue;
                            st.StaminaValue = currentStamina + injury.StaminaImpact;

                            // Add the StaminaTrack record back into the awayStaminas list
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                            _homeStaminas[index] = st;
                            injury.Impact = 1;
                        }
                    }
                }
                else
                {
                    // Need to check if the injury has expired
                    if (injury.EndQuarterImpact <= _quarter)
                    {
                        // Filters out the injuries that are before the end quarter
                        if (injury.EndQuarterImpact == _quarter && injury.EndTimeImpact > _time)
                        {
                            injury.Impact = 0;
                        }
                        else if (injury.EndQuarterImpact < _quarter)
                        {
                            injury.Impact = 0;
                        }
                    }
                }
            }

            // Away
            var awayInjuries = _awayInjuries.FindAll(x => x.Severity == 1 || x.Severity == 2);
            foreach (var injury in awayInjuries)
            {
                // Firstly, check to see if the injury is still active
                if (injury.Impact == 0)
                {
                    // Need to check to see if the injury is to be applied
                    // Injury is still in play
                    // Same quarter injury
                    if (injury.StartQuarterImpact <= _quarter)
                    {
                        if (injury.StartQuarterImpact == _quarter && injury.StartTimeImpact <= _time)
                        {
                            StaminaTrack st = _awayStaminas.Find(x => x.PlayerId == injury.PlayerId);

                            // Update the players stamina rating
                            decimal currentStamina = st.StaminaValue;
                            st.StaminaValue = currentStamina + injury.StaminaImpact;

                            // Add the StaminaTrack record back into the awayStaminas list
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                            _awayStaminas[index] = st;
                            injury.Impact = 1;
                        }
                        else if (injury.StartQuarterImpact < _quarter)
                        {
                            StaminaTrack st = _awayStaminas.Find(x => x.PlayerId == injury.PlayerId);

                            // Update the players stamina rating
                            decimal currentStamina = st.StaminaValue;
                            st.StaminaValue = currentStamina + injury.StaminaImpact;

                            // Add the StaminaTrack record back into the awayStaminas list
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                            _awayStaminas[index] = st;
                            injury.Impact = 1;
                        }
                    }
                }
                else
                {
                    // Need to check if the injury has expired
                    if (injury.EndQuarterImpact <= _quarter)
                    {
                        // Filters out the injuries that are before the end quarter
                        if (injury.EndQuarterImpact == _quarter && injury.EndTimeImpact > _time)
                        {
                            injury.Impact = 0;
                        }
                        else if (injury.EndQuarterImpact < _quarter)
                        {
                            injury.Impact = 0;
                        }
                    }
                }
            }
        }

        public int InjurySeverityValue()
        {
            int injurySeverity = 0;
            int severityResult = _random.Next(1, 101);

            if (severityResult <= 40)
            {
                injurySeverity = 1;
            }
            else if (severityResult > 40 && severityResult <= 65)
            {
                injurySeverity = 2;
            }
            else if (severityResult > 65 && severityResult <= 90)
            {
                injurySeverity = 3;
            }
            else if (severityResult > 90 && severityResult <= 97)
            {
                injurySeverity = 4;
            }
            else
            {
                injurySeverity = 5;
            }

            return injurySeverity;
        }

        /* REFACTORED */
        public void UpdateTimeInBoxScore(int time)
        {
            int minutes;
            int index;

            // Home PG
            BoxScore homePGBS = _homeBoxScores.Find(x => x.Id == homePG.Id);
            minutes = homePGBS.Minutes;
            homePGBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homePG.Id);
            _homeBoxScores[index] = homePGBS;

            // Home SG
            BoxScore homeSGBS = _homeBoxScores.Find(x => x.Id == homeSG.Id);
            minutes = homeSGBS.Minutes;
            homeSGBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homeSG.Id);
            _homeBoxScores[index] = homeSGBS;

            // Home SF
            BoxScore homeSFBS = _homeBoxScores.Find(x => x.Id == homeSF.Id);
            minutes = homeSFBS.Minutes;
            homeSFBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homeSF.Id);
            _homeBoxScores[index] = homeSFBS;

            // Home PF
            BoxScore homePFBS = _homeBoxScores.Find(x => x.Id == homePF.Id);
            minutes = homePFBS.Minutes;
            homePFBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homePF.Id);
            _homeBoxScores[index] = homePFBS;

            // Home C
            BoxScore homeCBS = _homeBoxScores.Find(x => x.Id == homeC.Id);
            minutes = homeCBS.Minutes;
            homeCBS.Minutes = minutes + time;
            index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
            _homeBoxScores[index] = homeCBS;

            // Away PG
            BoxScore awayPGBS = _awayBoxScores.Find(x => x.Id == awayPG.Id);
            minutes = awayPGBS.Minutes;
            awayPGBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awayPG.Id);
            _awayBoxScores[index] = awayPGBS;

            // Away SG
            BoxScore awaySGBS = _awayBoxScores.Find(x => x.Id == awaySG.Id);
            minutes = awaySGBS.Minutes;
            awaySGBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awaySG.Id);
            _awayBoxScores[index] = awaySGBS;

            // Away SF
            BoxScore awaySFBS = _awayBoxScores.Find(x => x.Id == awaySF.Id);
            minutes = awaySFBS.Minutes;
            awaySFBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awaySF.Id);
            _awayBoxScores[index] = awaySFBS;

            // Away PF
            BoxScore awayPFBS = _awayBoxScores.Find(x => x.Id == awayPF.Id);
            minutes = awayPFBS.Minutes;
            awayPFBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awayPF.Id);
            _awayBoxScores[index] = awayPFBS;

            // Away C
            BoxScore awayCBS = _awayBoxScores.Find(x => x.Id == awayC.Id);
            minutes = awayCBS.Minutes;
            awayCBS.Minutes = minutes + time;
            index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
            _awayBoxScores[index] = awayCBS;
        }

        public void UpdatePlusMinusBoxScore(int value)
        {
            if (_teamPossession == 0)
            {
                // home team scored
                int index;
                int plusminus;

                // Home PG
                BoxScore homePGBS = _homeBoxScores.Find(x => x.Id == homePG.Id);
                plusminus = homePGBS.PlusMinus;
                homePGBS.PlusMinus = plusminus + value;
                index = _homeBoxScores.FindIndex(x => x.Id == homePG.Id);
                _homeBoxScores[index] = homePGBS;

                // Home SG
                BoxScore homeSGBS = _homeBoxScores.Find(x => x.Id == homeSG.Id);
                plusminus = homeSGBS.PlusMinus;
                homeSGBS.PlusMinus = plusminus + value;
                index = _homeBoxScores.FindIndex(x => x.Id == homeSG.Id);
                _homeBoxScores[index] = homeSGBS;

                // Home SF
                BoxScore homeSFBS = _homeBoxScores.Find(x => x.Id == homeSF.Id);
                plusminus = homeSFBS.PlusMinus;
                homeSFBS.PlusMinus = plusminus + value;
                index = _homeBoxScores.FindIndex(x => x.Id == homeSF.Id);
                _homeBoxScores[index] = homeSFBS;

                // Home PF
                BoxScore homePFBS = _homeBoxScores.Find(x => x.Id == homePF.Id);
                plusminus = homePFBS.PlusMinus;
                homePFBS.PlusMinus = plusminus + value;
                index = _homeBoxScores.FindIndex(x => x.Id == homePF.Id);
                _homeBoxScores[index] = homePFBS;

                // Home C
                BoxScore homeCBS = _homeBoxScores.Find(x => x.Id == homeC.Id);
                plusminus = homeCBS.PlusMinus;
                homeCBS.PlusMinus = plusminus + value;
                index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
                _homeBoxScores[index] = homeCBS;

                // Away PG
                BoxScore awayPGBS = _awayBoxScores.Find(x => x.Id == awayPG.Id);
                plusminus = awayPGBS.PlusMinus;
                awayPGBS.PlusMinus = plusminus - value;
                index = _awayBoxScores.FindIndex(x => x.Id == awayPG.Id);
                _awayBoxScores[index] = awayPGBS;

                // Away SG
                BoxScore awaySGBS = _awayBoxScores.Find(x => x.Id == awaySG.Id);
                plusminus = awaySGBS.PlusMinus;
                awaySGBS.PlusMinus = plusminus - value;
                index = _awayBoxScores.FindIndex(x => x.Id == awaySG.Id);
                _awayBoxScores[index] = awaySGBS;

                // Away SF
                BoxScore awaySFBS = _awayBoxScores.Find(x => x.Id == awaySF.Id);
                plusminus = awaySFBS.PlusMinus;
                awaySFBS.PlusMinus = plusminus - value;
                index = _awayBoxScores.FindIndex(x => x.Id == awaySF.Id);
                _awayBoxScores[index] = awaySFBS;

                // Away PF
                BoxScore awayPFBS = _awayBoxScores.Find(x => x.Id == awayPF.Id);
                plusminus = awayPFBS.PlusMinus;
                awayPFBS.PlusMinus = plusminus - value;
                index = _awayBoxScores.FindIndex(x => x.Id == awayPF.Id);
                _awayBoxScores[index] = awayPFBS;

                // Away C
                BoxScore awayCBS = _awayBoxScores.Find(x => x.Id == awayC.Id);
                plusminus = awayCBS.PlusMinus;
                awayCBS.PlusMinus = plusminus - value;
                index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
                _awayBoxScores[index] = awayCBS;

            }
            else
            {
                // away team scored
                int index;
                int plusminus;

                // Home PG
                BoxScore homePGBS = _homeBoxScores.Find(x => x.Id == homePG.Id);
                plusminus = homePGBS.PlusMinus;
                homePGBS.PlusMinus = plusminus - value;
                index = _homeBoxScores.FindIndex(x => x.Id == homePG.Id);
                _homeBoxScores[index] = homePGBS;

                // Home SG
                BoxScore homeSGBS = _homeBoxScores.Find(x => x.Id == homeSG.Id);
                plusminus = homeSGBS.PlusMinus;
                homeSGBS.PlusMinus = plusminus - value;
                index = _homeBoxScores.FindIndex(x => x.Id == homeSG.Id);
                _homeBoxScores[index] = homeSGBS;

                // Home SF
                BoxScore homeSFBS = _homeBoxScores.Find(x => x.Id == homeSF.Id);
                plusminus = homeSFBS.PlusMinus;
                homeSFBS.PlusMinus = plusminus - value;
                index = _homeBoxScores.FindIndex(x => x.Id == homeSF.Id);
                _homeBoxScores[index] = homeSFBS;

                // Home PF
                BoxScore homePFBS = _homeBoxScores.Find(x => x.Id == homePF.Id);
                plusminus = homePFBS.PlusMinus;
                homePFBS.PlusMinus = plusminus - value;
                index = _homeBoxScores.FindIndex(x => x.Id == homePF.Id);
                _homeBoxScores[index] = homePFBS;

                // Home C
                BoxScore homeCBS = _homeBoxScores.Find(x => x.Id == homeC.Id);
                plusminus = homeCBS.PlusMinus;
                homeCBS.PlusMinus = plusminus - value;
                index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
                _homeBoxScores[index] = homeCBS;

                // Away PG
                BoxScore awayPGBS = _awayBoxScores.Find(x => x.Id == awayPG.Id);
                plusminus = awayPGBS.PlusMinus;
                awayPGBS.PlusMinus = plusminus + value;
                index = _awayBoxScores.FindIndex(x => x.Id == awayPG.Id);
                _awayBoxScores[index] = awayPGBS;

                // Away SG
                BoxScore awaySGBS = _awayBoxScores.Find(x => x.Id == awaySG.Id);
                plusminus = awaySGBS.PlusMinus;
                awaySGBS.PlusMinus = plusminus + value;
                index = _awayBoxScores.FindIndex(x => x.Id == awaySG.Id);
                _awayBoxScores[index] = awaySGBS;

                // Away SF
                BoxScore awaySFBS = _awayBoxScores.Find(x => x.Id == awaySF.Id);
                plusminus = awaySFBS.PlusMinus;
                awaySFBS.PlusMinus = plusminus + value;
                index = _awayBoxScores.FindIndex(x => x.Id == awaySF.Id);
                _awayBoxScores[index] = awaySFBS;

                // Away PF
                BoxScore awayPFBS = _awayBoxScores.Find(x => x.Id == awayPF.Id);
                plusminus = awayPFBS.PlusMinus;
                awayPFBS.PlusMinus = plusminus + value;
                index = _awayBoxScores.FindIndex(x => x.Id == awayPF.Id);
                _awayBoxScores[index] = awayPFBS;

                // Away C
                BoxScore awayCBS = _awayBoxScores.Find(x => x.Id == awayC.Id);
                plusminus = awayCBS.PlusMinus;
                awayCBS.PlusMinus = plusminus + value;
                index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
                _awayBoxScores[index] = awayCBS;
            }
        }

        /* REFACTORED */
        public Player UpdateFouler(int playerPos)
        {
            Player playerFouling = null;
            int playerId = 0;

            if (_teamPossession == 1)
            {
                switch (playerPos)
                {
                    case 1:
                        playerId = homePG.Id;
                        playerFouling = homePG;
                        break;
                    case 2:
                        playerId = homeSG.Id;
                        playerFouling = homeSG;
                        break;
                    case 3:
                        playerId = homeSF.Id;
                        playerFouling = homeSF;
                        break;
                    case 4:
                        playerId = homePF.Id;
                        playerFouling = homePF;
                        break;
                    case 5:
                        playerId = homeC.Id;
                        playerFouling = homeC;
                        break;
                    default:
                        break;
                }

                BoxScore temp = _homeBoxScores.Find(x => x.Id == playerId);
                temp.Fouls++;
                int index = _homeBoxScores.FindIndex(x => x.Id == playerId);
                _homeBoxScores[index] = temp;
            }
            else
            {
                switch (playerPos)
                {
                    case 1:
                        playerId = awayPG.Id;
                        playerFouling = awayPG;
                        break;
                    case 2:
                        playerId = awaySG.Id;
                        playerFouling = awaySG;
                        break;
                    case 3:
                        playerId = awaySF.Id;
                        playerFouling = awaySF;
                        break;
                    case 4:
                        playerId = awayPF.Id;
                        playerFouling = awayPF;
                        break;
                    case 5:
                        playerId = awayC.Id;
                        playerFouling = awayC;
                        break;
                    default:
                        break;
                }
                BoxScore temp = _awayBoxScores.Find(x => x.Id == playerId);
                temp.Fouls++;
                int index = _awayBoxScores.FindIndex(x => x.Id == playerId);
                _awayBoxScores[index] = temp;
            }
            return playerFouling;
        }

        // HELPERS
        /* REFACTORED */
        public string GetPlayerFullNameForPosition(int teamPos, int playerId)
        {
            if (teamPos == 1)
            {
                // Home
                var player = _homePlayers.Find(x => x.Id == playerId);
                return player.FirstName + " " + player.Surname;
            }
            else
            {
                // Away
                var player = _awayPlayers.Find(x => x.Id == playerId);
                return player.FirstName + " " + player.Surname;
            }
        }

        public string GetPlayerFullNameForPositionForFouler(int teamPos, int playerId)
        {
            if (teamPos == 0)
            {
                // Home
                var player = _homePlayers.Find(x => x.Id == playerId);
                return player.FirstName + " " + player.Surname;
            }
            else
            {
                // Away
                var player = _awayPlayers.Find(x => x.Id == playerId);
                return player.FirstName + " " + player.Surname;
            }
        }

        /* REFACTORED */
        public string GetCurrentPlayerFullName()
        {
            string name = "";
            if (_teamPossession == 0)
            {
                // Home
                switch (_playerPossession)
                {
                    case 1:
                        name = homePG.FirstName + " " + homePG.Surname;
                        break;
                    case 2:
                        name = homeSG.FirstName + " " + homeSG.Surname;
                        break;
                    case 3:
                        name = homeSF.FirstName + " " + homeSF.Surname;
                        break;
                    case 4:
                        name = homePF.FirstName + " " + homePF.Surname;
                        break;
                    case 5:
                        name = homeC.FirstName + " " + homeC.Surname;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Away
                switch (_playerPossession)
                {
                    case 1:
                        name = awayPG.FirstName + " " + awayPG.Surname;
                        break;
                    case 2:
                        name = awaySG.FirstName + " " + awaySG.Surname;
                        break;
                    case 3:
                        name = awaySF.FirstName + " " + awaySF.Surname;
                        break;
                    case 4:
                        name = awayPF.FirstName + " " + awayPF.Surname;
                        break;
                    case 5:
                        name = awayC.FirstName + " " + awayC.Surname;
                        break;
                    default:
                        break;
                }
            }
            return name;
        }

        /* REFACTORED */
        public PlayerRating GetCurrentPlayersRatings()
        {
            PlayerRating current = new PlayerRating();
            if (_teamPossession == 0)
            {
                // Home
                switch (_playerPossession)
                {
                    case 1:
                        current = homePGRatings;
                        break;
                    case 2:
                        current = homeSGRatings;
                        break;
                    case 3:
                        current = homeSFRatings;
                        break;
                    case 4:
                        current = homePFRatings;
                        break;
                    case 5:
                        current = homeCRatings;
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                // Away
                switch (_playerPossession)
                {
                    case 1:
                        current = awayPGRatings;
                        break;
                    case 2:
                        current = awaySGRatings;
                        break;
                    case 3:
                        current = awaySFRatings;
                        break;
                    case 4:
                        current = awayPFRatings;
                        break;
                    case 5:
                        current = awayCRatings;
                        break;
                    default:
                        return null;
                }
            }
            return current;
        }

        /* REFACTORED */
        public PlayerTendancy GetCurrentPlayersTendancies()
        {
            PlayerTendancy current = new PlayerTendancy();
            if (_teamPossession == 0)
            {
                // Home
                switch (_playerPossession)
                {
                    case 1:
                        current = homePGTendancy;
                        break;
                    case 2:
                        current = homeSGTendancy;
                        break;
                    case 3:
                        current = homeSGTendancy;
                        break;
                    case 4:
                        current = homePFTendancy;
                        break;
                    case 5:
                        current = homeCTendancy;
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                // Away
                switch (_playerPossession)
                {
                    case 1:
                        current = awayPGTendancy;
                        break;
                    case 2:
                        current = awaySGTendancy;
                        break;
                    case 3:
                        current = awaySFTendancy;
                        break;
                    case 4:
                        current = awayPFTendancy;
                        break;
                    case 5:
                        current = awayCTendancy;
                        break;
                    default:
                        return null;
                }
            }
            return current;
        }

        public int GetPlayerIdForPosition(int team, int position)
        {
            if (team == 0)
            {
                switch (position)
                {
                    case 1:
                        return homePG.Id;
                    case 2:
                        return homeSG.Id;
                    case 3:
                        return homeSF.Id;
                    case 4:
                        return homePF.Id;
                    case 5:
                        return homeC.Id;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (position)
                {
                    case 1:
                        return awayPG.Id;
                    case 2:
                        return awaySG.Id;
                    case 3:
                        return awaySF.Id;
                    case 4:
                        return awayPF.Id;
                    case 5:
                        return awayC.Id;
                    default:
                        return 0;
                }
            }
        }

        /* REFACTORED */
        public int CheckCurrentPosition(int playerId)
        {
            if (awayPG.Id == playerId)
            {
                return 1;
            }
            else if (awaySG.Id == playerId)
            {
                return 2;
            }
            else if (awaySF.Id == playerId)
            {
                return 3;
            }
            else if (awayPF.Id == playerId)
            {
                return 4;
            }
            else if (awayC.Id == playerId)
            {
                return 5;
            }
            else if (homePG.Id == playerId)
            {
                return 1;
            }
            else if (homeSG.Id == playerId)
            {
                return 2;
            }
            else if (homeSF.Id == playerId)
            {
                return 3;
            }
            else if (homePF.Id == playerId)
            {
                return 4;
            }
            else if (homeC.Id == playerId)
            {
                return 5;
            }
            return 0;
        }

        /* REFACTORED */
        [HttpGet("getboxscoresforgameid/{gameId}")]
        public async Task<IEnumerable<BoxScore>> GetBoxScoresForGameId(int gameId)
        {
            var boxScores = await _repo.GetBoxScoresForGameId(gameId);
            return boxScores;
        }

        [HttpGet("getboxscoresforgameidplayoffs/{gameId}")]
        public async Task<IEnumerable<BoxScore>> GetBoxScoresForGameIdPlayoffs(int gameId)
        {
            var boxScores = await _repo.GetBoxScoresForGameIdPlayoffs(gameId);
            return boxScores;
        }

        public void PlayByPlayTracker(string commentary, int endOfPlay)
        {
            PlayByPlay pbp = new PlayByPlay
            {
                GameId = _game.GameId,
                Ordering = _ordering,
                PlayNumber = _playNumber,
                Commentary = commentary
            };
            _playByPlays.Add(pbp);

            _ordering++;

            if (endOfPlay == 1)
            {
                _playNumber++;
            }
        }

        public int GetPlayerPositionForPlayerId(int playerId, int team)
        {
            if (team == 0)
            {
                // Home
                if (homePG.Id == playerId)
                {
                    return 1;
                }
                else if (homeSG.Id == playerId)
                {
                    return 2;
                }
                else if (homeSF.Id == playerId)
                {
                    return 3;
                }
                else if (homePF.Id == playerId)
                {
                    return 4;
                }
                else if (homeC.Id == playerId)
                {
                    return 5;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                // Away
                if (awayPG.Id == playerId)
                {
                    return 1;
                }
                else if (awaySG.Id == playerId)
                {
                    return 2;
                }
                else if (awaySF.Id == playerId)
                {
                    return 3;
                }
                else if (awayPF.Id == playerId)
                {
                    return 4;
                }
                else if (awayC.Id == playerId)
                {
                    return 5;
                }
                else
                {
                    return 0;
                }
            }
        }

        #region Sub Code Rebuild

        // This function will replace - CheckForSubs calls
        public void SubstitutionCheck(int ftPlayerId)
        {
            if ((_quarter > 3 && _time <= 240) || (_quarter > 4))
            {
                EndGameSubCheck(ftPlayerId);
            }
            else
            {
                for (int i = 1; i < 6; i++)
                {
                    int result = 0;
                    int toSub = 0;

                    // Check to see if the player is at the free throw line
                    result = CheckIfShooter(0, i, ftPlayerId);
                    Player p = new Player();

                    if (result != 1)
                    {
                        switch (i)
                        {
                            case 1:
                                p = homePG;
                                break;
                            case 2:
                                p = homeSG;
                                break;
                            case 3:
                                p = homeSF;
                                break;
                            case 4:
                                p = homePF;
                                break;
                            case 5:
                                p = homeC;
                                break;
                            default:
                                break;
                        }

                        toSub = CheckIfPlayerNeedsSub(p.Id, 0, i);
                    }

                    // Now to action what should happen with this player
                    if (toSub != 0)
                    {
                        Player newPlayer = new Player();
                        // The player could conceivably stay on
                        if (toSub == 1 || toSub == 2)
                        {
                            // The player can be subbed out
                            newPlayer = FindPlayerToBeSubbed(p.Id, 0, i);

                            if (newPlayer == null)
                            {
                                // We do not have a player on.
                                // Realistically this should not happen for a toSub 1 or 2 as the player should continue
                            }

                            if (newPlayer.Id != p.Id)
                            {
                                // The Player is to be subbed
                                SubPlayer(0, i, newPlayer); // Covered in function

                                // Add the commentary here
                                string outPlayer = p.FirstName + " " + p.Surname;
                                string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                                // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                                PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                            }
                        }
                        else
                        {
                            // Then the player has a concern and is possibly requiring to be subbed out
                            newPlayer = FindPlayerToBeSubbedMandatory(p.Id, 0, i);

                            // Now we go through the SubPlayer actions 
                            SubPlayer(0, i, newPlayer); // Covered in above function
                            // Add the commentary here
                            string outPlayer = p.FirstName + " " + p.Surname;
                            string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                            // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                            PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                        }
                    }
                }

                for (int i = 1; i < 6; i++)
                {
                    int result = 0;
                    int toSub = 0;

                    // Check to see if the player is at the free throw line
                    result = CheckIfShooter(1, i, ftPlayerId);
                    Player p = new Player();

                    if (result != 1)
                    {
                        switch (i)
                        {
                            case 1:
                                p = awayPG;
                                break;
                            case 2:
                                p = awaySG;
                                break;
                            case 3:
                                p = awaySF;
                                break;
                            case 4:
                                p = awayPF;
                                break;
                            case 5:
                                p = awayC;
                                break;
                            default:
                                break;
                        }

                        toSub = CheckIfPlayerNeedsSub(p.Id, 1, i);
                    }

                    // Now to action what should happen with this player
                    if (toSub != 0)
                    {
                        Player newPlayer = new Player();
                        // The player could conceivably stay on
                        if (toSub == 1 || toSub == 2)
                        {
                            // The player can be subbed out
                            newPlayer = FindPlayerToBeSubbed(p.Id, 1, i);

                            if (newPlayer == null)
                            {
                                // We do not have a player on.
                                // Realistically this should not happen for a toSub 1 or 2 as the player should continue
                            }

                            if (newPlayer.Id != p.Id)
                            {
                                // The Player is to be subbed
                                SubPlayer(1, i, newPlayer);

                                string outPlayer = p.FirstName + " " + p.Surname;
                                string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                                // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                                PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                            }
                        }
                        else
                        {
                            // Then the player has a concern and is possibly requiring to be subbed out
                            newPlayer = FindPlayerToBeSubbedMandatory(p.Id, 1, i);

                            // Now we go through the SubPlayer actions
                            SubPlayer(1, 1, newPlayer);

                            string outPlayer = p.FirstName + " " + p.Surname;
                            string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                            // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                            PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                        }
                    }
                }
            }
        }

        public int CheckIfPlayerNeedsSub(int playerId, int teamId, int position)
        {
            InjuryDto injuryCheck = null;
            BoxScore bs = null;
            if (teamId == 0)
            {
                injuryCheck = _homeInjuries.Find(x => x.PlayerId == playerId);
                bs = _homeBoxScores.FirstOrDefault(x => x.Id == playerId);
            }
            else
            {
                injuryCheck = _awayInjuries.Find(x => x.PlayerId == playerId);
                bs = _awayBoxScores.FirstOrDefault(x => x.Id == playerId);
            }
            // CheckSubEligility(p, 0); is replaced by below

            // Check to see if the player has fouled out
            if (bs.Fouls >= 6)
            {
                return 4;
            }

            // Check to see if player is injured
            if (injuryCheck != null)
            {
                if (injuryCheck.Severity > 2)
                {
                    return 3;
                }
            }

            if (_quarter == 1 && bs.Fouls == 2)
            {
                string t = "";
            }

            // Check to see if the player is in foul trouble
            int foulTrouble = FoulTroubleCheck(teamId, playerId);
            if (foulTrouble == 1)
            {
                return 2;
            }

            // Check to see if the player is fatigued
            int fatigued = CheckPlayerFatigue(teamId, position, playerId);
            if (fatigued == 1)
            {
                return 1;
            }
            return 0;
        }

        public Player FindPlayerToBeSubbed(int playerId, int team, int position)
        {
            // Now set up the filtered depth chart and teams players for later use
            List<DepthChart> filterDepth = new List<DepthChart>();
            List<Player> teamPlayers = new List<Player>();
            if (team == 0)
            {
                List<DepthChart> depth = _homeDepth.OrderBy(x => x.Depth).ToList();
                filterDepth = depth.Where(x => x.Position == position).ToList();
                teamPlayers = _homePlayers;

            }
            else
            {
                List<DepthChart> depth = _awayDepth.OrderBy(x => x.Depth).ToList();
                filterDepth = depth.Where(x => x.Position == position).ToList();
                teamPlayers = _awayPlayers;
            }

            for (int i = 0; i < filterDepth.Count; i++)
            {
                DepthChart dc = filterDepth[i];
                // Need to take out current player
                if (dc.PlayerId != 0 && dc.PlayerId != playerId)
                {
                    // Potential replacement player
                    Player p = teamPlayers.FirstOrDefault(x => x.Id == dc.PlayerId);

                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, p.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(p.Id, team, position);
                        if (result == 0)
                        {
                            // Player is totally fine to come on and can be subbed on
                            return p;
                        }
                    }
                }
            }

            // Now potentially we could not find a player to sub on initially, so next round of checks
            // Ignore fatigue in the depth chart
            // Firstly check to see if the current player is fatigued
            int currentResult = CheckIfPlayerNeedsSub(playerId, team, position);
            if (currentResult == 1)
            {
                // Current Player is fatigued but can continue
                Player p = teamPlayers.FirstOrDefault(x => x.Id == playerId);
                return p;
            }

            // Else we need to check through the depth charts
            for (int i = 0; i < filterDepth.Count; i++)
            {
                DepthChart dc = filterDepth[i];
                // Need to take out current player
                if (dc.PlayerId != 0 && dc.PlayerId != playerId)
                {
                    // Potential replacement player
                    Player p = teamPlayers.FirstOrDefault(x => x.Id == dc.PlayerId);

                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, p.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(p.Id, team, position);
                        if (result == 1)
                        {
                            // Player is fatigued but could continue
                            return p;
                        }
                    }
                }
            }

            // Now if we still have not got a player to sub on, we can ignore foul trouble
            // Secondly check to see if the current player is in foul trouble
            int currentFoulResult = CheckIfPlayerNeedsSub(playerId, team, position);
            if (currentFoulResult == 2)
            {
                // Current Player is in foul trouble but can continue
                Player p = teamPlayers.FirstOrDefault(x => x.Id == playerId);
                return p;
            }

            // Else we need to check through the depth charts
            for (int i = 0; i < filterDepth.Count; i++)
            {
                DepthChart dc = filterDepth[i];
                // Need to take out current player
                if (dc.PlayerId != 0 && dc.PlayerId != playerId)
                {
                    // Potential replacement player
                    Player p = teamPlayers.FirstOrDefault(x => x.Id == dc.PlayerId);

                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, p.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(p.Id, team, position);
                        if (result == 2)
                        {
                            // Player is in foul trouble but could continue
                            return p;
                        }
                    }
                }
            }
            return null;
        }

        public Player FindPlayerToBeSubbedMandatory(int playerId, int team, int position)
        {
            // Now set up the filtered depth chart and teams players for later use
            List<DepthChart> filterDepth = new List<DepthChart>();
            List<Player> teamPlayers = new List<Player>();
            if (team == 0)
            {
                List<DepthChart> depth = _homeDepth.OrderBy(x => x.Depth).ToList();
                filterDepth = depth.Where(x => x.Position == position).ToList();
                teamPlayers = _homePlayers;
            }
            else
            {
                List<DepthChart> depth = _awayDepth.OrderBy(x => x.Depth).ToList();
                filterDepth = depth.Where(x => x.Position == position).ToList();
                teamPlayers = _awayPlayers;
            }

            for (int i = 0; i < filterDepth.Count; i++)
            {
                DepthChart dc = filterDepth[i];
                // Need to take out current player
                if (dc.PlayerId != 0 && dc.PlayerId != playerId)
                {
                    // Potential replacement player
                    Player p = teamPlayers.FirstOrDefault(x => x.Id == dc.PlayerId);

                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, p.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(p.Id, team, position);
                        if (result == 0)
                        {
                            // Player is totally fine to come on and can be subbed on
                            return p;
                        }
                    }
                }
            }

            // Now potentially we could not find a player to sub on initially, so next round of checks
            // Ignore fatigue in the depth chart - we need to check through the depth charts
            for (int i = 0; i < filterDepth.Count; i++)
            {
                DepthChart dc = filterDepth[i];
                // Need to take out current player
                if (dc.PlayerId != 0 && dc.PlayerId != playerId)
                {
                    // Potential replacement player
                    Player p = teamPlayers.FirstOrDefault(x => x.Id == dc.PlayerId);

                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, p.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(p.Id, team, position);
                        if (result == 1)
                        {
                            // Player is fatigued but could continue
                            return p;
                        }
                    }
                }
            }

            // Now if we still have not got a player to sub on, we can ignore foul trouble - we need to check through the depth charts
            for (int i = 0; i < filterDepth.Count; i++)
            {
                DepthChart dc = filterDepth[i];
                // Need to take out current player
                if (dc.PlayerId != 0 && dc.PlayerId != playerId)
                {
                    // Potential replacement player
                    Player p = teamPlayers.FirstOrDefault(x => x.Id == dc.PlayerId);

                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, p.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(p.Id, team, position);
                        if (result == 2)
                        {
                            // Player is in foul trouble but could continue
                            return p;
                        }
                    }
                }
            }

            // Now if we do not have a player - we need 
            Player returned = CheckPlayersAtPosition(playerId, team, position);
            if (returned != null)
            {
                return returned;
            }

            // If we have reached here, we still have no player - so now go through secondard positions
            Player secondary = CheckPlayersAtPositionSecondary(playerId, team, position);
            if (secondary != null)
            {
                return secondary;
            }

            // Now if we reach here, then still no player found, now we open it up to anything
            Player anyPlayer = CheckAllPlayersForSub(playerId, team, position);
            return anyPlayer;
        }

        public Player CheckPlayersAtPosition(int playerId, int team, int position)
        {
            List<Player> teamPlayers = new List<Player>();
            List<Player> filteredPlayers = new List<Player>();
            if (team == 0)
            {
                teamPlayers = _homePlayers;
            }
            else
            {
                teamPlayers = _awayPlayers;
            }

            switch (position)
            {
                case 1:
                    filteredPlayers = teamPlayers.FindAll(x => x.PGPosition == 1);
                    break;
                case 2:
                    filteredPlayers = teamPlayers.FindAll(x => x.SGPosition == 1);
                    break;
                case 3:
                    filteredPlayers = teamPlayers.FindAll(x => x.SFPosition == 1);
                    break;
                case 4:
                    filteredPlayers = teamPlayers.FindAll(x => x.PFPosition == 1);
                    break;
                case 5:
                    filteredPlayers = teamPlayers.FindAll(x => x.CPosition == 1);
                    break;
                default:
                    break;
            }

            // Now we need to go through the checks for these players
            for (int i = 0; i < filteredPlayers.Count; i++)
            {
                Player pc = filteredPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 0)
                        {
                            // Player is totally fine to come on and can be subbed on
                            return pc;
                        }
                    }
                }
            }

            // Now potentially we could not find a player to sub on initially, so next round of checks
            for (int i = 0; i < filteredPlayers.Count; i++)
            {
                Player pc = filteredPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 1)
                        {
                            // Player is fatigued but could continue
                            return pc;
                        }
                    }
                }
            }

            // Now if we still have not got a player to sub on, we can ignore foul trouble - we need to check through the depth charts
            for (int i = 0; i < filteredPlayers.Count; i++)
            {
                Player pc = filteredPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 2)
                        {
                            // Player is in foul trouble but could continue
                            return pc;
                        }
                    }
                }
            }
            return null;
        }

        public Player CheckPlayersAtPositionSecondary(int playerId, int team, int position)
        {
            List<Player> teamPlayers = new List<Player>();
            List<Player> filteredPlayers = new List<Player>();
            if (team == 0)
            {
                teamPlayers = _homePlayers;
            }
            else
            {
                teamPlayers = _awayPlayers;
            }

            switch (position)
            {
                case 1:
                    filteredPlayers = teamPlayers.FindAll(x => x.SGPosition == 1);
                    break;
                case 2:
                    filteredPlayers = teamPlayers.FindAll(x => x.PGPosition == 1 || x.SFPosition == 1);
                    break;
                case 3:
                    filteredPlayers = teamPlayers.FindAll(x => x.SGPosition == 1 || x.PFPosition == 1);
                    break;
                case 4:
                    filteredPlayers = teamPlayers.FindAll(x => x.SFPosition == 1 || x.CPosition == 1);
                    break;
                case 5:
                    filteredPlayers = teamPlayers.FindAll(x => x.PFPosition == 1);
                    break;
                default:
                    break;
            }

            // Now we need to go through the checks for these players
            for (int i = 0; i < filteredPlayers.Count; i++)
            {
                Player pc = filteredPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 0)
                        {
                            // Player is totally fine to come on and can be subbed on
                            return pc;
                        }
                    }
                }
            }

            // Now potentially we could not find a player to sub on initially, so next round of checks
            for (int i = 0; i < filteredPlayers.Count; i++)
            {
                Player pc = filteredPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 1)
                        {
                            // Player is fatigued but could continue
                            return pc;
                        }
                    }
                }
            }

            // Now if we still have not got a player to sub on, we can ignore foul trouble - we need to check through the depth charts
            for (int i = 0; i < filteredPlayers.Count; i++)
            {
                Player pc = filteredPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 2)
                        {
                            // Player is in foul trouble but could continue
                            return pc;
                        }
                    }
                }
            }
            return null;
        }

        public Player CheckAllPlayersForSub(int playerId, int team, int position)
        {
            List<Player> teamPlayers = new List<Player>();
            List<Player> filteredPlayers = new List<Player>();
            if (team == 0)
            {
                teamPlayers = _homePlayers;
            }
            else
            {
                teamPlayers = _awayPlayers;
            }

            // Now we need to go through the checks for these players
            for (int i = 0; i < teamPlayers.Count; i++)
            {
                Player pc = teamPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 0)
                        {
                            // Player is totally fine to come on and can be subbed on
                            return pc;
                        }
                    }
                }
            }

            // Now potentially we could not find a player to sub on initially, so next round of checks
            for (int i = 0; i < filteredPlayers.Count; i++)
            {
                Player pc = filteredPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 1)
                        {
                            // Player is fatigued but could continue
                            return pc;
                        }
                    }
                }
            }

            // Now if we still have not got a player to sub on, we can ignore foul trouble - we need to check through the depth charts
            for (int i = 0; i < filteredPlayers.Count; i++)
            {
                Player pc = filteredPlayers[i];
                // Need to take out current player
                if (pc.Id != playerId)
                {
                    // Now to check if the player is already on the court in a different position
                    int onCourt = CheckIfPlayerIsOnCourt(team, pc.Id);
                    if (onCourt == 0)
                    {
                        int result = CheckIfPlayerNeedsSub(pc.Id, team, position);
                        if (result == 2)
                        {
                            // Player is in foul trouble but could continue
                            return pc;
                        }
                    }
                }
            }
            return null;
        }

        public void EndGameSubCheck(int ftPlayerId)
        {
            // Home Team Check First
            for (int i = 1; i < 6; i++)
            {
                int result = 0;
                int toSub = 0;

                // Check to see if the player is at the free throw line
                result = CheckIfShooter(0, i, ftPlayerId);
                Player p = new Player();

                DepthChart starter = new DepthChart();
                DepthChart backup = new DepthChart();
                switch (i)
                {
                    case 1:
                        starter = _homeDepth.Find(x => x.Position == 1 && x.Depth == 1);
                        backup = _homeDepth.Find(x => x.Position == 1 && x.Depth == 2);
                        p = homePG;
                        break;
                    case 2:
                        starter = _homeDepth.Find(x => x.Position == 2 && x.Depth == 1);
                        backup = _homeDepth.Find(x => x.Position == 2 && x.Depth == 2);
                        p = homeSG;
                        break;
                    case 3:
                        starter = _homeDepth.Find(x => x.Position == 3 && x.Depth == 1);
                        backup = _homeDepth.Find(x => x.Position == 3 && x.Depth == 2);
                        p = homeSF;
                        break;
                    case 4:
                        starter = _homeDepth.Find(x => x.Position == 4 && x.Depth == 1);
                        backup = _homeDepth.Find(x => x.Position == 4 && x.Depth == 2);
                        p = homePF;
                        break;
                    case 5:
                        starter = _homeDepth.Find(x => x.Position == 5 && x.Depth == 1);
                        backup = _homeDepth.Find(x => x.Position == 5 && x.Depth == 2);
                        p = homeC;
                        break;
                    default:
                        break;
                }

                // Check to see if player on is starter
                if (p.Id == starter.PlayerId)
                {
                    toSub = CheckIfPlayerNeedsSub(p.Id, 0, i);

                    if (toSub >= 3)
                    {
                        // The player must be subbed
                        Player newPlayer = FindPlayerToBeSubbedMandatory(p.Id, 0, i);

                        // Now we go through the SubPlayer actions
                        SubPlayer(0, i, newPlayer);
                        // Add the commentary here 
                        string outPlayer = p.FirstName + " " + p.Surname;
                        string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                    }
                    // else we continue on with this player on
                }
                else
                {
                    // The player is not the starter - so check if the starter can come on
                    toSub = CheckIfPlayerNeedsSub(starter.PlayerId, 0, i);

                    if (toSub < 3)
                    {
                        // The starter can come back on
                        Player newPlayer = _homePlayers.FirstOrDefault(x => x.Id == starter.PlayerId);

                        // Now we go through the SubPlayer actions
                        SubPlayer(0, i, newPlayer);
                        // Add the commentary here
                        string outPlayer = p.FirstName + " " + p.Surname;
                        string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                    }
                    else
                    {
                        // The starter could not come on, so now check the 2nd in Depth Chart
                        if (p.Id == backup.PlayerId)
                        {
                            // The backup is on the court
                            toSub = CheckIfPlayerNeedsSub(backup.PlayerId, 0, i);

                            if (toSub >= 3)
                            {
                                // The player must be subbed
                                Player newPlayer = FindPlayerToBeSubbedMandatory(p.Id, 0, i);

                                // Now we go through the SubPlayer actions
                                SubPlayer(0, i, newPlayer);
                                // Add the commentary here
                                string outPlayer = p.FirstName + " " + p.Surname;
                                string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                                // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                                PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                            }
                            // else we continue on with this player on
                        }
                        else
                        {
                            // The backup is not on the court either, so now we need to see if the current player needs to be subbed off
                            toSub = CheckIfPlayerNeedsSub(p.Id, 0, i);

                            if (toSub >= 3)
                            {
                                // The player must be subbed
                                Player newPlayer = FindPlayerToBeSubbedMandatory(p.Id, 0, i);

                                // Now we go through the SubPlayer actions
                                SubPlayer(0, i, newPlayer);
                                // Add the commentary here
                                string outPlayer = p.FirstName + " " + p.Surname;
                                string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                                // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                                PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                            }
                            // else we continue on with this player on
                        }
                    }
                }
            }

            // Away Team Check First
            for (int i = 1; i < 6; i++)
            {
                int result = 0;
                int toSub = 0;

                // Check to see if the player is at the free throw line
                result = CheckIfShooter(1, i, ftPlayerId);
                Player p = new Player();

                DepthChart starter = new DepthChart();
                DepthChart backup = new DepthChart();
                switch (i)
                {
                    case 1:
                        starter = _awayDepth.Find(x => x.Position == 1 && x.Depth == 1);
                        backup = _awayDepth.Find(x => x.Position == 1 && x.Depth == 2);
                        p = awayPG;
                        break;
                    case 2:
                        starter = _awayDepth.Find(x => x.Position == 2 && x.Depth == 1);
                        backup = _awayDepth.Find(x => x.Position == 2 && x.Depth == 2);
                        p = awaySG;
                        break;
                    case 3:
                        starter = _awayDepth.Find(x => x.Position == 3 && x.Depth == 1);
                        backup = _awayDepth.Find(x => x.Position == 3 && x.Depth == 2);
                        p = awaySF;
                        break;
                    case 4:
                        starter = _awayDepth.Find(x => x.Position == 4 && x.Depth == 1);
                        backup = _awayDepth.Find(x => x.Position == 4 && x.Depth == 2);
                        p = awayPF;
                        break;
                    case 5:
                        starter = _awayDepth.Find(x => x.Position == 5 && x.Depth == 1);
                        backup = _awayDepth.Find(x => x.Position == 5 && x.Depth == 2);
                        p = awayC;
                        break;
                    default:
                        break;
                }

                // Check to see if player on is starter
                if (p.Id == starter.PlayerId)
                {
                    toSub = CheckIfPlayerNeedsSub(p.Id, 1, i);

                    if (toSub >= 3)
                    {
                        // The player must be subbed
                        Player newPlayer = FindPlayerToBeSubbedMandatory(p.Id, 1, i);

                        // Now we go through the SubPlayer actions
                        SubPlayer(1, i, newPlayer);
                        // Add the commentary here
                        string outPlayer = p.FirstName + " " + p.Surname;
                        string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    }
                    // else we continue on with this player on
                }
                else
                {
                    // The player is not the starter - so check if the starter can come on
                    toSub = CheckIfPlayerNeedsSub(starter.PlayerId, 1, i);

                    if (toSub < 3)
                    {
                        // The starter can come back on
                        Player newPlayer = _awayPlayers.FirstOrDefault(x => x.Id == starter.PlayerId);
                        int onCourt = CheckIfPlayerIsOnCourt(1, newPlayer.Id);

                        if (onCourt == 0)
                        {
                        // Now we go through the SubPlayer actions
                        SubPlayer(1, i, newPlayer);
                        // Add the commentary here
                        string outPlayer = p.FirstName + " " + p.Surname;
                        string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                        }
                    }
                    else
                    {
                        // The starter could not come on, so now check the 2nd in Depth Chart
                        if (p.Id == backup.PlayerId)
                        {
                            // The backup is on the court
                            toSub = CheckIfPlayerNeedsSub(backup.PlayerId, 1, i);

                            if (toSub >= 3)
                            {
                                // The player must be subbed
                                Player newPlayer = FindPlayerToBeSubbedMandatory(p.Id, 1, i);

                                // Now we go through the SubPlayer actions
                                SubPlayer(1, i, newPlayer);
                                // Add the commentary here
                                string outPlayer = p.FirstName + " " + p.Surname;
                                string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                                // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                                PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                            }
                            // else we continue on with this player on
                        }
                        else
                        {
                            // The backup is not on the court either, so now we need to see if the current player needs to be subbed off
                            toSub = CheckIfPlayerNeedsSub(p.Id, 1, i);

                            if (toSub >= 3)
                            {
                                // The player must be subbed
                                Player newPlayer = FindPlayerToBeSubbedMandatory(p.Id, 1, i);

                                // Now we go through the SubPlayer actions
                                SubPlayer(1, i, newPlayer);
                                // Add the commentary here
                                string outPlayer = p.FirstName + " " + p.Surname;
                                string inPlayer = newPlayer.FirstName + " " + newPlayer.Surname;
                                // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                                PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                            }
                            // else we continue on with this player on
                        }
                    }
                }
            }
        }

        #endregion
    }
}
