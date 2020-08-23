import { Component, OnInit, TemplateRef, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { League } from '../_models/league';
import { Team } from '../_models/team';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { WaivedPlayer } from '../_models/waivedPlayer';
import { TransferService } from '../_services/transfer.service';
import { PlayerInjury } from '../_models/playerInjury';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.css']
})
export class TeamComponent implements OnInit {
  league: League;
  team: Team;
  playingRoster: ExtendedPlayer[] = [];
  isAdmin = 0;
  playerCount = 0;
  selectedPlayer: ExtendedPlayer;
  message: number;

  public modalRef: BsModalRef;

  teamsInjuries: PlayerInjury[] = [];

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService, private modalService: BsModalService,
              private transferService: TransferService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

    // get the league object - TODO - roll the league state into the object as a Dto and pass back
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.getPlayerInjuries();
      this.getRosterForTeam();
    });
  }

  getPlayerInjuries() {
    this.teamService.getPlayerInjuriesForTeam(this.team.id).subscribe(result => {
      this.teamsInjuries = result;
    }, error => {
      this.alertify.error('Error getting teams injuries');
    });
  }

  checkIfInjured(playerId: number) {
    const injured = this.teamsInjuries.find(x => x.playerId === playerId);

    if(injured) {
      return 1;
    } else {
      return 0;
    }
  }

  public openModal(template: TemplateRef<any>, player: ExtendedPlayer) {
    this.selectedPlayer = player;
    this.modalRef = this.modalService.show(template);
  }

  getRosterForTeam() {
    this.teamService.getExtendedRosterForTeam(this.team.id).subscribe(result => {
      this.playingRoster = result;
      this.playerCount = this.playingRoster.length;
    }, error => {
      this.alertify.error('Error getting your roster');
    });
  }

  confirmedWaived() {
    const waivePlayer: WaivedPlayer = {
      teamId: this.team.id,
      playerId: this.selectedPlayer.playerId
    };
    this.teamService.waivePlayer(waivePlayer).subscribe(result => {

    }, error => {
      this.alertify.error('Error waiving player');
    }, () => {
      this.getRosterForTeam();
      this.modalRef.hide();
    });
  }

  viewPlayer(player: ExtendedPlayer) {
    this.transferService.setData(player.playerId);
    this.router.navigate(['/view-player']);
  }

  goToCoaching() {
    this.router.navigate(['/coaching']);
  }

  goToDepthCharts() {
    this.router.navigate(['/depthchart']);
  }

  goToFreeAgents() {
    this.router.navigate(['/freeagents']);
  }

  goToTrades() {
    this.router.navigate(['/trades']);
  }

}
