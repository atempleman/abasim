import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SimGame } from '../_models/simGame';
import { GameEngineService } from '../_services/game-engine.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-admintestengine',
  templateUrl: './admintestengine.component.html',
  styleUrls: ['./admintestengine.component.css']
})
export class AdmintestengineComponent implements OnInit {
  gameSetupForm: FormGroup;
  gameModel: any = {};
  commentaryData: string[] = [];

  homeId = 0;
  awayId = 0;

  setupDisplay = 1;
  detailsDisplay = 0;
  dataToDisplay = 0;

  constructor(private fb: FormBuilder, private engineService: GameEngineService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.createRunGameForm();
  }

  createRunGameForm() {
    this.gameSetupForm = this.fb.group({
      awayid: ['', Validators.required],
      homeid: ['', Validators.required]
    });
  }

  gameSetup() {
    const awayKey = 'awayid';
    const homeKey = 'homeid';
    this.setupDisplay = 0;
    this.detailsDisplay = 1;

    const simGame: SimGame = {
      awayId:  +this.gameSetupForm.controls[awayKey].value,
      homeId:  +this.gameSetupForm.controls[homeKey].value,
      gameId:  1
    };

    this.engineService.startTestGame(simGame).subscribe(result => {
      // console.log(result);

      Object.assign(this.commentaryData, result);
      
    }, error => {
      this.alertify.error('Error starting game');
    }, () => {
      // this.alertify.success('League Status updated.');
      console.log(this.commentaryData);
      console.log(this.commentaryData.length);
      this.dataToDisplay = 1;
      console.log(this.commentaryData[0]);
    });

  }

}
