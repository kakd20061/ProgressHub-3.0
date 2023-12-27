import { Component } from '@angular/core';
import {ChangePasswordService} from "../change-password.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {verificationModel} from "../models/verificationModel";
import {AuthService} from "../auth.service";
import {Router} from "@angular/router";
import {changePasswordModel} from "../models/changePasswordModel";

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  changePasswordForm = new FormGroup({
    password: new FormControl('', [
      Validators.required,
      Validators.pattern(
        '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
      ),
    ]),
  });
  private email: string = '';
  isEnabled: boolean = false;
  isValidPassword: boolean = true;
  isSuccess: number = 0;
  isLoading: boolean = false;


  constructor(private _changePasswordService: ChangePasswordService, private _apiService : AuthService, private _router:Router) {}

  //functions-------------------------------------------------------------------
  ngOnInit():void {
    this.email = this._changePasswordService.getEmail();
    this.subscribeToValueChanges();
  }
  async changePassword(): Promise<void> {
    if (this.changePasswordForm.valid) {
      this.isValidPassword = true;
      await this.sendRequest();
    } else {
      this.isValidPassword = false;
    }
  }
  async sendRequest(): Promise<void> {

    let model: changePasswordModel = {
      email: this.email,
      password: this.changePasswordForm.value.password,
    };
    this.isLoading = true;
    this._apiService
      .sendRequest('https://localhost:7034/api/features/change', model)
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

  subscribeToValueChanges(): void {
    this.changePasswordForm.get('password')?.valueChanges.subscribe((value) => {
      this.inputChanged(value);
    });
  }

  inputChanged(event: any): void {
    this.isEnabled = event.length > 0;
  }
}
