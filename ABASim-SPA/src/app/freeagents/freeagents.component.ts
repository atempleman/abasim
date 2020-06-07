import { Component, OnInit, TemplateRef } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { Player } from '../_models/player';
import { PlayerService } from '../_services/player.service';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { Router } from '@angular/router';
import { TransferService } from '../_services/transfer.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { SignedPlayer } from '../_models/signedPlayer';

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
  selectedPlayer: Player;
  public modalRef: BsModalRef;

  constructor(private alertify: AlertifyService, private playerService: PlayerService, private teamService: TeamService,
              private authService: AuthService, private router: Router, private transferService: TransferService,
              private modalService: BsModalService) { }

  ngOnInit() {
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.CheckRosterSpots();
    });

    this.GetFreeAgents();
  }

  GetFreeAgents() {
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
      console.log(this.rosterSpotAvailable);
    }, error => {
      this.alertify.error('Error checking roster spots');
    });
  }

  viewPlayer(player: Player) {
    this.transferService.setData(player.id);
    this.router.navigate(['/view-player']);
  }

  signPlayer() {
    const signedPlayer: SignedPlayer = {
      teamId: this.team.id,
      playerId: this.selectedPlayer.id
    };

    this.teamService.signPlayer(signedPlayer).subscribe(result => {

    }, error => {
      this.alertify.error('Error signing player');
    }, () => {
      this.modalRef.hide();
      this.GetFreeAgents();
      this.alertify.success('Player signed successfully');
    });
  }

  public openModal(template: TemplateRef<any>, player: Player) {
    this.selectedPlayer = player;
    this.modalRef = this.modalService.show(template);
  }

  goToTeam() {
    this.router.navigate(['/team']);
  }

  goToDepthCharts() {
    this.router.navigate(['/depthchart']);
  }

  goToCoaching() {
    this.router.navigate(['/coaching']);
  }

  goToTrades() {
    this.router.navigate(['/trades']);
  }
}
