import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CodeReviewRequest, CodeReviewResponse } from '../models/code-review.model';

@Injectable({
  providedIn: 'root'
})
export class CodeReviewService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  reviewCode(payload: CodeReviewRequest): Observable<CodeReviewResponse> {
    return this.http.post<CodeReviewResponse>(`${this.apiUrl}/code-review`, payload);
  }
}