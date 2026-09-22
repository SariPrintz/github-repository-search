import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { AuthService } from './auth.service';
import { LoginComponent } from './features/auth/login/login';
import { Repositories } from './features/repositories/repositories';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoginComponent, Repositories],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('github-repository-search-client');
  protected readonly auth = inject(AuthService);
}

