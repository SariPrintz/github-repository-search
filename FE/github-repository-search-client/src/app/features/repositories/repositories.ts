import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

import { RepositoryItem, RepositoryService } from '../../repository.service';

@Component({
  selector: 'app-repositories',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './repositories.html',
  styleUrl: './repositories.css',
})
export class Repositories {
readonly searchForm = new FormGroup({
  keyword: new FormControl(''),
});  repositories: RepositoryItem[] = [];
  loading = false;
  errorMessage: string | null = null;
  hasSearched = false;

  constructor(private readonly repositoryService: RepositoryService) {}

  onSubmit(): void {
    debugger;
const trimmedKeyword = this.searchForm.controls.keyword.value?.trim() ?? '';
    if (!trimmedKeyword) {
      this.repositories = [];
      this.errorMessage = null;
      return;
    }
    this.hasSearched = true;
    this.loading = true;
    this.errorMessage = null;

    this.repositoryService.searchRepositories(trimmedKeyword).subscribe({
      next: (response) => {
        this.repositories = response.items ?? [];
        this.loading = false;
      },
      error: (error) => {
        this.repositories = [];
        this.errorMessage = 'Unable to load repositories. Please try again.';
        this.loading = false;
        console.error('Repository search failed:', error);
      },
    });
  }
}
