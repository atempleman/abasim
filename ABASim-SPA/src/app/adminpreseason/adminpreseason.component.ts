import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormGroup, FormBuilder } from '@angular/forms';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { AdminService } from '../_services/admin.service';
import { LeagueService } from '../_services/league.service';

@Component({
  selector: 'app-adminpreseason',
  templateUrl: './adminpreseason.component.html',
  styleUrls: ['./adminpreseason.component.css']
})
export class AdminpreseasonComponent implements OnInit {
  public modalRef: BsModalRef;
  statusForm: FormGroup;
  league: League;

  constructor(private alertify: AlertifyService, private adminService: AdminService,
              private fb: FormBuilder, private modalService: BsModalService, private leagueService: LeagueService) { }

  ngOnInit() {
  }

}
