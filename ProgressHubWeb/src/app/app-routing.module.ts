import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './auth.guard';
import { SignupComponent } from './signup/signup.component';
import { VerificationComponent } from './verification/verification.component';
import {VerificationPasswordComponent} from "./verification-password/verification-password.component";
import {ChangePasswordComponent} from "./change-password/change-password.component";
import {ChangePasswordGuard} from "./change-password.guard";

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'verify', component: VerificationComponent },
  { path: 'verify-password', component: VerificationPasswordComponent },
  { path: 'change-password', component: ChangePasswordComponent, canActivate: [ChangePasswordGuard] },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
