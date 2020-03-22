import { Component, OnInit } from '@angular/core';
import { TeamService } from '../_services/team.service';
import { AlertifyService } from '../_services/alertify.service';
import { Team } from '../_models/team';
import { AuthService } from '../_services/auth.service';
import { Player } from '../_models/player';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { LeagueState } from '../_models/leagueState';

@Component({
  selector: 'app-roster',
  templateUrl: './roster.component.html',
  styleUrls: ['./roster.component.css']
})
export class RosterComponent implements OnInit {
  rosterDisplayed = 1;
  coachingDisplayed = 0;
  depthChartDisplayed = 0;
  freeAgentsDisplayed = 0;
  tradesDisplayed = 0;
  // currentState: LeagueState;
  team: Team;
  players: Player[] = [];
  league: League;

  constructor(private teamService: TeamService, private authService: AuthService, private alertify: AlertifyService,
              private leagueService: LeagueService) { }

  ngOnInit() {
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      console.log(this.team);
      this.getRosterForTeamId();
    });

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
      console.log(this.league);
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    //  this.getCurrentLeagueState();
    });
  }

  // getCurrentLeagueState() {
  //   this.leagueService.getLeagueStatusForId(this.league.stateId).subscribe(result => {
  //     this.currentState = result;
  //   }, error => {
  //     console.log('Error getting current league state');
  //   });
  // }

  getRosterForTeamId() {
    this.teamService.getRosterForTeam(this.team.id).subscribe(result => {
      this.players = result;
    }, error => {
      this.alertify.error('Error getting player roster');
    }, () => {
      console.log(this.players);
    });
  }

  rosterClicked() {
    console.log('roster clicked');

    this.rosterDisplayed = 1;
    this.freeAgentsDisplayed = 0;
    this.tradesDisplayed = 0;
    this.depthChartDisplayed = 0;
    this.coachingDisplayed = 0;
  }

  coachingClicked() {
    console.log('coaching clicked');

    this.rosterDisplayed = 0;
    this.freeAgentsDisplayed = 0;
    this.tradesDisplayed = 0;
    this.depthChartDisplayed = 0;
    this.coachingDisplayed = 1;
  }

  depthChartsClicked() {
    console.log('depth chart clicked');

    this.rosterDisplayed = 0;
    this.freeAgentsDisplayed = 0;
    this.tradesDisplayed = 0;
    this.depthChartDisplayed = 1;
    this.coachingDisplayed = 0;
  }

  tradesClicked() {
    console.log('trades clicked');

    this.rosterDisplayed = 0;
    this.freeAgentsDisplayed = 0;
    this.tradesDisplayed = 1;
    this.depthChartDisplayed = 0;
    this.coachingDisplayed = 0;
  }

  freeAgentsClicked() {
    console.log('freeAgents clicked');

    this.rosterDisplayed = 0;
    this.freeAgentsDisplayed = 1;
    this.tradesDisplayed = 0;
    this.depthChartDisplayed = 0;
    this.coachingDisplayed = 0;
  }

}
