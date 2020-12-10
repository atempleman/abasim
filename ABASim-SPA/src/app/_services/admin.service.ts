import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LeagueState } from '../_models/leagueState';
import { environment } from 'src/environments/environment';
import { CheckGame } from '../_models/checkGame';
import { Observable } from 'rxjs';
import { GameDisplayCurrent } from '../_models/gameDisplayCurrent';

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

  changeDay(day: number) {
    return this.http.get<boolean>(this.baseUrl + 'changeday/' + day);
  }

  beginPlayoffs() {
    return this.http.get<boolean>(this.baseUrl + 'beginplayoffs');
  }

  beginConfSemis() {
    return this.http.get<boolean>(this.baseUrl + 'beginconfsemis');
  }

  beginConfFinals() {
    return this.http.get<boolean>(this.baseUrl + 'beginconffinals');
  }

  beginFinals() {
    return this.http.get<boolean>(this.baseUrl + 'beginfinals');
  }

  endSeason() {
    return this.http.get<boolean>(this.baseUrl + 'endseason');
  }

  runTeamDraftPicksSetup() {
    return this.http.get<boolean>(this.baseUrl + 'runteamdraftpicks');
  }

  generateInitalContracts() {
    return this.http.get<boolean>(this.baseUrl + 'generateinitialcontracts');
  }

  generateInitialSalaryCaps() {
    return this.http.get<boolean>(this.baseUrl + 'generateinitialsalarycaps');
  }

  generateAutoPicks() {
    return this.http.get<boolean>(this.baseUrl + 'testautopickordering');
  }

  getGamesForReset(): Observable<GameDisplayCurrent[]> {
    return this.http.get<GameDisplayCurrent[]>(this.baseUrl + 'getgamesforreset');
  }

  resetGame(gameId: number) {
    return this.http.get<boolean>(this.baseUrl + 'resetgame/' + gameId);
  }

  rolloverSeasonStats() {
    return this.http.get<boolean>(this.baseUrl + 'rolloverseasonstats');
  }

  rolloverAwardWinners() {
    return this.http.get<boolean>(this.baseUrl + 'rolloverawards');
  }

  rolloverContractUpdates() {
    return this.http.get<boolean>(this.baseUrl + 'rollovercontractupdates');
  }

  generateDraft() {
    return this.http.get<boolean>(this.baseUrl + 'generatedraft');
  }

  deletePreseasonAndPlayoffsData() {
    return this.http.get<boolean>(this.baseUrl + 'deletepreseasonplayoffs');
  }

  deleteTeamSettingsData() {
    return this.http.get<boolean>(this.baseUrl + 'deleteteamsettings');
  }

  deleteAwardsData() {
    return this.http.get<boolean>(this.baseUrl + 'deleteawards');
  }

  deleteOtherData() {
    return this.http.get<boolean>(this.baseUrl + 'deleteother');
  }

  deleteSeasonData() {
    return this.http.get<boolean>(this.baseUrl + 'deleteseason');
  }

  resetStandings() {
    return this.http.get<boolean>(this.baseUrl + 'resetstandings');
  }

  rolloverLeague() {
    return this.http.get<boolean>(this.baseUrl + 'rolloverleague');
  }

  resetLeague() {
    return this.http.get<boolean>(this.baseUrl + 'resetleague');
  }
}
