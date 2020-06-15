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

  statusStats = true;
  statusGrades = false;
  statusRatings = false;
  statusTendancies = false;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService, private transferService: TransferService,
              private playerService: PlayerService) { }

  ngOnInit() {
    this.playerId = this.transferService.getData();
    this.playerService.playerForPlayerProfileById(this.playerId).subscribe(result => {
      this.detailedPlayer = result;
      console.log(this.detailedPlayer);
    }, error => {
      this.alertify.error('Error getting player profile');
    });
  }

  getMinutesAverage() {
    const value = ((this.detailedPlayer.minutesStats / this.detailedPlayer.gamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getFGAverage() {
    const value = (this.detailedPlayer.fgmStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFGAAverage() {
    const value = (this.detailedPlayer.fgaStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFgPercentage() {
    const value = ((this.detailedPlayer.fgmStats) / (this.detailedPlayer.fgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getThreeFGAverage() {
    const value = (this.detailedPlayer.threeFgmStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFGAAverage() {
    const value = (this.detailedPlayer.threeFgaStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFgPercentage() {
    const value = ((this.detailedPlayer.threeFgmStats) / (this.detailedPlayer.threeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getFTAverage() {
    const value = (this.detailedPlayer.ftmStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTAAverage() {
    const value = (this.detailedPlayer.ftaStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTPercentage() {
    const value = ((this.detailedPlayer.ftmStats) / (this.detailedPlayer.ftaStats));
    const display = value.toFixed(3);
    return display;
  }

  getOrebAverage() {
    const value = (this.detailedPlayer.orebsStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getDrebverage() {
    const value = (this.detailedPlayer.drebsStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalRebAverage() {
    const totalRebs = this.detailedPlayer.orebsStats + this.detailedPlayer.drebsStats;
    console.log('total rebs = ' + totalRebs);
    const value = (totalRebs / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalAstAverage() {
    const value = (this.detailedPlayer.astStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalStlAverage() {
    const value = (this.detailedPlayer.stlStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalBlkAverage() {
    const value = (this.detailedPlayer.blkStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalTovAverage() {
    const value = (this.detailedPlayer.toStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalFoulsAverage() {
    const value = (this.detailedPlayer.flsStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalPointsAverage() {
    const value = (this.detailedPlayer.ptsStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }



  gradesClick() {
    this.statusStats = false;
    this.statusRatings = false;
    this.statusTendancies = false;
    this.statusGrades = true;
  }

  statisticsClick() {
    this.statusRatings = false;
    this.statusTendancies = false;
    this.statusGrades = false;
    this.statusStats = true;
  }

  ratingsClick() {
    this.statusTendancies = false;
    this.statusGrades = false;
    this.statusStats = false;
    this.statusRatings = true;
  }

  tendanciesClick() {
    this.statusGrades = false;
    this.statusStats = false;
    this.statusRatings = false;
    this.statusTendancies = true;
  }
}
