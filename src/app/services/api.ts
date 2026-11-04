
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Movie } from '../models/movie';
import { Show } from '../models/show';
import { Seat } from '../models/seat';

export interface MovieResponse {
  items: Movie[];
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private baseUrl = 'https://localhost:7226/api';

  constructor(private http: HttpClient) {}

  // Get all movies
  getMovies(): Observable<MovieResponse> {
    return this.http.get<MovieResponse>(
      `${this.baseUrl}/movies`
    );
  }

  // Get movie by ID
  getMovieById(id: number): Observable<Movie> {
    return this.http.get<Movie>(
      `${this.baseUrl}/movies/${id}`
    );
  }

  // Get all shows
  getShows(): Observable<Show[]> {
    return this.http.get<Show[]>(
      `${this.baseUrl}/shows`
    );
  }

  // Get shows by movie
  getShowsByMovie(movieId: number): Observable<Show[]> {
    return this.http.get<Show[]>(
      `${this.baseUrl}/shows/movie/${movieId}`
    );
  }

  // Get show by ID
  getShowById(id: number): Observable<Show> {
    return this.http.get<Show>(
      `${this.baseUrl}/shows/${id}`
    );
  }

  // Get seats for a show
  getShowSeats(showId: number): Observable<Seat[]> {
    return this.http.get<Seat[]>(
      `${this.baseUrl}/shows/${showId}/seats`
    );
  }
}
