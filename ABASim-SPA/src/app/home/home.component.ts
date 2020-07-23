import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { User } from '../_models/user';
import { TeamService } from '../_services/team.service';
import { ContactForm } from '../_models/contactForm';
import { ContactService } from '../_services/contact.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  loginModel: any = {};

  registerModel: any = {};
  user: User;

  registerForm: FormGroup;
  loginForm: FormGroup;
  contactForm: FormGroup;

  usernameRequired = 0;
  passwordRequired = 0;
  confirmRequired = 0;
  teamnameRequired = 0;
  mascotRequired = 0;
  shortcodeRequired = 0;
  nameRequired = 0;
  emailRequired = 0;
  passwordLength = 0;
  passwordMatch = 0;

  availableTeams = false;
  loginDisplay = false;

  contactObject: ContactForm;

  constructor(private authService: AuthService, private alertify: AlertifyService, private fb: FormBuilder,
              private teamService: TeamService, private router: Router, private contactService: ContactService) { }

  ngOnInit() {
    this.createContactForm();
    this.createLoginForm();
    // check available teams
    this.teamService.checkAvailableTeams().subscribe(result => {
      this.availableTeams = result;
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.createRegisterForm();
    });
  }

  createLoginForm() {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(100)]],
      confirmPassword: ['', Validators.required],
      email: ['', Validators.required],
      name: ['', Validators.required],
      teamname: ['', Validators.required],
      mascot: ['', Validators.required],
      shortcode: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  createContactForm() {
    this.contactForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required]],
      contact: ['', Validators.required],
    });
  }

  submitContactForm() {
    if (this.contactForm.valid) {
      this.contactObject = Object.assign({}, this.contactForm.value);
      this.contactService.saveContact(this.contactObject).subscribe(() => {
        this.alertify.success('Contact submitted successfully');
      }, error => {
        this.alertify.error(error);
      }, () => {
        // clear the form
        this.contactForm.reset();
      });
    }
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch: true };
  }

  login() {
    this.loginModel = Object.assign({}, this.loginForm.value);
    this.authService.login(this.loginModel).subscribe(next => {
      this.alertify.success('Logged in successfully');
      localStorage.setItem('currentUserId', this.authService.decodedToken.nameid);
      localStorage.setItem('isAdmin', this.authService.decodedToken.primarygroupsid);
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/dashboard']);
    });
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Registration successful');
      }, error => {
        this.alertify.error(error);
      }, () => {
        // Need to update the user passed in
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/dashboard']);
        });
      });
    } else {
      // Need to determine what failed the validation and then display the appropriate message
      if (this.registerForm.controls.username.value === '') {
        this.usernameRequired = 1;
      } else {
        this.usernameRequired = 0;
      }

      if (this.registerForm.controls.password.value === '') {
        this.passwordRequired = 1;
      } else {
        this.passwordRequired = 0;
      }

      if (this.registerForm.controls.confirmPassword.value === '') {
        this.confirmRequired = 1;
      } else {
        this.confirmRequired = 0;
      }

      if (this.registerForm.controls.email.value === '') {
        this.emailRequired = 1;
      } else {
        this.emailRequired = 0;
      }

      if (this.registerForm.controls.name.value === '') {
        this.nameRequired = 1;
      } else {
        this.nameRequired = 0;
      }

      if (this.registerForm.controls.teamname.value === '') {
        this.teamnameRequired = 1;
      } else {
        this.teamnameRequired = 0;
      }

      if (this.registerForm.controls.shortcode.value === '') {
        this.shortcodeRequired = 1;
      } else {
        this.shortcodeRequired = 0;
      }

      if (this.registerForm.controls.mascot.value === '') {
        this.mascotRequired = 1;
      } else {
        this.mascotRequired = 0;
      }

      if (this.registerForm.controls.password.value.legnth < 4) {
        this.passwordLength = 1;
      } else {
        this.passwordLength = 0;
      }

      // Mismath password
      if (this.registerForm.errors != null) {
        this.passwordMatch = 1;
      } else {
        this.passwordMatch = 0;
      }
    }
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  loginDisplayToggle() {
    if (this.loginDisplay) {
      this.loginDisplay = false;
    } else {
      this.loginDisplay = true;
    }
  }
}
