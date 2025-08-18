import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { NewsFeedComponent } from './news-feed.component';
import { NewsFeedService, Story } from '../_services/news-feed-service';

function makeStories(count: number, startId = 1): Story[] {
  return Array.from({ length: count }, (_, i) => ({
    id: startId + i,
    title: `Story ${startId + i}`,
    url: `https://example.com/${startId + i}`,
  }));
}

describe('NewsFeedComponent', () => {
  let component: NewsFeedComponent;
  let serviceSpy: jasmine.SpyObj<NewsFeedService>;

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj('NewsFeedService', ['getNewsFeed']);

    await TestBed.configureTestingModule({
      imports: [NewsFeedComponent],
      providers: [{ provide: NewsFeedService, useValue: serviceSpy }],
    }).overrideComponent(NewsFeedComponent, { set: { template: '' } })
      .compileComponents();

    const fixture = TestBed.createComponent(NewsFeedComponent);
    component = fixture.componentInstance;
  });

  it('calls service on init and populates pages/page', () => {
    const page1 = makeStories(15, 1);
    const page2 = makeStories(15, 100);
    const page3 = makeStories(15, 200);
    serviceSpy.getNewsFeed.and.returnValue(of([page1, page2, page3]));

    component.ngOnInit();

    expect(serviceSpy.getNewsFeed).toHaveBeenCalledWith(1, 15, null);
    expect(component.loading()).toBe(false);
    expect(component.pages()).toEqual([page1, page2, page3]);
    expect(component.page()).toEqual(page1);
    expect(component.error()).toBeNull();
  });
});
