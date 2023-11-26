import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SyntaxPoetryComponent } from './syntax-poetry.component';

describe('SyntaxPoetryComponent', () => {
  let component: SyntaxPoetryComponent;
  let fixture: ComponentFixture<SyntaxPoetryComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SyntaxPoetryComponent]
    });
    fixture = TestBed.createComponent(SyntaxPoetryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
