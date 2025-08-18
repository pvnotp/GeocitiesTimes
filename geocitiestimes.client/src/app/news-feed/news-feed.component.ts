import { Component, computed, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { NewsFeedService, Story } from '../_services/news-feed-service';
import { PaginationComponent } from '../pagination/pagination.component';
import { PageSizeComponent } from '../page-size/page-size.component';
import { SearchComponent } from '../search/search.component';

@Component({
  selector: 'app-news-feed',
  imports: [PaginationComponent, PageSizeComponent, SearchComponent],
  templateUrl: './news-feed.component.html',
  styleUrl: './news-feed.component.css'
})
export class NewsFeedComponent {
  private newsFeedService = inject(NewsFeedService);

  loading = signal(false);

  pageNum = signal(1);
  pageSize = signal(15);
  searchTerm = signal<string | null>(null);
  pages = signal<Story[][]>([]);
  page = computed<Story[]>(() => {
    const index = this.pageNum() - 1;
    return this.pages()[index] ?? [];
  });

  error = signal<string | null>(null)

  ngOnInit() {
    this.getNewStories();
  }

  //Fetch new stories from API
  getNewStories() {
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
}
