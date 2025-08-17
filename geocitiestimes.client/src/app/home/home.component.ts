import { Component, input } from '@angular/core';
import { Story } from '../_services/news-feed-service';
import { NewsFeedComponent } from '../news-feed/news-feed.component';

@Component({
  selector: 'app-home',
  imports: [NewsFeedComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
