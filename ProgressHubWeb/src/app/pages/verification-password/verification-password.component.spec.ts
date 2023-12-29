import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerificationPasswordComponent } from './verification-password.component';

describe('VerificationPasswordComponent', () => {
  let component: VerificationPasswordComponent;
  let fixture: ComponentFixture<VerificationPasswordComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VerificationPasswordComponent]
    });
    fixture = TestBed.createComponent(VerificationPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
