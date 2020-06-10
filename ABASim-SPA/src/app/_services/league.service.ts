import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { League } from '../_models/league';
import { LeagueState } from '../_models/leagueState';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameDisplay } from '../_models/gameDisplay';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { Standing } from '../_models/standing';

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

  getPreseasonGamesForToday(): Observable<GameDisplayCurrent[]> {
    return this.http.get<GameDisplayCurrent[]>(this.baseUrl + 'getgamesfortoday');
  }

  getConferenceStandings(conference: number): Observable<Standing[]> {
    return this.http.get<Standing[]>(this.baseUrl + 'getstandingsforconference/' + conference);
  }

  getDivisionStandings(division: number): Observable<Standing[]> {
    return this.http.get<Standing[]>(this.baseUrl + 'getstandingsfordivision/' + division);
  }

  getLeagueStandings(): Observable<Standing[]> {
    return this.http.get<Standing[]>(this.baseUrl + 'getstandingsforleague');
  }
}
