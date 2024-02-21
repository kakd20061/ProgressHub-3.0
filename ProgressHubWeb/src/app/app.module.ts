import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { HomeComponent } from './pages/home/home.component';
import { JwtModule } from '@auth0/angular-jwt';
import { SignupComponent } from './pages/signup/signup.component';
import {
  RECAPTCHA_SETTINGS,
  RecaptchaFormsModule,
  RecaptchaModule,
  RecaptchaSettings,
} from 'ng-recaptcha';
import { environment } from '../environments/environment';

import { VerificationComponent } from './pages/verification/verification.component';
import { NgxOtpInputModule } from 'ngx-otp-input';
import {
  heroCheckCircle,
  heroXCircle,
  heroEnvelope,
  heroFaceFrown,
  heroBars3,
  heroXMark,
} from '@ng-icons/heroicons/outline';
import { NgIconsModule } from '@ng-icons/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { SocialLoginModule, SocialAuthServiceConfig } from '@abacritt/angularx-social-login';
import { GoogleLoginProvider } from '@abacritt/angularx-social-login';
import { GoogleSigninComponent } from './components/google-signin/google-signin.component';
import { NgOptimizedImage } from '@angular/common';
import { VerificationPasswordComponent } from './pages/verification-password/verification-password.component';
import { ChangePasswordComponent } from './pages/change-password/change-password.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { MainComponent } from './pages/main/main.component';

export function tokenGetter() {
  return localStorage.getItem('jwt');
}
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    SignupComponent,
    VerificationComponent,
    GoogleSigninComponent,
    VerificationPasswordComponent,
    ChangePasswordComponent,
    NavbarComponent,
    MainComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    RecaptchaModule,
    RecaptchaFormsModule,
    NgxOtpInputModule,
    MatProgressSpinnerModule,
    SocialLoginModule,
    NgOptimizedImage,
    NgIconsModule.withIcons({
      heroCheckCircle,
      heroXCircle,
      heroEnvelope,
      heroFaceFrown,
      heroBars3,
      heroXMark,
    }),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ['localhost:5001'],
        disallowedRoutes: [],
      },
    }),
    BrowserAnimationsModule,
  ],
  providers: [
    {
      provide: RECAPTCHA_SETTINGS,
      useValue: {
        siteKey: environment.reCaptcha.siteKey,
      } as RecaptchaSettings,
    },

    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(
              environment.google.clientId,{oneTapEnabled:false, prompt:'consent'}
            )
          }
        ]
      } as SocialAuthServiceConfig,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
