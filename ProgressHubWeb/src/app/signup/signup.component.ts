import {Component, OnInit} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { signUpModel } from '../models/signUpModel';
import {ExternalAuthModel} from "../models/externalAuthModel";
import {SocialAuthService, SocialUser} from "@abacritt/angularx-social-login";
import {AuthenticationResponseModel} from "../models/authenticationResponseModel";

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css'],
})
export class SignupComponent implements OnInit{
  //variables-------------------------------------------------------------------
  signUpForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    name: new FormControl('', [Validators.required, Validators.minLength(2)]),
    lastName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
    ]),
    nickname: new FormControl('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.pattern(
        '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
      ),
    ]),
    repeatPassword: new FormControl('', [
      Validators.required,
      this.confirmPasswordValidator,
    ]),
    agreement: new FormControl('', [Validators.requiredTrue]),
  });

  isEnabled: boolean = false;
  isValid: boolean[] = [true, true, true, true, true, true, true];
  errorContent: string[] = [
    'Please enter valid email address',
    'Please enter valid nickname (min length should be 5 letters)',
  ];
  isLoading: boolean = false;
  isExternalLoginValid: boolean = true;
  captchaResolved: boolean = false;
  externalAuth: ExternalAuthModel = {} as ExternalAuthModel;

  //functions-------------------------------------------------------------------

  constructor(private _apiService: AuthService, private _router: Router, private _externalAuthService: SocialAuthService) {}

  ngOnInit(): void {
    this.subscribeToAuthState();
    // this.subscribeToValueChanges();
    }
  subscribeToAuthState(): void {
    this._externalAuthService.authState.subscribe((user: SocialUser):void => {
      this.externalAuth = {
        provider: user.provider,
        idToken: user.idToken
      }
      this.sendRequest(true);
    });
  }

  googleSignIn(googleWrapper: any):void {
    googleWrapper.click();
  }
  sendRequest(isExternal: boolean): void {
    this.isLoading = true;
    let model:any = {};
    let url:string = '';

    if(!isExternal) {
      let name = this.signUpForm.value.name!;
      let lastName = this.signUpForm.value.lastName!;

      name = name[0].toUpperCase() + name.slice(1);
      lastName = lastName[0].toUpperCase() + lastName.slice(1);
      model = {
        name: name,
        lastName: lastName,
        email: this.signUpForm.value.email,
        nickname: this.signUpForm.value.nickname,
        password: this.signUpForm.value.password,
      } as signUpModel;
      url = 'https://localhost:7034/api/auth/signup';
    }else{
      model = this.externalAuth;
      url = 'https://localhost:7034/api/auth/external';
    }

    this._apiService
      .sendRequest(url, model)
      .subscribe({
        next: (res: AuthenticationResponseModel | any):void => {
          if(!isExternal){
            this.isLoading = false;
            this._router.navigate(['/verify'], {
              queryParams: { email: model.email },
            });
          } else {
            this.setToken(res.accessToken,res.refreshToken);
            setTimeout(():void => {
              this.isExternalLoginValid = true;
              this.isLoading = false;
              this._router.navigate(['/']);
            }, 1000);
          }
        },
        error: (err) => {
          setTimeout(():void => {
            if(!isExternal) {
              this.isLoading = false;
              if (err.error.includes('email')) {
                this.errorContent[0] = 'This email is already taken';
                this.isValid[0] = false;
              } else this.isValid[0] = true;
              if (err.error.includes('nickname')) {
                this.errorContent[1] = 'This nickname is already taken';
                this.isValid[3] = false;
              } else this.isValid[3] = true;
            } else {
                if(isExternal)this.isExternalLoginValid = false;
                this.isLoading = false
            }
            }, 1000);
        },
      });
  }
  //todo: google login
  confirmPasswordValidator(
    control: FormControl
  ): { [key: string]: boolean } | null {
    const password = control.root.get('password');
    const confirmPassword = control.value;

    if (password && confirmPassword !== password.value) {
      return { confirmPassword: true };
    }

    return null;
  }

  valueChanged(): void {
    this.isEnabled = !!(this.signUpForm.get('email')?.value?.length! > 0 &&
      this.signUpForm.value.name?.length! > 0 &&
      this.signUpForm.value.lastName?.length! > 0 &&
      this.signUpForm.value.nickname?.length! > 0 &&
      this.signUpForm.value.password?.length! > 0 &&
      this.signUpForm.value.repeatPassword?.length! > 0 &&
      this.signUpForm.get("agreement")?.valid &&
      this.captchaResolved);
  }
  checkCaptcha(captchaResponse: string): void {
    this.captchaResolved =
      !!(captchaResponse && captchaResponse.length > 0);
    this.valueChanged();
  }
  signUp() : void {
    if (this.signUpForm.valid) {
      this.isValid = [true, true, true, true, true, true, true];

      //send signup request
      this.sendRequest(false);
    } else {
      const formControls: string[] = [
        'email',
        'name',
        'lastName',
        'nickname',
        'password',
        'repeatPassword',
        'agreement',
      ];
      this.isValid = formControls.map(
        (control) => this.signUpForm.get(control)?.valid ?? true
      );

      if (!this.isValid[0])
        this.errorContent[0] = 'Please enter valid email address';
      if (!this.isValid[3])
        this.errorContent[1] =
          'Please enter valid nickname (min length should be 5 letters)';
    }
  }
  setToken(token:string,refreshToken:string): void {
    localStorage.setItem('jwt', token);
    localStorage.setItem('refreshToken', refreshToken);
  }
}
