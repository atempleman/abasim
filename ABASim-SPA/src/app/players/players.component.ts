import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { Player } from '../_models/player';

@Component({
  selector: 'app-players',
  templateUrl: './players.component.html',
  styleUrls: ['./players.component.css']
})
export class PlayersComponent implements OnInit {
  allPlayers: Player[] = [];

  constructor(private router: Router, private alertify: AlertifyService, private playerService: PlayerService) { }

  ngOnInit() {
    this.playerService.getAllPlayers().subscribe(result => {
      this.allPlayers = result;
    }, error => {
      this.alertify.error('Error getting players');
    });
  }


}
