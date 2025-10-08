import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserReceipt } from './user-receipt';

describe('UserReceipt', () => {
  let component: UserReceipt;
  let fixture: ComponentFixture<UserReceipt>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserReceipt]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserReceipt);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
