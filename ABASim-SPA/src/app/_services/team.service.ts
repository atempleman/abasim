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
import { Trade } from '../_models/trade';
import { TradeMessage } from '../_models/tradeMessage';
import { TeamDraftPick } from '../_models/teamDraftPick';
import { PlayerInjury } from '../_models/playerInjury';

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
    return this.http.post(this.baseUrl + 'savedepthchart', depthCharts);
  }

  rosterSpotCheck(teamId: number) {
    return this.http.get<boolean>(this.baseUrl + 'rosterSpotCheck/' + teamId);
  }

  getTeamInitialLotteryOrder(): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getTeamInitialLotteryOrder');
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

  getAllTeamsExceptUsers(teamId: number): Observable<Team[]> {
    return this.http.get<Team[]>(this.baseUrl + 'getallteamsexceptusers/' + teamId);
  }

  getTradeOffers(teamId: number): Observable<Trade[]> {
    return this.http.get<Trade[]>(this.baseUrl + 'gettradeoffers/' + teamId);
  }

  saveTradeProposal(trade: Trade[]) {
    console.log(trade);
    return this.http.post(this.baseUrl + 'savetradeproposal', trade);
  }

  acceptTradeProposal(tradeId: number) {
    return this.http.get<boolean>(this.baseUrl + 'acceptradeproposal/' + tradeId);
  }

  pullTradeProposal(tradeId: number) {
    return this.http.get<boolean>(this.baseUrl + 'pullradeproposal/' + tradeId);
  }

  rejectTradeProposal(trade: TradeMessage) {
    return this.http.post(this.baseUrl + 'rejecttradeproposal', trade);
  }

  getTradeMessageForTradeId(tradeId: number) {
    return this.http.get<TradeMessage>(this.baseUrl + 'gettrademessage/' + tradeId);
  }

  getTeamDraftPicks(teamId: number): Observable<TeamDraftPick[]> {
    return this.http.get<TeamDraftPick[]>(this.baseUrl + 'getteamsdraftpicks/' + teamId);
  }

  getPlayerInjuriesForTeam(teamId: number): Observable<PlayerInjury[]> {
    return this.http.get<PlayerInjury[]>(this.baseUrl + 'getinjuriesforteam/' + teamId);
  }

  getInjruiesForFreeAgents(): Observable<PlayerInjury[]> {
    return this.http.get<PlayerInjury[]>(this.baseUrl + 'getinjuriesforfreeagents');
  }

  getInjuryForPlayer(playerId: number): Observable<PlayerInjury> {
    return this.http.get<PlayerInjury>(this.baseUrl + 'getinjuryforplayer/' + playerId);
  }

}
