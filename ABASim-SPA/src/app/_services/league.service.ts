import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { League } from '../_models/league';
import { LeagueState } from '../_models/leagueState';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameDisplay } from '../_models/gameDisplay';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';
import { Standing } from '../_models/standing';
import { Schedule } from '../_models/schedule';
import { Transaction } from '../_models/transaction';
import { PlayByPlay } from '../_models/playByPlay';
import { GameDetails } from '../_models/gameDetails';

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

  getScheduleGames(day: number): Observable<Schedule[]> {
    return this.http.get<Schedule[]>(this.baseUrl + 'getscheduledisplay/' + day);
  }

  getTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.baseUrl + 'gettransactions');
  }

  getPlayByPlaysForId(gameId: number): Observable<PlayByPlay[]> {
    return this.http.get<PlayByPlay[]>(this.baseUrl + 'getgameplaybyplay/' + gameId);
  }

  getGameDetailsPreseason(gameId: number) {
    return this.http.get<GameDetails>(this.baseUrl + 'getpreseasongamedetails/ ' + gameId);
  }

  getGameDetailsSeason(gameId: number) {
    return this.http.get<GameDetails>(this.baseUrl + 'getseasongamedetails/ ' + gameId);
  }

  getSeasonGamesForTomorrow(): Observable<GameDisplay[]> {
    return this.http.get<GameDisplay[]>(this.baseUrl + 'getgamesfortomorrowseason');
  }

  getSeasonGamesForToday(): Observable<GameDisplayCurrent[]> {
    return this.http.get<GameDisplayCurrent[]>(this.baseUrl + 'getgamesfortodayseason');
  }
}
