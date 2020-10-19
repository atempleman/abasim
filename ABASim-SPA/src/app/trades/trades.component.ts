import { Component, OnInit, TemplateRef } from '@angular/core';
import { League } from '../_models/league';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { Player } from '../_models/player';
import { TransferService } from '../_services/transfer.service';
import { Trade } from '../_models/trade';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { TradeMessage } from '../_models/tradeMessage';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TeamDraftPick } from '../_models/teamDraftPick';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-trades',
  templateUrl: './trades.component.html',
  styleUrls: ['./trades.component.css']
})
export class TradesComponent implements OnInit {
  league: League;
  allOtherTeams: Team[] = [];
  teamSelected: number;
  yourTeamRoster: Player[] = [];
  selectedTeamRoster: Player[] = [];
  playersInTrade: Player[] = [];
  team: Team;
  tradeTeam: Team;

  selectedTeamPicks: TeamDraftPick[] = [];
  yourTeamPicks: TeamDraftPick[] = [];
  masterSelectedTeamPicks: TeamDraftPick[] = [];
  masterYourTeamPicks: TeamDraftPick[] = [];
  picksInTrade: TeamDraftPick[] = [];

  offeredTrades: Trade[] = [];
  tradeIds: number[] = [];
  tradesToDisplay: Trade[] = [];
  tradeDisplay: Trade[] = [];
  tradesReady = false;

  proposedTradeSending: Trade[] = [];
  proposedTradeReceiving: Trade[] = [];

  actualTradeOffer: Trade[] = [];

  displayTeams = 0;
  recevingTeamText = '';

  pickText = '';

  tmForm: FormGroup;
  tmDisplay = false;
  tradeText = '';
  tradeMessage: TradeMessage;

  showPropose = false;

  public modalRef: BsModalRef;

  yourPlayersSelection = true;
  yourPicksSelection = false;
  theirPlayersSelection = true;
  theirPicksSelection = false;

  constructor(private alertify: AlertifyService, private router: Router, private teamService: TeamService,
              private transferService: TransferService, private modalService: BsModalService,
              private fb: FormBuilder, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    const teamId = +localStorage.getItem('teamId');
    this.teamService.getAllTeamsExceptUsers(teamId).subscribe(result => {
      this.allOtherTeams = result;
    }, error => {
      this.alertify.error('Error gettings teams to trade with');
    });

    this.teamService.getTeamForTeamId(teamId).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    });

    this.teamService.getRosterForTeam(teamId).subscribe(result => {
      this.yourTeamRoster = result;
    }, error => {
      this.alertify.error('Error getting your roster');
    });

    this.teamService.getTradeOffers(teamId).subscribe(result => {
      this.offeredTrades = result;
      console.log(result);
      this.offeredTrades.forEach(element => {
        const value = this.tradeIds.includes(element.tradeId);
        if (!value) {
          console.log('test ash');
          console.log(element);
          this.tradeIds.push(element.tradeId);
          this.tradesToDisplay.push(element);
        }
      });
      console.log(this.tradesToDisplay);
    }, error => {
      this.alertify.error('Error getting your offered trades');
    }, () => {
      this.tradesReady = true;
    });

    this.teamService.getTeamDraftPicks(teamId).subscribe(result => {
      this.yourTeamPicks = result;
      // this.masterYourTeamPicks = result;
      this.yourTeamPicks.forEach(val => this.masterYourTeamPicks.push(Object.assign({}, val)));
      console.log(this.masterYourTeamPicks);
    }, error => {
      this.alertify.error('Error getting your teams picks');
    });
  }

  getTeamsPlayers() {
    this.showPropose = true;

    // tslint:disable-next-line: triple-equals
    const temp = this.allOtherTeams.filter(x => x.id == this.teamSelected);
    this.tradeTeam = temp[0];

    this.teamService.getRosterForTeam(this.teamSelected).subscribe(result => {
      this.selectedTeamRoster = result;
    }, error => {
      this.alertify.error('Error getting selected roster');
    }, () => {
      this.displayTeams = 1;
    });

    this.teamService.getTeamDraftPicks(this.teamSelected).subscribe(result => {
      this.selectedTeamPicks = result;
      // this.masterSelectedTeamPicks = result.map(x => Object.assign({}, x));
      this.selectedTeamPicks.forEach(val => this.masterSelectedTeamPicks.push(Object.assign({}, val)));

    }, error => {
      this.alertify.error('Error getting selected teams picks');
    });
  }

  viewPlayer(player: Player) {
    this.transferService.setData(player.id);
    this.router.navigate(['/view-player']);
  }

  viewPlayerForId(player: number) {
    this.transferService.setData(player);
    this.modalRef.hide();
    this.router.navigate(['/view-player']);
  }

  getTeamShortCode(teamId: number) {
    if (this.team.id === teamId) {
      return this.team.shortCode;
    } else {
      console.log(this.allOtherTeams);
      const team = this.allOtherTeams.find(x => x.id === teamId);
      console.log(team);
      return team.shortCode;
    }
  }

  proposeTrade() {
    this.spinner.show();
    if (this.proposedTradeSending.length !== 0 || this.proposedTradeReceiving.length !== 0) {
      // Now need to create an array to pass through into API
      this.proposedTradeSending.forEach(element => {
        this.actualTradeOffer.push(element);
      });

      this.proposedTradeReceiving.forEach(element => {
        this.actualTradeOffer.push(element);
      });

      // Now need to pass into the service
      this.teamService.saveTradeProposal(this.actualTradeOffer).subscribe(result => {

      }, error => {
        this.alertify.error('Error making trade offer');
        this.spinner.hide();
      }, () => {
        this.alertify.success('Trade offer has been made');
        // Now need to update the screen back to its original state - do this with a page reload
        window.location.reload();
        this.spinner.hide();
      });
    }
  }

  removePlayer(player: Trade, side: number) {
    if (side === 0) {
      if (player.pick === 0) {
        const index = this.proposedTradeReceiving.findIndex(x => x.playerId === player.playerId);
        this.proposedTradeReceiving.splice(index, 1);

        // Need to add the player back to the player lists
        const idx = this.playersInTrade.findIndex(x => x.id === player.playerId);
        const ply = this.playersInTrade[idx];
        this.selectedTeamRoster.push(ply);

        this.playersInTrade.splice(idx, 1);
      } else {
        // Pick is being removed
        const index = this.proposedTradeReceiving.findIndex(x => x.pick === player.pick);
        this.proposedTradeReceiving.splice(index, 1);

        // Need to add the pick back to the pick lists
        const idx = this.picksInTrade.findIndex(x => x.round === player.pick);
        const pck = this.picksInTrade[idx];
        this.selectedTeamPicks.push(pck);

        this.picksInTrade.splice(idx, 1);
      }
    } else {
      if (player.pick === 0) {
        const index = this.proposedTradeSending.findIndex(x => x.playerId === player.playerId);
        this.proposedTradeSending.splice(index, 1);

        // Need to add the player back to the player lists
        const idx = this.playersInTrade.findIndex(x => x.id === player.playerId);
        const ply = this.playersInTrade[idx];
        this.yourTeamRoster.push(ply);

        this.playersInTrade.splice(idx, 1);
      } else {
        // Pick is being removed
        const index = this.proposedTradeSending.findIndex(x => x.pick === player.pick);
        this.proposedTradeSending.splice(index, 1);

        // Need to add the pick back to the pick lists
        const idx = this.picksInTrade.findIndex(x => x.round === player.pick);
        const pck = this.picksInTrade[idx];
        this.yourTeamPicks.push(pck);

        this.picksInTrade.splice(idx, 1);
      }
    }
  }

  acceptTrade(tradeId: number) {
    this.spinner.show();
    let tradeResult = false;
    this.teamService.acceptTradeProposal(tradeId).subscribe(result => {
      tradeResult = result;
    }, error => {
      this.alertify.error('Error accepting trade');
    }, () => {
      if (!tradeResult) {
        this.alertify.error('Error accepting trade');
      } else {
        this.modalRef.hide();
        this.alertify.success('Trade completed!');
        this.spinner.hide();
        this.goToTeam();
      }
    });
  }

  pullTrade(tradeId: number) {
    this.spinner.show();
    let tradeResult = false;
    this.teamService.pullTradeProposal(tradeId).subscribe(result => {
      tradeResult = result;
    }, error => {
      this.alertify.error('Error pulling trade');
    }, () => {
      if (!tradeResult) {
        this.alertify.error('Error pulling trade');
      } else {
        this.modalRef.hide();
        this.alertify.success('Trade has been cancelled!');
        window.location.reload();
        this.spinner.hide();
      }
    });
  }

  rejectTrade() {
    // let tradeId = this.tradeDisplay[0].tradeId;
    this.tmForm = this.fb.group({
      message: ['']
    });
    this.tmDisplay = true;
  }

  submitTradeMessage() {
    this.spinner.show();
    let ism = 0;
    if (this.tradeText) {
      ism = 1;
    }
    const tradeMessage: TradeMessage = {
      tradeId: this.tradeDisplay[0].tradeId,
      isMessage: ism,
      message: this.tradeText
    };

    // let rejectResult = false;
    this.teamService.rejectTradeProposal(tradeMessage).subscribe(result => {
      // rejectResult = result;
    }, error => {
      this.alertify.error('Error rejecting trade');
      this.spinner.hide();
    }, () => {
      this.alertify.success('Trade has been rejected');
      this.modalRef.hide();
      window.location.reload();
      this.spinner.hide();
    });
  }

  backToTrade() {
    this.tmDisplay = false;
  }

  addToTrade(player: Player, side: number) {
    if (side === 0) {
      // its your team
      const trade: Trade = {
        tradingTeam: this.team.id,
        tradingTeamName: this.team.mascot,
        receivingTeam: +this.teamSelected,
        receivingTeamName: '',
        tradeId: 0,
        playerId: player.id,
        playerName: player.firstName + ' ' + player.surname,
        pick: 0,
        year: 0,
        originalTeamId: 0,
        status: 0
      };

      this.proposedTradeSending.push(trade);

      // Need to remove the player from the players list
      this.playersInTrade.push(player);
      const index = this.yourTeamRoster.findIndex(x => x.id === player.id);
      this.yourTeamRoster.splice(index, 1);
    } else if (side === 1) {
      // the selected team
      // Create a new trade object
      const trade: Trade = {
        tradingTeam: +this.teamSelected,
        tradingTeamName: '',
        receivingTeam: this.team.id,
        receivingTeamName: this.team.mascot,
        tradeId: 0,
        playerId: player.id,
        playerName: player.firstName + ' ' + player.surname,
        pick: 0,
        year: 0,
        originalTeamId: 0,
        status: 0
      };

      this.proposedTradeReceiving.push(trade);

      // Need to remove the player from the players list
      this.playersInTrade.push(player);
      const index = this.selectedTeamRoster.findIndex(x => x.id === player.id);
      this.selectedTeamRoster.splice(index, 1);
    }
  }

  addPickToTrade(pick: TeamDraftPick, side: number) {
    console.log('trading pick');
    console.log('ashley');
    console.log(+pick.originalTeam);
    if (side === 0) {
      // its your team
      const trade: Trade = {
        tradingTeam: this.team.id,
        tradingTeamName: this.team.mascot,
        receivingTeam: +this.teamSelected,
        receivingTeamName: '',
        tradeId: 0,
        playerId: 0,
        playerName: '',
        pick: pick.round,
        year: pick.year,
        originalTeamId: pick.originalTeam,
        status: 0
      };
      console.log(trade);
      this.proposedTradeSending.push(trade);

      // Need to remove the player from the players list
      this.picksInTrade.push(pick);
      const index = this.yourTeamPicks.findIndex(x => x.round === pick.round && x.year === pick.year &&
                                                x.originalTeam === pick.originalTeam);
      this.yourTeamPicks.splice(index, 1);
    } else {
      // the selected team
      // Create a new trade object
      const trade: Trade = {
        tradingTeam: +this.teamSelected,
        tradingTeamName: '',
        receivingTeam: this.team.id,
        receivingTeamName: this.team.mascot,
        tradeId: 0,
        playerId: 0,
        playerName: '',
        pick: pick.round,
        year: pick.year,
        originalTeamId: pick.originalTeam,
        status: 0
      };

      this.proposedTradeReceiving.push(trade);

      // Need to remove the player from the players list
      this.picksInTrade.push(pick);
      const index = this.selectedTeamPicks.findIndex(x => x.round === pick.round && x.year === pick.year &&
                                                     x.originalTeam === pick.originalTeam);
      this.selectedTeamPicks.splice(index, 1);
    }
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

  goToTeam() {
    this.router.navigate(['/team']);
  }

  yourSelection(value: number) {
    if (value === 0) {
      this.yourPicksSelection = false;
      this.yourPlayersSelection = true;
    } else {
      this.yourPicksSelection = true;
      this.yourPlayersSelection = false;
    }
  }

  theirSelection(value: number) {
    if (value === 0) {
      this.theirPicksSelection = false;
      this.theirPlayersSelection = true;
    } else {
      this.theirPicksSelection = true;
      this.theirPlayersSelection = false;
    }
  }

  getPickDetails(side: number, round: number, year: number, origTeam: number) {
    // console.log(origTeam);
    if (side === 0) {
      // console.log(this.masterYourTeamPicks);
      // console.log(year);
      // console.log(round);
      // console.log(origTeam);
      const index = this.masterYourTeamPicks.findIndex(x => x.round === round && x.year === year &&
        x.originalTeam === origTeam);
      const value = this.masterYourTeamPicks[index];
      // console.log(value.originalTeamName + ' Year: ' + value.year + ' Round: ' + value.round);
      return value.originalTeamName + ' Year: ' + value.year + ' Round: ' + value.round;
    } else {
      // console.log(this.masterSelectedTeamPicks);
      // console.log(round);
      // console.log(year);
      // console.log(origTeam);
      const index = this.masterSelectedTeamPicks.findIndex(x => x.round === round && x.year === year &&
        x.originalTeam === origTeam);
      // console.log(index);
      const value = this.masterSelectedTeamPicks[index];
      return value.originalTeamName + ' Year: ' + value.year + ' Round: ' + value.round;
    }
  }

  public openModal(template: TemplateRef<any>, tradeId: number) {
    // console.log(this.tradesToDisplay);
    this.tradeDisplay = this.offeredTrades.filter(x => x.tradeId === tradeId);
    if (this.team.id !== this.tradeDisplay[0].receivingTeam) {
      this.recevingTeamText = this.tradesToDisplay[0].receivingTeamName;
    } else if (this.team.id !== this.tradeDisplay[0].tradingTeam) {
      this.recevingTeamText = this.tradesToDisplay[0].tradingTeamName;
    }
    // console.log(this.tradeDisplay);
    if (this.tradeDisplay[0].status === 2) {
      this.teamService.getTradeMessageForTradeId(this.tradeDisplay[0].tradeId).subscribe(result => {
        this.tradeMessage = result;
      }, error => {
        this.alertify.error('Error getting trade message');
      }, () => {
        this.modalRef = this.modalService.show(template);
      });
    } else {
      this.modalRef = this.modalService.show(template);
    }
  }
}
