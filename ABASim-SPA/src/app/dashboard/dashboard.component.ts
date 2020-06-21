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

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  league: League;
  team: Team;
  isAdmin = 0;
  upcomingGames: GameDisplay[] = [];
  todaysGames: GameDisplayCurrent[] = [];
  noRun = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService, private adminService: AdminService,
              private gameEngine: GameEngineService, private transferService: TransferService, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();
    localStorage.setItem('isAdmin', this.isAdmin.toString());

    // get the league object - TODO - roll the league state into the object as a Dto and pass back
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.spinner.show();
      this.getTodaysEvents();
      this.getUpcomingEvents();
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
      // Need to persist the team to cookie
      localStorage.setItem('teamId', this.team.id.toString());
    }, error => {
      this.alertify.error('Error getting your Team');
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
      awayId:  game.awayTeamId,
      homeId:  game.homeTeamId,
      gameId:  game.id,
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

  runGameSeason(game: GameDisplayCurrent) {
    this.noRun = 1;
    const simGame: SimGame = {
      awayId:  game.awayTeamId,
      homeId:  game.homeTeamId,
      gameId:  game.id,
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

  watchGame(gameId: number) {
    console.log(gameId);
    this.transferService.setData(gameId);
    this.router.navigate(['/watch-game']);
  }

  viewBoxScore(gameId: number) {
    this.transferService.setData(gameId);
    this.router.navigate(['/box-score']);
  }

}
