import { TestBed } from '@angular/core/testing';

import { Atraction } from './atraction';

describe('Atraction', () => {
  let service: Atraction;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Atraction);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
