import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { League } from '../_models/league';
import { LeagueState } from '../_models/leagueState';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { GameDisplay } from '../_models/gameDisplay';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { AdminService } from '../_services/admin.service';
import { SimGame } from '../_models/simGame';
import { GameEngineService } from '../_services/game-engine.service';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { LeagueLeadersPoints } from '../_models/leagueLeadersPoints';
import { LeagueLeadersRebounds } from '../_models/leagueLeadersRebounds';
import { LeagueLeadersAssists } from '../_models/leagueLeadersAssists';
import { LeagueLeadersSteals } from '../_models/leagueLeadersSteals';
import { LeagueLeadersBlocks } from '../_models/leagueLeadersBlocks';
import { PlayoffSummary } from '../_models/playoffSummary';
import { GlobalChat } from '../_models/globalChat';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { User } from '../_models/user';
// import { DatePipe } from '@angular/common';
import { formatDate } from '@angular/common';
import { ContactService } from '../_services/contact.service';
import { DraftTracker } from '../_models/draftTracker';
import { DraftService } from '../_services/draft.service';
import { DashboardDraftPick } from '../_models/dashboardDraftPick';
import { Transaction } from '../_models/transaction';
import { Votes } from '../_models/votes';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  league: League;
  team: Team;
  champion: Team;
  isAdmin = 0;
  upcomingGames: GameDisplay[] = [];
  todaysGames: GameDisplayCurrent[] = [];
  playoffSummaries: PlayoffSummary[] = [];
  noRun = 0;

  topFivePoints: LeagueLeadersPoints[] = [];
  topFiveRebounds: LeagueLeadersRebounds[] = [];
  topFiveAssists: LeagueLeadersAssists[] = [];
  topFiveSteals: LeagueLeadersSteals[] = [];
  topFiveBlocks: LeagueLeadersBlocks[] = [];

  chatRecords: GlobalChat[] = [];
  chatForm: FormGroup;
  user: User;
  interval;
  draftInterval;

  tracker: DraftTracker;
  currentPick: DashboardDraftPick;
  previousPick: DashboardDraftPick;
  nextPick: DashboardDraftPick;

  yesterdaysTransactions: Transaction[] = [];

  mvp: Votes[] = [];
  sixth: Votes[] = [];
  dpoy: Votes[] = [];

  primaryColor: string = '22, 24, 100';
  secondaryColor: string = '12,126,120';

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
    private authService: AuthService, private teamService: TeamService, private adminService: AdminService,
    private gameEngine: GameEngineService, private transferService: TransferService, private spinner: NgxSpinnerService,
    private fb: FormBuilder, private contactService: ContactService, private draftService: DraftService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();
    localStorage.setItem('isAdmin', this.isAdmin.toString());

    this.createChatForm();

    this.refreshChat();
    this.interval = setInterval(() => {
      this.refreshChat();
    }, 600000);

    // get the league object - TODO - roll the league state into the object as a Dto and pass back
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.spinner.show();
      if (this.league.stateId === 4) {
        this.getDraftTracker();
      } else if (this.league.stateId === 7) {
        this.getLeagueLeaders();
      } else if (this.league.stateId === 8) {
        // get the playoff series
        this.getRoundOneSummaries();
      } else if (this.league.stateId === 9) {
        // get playoff series
        this.getConfSemiSummaries();
      } else if (this.league.stateId === 10) {
        this.getConfFinalsSummaries();
      } else if (this.league.stateId === 11) {
        this.getFinalsSummaries();
      }

      if (this.league.stateId === 3 || this.league.stateId === 4) {
        this.interval = setInterval(() => {
          this.getPicksToDisplay();
        }, 210000);
      }

      this.getTodaysEvents();
      this.getYesterdaysTransactions();

      if (this.league.stateId === 11 && this.league.day > 28) {
        this.leagueService.getChampion().subscribe(result => {
          this.champion = result;
        }, error => {
          this.alertify.error('Error getting the champion');
        }, () => {
          this.spinner.hide();
        });
      } else {
        this.spinner.hide();
      }

      if (this.league.stateId > 7) {
        this.leagueService.getMvpTopFive().subscribe(result => {
          this.mvp = result;
        }, error => {
          this.alertify.error('Error getting MVP');
        });

        this.leagueService.getSixthManTopFive().subscribe(result => {
          this.sixth = result;
        }, error => {
          this.alertify.error('Error getting Sixth Man');
        });

        this.leagueService.getDpoyTopFive().subscribe(result => {
          this.dpoy = result;
        }, error => {
          this.alertify.error('Error getting DPOY');
        });
      }
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
      // Need to persist the team to cookie
      localStorage.setItem('teamId', this.team.id.toString());
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.backgroundStyle();
    });
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

  getRoundOneSummaries() {
    this.leagueService.getFirstRoundSummaries(1).subscribe(result => {
      this.playoffSummaries = result;
      console.log(this.playoffSummaries);
    }, error => {
      this.alertify.error('Error getting playoff summaries');
    });
  }

  getConfSemiSummaries() {
    this.leagueService.getFirstRoundSummaries(2).subscribe(result => {
      this.playoffSummaries = result;
    }, error => {
      this.alertify.error('Error getting playoff summaries');
    });
  }

  getConfFinalsSummaries() {
    this.leagueService.getFirstRoundSummaries(3).subscribe(result => {
      this.playoffSummaries = result;
    }, error => {
      this.alertify.error('Error getting playoff summaries');
    });
  }

  getFinalsSummaries() {
    this.leagueService.getFirstRoundSummaries(4).subscribe(result => {
      this.playoffSummaries = result;
    }, error => {
      this.alertify.error('Error getting playoff summaries');
    });
  }

  getLeagueLeaders() {
    this.leagueService.getTopFivePoints().subscribe(result => {
      this.topFivePoints = result;
    }, error => {
      this.alertify.error('Error getting points leaders');
    });

    this.leagueService.getTopFiveAssists().subscribe(result => {
      this.topFiveAssists = result;
    }, error => {
      this.alertify.error('Error getting assists leaders');
    });

    this.leagueService.getTopFiveBlocks().subscribe(result => {
      this.topFiveBlocks = result;
    }, error => {
      this.alertify.error('Error getting blocks leaders');
    });

    this.leagueService.getTopFiveRebounds().subscribe(result => {
      this.topFiveRebounds = result;
    }, error => {
      this.alertify.error('Error getting rebounds leaders');
    });

    this.leagueService.getTopFiveSteals().subscribe(result => {
      this.topFiveSteals = result;
    }, error => {
      this.alertify.error('Error getting steals leaders');
    });
  }

  getTodaysEvents() {
    if (this.league.stateId === 6 && this.league.day !== 0) {
      this.leagueService.getPreseasonGamesForToday().subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error getting todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 7 && this.league.day !== 0) {
      this.leagueService.getSeasonGamesForToday().subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error getting todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 8 && this.league.day !== 0) {
      this.leagueService.getFirstRoundGamesForToday().subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error gettings todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 9 && this.league.day !== 0) {
      this.leagueService.getFirstRoundGamesForToday().subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error gettings todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 10 && this.league.day !== 0) {
      this.leagueService.getFirstRoundGamesForToday().subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error gettings todays events');
      }, () => {
        this.spinner.hide();
      });
    } else if (this.league.stateId === 11 && this.league.day !== 0) {
      this.leagueService.getFirstRoundGamesForToday().subscribe(result => {
        this.todaysGames = result;
      }, error => {
        this.alertify.error('Error gettings todays events');
      }, () => {
        this.spinner.hide();
      });
    }
  }

  getUpcomingEvents() {
    // Preseason
    if (this.league.stateId === 6) {
      // Need to get the games for the day
      this.leagueService.getPreseasonGamesForTomorrow().subscribe(result => {
        this.upcomingGames = result;
      }, error => {
        this.alertify.error('Error getting upcoming games');
      });
    } else if (this.league.stateId === 7) {
      this.leagueService.getSeasonGamesForTomorrow().subscribe(result => {
        this.upcomingGames = result;
      }, error => {
        this.alertify.error('Error getting upcoming games');
      });
    }
  }

  goToRoster() {
    this.router.navigate(['/team']);
  }

  goToDraft() {
    this.router.navigate(['/draft']);
  }

  goToAdmin() {
    this.router.navigate(['/admin']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToPlayers() {
    this.router.navigate(['/players']);
  }

  runGame(game: GameDisplayCurrent) {
    this.noRun = 1;
    const simGame: SimGame = {
      awayId: game.awayTeamId,
      homeId: game.homeTeamId,
      gameId: game.id,
    };

    this.gameEngine.startPreseasonGame(simGame).subscribe(result => {
    }, error => {
      this.alertify.error(error);
      this.noRun = 0;
    }, () => {
      // Need to pass feedback and re-get the days games
      this.alertify.success('Game run successfully');
      this.noRun = 0;
      this.getTodaysEvents();
    });
  }

  runGamePlayoffs(game: GameDisplayCurrent) {
    console.log('ashley testing here');
    this.noRun = 1;
    const simGame: SimGame = {
      awayId: game.awayTeamId,
      homeId: game.homeTeamId,
      gameId: game.id,
    };

    console.log(simGame);

    this.gameEngine.startPlayoffGame(simGame).subscribe(result => {
    }, error => {
      this.alertify.error(error);
      this.noRun = 0;
    }, () => {
      // Need to pass feedback and re-get the days games
      this.alertify.success('Game run successfully');
      this.noRun = 0;
      this.getTodaysEvents();
    });
  }

  runGameSeason(game: GameDisplayCurrent) {
    this.noRun = 1;
    const simGame: SimGame = {
      awayId: game.awayTeamId,
      homeId: game.homeTeamId,
      gameId: game.id,
    };

    this.gameEngine.startSeasonGame(simGame).subscribe(result => {
    }, error => {
      this.alertify.error(error);
      this.noRun = 0;
    }, () => {
      // Need to pass feedback and re-get the days games
      this.alertify.success('Game run successfully');
      this.noRun = 0;
      this.getTodaysEvents();
    });
  }

  watchGame(gameId: number, stateId: number) {
    this.transferService.setData(gameId);
    this.transferService.setState(stateId);
    this.router.navigate(['/watch-game']);
  }

  fullGame(gameId: number, stateId: number) {
    this.transferService.setData(gameId);
    this.transferService.setState(stateId);
    this.router.navigate(['/full-game-comm']);
  }

  viewBoxScore(gameId: number) {
    this.transferService.setData(gameId);
    this.router.navigate(['/box-score']);
  }

  goToStats(value: number) {
    this.transferService.setData(value);
    this.router.navigate(['/stats']);
  }

  goToPlayoffs() {
    this.router.navigate(['/playoffs']);
  }

  refreshChat() {
    this.contactService.getChatRecords().subscribe(result => {
      this.chatRecords = result;
    }, error => {
      this.alertify.error('Error getting chat messages');
    });
  }

  sendChat() {
    if (this.chatForm.valid) {
      const result = this.chatForm.controls['message'].value;
      // const myDate = new Date();
      // const dt = this.datePipe.transform(myDate, 'dd-MM-yyyy');
      const dt = formatDate(new Date(), 'dd/MM/yyyy', 'en');
      const chatRecord: GlobalChat = {
        chatText: result,
        username: this.authService.decodedToken.nameid,
        chatTime: dt.toString()
      };

      this.contactService.sendChat(chatRecord).subscribe(rst => {
      }, error => {
        this.alertify.error('Error sending message');
      }, () => {
        this.alertify.success('Message posted');
        this.chatForm.reset();
        // Need to get the chat messages again
        this.contactService.getChatRecords().subscribe(r => {
          this.chatRecords = r;
        }, error => {
          this.alertify.error('Error getting chat messages');
        });
      });
    } else {
      this.alertify.error('Please populate your chat message');
    }
  }

  createChatForm() {
    this.chatForm = this.fb.group({
      message: ['', Validators.required]
    });
  }

  getDraftTracker() {
    this.draftService.getDraftTracker().subscribe(result => {
      this.tracker = result;
    }, error => {
      this.alertify.error('Error getting draft tracker');
    }, () => {
      // Now need to get the Previous, Current and Next picks
      this.getPicksToDisplay();
    });
  }

  getPicksToDisplay() {
    // Get the previous pick
    this.draftService.getDashboardPicks(-1).subscribe(result => {
      this.previousPick = result;
    }, error => {
      this.alertify.error('Error getting last pick');
    });

    // Get the Current pick
    this.draftService.getDashboardPicks(0).subscribe(result => {
      this.currentPick = result;
    }, error => {
      this.alertify.error('Error getting current pick');
    });

    // Get Next Pick
    this.draftService.getDashboardPicks(1).subscribe(result => {
      this.nextPick = result;
    }, error => {
      this.alertify.error('Error getting next pick');
    });
  }

  viewTeam(teamMascot: string) {
    let team: Team;
    // Need to go a call to get the team id
    this.teamService.getTeamForTeamMascot(teamMascot).subscribe(result => {
      team = result;
    }, error => {
      this.alertify.error('Error getting players team');
    }, () => {
      this.transferService.setData(team.id);
      this.router.navigate(['/view-team']);
    });
  }

  getYesterdaysTransactions() {
    this.leagueService.getYesterdaysTransactins().subscribe(result => {
      this.yesterdaysTransactions = result;
    }, error => {
      this.alertify.error('Error getting transactions');
    });
  }

  backgroundStyle() {
    switch (this.team.id) {
      case 2:
        // Toronto
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 3:
        // Milwaukee
        this.primaryColor = '0,71,27';
        this.secondaryColor = '240,235,210';
        break;
      case 4:
        // Miami
        this.primaryColor = '152,0,46';
        this.secondaryColor = '249,160,27';
        break;
      case 5:
        // Denver
        this.primaryColor = '13,34,64';
        this.secondaryColor = '255,198,39';
        break;
      case 6:
        // Lakers
        this.primaryColor = '85,37,130';
        this.secondaryColor = '253,185,39';
        break;
      case 7:
        // Rockets
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 8:
        // Boston
        this.primaryColor = '0, 122, 51';
        this.secondaryColor = '139,111,78';
        break;
      case 9:
        // Indiana
        this.primaryColor = '0,45,98';
        this.secondaryColor = '253,187,48';
        break;
      case 10:
        // Orlando
        this.primaryColor = '0,125,197';
        this.secondaryColor = '196,206,211';
        break;
      case 11:
        // OKC
        this.primaryColor = '0,125,195';
        this.secondaryColor = '239,59,36';
        break;
      case 12:
        // Clippers
        this.primaryColor = '200,16,46';
        this.secondaryColor = '29,66,148';
        break;
      case 13:
        // Dallas
        this.primaryColor = '0,83,188';
        this.secondaryColor = '0,43,92';
        break;
      case 14:
        // 76ers
        this.primaryColor = '0,107,182';
        this.secondaryColor = '237,23,76';
        break;
      case 15:
        // Chicago
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 16:
        // Charlotte
        this.primaryColor = '29,17,96';
        this.secondaryColor = '0,120,140';
        break;
      case 17:
        // Utah
        this.primaryColor = '0,43,92';
        this.secondaryColor = '0,71,27';
        break;
      case 18:
        // Phoenix
        this.primaryColor = '29,17,96';
        this.secondaryColor = '229,95,32';
        break;
      case 19:
        // Memphis
        this.primaryColor = '93,118,169';
        this.secondaryColor = '18,23,63';
        break;
      case 20:
        // Brooklyn
        this.primaryColor = '0,0,0';
        this.secondaryColor = '119,125,132';
        break;
      case 21:
        // Detroit
        this.primaryColor = '200,16,46';
        this.secondaryColor = '29,66,138';
        break;
      case 22:
        // Washington
        this.primaryColor = '0,43,92';
        this.secondaryColor = '227,24,55';
        break;
      case 23:
        // Portland
        this.primaryColor = '224,58,62';
        this.secondaryColor = '6,25,34';
        break;
      case 24:
        // Sacromento
        this.primaryColor = '91,43,130';
        this.secondaryColor = '99,113,122';
        break;
      case 25:
        // Spurs
        this.primaryColor = '196,206,211';
        this.secondaryColor = '6,25,34';
        break;
      case 26:
        // Knicks
        this.primaryColor = '0,107,182';
        this.secondaryColor = '245,132,38';
        break;
      case 27:
        // Cavs
        this.primaryColor = '134,0,56';
        this.secondaryColor = '4,30,66';
        break;
      case 28:
        // Atlanta
        this.primaryColor = '225,68,52';
        this.secondaryColor = '196,214,0';
        break;
      case 29:
        // Minnesota
        this.primaryColor = '12,35,64';
        this.secondaryColor = '35,97,146';
        break;
      case 30:
        // GSW
        this.primaryColor = '29,66,138';
        this.secondaryColor = '255,199,44';
        break;
      case 32:
        // New Orleans
        this.primaryColor = '0,22,65';
        this.secondaryColor = '225,58,62';
        break;
      default:
        this.primaryColor = '22, 24, 100';
        this.secondaryColor = '12, 126, 120';
        break;
    }
  }

}
