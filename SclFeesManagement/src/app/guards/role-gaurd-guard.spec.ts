import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { roleGaurdGuard } from './role-gaurd-guard';

describe('roleGaurdGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => roleGaurdGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
