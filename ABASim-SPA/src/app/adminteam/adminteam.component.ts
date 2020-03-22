import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';
import { AdminService } from '../_services/admin.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';

@Component({
  selector: 'app-adminteam',
  templateUrl: './adminteam.component.html',
  styleUrls: ['./adminteam.component.css']
})
export class AdminteamComponent implements OnInit {

  public modalRef: BsModalRef;
  statusForm: FormGroup;
  teams: Team[] = [];

  teamSelected: number;

  constructor(private alertify: AlertifyService, private adminService: AdminService,
              private fb: FormBuilder, private modalService: BsModalService, private teamService: TeamService) { }

  ngOnInit() {
    this.teamService.getAllTeams().subscribe(result => {
      this.teams = result;
    }, error => {
      this.alertify.error('Error getting all teams');
    });
  }

  public openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template); // {3}
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
