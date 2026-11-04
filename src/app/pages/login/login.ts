import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  email = '';
  password = '';
  message = '';
  loading = false;

  constructor(
    private auth: Auth,
    private router: Router
  ) {}

  login(): void {

    if (!this.email || !this.password) {
      this.message = 'Please enter email and password.';
      return;
    }

    this.loading = true;
    this.message = '';

    const loginData = {
      email: this.email,
      password: this.password
    };

    this.auth.login(loginData).subscribe({

      next: (response: any) => {

        this.auth.saveToken(response.token);

        this.message = 'Login successful!';

        this.loading = false;

        this.router.navigate(['/movies']);
      },

      error: (error: any) => {

        console.error('Login Error:', error);

        this.loading = false;

        this.message =
          error.error?.message || 'Invalid email or password.';
      }

    });
  }
}