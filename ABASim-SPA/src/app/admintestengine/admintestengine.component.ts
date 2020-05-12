import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SimGame } from '../_models/simGame';
import { GameEngineService } from '../_services/game-engine.service';
import { AlertifyService } from '../_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { BoxScore } from '../_models/boxScore';

@Component({
  selector: 'app-admintestengine',
  templateUrl: './admintestengine.component.html',
  styleUrls: ['./admintestengine.component.css']
})
export class AdmintestengineComponent implements OnInit {
  gameSetupForm: FormGroup;
  gameModel: any = {};
  commentaryData: string[] = [];
  boxScores: BoxScore[] = [];
  homeBoxScores: BoxScore[] = [];
  awayBoxScores: BoxScore[] = [];

  homeId = 0;
  awayId = 0;
  gameId = 0;

  setupDisplay = 1;
  detailsDisplay = 0;
  dataToDisplay = 0;
  boxScoreButtonsDisplay = 0;
  homeBoxScoreDisplay = 0;
  awayBoxScoreDisplay = 0;

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

  constructor(private fb: FormBuilder, private engineService: GameEngineService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.createRunGameForm();
  }

  createRunGameForm() {
    this.gameSetupForm = this.fb.group({
      gameid: ['', Validators.required],
      awayid: ['', Validators.required],
      homeid: ['', Validators.required]
    });
  }

  gameSetup() {
    const awayKey = 'awayid';
    const homeKey = 'homeid';
    const gameKey = 'gameid';
    this.setupDisplay = 0;
    this.detailsDisplay = 1;

    const simGame: SimGame = {
      awayId:  +this.gameSetupForm.controls[awayKey].value,
      homeId:  +this.gameSetupForm.controls[homeKey].value,
      gameId:  +this.gameSetupForm.controls[gameKey].value,
    };

    this.homeId = +this.gameSetupForm.controls[homeKey].value;
    this.awayId = +this.gameSetupForm.controls[awayKey].value;
    this.gameId = +this.gameSetupForm.controls[gameKey].value;

    this.engineService.startTestGame(simGame).subscribe(result => {
      Object.assign(this.commentaryData, result);
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.dataToDisplay = 1;
      this.boxScoreButtonsDisplay = 1;
      this.retrieveBoxScoreData();
    });
  }

  retrieveBoxScoreData() {
    console.log('here');
    this.engineService.getBoxScoreForGameId(this.gameId).subscribe(result => {
      this.boxScores = result;
      console.log(this.boxScores);
      console.log(this.boxScores[0].fga);
    }, error => {
      this.alertify.error('Wrror getting box scores');
    }, () => {
      this.boxScores = this.boxScores.sort((a, b) => a.minutes < b.minutes ? 1 : a.minutes > b.minutes ? -1 : 0);
      this.homeBoxScores = this.boxScores.filter(bs => bs.teamId === this.homeId);
      this.awayBoxScores = this.boxScores.filter(bs => bs.teamId === this.awayId);

      this.calculateTeamTotals();
    });
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

  homeBoxScore() {
    this.homeBoxScoreDisplay = 1;
  }

  awayBoxScore() {
    this.awayBoxScoreDisplay = 1;
  }

}
