import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Player } from '../_models/player';
import { Observable } from 'rxjs';
import { Team } from '../_models/team';
import { DepthChart } from '../_models/depthChart';
import { environment } from 'src/environments/environment';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { WaivedPlayer } from '../_models/waivedPlayer';
import { CoachSetting } from '../_models/coachSetting';
import { SignedPlayer } from '../_models/signedPlayer';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  baseUrl = environment.apiUrl + '/team/';

  constructor(private http: HttpClient) { }

  checkAvailableTeams() {
    return this.http.get<boolean>(this.baseUrl + 'checkavailableteams');
  }

  getRosterForTeam(teamId: number): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl + 'getrosterforteam/' + teamId);
  }

  getExtendedRosterForTeam(teamId: number): Observable<ExtendedPlayer[]> {
    return this.http.get<ExtendedPlayer[]>(this.baseUrl + 'getextendedroster/' + teamId);
  }

  getTeamForUserId(userId: number) {
    return this.http.get<Team>(this.baseUrl + 'getteamforuserid/' + userId);
  }

  getAllTeams(): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getallteams');
  }

  getTeamForTeamId(teamId: number) {
    return this.http.get<Team>(this.baseUrl + 'getteamforteamid/' + teamId);
  }

  getDepthChartForTeamId(teamId: number): Observable<DepthChart[]> {
    return this.http.get<DepthChart[]>(this.baseUrl + 'getteamdepthchart/' + teamId);
  }

  saveDepthCharts(depthCharts: DepthChart[]) {
    console.log(depthCharts);
    return this.http.post(this.baseUrl + 'savedepthchart', depthCharts);
  }

  rosterSpotCheck(teamId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rosterSpotCheck/' + teamId);
  }

  waivePlayer(waivedPlayer: WaivedPlayer) {
    return this.http.post(this.baseUrl + 'waiveplayer', waivedPlayer);
  }

  signPlayer(signedPlayer: SignedPlayer) {
    return this.http.post(this.baseUrl + 'signplayer', signedPlayer);
  }

  getCoachingSettings(teamId: number) {
    return this.http.get<CoachSetting>(this.baseUrl + 'getcoachsettings/' + teamId);
  }

  saveCoachingSettings(setting: CoachSetting) {
    console.log('test');
    console.log(setting);
    return this.http.post(this.baseUrl + 'savecoachsetting', setting);
  }

}
