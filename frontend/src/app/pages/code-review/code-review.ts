import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CodeReviewService } from '../../services/code-review';
import { CodeReviewResponse } from '../../models/code-review.model';

@Component({
  selector: 'app-code-review',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './code-review.html',
  styleUrl: './code-review.css'
})
export class CodeReview {
  private codeReviewService = inject(CodeReviewService);

  code = '';
  language = 'csharp';
  isLoading = signal(false);
  result = signal<CodeReviewResponse | null>(null);
  errorMessage = signal('');

  languages = ['csharp', 'javascript', 'typescript', 'python', 'java'];

  onSubmit(): void {
    if (!this.code.trim()) return;

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.result.set(null);

    this.codeReviewService.reviewCode({ code: this.code, language: this.language }).subscribe({
      next: (res) => {
        this.result.set(res);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Review failed. Please try again.');
        this.isLoading.set(false);
      }
    });
  }
}