import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ContactForm } from '../_models/contactForm';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  baseUrl = 'http://localhost:5000/api/contact/';

  constructor(private http: HttpClient) { }

  saveContact(contactForm: ContactForm) {
    return this.http.post(this.baseUrl + 'savecontact', contactForm);
  }
}
