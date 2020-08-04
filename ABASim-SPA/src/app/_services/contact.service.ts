import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ContactForm } from '../_models/contactForm';
import { environment } from 'src/environments/environment';
import { GlobalChat } from '../_models/globalChat';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  baseUrl = environment.apiUrl + '/contact/';

  constructor(private http: HttpClient) { }

  saveContact(contactForm: ContactForm) {
    return this.http.post(this.baseUrl + 'savecontact', contactForm);
  }

  sendChat(chat: GlobalChat) {
    return this.http.post(this.baseUrl + 'savechatrecord', chat);
  }

  getChatRecords(): Observable<GlobalChat[]> {
    return this.http.get<GlobalChat[]>(this.baseUrl + 'getchatrecords');
  }
}
