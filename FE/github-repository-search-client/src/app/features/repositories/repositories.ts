import { Component, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { BookmarkService } from '../../bookmark.service';
import { RepositoryItem, RepositoryService } from '../../repository.service';

@Component({
  selector: 'app-repositories',
  standalone: true,
  imports: [ReactiveFormsModule, MatCardModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule],
  templateUrl: './repositories.html',
  styleUrl: './repositories.css',
})
export class Repositories {
  readonly searchForm = new FormGroup({
    keyword: new FormControl(''),
  });

  readonly repositories = signal<RepositoryItem[]>([]);
  loading = false;
  errorMessage: string | null = null;
  hasSearched = false;
  readonly bookmarkedUrls = signal<Set<string>>(new Set());

  constructor(
    private readonly repositoryService: RepositoryService,
    private readonly bookmarkService: BookmarkService,
  ) {}

  private loadBookmarks(): void {
    this.bookmarkService.getBookmarks().subscribe({
      next: (bookmarks) => {
        this.bookmarkedUrls.set(new Set(bookmarks.map((bookmark) => bookmark.html_url)));
      },
    });
  }

  toggleBookmark(repository: RepositoryItem): void {
    const isBookmarked = this.bookmarkedUrls().has(repository.html_url);

    if (isBookmarked) {
      this.bookmarkService.removeBookmark(repository.html_url).subscribe({
        next: () => {
          const updated = new Set(this.bookmarkedUrls());
          updated.delete(repository.html_url);
          this.bookmarkedUrls.set(updated);
          this.errorMessage = null;
        },
        error: () => {
          this.errorMessage = 'Unable to remove bookmark. Please try again.';
        },
      });
      return;
    }

    this.bookmarkService.addBookmark(repository).subscribe({
      next: () => {
        const updated = new Set(this.bookmarkedUrls());
        updated.add(repository.html_url);
        this.bookmarkedUrls.set(updated);
        this.errorMessage = null;
      },
      error: () => {
        this.errorMessage = 'Unable to add bookmark. Please try again.';
      },
    });
  }

  onSubmit(): void {
    const trimmedKeyword = this.searchForm.controls.keyword.value?.trim() ?? '';

    if (!trimmedKeyword) {
      this.repositories.set([]);
      this.errorMessage = null;
      return;
    }

    this.hasSearched = true;
    this.loading = true;
    this.errorMessage = null;

    this.repositoryService.searchRepositories(trimmedKeyword).subscribe({
      next: (response) => {
        this.repositories.set(response.items ?? []);
        this.loading = false;
        this.loadBookmarks();
      },
      error: () => {
        this.repositories.set([]);
        this.errorMessage = 'Unable to load repositories. Please try again.';
        this.loading = false;
      },
    });
  }
}
