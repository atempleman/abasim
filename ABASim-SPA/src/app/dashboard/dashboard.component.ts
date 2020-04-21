import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { League } from '../_models/league';
import { LeagueState } from '../_models/leagueState';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  league: League;
  currentState: LeagueState;
  isAdmin = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
     this.getCurrentLeagueState();
    });

    this.isAdmin = this.authService.isAdmin();
  }

  getCurrentLeagueState() {
    this.leagueService.getLeagueStatusForId(this.league.stateId).subscribe(result => {
      this.currentState = result;
    }, error => {
      console.log('Error getting current league state');
    });
  }

  goToRoster() {
    this.router.navigate(['/roster']);
  }

  goToPlayers() {
    this.router.navigate(['/players']);
  }

  goToDraft() {
    this.router.navigate(['/draft']);
  }

  goToStats() {
    this.router.navigate(['/stats']);
  }

  goToStandings() {
    this.router.navigate(['/standings']);
  }

  goToScheduleAndReuslts() {
    this.router.navigate(['/scheduleandresults']);
  }

  goToFinances() {
    this.router.navigate(['/finances']);
  }

  goToAdmin() {
    this.router.navigate(['/admin']);
  }

}
