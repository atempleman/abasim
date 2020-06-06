import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { AlertifyService } from '../_services/alertify.service';
import { TransferService } from '../_services/transfer.service';
import { PlayerService } from '../_services/player.service';
import { CompletePlayer } from '../_models/completePlayer';

@Component({
  selector: 'app-view-player',
  templateUrl: './view-player.component.html',
  styleUrls: ['./view-player.component.css']
})
export class ViewPlayerComponent implements OnInit {
  playerId: number;
  detailedPlayer: CompletePlayer;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService, private transferService: TransferService,
              private playerService: PlayerService) { }

  ngOnInit() {
    this.playerId = this.transferService.getData();
    this.playerService.playerForPlayerProfileById(this.playerId).subscribe(result => {
      this.detailedPlayer = result;
      console.log(this.detailedPlayer)
    }, error => {
      this.alertify.error('Error getting player profile');
    });
  }
}
