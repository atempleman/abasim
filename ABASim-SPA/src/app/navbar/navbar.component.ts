import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  public isCollapsed = true;

  constructor(private router: Router, public authService: AuthService) { }

  ngOnInit() {
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
