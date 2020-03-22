import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { DraftPlayer } from '../_models/draftPlayer';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { DraftService } from '../_services/draft.service';
import { AddDraftRank } from '../_models/addDraftRank';

@Component({
  selector: 'app-draft-player-pool',
  templateUrl: './draft-player-pool.component.html',
  styleUrls: ['./draft-player-pool.component.css']
})
export class DraftPlayerPoolComponent implements OnInit {
    draftPlayers: DraftPlayer[] = [];
    draftboardPlayers: DraftPlayer[] = [];
    team: Team;
    // newRanking: AddDraftRank = {};

    constructor(private alertify: AlertifyService, private playerService: PlayerService, private teamService: TeamService,
                private authService: AuthService, private draftService: DraftService) { }

    ngOnInit() {
      this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
        this.team = result;
      }, error => {
        this.alertify.error('Error getting your team');
      }, () => {
        this.getDraftboardPlayers();
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
      this.playerService.getInitialDraftPlayers().subscribe(result => {
        this.draftPlayers = result;
      }, error => {
        this.alertify.error('Error getting players available for the draft');
      }, () => {
        console.log(this.draftPlayers);
      });
    }

    checkPlayer(playerId: number) {
      const db = this.draftboardPlayers.find(x => x.playerId === playerId);
      if (db) {
        return 1;
      } else {
        return 0;
      }
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
