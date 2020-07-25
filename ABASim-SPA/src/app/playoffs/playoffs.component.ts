import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { League } from '../_models/league';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-playoffs',
  templateUrl: './playoffs.component.html',
  styleUrls: ['./playoffs.component.css']
})
export class PlayoffsComponent implements OnInit {
  league: League;
  isAdmin = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    });
  }

  goToStats() {
    this.router.navigate(['/playoffs-stats']);
  }

}
