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
import { TeamSalaryCapInfo } from '../_models/teamSalaryCapInfo';
import { PlayerContractDetailed } from '../_models/playerContractDetailed';
import { TradePlayerView } from '../_models/tradePlayerView';

@Component({
  selector: 'app-trades',
  templateUrl: './trades.component.html',
  styleUrls: ['./trades.component.css']
})
export class TradesComponent implements OnInit {
  league: League;
  allOtherTeams: Team[] = [];
  teamSelected: number;
  // yourTeamRoster: Player[] = [];
  // selectedTeamRoster: Player[] = [];
  playersInTrade: TradePlayerView[] = [];
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

  yourSalaryCapSpace: TeamSalaryCapInfo;
  theirSalaryCapSpace: TeamSalaryCapInfo;

  invalidTradeMessage = '';
  validTrade = 0;
  yourTeamRoster: TradePlayerView[] = [];
  selectedTeamRoster: TradePlayerView[] = [];

  constructor(private alertify: AlertifyService, private router: Router, private teamService: TeamService,
    private transferService: TransferService, private modalService: BsModalService,
    private fb: FormBuilder, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    const teamId = +localStorage.getItem('teamId');
    this.teamService.getAllTeamsExceptUsers(teamId).subscribe(result => {
      this.allOtherTeams = result;
      this.teamSelected = this.allOtherTeams[0].id;
    }, error => {
      this.alertify.error('Error gettings teams to trade with');
    });

    this.teamService.getTeamForTeamId(teamId).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    });

    this.teamService.getTradePlayerView(teamId).subscribe(result => {
      this.yourTeamRoster = result;
    }, error => {
      this.alertify.error('Error gett player details');
    });

    this.teamService.getTradeOffers(teamId).subscribe(result => {
      this.offeredTrades = result;
      this.offeredTrades.forEach(element => {
        const value = this.tradeIds.includes(element.tradeId);
        if (!value) {
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
      this.yourTeamPicks.forEach(val => this.masterYourTeamPicks.push(Object.assign({}, val)));
    }, error => {
      this.alertify.error('Error getting your teams picks');
    });

    this.teamService.getTeamSalaryCapDetails(teamId).subscribe(result => {
      this.yourSalaryCapSpace = result;
    }, error => {
      this.alertify.error('Error getting your teams salary cap');
    });
  }

  getTeamsPlayers() {
    this.showPropose = true;

    // tslint:disable-next-line: triple-equals
    const temp = this.allOtherTeams.filter(x => x.id == this.teamSelected);
    this.tradeTeam = temp[0];

    this.teamService.getTradePlayerView(this.tradeTeam.id).subscribe(result => {
      this.selectedTeamRoster = result;
    }, error => {
      this.alertify.error('Error get player details');
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

    this.teamService.getTeamSalaryCapDetails(this.teamSelected).subscribe(result => {
      this.theirSalaryCapSpace = result;
    }, error => {
      this.alertify.error('Error getting selected teams salary cap');
    });
  }

  viewPlayer(player: TradePlayerView) {
    this.transferService.setData(player.playerId);
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
    // First we need to check if the salary cap rules pass for the trade to be accepted
    const yourSalary = this.yourSalaryCapSpace.salaryCapAmount - this.yourSalaryCapSpace.currentSalaryAmount;
    const theirSalary = this.theirSalaryCapSpace.salaryCapAmount - this.theirSalaryCapSpace.currentSalaryAmount;
    this.validTrade = 0;

    // tslint:disable-next-line: max-line-length
    if ((yourSalary >= 0) && (theirSalary >= 0)) {
      // Both Teams are under the salary cap correctly
      this.validTrade = 1;
    } else if (yourSalary < 0) {
      // Your Team is over the cap
      let yourSalaryReceived = 0;
      let theirSalaryReceived = 0;

      this.proposedTradeSending.forEach(element => {
        theirSalaryReceived = theirSalaryReceived + element.yearOne;
      });

      this.proposedTradeReceiving.forEach(element => {
        yourSalaryReceived = yourSalaryReceived + element.yearOne;
      });

      // Now the calc to check if the trade is valid
      const value = 100000 + (theirSalaryReceived * .25);

      if (value < yourSalaryReceived) {
        this.invalidTradeMessage = 'Your team cannot make this trade due to salary cap rules';
      } else {
        this.validTrade = 1;
      }
    } else if (theirSalary < 0) {
      // Receiving team is over the cap
      let yourSalaryReceived = 0;
      let theirSalaryReceived = 0;

      this.proposedTradeSending.forEach(element => {
        theirSalaryReceived = theirSalaryReceived + element.yearOne;
      });

      this.proposedTradeReceiving.forEach(element => {
        yourSalaryReceived = yourSalaryReceived + element.yearOne;
      });

      // Now the calc to check if the trade is valid
      const value = 100000 + (yourSalaryReceived * .25);

      if (value < theirSalaryReceived) {
        this.invalidTradeMessage = 'Their team cannot make this trade due to salary cap rules';
      } else {
        this.validTrade = 1;
      }
    }

    if (this.validTrade === 1) {
      this.spinner.show();
      if (this.proposedTradeSending.length !== 0 && this.proposedTradeReceiving.length !== 0) {
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
      } else {
        this.alertify.error('Both sides must have something in a trade');
      }
    } else {
      this.alertify.error('Trade is invalid due to salary cap rules');
    }

  }

  removePlayer(player: Trade, side: number) {
    if (side === 0) {
      if (player.pick === 0) {
        const index = this.proposedTradeReceiving.findIndex(x => x.playerId === player.playerId);
        this.proposedTradeReceiving.splice(index, 1);

        // Need to add the player back to the player lists
        const idx = this.playersInTrade.findIndex(x => x.playerId === player.playerId);
        const ply = this.playersInTrade[idx];
        this.selectedTeamRoster.push(ply);

        this.playersInTrade.splice(idx, 1);

        // Now need to update the salary caps
        this.yourSalaryCapSpace.currentSalaryAmount = this.yourSalaryCapSpace.currentSalaryAmount - ply.currentSeasonValue;
        this.theirSalaryCapSpace.currentSalaryAmount = this.theirSalaryCapSpace.currentSalaryAmount + ply.currentSeasonValue;
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
        const idx = this.playersInTrade.findIndex(x => x.playerId === player.playerId);
        const ply = this.playersInTrade[idx];
        this.yourTeamRoster.push(ply);

        this.playersInTrade.splice(idx, 1);

        // Now need to update the salary caps
        this.yourSalaryCapSpace.currentSalaryAmount = this.yourSalaryCapSpace.currentSalaryAmount + ply.currentSeasonValue;
        this.theirSalaryCapSpace.currentSalaryAmount = this.theirSalaryCapSpace.currentSalaryAmount - ply.currentSeasonValue;
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
    const yourSalary = this.yourSalaryCapSpace.salaryCapAmount - this.yourSalaryCapSpace.currentSalaryAmount;
    const theirSalary = this.theirSalaryCapSpace.salaryCapAmount - this.theirSalaryCapSpace.currentSalaryAmount;
    this.validTrade = 0;

    // tslint:disable-next-line: max-line-length
    if ((yourSalary >= 0) && (theirSalary >= 0)) {
      // Both Teams are under the salary cap correctly
      this.validTrade = 1;
    } else if (yourSalary < 0) {
      // Your Team is over the cap
      let yourSalaryReceived = 0;
      let theirSalaryReceived = 0;

      this.proposedTradeSending.forEach(element => {
        theirSalaryReceived = theirSalaryReceived + element.yearOne;
      });

      this.proposedTradeReceiving.forEach(element => {
        yourSalaryReceived = yourSalaryReceived + element.yearOne;
      });

      // Now the calc to check if the trade is valid
      const value = 100000 + (theirSalaryReceived * .25);

      if (value < yourSalaryReceived) {
        this.invalidTradeMessage = 'Your team cannot make this trade due to salary cap rules';
      } else {
        this.validTrade = 1;
      }
    } else if (theirSalary < 0) {
      // Receiving team is over the cap
      let yourSalaryReceived = 0;
      let theirSalaryReceived = 0;

      this.proposedTradeSending.forEach(element => {
        theirSalaryReceived = theirSalaryReceived + element.yearOne;
      });

      this.proposedTradeReceiving.forEach(element => {
        yourSalaryReceived = yourSalaryReceived + element.yearOne;
      });

      // Now the calc to check if the trade is valid
      const value = 100000 + (yourSalaryReceived * .25);

      if (value < theirSalaryReceived) {
        this.invalidTradeMessage = 'Their team cannot make this trade due to salary cap rules';
      } else {
        this.validTrade = 1;
      }
    }

    if (this.validTrade === 1) {
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

  addToTrade(player: TradePlayerView, side: number) {
    if (side === 0) {
      // its your team
      const trade: Trade = {
        tradingTeam: this.team.id,
        tradingTeamName: this.team.mascot,
        receivingTeam: +this.teamSelected,
        receivingTeamName: '',
        tradeId: 0,
        playerId: player.playerId,
        playerName: player.surname,
        pick: 0,
        year: 0,
        originalTeamId: 0,
        status: 0,
        yearOne: player.currentSeasonValue,
        totalValue: player.totalValue,
        years: player.years
      };

      this.proposedTradeSending.push(trade);

      // Need to remove the player from the players list
      this.playersInTrade.push(player);
      const index = this.yourTeamRoster.findIndex(x => x.playerId === player.playerId);
      this.yourTeamRoster.splice(index, 1);

      // Now need to update both teams salary caps
      this.yourSalaryCapSpace.currentSalaryAmount = this.yourSalaryCapSpace.currentSalaryAmount - trade.yearOne;
      this.theirSalaryCapSpace.currentSalaryAmount = this.theirSalaryCapSpace.currentSalaryAmount + trade.yearOne;


    } else if (side === 1) {
      // the selected team
      // Create a new trade object
      const trade: Trade = {
        tradingTeam: +this.teamSelected,
        tradingTeamName: '',
        receivingTeam: this.team.id,
        receivingTeamName: this.team.mascot,
        tradeId: 0,
        playerId: player.playerId,
        playerName: player.surname,
        pick: 0,
        year: 0,
        originalTeamId: 0,
        status: 0,
        yearOne: player.currentSeasonValue,
        totalValue: player.totalValue,
        years: player.years
      };

      this.proposedTradeReceiving.push(trade);

      // Need to remove the player from the players list
      this.playersInTrade.push(player);
      const index = this.selectedTeamRoster.findIndex(x => x.playerId === player.playerId);
      this.selectedTeamRoster.splice(index, 1);

      // Need to update cap space situations
      this.yourSalaryCapSpace.currentSalaryAmount = this.yourSalaryCapSpace.currentSalaryAmount + trade.yearOne;
      this.theirSalaryCapSpace.currentSalaryAmount = this.theirSalaryCapSpace.currentSalaryAmount - trade.yearOne;
    }
  }

  addPickToTrade(pick: TeamDraftPick, side: number) {
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
        status: 0,
        yearOne: 0,
        totalValue: 0,
        years: 0
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
        status: 0,
        yearOne: 0,
        totalValue: 0,
        years: 0
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
    this.tradeDisplay = this.offeredTrades.filter(x => x.tradeId === tradeId);

    console.log('TeamId: ' + this.team.id);
    console.log('Receiving Team Id: ' + this.tradeDisplay[0].receivingTeam);
    console.log('Trading Team Id: ' + this.tradeDisplay[0].tradingTeam);

    if (this.team.id !== this.tradeDisplay[0].receivingTeam) {
      this.recevingTeamText = this.tradesToDisplay[0].receivingTeamName;
    } else if (this.team.id !== this.tradeDisplay[0].tradingTeam) {
      this.recevingTeamText = this.tradesToDisplay[0].tradingTeamName;
    }

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
