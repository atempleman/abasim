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
import { LeagueScoring } from '../_models/leagueScoring';
import { LeagueRebounding } from '../_models/leagueRebounding';
import { LeagueOther } from '../_models/leagueOther';
import { LeagueDefence } from '../_models/leagueDefence';
import { LeagueLeadersPoints } from '../_models/leagueLeadersPoints';
import { LeagueLeadersAssists } from '../_models/leagueLeadersAssists';
import { LeagueLeadersRebounds } from '../_models/leagueLeadersRebounds';
import { LeagueLeadersBlocks } from '../_models/leagueLeadersBlocks';
import { LeagueLeadersSteals } from '../_models/leagueLeadersSteals';
import { LeagueLeadersTurnover } from '../_models/leagueLeadersTurnovers';
import { LeagueLeadersFouls } from '../_models/leagueLeadersFouls';
import { LeagueLeadersMinutes } from '../_models/leagueLeadersMinutes';

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

  // getLeagueScoring(): Observable<LeagueScoring[]> {
  //   return this.http.get<LeagueScoring[]>(this.baseUrl + 'getleaguescoring');
  // }

  // getLeagueRebounding(): Observable<LeagueRebounding[]> {
  //   return this.http.get<LeagueRebounding[]>(this.baseUrl + 'getleaguerebounding');
  // }

  // getLeagueOther(): Observable<LeagueOther[]> {
  //   return this.http.get<LeagueOther[]>(this.baseUrl + 'getleagueother');
  // }

  // getLeagueDefence(): Observable<LeagueDefence[]> {
  //   return this.http.get<LeagueDefence[]>(this.baseUrl + 'getleaguedefence');
  // }

  getPointsLeagueLeadersForPage(page: number): Observable<LeagueLeadersPoints[]> {
    return this.http.get<LeagueLeadersPoints[]>(this.baseUrl + 'leagueleaderspoints/' + page);
  }

  getCountOfPointsLeagueLeaders() {
    return this.http.get<number>(this.baseUrl + 'getcountofpointsleaders');
  }

  getAssistsLeagueLeadersForPage(page: number): Observable<LeagueLeadersAssists[]> {
    return this.http.get<LeagueLeadersAssists[]>(this.baseUrl + 'leagueleadersassists/' + page);
  }

  getReboundsLeagueLeadersForPage(page: number): Observable<LeagueLeadersRebounds[]> {
    return this.http.get<LeagueLeadersRebounds[]>(this.baseUrl + 'leagueleadersrebounds/' + page);
  }

  getBlocksLeagueLeadersForPage(page: number): Observable<LeagueLeadersBlocks[]> {
    return this.http.get<LeagueLeadersBlocks[]>(this.baseUrl + 'leagueleadersblocks/' + page);
  }

  getStealsLeagueLeadersForPage(page: number): Observable<LeagueLeadersSteals[]> {
    return this.http.get<LeagueLeadersSteals[]>(this.baseUrl + 'leagueleaderssteals/' + page);
  }

  getTurnoversLeagueLeadersForPage(page: number): Observable<LeagueLeadersTurnover[]> {
    return this.http.get<LeagueLeadersTurnover[]>(this.baseUrl + 'leagueleadersturnovers/' + page);
  }

  getFoulsLeagueLeadersForPage(page: number): Observable<LeagueLeadersFouls[]> {
    return this.http.get<LeagueLeadersFouls[]>(this.baseUrl + 'leagueleadersfouls/' + page);
  }

  getMinutesLeagueLeadersForPage(page: number): Observable<LeagueLeadersMinutes[]> {
    return this.http.get<LeagueLeadersMinutes[]>(this.baseUrl + 'leagueleadersminutes/' + page);
  }

  getTopFivePoints(): Observable<LeagueLeadersPoints[]> {
    return this.http.get<LeagueLeadersPoints[]>(this.baseUrl + 'gettopfivepoints');
  }

  getTopFiveAssists(): Observable<LeagueLeadersAssists[]> {
    return this.http.get<LeagueLeadersAssists[]>(this.baseUrl + 'gettopfiveassists');
  }

  getTopFiveSteals(): Observable<LeagueLeadersSteals[]> {
    return this.http.get<LeagueLeadersSteals[]>(this.baseUrl + 'gettopfivesteals');
  }

  getTopFiveRebounds(): Observable<LeagueLeadersRebounds[]> {
    return this.http.get<LeagueLeadersRebounds[]>(this.baseUrl + 'gettopfiverebounds');
  }

  getTopFiveBlocks(): Observable<LeagueLeadersBlocks[]> {
    return this.http.get<LeagueLeadersBlocks[]>(this.baseUrl + 'gettopfiveblocks');
  }
}
