import { TestBed } from '@angular/core/testing';
import { PaginationComponent } from './pagination.component';
import { ComponentRef } from '@angular/core';

type Story = { id: number }; // minimal shape for tests

function makePages(lengths: number[]): Story[][] {
  return lengths.map((n, pageIdx) =>
    Array.from({ length: n }, (_, i) => ({ id: pageIdx * 1000 + i }))
  );
}

describe('PaginationComponent', () => {
  let fixture: ReturnType<typeof TestBed.createComponent<PaginationComponent>>;
  let comp: PaginationComponent

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaginationComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PaginationComponent);
    comp = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('finalPageNum', () => {
    it('sets finalPageNum when the last page is shorter than pageSize', () => {
      fixture.componentRef.setInput('pageSize', 15);
      fixture.componentRef.setInput('pages', makePages([15, 15, 7]));
      fixture.detectChanges();

      expect(comp.finalPageNum()).toBe(3);
      expect(comp.canGetNextGroup()).toBeFalse();
    });

    it('also sets finalPageNum when the current pageWindow is shorter than 3', () => {
      fixture.componentRef.setInput('pages', makePages([15, 15]));
      comp.pageNum.set(2);
      fixture.detectChanges();

      expect(comp.pageWindow()).toEqual([1, 2]); // instead of [1, 2, 3]
      expect(comp.finalPageNum()).toBe(2);
      expect(comp.canGetNextGroup()).toBeFalse();
    });
  });
});
