import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { DraftService } from '../_services/draft.service';
import { DraftPlayer } from '../_models/draftPlayer';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { AddDraftRank } from '../_models/addDraftRank';
import { Router } from '@angular/router';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-draftboard',
  templateUrl: './draftboard.component.html',
  styleUrls: ['./draftboard.component.css']
})
export class DraftboardComponent implements OnInit {
  draftPlayers: DraftPlayer[] = [];
  team: Team;

  constructor(private alertify: AlertifyService, private draftService: DraftService, private authService: AuthService,
              private teamService: TeamService, private router: Router, private transferService: TransferService,
              private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.spinner.show();
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.getDraftboardPlayers();
    });
  }

  getDraftboardPlayers() {
    this.draftService.getDraftBoardForTeam(this.team.id).subscribe(result => {
      this.draftPlayers = result;
    }, error => {
      this.alertify.error('Error getting draftboard');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  removeDraftRanking(player: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = player.playerId;
    newRanking.teamId = this.team.id;

    this.draftService.removeDraftPlayerRanking(newRanking).subscribe(result => {
    }, error => {
      this.alertify.error('Error removing draftboard record');
    }, () => {
      // Now need to remove the record from the screen
      const index = this.draftPlayers.findIndex(x => x.playerId === player.playerId);
      this.draftPlayers.splice(index, 1);
    });
  }

  moveUp(player: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = player.playerId;
    newRanking.teamId = this.team.id;
    this.draftService.moveRankingUp(newRanking).subscribe(result => {
    }, error => {
      this.alertify.error('Error changing ranking');
    }, () => {
      this.getDraftboardPlayers();
    });
  }

  moveDown(player: DraftPlayer) {
    const newRanking = {} as AddDraftRank;
    newRanking.playerId = player.playerId;
    newRanking.teamId = this.team.id;
    this.draftService.moveRankingDown(newRanking).subscribe(result => {
    }, error => {
      this.alertify.error('Error changing ranking');
    }, () => {
      this.getDraftboardPlayers();
    });
  }

  draftHQClicked() {
    this.router.navigate(['/draft']);
  }

  playerPoolClicked() {
    this.router.navigate(['/draftplayerpool']);
  }

  lotteryClicked() {
    this.router.navigate(['/initiallottery']);
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

}
