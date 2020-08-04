/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { FullgamepbpComponent } from './fullgamepbp.component';

describe('FullgamepbpComponent', () => {
  let component: FullgamepbpComponent;
  let fixture: ComponentFixture<FullgamepbpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FullgamepbpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FullgamepbpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
