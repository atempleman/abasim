import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { League } from '../_models/league';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { LeagueState } from '../_models/leagueState';
import { FormGroup } from '@angular/forms';
import { AdminService } from '../_services/admin.service';
import { Team } from '../_models/team';
import { TeamService } from '../_services/team.service';

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

  // removeRegoForm: FormGroup;
  teams: Team[] = [];
  teamSelected: number;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private modalService: BsModalService, private adminService: AdminService,
              private teamService: TeamService) { }

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
    console.log(selection);

    if (selection === 0) {
      // League status
      this.getLeagueStatusData();
    } else if (selection === 1) {
      // Remove team rego
      this.getRemoveTeamRegoData();
    }
    this.modalRef = this.modalService.show(template);
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

}
