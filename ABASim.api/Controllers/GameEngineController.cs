using System;
using System.Collections.Generic;
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

        public GameEngineController(IGameEngineRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("startGame")]
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

            commentaryData.Add(comm.GetGameIntroCommentry(_awayTeam, _homeTeam));
            commentaryData.Add(comm.GetStartingLineupsCommentary(awayPG, awaySG, awaySF, awayPF, awayC));
            commentaryData.Add(comm.GetStartingLineupsCommentary(homePG, homeSG, homeSF, homePF, homeC));
            commentaryData.Add("It's now time for the opening tip");
            Console.WriteLine(comm.GetGameIntroCommentry(_awayTeam, _homeTeam));
            Console.WriteLine(comm.GetStartingLineupsCommentary(awayPG, awaySG, awaySF, awayPF, awayC));
            Console.WriteLine(comm.GetStartingLineupsCommentary(homePG, homeSG, homeSF, homePF, homeC));
            Console.WriteLine("It's now time for the opening tip");
            
            Jumpball();
            _initialPossession = _teamPossession;

            // 1st Quarter
            RunQuarter();

            // 2nd Quarter
            RunQuarter();

            // 3rd Quarter
            RunQuarter();

            // 4th Quarter
            RunQuarter();

            // Overtime - TODO

            return Ok(commentaryData);
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
            Console.WriteLine(comm.GetJumpballCommentary(winningTeam, _quarter, _time, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
        }

        public void RunQuarter()
        {
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
                            break;
                        case 5:
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

            // Now need to get the current players tendancies
            PlayerTendancy tendancy = GetCurrentPlayersTendancies();
            
            // This could change if there are bonuses which will be added later on
            int value = 1000;
            int result = _random.Next(1, value + 1);

            int twoSection = tendancy.TwoPointTendancy;
            int threeSection = twoSection + tendancy.ThreePointTendancy;
            int foulSection = threeSection + tendancy.FouledTendancy;
            int turnoverSection = foulSection + tendancy.TurnoverTendancy;

            if (result <= twoSection)
            {
                decision = 2;
            } 
            else if (result > twoSection && result <= threeSection)
            {
                decision = 3;
            } 
            else if (result > threeSection && result <= foulSection)
            {
                decision = 4;
            }
            else if (result > foulSection && result <= turnoverSection)
            {
                decision = 5;
            } 
            else 
            {
                // The player has passed the ball
                decision = 1; 
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

                    // UPDATE THE TIME IN THE BOX SCORE OBJECTS
                    UpdateTimeInBoxScores(timeValue);

                    // UPDATE THE STAMINA
                    StaminaUpdates(timeValue);

                    // // COMMENTARY
                    // commentaryData.Add(comm.EndOfQuarterCommtary(_time, _quarter));
                    // Console.WriteLine(comm.EndOfQuarterCommtary(_time, _quarter));

                    // // Call End of Quarter - TODO
                }
                else if (timeValue > _shotClock)
                {
                    timeValue = _shotClock;
                    _shotClock = _shotClock - timeValue;
                    _time = _time - timeValue;

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

                // UPDATE THE TIME IN THE BOX SCORE OBJECTS
                UpdateTimeInBoxScores(timeValue);

                // UPDATE THE STAMINA
                StaminaUpdates(timeValue);

                // COMMENTARY
                commentaryData.Add(comm.GetPassCommentary(passer, receiver, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                Console.WriteLine(comm.GetPassCommentary(passer, receiver, _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            }
        }

        public void TwoPointShot()
        {
            PlayerRating currentRating = GetCurrentPlayersRatings();
            int result = _random.Next(1, 1001);

            int twoRating = currentRating.TwoRating;

            if (twoRating >= result)
            {
                // Show was made
                 int timeValue = _random.Next(2, 7);

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
                }

                // Comm
                commentaryData.Add(comm.GetTwoPointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                Console.WriteLine(comm.GetTwoPointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

                if (_teamPossession == 0)
                {
                    _teamPossession = 1;
                } else {
                    _teamPossession = 0;
                }

                Inbounds();
            } else {
                // Shot missed
                int timeValue = _random.Next(2, 7);

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
                Console.WriteLine(comm.GetTwoPointMissCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

                // Go to the Rebound
                Rebound();
            }
        }

        public void ThreePointShot()
        {
            PlayerRating currentRating = GetCurrentPlayersRatings();
            int result = _random.Next(1, 1001);

            int twoRating = currentRating.ThreeRating;

            if (twoRating >= result)
            {
                // Show was made
                 int timeValue = _random.Next(2, 7);

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
                }

                // Comm
                commentaryData.Add(comm.GetThreePointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                Console.WriteLine(comm.GetThreePointMakeCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

                if (_teamPossession == 0)
                {
                    _teamPossession = 1;
                } else {
                    _teamPossession = 0;
                }

                Inbounds();
            } else {
                // Shot missed
                int timeValue = _random.Next(2, 7);

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
                Console.WriteLine(comm.GetThreePointMissCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

                // Go to the Rebound
                Rebound();
            }
        }

        public void Rebound()
        {
            int offensiveRate = 0;
            int defensiveRate = 0;

            if (_teamPossession == 0)
            {
                int homePGRebound = homePGRatings.ORebRating;
                int homeSGRebound = homeSGRatings.ORebRating;
                int homeSFRebound = homeSFRatings.ORebRating;
                int homePFRebound = homePFRatings.ORebRating;
                int homeCRebound = homeCRatings.ORebRating;

                int awayPGRebound =awayPGRatings.DRebRating;
                int awaySGRebound = awaySGRatings.DRebRating;
                int awaySFRebound = awaySFRatings.DRebRating;
                int awayPFRebound = awayPFRatings.DRebRating;
                int awayCRebound = awayCRatings.DRebRating;

                offensiveRate = homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound;
                defensiveRate = awayPGRebound + awaySGRebound + awaySFRebound + awaySFRebound + awayCRebound;

                // home team shot
                int randValue = offensiveRate + defensiveRate;
                int result = _random.Next(1, randValue + 1);

                // Firstly determine if it is offensive or defensive
                if (result < offensiveRate)
                {
                    // Offensive Rebound
                    if (result < homePGRebound)
                    {
                        _playerPossession = 1;
                        _shotClock = 14;

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
                        _shotClock = 14;

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
                        _shotClock = 14;

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
                        _shotClock = 14;
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
                        _shotClock = 14;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeC.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
                        _homeBoxScores[index] = temp;
                    }

                    // Commentary for Offensive Rebound
                    commentaryData.Add(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    Console.WriteLine(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                }
                else
                {
                    // Defensive Rebound
                    if (result < (offensiveRate + awayPGRebound))
                    {
                        _teamPossession = 1;
                        _playerPossession = 1;
                        _shotClock = 24;

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
                        _shotClock = 24;

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
                        _shotClock = 24;

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
                        _shotClock = 24;

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
                        _shotClock = 24;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayC.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
                        _awayBoxScores[index] = temp;
                    }

                    // Display Defensive Rebound Commentary
                    commentaryData.Add(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    Console.WriteLine(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                }
            } else {
                int awayPGRebound = awayPGRatings.ORebRating;
                int awaySGRebound = awaySGRatings.ORebRating;
                int awaySFRebound = awaySFRatings.ORebRating;
                int awayPFRebound = awayPFRatings.ORebRating;
                int awayCRebound = awayCRatings.ORebRating;

                int homePGRebound = homePGRatings.DRebRating;
                int homeSGRebound = homeSGRatings.DRebRating;
                int homeSFRebound = homeSFRatings.DRebRating;
                int homePFRebound = homePFRatings.DRebRating;
                int homeCRebound = homeCRatings.DRebRating;

                defensiveRate = homePGRebound + homeSGRebound + homeSFRebound + homePFRebound + homeCRebound;
                offensiveRate = awayPGRebound + awaySGRebound + awaySFRebound + awaySFRebound + awayCRebound;

                // home team shot
                int randValue = offensiveRate + defensiveRate;
                int result = _random.Next(1, randValue + 1);

                // Firstly determine if it is offensive or defensive
                if (result < offensiveRate)
                {
                    // Offensive Rebound
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
                        _shotClock = 14;

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
                        _shotClock = 14;

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
                        _shotClock = 14;
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
                        _shotClock = 14;

                        // Update the Box Score
                        BoxScore temp = _awayBoxScores.Find(x => x.Id == awayC.Id);
                        temp.ORebs++;
                        temp.Rebounds++;
                        int index = _awayBoxScores.FindIndex(x => x.Id == awayC.Id);
                        _awayBoxScores[index] = temp;
                    }

                    // Commentary for Offensive Rebound
                    commentaryData.Add(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    Console.WriteLine(comm.GetOffensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                }
                else
                {
                    // Defensive Rebound
                    if (result < (offensiveRate + homePGRebound))
                    {
                        _teamPossession = 0;
                        _playerPossession = 1;
                        _shotClock = 24;

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
                        _shotClock = 24;

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
                        _shotClock = 24;

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
                        _shotClock = 24;

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
                        _shotClock = 24;

                        // Update the Box Score
                        BoxScore temp = _homeBoxScores.Find(x => x.Id == homeC.Id);
                        temp.DRebs++;
                        temp.Rebounds++;
                        int index = _homeBoxScores.FindIndex(x => x.Id == homeC.Id);
                        _homeBoxScores[index] = temp;
                    }

                    // Display Defensive Rebound Commentary
                    commentaryData.Add(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                    Console.WriteLine(comm.GetDefensiveReboundCommentary(GetCurrentPlayerFullName(), _time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
                }
            }
        }

        public void ShotClockViolation()
        {
            _shotClock = 24;
            
            if (_teamPossession == 0)
            {
                _teamPossession = 1;
            }
            else
            {
                _teamPossession = 0;
            }

            // Now it would go to check for SUBS - TODO

            // Need to add the commentary here
            commentaryData.Add(comm.GetShotClockTurnoverCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            Console.WriteLine(comm.GetShotClockTurnoverCommentary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

            // Inbounds the ball to continue on
            Inbounds();
        }

        public void EndOfQuarter()
        {
            commentaryData.Add(comm.EndOfQuarterCommtary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));
            Console.WriteLine(comm.EndOfQuarterCommtary(_time, _quarter, _awayScore, _homeScore, _teamPossession, _awayTeam.Mascot, _homeTeam.Mascot));

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
                        // Would be OVERTIME HERE - TODO
                    } else {
                        // End of Game commentary
                        commentaryData.Add(comm.EndGameCommentary(_awayTeam, _homeTeam, _awayScore, _homeScore));
                        Console.WriteLine(comm.EndGameCommentary(_awayTeam, _homeTeam, _awayScore, _homeScore));
                    }
                    break;
                default:
                    // This would be any other overtimes - TODO
                    break;
            }
            
        }

        public void Inbounds()
        {
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

        // STATS & TRACKING
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

        // HELPERS
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
                        current = homePGTendancy;
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
    
        
    }
}