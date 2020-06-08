import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { Player } from '../_models/player';
import { TransferService } from '../_services/transfer.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-players',
  templateUrl: './players.component.html',
  styleUrls: ['./players.component.css']
})
export class PlayersComponent implements OnInit {
  allPlayers: Player[] = [];

  constructor(private router: Router, private alertify: AlertifyService, private authService: AuthService,
              private transferService: TransferService, private playerService: PlayerService) { }

  ngOnInit() {
    this.playerService.getAllPlayers().subscribe(result => {
      this.allPlayers = result;
    }, error => {
      this.alertify.error('Error getting players');
    });
  }

  viewPlayer(player: Player) {
    this.transferService.setData(player.id);
    this.router.navigate(['/view-player']);
  }
}
