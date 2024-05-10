import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgxOtpInputConfig } from 'ngx-otp-input';
import { AuthService } from '../../services/auth.service';
import { verificationModel } from '../../models/verificationModel';
import {CommonService} from "../../services/common.service";
import configs from "@tsparticles/configs";
import {Engine} from "@tsparticles/engine";
import {loadSlim} from "@tsparticles/slim";
import {NgParticlesService} from "@tsparticles/angular";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-verification',
  templateUrl: './verification.component.html',
  styleUrls: ['./verification.component.css'],
})
export class VerificationComponent {
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
    public _commonService: CommonService,
    private _ngParticlesService: NgParticlesService
  ) {}
  otpInputConfig: NgxOtpInputConfig = {
    otpLength: 4,
    classList: {
      input: 'otp-input',
    inputFilled: 'my-super-filled-class',
    inputDisabled: 'my-super-disable-class',
    inputSuccess: 'my-super-success-class',
    inputError: 'my-super-error-class',
    },
  };
  onOtpChange(event: string[]): void {
    this.isEnabled = this._commonService.onOtpChange(event);
    if(this.isEnabled){
      this.otpValue = event;
    }
  }
  ngOnInit() {
    this._ngParticlesService.init(async (engine:Engine) => {
      console.log(engine);
      await loadSlim(engine);
    });

    this._route.queryParams.subscribe((params) => {
      this.email = params['email'];
    });
    console.log(this.email);
  }

  async sendRequest(): Promise<void> {
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
      .sendRequest(environment.backend.baseUrl+'auth/verify', model)
      .subscribe({
        next: (res):void => {
          this.isSuccess = 1;
          this.isLoading = false;
          setTimeout(() => {
            this._router.navigate(['/login']);
          }, 3000);
        },
        error: (err):void => {
          this.isSuccess = 2;
          this.isLoading = false;
          setTimeout(() => {
            this.isSuccess = 0;
            this.isEnabled = false;
          }, 3000);
        },
      });
  }
  async resend(): Promise<void> {
    this.isLoading = true;
    this.isResend = true;
    this._apiService
      .resendRequest(environment.backend.baseUrl+'auth/resend', this.email)
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

  protected readonly CommonService = CommonService;
}
