import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import {ChangePasswordService} from "./change-password.service";
@Injectable({
  providedIn: 'root',
})
export class ChangePasswordGuard {
  constructor(
    private _router: Router,
    private _changePasswordService: ChangePasswordService
  ) {}

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const isVerificationSuccessful = this._changePasswordService.getVerificationStatus();

    if (isVerificationSuccessful) {
      return true;
    } else {
      return this._router.parseUrl('/home');
    }
  }
}
