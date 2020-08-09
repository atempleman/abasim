import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { ContactService } from '../_services/contact.service';
import { CountOfMessages } from '../_models/countOfMessages';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  public isCollapsed = true;
  isAdmin = 0;
  interval;
  refresh;
  checks = 0;

  league: League;
  countOfMessages: CountOfMessages;

  constructor(private router: Router, public authService: AuthService, private leagueService: LeagueService,
              private alertify: AlertifyService, private contactService: ContactService) { }

  ngOnInit() {
    this.interval = setInterval(() => {
      this.isAdmin = this.authService.isAdmin();
      this.checks++;
      if (this.isAdmin === 1 || this.checks === 5) {
        clearInterval(this.interval);
      }
    }, 5000);

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });

    this.checkMessages();
    this.refresh = setInterval(() => {
      this.checkMessages();
    }, 60000);
  }

  checkMessages() {
    this.contactService.getCountOfMessages(+localStorage.getItem('teamId')).subscribe(result => {
      this.countOfMessages = result;
    }, error => {
      this.alertify.error('Error getting inbox message count');
    });
  }

  contactus() {
    this.router.navigate(['/contact']);
  }

  goToDashboard() {
    this.router.navigate(['/dashboard']);
  }

  loggedIn() {
    // console.log('hitting logged in on auth service');
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('isAdmin');
    localStorage.removeItem('teamId');
    localStorage.clear();
    this.router.navigate(['']);
  }
}
