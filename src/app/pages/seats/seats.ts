import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { ApiService } from '../../services/api';
import { Seat } from '../../models/seat';

@Component({
  selector: 'app-seats',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './seats.html',
  styleUrl: './seats.css'
})
export class Seats implements OnInit {

  seats: Seat[] = [];

  showId!: number;

  loading = false;
  message = '';

  selectedSeats: Seat[] = [];

  constructor(
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {

    this.showId = Number(
      this.route.snapshot.paramMap.get('showId')
    );

    if (!this.showId || Number.isNaN(this.showId)) {
      this.message = 'Invalid show selection.';
      return;
    }

    this.loadSeats();
  }

  // Load seats from API
  loadSeats(): void {

    this.loading = true;
    this.message = '';

    this.apiService.getShowSeats(this.showId).subscribe({

      next: (response: Seat[]) => {

        this.seats = response.map(seat => ({
          ...seat,
          isSelected: false
        }));

        this.loading = false;

        if (this.seats.length === 0) {
          this.message = 'No seats available.';
        }

      },

      error: (error: any) => {

        console.error('Seat API Error:', error);

        this.loading = false;

        this.message = 'Unable to load seats. Please check the backend API.';

      }

    });

  }

  // Select or deselect a seat
  toggleSeat(seat: Seat): void {

    if (seat.isBooked) {
      return;
    }

    seat.isSelected = !seat.isSelected;

    this.selectedSeats = this.seats.filter(
      s => s.isSelected
    );

  }

  // Calculate total amount
  get totalAmount(): number {

    return this.selectedSeats.reduce(
      (total, seat) => total + seat.price,
      0
    );

  }

  // Continue to booking
  proceedToBooking(): void {

    if (this.selectedSeats.length === 0) {
      this.message = 'Please select at least one seat.';
      return;
    }

    const seatIds = this.selectedSeats.map(
      seat => seat.seatId
    );

    this.router.navigate(
      ['/booking', this.showId],
      {
        queryParams: {
          seats: seatIds.join(',')
        }
      }
    );

  }

  // Back to shows
  goBack(): void {

    this.router.navigate([
      '/shows',
      this.showId
    ]);

  }

}