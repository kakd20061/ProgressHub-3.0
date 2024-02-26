import { Component,OnInit } from '@angular/core';
import {JwtHelperService} from "@auth0/angular-jwt";
import {jwtUserModel} from "../../models/jwtUserModel";
import {userModel} from "../../models/userModel";
import {AuthService} from "../../services/auth.service";
import {Flowbite} from "../../flowbiteDecorator";
import {AuthenticationResponseModel} from "../../models/authenticationResponseModel";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
@Flowbite()
export class NavbarComponent implements OnInit{
  user:userModel|null = {} as userModel;
  isAuthenticated: boolean = false;
  constructor(private _jwtHelper: JwtHelperService, private _authService: AuthService) {}

  ngOnInit(): void {
    this.refresh();
    this.isAuthenticated = this.isUserAuthenticated();
  }
  isUserAuthenticated(): boolean {
    const token = localStorage.getItem('jwt');
    if(this._authService.checkIfUserIsAuthenticated()){
      this.user = this._authService.getUserModelFromJwt(this._jwtHelper.decodeToken(token!));
      return true;
    }
    return false;
  }

  logOut(): void {
    this._authService.logOut();
    this.isAuthenticated = false;
  }

  toggleSidebar(): void {
    const menu = document.querySelector('.navbar-menu');
    const sidebar = document.querySelector('.sidebar');

    if(sidebar?.classList.contains('animate-open-sidebar')){
      sidebar?.classList.remove('animate-open-sidebar');
      sidebar?.classList.add('animate-close-sidebar');
      setTimeout(() => {
        menu?.classList.toggle('hidden');
      }, 250);
    } else if(sidebar?.classList.contains('animate-close-sidebar')){
      menu?.classList.toggle('hidden');
      sidebar?.classList.remove('animate-close-sidebar');
      sidebar?.classList.add('animate-open-sidebar');
    }
    else {
      menu?.classList.toggle('hidden');
    }
  }

  async refresh() {
    const token = localStorage.getItem('jwt');
    if (token && !this._jwtHelper.isTokenExpired(token)) {
      this.isAuthenticated = true;
    }
    else {
      const refreshToken: string = localStorage.getItem('refreshToken')!;
      if (token && refreshToken) {
        const credentials = JSON.stringify({
          accessToken: token,
          refreshToken: refreshToken,
        });

        const isRefreshSuccess = await new Promise<AuthenticationResponseModel>(
          () => {
            this._authService
              .tryRefreshingTokens(
                token!,
                'https://localhost:7034/api/token/refresh',
                credentials
              )
              .subscribe({
                next: (res: AuthenticationResponseModel) => {
                  localStorage.setItem('jwt', res.accessToken);
                  localStorage.setItem('refreshToken', res.refreshToken);
                  console.log('refresh');
                  this.ngOnInit();
                },
                error: (_) => {
                  console.log('logout');
                  this.isAuthenticated = false;
                },
              });
          }
        );
      }
      }
    }
}
