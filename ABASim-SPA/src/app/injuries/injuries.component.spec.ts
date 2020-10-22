/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { InjuriesComponent } from './injuries.component';

describe('InjuriesComponent', () => {
  let component: InjuriesComponent;
  let fixture: ComponentFixture<InjuriesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InjuriesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InjuriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
