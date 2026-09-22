import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

import { BookmarkService } from '../../bookmark.service';
import { RepositoryItem } from '../../repository.service';

@Component({
  selector: 'app-bookmarks',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './bookmarks.html',
  styleUrl: './bookmarks.css',
})
export class BookmarksComponent {
  readonly bookmarkService = inject(BookmarkService);
  readonly bookmarks = signal<RepositoryItem[]>([]);
  readonly errorMessage = signal<string | null>(null);

  constructor() {
    this.bookmarkService.getBookmarks().subscribe({
      next: (result) => {
        this.bookmarks.set(result);
        this.errorMessage.set(null);
      },
      error: () => {
        this.errorMessage.set('Unable to load bookmarks. Please try again.');
      },
    });
  }

  removeBookmark(repository: RepositoryItem): void {
    this.bookmarkService.removeBookmark(repository.html_url).subscribe({
      next: () => {
        this.bookmarks.set(this.bookmarks().filter((repo) => repo.html_url !== repository.html_url));
      },
    });
  }
}
