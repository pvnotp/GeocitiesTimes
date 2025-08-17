import { Component, inject, output, OutputEmitterRef, signal } from '@angular/core';
import { NewsFeedService, Story } from '../_services/news-feed-service';
import { finalize } from 'rxjs';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-news-feed',
  imports: [ReactiveFormsModule],
  templateUrl: './news-feed.component.html',
  styleUrl: './news-feed.component.css'
})
export class NewsFeedComponent {

  newsFeedService = inject(NewsFeedService);
  loading = false;
  pageNum = signal(1);
  pageSize = signal(20);
  pages: Story[][] | null = null;
  page: Story[] | null = null;
  pageArray: number[] = [];
  finalPage: number | null = null;
  searchControl = new FormControl<string>('', { nonNullable: true });

  ngOnInit() {
    this.getNewStories();
  }

  getNewStories(searchTerm: string | null = null) {
    this.loading = true;

    this.newsFeedService.getNewsFeed(this.pageNum(), this.pageSize(), searchTerm)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: results => {
          this.pages = results ?? []
          this.page = this.pages[this.pageNum() - 1] ?? [];
          this.updatePageArray();
        },
        error: err => console.log(err)
      });
  }


  triggerSearch(raw: string) {
    this.pageNum.set(1);
    const term = raw?.trim() || null;
    this.getNewStories(term);
  }



  setPage(pageNumber: number) {
    if (!this.pages) return;
    this.page = this.pages[pageNumber - 1] ?? null;
    this.pageNum.set(pageNumber);
  }

  get nextPage() {
    return this.pageArray[this.pageArray.length - 1] + 1;
  }
  get prevPage() {
    return this.pageArray[0] - 1;
  }

  updatePageArray() {
    if (!this.pages || this.pages.length === 0) {
      this.pageArray = [];
      return;
    }
    const groupSize = 3;
    const groupIndex = Math.floor((this.pageNum() - 1) / groupSize);
    const start = groupIndex * groupSize + 1;
    let end = 0;
    if (start + groupSize - 1 === this.pages.length &&
      this.pages[this.pages.length - 1].length < this.pageSize()) {
      end = start + groupSize - 1;
      this.finalPage = end;
    } else if (start + groupSize - 1 > this.pages.length) {
      end = this.pages.length;
      this.finalPage = end;
    } else {
      end = start + groupSize - 1;

    }

    this.pageArray = Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }
}
