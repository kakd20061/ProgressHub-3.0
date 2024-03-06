import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { SignupComponent } from './pages/signup/signup.component';
import { VerificationComponent } from './pages/verification/verification.component';
import {VerificationPasswordComponent} from "./pages/verification-password/verification-password.component";
import {ChangePasswordComponent} from "./pages/change-password/change-password.component";
import {ChangePasswordGuard} from "./guards/change-password.guard";
import {MainComponent} from "./pages/main/main.component";
import {AccountSettingsComponent} from "./pages/account-settings/account-settings.component";

const routes: Routes = [
  { path: '', redirectTo: '/main', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'verify', component: VerificationComponent },
  { path: 'verify-password', component: VerificationPasswordComponent },
  { path: 'change-password', component: ChangePasswordComponent, canActivate: [ChangePasswordGuard] },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'account-settings', component: AccountSettingsComponent, canActivate: [AuthGuard] },
  { path: 'main', component: MainComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
