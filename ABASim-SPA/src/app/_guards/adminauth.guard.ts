import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
    providedIn: 'root'
})
export class AdminAuthGuard implements CanActivate {

    constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {}

    canActivate(): boolean {
        if (this.authService.loggedIn()) {
            console.log('here');
            if (this.authService.isAdmin() === 1) {
                return true;
            }
            this.alertify.error('You are not an admin.');
            this.router.navigate(['/dashboard']);
            return false;
        }
        this.alertify.error('You shall not pass!!!');
        this.router.navigate(['/home']);
        return false;
    }
}
