import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { Player } from '../_models/player';
import { PlayerService } from '../_services/player.service';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';

@Component({
  selector: 'app-freeagents',
  templateUrl: './freeagents.component.html',
  styleUrls: ['./freeagents.component.css']
})
export class FreeagentsComponent implements OnInit {
  league: League;
  team: Team;
  rosterSpotAvailable = true;
  freeAgents: Player[] = [];

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private playerService: PlayerService,
              private teamService: TeamService, private authService: AuthService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.CheckRosterSpots();
    });

    // Get the freeagents player listing
    this.playerService.getFreeAgents().subscribe(result => {
      this.freeAgents = result;
    }, error => {
      this.alertify.error('Error getting free agents');
    });
  }

  CheckRosterSpots() {
    this.teamService.rosterSpotCheck(this.team.id).subscribe(result => {
      this.rosterSpotAvailable = result;
    }, error => {
      this.alertify.error('Error checking roster spots');
    });
  }

}
