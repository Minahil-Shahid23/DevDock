import { TestBed } from '@angular/core/testing';

import { CodeReview } from './code-review';

describe('CodeReview', () => {
  let service: CodeReview;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CodeReview);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
