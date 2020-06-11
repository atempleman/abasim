import { Component, OnInit } from '@angular/core';
import { League } from '../_models/league';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css']
})
export class ScheduleComponent implements OnInit {
  league: League;
  isAdmin = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      // Now need to get the schedule

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

}
