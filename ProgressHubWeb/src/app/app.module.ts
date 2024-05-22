import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
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
import { NgxParticlesModule } from "@tsparticles/angular";
import { VerificationComponent } from './pages/verification/verification.component';
import { NgxOtpInputModule } from 'ngx-otp-input';
import {
  heroCheckCircle,
  heroXCircle,
  heroEnvelope,
  heroFaceFrown,
  heroBars3,
  heroXMark,
  heroChevronDoubleDown,
  heroChevronDoubleUp,
  heroCog6Tooth,
  heroArrowRightOnRectangle,
  heroCloudArrowUp,
} from '@ng-icons/heroicons/outline';
import{heroUsersSolid,heroChartPieSolid,heroUserGroupSolid,heroChartBarSolid,heroPhotoSolid,heroTagSolid,heroLockClosedSolid,heroBars3BottomLeftSolid,heroCheckCircleSolid,heroXCircleSolid} from "@ng-icons/heroicons/solid";
import { NgIconsModule } from '@ng-icons/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { SocialLoginModule, SocialAuthServiceConfig } from '@abacritt/angularx-social-login';
import { GoogleLoginProvider } from '@abacritt/angularx-social-login';
import { GoogleSigninComponent } from './components/google-signin/google-signin.component';
import {AsyncPipe, NgOptimizedImage} from '@angular/common';
import { VerificationPasswordComponent } from './pages/verification-password/verification-password.component';
import { ChangePasswordComponent } from './pages/change-password/change-password.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { MainComponent } from './pages/main/main.component';
import { AccountSettingsComponent } from './pages/account-settings/account-settings.component';
import {MatChipsModule} from '@angular/material/chips';
import {PipeFor} from "./pipeFor";
import { AlertsComponent } from './components/alerts/alerts.component';
import { AdministrationPanelComponent } from './pages/administration-panel/administration-panel.component';
import {MatTableModule} from '@angular/material/table';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatSortModule} from '@angular/material/sort';
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatIconModule} from "@angular/material/icon";
import {MatAutocompleteModule} from "@angular/material/autocomplete";
import {MatDialogModule} from '@angular/material/dialog';
import { UserEditDialogComponent } from './components/user-edit-dialog/user-edit-dialog.component';
import {MatGridListModule} from "@angular/material/grid-list";
import {MatInputModule} from "@angular/material/input";
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
    AccountSettingsComponent,
    PipeFor,
    AlertsComponent,
    AdministrationPanelComponent,
    UserEditDialogComponent
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
      heroChevronDoubleDown,
      heroCog6Tooth,
      heroArrowRightOnRectangle,
      heroChevronDoubleUp,
      heroUsersSolid,
      heroPhotoSolid,
      heroTagSolid,
      heroLockClosedSolid,
      heroBars3BottomLeftSolid,
      heroXCircleSolid,
      heroCheckCircleSolid,
      heroCloudArrowUp,
      heroChartBarSolid,
      heroUserGroupSolid,
      heroChartPieSolid,
    }),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ['localhost:5001'],
        disallowedRoutes: [],
      },
    }),
    BrowserAnimationsModule,
    NgxParticlesModule,
    MatChipsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    FormsModule,
    MatFormFieldModule,
    MatIconModule,
    MatAutocompleteModule,
    AsyncPipe,
    MatDialogModule,
    MatGridListModule,
    MatInputModule
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
