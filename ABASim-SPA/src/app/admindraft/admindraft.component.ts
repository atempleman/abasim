import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';
import { AdminService } from '../_services/admin.service';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { DraftService } from '../_services/draft.service';

@Component({
  selector: 'app-admindraft',
  templateUrl: './admindraft.component.html',
  styleUrls: ['./admindraft.component.css']
})
export class AdmindraftComponent implements OnInit {

  public modalRef: BsModalRef;
  statusForm: FormGroup;
  league: League;

  constructor(private alertify: AlertifyService, private adminService: AdminService,
              private fb: FormBuilder, private modalService: BsModalService, private leagueService: LeagueService,
              private draftService: DraftService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });
  }

  public openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  beginRunningDraft() {
    this.draftService.beginInitialDraft().subscribe(result => {

    }, error => {
      this.alertify.error('Error starting the draft');
    }, () => {
      this.alertify.success('Initial Draft has begun!');
      this.modalRef.hide();
    });
  }

  runInitialDraftLottery() {
    this.adminService.runInitialDraftLottery().subscribe(result => {
    }, error => {
      this.alertify.error('Error running initial draft lottery');
    }, () => {
      this.alertify.success('Initial Draft Lottery Run successfully');
      this.modalRef.hide();
    });
  }
}
