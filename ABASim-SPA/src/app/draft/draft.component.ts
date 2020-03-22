import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-draft',
  templateUrl: './draft.component.html',
  styleUrls: ['./draft.component.css']
})
export class DraftComponent implements OnInit {
  league: League;
  lotteryDisplayed = 0;
  playerPoolDisplayed = 0;
  rankingsDisplayed = 0;
  draftDisplayed = 1;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
      console.log(this.league);
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    //  this.getCurrentLeagueState();
    });
  }

  playerPoolClicked() {
    this.draftDisplayed = 0;
    this.lotteryDisplayed = 0;
    this.rankingsDisplayed = 0;
    this.playerPoolDisplayed = 1;
  }

  rankingsClicked() {
    this.draftDisplayed = 0;
    this.lotteryDisplayed = 0;
    this.rankingsDisplayed = 1;
    this.playerPoolDisplayed = 0;
  }

  lotteryClicked() {
    this.draftDisplayed = 0;
    this.lotteryDisplayed = 1;
    this.rankingsDisplayed = 0;
    this.playerPoolDisplayed = 0;
  }

  draftClicked() {
    this.draftDisplayed = 1;
    this.lotteryDisplayed = 0;
    this.rankingsDisplayed = 0;
    this.playerPoolDisplayed = 0;
  }

}
