import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { NewsFeedService, Story } from '../_services/news-feed-service';

@Component({
  selector: 'app-news-feed',
  imports: [ReactiveFormsModule],
  templateUrl: './news-feed.component.html',
  styleUrl: './news-feed.component.css'
})
export class NewsFeedComponent {
  private newsFeedService = inject(NewsFeedService);

  loading = signal(false);
  pageNum = signal(1);
  searchTerm = signal<string | null>(null);
  searchControl = new FormControl<string>('', {
    nonNullable: true,
    validators: [Validators.minLength(2)]
  });

  pages = signal<Story[][]>([]);
  page = computed<Story[]>(() => {
    const idx = this.pageNum() - 1;
    return this.pages()[idx] ?? [];
  });

  pageSize = signal(15);
  pageSizeControl = new FormControl<number>(15, {
    nonNullable: true,
    validators: [Validators.min(1), Validators.max(50)]
  });

  error = signal<string | null>(null)

  // Pagination grouping
  groupStart = computed(() => Math.floor((this.pageNum() - 1) / 3) * 3 + 1);

  // Sliding window shows [1,2,3] => [4,5,6] => [7,8,9] etc
  pageWindow = computed<number[]>(() => {
    const start = this.groupStart();
    const knownMax = this.pages().length || 1;
    const end = Math.min(start + 2, knownMax);
    const len = end >= start ? end - start + 1 : 0;
    return Array.from({ length: len }, (_, i) => start + i);
  });

  // boolean to flag that we know we have the final page
  finalKnown = computed<boolean>(() => {
    const p = this.pages();
    if (!p.length) {
      return false;
    }
    const last = p[p.length - 1];
    return last.length < this.pageSize() || this.pageWindow().length < 3;
  });

  canGetPrevGroup = computed(() => this.groupStart() > 1);
  canGetNextGroup = computed(() => {
    if (!this.finalKnown()) {
      return true;
    }
    const nextStart = this.groupStart() + 3;
    return nextStart <= this.pages().length;
  });




  ngOnInit() {
    this.getNewStories();
  }

  //Fetch new stories from API
  private getNewStories() {
    this.loading.set(true);
    this.newsFeedService
      .getNewsFeed(this.pageNum(), this.pageSize(), this.searchTerm())
      .pipe(finalize(() => (this.loading.set(false))))
      .subscribe({
        next: (results) => {
          this.pages.set(results ?? []);
          const max = this.pages().length || 1;
          if (this.pageNum() > max) this.pageNum.set(max);
        },
        error: (err) => this.error.set(err)
      });
  }


  triggerSearch(raw: string) {
    // Mark as touched so errors show
    this.searchControl.markAsTouched();

    if (this.searchControl.invalid) {
      return; // don’t fire search if invalid
    }

    this.pageNum.set(1);
    const term = raw?.trim() || null;
    this.searchTerm.set(term);
    this.getNewStories();
  }

  setPage(n: number) {
    if (n < 1) {
      return;
    }
    this.pageNum.set(n);
  }

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
    this.getNewStories();
  }

  updatePageSize(pageSize: number) {
    this.setPage(1);
    this.pageSize.set(pageSize);
    this.getNewStories();
  }
}
