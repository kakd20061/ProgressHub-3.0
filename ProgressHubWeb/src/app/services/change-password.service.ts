import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ChangePasswordService {

  private isVerificationSuccessful = false;
  private email:string = '';
  setVerificationStatus(status: boolean): void {
    this.isVerificationSuccessful = status;
  }
  setEmail(email: string): void {
    this.email = email;
  }
  getEmail(): string {
    return this.email;
  }
  getVerificationStatus(): boolean {
    return this.isVerificationSuccessful;
  }
}
