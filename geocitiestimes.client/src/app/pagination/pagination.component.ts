import { Component, computed, inject, input, model, output, signal } from '@angular/core';
import { Story } from '../_services/news-feed-service';

@Component({
  selector: 'app-pagination',
  imports: [],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css'
})

export class PaginationComponent {
  pageNum = model<number>(1);
  pages = input<Story[][]>([]);
  pageSize = input<number>(15);

  groupStart = computed(() => Math.floor((this.pageNum() - 1) / 3) * 3 + 1);

  // Sliding window shows [1,2,3] => [4,5,6] => [7,8,9] etc
  pageWindow = computed<number[]>(() => {
    // First number in pagination group (always one more than a multple of 3)
    const start = this.groupStart();
    const maxPageNum = this.pages().length || 1;
    const end = Math.min(start + 2, maxPageNum);
    const groupLength = end - start + 1;
    return Array.from({ length: groupLength }, (_, i) => start + i);
  });

  // Tells us which pageNum is the last page.  Is null if we don't know yet.
  finalPageNum = computed<number | null>(() => {
    if (!this.pages()?.length) {
      return null;
    }

    const latestPageNum = this.pages().length;
    const lastPage = this.pages()[latestPageNum -1];

    /* If the last page is shorter than the page size, we ran out of stories,
     * so that's the last page.  Or if there are fewer than 3 pages left,
     * our pagination group is short, so the last one is the last page.
     * WARNING: if the last page is exactly the same length as the page size
     * and the last member of the pagination group, then we won't know
     * it's the last page!!  Handling for this case is left as an exercise
     * to the reader.  (Probably better served by info coming from the backend.)
     */
    if (lastPage.length < this.pageSize() || this.pageWindow().length < 3) {
      return latestPageNum
    }

    return null;
  });

  canGetPrevGroup = computed(() => this.groupStart() > 1);
  canGetNextGroup = computed(() => {
    if (this.finalPageNum() === null) {
      return true;
    }
    const nextStart = this.groupStart() + 3;
    return nextStart <= this.pages().length;
  });

  loadMorePages = output();

  prevGroup() {
    if (!this.canGetPrevGroup()) {
      return;
    }
    this.setPage(this.groupStart() - 3);
  }

  nextGroup() {
    if (!this.canGetNextGroup()) {
      return;
    }
    this.setPage(this.groupStart() + 3);
    this.loadMorePages.emit();
  }

  setPage(n: number) {
    if (n < 1) {
      return;
    }
    this.pageNum.set(n);
  }
}
