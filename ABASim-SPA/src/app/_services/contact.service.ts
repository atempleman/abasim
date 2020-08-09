import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ContactForm } from '../_models/contactForm';
import { environment } from 'src/environments/environment';
import { GlobalChat } from '../_models/globalChat';
import { InboxMessage } from '../_models/inboxMessage';
import { Observable } from 'rxjs';
import { CountOfMessages } from '../_models/countOfMessages';

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

  getInboxMessages(teamId: number): Observable<InboxMessage[]> {
    return this.http.get<InboxMessage[]>(this.baseUrl + 'getinboxmessages/' + teamId);
  }

  sendInboxMessage(message: InboxMessage) {
    return this.http.post(this.baseUrl + 'sendinboxmessage', message);
  }

  deleteInboxMessage(message: number) {
    return this.http.get<boolean>(this.baseUrl + 'deletemessage/' + message);
  }

  getCountOfMessages(teamId: number) {
    return this.http.get<CountOfMessages>(this.baseUrl + 'getcountofmessages/' + teamId);
  }

  markMessageRead(messageId: number) {
    return this.http.get<boolean>(this.baseUrl + 'markasread/' + messageId);
  }
}
