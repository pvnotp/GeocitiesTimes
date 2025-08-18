import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NoUrlComponent } from './no-url/no-url.component';



export const routes: Routes = [
  { path: 'no-url', component: NoUrlComponent },
  { path: '', component: HomeComponent },
  { path: '**', component: HomeComponent, pathMatch: 'full' }
]
