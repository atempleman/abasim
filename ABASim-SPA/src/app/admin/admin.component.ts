import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

  goToLeagueAdmin() {
    this.router.navigate(['/adminleague']);
  }

  goToTeamAdmin() {
    this.router.navigate(['/adminteam']);
  }

  gotToDraft() {
    this.router.navigate(['/admindraft']);
  }

  goToPreseaon() {
    this.router.navigate(['/adminpreseason']);
  }

  runEngine() {
    this.router.navigate(['/admintestengine']);
  }

}
