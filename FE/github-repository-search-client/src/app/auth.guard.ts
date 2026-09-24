import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const tokenInStorage = sessionStorage.getItem('jwt_token');
  const signalValue = authService.token();
  const isAuthenticated = authService.isAuthenticated();


  if (isAuthenticated) {
    return true;
  }

  return router.createUrlTree(['/']);
};
