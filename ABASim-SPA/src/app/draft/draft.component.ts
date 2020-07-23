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
import { AdminService } from '../_services/admin.service';

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

  onClockLoaded = 0;

  timeRemaining: number;
  timeDisplay: string;
  interval;
  pageInterval;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private router: Router,
              private draftService: DraftService, private teamService: TeamService, private authService: AuthService,
              private modalService: BsModalService, private playerService: PlayerService, private adminService: AdminService) { }

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

    this.pageInterval = setInterval(() => {
      this.getDraftDetails();
      this.getDraftTracker();
    }, 10000);
  }

  counter(i: number) {
    return new Array(i);
  }

  getTeamOnClock() {
    const pick = this.tracker.pick;
    const round = this.tracker.round;
    const dp = this.draftPicks.find(x => x.pick === pick && x.round === round);
    if (dp) {
      return ' - ' + dp.teamName + ' are on the clock';
    } else {
      return '';
    }
  }

  timerDisplay() {
    this.interval = setInterval(() => {
      const time = this.tracker.dateTimeOfLastPick + ' UTC';
      const dtPick = new Date(time);
      const currentTime = new Date();

      this.timeRemaining =  dtPick.getTime() - currentTime.getTime();
      const value = (this.timeRemaining / 1000).toFixed(0);
      this.timeRemaining = +value;

      if (this.timeRemaining > 0) {
        this.timeRemaining--;
        this.timeDisplay = this.transform(this.timeRemaining);
      } else {
        this.timeRemaining = 0;
        this.timeDisplay = 'Time Expired';
      }
    }, 1000);
  }

  transform(value: number): string {
    const minutes: number = Math.floor(value / 60);
    return minutes + ':' + (value - minutes * 60);
 }

  getDraftTracker() {
    // Get the draft tracker
    this.draftService.getDraftTracker().subscribe(result => {
      this.tracker = result;
    }, error => {
      this.alertify.error('Error getting draft tracker');
    }, () => {
      this.onClockLoaded++;
      this.timerDisplay();
    });
  }

  getDraftDetails() {
    // Get the Initial Draft Details
    this.draftService.getDraftPicksForRound(this.currentRound).subscribe(result => {
      this.draftPicks = result;
    }, error => {
      this.alertify.error('Error getting Draft Picks');
    }, () => {
      this.onClockLoaded++;
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

      if (this.tracker.round === 13 && this.tracker.pick === 30) {
        // Update the leage state here
        this.adminService.updateLeagueStatus(5).subscribe(result => {
        }, error => {
          this.alertify.error('Error changing league state');
        }, () => {
          this.alertify.success('Draft Completed');
        });
      } else {
        this.getDraftTracker();
      }
    });
   }

   autoPickAction() {
    console.log('Auto-picking');
    const selectedPick: DraftSelection = {
      pick: this.tracker.pick,
      playerId: 0,
      round: this.tracker.round,
      teamId: 0
    };

    this.draftService.makeAutoPick(selectedPick).subscribe(result => {
    }, error => {
      this.alertify.error('Error making pick');
    }, () => {
      this.getDraftDetails();

      if (this.tracker.round === 13 && this.tracker.pick === 30) {
        // Update the leage state here
        this.adminService.updateLeagueStatus(5).subscribe(result => {
        }, error => {
          this.alertify.error('Error changing league state');
        }, () => {
          this.alertify.success('Draft Completed');
        });
      } else {
        this.getDraftTracker();
      }
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
