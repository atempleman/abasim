import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { League } from '../_models/league';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';

@Component({
  selector: 'app-initiallottery',
  templateUrl: './initiallottery.component.html',
  styleUrls: ['./initiallottery.component.css']
})
export class InitiallotteryComponent implements OnInit {
  league: League;
  teams: Team[] = [];

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private teamService: TeamService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
      // console.log(this.league);
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.getTeams();
    });
  }

  getTeams() {
    this.teamService.getAllTeams().subscribe(result => {
      this.teams = result;
    }, error => {
      this.alertify.error('Error getting all teams');
    });
  }

}
