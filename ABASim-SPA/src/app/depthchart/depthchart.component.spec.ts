/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DepthchartComponent } from './depthchart.component';

describe('DepthchartComponent', () => {
  let component: DepthchartComponent;
  let fixture: ComponentFixture<DepthchartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DepthchartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DepthchartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
