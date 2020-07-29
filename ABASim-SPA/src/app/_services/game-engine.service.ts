import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { SimGame } from '../_models/simGame';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { BoxScore } from '../_models/boxScore';

@Injectable({
  providedIn: 'root'
})
export class GameEngineService {

  baseUrl = environment.apiUrl + '/gameEngine/';

  constructor(private http: HttpClient) { }

  startTestGame(game: SimGame) {
    return this.http.post(this.baseUrl + 'startGame', game);
  }

  startPreseasonGame(game: SimGame) {
    return this.http.post(this.baseUrl + 'startPreseasonGame', game);
  }

  startSeasonGame(game: SimGame) {
    return this.http.post(this.baseUrl + 'startSeasonGame', game);
  }

  startPlayoffGame(game: SimGame) {
    console.log('inside game engine');
    return this.http.post(this.baseUrl + 'startPlayoffGame', game);
  }

  getBoxScoreForGameId(gameId: number): Observable<BoxScore[]> {
    return this.http.get<BoxScore[]>(this.baseUrl + 'getboxscoresforgameid/' + gameId);
  }

  getBoxScoreForGameIdPlayoffs(gameId: number): Observable<BoxScore[]> {
    return this.http.get<BoxScore[]>(this.baseUrl + 'getboxscoresforgameidplayoffs/' + gameId);
  }
}
