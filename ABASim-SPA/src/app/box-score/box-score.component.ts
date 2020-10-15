import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { TransferService } from '../_services/transfer.service';
import { GameDetails } from '../_models/gameDetails';
import { GameEngineService } from '../_services/game-engine.service';
import { BoxScore } from '../_models/boxScore';
import { League } from '../_models/league';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
import { PlayerService } from '../_services/player.service';
import { Player } from '@angular/core/src/render3/interfaces/player';

@Component({
  selector: 'app-box-score',
  templateUrl: './box-score.component.html',
  styleUrls: ['./box-score.component.css']
})
export class BoxScoreComponent implements OnInit {
  gameId: number;
  gameDetails: GameDetails;
  boxScores: BoxScore[] = [];
  homeBoxScores: BoxScore[] = [];
  awayBoxScores: BoxScore[] = [];
  league: League;

  // Totals
  awayPoints = 0;
  awayRebounds = 0;
  awayAssists = 0;
  awaySteals = 0;
  awayBlocks = 0;
  awayMinutes = 0;
  awayBlockedAttempts = 0;
  awayTurnovers = 0;
  awayFouls = 0;
  awayFGA = 0;
  awayFGM = 0;
  awayFTM = 0;
  awayFTA = 0;
  away3FGA = 0;
  away3FGM = 0;
  homePoints = 0;
  homeRebounds = 0;
  homeAssists = 0;
  homeSteals = 0;
  homeBlocks = 0;
  homeMinutes = 0;
  homeBlockedAttempts = 0;
  homeTurnovers = 0;
  homeFouls = 0;
  homeFGA = 0;
  homeFGM = 0;
  homeFTM = 0;
  homeFTA = 0;
  home3FGA = 0;
  home3FGM = 0;

  constructor(private alertify: AlertifyService, private authService: AuthService, private leagueService: LeagueService,
              private transferService: TransferService, private engineService: GameEngineService, private spinner: NgxSpinnerService,
              private router: Router, private playerService: PlayerService) { }

  ngOnInit() {
    this.gameId = this.transferService.getData();

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    }, () => {
      this.getGameDetails();
    });
  }

  getGameDetails() {
    if (this.league.stateId === 6) {
      this.spinner.show();
      this.leagueService.getGameDetailsPreseason(this.gameId).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      }, () => {
        this.retrieveBoxScoreData();
        this.calculateTeamTotals();
        this.spinner.hide();
      });
    } else if (this.league.stateId === 7) {
      this.spinner.show();
      this.leagueService.getGameDetailsSeason(this.gameId).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
        this.spinner.hide();
      }, () => {
        this.retrieveBoxScoreData();
        this.calculateTeamTotals();
        this.spinner.hide();
      });
    } else if (this.league.stateId === 8 || this.league.stateId === 9 || this.league.stateId === 10 || this.league.stateId === 11) {
      this.spinner.show();
      this.leagueService.getGameDetailsPlayoffs(this.gameId).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
        this.spinner.hide();
      }, () => {
        this.retrieveBoxScoreData();
        this.calculateTeamTotals();
        this.spinner.hide();
      });
    }
  }

  retrieveBoxScoreData() {
    if (this.league.stateId === 8 || this.league.stateId === 9 || this.league.stateId === 10 || this.league.stateId === 11) {
      this.engineService.getBoxScoreForGameIdPlayoffs(this.gameId).subscribe(result => {
        this.boxScores = result;
      }, error => {
        this.alertify.error('Wrror getting box scores');
      }, () => {
        this.boxScores = this.boxScores.sort((a, b) => a.minutes < b.minutes ? 1 : a.minutes > b.minutes ? -1 : 0);
        this.homeBoxScores = this.boxScores.filter(bs => bs.teamId === this.gameDetails.homeTeamId);
        this.awayBoxScores = this.boxScores.filter(bs => bs.teamId === this.gameDetails.awayTeamId);
        this.calculateTeamTotals();
      });
    } else {
      this.engineService.getBoxScoreForGameId(this.gameId).subscribe(result => {
        this.boxScores = result;
      }, error => {
        this.alertify.error('Wrror getting box scores');
      }, () => {
        this.boxScores = this.boxScores.sort((a, b) => a.minutes < b.minutes ? 1 : a.minutes > b.minutes ? -1 : 0);
        this.homeBoxScores = this.boxScores.filter(bs => bs.teamId === this.gameDetails.homeTeamId);
        this.awayBoxScores = this.boxScores.filter(bs => bs.teamId === this.gameDetails.awayTeamId);

        this.calculateTeamTotals();
      });
    }
  }

  calculateTeamTotals() {
    this.awayBoxScores.forEach(bs => {
      this.awayFGA = this.awayFGA + bs.fga;
      this.awayFGM = this.awayFGM + bs.fgm;
      this.awayFTM = this.awayFTM + bs.ftm;
      this.awayFTA = this.awayFTA + bs.fta;
      this.awayPoints = this.awayPoints + bs.points;
      this.awayRebounds = this.awayRebounds + bs.rebounds;
      this.awayAssists = this.awayAssists + bs.assists;
      this.awaySteals = this.awaySteals + bs.steals;
      this.awayBlocks = this.awayBlocks + bs.blocks;
      this.awayBlockedAttempts = this.awayBlockedAttempts + bs.blockedAttempts;
      this.awayMinutes = this.awayMinutes + bs.minutes;
      this.awayTurnovers = this.awayTurnovers + bs.turnovers;
      this.awayFouls = this.awayFouls + bs.fouls;
      this.away3FGA = this.away3FGA + bs.threeFGA;
      this.away3FGM = this.away3FGM + bs.threeFGM;
    });

    this.homeBoxScores.forEach(bs => {
      this.homeFGA = this.homeFGA + bs.fga;
      this.homeFGM = this.homeFGM + bs.fgm;
      this.homeFTM = this.homeFTM + bs.ftm;
      this.homeFTA = this.homeFTA + bs.fta;
      this.homePoints = this.homePoints + bs.points;
      this.homeRebounds = this.homeRebounds + bs.rebounds;
      this.homeAssists = this.homeAssists + bs.assists;
      this.homeSteals = this.homeSteals + bs.steals;
      this.homeBlocks = this.homeBlocks + bs.blocks;
      this.homeBlockedAttempts = this.homeBlockedAttempts + bs.blockedAttempts;
      this.homeMinutes = this.homeMinutes + bs.minutes;
      this.homeTurnovers = this.homeTurnovers + bs.turnovers;
      this.homeFouls = this.homeFouls + bs.fouls;
      this.home3FGA = this.home3FGA + bs.threeFGA;
      this.home3FGM = this.home3FGM + bs.threeFGM;
    });
  }

  viewPlayer(firstName: string, lastName: string) {
    // Need to get player id for name
    let playerId = 0;
    const playerName = firstName + ' ' + lastName;
    this.playerService.getPlayerForName(playerName).subscribe(result => {
      playerId = result.id;
    }, error => {
      this.alertify.error('Error getting player name');
    }, () => {
      this.transferService.setData(playerId);
      this.router.navigate(['/view-player']);
    });
  }
}
