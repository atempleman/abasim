/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { GameEngineService } from './game-engine.service';

describe('Service: GameEngine', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [GameEngineService]
    });
  });

  it('should ...', inject([GameEngineService], (service: GameEngineService) => {
    expect(service).toBeTruthy();
  }));
});
