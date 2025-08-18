import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NoUrlComponent } from './no-url.component';

describe('NoUrlComponent', () => {
  let component: NoUrlComponent;
  let fixture: ComponentFixture<NoUrlComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NoUrlComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NoUrlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
