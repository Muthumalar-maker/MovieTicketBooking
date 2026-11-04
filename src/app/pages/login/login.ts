
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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

  constructor(private auth: Auth) {}

  login(): void {
    const loginData = {
      email: this.email,
      password: this.password
    };

    this.auth.login(loginData).subscribe({
      next: (response: any) => {
        this.auth.saveToken(response.token);
        this.message = 'Login successful!';
      },
      error: (error) => {
        this.message = error.error?.message || 'Login failed!';
      }
    });
  }
}
