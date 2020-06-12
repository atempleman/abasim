import { Component, OnInit, TemplateRef } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AdminService } from '../_services/admin.service';
import { AlertifyService } from '../_services/alertify.service';
import { LeagueState } from '../_models/leagueState';
import { FormGroup, FormBuilder } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';

@Component({
  selector: 'app-adminleague',
  templateUrl: './adminleague.component.html',
  styleUrls: ['./adminleague.component.css']
})
export class AdminleagueComponent implements OnInit {
  public modalRef: BsModalRef;

  // leagueStatusModalDisplay = 0;
  cardDisplay = 1;

  statusSelection: number;
  currentStatus = '';

  league: League;
  leagueStates: LeagueState[] = [];

  statusForm: FormGroup;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private adminService: AdminService,
              private fb: FormBuilder, private modalService: BsModalService) { }

  ngOnInit() {
  }

  public openModal(template: TemplateRef<any>) {
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

    this.modalRef = this.modalService.show(template); // {3}
  }

  updateLeagueStatus() {
    this.adminService.updateLeagueStatus(this.statusSelection).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving league status');
    }, () => {
      this.alertify.success('League Status updated.');
      this.modalRef.hide();
      this.league.stateId = this.statusSelection;
      console.log(this.league.state);
      
      console.log(this.league.state);
    });
  }

  
}
