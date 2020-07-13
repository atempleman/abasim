import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-draft',
  templateUrl: './draft.component.html',
  styleUrls: ['./draft.component.css']
})
export class DraftComponent implements OnInit {
  league: League;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
      console.log(this.league);
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    });
  }

  playerPoolClicked() {
    this.router.navigate(['/draftplayerpool']);
  }

  rankingsClicked() {
    this.router.navigate(['/draftboard']);
  }

  lotteryClicked() {
    this.router.navigate(['/initiallottery']);
  }
}
