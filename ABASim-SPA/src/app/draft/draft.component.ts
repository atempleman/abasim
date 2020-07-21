import { Component, OnInit, TemplateRef } from '@angular/core';
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
import { DraftTracker } from '../_models/draftTracker';
import { DraftPlayer } from '../_models/draftPlayer';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { PlayerService } from '../_services/player.service';
import { DraftSelection } from '../_models/draftSelection';

@Component({
  selector: 'app-draft',
  templateUrl: './draft.component.html',
  styleUrls: ['./draft.component.css']
})
export class DraftComponent implements OnInit {
  league: League;
  public modalRef: BsModalRef;
  draftPicks: DraftPick[] = [];
  teamOnClock: Team;
  team: Team;

  tracker: DraftTracker;

  allDraftPicks: InitialDraftPicks[] = [];
  currentRound = 1;
  roundDraftPicks: InitialDraftPicks[] = [];
  draftablePlayers: DraftPlayer[] = [];

  allTeams: Team[] = [];

  loaded = 0;
  isAdmin = 0;
  teamId = 0;
  draftSelection = 0;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private router: Router,
              private draftService: DraftService, private teamService: TeamService, private authService: AuthService,
              private modalService: BsModalService, private playerService: PlayerService) { }

  ngOnInit() {
    this.isAdmin = +localStorage.getItem('isAdmin');
    this.teamId = +localStorage.getItem('teamId');

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.loaded = 1;
      if (this.league.stateId === 3) {
        this.getDraftDetails();
      } else if (this.league.stateId === 4) {
        this.getDraftDetails();
        this.getDraftTracker();
      }
    });

    this.teamService.getTeamForTeamId(this.teamId).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    });

    this.playerService.getAllInitialDraftPlayers().subscribe(result => {
      this.draftablePlayers = result;
    }, error => {
      this.alertify.error('Error getting available players to draft');
    }, () => {
    });
  }

  counter(i: number) {
    return new Array(i);
  }

  getDraftTracker() {
    // Get the draft tracker
    this.draftService.getDraftTracker().subscribe(result => {
      this.tracker = result;
    }, error => {
      this.alertify.error('Error getting draft tracker');
    }, () => {
    });
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
      this.league.stateId = 4;
    });
  }

  public openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  makeDraftPick() {
    console.log(this.draftSelection);
    const selectedPick: DraftSelection = {
      pick: this.tracker.pick,
      playerId: +this.draftSelection,
      round: this.tracker.round,
      teamId: this.team.id
    };

    this.draftService.makeDraftPick(selectedPick).subscribe(result => {
    }, error => {
      this.alertify.error('Error making pick');
    }, () => {
      this.modalRef.hide();
      this.getDraftDetails();
      this.getDraftTracker();
    });
   }

  // getTeamOnTheClock() {
  //   const draftPick = this.allDraftPicks.find(x => x.pick === this.tracker.pick && x.round === this.tracker.round);
  //   this.teamService.getTeamForTeamId(draftPick.teamId).subscribe(result => {
  //     this.teamOnClock = result;
  //   }, error => {
  //     this.alertify.error('Error getting team on the clock');
  //   });
  // }

  // getTeamNameForSelection(round: number, pick: number) {
  //   const selection = this.allDraftPicks.find(x => x.pick === pick && x.round === round);
  //   const teamSelecting = this.allTeams.find(x => x.id === selection.teamId);
  //   return teamSelecting.teamname + ' ' + teamSelecting.mascot;
  // }

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
