import {Component, OnInit} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { loginModel } from '../../models/loginModel';
import { AuthenticationResponseModel } from '../../models/authenticationResponseModel';
import { SocialAuthService, SocialUser } from "@abacritt/angularx-social-login";
import {ExternalAuthModel} from "../../models/externalAuthModel";
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  //variables-------------------------------------------------------------------
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.pattern(
        '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
      ),
    ]),
  });
  isEnabled: boolean = false;
  isValid: boolean = true;
  isValidPassword: boolean = true;
  isNextStep: boolean = false;
  isValidData: boolean = true;
  isLoading: boolean = false;
  isExternalLoginValid: boolean = true;
  externalAuth: ExternalAuthModel = {} as ExternalAuthModel;
  //functions-------------------------------------------------------------------
  constructor(private _apiService: AuthService, private _router: Router, private _externalAuthService: SocialAuthService) {}

  ngOnInit():void {
    this.subscribeToAuthState();
    this.subscribeToValueChanges();
  }
  forgotPassword():void {
    this._router.navigate(['/verify-password'], {
      queryParams: { email: this.loginForm.value.email},
    });
  }
  subscribeToAuthState(): void {
    this._externalAuthService.authState.subscribe((user: SocialUser):void => {
      this.externalAuth = {
        provider: user.provider,
        idToken: user.idToken
      }
      this.loginRequest(true);
    });
  }

  subscribeToValueChanges(): void {
    this.loginForm.get('email')?.valueChanges.subscribe((value) => {
      this.inputChanged(value);
    });

    this.loginForm.get('password')?.valueChanges.subscribe((value) => {
      this.inputChanged(value);
    });
  }
  googleSignIn(googleWrapper: any):void {
    googleWrapper.click();
  }


  loginRequest(isExternal: boolean): void {
    this.isLoading = true;
    let model:any = {};
    let url:string = '';
    if(isExternal){
      model = this.externalAuth;
      url = 'https://localhost:7034/api/auth/external';
    }
    else{
      model = {
        email: this.loginForm.value.email,
        password: this.loginForm.value.password,
      } as loginModel;
      url = 'https://localhost:7034/api/auth/signin';
    }

    this._apiService
      .sendRequest(url, model)
      .subscribe({
        next: (res: AuthenticationResponseModel):void => {
          this.setToken(res.accessToken,res.refreshToken);
          if(!isExternal)this.isValidData = true;
          setTimeout(():void => {
            if(isExternal)this.isExternalLoginValid = true;
            this.isLoading = false;
            this._router.navigate(['/']);
          }, 1000);
        },
        error: (err):void => {
          if(!isExternal)this.isValidData = false;
          setTimeout(():void => {
            if(isExternal)this.isExternalLoginValid = false;
            this.isLoading = false
          }, 1000);
        }
      });
  }
  login(): void {
    if (this.loginForm.valid) {
      this.isValid = true;
      this.isValidPassword = true;
      this.loginRequest(false);
      console.log('Log in!');
    } else {
      if (!this.loginForm.get('email')?.valid) {
        this.isValid = false;
        this.isNextStep = false;
      }
      if (!this.loginForm.get('password')?.valid) {
        this.isValidPassword = false;
      }
    }
  }

  nextStep(): void {
    if (this.loginForm.get('email')?.valid) {
      this.isValid = true;
      this.isNextStep = true;
      this.isEnabled = false;
      this.isValidPassword = true;
      this.loginForm.get('password')?.setValue('');
    } else {
      this.isValid = false;
      this.isNextStep = false;
    }
  }
  previousStep(): void {
    this.isNextStep = false;
    this.isEnabled = true;
    this.isValidData = true;
  }
  inputChanged(event: any): void {
    this.isEnabled = event.length > 0;
  }

  setToken(token:string,refreshToken:string): void {
    localStorage.setItem('jwt', token);
    localStorage.setItem('refreshToken', refreshToken);
  }
}
