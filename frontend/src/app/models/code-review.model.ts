export interface CodeReviewRequest {
  code: string;
  language: string;
  projectId?: string;
}

export interface CodeReviewResponse {
  id: string;
  bugCount: number;
  securityIssueCount: number;
  performanceIssueCount: number;
  suggestions: string[];
  createdAt: string;
}