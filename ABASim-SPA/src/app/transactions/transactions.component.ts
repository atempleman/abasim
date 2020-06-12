import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { League } from '../_models/league';
import { Transaction } from '../_models/transaction';
import { Player } from '@angular/core/src/render3/interfaces/player';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']
})
export class TransactionsComponent implements OnInit {
  transactions: Transaction[] = [];
  transCount = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private transferService: TransferService) { }

  ngOnInit() {
    this.leagueService.getTransactions().subscribe(result => {
      this.transactions = result;
      this.transCount = this.transactions.length;
    }, error => {
      this.alertify.error('Error getting league transactions');
    });
  }

  goToStandings() {
    this.router.navigate(['/standings']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToSchedule() {
    this.router.navigate(['/schedule']);
  }

  goToStats() {
    this.router.navigate(['/stats']);
  }

  viewPlayer(player: Transaction) {
    this.transferService.setData(player.playerId);
    this.router.navigate(['/view-player']);
  }

}
