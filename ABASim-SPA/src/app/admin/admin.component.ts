import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { League } from '../_models/league';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { LeagueState } from '../_models/leagueState';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { Team } from '../_models/team';
import { TeamService } from '../_services/team.service';
import { CheckGame } from '../_models/checkGame';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { NgxSpinnerService } from 'ngx-spinner';
import { DraftService } from '../_services/draft.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  league: League;
  public modalRef: BsModalRef;

  statusSelection: number;
  currentStatus = '';
  leagueStates: LeagueState[] = [];
  // statusForm: FormGroup;
  gamesAllRun = 0;
  rolloverResult = false;

  // removeRegoForm: FormGroup;
  teams: Team[] = [];
  teamSelected: number;

  run = 0;
  progress = 0;

  dayEntered = 0;
  dayForm: FormGroup;

  todaysGames: GameDisplayCurrent[] = [];

  username = 0;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private modalService: BsModalService, private adminService: AdminService,
              private teamService: TeamService, private fb: FormBuilder, private spinner: NgxSpinnerService,
              private draftService: DraftService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    });

    this.getTodaysGames();

    this.username = +this.authService.decodedToken.nameid;
    console.log(this.username);
  }

  getTodaysGames() {
    this.adminService.getGamesForReset().subscribe(result => {
      this.todaysGames = result;
    }, error => {
      this.alertify.error('Error getting todays games');
    });
  }

  public openConfirm(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  // State 2
  moveToInitialLottery() {
    this.adminService.updateLeagueStatus(2).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = this.statusSelection;
    });
  }

  // Running lottery and changing to state 3
  runInitialDraftLottery() {
    this.adminService.runInitialDraftLottery().subscribe(result => {
    }, error => {
      this.alertify.error('Error running initial draft lottery');
    }, () => {
      this.alertify.success('Initial Draft Lottery Run successfully');
      this.league.stateId = 3;
      this.modalRef.hide();
    });
  }

  // Begin the draft
  beginDraft() {
    this.draftService.beginInitialDraft().subscribe(result => {
    }, error => {
      this.alertify.error('Error starting the draft');
    }, () => {
      this.alertify.success('Initial Draft has begun!');
      this.league.stateId = 4;
      this.modalRef.hide();
    });
  }

  // Begin the preseason
  beginPreseason() {
    this.adminService.updateLeagueStatus(6).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = 6;
    });
  }

  beginSeason() {
    this.adminService.updateLeagueStatus(7).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = 7;
    });
  }


  //#region Super Admin Functions

  resetLeague() {
    this.adminService.resetLeague().subscribe(result => {

    }, error => {
      this.alertify.error('Error resetting the league.');
    }, () => {
      this.league.stateId = 1;
      this.modalRef.hide();
      this.alertify.success('League has been completely reset');
    });
  }

  //#endregion

























  runEngine() {
    this.router.navigate(['/admintestengine']);
  }

  // League Status
  public openModal(template: TemplateRef<any>, selection: number) {
    if (selection === 0) {
      // League status
      this.getLeagueStatusData();
    } else if (selection === 1) {
      // Remove team rego
      this.getRemoveTeamRegoData();
    } else if (selection === 2) {
      this.rollOverDay();
    } else if (selection === 3) {
      this.dayForm = this.fb.group({
        updateDay: ['', Validators.required]
      });
    }
    this.modalRef = this.modalService.show(template);
  }

  beginPlayoffs() {
    this.adminService.beginPlayoffs().subscribe(result => {

    }, error => {
      this.alertify.error('Error beginning the playoffs');
    }, () => {
      this.alertify.success('Playoffs have been setup');
      this.league.stateId = 8;
      this.league.day = 0;
      this.modalRef.hide();
    });
  }

  beginConfSemis() {
    this.adminService.beginConfSemis().subscribe(result => {

    }, error => {
      this.alertify.error('Error beginning the playoffs');
    }, () => {
      this.alertify.success('Playoffs have been setup');
      this.league.stateId = 9;
      this.league.day = 8;
      this.modalRef.hide();
    });
  }

  beginConfFinals() {
    this.adminService.beginConfFinals().subscribe(result => {

    }, error => {
      this.alertify.error('Error beginning the playoffs');
    }, () => {
      this.alertify.success('Playoffs have been setup');
      this.league.stateId = 10;
      this.league.day = 15;
      this.modalRef.hide();
    });
  }

  beginFinals() {
    this.adminService.beginFinals().subscribe(result => {

    }, error => {
      this.alertify.error('Error beginning the playoffs');
    }, () => {
      this.alertify.success('Playoffs have been setup');
      this.league.stateId = 11;
      this.league.day = 22;
      this.modalRef.hide();
    });
  }

  getLeagueStatusData() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });

    this.leagueService.getLeagueStatuses().subscribe(result => {
      this.leagueStates = result;
    }, error => {
      this.alertify.error('Error getting league statuses');
    }, () => {
      this.currentStatus = this.leagueStates.find(x => x.id === this.league.stateId).state;
    });
  }

  updateLeagueStatus() {
    this.adminService.updateLeagueStatus(this.statusSelection).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = this.statusSelection;
      if (this.statusSelection === 7) {
        this.league.day = 0;
      }

      this.league.state = this.getLeagueStateForId(this.statusSelection);
    });
  }

  getLeagueStateForId(id: number) {
    const state = this.leagueStates.find(x => x.id === +id);
    return state.state;
  }

  getRemoveTeamRegoData() {
    this.teamService.getAllTeams().subscribe(result => {
      this.teams = result;
    }, error => {
      this.alertify.error('Error getting all teams');
    });
  }

  removeTeamRegistration() {
    console.log(this.teamSelected);
    this.adminService.removeTeamRegistration(this.teamSelected).subscribe(result => {
    }, error => {
      this.alertify.error('Error updating team registration');
    }, () => {
      this.alertify.success('Team Rego updated.');
      this.modalRef.hide();
    });
  }

  rollOverDay() {
    // tslint:disable-next-line: prefer-const
    let value = false;
    this.adminService.checkAllGamesRun().subscribe(result => {
      value = result;
    }, error => {
      this.alertify.error('Error checking if games are run');
    }, () => {
      console.log(value);
      if (value) {
        // Now run the roll over process
        this.alertify.success('Games are run');
        this.gamesAllRun = 1;
      } else {
        this.alertify.error('Not all games are run');
      }
    });
  }

  confirmRollOverDay() {
    this.run = 1;
    this.adminService.rolloverDay().subscribe(result => {
      this.rolloverResult = result;
    }, error => {
      this.alertify.error('Error rolling over day');
    }, () => {
      if (this.rolloverResult) {
        this.alertify.success('Day Rolled over successfully');
        this.modalRef.hide();
        this.run = 0;
        this.league.day = this.league.day + 1;
      } else {
        this.alertify.error('Error rolling over day');
      }
    });
  }

  changeDay() {
    this.dayEntered = this.dayForm.controls.updateDay.value;
    console.log(this.dayEntered);

    let value = false;

    // Now need to call service to perform the change
    this.adminService.changeDay(this.dayEntered).subscribe(result => {
      value = result;
    }, error => {
      this.alertify.error('Error changing the current day');
    }, () => {
      if (value) {
        this.alertify.success('Day has been changed');
        this.modalRef.hide();
        this.league.day = this.dayEntered;
      } else {
        this.alertify.error('Error changing the current day');
      }
    });
  }

  runDraftPicks() {
    this.adminService.runTeamDraftPicksSetup().subscribe(result => {

    }, error => {
      this.alertify.error('Error running team pick setup');
    }, () => {
      this.alertify.success('Team pick setup run successfully');
      this.modalRef.hide();
    });
  }

  generateControcts() {
    this.adminService.generateInitalContracts().subscribe(result => {

    }, error => {
      this.alertify.error('Error generating contracts');
    }, () => {
      this.alertify.success('Contracts generated');
      this.modalRef.hide();
    });
  }

  generateSalaryCaps() {
    this.adminService.generateInitialSalaryCaps().subscribe(result => {

    }, error => {
      this.alertify.error('Error generating salary caps');
    }, () => {
      this.alertify.success('Salary caps updated');
      this.modalRef.hide();
    });
  }

  generateAutoPick() {
    this.adminService.generateAutoPicks().subscribe(result => {

    }, error => {
      this.alertify.error('Error generating auto picks');
    }, () => {
      this.alertify.success('Autopicks generated');
      this.modalRef.hide();
    });
  }

  resetGame(gameId: number) {
    this.adminService.resetGame(gameId).subscribe(result => {

    }, error => {
      this.alertify.error('Error resetting game');
    }, () => {
      this.alertify.success('Game has been reset');
      this.getTodaysGames();
      this.modalRef.hide();

    });
  }

  rolloverToNextSeason() {
    this.spinner.show();

    this.adminService.rolloverSeasonStats().subscribe(result => {
    }, error => {
      this.alertify.error('Error rolling over season stats');
    }, () => {
      this.progress = 10;
      this.rolloverAwardWinners();
    });
  }

  rolloverAwardWinners() {
    this.adminService.rolloverAwardWinners().subscribe(result => {
    }, error => {
      this.alertify.error('Error rolling over historical records and award winners');
    }, () => {
      this.progress = 20;
      this.rolloverContractUpdates();
    });
  }

  rolloverContractUpdates() {
    this.adminService.rolloverContractUpdates().subscribe(result => {
    }, error => {
      this.alertify.error('Error rolling over contracts');
    }, () => {
      this.progress = 30;
      this.generateDraft();
    });
  }

  generateDraft() {
    this.adminService.generateDraft().subscribe(result => {
    }, error => {
      this.alertify.error('Error setting up draft');
    }, () => {
      this.progress = 40;
      this.deleteData();
    });
  }

  deleteData() {
    this.adminService.deletePreseasonAndPlayoffsData().subscribe(result => {
    }, error => {
      this.alertify.error('Error deleting preason and playoff data');
    }, () => {
      this.progress = 50;
      this.adminService.deleteAwardsData().subscribe(result => {

      }, error => {
        this.alertify.error('Error saving stats and deleting awards');
      }, () => {
        this.progress = 60;
        this.adminService.deleteOtherData().subscribe(result => {
        }, error => {
          this.alertify.error('Error deleting other data');
        }, () => {
          this.progress = 70;
          this.adminService.deleteTeamSettingsData().subscribe(result => {
          }, error => {
            this.alertify.error('Error deleting team settings data');
          }, () => {
            this.progress = 75;
            this.adminService.deleteSeasonData().subscribe(result => {
              this.alertify.error('Error deleting season data');
            }, () => {
              this.progress = 80;
              this.resetStandings();
            });
          });
        });
      });
    });
  }

  resetStandings() {
    this.adminService.resetStandings().subscribe(result => {
    }, error => {
      this.alertify.error('Error resetting standings');
    }, () => {
      this.progress = 90;
      this.rolloverLeague();
    });
  }

  endSeason() {
    this.adminService.endSeason().subscribe(result => {
    }, error => {
      this.alertify.error('Error ending season properly');
    }, () => {
      this.rolloverToNextSeason();
    });
  }

  rolloverLeague() {
    this.adminService.rolloverLeague().subscribe(result => {
    }, error => {
      this.alertify.error('Error rolling over league');
    }, () => {
      this.modalRef.hide();
      this.spinner.hide();
    });
  }
}
