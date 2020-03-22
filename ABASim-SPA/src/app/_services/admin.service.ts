import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LeagueState } from '../_models/leagueState';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = 'http://localhost:5000/api/admin/';

  constructor(private http: HttpClient) { }

  updateLeagueStatus(newStatus: number) {
    return this.http.get<boolean>(this.baseUrl + 'updateleaguestatus/' + newStatus);
  }

  removeTeamRegistration(teamId: number) {
    return this.http.get<boolean>(this.baseUrl + 'removeteamrego/' + teamId);
  }

  runInitialDraftLottery() {
    return this.http.get<boolean>(this.baseUrl + 'runinitialdraftlottery');
  }
}
