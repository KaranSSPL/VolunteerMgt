import { TestBed } from '@angular/core/testing';

import { HandlekeydownService } from './handlekeydown.service';

describe('HandlekeydownService', () => {
  let service: HandlekeydownService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HandlekeydownService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
