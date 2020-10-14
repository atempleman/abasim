import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { Standing } from '../_models/standing';
import { Router } from '@angular/router';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';

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

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private transferService: TransferService,
              private authService: AuthService, private router: Router, private teamService: TeamService,
              private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.leagueService.getConferenceStandings(1).subscribe(result => {
      this.spinner.show();
      // tslint:disable-next-line: max-line-length
      this.eastStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
      // this.eastStandings = result;
    }, error => {
      this.alertify.success('Error getting eastern conference standings');
    }, () => {
      this.spinner.hide();
    });

    this.leagueService.getConferenceStandings(2).subscribe(result => {
      this.spinner.show();
      // tslint:disable-next-line: max-line-length
      this.westStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
      // this.westStandings = result;
    }, error => {
      this.alertify.success('Error getting western conference standings');
    }, () => {
      this.spinner.hide();
    });
  }

  conferenceClick() {
    this.leagueService.getConferenceStandings(1).subscribe(result => {
      // this.eastStandings = result;
      // tslint:disable-next-line: max-line-length
      this.eastStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting eastern conference standings');
    });

    this.leagueService.getConferenceStandings(2).subscribe(result => {
      // this.westStandings = result;
      // tslint:disable-next-line: max-line-length
      this.westStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting western conference standings');
    });

    this.statusLeague = false;
    this.statusDivision = false;
    this.statusConference = true;
  }

  divisionClick() {
    this.leagueService.getDivisionStandings(1).subscribe(result => {
      // this.atlanticStandings = result;
      // tslint:disable-next-line: max-line-length
      this.atlanticStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(2).subscribe(result => {
      // this.centralstandings = result;
      // tslint:disable-next-line: max-line-length
      this.centralstandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(3).subscribe(result => {
      // this.southeastStandings = result;
      // tslint:disable-next-line: max-line-length
      this.southeastStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(4).subscribe(result => {
      // this.northwestStandings = result;
      // tslint:disable-next-line: max-line-length
      this.northwestStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(5).subscribe(result => {
      // this.pacificStandings = result;
      // tslint:disable-next-line: max-line-length
      this.pacificStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.leagueService.getDivisionStandings(6).subscribe(result => {
      // this.southwestStandings = result;
      // tslint:disable-next-line: max-line-length
      this.southwestStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting division standings');
    });

    this.statusConference = false;
    this.statusLeague = false;
    this.statusDivision = true;
  }

  leagueClick() {
    this.leagueService.getLeagueStandings().subscribe(result => {
      // this.allStandings = result;
      // tslint:disable-next-line: max-line-length
      this.allStandings = result.sort((a, b) => (a.wins / a.gamesPlayed) < (b.wins / b.gamesPlayed) ? 1 : (a.wins / a.gamesPlayed) > (b.wins / b.gamesPlayed) ? -1 : 0);
    }, error => {
      this.alertify.error('Error getting league standings');
    });

    this.statusConference = false;
    this.statusDivision = false;
    this.statusLeague = true;
  }

  getWinPercantage(wins: number, played: number) {
    return (wins / played).toFixed(3);
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

  viewTeam(name: string) {
    // Need to go a call to get the team id
    console.log(name);
    let team: Team;
    this.teamService.getTeamForTeamName(name).subscribe(result => {
      team = result;
    }, error => {
      this.alertify.error('Error getting players team');
    }, () => {
      this.transferService.setData(team.id);
      this.router.navigate(['/view-team']);
    });
  }

}
