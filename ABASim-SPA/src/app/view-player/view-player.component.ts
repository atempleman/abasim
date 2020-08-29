import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { AlertifyService } from '../_services/alertify.service';
import { TransferService } from '../_services/transfer.service';
import { PlayerService } from '../_services/player.service';
import { CompletePlayer } from '../_models/completePlayer';
import { League } from '../_models/league';
import { PlayerInjury } from '../_models/playerInjury';

@Component({
  selector: 'app-view-player',
  templateUrl: './view-player.component.html',
  styleUrls: ['./view-player.component.css']
})
export class ViewPlayerComponent implements OnInit {
  playerId: number;
  detailedPlayer: CompletePlayer;
  league: League;

  statusStats = true;
  statusPlayoffStats = false;
  statusGrades = false;
  statusRatings = false;
  statusTendancies = false;

  playerInjury: PlayerInjury;
  injurySet = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService, private transferService: TransferService,
              private playerService: PlayerService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });

    this.playerId = this.transferService.getData();
    console.log('value: ' + this.playerId);
    this.playerService.playerForPlayerProfileById(this.playerId).subscribe(result => {
      this.detailedPlayer = result;
      console.log(result);
      console.log(this.detailedPlayer);
    }, error => {
      this.alertify.error('Error getting player profile');
    });

    this.teamService.getInjuryForPlayer(this.playerId).subscribe(result => {
      this.playerInjury = result;
      if (this.playerInjury) {
        this.injurySet = 1;
      }
    }, error => {
      this.alertify.error('Error checking player injury');
    });
  }

  getMinutesAverage() {
    console.log('here');
    console.log(this.detailedPlayer.minutesStats);
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

  getPlayoffMinutesAverage() {
    // console.log('here');
    // console.log(this.detailedPlayer.minutesStats);
    const value = ((this.detailedPlayer.playoffMinutesStats / this.detailedPlayer.playoffGamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAverage() {
    const value = (this.detailedPlayer.playoffFgmStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAAverage() {
    const value = (this.detailedPlayer.playoffFgaStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFgPercentage() {
    const value = ((this.detailedPlayer.playoffFgmStats) / (this.detailedPlayer.playoffFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffThreeFGAverage() {
    const value = (this.detailedPlayer.playoffThreeFgmStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFGAAverage() {
    const value = (this.detailedPlayer.playoffThreeFgaStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFgPercentage() {
    const value = ((this.detailedPlayer.playoffThreeFgmStats) / (this.detailedPlayer.playoffThreeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffFTAverage() {
    const value = (this.detailedPlayer.playoffFtmStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTAAverage() {
    const value = (this.detailedPlayer.playoffFtaStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTPercentage() {
    const value = ((this.detailedPlayer.playoffFtmStats) / (this.detailedPlayer.playoffFtaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffOrebAverage() {
    const value = (this.detailedPlayer.playoffOrebsStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffDrebverage() {
    const value = (this.detailedPlayer.playoffDrebsStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalRebAverage() {
    const totalRebs = this.detailedPlayer.playoffOrebsStats + this.detailedPlayer.playoffDrebsStats;
    // console.log('total rebs = ' + totalRebs);
    const value = (totalRebs / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalAstAverage() {
    const value = (this.detailedPlayer.playoffAstStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalStlAverage() {
    const value = (this.detailedPlayer.playoffStlStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalBlkAverage() {
    const value = (this.detailedPlayer.playoffBlkStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalTovAverage() {
    const value = (this.detailedPlayer.playoffToStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalFoulsAverage() {
    const value = (this.detailedPlayer.playoffFlsStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalPointsAverage() {
    const value = (this.detailedPlayer.playoffPtsStats / this.detailedPlayer.playoffGamesStats);
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

  playoffsStatisticsClick() {
    this.statusRatings = false;
    this.statusTendancies = false;
    this.statusGrades = false;
    this.statusStats = false;
    this.statusPlayoffStats = true;
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
