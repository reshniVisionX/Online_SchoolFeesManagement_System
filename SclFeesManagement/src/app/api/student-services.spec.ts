import { TestBed } from '@angular/core/testing';

import { StudentServices } from './student-services';

describe('StudentServices', () => {
  let service: StudentServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudentServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
