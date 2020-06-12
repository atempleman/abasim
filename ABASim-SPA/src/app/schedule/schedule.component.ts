import { Component, OnInit } from '@angular/core';
import { League } from '../_models/league';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Schedule } from '../_models/schedule';

@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css']
})
export class ScheduleComponent implements OnInit {
  league: League;
  isAdmin = 0;
  schedules: Schedule[] = [];
  gameDayViewing = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      // Now need to get the schedule
      // console.log(localStorage.getItem('teamId'));
      this.getScheduleForDay(this.league.day);
    });
  }

  getScheduleForDay(day: number) {
    this.gameDayViewing = day;

    this.leagueService.getScheduleGames(this.gameDayViewing).subscribe(result => {
      this.schedules = result;
    }, error => {
      this.alertify.error('Error getting schedule games');
    });
  }

  viewBoxScore(game: Schedule) {
    if (game.awayScore === 0 || game.homeScore === 0) {
      this.alertify.error('Game has not been played yet');
    } else {
      console.log('calling box score - TODO');
    }
  }

  getDaysViewing() {
    let startNumber = 0;
    let endNumber = 0;
    if (this.gameDayViewing - 2 < 0) {
      startNumber = 1;
    } else {
      startNumber = this.gameDayViewing - 2;
    }

    if (this.gameDayViewing + 2 > 150) {
      endNumber = 150;
    } else {
      endNumber = this.gameDayViewing + 2;
    }
    return 'Days ' + (startNumber.toString() + ' to ' + endNumber.toString());
  }

  getNextDays() {
    if (this.gameDayViewing - 3 > 150) {
      this.gameDayViewing = 150;
    } else {
      this.gameDayViewing = this.gameDayViewing + 3;
    }

    this.leagueService.getScheduleGames(this.gameDayViewing).subscribe(result => {
      this.schedules = result;
      console.log(this.schedules);
    }, error => {
      this.alertify.error('Error getting schedule games');
    });
  }

  getPrevDays() {
    if (this.gameDayViewing - 3 < 1) {
      this.gameDayViewing = 1;
    } else {
      this.gameDayViewing = this.gameDayViewing - 3;
    }

    this.leagueService.getScheduleGames(this.gameDayViewing).subscribe(result => {
      this.schedules = result;
      console.log(this.schedules);
    }, error => {
      this.alertify.error('Error getting schedule games');
    });
  }

  goToStandings() {
    this.router.navigate(['/standings']);
  }

  goToStats() {
    this.router.navigate(['/stats']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToTransactions() {
    this.router.navigate(['/transactions']);
  }

}
