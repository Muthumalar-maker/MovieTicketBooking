
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { ApiService, MovieResponse } from '../../services/api';
import { Movie } from '../../models/movie';

@Component({
  selector: 'app-movies',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movies.html',
  styleUrl: './movies.css'
})
export class Movies implements OnInit {

  movies: Movie[] = [];
  loading = false;
  message = '';

  constructor(
    private apiService: ApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadMovies();
  }

  loadMovies(): void {
    this.loading = true;
    this.message = '';

    this.apiService.getMovies().subscribe({
      next: (response: MovieResponse) => {
        console.log('Movie API Response:', response);

        this.movies = response.items ?? [];
        this.loading = false;

        if (this.movies.length === 0) {
          this.message = 'No movies available.';
        }
      },

      error: (error: any) => {
        console.error('Movie API Error:', error);

        this.loading = false;
        this.message = 'Unable to load movies. Please try again.';
      }
    });
  }

  selectMovie(movie: Movie): void {
    this.router.navigate(['/shows', movie.movieId]);
  }
}
