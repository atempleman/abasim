import { Component, OnInit, TemplateRef } from '@angular/core';
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
import { DraftTracker } from '../_models/draftTracker';
import { InitialDraftPicks } from '../_models/initialDraftPicks';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { AdminService } from '../_services/admin.service';
import { DraftSelection } from '../_models/draftSelection';

@Component({
  selector: 'app-draftboard',
  templateUrl: './draftboard.component.html',
  styleUrls: ['./draftboard.component.css']
})
export class DraftboardComponent implements OnInit {
  draftPlayers: DraftPlayer[] = [];
  team: Team;
  public modalRef: BsModalRef;
  currentPick: InitialDraftPicks;
  selection: DraftPlayer;

  constructor(private alertify: AlertifyService, private draftService: DraftService, private authService: AuthService,
              private teamService: TeamService, private router: Router, private transferService: TransferService,
              private spinner: NgxSpinnerService, private modalService: BsModalService, private adminService: AdminService) { }

  ngOnInit() {
    this.spinner.show();
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.getDraftboardPlayers();
    });

    // this.draftService.getDraftTracker().subscribe(result => {
    //   this.tracker = result;
    // }, error => {
    //   this.alertify.error('Error getting draft tracker');
    // }, () => {
    //   this.currentRound = this.tracker.round;
    // });

    this.draftService.getCurrentInitialDraftPick().subscribe(result => {
      this.currentPick = result;
      console.log(this.currentPick);
    }, error => {
      this.alertify.error('Error getting current draft pick');
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

  public openModal(template: TemplateRef<any>, selection: DraftPlayer) {
    this.selection = selection;
    this.modalRef = this.modalService.show(template);
  }

  makeDraftPick() {
    const selectedPick: DraftSelection = {
      pick: this.currentPick.pick,
      playerId: this.selection.playerId,
      round: this.currentPick.round,
      teamId: this.team.id
    };

    this.draftService.makeDraftPick(selectedPick).subscribe(result => {
    }, error => {
      this.alertify.error('Error making pick');
    }, () => {
      this.modalRef.hide();
      this.alertify.success('Selection made successfully');

      if (this.currentPick.round === 13 && this.currentPick.pick === 30) {
        // Update the leage state here
        this.adminService.updateLeagueStatus(5).subscribe(result => {
        }, error => {
          this.alertify.error('Error changing league state');
        }, () => {
          this.alertify.success('Draft Completed');
        });
      } else {
        this.draftService.getCurrentInitialDraftPick().subscribe(result => {
          this.currentPick = result;
          console.log(this.currentPick);
        }, error => {
          this.alertify.error('Error getting current draft pick');
        });
      }
    });
   }

}
