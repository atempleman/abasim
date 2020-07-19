import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { DraftService } from '../_services/draft.service';
import { InitialDraftPicks } from '../_models/initialDraftPicks';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { DraftPick } from '../_models/draftPick';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-draft',
  templateUrl: './draft.component.html',
  styleUrls: ['./draft.component.css']
})
export class DraftComponent implements OnInit {
  league: League;
  draftPicks: DraftPick[] = [];


  allDraftPicks: InitialDraftPicks[] = [];
  currentRound = 1;
  roundDraftPicks: InitialDraftPicks[] = [];

  allTeams: Team[] = [];

  roundPicks: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 , 30];
  loaded = 0;
  isAdmin = 0;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private router: Router,
              private draftService: DraftService, private teamService: TeamService, private authService: AuthService) { }

  ngOnInit() {
    this.isAdmin = +localStorage.getItem('isAdmin');

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.loaded = 1;
      if (this.league.stateId === 3) {
        this.getDraftDetails();
      }
    });
    // this.teamService.getAllTeams().subscribe(result => {
    //   this.allTeams = result;
    // }, error => {
    //   this.alertify.error('Error getting all teams');
    // }, () => {
    // });
  }

  counter(i: number) {
    return new Array(i);
  }

  getDraftDetails() {
    // Get the Initial Draft Details
    this.draftService.getDraftPicksForRound(this.currentRound).subscribe(result => {
      this.draftPicks = result;
    }, error => {
      this.alertify.error('Error getting Draft Picks');
    });
  }

  beginDraft() {
    this.draftService.beginInitialDraft().subscribe(result => {
    }, error => {
      this.alertify.error('Error starting the draft');
    }, () => {
      this.alertify.success('Initial Draft has begun!');
    });
  }

  getTeamNameForSelection(round: number, pick: number) {
    const selection = this.allDraftPicks.find(x => x.pick === pick && x.round === round);
    const teamSelecting = this.allTeams.find(x => x.id === selection.teamId);
    return teamSelecting.teamname + ' ' + teamSelecting.mascot;
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
