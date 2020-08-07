import { Component, OnInit, TemplateRef } from '@angular/core';
import { League } from '../_models/league';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { InboxMessage } from '../_models/inboxMessage';
import { ContactService } from '../_services/contact.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';

@Component({
  selector: 'app-inbox',
  templateUrl: './inbox.component.html',
  styleUrls: ['./inbox.component.css']
})
export class InboxComponent implements OnInit {
  league: League;
  team: Team;
  messages: InboxMessage[] = [];
  public modalRef: BsModalRef;
  messageState = 0;
  viewedMessage: InboxMessage;
  teams: Team[] = [];
  selectedTeam: Team;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private teamService: TeamService,
              private authService: AuthService, private contactService: ContactService, private modalService: BsModalService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, () => {
      this.alertify.error('Error getting league details');
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
      // Need to persist the team to cookie
      localStorage.setItem('teamId', this.team.id.toString());
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.getMessages();
    });

    this.teamService.getAllTeams().subscribe(result => {
      this.teams = result;
    }, error => {
      this.alertify.error('Error getting teams');
    });
  }

  getMessages() {
    this.contactService.getInboxMessages(this.team.id).subscribe(result => {
      this.messages = result;
    }, error => {
      this.alertify.error('Error getting your messages');
    });
  }

  deleteMessage(message: number) {
    this.contactService.deleteInboxMessage(message).subscribe(result => {

    }, error => {
      this.alertify.error('Error deleting message');
    });
  }

  deleteMessageFromModal() {
    this.contactService.deleteInboxMessage(this.viewedMessage.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error deleting message');
    }, () => {
      this.getMessages();
      this.modalRef.hide();
    });
  }

  public openModal(template: TemplateRef<any>, message: InboxMessage) {
    this.messageState = 1;
    this.viewedMessage = message;
    this.modalRef = this.modalService.show(template);
  }

  replyAction() {
    this.messageState = 2;
  }

  updateMessageForm() {
    // this.selectedTeam
    // now need to get the subject and body values
    
  }

  cancelReply() {
    this.messageState = 1;
  }
}
