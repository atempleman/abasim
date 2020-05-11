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
    });
  }

  homeBoxScore() {
    this.homeBoxScoreDisplay = 1;
  }

  awayBoxScore() {
    this.awayBoxScoreDisplay = 1;
  }

}
