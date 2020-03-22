/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { DraftService } from './draft.service';

describe('Service: Draft', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DraftService]
    });
  });

  it('should ...', inject([DraftService], (service: DraftService) => {
    expect(service).toBeTruthy();
  }));
});
