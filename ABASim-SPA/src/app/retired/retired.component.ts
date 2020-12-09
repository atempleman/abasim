import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { RetiredPlayer } from '../_models/retiredPlayer';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-retired',
  templateUrl: './retired.component.html',
  styleUrls: ['./retired.component.css']
})
export class RetiredComponent implements OnInit {
  allPlayers: RetiredPlayer[] = [];

  constructor(private router: Router, private alertify: AlertifyService, private playerService: PlayerService,
              private spinner: NgxSpinnerService, private transferService: TransferService) { }

  ngOnInit() {
    this.playerService.getRetiredPlayers().subscribe(result => {
      this.allPlayers = result;
    }, error => {
      this.alertify.error('Error getting retired players');
    });
  }

  viewPlayer(player: RetiredPlayer) {
    this.transferService.setData(player.id);
    this.router.navigate(['/view-retired-player']);
  }
}
