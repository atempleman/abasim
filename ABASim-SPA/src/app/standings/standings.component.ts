import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { Standing } from '../_models/standing';
import { Router } from '@angular/router';

@Component({
  selector: 'app-standings',
  templateUrl: './standings.component.html',
  styleUrls: ['./standings.component.css']
})
export class StandingsComponent implements OnInit {
  statusConference = true;
  statusDivision = false;
  statusLeague = false;

  eastStandings: Standing[] = [];
  westStandings: Standing[] = [];

  allStandings: Standing[] = [];

  atlanticStandings: Standing[] = [];
  centralstandings: Standing[] = [];
  southeastStandings: Standing[] = [];
  northwestStandings: Standing[] = [];
  pacificStandings: Standing[] = [];
  southwestStandings: Standing[] = [];

  constructor(private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private router: Router) { }

  ngOnInit() {
    this.leagueService.getConferenceStandings(1).subscribe(result => {
      this.eastStandings = result;
    }, error => {
      this.alertify.success('Error getting eastern conference standings');
    });

    this.leagueService.getConferenceStandings(2).subscribe(result => {
      this.westStandings = result;
    }, error => {
      this.alertify.success('Error getting western conference standings');
    });
  }

  conferenceClick() {
    this.leagueService.getConferenceStandings(1).subscribe(result => {
      this.eastStandings = result;
    }, error => {
      this.alertify.error('Error getting eastern conference standings');
    });

    this.leagueService.getConferenceStandings(2).subscribe(result => {
      this.westStandings = result;
    }, error => {
      this.alertify.error('Error getting western conference standings');
    });

    this.statusLeague = false;
    this.statusDivision = false;
    this.statusConference = true;
  }

  divisionClick() {
    this.leagueService.getDivisionStandings(1).subscribe(result => {
      this.atlanticStandings = result;
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(2).subscribe(result => {
      this.centralstandings = result;
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(3).subscribe(result => {
      this.southeastStandings = result;
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(4).subscribe(result => {
      this.northwestStandings = result;
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(5).subscribe(result => {
      this.pacificStandings = result;
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(6).subscribe(result => {
      this.southwestStandings = result;
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.statusConference = false;
    this.statusLeague = false;
    this.statusDivision = true;
  }

  leagueClick() {
    this.leagueService.getLeagueStandings().subscribe(result => {
      this.allStandings = result;
    }, error => {
      this.alertify.error('Error getting league standings');
    });

    this.statusConference = false;
    this.statusDivision = false;
    this.statusLeague = true;
  }

  goToStats() {
    this.router.navigate(['/stats']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToSchedule() {
    this.router.navigate(['/schedule']);
  }

  goToTransactions() {
    this.router.navigate(['/transactions']);
  }

}
