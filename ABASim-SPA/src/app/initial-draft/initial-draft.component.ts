import { Component, OnInit, TemplateRef } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { DraftService } from '../_services/draft.service';
import { Team } from '../_models/team';
import { League } from '../_models/league';
import { LeagueService } from '../_services/league.service';
import { DraftTracker } from '../_models/draftTracker';
import { InitialDraftPicks } from '../_models/initialDraftPicks';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { DraftPlayer } from '../_models/draftPlayer';
import { DraftSelection } from '../_models/draftSelection';
import { Player } from '../_models/player';

@Component({
  selector: 'app-initial-draft',
  templateUrl: './initial-draft.component.html',
  styleUrls: ['./initial-draft.component.css']
})
export class InitialDraftComponent implements OnInit {
  team: Team;
  teamOnClock: Team;
  nextTeam: Team;
  allTeams: Team[] = [];
  league: League;
  tracker: DraftTracker;
  allDraftPicks: InitialDraftPicks[] = [];
  rounds: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 ];
  roundPicks: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 , 30];
  draftablePlayers: DraftPlayer[] = [];
  draftSelection = 0;
  loaded = 0;

  timeRemaining: number;
  interval;
  timeDisplay: string;

  isAdmin = 0;
  players: Player[] = [];

  constructor(private alertify: AlertifyService, private playerService: PlayerService, private teamService: TeamService,
              private authService: AuthService, private draftService: DraftService, private leagueService: LeagueService,
              private modalService: BsModalService) { }

  ngOnInit() {
    this.teamService.getAllTeams().subscribe(result => {
      this.allTeams = result;
    }, error => {
      this.alertify.error('Error getting all teams');
    }, () => {
      this.loaded++;
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.loaded++;
    });

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    }, () => {
      this.getDraftDetails();
    });

    this.isAdmin = this.authService.isAdmin();

    // this.playerService.getInitialDraftPlayers().subscribe(result => {
    //   this.draftablePlayers = result;
    // }, error => {
    //   this.alertify.error('Error getting available players to draft');
    // }, () => {
    //   this.loaded++;
    // });

    this.playerService.getAllPlayers().subscribe(result => {
      this.players = result;
    }, error => {
      this.alertify.error('Error getting players');
    });
  }

  refresh() {
    // this.playerService.getInitialDraftPlayers().subscribe(result => {
    //   this.draftablePlayers = result;
    // }, error => {
    //   this.alertify.error('Error getting available players to draft');
    // }, () => {
    //   this.loaded++;
    // });

    // this.getDraftDetails();
  }

  getDraftDetails() {
    // Get the draft tracker
    this.draftService.getDraftTracker().subscribe(result => {
      this.tracker = result;
    }, error => {
      this.alertify.error('Error getting draft tracker');
    }, () => {
    });

    // Get the Initial Draft Details
    this.draftService.getInitialDraftPicks().subscribe(result => {
      this.allDraftPicks = result;
      console.log(this.allDraftPicks);
    }, error => {
      this.alertify.error('Error getting Draft Picks');
    }, () => {
      this.loaded++;
      // Now need to setup the different sections to display:
      this.getTeamOnTheClock();
      this.getNextDraftingTeam();
      this.timerDisplay();
    });
  }

  getTeamOnTheClock() {
    const draftPick = this.allDraftPicks.find(x => x.pick === this.tracker.pick && x.round === this.tracker.round);
    this.teamService.getTeamForTeamId(draftPick.teamId).subscribe(result => {
      this.teamOnClock = result;
    }, error => {
      this.alertify.error('Error getting team on the clock');
    });
  }

  getNextDraftingTeam() {
    let draftOver = 0;
    let pick = this.tracker.pick + 1;
    let round = this.tracker.round;

    if (pick === 31) {
      if (round === 13) {
        // Draft is over
        draftOver = 1;
      } else {
        pick = 1;
        round = round + 1;
      }
    }

    if (draftOver !== 1) {
      const draftPick = this.allDraftPicks.find(x => x.pick === pick && x.round === round);
      this.teamService.getTeamForTeamId(draftPick.teamId).subscribe(result => {
        this.nextTeam = result;
      }, error => {
        this.alertify.error('Error getting team on the clock');
      });
    }
  }

  timerDisplay() {
    const time = this.tracker.dateTimeOfLastPick + ' UTC';
    const dtPick = new Date(time);
    const currentTime = new Date();

    this.interval = setInterval(() => {
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

 getTeamNameForSelection(round: number, pick: number) {
    const selection = this.allDraftPicks.find(x => x.pick === pick && x.round === round);
    const teamSelecting = this.allTeams.find(x => x.id === selection.teamId);
    return teamSelecting.teamname + ' ' + teamSelecting.mascot;
 }

 getPlayerSelected(round: number, pick: number) {
  const selection = this.allDraftPicks.find(x => x.pick === pick && x.round === round);

  if (selection.playerId !== 0) {
    const player = this.players.find(x => x.id === selection.playerId);
    return player.firstName + ' ' + player.surname;
  } else {
    return '';
  }
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
    this.refresh();
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
    this.refresh();
  });
 }
}
