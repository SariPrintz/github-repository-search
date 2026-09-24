import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from './auth.service';
import { LoginComponent } from './features/auth/login/login';
import { Repositories } from './features/repositories/repositories';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, LoginComponent, Repositories],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  public readonly auth = inject(AuthService);

  

  public logLogoutState(): void {
    this.auth.logout();
  }
}

