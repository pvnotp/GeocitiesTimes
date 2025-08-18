import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { PageSizeComponent } from './page-size.component';

@Component({
  standalone: true,
  imports: [PageSizeComponent],
  template: `<app-page-size (pageSizeUpdate)="onUpdate($event)"></app-page-size>`
})
class HostComponent {
  lastEmitted?: number;
  onUpdate(n: number) {
    this.lastEmitted = n;
  }
}

describe('PageSizeComponent', () => {
  let comp: PageSizeComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent],
    }).compileComponents();

    comp = TestBed.createComponent(PageSizeComponent).componentInstance;
  });

  it('should create', () => {
    expect(comp).toBeTruthy();
    expect(comp.pageSizeControl.value).toBe(15);
  });

  it('should validate page size', () => {
    comp.pageSizeControl.setValue(0);
    expect(comp.pageSizeControl.valid).toBeFalse();

    comp.pageSizeControl.setValue(25);
    expect(comp.pageSizeControl.valid).toBeTrue();

    comp.pageSizeControl.setValue(51);
    expect(comp.pageSizeControl.valid).toBeFalse();
  });
});
