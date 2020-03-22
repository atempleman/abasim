import { Component, OnInit } from '@angular/core';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { AlertifyService } from '../_services/alertify.service';
import { Player } from '../_models/player';

@Component({
  selector: 'app-team-roster',
  templateUrl: './team-roster.component.html',
  styleUrls: ['./team-roster.component.css']
})
export class TeamRosterComponent implements OnInit {
  team: Team;
  playingRoster: Player[] = [];
  playerCount = 0;

  constructor(private teamService: TeamService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.getRosterForTeam();
    });
  }

  getRosterForTeam() {
    this.teamService.getRosterForTeam(this.team.id).subscribe(result => {
      this.playingRoster = result;
      console.log(this.playingRoster);
      this.playerCount = this.playingRoster.length;
    }, error => {
      this.alertify.error('Error getting your roster');
    });
  }

}
