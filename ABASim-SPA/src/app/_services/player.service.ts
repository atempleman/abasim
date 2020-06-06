import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DraftPlayer } from '../_models/draftPlayer';
import { Player } from '../_models/player';
import { environment } from 'src/environments/environment';
import { CompletePlayer } from '../_models/completePlayer';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  baseUrl = environment.apiUrl + '/player/';

  constructor(private http: HttpClient) { }

  getInitialDraftPlayers(): Observable<DraftPlayer[]> {
    return this.http.get<DraftPlayer[]>(this.baseUrl + 'getinitialdraftplayers');
  }

  getPlayerForId(playerId: number) {
    return this.http.get<Player>(this.baseUrl + 'getplayerforid/' + playerId);
  }

  getAllPlayers(): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl + 'getallplayers');
  }

  getFreeAgents(): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl + 'getfreeagents');
  }

  playerForPlayerProfileById(playerId: number) {
    return this.http.get<CompletePlayer>(this.baseUrl + 'getcompleteplayer/' + playerId);
  }
}
