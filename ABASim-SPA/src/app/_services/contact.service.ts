import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ContactForm } from '../_models/contactForm';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  baseUrl = environment.apiUrl + '/contact/';

  constructor(private http: HttpClient) { }

  saveContact(contactForm: ContactForm) {
    return this.http.post(this.baseUrl + 'savecontact', contactForm);
  }
}
