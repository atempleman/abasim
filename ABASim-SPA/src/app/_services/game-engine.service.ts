import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { SimGame } from '../_models/simGame';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class GameEngineService {

  baseUrl = environment.apiUrl + '/gameEngine/';

  constructor(private http: HttpClient) { }

  startTestGame(game: SimGame) {
    return this.http.post(this.baseUrl + 'startGame', game);
  }
}
