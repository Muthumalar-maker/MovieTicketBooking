import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { Movies } from './pages/movies/movies';
import { Shows } from './pages/shows/shows';
import { Seats } from './pages/seats/seats';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    component: Login
  },

  {
    path: 'movies',
    component: Movies
  },

  {
    path: 'shows/:movieId',
    component: Shows
  },

  {
    path: 'seats/:showId',
    component: Seats
  },

  {
    path: '**',
    redirectTo: 'login'
  }

];