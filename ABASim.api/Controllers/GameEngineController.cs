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
            SetStartingLineups();

            commentaryData.Add(comm.GetGameIntroCommentry(_awayTeam, _homeTeam)); // Need a way to block this out when run games for real
            commentaryData.Add(comm.GetStartingLineupsCommentary(awayPG, awaySG, awaySF, awayPF, awayC));
            commentaryData.Add(comm.GetStartingLineupsCommentary(homePG, homeSG, homeSF, homePF, homeC));
            commentaryData.Add("It's now time for the opening tip");
            
            PlayByPlayTracker(comm.GetGameIntroCommentry(_awayTeam, _homeTeam) , 0);
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
            BackToStarters();
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
            if (_awayScore > _homeScore) {
                winningTeamId = _awayTeam.Id;
            } else {
                winningTeamId = _homeTeam.Id;
            }
            bool savedGame = await _repo.SavePreseasonResult(_awayScore, _homeScore, winningTeamId, game.GameId);

            return Ok(true);
        }

        [HttpPost("startPlayoffGame")]
        public async Task<IActionResult> StartPlayoffGame(SimGameDto game)
        {
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
            if (_awayScore > _homeScore) {
                winningTeamId = _awayTeam.Id;
                losingTeamId = _homeTeam.Id;
            } else {
                winningTeamId = _homeTeam.Id;
                losingTeamId = _awayTeam.Id;
            }
            bool savedGame = await _repo.SavePlayoffResult(_awayScore, _homeScore, winningTeamId, game.GameId, losingTeamId);
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
            if (_awayScore > _homeScore) {
                winningTeamId = _awayTeam.Id;
                losingTeamId = _homeTeam.Id;
            } else {
                winningTeamId = _homeTeam.Id;
                losingTeamId = _awayTeam.Id;
            }
            bool savedGame = await _repo.SaveSeasonResult(_awayScore, _homeScore, winningTeamId, game.GameId, losingTeamId);

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
            _awayRoster = (List<Roster>) ar;
            _homeRoster = (List<Roster>) hr;

            return Ok(true);
        }

        public async Task<IActionResult> SetupDepthCharts()
        {
            var adc = await _repo.GetDepthChart(_awayTeam.Id);
            var hdc = await _repo.GetDepthChart(_homeTeam.Id);
            _awayDepth = (List<DepthChart>) adc;
            _homeDepth = (List<DepthChart>) hdc;
            
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
            var hd = _homeDepth.FindAll(x => x.Depth == 1);

            for (int i = 0; i < ad.Count; i++)
            {
                int index;
                switch (ad[i].Position)
                {
                    case 1:
                        awayPG = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                        awayPGRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                        awayPGTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId); 

                        StaminaTrack awatSTPG = _awayStaminas.Find(x => x.PlayerId == ad[i].PlayerId);
                        awatSTPG.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awatSTPG.PlayerId);
                        _awayStaminas[index] = awatSTPG;
                        break;
                    case 2:
                        awaySG = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                        awaySGRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                        awaySGTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);

                        StaminaTrack awaySTSG = _awayStaminas.Find(x => x.PlayerId == ad[i].PlayerId);
                        awaySTSG.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awaySTSG.PlayerId);
                        _awayStaminas[index] = awaySTSG;
                        break;
                    case 3:
                        awaySF = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                        awaySFRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                        awaySFTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);

                        StaminaTrack awaySTSF = _awayStaminas.Find(x => x.PlayerId == ad[i].PlayerId);
                        awaySTSF.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awaySTSF.PlayerId);
                        _awayStaminas[index] = awaySTSF;
                        break;
                    case 4:
                        awayPF = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                        awayPFRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                        awayPFTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);

                        StaminaTrack awaySTPF = _awayStaminas.Find(x => x.PlayerId == ad[i].PlayerId);
                        awaySTPF.OnOff = 1;
                        index = _awayStaminas.FindIndex(x => x.PlayerId == awaySTPF.PlayerId);
                        _awayStaminas[index] = awaySTPF;
                        break;
                    case 5:
                        awayC = _awayPlayers.Find(x => x.Id == ad[i].PlayerId);
                        awayCRatings = _awayRatings.Find(x => x.PlayerId == ad[i].PlayerId);
                        awayCTendancy = _awayTendancies.Find(x => x.PlayerId == ad[i].PlayerId);

                        StaminaTrack awaySTC = _awayStaminas.Find(x => x.PlayerId == ad[i].PlayerId);
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
                switch (hd[i].Position)
                {
                    case 1:
                        homePG = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                        homePGRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                        homePGTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);

                        StaminaTrack homeSTPG = _homeStaminas.Find(x => x.PlayerId == hd[i].PlayerId);
                        homeSTPG.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTPG.PlayerId);
                        _homeStaminas[index] = homeSTPG;
                        break;
                    case 2:
                        homeSG = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                        homeSGRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                        homeSGTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);

                        StaminaTrack homeSTSG = _homeStaminas.Find(x => x.PlayerId == hd[i].PlayerId);
                        homeSTSG.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTSG.PlayerId);
                        _homeStaminas[index] = homeSTSG;
                        break;
                    case 3:
                        homeSF = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                        homeSFRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                        homeSFTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);

                        StaminaTrack homeSTSF = _homeStaminas.Find(x => x.PlayerId == hd[i].PlayerId);
                        homeSTSF.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTSF.PlayerId);
                        _homeStaminas[index] = homeSTSF;
                        break;
                    case 4:
                        homePF = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                        homePFRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                        homePFTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);

                        StaminaTrack homeSTPF = _homeStaminas.Find(x => x.PlayerId == hd[i].PlayerId);
                        homeSTPF.OnOff = 1;
                        index = _homeStaminas.FindIndex(x => x.PlayerId == homeSTPF.PlayerId);
                        _homeStaminas[index] = homeSTPF;
                        break;
                    case 5:
                        homeC = _homePlayers.Find(x => x.Id == hd[i].PlayerId);
                        homeCRatings = _homeRatings.Find(x => x.PlayerId == hd[i].PlayerId);
                        homeCTendancy = _homeTendancies.Find(x => x.PlayerId == hd[i].PlayerId);

                        StaminaTrack homeSTC = _homeStaminas.Find(x => x.PlayerId == hd[i].PlayerId);
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
            } else
            {
                // Away Team wins the jumpball
                _teamPossession = 1;
                winningTeam = _awayTeam;
            }

            int j = _random.Next(1, 101);
            
            _playerPossession = new Jumpball().GetJumpPlayer(j);
            string currentPlayerName = GetCurrentPlayerFullName();

            // Need to do the commentary
            commentaryData.Add(comm.GetJumpballCommentary(winningTeam, _quarter, _time, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            PlayByPlayTracker(comm.GetJumpballCommentary(winningTeam, _quarter, _time, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

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

            while (_time > 0)
            {
                if(_shotClock > 0)
                {
                    // int decision = -1;

                    // if (_quarter > 3 && _time <= 48) {
                    //     decision = EndGameDecisions();
                    // } else {
                    //     decision = GetPlayerDecision();
                    // }
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
                            PlayByPlayTracker(comm.GetHoldBallCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                            break;
                        default:
                            break;
                    }
                } else {
                    ShotClockViolation();
                }
            }
            EndOfQuarter();
        }

         public void RunOvertime()
        {
            _homeFoulBonus = 0;
            _awayFoulBonus = 0;
            _time = 300;
            _quarter++;

            while (_time > 0)
            {
                if(_shotClock > 0)
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
                } else {
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

            // Get the shot clock shoot bonus - TODO here around the end game bonuses
            if (_quarter > 3 && _time <= 48) {
                _endGameFoulAddition = 0; // Done
                _endGameResultIncrease = 0; // Done
                _endGameShotClockBonus = 0; // Done
                _endGameStealAddition = 0; // Done
                _endGameThreePointAddition = 0;
                _endGameTwoPointAddition = 0;
                
                EndGameShootBonus();
            } else {
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

                int foulBonusValue = 0;
                if (_quarter > 3 && _time <= 48 && _endGameFoulAddition != 0) {
                    foulBonusValue = _endGameFoulAddition;
                } else {
                    foulBonusValue = (int)(tendancy.FouledTendancy * 0.1);
                }

                int threeTendancy = tendancy.ThreePointTendancy;
                int twoTendancy = tendancy.TwoPointTendancy;
                int passTendancy = tendancy.PassTendancy;
                if (_quarter > 3 && _time <= 48 && _endGameThreePointAddition != 0) {
                    threeTendancy = tendancy.ThreePointTendancy + _endGameThreePointAddition;
                    
                    if (tendancy.TwoPointTendancy < _endGameThreePointAddition) {
                        twoTendancy = 0;
                        int remainder = _endGameThreePointAddition - tendancy.ThreePointTendancy;
                        passTendancy = tendancy.PassTendancy - remainder;
                    } else {
                        twoTendancy = tendancy.TwoPointTendancy - _endGameThreePointAddition;
                    }
                }

                if (_quarter > 3 && _time <= 48 && _endGameTwoPointAddition != 0) {
                    twoTendancy = tendancy.TwoPointTendancy + _endGameTwoPointAddition;
                    
                    if (tendancy.PassTendancy < _endGameTwoPointAddition) {
                        passTendancy = 0;
                        int remainder = _endGameTwoPointAddition - tendancy.PassTendancy;
                        twoTendancy = tendancy.TwoPointTendancy - remainder;
                    } else {
                        passTendancy = tendancy.PassTendancy - _endGameTwoPointAddition;
                    }
                }

                int twoSection = twoTendancy;
                int threeSection = (twoTendancy + threeTendancy);
                int foulSection = (twoTendancy + threeTendancy + tendancy.FouledTendancy + foulBonusValue);
                int turnoverSection = (twoTendancy + threeTendancy + tendancy.FouledTendancy + foulBonusValue + tendancy.TurnoverTendancy);

                // int twoSection = tendancy.TwoPointTendancy;
                // int threeSection = (tendancy.TwoPointTendancy + tendancy.ThreePointTendancy);
                // int foulSection = (tendancy.TwoPointTendancy + tendancy.ThreePointTendancy + tendancy.FouledTendancy + foulBonusValue);
                // int turnoverSection = (tendancy.TwoPointTendancy + tendancy.ThreePointTendancy + tendancy.FouledTendancy + foulBonusValue + tendancy.TurnoverTendancy);

                if (shotClockBonus > 0 || _endGameShotClockBonus > 0)
                {
                    if (_quarter > 3 && _time <= 48) {
                        shotClockBonus = _endGameShotClockBonus;
                    }
                    // Then we have some bonus to work out
                    int total = twoTendancy + threeTendancy + tendancy.FouledTendancy + foulBonusValue + tendancy.TurnoverTendancy;

                    double twoUpgrade = (double) tendancy.TwoPointTendancy / total;
                    double threeUpgrade = (double) tendancy.ThreePointTendancy / total;
                    double foulUpgrade = (double) (tendancy.FouledTendancy + foulBonusValue) / total;
                    double turnoverUpgrade = (double) tendancy.TurnoverTendancy / total;

                    int twoPointBonus = (int) (shotClockBonus * twoUpgrade);
                    int threePointBonus = (int) (shotClockBonus * threeUpgrade);
                    int foulBonus = (int) (shotClockBonus * foulUpgrade);
                    int turnoverBonus = (int) (shotClockBonus * turnoverUpgrade);

                    twoSection = twoTendancy + twoPointBonus;
                    threeSection = threeTendancy + threePointBonus + twoSection;
                    foulSection = tendancy.FouledTendancy + foulBonusValue + foulBonus + threeSection;
                    turnoverSection = tendancy.TurnoverTendancy + turnoverBonus + foulSection;
                } else {
                    if (shotClockBonus != 0 || _endGameShotClockBonus != 0) {
                        if (_quarter > 3 && _time <= 48) {
                            shotClockBonus = _endGameShotClockBonus;
                        }

                        int total = twoTendancy + threeTendancy;

                        double twoUpgrade = (double) tendancy.TwoPointTendancy / total;
                        double threeUpgrade = (double) tendancy.ThreePointTendancy / total;

                        int twoPointBonus = (int) (shotClockBonus * twoUpgrade);
                        int threePointBonus = (int) (shotClockBonus * threeUpgrade);

                        twoSection = twoTendancy + twoPointBonus;
                        threeSection = threeTendancy + threePointBonus + twoSection;
                        foulSection = tendancy.FouledTendancy + foulBonusValue + threeSection;
                        turnoverSection = tendancy.TurnoverTendancy + foulSection;
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
                    } else {
                        decision = 7; // Player has had to hold onto the ball
                    }
                }
                else if (result > foulSection && result <= turnoverSection)
                {
                    // The player has potentially fouled
                    int check = _random.Next(1, 101);
                    if (check < 45)
                    {
                        // The player has turned the ball over
                        decision = 5;
                    } else {
                        decision = 7; // Player has had to hold onto the ball
                    }
                } 
                else 
                {
                    // The player has passed the ball
                    decision = 1; 
                }
            } else {
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

            // Check 
            if (_teamPossession == 0)
            {
                switch (_playerPossession)
                {
                    case 1:
                        totalUsage = homeSGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating + homeCRatings.UsageRating;
                        break;
                    case 2:
                        totalUsage = homePGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating + homeCRatings.UsageRating;
                        break;
                    case 3:
                        totalUsage = homeSGRatings.UsageRating + homePGRatings.UsageRating + homePFRatings.UsageRating + homeCRatings.UsageRating;
                        break;
                    case 4:
                        totalUsage = homeSGRatings.UsageRating + homeSFRatings.UsageRating + homePGRatings.UsageRating + homeCRatings.UsageRating;
                        break;
                    case 5:
                        totalUsage = homeSGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating + homePGRatings.UsageRating;
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
                switch (_playerPossession)
                {
                    case 1:
                        totalUsage = awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating + awayCRatings.UsageRating;
                        break;
                    case 2:
                        totalUsage = awayPGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating + awayCRatings.UsageRating;
                        break;
                    case 3:
                        totalUsage = awaySGRatings.UsageRating + awayPGRatings.UsageRating + awayPFRatings.UsageRating + awayCRatings.UsageRating;
                        break;
                    case 4:
                        totalUsage = awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayPGRatings.UsageRating + awayCRatings.UsageRating;
                        break;
                    case 5:
                        totalUsage = awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating + awayPGRatings.UsageRating;
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
                        if (result < homeSGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            // Home SG receives the ball
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homeSGRatings.UsageRating && result < (homeSGRatings.UsageRating + homeSFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homeSGRatings.UsageRating + homeSFRatings.UsageRating) && result < (homeSGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homeSGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating) && result < (homeSGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating + homeCRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 2:
                        if (result < homePGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            // Home PG receives the ball
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homePGRatings.UsageRating && result < (homePGRatings.UsageRating + homeSFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGRatings.UsageRating + homeSFRatings.UsageRating) && result < (homePGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating) && result < (homePGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating + homeCRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 3:
                        if (result < homePGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            // Home PG receives the ball
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homePGRatings.UsageRating && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGRatings.UsageRating + homeSGRatings.UsageRating) && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating + homePFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGRatings.UsageRating + homeSGRatings.UsageRating + homePFRatings.UsageRating) && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating + homePFRatings.UsageRating + homeCRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 4:
                        if (result < homePGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            // Home SG receives the ball
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homePGRatings.UsageRating && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGRatings.UsageRating + homeSGRatings.UsageRating) && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating + homeSFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGRatings.UsageRating + homeSGRatings.UsageRating + homeSFRatings.UsageRating) && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating + homeSFRatings.UsageRating + homeCRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 5:
                        if (result < homePGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= homePGRatings.UsageRating && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGRatings.UsageRating + homeSGRatings.UsageRating) && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating + homeSFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (homePGRatings.UsageRating + homeSGRatings.UsageRating + homeSFRatings.UsageRating) && result < (homePGRatings.UsageRating + homeSGRatings.UsageRating + homeSFRatings.UsageRating + homePFRatings.UsageRating))
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
                        if (result < awaySGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awaySGRatings.UsageRating && result < (awaySGRatings.UsageRating + awaySFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awaySGRatings.UsageRating + awaySFRatings.UsageRating) && result < (awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating) && result < (awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating + awayCRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 2:
                        if (result < awayPGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awayPGRatings.UsageRating && result < (awayPGRatings.UsageRating + awaySFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGRatings.UsageRating + awaySFRatings.UsageRating) && result < (awayPGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating) && result < (awayPGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating + awayCRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 3:
                        if (result < awayPGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awayPGRatings.UsageRating && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGRatings.UsageRating + awaySGRatings.UsageRating) && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awayPFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 4;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awayPFRatings.UsageRating) && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awayPFRatings.UsageRating + awayCRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 4:
                        if (result < awayPGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awayPGRatings.UsageRating && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGRatings.UsageRating + awaySGRatings.UsageRating) && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awaySFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awaySFRatings.UsageRating) && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayCRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 5;
                            receiver = GetCurrentPlayerFullName();
                        }
                        break;
                    case 5:
                        if (result < awayPGRatings.UsageRating)
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 1;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= awayPGRatings.UsageRating && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 2;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGRatings.UsageRating + awaySGRatings.UsageRating) && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awaySFRatings.UsageRating))
                        {
                            passer = GetCurrentPlayerFullName();
                            _playerPossession = 3;
                            receiver = GetCurrentPlayerFullName();
                        }
                        else if (result >= (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awaySFRatings.UsageRating) && result < (awayPGRatings.UsageRating + awaySGRatings.UsageRating + awaySFRatings.UsageRating + awayPFRatings.UsageRating))
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
                commentaryData.Add(comm.GetPassCommentary(passer, receiver, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                PlayByPlayTracker(comm.GetPassCommentary(passer, receiver, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                // Console.WriteLine(comm.GetPassCommentary(passer, receiver, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            }
        }

        public void TwoPointShot()
        {
            twosTaken++;
            int possibleAssist = 0;
            string assistingPlayingName = "";
            if(_playerPassed != null)
            {
                assistingPlayingName = _playerPassed.FirstName + " " + _playerPassed.Surname;
            }

            // Need to see if the shot was blocked
            int blockResult = BlockCheck();

            if (blockResult == 0)
            {
                PlayerRating currentRating = GetCurrentPlayersRatings();
                int result = _random.Next(1, 1001);

                int twoRating = StaminaEffect(currentRating.PlayerId, _teamPossession, currentRating.TwoRating);

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

                result = result - orpmValue + drpmValue;
                result = result + _endGameResultIncrease;

                // Need to determine whether an assist chance has been created
                if (_playerRatingPassed != null)
                {
                    int assistRating = (_playerRatingPassed.AssitRating * 5);
                    int assistResult = _random.Next(0, 1000);

                    if (assistResult <= assistRating)
                    {
                        assistCounterChance++;
                        possibleAssist = 1;
                        // update the result
                        result = result - 80;
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

                        if (possibleAssist == 1)
                        {
                            // Update the Box Score
                            BoxScore tempAst = _homeBoxScores.Find(x => x.Id == _playerPassed.Id);
                            tempAst.Assists++;
                            int indexAst = _homeBoxScores.FindIndex(x => x.Id == _playerPassed.Id);
                            _homeBoxScores[indexAst] = tempAst;
                        }
                    } else {
                        _awayScore = _awayScore + 2;
                        UpdatePlusMinusBoxScore(2);

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == currentRating.PlayerId);
                        temp.FGA++;
                        temp.FGM++;
                        temp.Points = temp.Points + 2;
                        int index = _awayBoxScores.FindIndex(x => x.Id == currentRating.PlayerId);
                        _awayBoxScores[index] = temp;

                        if (possibleAssist == 1)
                        {
                            // Update the Box Score
                            BoxScore tempAst = _awayBoxScores.Find(x => x.Id == _playerPassed.Id);
                            tempAst.Assists++;
                            int indexAst = _awayBoxScores.FindIndex(x => x.Id == _playerPassed.Id);
                            _awayBoxScores[indexAst] = tempAst;
                        }
                    }

                    // Comm
                    commentaryData.Add(comm.GetTwoPointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName));
                    // Console.WriteLine(comm.GetTwoPointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName));
                    PlayByPlayTracker(comm.GetTwoPointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName), 0);

                    if (_teamPossession == 0)
                    {
                        _teamPossession = 1;
                    } else {
                        _teamPossession = 0;
                    }

                    Inbounds();
                } else {
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
            } else {
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
            if(_playerPassed != null)
            {
                assistingPlayingName = _playerPassed.FirstName + " " + _playerPassed.Surname;
            }

            // Need to see if the shot was blocked
            int blockResult = BlockCheck();

            if (blockResult == 0)
            {
                PlayerRating currentRating = GetCurrentPlayersRatings();
                int result = _random.Next(1, 1001);

                int threeRating = StaminaEffect(currentRating.PlayerId, _teamPossession, currentRating.ThreeRating);

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

                result = result - orpmValue + drpmValue;
                result = result + _endGameResultIncrease;

                // Need to determine whether an assist chance has been created
                if (_playerRatingPassed != null)
                {
                    
                    int assistRating = (_playerRatingPassed.AssitRating * 5); // Factor applied to increase the low Assist to Pass rate for low pass counts in sim
                    int assistResult = _random.Next(0, 1000);

                    if (assistResult <= assistRating)
                    {
                        assistCounterChance++;
                        possibleAssist = 1;
                        // update the result
                        result = result - 80;
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

                        if (possibleAssist == 1)
                        {
                            assistCounter++;

                            // Update the Box Score
                            BoxScore temp2 = _homeBoxScores.Find(x => x.Id == _playerPassed.Id);
                            temp2.Assists++;
                            int index2 = _homeBoxScores.FindIndex(x => x.Id == _playerPassed.Id);
                            _homeBoxScores[index2] = temp2;
                        }
                        
                    } else {
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

                        if (possibleAssist == 1)
                        {
                            assistCounter++;

                            // Update the Box Score
                            BoxScore temp2 = _awayBoxScores.Find(x => x.Id == _playerPassed.Id);
                            temp2.Assists++;
                            int index2 = _awayBoxScores.FindIndex(x => x.Id == _playerPassed.Id);
                            _awayBoxScores[index2] = temp2;
                        }
                    }

                    // Comm
                    commentaryData.Add(comm.GetThreePointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName));
                    // Console.WriteLine(comm.GetThreePointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName));
                    PlayByPlayTracker(comm.GetThreePointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot, possibleAssist, assistingPlayingName), 0);

                    if (_teamPossession == 0)
                    {
                        _teamPossession = 1;
                    } else {
                        _teamPossession = 0;
                    }

                    Inbounds();
                } else {
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
            } else {
                // Shot has been blocked
                blockCounter++;
                // TODO - need to handle the fact the shot clock would not reset on offensive rebound here
                Rebound();
            }
        }

        public int BlockCheck()
        {
            PlayerRating currentRating = GetCurrentPlayersRatings();

            if (_teamPossession == 0)
            {
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
                    int rating = StaminaEffect(checking.PlayerId, 1, checking.BlockRating);
                    int result = _random.Next(1, 2001);

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

                        // Commentary
                        commentaryData.Add(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

                        return 1;
                    }
                }
            } else {
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
                    int rating = StaminaEffect(checking.PlayerId, 0, checking.BlockRating);
                    int result = _random.Next(1, 2501);

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

                        // Commentary
                        commentaryData.Add(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.BlockCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

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
                int homePGRebound = StaminaEffect(homePGRatings.PlayerId, 0, homePGRatings.ORebRating);
                int homeSGRebound = StaminaEffect(homeSGRatings.PlayerId, 0, homeSGRatings.ORebRating);
                int homeSFRebound = StaminaEffect(homeSFRatings.PlayerId, 0, homeSFRatings.ORebRating);
                int homePFRebound = StaminaEffect(homePFRatings.PlayerId, 0, homePFRatings.ORebRating);
                int homeCRebound = StaminaEffect(homeCRatings.PlayerId, 0, homeCRatings.ORebRating);

                int awayPGRebound = StaminaEffect(awayPGRatings.PlayerId, 1, awayPGRatings.DRebRating);
                int awaySGRebound = StaminaEffect(awaySGRatings.PlayerId, 1, awaySGRatings.DRebRating);
                int awaySFRebound = StaminaEffect(awaySFRatings.PlayerId, 1, awaySFRatings.DRebRating);
                int awayPFRebound = StaminaEffect(awayPFRatings.PlayerId, 1, awayPFRatings.DRebRating);
                int awayCRebound = StaminaEffect(awayCRatings.PlayerId, 1, awayCRatings.DRebRating);

                offensiveRate = homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound;
                defensiveRate = awayPGRebound + awaySGRebound + awaySFRebound + awaySFRebound + awayCRebound;

                // home team shot
                int randValue = offensiveRate + defensiveRate;
                int result = _random.Next(1, randValue + 1);

                // Firstly determine if it is offensive or defensive
                if (result < offensiveRate)
                {
                    // Offensive Rebound
                    _shotClock = 14;

                    if (result < homePGRebound)
                    {
                        _playerPossession = 1;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homePG.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homePG.Id);
                        _homeBoxScores[index] = temp;
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
                    }

                    // Commentary for Offensive Rebound
                    commentaryData.Add(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                }
                else
                {
                    // Defensive Rebound
                    _shotClock = 24;

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
                    }

                    // Display Defensive Rebound Commentary
                    commentaryData.Add(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                }
            } else {
                int homePGRebound = StaminaEffect(homePGRatings.PlayerId, 0, homePGRatings.DRebRating);
                int homeSGRebound = StaminaEffect(homeSGRatings.PlayerId, 0, homeSGRatings.DRebRating);
                int homeSFRebound = StaminaEffect(homeSFRatings.PlayerId, 0, homeSFRatings.DRebRating);
                int homePFRebound = StaminaEffect(homePFRatings.PlayerId, 0, homePFRatings.DRebRating);
                int homeCRebound = StaminaEffect(homeCRatings.PlayerId, 0, homeCRatings.DRebRating);

                int awayPGRebound = StaminaEffect(awayPGRatings.PlayerId, 1, awayPGRatings.ORebRating);
                int awaySGRebound = StaminaEffect(awaySGRatings.PlayerId, 1, awaySGRatings.ORebRating);
                int awaySFRebound = StaminaEffect(awaySFRatings.PlayerId, 1, awaySFRatings.ORebRating);
                int awayPFRebound = StaminaEffect(awayPFRatings.PlayerId, 1, awayPFRatings.ORebRating);
                int awayCRebound = StaminaEffect(awayCRatings.PlayerId, 1, awayCRatings.ORebRating);

                defensiveRate = homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound;
                offensiveRate = awayPGRebound + awaySGRebound + awaySFRebound + awaySFRebound + awayCRebound;

                // home team shot
                int randValue = offensiveRate + defensiveRate;
                int result = _random.Next(1, randValue + 1);

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
                    }

                    // Commentary for Offensive Rebound
                    commentaryData.Add(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
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
                    }

                    // Display Defensive Rebound Commentary
                    commentaryData.Add(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                }
            }
        }

        public int StealCheck()
        {
            PlayerRating currentRating = GetCurrentPlayersRatings();
            int stealBonus = 0;

            if (_endGameStealAddition > 0) {
                stealBonus = _endGameStealAddition;
            }

            if (_teamPossession == 0)
            {
                // int stealingTeam = 1;

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
                    int rating = StaminaEffect(checking.PlayerId, 1, checking.StealRating);
                    int result = _random.Next(1, (4501 - stealBonus)); // This is times 5 to account for all 5 players pn the court

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

                        // Commentary
                        commentaryData.Add(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

                        _teamPossession = 1;
                        _playerPossession = _random.Next(1, 6); // not sure how to make this correct

                        _playerPassed = null;
                        _playerRatingPassed = null;

                        return 1;
                    }
                }
            } else {
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
                    int rating = StaminaEffect(checking.PlayerId, 0, checking.StealRating);
                    int result = _random.Next(1, (4501 - stealBonus));

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

                        // Commentary
                        commentaryData.Add(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.StealCommentary(GetPlayerFullNameForPosition(_teamPossession, checking.PlayerId), GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

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
            foulCounter++;
            _playerRatingPassed = null;
            _playerPassed = null;

            // Setup fouling objects
            Player playerFouling = new Player();
            int isFreeThrows = 0;
            int teamWhichFouled = 2;

            // Set the time
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

            // Update what goes game wide
            _time = _time - timeValue;
            _shotClock = _shotClock - timeValue;
            timeCounter = timeCounter + timeValue;

            StaminaUpdates(timeValue);
            UpdateTimeInBoxScore(timeValue);

            if (_shotClock < 14) {
                _shotClock = 14;
            }

            // Need to update the player fouling boxscore
            int fouler = _random.Next(1, 6);
            playerFouling = UpdateFouler(fouler);
            string fouling = playerFouling.FirstName + " " + playerFouling.Surname;

            int result = _random.Next(1, 20);

            if (result < 16)
            {
                // Non-Shooting Foul
                if (_teamPossession == 1)
                {
                    // Commentary
                    commentaryData.Add(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    _homeFoulBonus++;
                    teamWhichFouled = 0;

                    if (_homeFoulBonus > 4)
                    {
                        // Commentary
                        commentaryData.Add(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                        isFreeThrows = 1;
                    }
                }
                else
                {
                    // Commentary
                    commentaryData.Add(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 1, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    _awayFoulBonus++;
                    teamWhichFouled = 1;

                    if (_awayFoulBonus > 4)
                    {
                        // Commentary
                        commentaryData.Add(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.BonusCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                        isFreeThrows = 1;
                    }
                }

                // // Foul Trouble Check for subs
                int sub = FoulTroubleCheck(teamWhichFouled, fouler);

                if (sub == 1)
                {
                    string outPlayer = GetPlayerFullNameForPositionForFouler(teamWhichFouled, GetPlayerIdForPosition(teamWhichFouled, fouler));
                    // Player needs to be subbed out due to foul trouble
                    Substitution(teamWhichFouled, fouler);

                    // Now need to sort out the sub commentary
                    string inPlayer = GetPlayerFullNameForPositionForFouler(teamWhichFouled, GetPlayerIdForPosition(teamWhichFouled, fouler));

                    int teamToDisplay = 0;
                    if (teamWhichFouled == 1) {
                        teamToDisplay = 1;
                    }
                        
                    commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, teamToDisplay, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, teamToDisplay, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, teamToDisplay, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                }
                //  else if (sub == 6) {
                //     // player has fouled out
                //     int pid = GetPlayerIdForPosition(teamWhichFouled, fouler);
                //     fouledOutPlayers.Add(pid);
                    
                //     // call substituion
                //     Substitution(teamWhichFouled, fouler);
                // }
                
                // Check if it is free throws
                if (isFreeThrows == 1)
                {
                    // Need to check if there are any subs to be made
                    SubCheckFT();

                    int ftResult = FreeThrows(2);
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
                    SubCheck();
                    Inbounds();
                }
            }
            else
            {
                // Shooting Foul
                if (_teamPossession == 0)
                {
                    _homeFoulBonus++;
                }
                else
                {
                    _awayFoulBonus++;
                }

                // Need to determine if it is a 3 or 2
                int shots = _random.Next(1, 11);

                if (shots <= 8)
                {
                    // Commentary
                    commentaryData.Add(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 2, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 2, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 2, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 0);

                    // Need to check if there are any subs to be made
                    SubCheckFT();

                    int ftResult = FreeThrows(2);
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
                    // Commentary
                    commentaryData.Add(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 3, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 3, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.FoulCommentary(GetCurrentPlayerFullName(), fouling, 3, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);

                    // Need to check if there are any subs to be made
                    SubCheckFT();

                    int ftResult = FreeThrows(3);
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
                        // Finish the action
                        Inbounds();
                    }
                    else
                    {
                        Rebound();
                    }
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
                int ftRating = StaminaEffect(currentPlayerRating.PlayerId, _teamPossession, currentPlayerRating.FTRating);

                if (result <= ftRating)
                {
                    // Free throw is made
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
                    }
                    UpdatePlusMinusBoxScore(1);

                    // commentary
                    commentaryData.Add(comm.GetMadeFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    // Console.WriteLine(comm.GetMadeFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    PlayByPlayTracker(comm.GetMadeFreeThrowCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);
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

                    // Foul Trouble Check for subs
                    int sub = FoulTroubleCheck(0, fouler);

                    if (sub == 1)
                    {
                        string outPlayer = GetPlayerFullNameForPositionForFouler(0, GetPlayerIdForPosition(0, fouler));
                        // Player needs to be subbed out due to foul trouble
                        Substitution(1, fouler);

                        // Now need to sort out the sub commentary
                        string inPlayer = GetPlayerFullNameForPositionForFouler(0, GetPlayerIdForPosition(0, fouler));
                            
                        commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    } 
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

                // commentary
                commentaryData.Add(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                // Console.WriteLine(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                PlayByPlayTracker(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);

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

                    // Foul Trouble Check for subs
                    int sub = FoulTroubleCheck(1, fouler);

                    if (sub == 1)
                    {
                        string outPlayer = GetPlayerFullNameForPositionForFouler(1, GetPlayerIdForPosition(1, fouler));
                    
                        // Player needs to be subbed out due to foul trouble
                        Substitution(1, fouler);

                        // Now need to sort out the sub commentary
                        string inPlayer = GetPlayerFullNameForPositionForFouler(1, GetPlayerIdForPosition(1, fouler));
                            
                        commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                    } 
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

                // commentary
                commentaryData.Add(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                // Console.WriteLine(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                PlayByPlayTracker(comm.TurnoverCommentary(turnoverType, GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot), 1);

                // change possession
                _teamPossession = 0;
            }
            _shotClock = 24;
            SubCheck();
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
            SubCheck();

            // Inbounds the ball to continue on
            Inbounds();
        }

        public void EndGameShootBonus()
        {
            int diff = 0;
            if (_teamPossession == 0) {
                diff = _awayScore - _homeScore;

                if (diff < 0) {
                    // home team is winning
                    if (_time > 16 || _shotClock > 16) {
                        _endGameShotClockBonus = -200;
                    } else if (_time > 12 || _shotClock > 12) {
                        _endGameShotClockBonus = -100;
                    } else if (_time > 8 || _shotClock > 8) {
                        _endGameShotClockBonus = 100;
                    } else if (_time > 4 || _shotClock > 4) {
                        _endGameShotClockBonus = 300;
                    } else {
                        _endGameShotClockBonus = 500;
                    }
                    
                    if (diff <= 3) {
                        // Defensive teams actions
                        // increased steal chance
                        _endGameStealAddition = 500;
    
                        // much increased in fouls
                        _endGameFoulAddition = 800;
                    }
                } else if (diff > 0) {
                    // home team is losing
                    // losing margins
                    if (diff == 5 || diff == 6) {
                        // Shooting liklihood is increase significantly and 3's are increased most
                        // team will shoot quicker
                        if (_time > 16 || _shotClock > 16) {
                            _endGameShotClockBonus = 50;
                        } else if (_time > 14 || _shotClock > 14) {
                            _endGameShotClockBonus = 100;
                        } else if (_time > 12 || _shotClock > 12) {
                            _endGameShotClockBonus = 150;
                        } else if (_time > 10 || _shotClock > 10) {
                            _endGameShotClockBonus = 200;
                        } else if (_time > 8 || _shotClock > 8) {
                            _endGameShotClockBonus = 300;
                        } else if (_time > 6 || _shotClock > 6) {
                            _endGameShotClockBonus = 400;
                        } else if (_time > 4 || _shotClock > 4) {
                            _endGameShotClockBonus = 500;
                        } else {
                            _endGameShotClockBonus = 600;
                        }

                        // increase in 3s taken
                        _endGameThreePointAddition = (int) (GetCurrentPlayersTendancies().ThreePointTendancy * 0.2);

                        // Result increase by 15% due to tough shots
                        _endGameResultIncrease = 150;

                        // Apply winning to team
                        // no changes for defensive team
                    } else if (diff == 4) {
                        // team will shoot quicker
                        if (_time > 16 || _shotClock > 16) {
                            _endGameShotClockBonus = 50;
                        } else if (_time > 14 || _shotClock > 14) {
                            _endGameShotClockBonus = 100;
                        } else if (_time > 12 || _shotClock > 12) {
                            _endGameShotClockBonus = 150;
                        } else if (_time > 10 || _shotClock > 10) {
                            _endGameShotClockBonus = 200;
                        } else if (_time > 8 || _shotClock > 8) {
                            _endGameShotClockBonus = 300;
                        } else if (_time > 6 || _shotClock > 6) {
                            _endGameShotClockBonus = 400;
                        } else if (_time > 4 || _shotClock > 4) {
                            _endGameShotClockBonus = 500;
                        } else {
                            _endGameShotClockBonus = 600;
                        }
                        
                        // random between 5 and 10% added to shot result
                        _endGameResultIncrease = (_random.Next(50, 101));

                        // No change for the defensive team
                    } else if (diff == 3) {
                        // 33% increase of shooting the 3 ball
                        _endGameThreePointAddition = (int) (GetCurrentPlayersTendancies().ThreePointTendancy * 0.33);
                        // increase is shot overall
                        if (_time > 14 || _shotClock > 14) {
                            _endGameShotClockBonus = 0;
                        } else if (_time > 12 || _shotClock > 12) {
                            _endGameShotClockBonus = 50;
                        } else if (_time > 10 || _shotClock > 10) {
                            _endGameShotClockBonus = 100;
                        } else if (_time > 8 || _shotClock > 8) {
                            _endGameShotClockBonus = 150;
                        } else if (_time > 6 || _shotClock > 6) {
                            _endGameShotClockBonus = 250;
                        } else if (_time > 4 || _shotClock > 4) {
                            _endGameShotClockBonus = 450;
                        } else {
                            _endGameShotClockBonus = 600;
                        }

                        // no defensive change
                    } else if (diff <= 2) {
                        // normal tendancies
                        // increase shot bonus as time runs out
                        if (_time > 14 || _shotClock > 14) {
                            _endGameShotClockBonus = -100;
                        } else if (_time > 12 || _shotClock > 12) {
                            _endGameShotClockBonus = -50;
                        } else if (_time > 10 || _shotClock > 10) {
                            _endGameShotClockBonus = 0;
                        } else if (_time > 8 || _shotClock > 8) {
                            _endGameShotClockBonus = 100;
                        } else if (_time > 6 || _shotClock > 6) {
                            _endGameShotClockBonus = 250;
                        } else if (_time > 4 || _shotClock > 4) {
                            _endGameShotClockBonus = 450;
                        } else {
                            _endGameShotClockBonus = 600;
                        }

                        // no defencive change
                    }
                }
            } else {
                diff = _homeScore - _awayScore;

                if (diff < 0) {
                    // away team is winning
                    if (_time > 16 || _shotClock > 16) {
                        _endGameShotClockBonus = -200;
                    } else if (_time > 12 || _shotClock > 12) {
                        _endGameShotClockBonus = -100;
                    } else if (_time > 8 || _shotClock > 8) {
                        _endGameShotClockBonus = 100;
                    } else if (_time > 4 || _shotClock > 4) {
                        _endGameShotClockBonus = 300;
                    } else {
                        _endGameShotClockBonus = 500;
                    }

                    
                    if (diff <= 3 && (_shotClock > _time)) {
                        // Defensive teams actions
                        // increased steal chance
                        _endGameStealAddition = 500;
    
                        // much increased in fouls
                        _endGameFoulAddition = 800;
                    }
                } else if (diff > 0) {
                    // away team is losing
                    // losing margins
                    if (diff == 5 || diff == 6) {
                        // Shooting liklihood is increase significantly and 3's are increased most
                        // team will shoot quicker
                        if (_time > 16 || _shotClock > 16) {
                            _endGameShotClockBonus = 50;
                        } else if (_time > 14 || _shotClock > 14) {
                            _endGameShotClockBonus = 100;
                        } else if (_time > 12 || _shotClock > 12) {
                            _endGameShotClockBonus = 150;
                        } else if (_time > 10 || _shotClock > 10) {
                            _endGameShotClockBonus = 200;
                        } else if (_time > 8 || _shotClock > 8) {
                            _endGameShotClockBonus = 300;
                        } else if (_time > 6 || _shotClock > 6) {
                            _endGameShotClockBonus = 400;
                        } else if (_time > 4 || _shotClock > 4) {
                            _endGameShotClockBonus = 500;
                        } else {
                            _endGameShotClockBonus = 600;
                        }

                        // increase in 3s taken
                        _endGameThreePointAddition = (int) (GetCurrentPlayersTendancies().ThreePointTendancy * 0.2);

                        // Result increase by 15% due to tough shots
                        _endGameResultIncrease = 150;

                        // Apply winning to team
                        // no changes for defensive team
                    } else if (diff == 4) {
                        // team will shoot quicker
                        if (_time > 16 || _shotClock > 16) {
                            _endGameShotClockBonus = 50;
                        } else if (_time > 14 || _shotClock > 14) {
                            _endGameShotClockBonus = 100;
                        } else if (_time > 12 || _shotClock > 12) {
                            _endGameShotClockBonus = 150;
                        } else if (_time > 10 || _shotClock > 10) {
                            _endGameShotClockBonus = 200;
                        } else if (_time > 8 || _shotClock > 8) {
                            _endGameShotClockBonus = 300;
                        } else if (_time > 6 || _shotClock > 6) {
                            _endGameShotClockBonus = 400;
                        } else if (_time > 4 || _shotClock > 4) {
                            _endGameShotClockBonus = 500;
                        } else {
                            _endGameShotClockBonus = 600;
                        }
                        
                        // random between 5 and 10% added to shot result
                        _endGameResultIncrease = (_random.Next(50, 101));

                        // No change for the defensive team
                    } else if (diff == 3) {
                        // 33% increase of shooting the 3 ball
                        _endGameThreePointAddition = (int) (GetCurrentPlayersTendancies().ThreePointTendancy * 0.33);
                        // increase is shot overall
                        if (_time > 14 || _shotClock > 14) {
                            _endGameShotClockBonus = 0;
                        } else if (_time > 12 || _shotClock > 12) {
                            _endGameShotClockBonus = 50;
                        } else if (_time > 10 || _shotClock > 10) {
                            _endGameShotClockBonus = 100;
                        } else if (_time > 8 || _shotClock > 8) {
                            _endGameShotClockBonus = 150;
                        } else if (_time > 6 || _shotClock > 6) {
                            _endGameShotClockBonus = 250;
                        } else if (_time > 4 || _shotClock > 4) {
                            _endGameShotClockBonus = 450;
                        } else {
                            _endGameShotClockBonus = 600;
                        }

                        // no defensive change
                    } else if (diff <= 2) {
                        // normal tendancies
                        // increase shot bonus as time runs out
                        if (_time > 14 || _shotClock > 14) {
                            _endGameShotClockBonus = -100;
                        } else if (_time > 12 || _shotClock > 12) {
                            _endGameShotClockBonus = -50;
                        } else if (_time > 10 || _shotClock > 10) {
                            _endGameShotClockBonus = 0;
                        } else if (_time > 8 || _shotClock > 8) {
                            _endGameShotClockBonus = 100;
                        } else if (_time > 6 || _shotClock > 6) {
                            _endGameShotClockBonus = 250;
                        } else if (_time > 4 || _shotClock > 4) {
                            _endGameShotClockBonus = 450;
                        } else {
                            _endGameShotClockBonus = 600;
                        }

                        // no defencive change
                    }
                } 
            }
        }

        public int ShotClockShootBonus()
        {
            if (_shotClock > 22)
            {
                return -200;
            }
            else if (_shotClock > 20)
            {
                return -100;
            }
            else if (_shotClock > 18)
            {
                return -25;
            } 
            else if (_shotClock > 14)
            {
                return 35;
            }
            else if (_shotClock > 12)
            {
                return 80;
            }
            else if (_shotClock > 10)
            {
                return 140;
            }
            else if (_shotClock > 8)
            {
                return 250;
            }
            else if (_shotClock > 6)
            {
                return 300;
            }
            else if (_shotClock > 4)
            {
                return 400;
            }
            else
            {
                return 500;
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
                    } else {
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

            Inbounding inbound = new Inbounding();
            int value = inbound.GetInboundsResult(_random.Next(100));
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
            } else
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
            if (team == 0)
            {
                int pgVal = homePGRatings.ORPMRating / 10;
                int sgVal = homeSGRatings.ORPMRating / 10;
                int sfVal = homeSFRatings.ORPMRating / 10;
                int pfVal = homePFRatings.ORPMRating / 10;
                int cVal = homeCRatings.ORPMRating / 10;

                int total = pgVal + sgVal + sfVal + pfVal + cVal;
                int valueToReturn = total / 5;
                return valueToReturn;
            }
            else
            {
                int pgVal = awayPGRatings.ORPMRating / 10;
                int sgVal = awaySGRatings.ORPMRating / 10;
                int sfVal = awaySFRatings.ORPMRating / 10;
                int pfVal = awayPFRatings.ORPMRating / 10;
                int cVal = awayCRatings.ORPMRating / 10;

                int total = pgVal + sgVal + sfVal + pfVal + cVal;
                int valueToReturn = total / 5;
                return valueToReturn;
            }
        }

        /* REFACTORED */
        public int GetDrpmValue(int team)
        {
            if (team == 0)
            {
                int pgVal = homePGRatings.DRPMRating / 10;
                int sgVal = homeSGRatings.DRPMRating / 10;
                int sfVal = homeSFRatings.DRPMRating / 10;
                int pfVal = homePFRatings.DRPMRating / 10;
                int cVal = homeCRatings.DRPMRating / 10;

                int total = pgVal + sgVal + sfVal + pfVal + cVal;
                int valueToReturn = total / 5;
                return valueToReturn;
            }
            else
            {
                int pgVal = awayPGRatings.DRPMRating / 10;
                int sgVal = awaySGRatings.DRPMRating / 10;
                int sfVal = awaySFRatings.DRPMRating / 10;
                int pfVal = awayPFRatings.DRPMRating / 10;
                int cVal = awayCRatings.DRPMRating / 10;

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
                switch (player)
                {
                    case 1:
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homePG.Id);
                        fouls = temp.Fouls;
                        break;
                    case 2:
                        BoxScore temp2 = _homeBoxScores.Find(x => x.Id == homeSG.Id);
                        fouls = temp2.Fouls;
                        break;
                    case 3:
                        BoxScore temp3 = _homeBoxScores.Find(x => x.Id == homeSF.Id);
                        fouls = temp3.Fouls;
                        break;
                    case 4:
                        BoxScore temp4 = _homeBoxScores.Find(x => x.Id == homePF.Id);
                        fouls = temp4.Fouls;
                        break;
                    case 5:
                        BoxScore temp5 = _homeBoxScores.Find(x => x.Id == homeC.Id);
                        fouls = temp5.Fouls;
                        break;
                }
            }
            else
            {
                switch (player)
                {
                    case 1:
                        BoxScore temp6 = _awayBoxScores.Find(x => x.Id == awayPG.Id);
                        fouls = temp6.Fouls;
                        break;
                    case 2:
                        BoxScore temp7 = _awayBoxScores.Find(x => x.Id == awaySG.Id);
                        fouls = temp7.Fouls;
                        break;
                    case 3:
                        BoxScore temp8 = _awayBoxScores.Find(x => x.Id == awaySF.Id);
                        fouls = temp8.Fouls;
                        break;
                    case 4:
                        BoxScore temp9 = _awayBoxScores.Find(x => x.Id == awayPF.Id);
                        fouls = temp9.Fouls;
                        break;
                    case 5:
                        BoxScore temp10 = _awayBoxScores.Find(x => x.Id == awayC.Id);
                        fouls = temp10.Fouls;
                        break;
                }
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

            if (fouls == 6)
            {
                subOut = 1;
            }
            return subOut;
        }

        public void SubFoulTrouble(int team, int player)
        {
            if (team == 0) {
                List<int> onCourtIds = new List<int>();
                onCourtIds.Add(homePG.Id);
                onCourtIds.Add(homeSG.Id);
                onCourtIds.Add(homeSF.Id);
                onCourtIds.Add(homePF.Id);
                onCourtIds.Add(homeC.Id);

                DepthChart dc = new DepthChart();

                switch (player)
                {
                    case 1:
                        int depth = _homeSubTracker.CurrentPG + 1;
                        dc = _homeDepth.FirstOrDefault(x => x.Depth == depth && x.Position == 1);
                        var exists = onCourtIds.Contains(dc.PlayerId);

                        if(!exists)
                        {
                            int check = FoulTroubleCheck(team, dc.PlayerId);

                            if (check == 0) {
                                // The player is not in foul trouble
                                Player current = homePG;
                                homePG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                homePGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                                // Need to update the stamina track objects for on and off court
                                StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                                stOff.OnOff = 0;
                                int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                _homeStaminas[index] = stOff;

                                StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePG.Id);
                                stOn.OnOff = 1;
                                index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                _homeStaminas[index] = stOn;

                                _homeSubTracker.CurrentPG = depth;
                            } else if (depth != 3) {
                                depth++;

                                Player current = homePG;
                                homePG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                homePGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                                // Need to update the stamina track objects for on and off court
                                StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                                stOff.OnOff = 0;
                                int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                _homeStaminas[index] = stOff;

                                StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePG.Id);
                                stOn.OnOff = 1;
                                index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                _homeStaminas[index] = stOn;

                                _homeSubTracker.CurrentPG = depth;
                            }
                        } else if (depth != 3) {
                            depth++;

                            Player current = homePG;
                            homePG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homePGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePG.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;

                            _homeSubTracker.CurrentPG = depth;   
                        }
                        break;
                    case 2:
                        int depth2 = _homeSubTracker.CurrentPG + 1;
                        dc = _homeDepth.FirstOrDefault(x => x.Depth == depth2 && x.Position == 2);
                        var exists2 = onCourtIds.Contains(dc.PlayerId);

                        if(!exists2)
                        {
                            int check = FoulTroubleCheck(team, dc.PlayerId);

                            if (check == 0) {
                                // The player is not in foul trouble
                                Player current = homeSG;
                                homeSG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                homeSGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                                // Need to update the stamina track objects for on and off court
                                StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                                stOff.OnOff = 0;
                                int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                _homeStaminas[index] = stOff;

                                StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSG.Id);
                                stOn.OnOff = 1;
                                index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                _homeStaminas[index] = stOn;

                                _homeSubTracker.CurrentSG = depth2;
                            } else if (depth2 != 3) {
                                depth2++;

                                Player current = homeSG;
                                homeSG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                homeSGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                                // Need to update the stamina track objects for on and off court
                                StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                                stOff.OnOff = 0;
                                int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                _homeStaminas[index] = stOff;

                                StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSG.Id);
                                stOn.OnOff = 1;
                                index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                _homeStaminas[index] = stOn;

                                _homeSubTracker.CurrentPG = depth2;
                            }
                        } else if (depth2 != 3) {
                            depth2++;

                            Player current = homeSG;
                            homeSG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homeSGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSG.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;

                            _homeSubTracker.CurrentPG = depth2;   
                        }
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
            } else {

            }
        }
       
        public void Substitution(int team, int position)
        {
            // // Now need to sort out the sub commentary
            // string outPlayer = "";
            // string inPlayer = "";

            if (team == 0)
            {
                // Need to find the correct position
                switch (position)
                {
                    case 1:
                        Player current = homePG;
                        // outPlayer = current.FirstName + " " + current.Surname;

                        PlayerRating currentRatings = homePGRatings;

                        List<DepthChart> depth = _homeDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> filterDepth = depth.Where(x => x.Position == 1).ToList();
                        int homePGSet = 0;

                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < filterDepth.Count; i++)
                        {
                            DepthChart dc = filterDepth[i];

                            var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(homePG.Id);
                                onCourtIds.Add(homeSG.Id);
                                onCourtIds.Add(homeSF.Id);
                                onCourtIds.Add(homePF.Id);
                                onCourtIds.Add(homeC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);
                                if(!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        homePG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = homePG.FirstName + " " + homePG.Surname;
                                        homePGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        homePGSet = 1;

                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                                        stOff.OnOff = 0;
                                        int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _homeStaminas[index] = stOff;

                                        StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePG.Id);
                                        stOn.OnOff = 1;
                                        index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _homeStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

                                        break;
                                    }
                                }
                            }
                        }

                        if (homePGSet != 1)
                        {
                            // We do not have a player set for the sub
                            // The player stays on
                        }
                        break;
                    case 2:
                        Player currentHomeSG = homeSG;
                        // outPlayer = currentHomeSG.FirstName + " " + currentHomeSG.Surname;
                        PlayerRating currentHomeSGRatings = homeSGRatings;
                        int homeSGSet = 0;

                        List<DepthChart> homeSGdepth = _homeDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> homeSGfilterDepth = homeSGdepth.Where(x => x.Position == 2).ToList();

                        for (int i = 0; i < homeSGfilterDepth.Count; i++)
                        {
                            DepthChart dc = homeSGfilterDepth[i];

                            var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(homePG.Id);
                                onCourtIds.Add(homeSG.Id);
                                onCourtIds.Add(homeSF.Id);
                                onCourtIds.Add(homePF.Id);
                                onCourtIds.Add(homeC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        homeSG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = homeSG.FirstName + " " + homeSG.Surname;
                                        homeSGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        homeSGSet = 1;

                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomeSG.Id);
                                        stOff.OnOff = 0;
                                        int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _homeStaminas[index] = stOff;

                                        StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSG.Id);
                                        stOn.OnOff = 1;
                                        index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _homeStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

                                        break;
                                    }
                                }
                            }
                        }

                        if (homeSGSet != 1)
                        {
                            // We do not have a player set for the sub
                            // LEave him on the court
                        }
                        break;
                    case 3:
                        Player currentHomeSF = homeSF;
                        // outPlayer = currentHomeSF.FirstName + " " + currentHomeSF.Surname;
                        PlayerRating currentHomeSFRatings = homeSFRatings;
                        int homeSFSet = 0;

                        List<DepthChart> homeSFdepth = _homeDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> homeSFfilterDepth = homeSFdepth.Where(x => x.Position == 3).ToList();
                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < homeSFfilterDepth.Count; i++)
                        {
                            DepthChart dc = homeSFfilterDepth[i];

                            var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(homePG.Id);
                                onCourtIds.Add(homeSG.Id);
                                onCourtIds.Add(homeSF.Id);
                                onCourtIds.Add(homePF.Id);
                                onCourtIds.Add(homeC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        homeSF = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = homeSF.FirstName + " " + homeSF.Surname;
                                        homeSFRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        homeSFSet = 1;

                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomeSF.Id);
                                        stOff.OnOff = 0;
                                        int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _homeStaminas[index] = stOff;

                                        StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSF.Id);
                                        stOn.OnOff = 1;
                                        index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _homeStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

                                        break;
                                    }
                                }
                            }
                        }

                        if (homeSFSet != 1)
                        {
                            // We do not have a player set for the sub
                            // Leave the player on the court
                        }
                        break;
                    case 4:
                        Player currentHomePF = homePF;
                        PlayerRating currentHomePFRatings = homePFRatings;
                        // outPlayer = currentHomePF.FirstName + " " + currentHomePF.Surname;
                        int homePFSet = 0;

                        List<DepthChart> homePFdepth = _homeDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> homePFfilterDepth = homePFdepth.Where(x => x.Position == 4).ToList();
                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < homePFfilterDepth.Count; i++)
                        {
                            DepthChart dc = homePFfilterDepth[i];
                            var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(homePG.Id);
                                onCourtIds.Add(homeSG.Id);
                                onCourtIds.Add(homeSF.Id);
                                onCourtIds.Add(homePF.Id);
                                onCourtIds.Add(homeC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        homePF = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = homePF.FirstName + " " + homePF.Surname;
                                        homePFRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        homePFSet = 1;

                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomePF.Id);
                                        stOff.OnOff = 0;
                                        int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _homeStaminas[index] = stOff;

                                        StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePF.Id);
                                        stOn.OnOff = 1;
                                        index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _homeStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

                                        break;
                                    }
                                }
                            }
                        }

                        if (homePFSet != 1)
                        {
                            // We do not have a player set for the sub
                            // LEave the player on the court
                        }
                        break;
                    case 5:
                        Player currentHomeC = homeC;
                        PlayerRating currentHomeCRatings = homeCRatings;
                        // outPlayer = currentHomeC.FirstName + " " + currentHomeC.Surname;
                        int homeCSet = 0;

                        List<DepthChart> homeCdepth = _homeDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> homeCfilterDepth = homeCdepth.Where(x => x.Position == 5).ToList();
                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < homeCfilterDepth.Count; i++)
                        {
                            DepthChart dc = homeCfilterDepth[i];
                            var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(homePG.Id);
                                onCourtIds.Add(homeSG.Id);
                                onCourtIds.Add(homeSF.Id);
                                onCourtIds.Add(homePF.Id);
                                onCourtIds.Add(homeC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        homeC = _homePlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = homeC.FirstName + " " + homeC.Surname;
                                        homeCRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        homeCSet = 1;

                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomeC.Id);
                                        stOff.OnOff = 0;
                                        int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _homeStaminas[index] = stOff;

                                        StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeC.Id);
                                        stOn.OnOff = 1;
                                        index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _homeStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        break;
                                    }
                                }
                            }
                        }

                        if (homeCSet != 1)
                        {
                            // We do not have a player set for the sub
                            // LEave player on the court
                        }
                        break;
                    default:
                        commentaryData.Add("Error making substitution");
                        break;
                }
            }
            else
            {
                switch (position)
                {
                    case 1:
                        Player current = awayPG;
                        // outPlayer = current.FirstName + " " + current.Surname;
                        PlayerRating currentRatings = awayPGRatings;
                        int awayPGSet = 0;

                        List<DepthChart> depth = _awayDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> filterDepth = depth.Where(x => x.Position == 1).ToList();
                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < filterDepth.Count; i++)
                        {
                            DepthChart dc = filterDepth[i];
                            var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(awayPG.Id);
                                onCourtIds.Add(awaySG.Id);
                                onCourtIds.Add(awaySF.Id);
                                onCourtIds.Add(awayPF.Id);
                                onCourtIds.Add(awayC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        awayPG = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = awayPG.FirstName + " " + awayPG.Surname;
                                        awayPGRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        awayPGSet = 1;

                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                                        stOff.OnOff = 0;
                                        int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _awayStaminas[index] = stOff;

                                        StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayPG.Id);
                                        stOn.OnOff = 1;
                                        index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _awayStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        break;
                                    }
                                }
                            }
                        }

                        if (awayPGSet != 1)
                        {
                            // We do not have a player set for the sub

                        }
                        break;
                    case 2:
                        Player currentAwaySG = awaySG;
                        // outPlayer = currentAwaySG.FirstName + " " + currentAwaySG.Surname;
                        PlayerRating currentAwaySGRatings = awaySGRatings;
                        int awaySGSet = 0;

                        List<DepthChart> awaySGdepth = _awayDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> awaySGfilterDepth = awaySGdepth.Where(x => x.Position == 2).ToList();
                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < awaySGfilterDepth.Count; i++)
                        {
                            DepthChart dc = awaySGfilterDepth[i];
                            var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(awayPG.Id);
                                onCourtIds.Add(awaySG.Id);
                                onCourtIds.Add(awaySF.Id);
                                onCourtIds.Add(awayPF.Id);
                                onCourtIds.Add(awayC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        awaySG = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = awaySG.FirstName + " " + awaySG.Surname;
                                        awaySGRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        awaySGSet = 1;
                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwaySG.Id);
                                        stOff.OnOff = 0;
                                        int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _awayStaminas[index] = stOff;

                                        StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awaySG.Id);
                                        stOn.OnOff = 1;
                                        index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _awayStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        break;
                                    }
                                }
                            }
                        }

                        if (awaySGSet != 1)
                        {
                            // We do not have a player set for the sub

                        }
                        break;
                    case 3:
                        Player currentAwaySF = awaySF;
                        // outPlayer = currentAwaySF.FirstName + " " + currentAwaySF.Surname;
                        PlayerRating currentAwaySFRatings = awaySFRatings;
                        int awaySFSet = 0;

                        List<DepthChart> awaySFdepth = _awayDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> awaySFfilterDepth = awaySFdepth.Where(x => x.Position == 3).ToList();
                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < awaySFfilterDepth.Count; i++)
                        {
                            DepthChart dc = awaySFfilterDepth[i];
                            var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(awayPG.Id);
                                onCourtIds.Add(awaySG.Id);
                                onCourtIds.Add(awaySF.Id);
                                onCourtIds.Add(awayPF.Id);
                                onCourtIds.Add(awayC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        awaySF = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = awaySF.FirstName + " " + awaySF.Surname;
                                        awaySFRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        awaySFSet = 1;
                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwaySF.Id);
                                        stOff.OnOff = 0;
                                        int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _awayStaminas[index] = stOff;

                                        StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awaySF.Id);
                                        stOn.OnOff = 1;
                                        index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _awayStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        break;
                                    }
                                }
                            }
                        }

                        if (awaySFSet != 1)
                        {
                            // We do not have a player set for the sub

                        }
                        break;
                    case 4:
                        Player currentAwayPF = awayPF;
                        // outPlayer = currentAwayPF.FirstName + " " + currentAwayPF.Surname;
                        PlayerRating currentAwayPFRatings = awayPFRatings;
                        int awayPFSet = 0;

                        List<DepthChart> awayPFdepth = _awayDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> awayPFfilterDepth = awayPFdepth.Where(x => x.Position == 4).ToList();
                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < awayPFfilterDepth.Count; i++)
                        {
                            DepthChart dc = awayPFfilterDepth[i];
                            var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(awayPG.Id);
                                onCourtIds.Add(awaySG.Id);
                                onCourtIds.Add(awaySF.Id);
                                onCourtIds.Add(awayPF.Id);
                                onCourtIds.Add(awayC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        awayPF = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = awayPF.FirstName + " " + awayPF.Surname;
                                        awayPFRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        awayPFSet = 1;

                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwayPF.Id);
                                        stOff.OnOff = 0;
                                        int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _awayStaminas[index] = stOff;

                                        StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayPF.Id);
                                        stOn.OnOff = 1;
                                        index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _awayStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        break;
                                    }
                                }
                            }
                        }

                        if (awayPFSet != 1)
                        {
                            // We do not have a player set for the sub

                        }
                        break;
                    case 5:
                        Player currentAwayC = awayC;
                        // outPlayer = currentAwayC.FirstName + " " + currentAwayC.Surname;
                        PlayerRating currentAwayCRatings = awayCRatings;
                        int awayCSet = 0;

                        List<DepthChart> awayCdepth = _awayDepth.OrderBy(x => x.Depth).ToList();
                        List<DepthChart> awayCfilterDepth = awayCdepth.Where(x => x.Position == 5).ToList();
                        // Now need to determine which player is being subbed in
                        for (int i = 0; i < awayCfilterDepth.Count; i++)
                        {
                            DepthChart dc = awayCfilterDepth[i];
                            var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);
                            if (bs.Fouls < 6) {
                                // Carry on as it is the correct position
                                // Need to check that the player is not on the court in another position
                                // Get list of the all of the current player ids
                                List<int> onCourtIds = new List<int>();
                                onCourtIds.Add(awayPG.Id);
                                onCourtIds.Add(awaySG.Id);
                                onCourtIds.Add(awaySF.Id);
                                onCourtIds.Add(awayPF.Id);
                                onCourtIds.Add(awayC.Id);

                                var exists = onCourtIds.Contains(dc.PlayerId);

                                if (!exists)
                                {
                                    // That way we don't accidently keep the same player on
                                    // Now need to check whether this player is not in foul trouble too
                                    int check = FoulTroubleCheck(team, dc.PlayerId);

                                    if (check == 0)
                                    {
                                        // Player can be subbed
                                        // Now need to get the player object from the roster
                                        awayC = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                                        // inPlayer = awayC.FirstName + " " + awayC.Surname;
                                        awayCRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                                        awayCSet = 1;

                                        // Need to update the stamina track objects for on and off court
                                        StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwayC.Id);
                                        stOff.OnOff = 0;
                                        int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                                        _awayStaminas[index] = stOff;

                                        StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayC.Id);
                                        stOn.OnOff = 1;
                                        index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                                        _awayStaminas[index] = stOn;

                                        // // Update the Commentary for the Sub
                                        // commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                                        break;
                                    }
                                }
                            }
                        }

                        if (awayCSet != 1)
                        {
                            // We do not have a player set for the sub
                        }
                        break;
                    default:
                        commentaryData.Add("Error making substitution");
                        break;
                }
            }

            
        }

        public void SubCheck()
        {
            if (_quarter > 3 && _time <= 240) {
                // This will now change how the subs are done - will go to a closing lineup
                // var starters = _homeDepth/ (x => x.Depth == 1);

                for (int i = 1; i < 6; i++) {
                    var starter = _homeDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == i);

                    // have the starter that should be on
                    switch (i)
                    {   
                        case 1:
                            if (starter.PlayerId != homePG.Id) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        case 2:
                            if (starter.PlayerId != homeSG.Id) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        case 3:
                            if (starter.PlayerId != homeSF.Id) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        case 4:
                            if (starter.PlayerId != homePF.Id) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        case 5:
                            if (starter.PlayerId != homeC.Id) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        default:
                            break;
                    }
                }

                 for (int i = 1; i < 6; i++) {
                    var starter = _awayDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == i);

                    // have the starter that should be on
                    switch (i)
                    {   
                        case 1:
                            if (starter.PlayerId != awayPG.Id) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        case 2:
                            if (starter.PlayerId != awaySG.Id) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        case 3:
                            if (starter.PlayerId != awaySF.Id) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        case 4:
                            if (starter.PlayerId != awayPF.Id) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        case 5:
                            if (starter.PlayerId != awayC.Id) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        default:
                            break;
                    }
                }
            } else {
                // This is for normal sub patterns based off stamina   
                // First Check the Home Team
                for (int i = 0; i < _homeStaminas.Count; i++)
                {
                    StaminaTrack st = _homeStaminas[i];

                    if (st.StaminaValue > 700 && st.OnOff == 1)
                    {
                        // Player should be subbed out
                        // Need to determine which position the player is currently at
                        int playerid = st.PlayerId;
                        int position = 0;
                        Player current = new Player();

                        if (homePG.Id == playerid)
                        {
                            position = 1;
                            current = homePG;
                        }
                        else if (homeSG.Id == playerid)
                        {
                            position = 2;
                            current = homeSG;
                        }
                        else if (homeSF.Id == playerid)
                        {
                            position = 3;
                            current = homeSF;
                        }
                        else if (homePF.Id == playerid)
                        {
                            position = 4;
                            current = homePF;
                        }
                        else if (homeC.Id == playerid)
                        {
                            position = 5;
                            current = homeC;
                        }

                        Substitution(0, position);

                        // Now need to sort out the sub commentary
                        string outPlayer = current.FirstName + " " + current.Surname;
                        string inPlayer = "";

                        switch (position)
                        {
                            case 1:
                                inPlayer = homePG.FirstName + " " + homePG.Surname;
                                break;
                            case 2:
                                inPlayer = homeSG.FirstName + " " + homeSG.Surname;
                                break;
                            case 3:
                                inPlayer = homeSF.FirstName + " " + homeSF.Surname;
                                break;
                            case 4:
                                inPlayer = homePF.FirstName + " " + homePF.Surname;
                                break;
                            case 5:
                                inPlayer = homeC.FirstName + " " + homeC.Surname;
                                break;
                        }
                        
                        commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                    }
                }

                // Now check the Away Team
                for (int i = 0; i < _awayStaminas.Count; i++)
                {
                    StaminaTrack st = _awayStaminas[i];
                    PlayerRating currentShooter = GetCurrentPlayersRatings();

                    if (st.StaminaValue > 700 && st.OnOff == 1)
                    {
                        // Player should be subbed out
                        // Need to determine which position the player is currently at
                        int playerid = st.PlayerId;
                        int position = 0;
                        Player current = new Player();

                        if (awayPG.Id == playerid)
                        {
                            position = 1;
                            current = awayPG;
                        }
                        else if (awaySG.Id == playerid)
                        {
                            position = 2;
                            current = awaySG;
                        }
                        else if (awaySF.Id == playerid)
                        {
                            position = 3;
                            current = awaySF;
                        }
                        else if (awayPF.Id == playerid)
                        {
                            position = 4;
                            current = awayPF;
                        }
                        else if (awayC.Id == playerid)
                        {
                            position = 5;
                            current = awayC;
                        }

                        Substitution(1, position);

                        // Now need to sort out the sub commentary
                        string outPlayer = current.FirstName + " " + current.Surname;
                        string inPlayer = "";

                        switch (position)
                        {
                            case 1:
                                inPlayer = awayPG.FirstName + " " + awayPG.Surname;
                                break;
                            case 2:
                                inPlayer = awaySG.FirstName + " " + awaySG.Surname;
                                break;
                            case 3:
                                inPlayer = awaySF.FirstName + " " + awaySF.Surname;
                                break;
                            case 4:
                                inPlayer = awayPF.FirstName + " " + awayPF.Surname;
                                break;
                            case 5:
                                inPlayer = awayC.FirstName + " " + awayC.Surname;
                                break;
                        }
                        commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                        // Console.WriteLine(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                    }
                }
            }
        }

        public void SubCheckFT()
        {
            if (_quarter > 3 && _time <= 240) {
                // This will now change how the subs are done - will go to a closing lineup
                // var starters = _homeDepth/ (x => x.Depth == 1);
                PlayerRating currentShooter = GetCurrentPlayersRatings();

                for (int i = 1; i < 6; i++) {
                    var starter = _homeDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == i);

                    // have the starter that should be on
                    switch (i)
                    {   
                        case 1:
                            if (starter.PlayerId != homePG.Id && homePG.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        case 2:
                            if (starter.PlayerId != homeSG.Id && homeSG.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        case 3:
                            if (starter.PlayerId != homeSF.Id && homeSF.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        case 4:
                            if (starter.PlayerId != homePF.Id && homePF.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        case 5:
                            if (starter.PlayerId != homeC.Id && homeC.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(0, i);
                            }
                            break;
                        default:
                            break;
                    }
                }

                 for (int i = 1; i < 6; i++) {
                    var starter = _awayDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == i);

                    // have the starter that should be on
                    switch (i)
                    {   
                        case 1:
                            if (starter.PlayerId != awayPG.Id && awayPG.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        case 2:
                            if (starter.PlayerId != awaySG.Id && awaySG.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        case 3:
                            if (starter.PlayerId != awaySF.Id && awaySF.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        case 4:
                            if (starter.PlayerId != awayPF.Id && awayPF.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        case 5:
                            if (starter.PlayerId != awayC.Id && awayC.Id != currentShooter.PlayerId) {
                                // Sub needed
                                SubToStarter(1, i);
                            }
                            break;
                        default:
                            break;
                    }
                }
            } else {
                // This is for normal sub patterns based off stamina

                // First Check the Home Team
                for (int i = 0; i < _homeStaminas.Count; i++)
                {
                    StaminaTrack st = _homeStaminas[i];
                    PlayerRating currentShooter = GetCurrentPlayersRatings();

                    if (st.PlayerId != currentShooter.PlayerId)
                    {
                        if (st.StaminaValue > 700 && st.OnOff == 1)
                        {
                            // Player should be subbed out
                            // Need to determine which position the player is currently at
                            int playerid = st.PlayerId;
                            int position = 0;
                            Player current = new Player();

                            if (homePG.Id == playerid)
                            {
                                position = 1;
                                current = homePG;
                            }
                            else if (homeSG.Id == playerid)
                            {
                                position = 2;
                                current = homeSG;
                            }
                            else if (homeSF.Id == playerid)
                            {
                                position = 3;
                                current = homeSF;
                            }
                            else if (homePF.Id == playerid)
                            {
                                position = 4;
                                current = homePF;
                            }
                            else if (homeC.Id == playerid)
                            {
                                position = 5;
                                current = homeC;
                            }

                            Substitution(0, position);

                            // Now need to sort out the sub commentary
                            string outPlayer = current.FirstName + " " + current.Surname;
                            string inPlayer = "";

                            switch (position)
                            {
                                case 1:
                                    inPlayer = homePG.FirstName + " " + homePG.Surname;
                                    break;
                                case 2:
                                    inPlayer = homeSG.FirstName + " " + homeSG.Surname;
                                    break;
                                case 3:
                                    inPlayer = homeSF.FirstName + " " + homeSF.Surname;
                                    break;
                                case 4:
                                    inPlayer = homePF.FirstName + " " + homePF.Surname;
                                    break;
                                case 5:
                                    inPlayer = homeC.FirstName + " " + homeC.Surname;
                                    break;
                            }

                            commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                            PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                        }
                    }
                }

                // Now check the Away Team
                for (int i = 0; i < _awayStaminas.Count; i++)
                {
                    StaminaTrack st = _awayStaminas[i];
                    PlayerRating currentShooter = GetCurrentPlayersRatings();
                    if (st.PlayerId != currentShooter.PlayerId)
                    {

                        if (st.StaminaValue > 700 && st.OnOff == 1)
                        {
                            // Player should be subbed out
                            // Need to determine which position the player is currently at
                            int playerid = st.PlayerId;
                            int position = 0;
                            Player current = new Player();

                            if (awayPG.Id == playerid)
                            {
                                position = 1;
                                current = awayPG;
                            }
                            else if (awaySG.Id == playerid)
                            {
                                position = 2;
                                current = awaySG;
                            }
                            else if (awaySF.Id == playerid)
                            {
                                position = 3;
                                current = awaySF;
                            }
                            else if (awayPF.Id == playerid)
                            {
                                position = 4;
                                current = awayPF;
                            }
                            else if (awayC.Id == playerid)
                            {
                                position = 5;
                                current = awayC;
                            }

                            Substitution(1, position);

                            // Now need to sort out the sub commentary
                            string outPlayer = current.FirstName + " " + current.Surname;
                            string inPlayer = "";

                            switch (position)
                            {
                                case 1:
                                    inPlayer = awayPG.FirstName + " " + awayPG.Surname;
                                    break;
                                case 2:
                                    inPlayer = awaySG.FirstName + " " + awaySG.Surname;
                                    break;
                                case 3:
                                    inPlayer = awaySF.FirstName + " " + awaySF.Surname;
                                    break;
                                case 4:
                                    inPlayer = awayPF.FirstName + " " + awayPF.Surname;
                                    break;
                                case 5:
                                    inPlayer = awayC.FirstName + " " + awayC.Surname;
                                    break;
                            }
                            commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                            PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 1);
                        }
                    }
                }
            }
        }

        public void SubToStarter(int team, int position)
        {
            List<int> onCourtIds = new List<int>();
            onCourtIds.Add(homePG.Id);
            onCourtIds.Add(homeSG.Id);
            onCourtIds.Add(homeSF.Id);
            onCourtIds.Add(homePF.Id);
            onCourtIds.Add(homeC.Id); // TODO add the check // TODO add foul trouble check

            Player current = new Player();
            var subbed = 0;

            if (team == 0) {

                    if (position == 1) {
                        current = homePG;

                        // Get the player from the depth chart
                        var dc = _homeDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 1);
                        var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            homePG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homePGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePG.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;

                            subbed = 1;
                        }
                    } else if (position == 2) {
                        current = homeSG;

                        // Get the player from the depth chart
                        var dc = _homeDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 2);

                         var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {    
                            homeSG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homeSGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSG.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;

                            subbed = 1;
                        }
                    } else if (position == 3) {
                        current = homeSF;

                        // Get the player from the depth chart
                        var dc = _homeDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 3);
                         var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            homeSF = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homeSFRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSF.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;

                            subbed = 1;
                        }
                    } else if (position == 4) {
                        current = homePF;

                        // Get the player from the depth chart
                        var dc = _homeDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 4);
                         var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            homePF = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homePFRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePF.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;

                            subbed = 1;
                        }
                    } else if (position == 5) {
                        current = homeC;

                        // Get the player from the depth chart
                        var dc = _homeDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 5);
                        var bs = _homeBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            homeC = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homeCRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeC.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;

                            subbed = 1;
                        }
                    }

                    if (subbed == 1) {
                        // Now need to sort out the sub commentary
                        string outPlayer = current.FirstName + " " + current.Surname;
                        string inPlayer = "";

                        switch (position)
                        {
                            case 1:
                                inPlayer = homePG.FirstName + " " + homePG.Surname;
                                break;
                            case 2:
                                inPlayer = homeSG.FirstName + " " + homeSG.Surname;
                                break;
                            case 3:
                                inPlayer = homeSF.FirstName + " " + homeSF.Surname;
                                break;
                            case 4:
                                inPlayer = homePF.FirstName + " " + homePF.Surname;
                                break;
                            case 5:
                                inPlayer = homeC.FirstName + " " + homeC.Surname;
                                break;
                        }
                        commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 0, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                    }
            } else {
                    if (position == 1) {
                        current = awayPG;

                        // Get the player from the depth chart
                        var dc = _awayDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 1);
                        var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            awayPG = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awayPGRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayPG.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;

                            subbed = 1;
                        }
                    } else if (position == 2) {
                        current = awaySG;

                        // Get the player from the depth chart
                        var dc = _awayDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 2);
                        var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            awaySG = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awaySGRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awaySG.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;

                            subbed = 1;
                        }
                    } else if (position == 3) {
                        current = awaySF;

                        // Get the player from the depth chart
                        var dc = _awayDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 3);
                        var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            awaySF = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awaySFRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awaySF.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;

                            subbed = 1;
                        }
                    } else if (position == 4) {
                        current = awayPF;

                        // Get the player from the depth chart
                        var dc = _awayDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 4);
                        var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            awayPF = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awayPFRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayPF.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;

                            subbed = 1;
                        }
                    } else if (position == 5) {
                        current = awayC;

                        // Get the player from the depth chart
                        var dc = _awayDepth.FirstOrDefault(x => x.Depth == 1 && x.Position == 5);
                        var bs = _awayBoxScores.FirstOrDefault(x => x.Id == dc.PlayerId);

                        if (bs.Fouls < 6) {
                            awayC = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awayCRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == current.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayC.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;

                            subbed = 1;
                        }
                    }

                    if (subbed == 1) {
                        // Now need to sort out the sub commentary
                        string outPlayer = current.FirstName + " " + current.Surname;
                        string inPlayer = "";

                        switch (position)
                        {
                            case 1:
                                inPlayer = awayPG.FirstName + " " + awayPG.Surname;
                                break;
                            case 2:
                                inPlayer = awaySG.FirstName + " " + awaySG.Surname;
                                break;
                            case 3:
                                inPlayer = awaySF.FirstName + " " + awaySF.Surname;
                                break;
                            case 4:
                                inPlayer = awayPF.FirstName + " " + awayPF.Surname;
                                break;
                            case 5:
                                inPlayer = awayC.FirstName + " " + awayC.Surname;
                                break;
                        }
                        commentaryData.Add(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot));
                        PlayByPlayTracker(comm.GetSubCommentary(outPlayer, inPlayer, 1, _awayTeam.Mascot, _homeTeam.Mascot), 0);
                    }
                }
        }

        public void BackToStarters()
        {
            // Home Team
            for (int i=1; i<6; i++)
            {
                DepthChart dc = null;
                List<int> onCourtIds = null;
                bool exists;
                switch (i)
                {
                    case 1:
                        Player currentHomePG = homePG;
                        PlayerRating currentHomePGRatings = homePGRatings;
                        //int awayPFSet = 0;

                        dc = _homeDepth.Where(x => x.Position == 1 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            homePG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homePGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomePG.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePG.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;
                        }
                        break;
                    case 2:
                        Player currentHomeSG = homeSG;
                        PlayerRating currentHomeSGRatings = homeSGRatings;
                        //int awayPFSet = 0;

                        dc = _homeDepth.Where(x => x.Position == 2 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            homeSG = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homeSGRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomeSG.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSG.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;
                        }
                        break;
                    case 3:
                        Player currentHomeSF = homeSF;
                        PlayerRating currentHomeSFRatings = homeSFRatings;
                        //int awayPFSet = 0;

                        dc = _homeDepth.Where(x => x.Position == 3 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            homeSF = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homeSFRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomeSF.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeSF.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;
                        }
                        break;
                    case 4:
                        Player currentHomePF = homePF;
                        PlayerRating currentHomePFRatings = homePFRatings;
                        //int awayPFSet = 0;

                        dc = _homeDepth.Where(x => x.Position == 4 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            homePF = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homePFRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomePF.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homePF.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;
                        }
                        break;
                    case 5:
                        Player currentHomeC = homeC;
                        PlayerRating currentHomeCRatings = homeCRatings;
                        //int awayPFSet = 0;

                        dc = _homeDepth.Where(x => x.Position == 5 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            homeC = _homePlayers.Find(x => x.Id == dc.PlayerId);
                            homeCRatings = _homeRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _homeStaminas.Find(x => x.PlayerId == currentHomeC.Id);
                            stOff.OnOff = 0;
                            int index = _homeStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _homeStaminas[index] = stOff;

                            StaminaTrack stOn = _homeStaminas.Find(x => x.PlayerId == homeC.Id);
                            stOn.OnOff = 1;
                            index = _homeStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _homeStaminas[index] = stOn;
                        }
                        break;
                }
            }

            // Away Team
            for (int i = 1; i < 6; i++)
            {
                DepthChart dc = null;
                List<int> onCourtIds = null;
                bool exists;

                switch (i)
                {
                    case 1:
                        Player currentAwayPG = awayPG;
                        PlayerRating currentAwayPGRatings = awayPGRatings;
                        //int awayPFSet = 0;

                        dc = _awayDepth.Where(x => x.Position == 1 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            awayPG = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awayPGRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwayPG.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayPG.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;
                        }
                        break;
                    case 2:
                        Player currentAwaySG = awaySG;
                        PlayerRating currentAwaySGRatings = awaySGRatings;
                        //int awayPFSet = 0;

                        dc = _awayDepth.Where(x => x.Position == 2 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            awaySG = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awaySGRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwaySG.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awaySG.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;
                        }
                        break;
                    case 3:
                        Player currentAwaySF = awaySF;
                        PlayerRating currentAwaySFRatings = awaySFRatings;
                        //int awayPFSet = 0;

                        dc = _awayDepth.Where(x => x.Position == 3 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            awaySF = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awaySFRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwaySF.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awaySF.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;
                        }
                        break;
                    case 4:
                        Player currentAwayPF = awayPF;
                        PlayerRating currentAwayPFRatings = awayPFRatings;
                        //int awayPFSet = 0;

                        dc = _awayDepth.Where(x => x.Position == 4 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if(!exists)
                        {
                            awayPF = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awayPFRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwayPF.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayPF.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;
                            
                        }
                        break;
                    case 5:
                        Player currentAwayC = awayC;
                        PlayerRating currentAwayCRatings = awayCRatings;
                        //int awayPFSet = 0;

                        dc = _awayDepth.Where(x => x.Position == 5 && x.Depth == 1).First();
                        onCourtIds = new List<int>();
                        onCourtIds.Add(awayPG.Id);
                        onCourtIds.Add(awaySG.Id);
                        onCourtIds.Add(awaySF.Id);
                        onCourtIds.Add(awayPF.Id);
                        onCourtIds.Add(awayC.Id);

                        exists = onCourtIds.Contains(dc.PlayerId);

                        if (!exists)
                        {
                            awayC = _awayPlayers.Find(x => x.Id == dc.PlayerId);
                            awayCRatings = _awayRatings.Find(x => x.PlayerId == dc.PlayerId);
                            //awayPFSet = 1;

                            // Need to update the stamina track objects for on and off court
                            StaminaTrack stOff = _awayStaminas.Find(x => x.PlayerId == currentAwayC.Id);
                            stOff.OnOff = 0;
                            int index = _awayStaminas.FindIndex(x => x.PlayerId == stOff.PlayerId);
                            _awayStaminas[index] = stOff;

                            StaminaTrack stOn = _awayStaminas.Find(x => x.PlayerId == awayC.Id);
                            stOn.OnOff = 1;
                            index = _awayStaminas.FindIndex(x => x.PlayerId == stOn.PlayerId);
                            _awayStaminas[index] = stOn;
                        }
                        break;
                }
            }
        }

        public int EndGameDecisions()
        {
            int decision = 0;
            int diff = ScoreDiffCheck();

            if (diff == 0) {
                // returns to normal
                decision = GetPlayerDecision();
            } else {
                if (_teamPossession == 0 && diff < 0) {
                    // Home team is winning and has the ball
                } else if (_teamPossession == 1 && diff > 0) {
                    // Away Team is winning and has the ball
                } else {
                    // Whoever has the ball is losing
                    if (_teamPossession == 0 && diff > 0) {
                        // home has the ball and is losing
                        if (diff <= 2) {

                        }

                    } else if (_teamPossession == 1 && diff < 0) {
                        // away has the ball and is losing
                        if (diff <= -2) {
                            
                        }
                    }
                }
            }

            return decision;
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
                    decimal percantageToApply = (decimal) time / 60;
                    decimal staminaUpdateAmount = current.StaminaRating * percantageToApply;
                    // Update the players stamina rating
                    decimal currentStamina = st.StaminaValue;
                    st.StaminaValue = currentStamina + staminaUpdateAmount;

                    // Add the StaminaTrack record back into the awayStaminas list
                    int index = _awayStaminas.FindIndex(x => x.PlayerId == st.PlayerId);
                    _awayStaminas[index] = st;
                } else {
                    // player is off the court
                    // Do this check as if the player has not stamina effect that can not receive any more
                    if (st.StaminaValue > 0)
                    {
                        // Get the players stamina rating
                        PlayerRating current = _awayRatings.Find(x => x.PlayerId == st.PlayerId);

                        // Determine how much of the rating gets applied for the time passed
                        decimal percantageToApply = (decimal) time / 120; // making this have half the effect of playing
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
                        decimal percantageToApply = (decimal)time / 120; // making this have half the effect of playing
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
            if(team == 0)
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
            } else
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
                value = (decimal) 2.5;
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

        public Player UpdateFouler(int playerPos)
        {
            Player playerFouling = null;
            switch (playerPos)
            {
                case 1:
                    if (_teamPossession == 1)
                    {
                        // Home PG fouled - update the box score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homePG.Id);
                        temp.Fouls++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homePG.Id);
                        _homeBoxScores[index] = temp;
                        playerFouling = homePG;
                    }
                    else
                    {
                        // Away PG
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayPG.Id);
                        temp.Fouls++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayPG.Id);
                        _awayBoxScores[index] = temp;
                        playerFouling = awayPG;
                    }
                    break;
                case 2:
                    if (_teamPossession == 1)
                    {
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeSG.Id);
                        temp.Fouls++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeSG.Id);
                        _homeBoxScores[index] = temp;
                        playerFouling = homeSG;
                    }
                    else
                    {
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awaySG.Id);
                        temp.Fouls++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awaySG.Id);
                        _awayBoxScores[index] = temp;
                        playerFouling = awaySG;
                    }
                    break;
                case 3:
                    if (_teamPossession == 1)
                    {
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeSF.Id);
                        temp.Fouls++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeSF.Id);
                        _homeBoxScores[index] = temp;
                        playerFouling = homeSF;
                    }
                    else
                    {
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awaySF.Id);
                        temp.Fouls++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awaySF.Id);
                        _awayBoxScores[index] = temp;
                        playerFouling = awaySF;
                    }
                    break;
                case 4:
                    if (_teamPossession == 1)
                    {
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homePF.Id);
                        temp.Fouls++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homePF.Id);
                        _homeBoxScores[index] = temp;
                        playerFouling = homePF;
                    }
                    else
                    {
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayPF.Id);
                        temp.Fouls++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayPF.Id);
                        _awayBoxScores[index] = temp;
                        playerFouling = awayPF;
                    }
                    break;
                case 5:
                    if (_teamPossession == 1)
                    {
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeC.Id);
                        temp.Fouls++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
                        _homeBoxScores[index] = temp;
                        playerFouling = homeC;
                    }
                    else
                    {
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayC.Id);
                        temp.Fouls++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
                        _awayBoxScores[index] = temp;
                        playerFouling = awayC;
                    }
                    break;
                default:
                    break;
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
            if (team == 0) {
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
            } else {
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
            } else if (awaySG.Id == playerId)
            {
                return 2;
            } else if (awaySF.Id == playerId)
            {
                return 3;
            } else if (awayPF.Id == playerId)
            {
                return 4;
            } else if (awayC.Id == playerId)
            {
                return 5;
            } else if (homePG.Id == playerId)
            {
                return 1;
            } else if (homeSG.Id == playerId)
            {
                return 2;
            } else if (homeSF.Id == playerId)
            {
                return 3;
            } else if (homePF.Id == playerId)
            {
                return 4;
            } else if (homeC.Id == playerId)
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

            if (endOfPlay == 1) {
                _playNumber++;
            }
        }
    }
}