import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { League } from '../_models/league';
import { LeagueState } from '../_models/leagueState';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameDisplay } from '../_models/gameDisplay';

@Injectable({
  providedIn: 'root'
})
export class LeagueService {
  baseUrl = environment.apiUrl + '/league/';

  constructor(private http: HttpClient) { }

  getLeague() {
    return this.http.get<League>(this.baseUrl + 'getleague');
  }

  getLeagueStatuses(): Observable<LeagueState[]> {
    return this.http.get<LeagueState[]>(this.baseUrl + 'getleaguestatus');
  }

  getLeagueStatusForId(stateId: number) {
    console.log('state id: ' + stateId);
    return this.http.get<LeagueState>(this.baseUrl + 'getleaguestateforid/' + stateId);
  }

  getPreseasonGamesForTomorrow(): Observable<GameDisplay[]> {
    return this.http.get<GameDisplay[]>(this.baseUrl + 'getgamesfortomorrow');
  }
}
