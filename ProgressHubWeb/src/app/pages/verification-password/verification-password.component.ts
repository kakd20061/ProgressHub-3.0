import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgxOtpInputConfig } from 'ngx-otp-input';
import { AuthService } from '../../services/auth.service';
import { verificationModel } from '../../models/verificationModel';
import {CommonService} from "../../services/common.service";
import {ChangePasswordService} from "../../services/change-password.service";

@Component({
  selector: 'app-verification-password',
  templateUrl: './verification-password.component.html',
  styleUrls: ['./verification-password.component.css']
})
export class VerificationPasswordComponent {
  email: string = '';
  isEnabled: boolean = false;
  otpValue: string[] = ['', '', '', ''];
  isSuccess: number = 0;
  isResend: boolean = false;
  isLoading: boolean = false;

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _apiService: AuthService,
    private _commonService: CommonService,
    private _changePasswordService: ChangePasswordService
  ) {}
  otpInputConfig: NgxOtpInputConfig = {
    otpLength: 4,
  };
  onOtpChange(event: string[]): void {
    this.isEnabled = this._commonService.onOtpChange(event);
    if(this.isEnabled){
      this.otpValue = event;
    }
  }
  async ngOnInit():Promise<void> {
    this._route.queryParams.subscribe((params) => {
      this.email = params['email'];
    });
    await this.resend();
  }

  async CheckCode(): Promise<void> {
    let code: number = 0;
    this.otpValue.forEach((element) => {
      code = code * 10 + parseInt(element);
    });
    let model: verificationModel = {
      email: this.email,
      code: code,
    };
    this.isResend = false;
    this.isLoading = true;
    this._apiService
      .sendRequest('https://localhost:7034/api/features/check', model)
      .subscribe({
        next: (res):void => {
          this.isSuccess = 1;
          this.isLoading = false;
          setTimeout(() => {
            this._changePasswordService.setVerificationStatus(true);
            this._changePasswordService.setEmail(this.email);
            this._router.navigate(['/change-password']);
          }, 3000);
        },
        error: (err):void => {
          this.isSuccess = 2;
          this.isLoading = false;
          setTimeout(() => {
            this.isSuccess = 0;
            this.isEnabled = false;
            this._changePasswordService.setVerificationStatus(false);
          }, 3000);
        },
      });
  }
  async resend(): Promise<void> {
    this.isLoading = true;
    this.isResend = true;
    this._apiService
      .resendRequest('https://localhost:7034/api/features/resend', this.email)
      .subscribe({
        next: (res):void => {
          this.isSuccess = 1;
          this.isLoading = false;
          setTimeout(() => {
            this.isSuccess = 0;
            this.isEnabled = false;
            this.isResend = false;
          }, 3000);
        },
        error: (err):void => {
          console.log(err);
          this.isSuccess = 2;
          this.isLoading = false;
          setTimeout(() => {
            this.isSuccess = 0;
            this.isEnabled = false;
            this.isResend = false;
          }, 3000);
        },
      });
  }
}
