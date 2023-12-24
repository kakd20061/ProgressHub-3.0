import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from './auth.service';
import { AuthenticationResponseModel } from './models/authenticationResponseModel';
@Injectable({
  providedIn: 'root',
})
export class AuthGuard {
  constructor(
    private _router: Router,
    private _jwtHelper: JwtHelperService,
    private _service: AuthService
  ) {}

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const token = localStorage.getItem('jwt');
    if (token && !this._jwtHelper.isTokenExpired(token)) {
      return true;
    }

    const refreshToken: string = localStorage.getItem('refreshToken')!;
    if (!token || !refreshToken) {
      await this._router.navigate(['/login']);
      return false;
    }

    const credentials = JSON.stringify({
      accessToken: token,
      refreshToken: refreshToken,
    });

    const isRefreshSuccess = await new Promise<AuthenticationResponseModel>(
      () => {
        this._service
          .tryRefreshingTokens(
            token!,
            'https://localhost:7034/api/token/refresh',
            credentials
          )
          .subscribe({
            next: (res: AuthenticationResponseModel) => {
              localStorage.setItem('jwt', res.accessToken);
              localStorage.setItem('refreshToken', res.refreshToken);
              return true;
            },
            error: (_) => {
              this._router.navigate(['/login']);
              return false;
            },
          });
      }
    );
    return true;
  }
}
