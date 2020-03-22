/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { InitiallotteryComponent } from './initiallottery.component';

describe('InitiallotteryComponent', () => {
  let component: InitiallotteryComponent;
  let fixture: ComponentFixture<InitiallotteryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InitiallotteryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InitiallotteryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
