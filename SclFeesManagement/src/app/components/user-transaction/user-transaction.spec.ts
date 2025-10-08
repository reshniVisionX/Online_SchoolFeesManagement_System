import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserTransaction } from './user-transaction';

describe('UserTransaction', () => {
  let component: UserTransaction;
  let fixture: ComponentFixture<UserTransaction>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserTransaction]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserTransaction);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
