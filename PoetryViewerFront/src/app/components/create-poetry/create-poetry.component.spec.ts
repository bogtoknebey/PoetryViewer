import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreatePoetryComponent } from './create-poetry.component';

describe('CreatePoetryComponent', () => {
  let component: CreatePoetryComponent;
  let fixture: ComponentFixture<CreatePoetryComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CreatePoetryComponent]
    });
    fixture = TestBed.createComponent(CreatePoetryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
