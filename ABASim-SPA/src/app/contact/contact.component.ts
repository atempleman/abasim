import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';
import { ContactForm } from '../_models/contactForm';
import { ContactService } from '../_services/contact.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {
  contactForm: FormGroup;
  contactObject: ContactForm;

  constructor(private alertify: AlertifyService, private fb: FormBuilder, private contactService: ContactService) { }

  ngOnInit() {
    this.createContactForm();
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
      });
    }
  }

}
