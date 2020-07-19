import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { DraftPlayer } from '../_models/draftPlayer';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { DraftService } from '../_services/draft.service';
import { AddDraftRank } from '../_models/addDraftRank';
import { Router } from '@angular/router';
import { TransferService } from '../_services/transfer.service';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-draft-player-pool',
  templateUrl: './draft-player-pool.component.html',
  styleUrls: ['./draft-player-pool.component.css']
})
export class DraftPlayerPoolComponent implements OnInit {
    draftPlayers: DraftPlayer[] = [];
    draftboardPlayers: DraftPlayer[] = [];
    pageOfPlayers: DraftPlayer[] = [];
    masterList: DraftPlayer[] = [];
    team: Team;
    pages = 1;
    pager = 1;
    recordTotal = 0;
    // newRanking: AddDraftRank = {};

    constructor(private alertify: AlertifyService, private playerService: PlayerService, private teamService: TeamService,
                private authService: AuthService, private draftService: DraftService, private router: Router,
                private transferService: TransferService, private spinner: NgxSpinnerService) { }

    ngOnInit() {
      this.spinner.show();

      this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
        this.team = result;
      }, error => {
        this.alertify.error('Error getting your team');
      }, () => {
        this.getDraftboardPlayers();
        this.getCountOfAvailablePlayers();
      });
    }

    getCountOfAvailablePlayers() {
      this.playerService.getCountOfAvailableDraftPlayers().subscribe(result => {
        this.recordTotal = result;
      }, error => {
        this.alertify.error('Error getting count of available players');
      }, () => {
        this.pages = +(this.recordTotal / 50).toFixed(0);
      });
    }

    getDraftboardPlayers() {
      // Need to get the draftboard players
      this.draftService.getDraftBoardForTeam(this.team.id).subscribe(result => {
        this.draftboardPlayers = result;
        console.log(result);
      }, error => {
        this.alertify.error('Error getting draftboard');
      }, () => {
        this.getDraftPlayers();
      });
    }

    getDraftPlayers() {
      // Get all draft players
      this.playerService.getInitialDraftPlayers(this.pager).subscribe(result => {
        this.draftPlayers = result;
        this.masterList = result;
        // this.setPageArray();
      }, error => {
        this.alertify.error('Error getting players available for the draft');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }

    counter(i: number) {
      return new Array(i);
    }

    viewPlayer(player: number) {
      console.log(player);
      this.transferService.setData(player);
      this.router.navigate(['/view-player']);
    }

    checkPlayer(playerId: number) {
      const db = this.draftboardPlayers.find(x => x.playerId === playerId);
      if (db) {
        return 1;
      } else {
        return 0;
      }
    }

    pagerNext() {
      this.spinner.show();
      this.pager = this.pager + 1;
      if (this.pager > this.pages) {
        this.pager = this.pager - 1;
      }

      this.playerService.getInitialDraftPlayers(this.pager).subscribe(result => {
        this.draftPlayers = result;
        this.masterList = result;
      }, error => {
        this.alertify.error('Error getting players available for the draft');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }

    pagerPrev() {
      this.spinner.show();

      this.pager = this.pager - 1;
      if (this.pager < 1) {
        this.pager = this.pager + 1;
      }

      this.playerService.getInitialDraftPlayers(this.pager).subscribe(result => {
        this.draftPlayers = result;
        this.masterList = result;
      }, error => {
        this.alertify.error('Error getting players available for the draft');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }

    goToPage(page: number) {
      this.spinner.show();

      this.pager = page;

      this.playerService.getInitialDraftPlayers(this.pager).subscribe(result => {
        this.draftPlayers = result;
        this.masterList = result;
      }, error => {
        this.alertify.error('Error getting players available for the draft');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }

    addPlayerToDraftRank(selectedPlayer: DraftPlayer) {
      const newRanking = {} as AddDraftRank;
      newRanking.playerId = selectedPlayer.playerId;
      newRanking.teamId = this.team.id;

      this.draftService.addDraftPlayerRanking(newRanking).subscribe(result => {
      }, error => {
        this.alertify.error('Error adding Player to Draft Board');
      }, () => {
        this.alertify.success('Player added to Draft Board.');
        const record = this.draftPlayers.find(x => x.playerId === selectedPlayer.playerId);
        this.draftboardPlayers.push(record);
      });
    }

    removePlayerDraftRank(selectedPlayer: DraftPlayer) {
      const newRanking = {} as AddDraftRank;
      newRanking.playerId = selectedPlayer.playerId;
      newRanking.teamId = this.team.id;

      this.draftService.removeDraftPlayerRanking(newRanking).subscribe(result => {

      }, error => {
        this.alertify.error('Error removing Player from Draft board');
      }, () => {
        const index = this.draftboardPlayers.findIndex(x => x.playerId === selectedPlayer.playerId);
        this.draftboardPlayers.splice(index, 1);
      });
    }

}
