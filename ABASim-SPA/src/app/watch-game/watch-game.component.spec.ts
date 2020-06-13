/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { WatchGameComponent } from './watch-game.component';

describe('WatchGameComponent', () => {
  let component: WatchGameComponent;
  let fixture: ComponentFixture<WatchGameComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WatchGameComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WatchGameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
