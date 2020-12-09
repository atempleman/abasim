import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CareerStats } from '../_models/careerStats';
import { DetailedRetiredPlayer } from '../_models/detailedRetiredPlayer';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { PlayerService } from '../_services/player.service';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-view-retired-player',
  templateUrl: './view-retired-player.component.html',
  styleUrls: ['./view-retired-player.component.css']
})
export class ViewRetiredPlayerComponent implements OnInit {
  playerId: number;
  detailedPlayer: DetailedRetiredPlayer;
  careerStats: CareerStats[] = [];
  imageSrc = 'http://placehold.it/150x150';
  league: League;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private transferService: TransferService,
              private playerService: PlayerService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });
    this.playerId = this.transferService.getData();

    this.playerService.getDetailedRetiredPlayer(this.playerId).subscribe(result => {
      this.detailedPlayer = result;
    }, error => {
      this.alertify.error('Error getting player profile');
    });

    this.playerService.getCareerStats(this.playerId).subscribe(result => {
      this.careerStats = result;
    }, error => {
      this.alertify.error('Error getting career stats');
    });
  }

  getMinutesAverageForCareer(stats: CareerStats) {
    const value = ((stats.minutesStats / stats.gamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getFGAverageForCareer(stats: CareerStats) {
    const value = (stats.fgmStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFGAAverageForCareer(stats: CareerStats) {
    const value = (stats.fgaStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFgPercentageForCareer(stats: CareerStats) {
    const value = ((stats.fgmStats) / (stats.fgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getThreeFGAverageForCareer(stats: CareerStats) {
    const value = (stats.threeFgmStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFGAAverageForCareer(stats: CareerStats) {
    const value = (stats.threeFgaStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFgPercentageForCareer(stats: CareerStats) {
    const value = ((stats.threeFgmStats) / (stats.threeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getFTAverageForCareer(stats: CareerStats) {
    const value = (stats.ftmStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTAAverageForCareer(stats: CareerStats) {
    const value = (stats.ftaStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTPercentageForCareer(stats: CareerStats) {
    const value = ((stats.ftmStats) / (stats.ftaStats));
    const display = value.toFixed(3);
    return display;
  }

  getOrebAverageForCareer(stats: CareerStats) {
    const value = (stats.orebsStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getDrebverageForCareer(stats: CareerStats) {
    const value = (stats.drebsStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalRebAverageForCareer(stats: CareerStats) {
    const totalRebs = stats.orebsStats + stats.drebsStats;
    const value = (totalRebs / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalAstAverageForCareer(stats: CareerStats) {
    const value = (stats.astStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalStlAverageForCareer(stats: CareerStats) {
    const value = (stats.stlStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalBlkAverageForCareer(stats: CareerStats) {
    const value = (stats.blkStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalTovAverageForCareer(stats: CareerStats) {
    const value = (stats.toStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalFoulsAverageForCareer(stats: CareerStats) {
    const value = (stats.flsStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalPointsAverageForCareer(stats: CareerStats) {
    const value = (stats.ptsStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffMinutesAverageForCareer(stats: CareerStats) {
    const value = ((stats.playoffMinutesStats / stats.playoffGamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFgmStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFgaStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFgPercentageForCareer(stats: CareerStats) {
    const value = ((stats.playoffFgmStats) / (stats.playoffFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffThreeFGAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffThreeFgmStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFGAAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffThreeFgaStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFgPercentageForCareer(stats: CareerStats) {
    const value = ((stats.playoffThreeFgmStats) / (stats.playoffThreeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffFTAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFtmStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTAAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFtaStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTPercentageForCareer(stats: CareerStats) {
    const value = ((stats.playoffFtmStats) / (stats.playoffFtaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffOrebAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffOrebsStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffDrebverageForCareer(stats: CareerStats) {
    const value = (stats.playoffDrebsStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalRebAverageForCareer(stats: CareerStats) {
    const totalRebs = stats.playoffOrebsStats + stats.playoffDrebsStats;
    const value = (totalRebs / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalAstAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffAstStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalStlAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffStlStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalBlkAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffBlkStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalTovAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffToStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalFoulsAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFlsStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalPointsAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffPtsStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

}
