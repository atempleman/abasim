import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { League } from '../_models/league';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { LeagueState } from '../_models/leagueState';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { Team } from '../_models/team';
import { TeamService } from '../_services/team.service';
import { CheckGame } from '../_models/checkGame';

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

  dayEntered = 0;
  dayForm: FormGroup;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private modalService: BsModalService, private adminService: AdminService,
              private teamService: TeamService, private fb: FormBuilder) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    });
  }

  gotToDraft() {
    this.router.navigate(['/admindraft']);
  }

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
}
