/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DraftboardComponent } from './draftboard.component';

describe('DraftboardComponent', () => {
  let component: DraftboardComponent;
  let fixture: ComponentFixture<DraftboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DraftboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DraftboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
