import { Routes } from '@angular/router';

import { authGuard } from './auth.guard';
import { BookmarksComponent } from './features/bookmarks/bookmarks';
import { Repositories } from './features/repositories/repositories';

export const routes: Routes = [
  {
    path: '',
    component: Repositories,
  },
  {
    path: 'bookmarks',
    component: BookmarksComponent,
    canActivate: [authGuard],
  },
];
