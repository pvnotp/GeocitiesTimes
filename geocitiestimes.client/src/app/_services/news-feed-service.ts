import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";

export interface Story {
  id: number;
  title: string;
  url: string;
}

@Injectable({
  providedIn: 'root',
})

export class NewsFeedService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getNewsFeed(pageNum: number, pageSize: number, searchTerm: string|null): Observable<Story[][]> {
    let params = new HttpParams().set('pagenum', pageNum).set('pagesize', pageSize);
    if (searchTerm) {
      params = params.set('searchterm', searchTerm);
    }
    return this.http.get<Story[][]>(this.baseUrl + 'stories/new', { params });
  }
}
