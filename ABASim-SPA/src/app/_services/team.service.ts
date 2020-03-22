import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Player } from '../_models/player';
import { Observable } from 'rxjs';
import { Team } from '../_models/team';
import { DepthChart } from '../_models/depthChart';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  baseUrl = 'http://localhost:5000/api/team/';

  constructor(private http: HttpClient) { }

  checkAvailableTeams() {
    return this.http.get<boolean>(this.baseUrl + 'checkavailableteams');
  }

  getRosterForTeam(teamId: number): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl + 'getrosterforteam/' + teamId);
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
    return this.http.post(this.baseUrl + 'savedepthchart', depthCharts);
  }

}
