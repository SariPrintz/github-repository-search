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

  constructor() {
    this.bookmarkService.getBookmarks().subscribe({
      next: (result) => {
        this.bookmarks.set(result);
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  removeBookmark(repository: RepositoryItem): void {
    this.bookmarkService.removeBookmark(repository.html_url).subscribe({
      next: () => {
        this.bookmarks.set(this.bookmarks().filter((repo) => repo.html_url !== repository.html_url));
      },
      error: (error) => {
        console.error(error);
      },
    });
  }
}
