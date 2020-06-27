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

  offeredTrades: Trade[] = [];
  tradeIds: number[] = [];
  tradesToDisplay: Trade[] = [];
  tradeDisplay: Trade[] = [];

  proposedTradeSending: Trade[] = [];
  proposedTradeReceiving: Trade[] = [];

  actualTradeOffer: Trade[] = [];

  displayTeams = 0;
  recevingTeamText = '';

  public modalRef: BsModalRef;

  constructor(private alertify: AlertifyService, private router: Router, private teamService: TeamService,
              private transferService: TransferService, private modalService: BsModalService) { }

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
        console.log(element.tradeId);
        const value = this.tradeIds.includes(element.tradeId);
        console.log('value: ' + value);
        if (!value) {
          this.tradeIds.push(element.tradeId);
          this.tradesToDisplay.push(element);
        }
      });
      console.log('check offered trades');
    }, error => {
      this.alertify.error('Error getting your offered trades');
    });

    // this.teamService.getTradesReceived(teamId).subscribe(result => {
    //   console.log('check received trades');
    //   console.log(result);
    //   this.receivedTrades = result;
    // }, error => {
    //   this.alertify.error('Error getting your received trades');
    // });
  }

  getTeamsPlayers() {
    // console.log(this.teamSelected);
    this.teamService.getRosterForTeam(this.teamSelected).subscribe(result => {
      this.selectedTeamRoster = result;
    }, error => {
      this.alertify.error('Error getting selected roster');
    }, () => {
      this.displayTeams = 1;
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

  proposeTrade() {
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
    }, () => {
      this.alertify.success('Trade offer has been made');
      // Now need to update the screen back to its original state - do this with a page reload
      window.location.reload();
    });
  }

  removePlayer(player: Trade, side: number) {
    if (side === 0) {
      const index = this.proposedTradeReceiving.findIndex(x => x.playerId === player.playerId);
      this.proposedTradeReceiving.splice(index, 1);

      // Need to add the player back to the player lists
      const idx = this.playersInTrade.findIndex(x => x.id === player.playerId);
      const ply = this.playersInTrade[idx];
      this.selectedTeamRoster.push(ply);

      this.playersInTrade.splice(idx, 1);
    } else {
      const index = this.proposedTradeSending.findIndex(x => x.playerId === player.playerId);
      this.proposedTradeSending.splice(index, 1);

      // Need to add the player back to the player lists
      const idx = this.playersInTrade.findIndex(x => x.id === player.playerId);
      const ply = this.playersInTrade[idx];
      this.yourTeamRoster.push(ply);

      this.playersInTrade.splice(idx, 1);
    }
  }

  acceptTrade(tradeId: number) {
    let tradeResult = false;
    this.teamService.acceptTradeProposal(tradeId).subscribe(result => {
      tradeResult = result;
    }, error => {
      this.alertify.error('Error accepting trade');
    }, () => {
      if (!tradeResult) {
        this.alertify.error('Error accepting trade');
      } else {
        this.alertify.success('Trade completed!');
        this.goToTeam();
      }
    });
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
        status: 0
      };

      this.proposedTradeReceiving.push(trade);

      // Need to remove the player from the players list
      this.playersInTrade.push(player);
      const index = this.selectedTeamRoster.findIndex(x => x.id === player.id);
      this.selectedTeamRoster.splice(index, 1);
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

  public openModal(template: TemplateRef<any>, tradeId: number) {
    // console.log(this.tradesToDisplay);
    this.tradeDisplay = this.offeredTrades.filter(x => x.tradeId === tradeId);
    if (this.team.id !== this.tradeDisplay[0].receivingTeam) {
      this.recevingTeamText = this.tradesToDisplay[0].receivingTeamName;
    } else if (this.team.id !== this.tradeDisplay[0].tradingTeam) {
      this.recevingTeamText = this.tradesToDisplay[0].tradingTeamName;
    }
    console.log(this.tradeDisplay);
    this.modalRef = this.modalService.show(template);
  }
}
