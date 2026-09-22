import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthService } from '../../../auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  protected readonly auth = inject(AuthService);

  protected readonly loginForm = new FormGroup({
    username: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(1)],
    }),
  });

  protected loading = false;
  protected loginError: string | null = null;

  login(): void {
    const username = this.loginForm.value.username?.trim();

    if (!username) {
      this.loginError = 'Please enter a username.';
      return;
    }

    this.loading = true;
    this.loginError = null;

    this.auth.login(username).subscribe({
      next: () => {
        this.loading = false;
        this.loginForm.reset();
      },
      error: () => {
        this.loading = false;
        this.loginError = 'Login failed. Please try again.';
      },
    });
  }
}
