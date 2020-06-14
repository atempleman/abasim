import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LeagueState } from '../_models/leagueState';
import { environment } from 'src/environments/environment';
import { CheckGame } from '../_models/checkGame';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl + '/admin/';

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

  checkAllGamesRun(): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + 'checkgamesrun');
  }

  rolloverDay() {
    return this.http.get<boolean>(this.baseUrl + 'rolloverday');
  }
}
