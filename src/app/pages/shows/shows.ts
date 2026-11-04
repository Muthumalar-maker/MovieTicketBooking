import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { ApiService } from '../../services/api';
import { Show } from '../../models/show';

@Component({
  selector: 'app-shows',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './shows.html',
  styleUrl: './shows.css'
})
export class Shows implements OnInit {

  shows: Show[] = [];

  movieId!: number;

  loading = false;
  message = '';

  constructor(
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {

    this.movieId = Number(
      this.route.snapshot.paramMap.get('movieId')
    );

    if (!this.movieId || Number.isNaN(this.movieId)) {
      this.message = 'Invalid movie selection.';
      return;
    }

    this.loadShows();
  }

  loadShows(): void {

    this.loading = true;
    this.message = '';

    this.apiService.getShowsByMovie(this.movieId).subscribe({

      next: (response: Show[]) => {

        this.shows = response;
        this.loading = false;

        if (this.shows.length === 0) {
          this.message = 'No shows available for this movie.';
        }

      },

      error: (error: any) => {

        console.error('Show API Error:', error);

        this.loading = false;
        this.message = 'Unable to load shows. Please try again.';

      }

    });

  }

  selectShow(show: Show): void {

    this.router.navigate(['/seats', show.showId]);

  }

  goBack(): void {

    this.router.navigate(['/movies']);

  }


}