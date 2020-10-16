import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AddDraftRank } from '../_models/addDraftRank';
import { DraftPlayer } from '../_models/draftPlayer';
import { DraftTracker } from '../_models/draftTracker';
import { Observable } from 'rxjs';
import { InitialDraftPicks } from '../_models/initialDraftPicks';
import { Team } from '../_models/team';
import { DraftSelection } from '../_models/draftSelection';
import { DraftPick } from '../_models/draftPick';
import { environment } from 'src/environments/environment';
import { DashboardDraftPick } from '../_models/dashboardDraftPick';
import { InitialPickSalary } from '../_models/initialPickSalary';

@Injectable({
  providedIn: 'root'
})
export class DraftService {
  baseUrl = environment.apiUrl + '/draft/';

  constructor(private http: HttpClient) { }

  addDraftPlayerRanking(newRanking: AddDraftRank) {
    return this.http.post(this.baseUrl + 'adddraftrank', newRanking);
  }

  removeDraftPlayerRanking(ranking: AddDraftRank) {
    return this.http.post(this.baseUrl + 'removedraftrank', ranking);
  }

  getDraftBoardForTeam(teamId: number): Observable<DraftPlayer[]> {
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'getdraftboard/' + teamId);
  }

  moveRankingUp(player: AddDraftRank) {
    return this.http.post(this.baseUrl + 'moveup', player);
  }

  moveRankingDown(player: AddDraftRank) {
    return this.http.post(this.baseUrl + 'movedown', player);
  }

  beginInitialDraft() {
    return this.http.get<boolean>(this.baseUrl + 'beginInitialDraft');
  }

  getDraftTracker() {
    return this.http.get<DraftTracker>(this.baseUrl + 'getdrafttracker');
  }

  getInitialDraftPicks(): Observable<InitialDraftPicks[]> {
    return this.http.get<InitialDraftPicks[]>(this.baseUrl + 'getinitialdraftpicks');
  }

  getCurrentInitialDraftPick(): Observable<InitialDraftPicks> {
    return this.http.get<InitialDraftPicks>(this.baseUrl + 'getcurrentinitialdraftpick');
  }

  makeDraftPick(draftPick: DraftSelection) {
    return this.http.post(this.baseUrl + 'initialdraftselection', draftPick);
  }

  makeAutoPick(draftPick: DraftSelection) {
    return this.http.post(this.baseUrl + 'makeautopick', draftPick);
  }

  getDraftPicksForRound(page: number): Observable<DraftPick[]> {
    return this.http.get<DraftPick[]>(this.baseUrl + 'getinitialdraftpicksforround/' + page);
  }

  getDashboardPicks(pick: number): Observable<DashboardDraftPick> {
    return this.http.get<DashboardDraftPick>(this.baseUrl + 'getdashboardcurrentpick/' + pick);
  }

  getInitialDraftSalaryDetails(): Observable<InitialPickSalary[]> {
    return this.http.get<InitialPickSalary[]>(this.baseUrl + 'getinitialdraftsalarydetails');
  }
}
