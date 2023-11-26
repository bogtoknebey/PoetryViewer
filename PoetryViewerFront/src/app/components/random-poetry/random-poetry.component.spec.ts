import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RandomPoetryComponent } from './random-poetry.component';

describe('RandomPoetryComponent', () => {
  let component: RandomPoetryComponent;
  let fixture: ComponentFixture<RandomPoetryComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RandomPoetryComponent]
    });
    fixture = TestBed.createComponent(RandomPoetryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
