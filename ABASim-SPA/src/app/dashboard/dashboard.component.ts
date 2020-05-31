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

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

    // get the league object - TODO - roll the league state into the object as a Dto and pass back
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.getTodaysEvents();
      this.getUpcomingEvents();
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your Team');
    });


  }

  getTodaysEvents() {
    if (this.league.stateId === 6 && this.league.day !== 0) {
      this.leagueService.getPreseasonGamesForToday().subscribe(result => {
        this.todaysGames = result;
        console.log(this.todaysGames);
      }, error => {
        this.alertify.error('Error getting todays events');
      });
    }
  }

  getUpcomingEvents() {
    // Preseason
    if (this.league.stateId === 6) {
      // Need to get the games for the day
      this.leagueService.getPreseasonGamesForTomorrow().subscribe(result => {
        console.log(result);
        this.upcomingGames = result;
        console.log(this.upcomingGames);
      }, error => {
        this.alertify.error('Error getting upcoming games');
      });
    }
  }

  goToRoster() {
    this.router.navigate(['/roster']);
  }

  goToDraft() {
    this.router.navigate(['/draft']);
  }

  goToAdmin() {
    this.router.navigate(['/admin']);
  }

  goToLeague() {

  }

  goToPlayers() {
    
  }

  runGame() {
    
  }

  watchGame() {

  }

}
