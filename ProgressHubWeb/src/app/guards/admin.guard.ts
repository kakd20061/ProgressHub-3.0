import {Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot} from "@angular/router";
import {JwtHelperService} from "@auth0/angular-jwt";
import {AuthService} from "../services/auth.service";
import {AuthenticationResponseModel} from "../models/authenticationResponseModel";
import {environment} from "../../environments/environment";
import {userModel} from "../models/userModel";

@Injectable({
  providedIn: 'root',
})
export class AdminGuard {
  User: userModel | null = {} as userModel;
  constructor(
    private _router: Router,
    private _jwtHelper: JwtHelperService,
    private _service: AuthService
  ) {}

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const token = localStorage.getItem('jwt');

    this.User = this._service.getUserModelFromJwt(this._jwtHelper.decodeToken(token!));

    if (token && !this._jwtHelper.isTokenExpired(token) && this.User.role === 'Admin') {
      return true;
    }

    const refreshToken: string = localStorage.getItem('refreshToken')!;
    if (!token || !refreshToken || this.User.role !== 'Admin') {
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
            environment.backend.baseUrl+'token/refresh',
            credentials
          )
          .subscribe({
            next: (res: AuthenticationResponseModel) => {
              localStorage.setItem('jwt', res.accessToken);
              localStorage.setItem('refreshToken', res.refreshToken);
              localStorage.setItem('hasPassword', res.hasPassword.toString());
              return this.User?.role === 'Admin';
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
