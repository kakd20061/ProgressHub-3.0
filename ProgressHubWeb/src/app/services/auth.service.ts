import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationResponseModel } from '../models/authenticationResponseModel';
import { HttpHeaders } from '@angular/common/http';
import {Observable} from "rxjs";
import {jwtUserModel} from "../models/jwtUserModel";
import {userModel} from "../models/userModel";
import {JwtHelperService} from "@auth0/angular-jwt";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private _http: HttpClient, private _jwtHelper: JwtHelperService) {}

  sendRequest(url: string, form: any):Observable<any> {
    return this._http.post<any>(`${url}`, form);
  }

  resendRequest(url: string, email: string):Observable<any> {
    return this._http.post<any>(`${url}?email=${email}`, null);
  }
  tryRefreshingTokens(token: string, url: string, credentials: string):Observable<AuthenticationResponseModel> {
    return this._http.post<AuthenticationResponseModel>(url, credentials, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
    });
  }

  getUserModelFromJwt(jwt: any): userModel {
    let user: userModel = {} as userModel;
    let email = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];
    let name = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    let surname = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname'];
    let nickname = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    let role = jwt['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    user.email = email;
    user.name = name;
    user.lastName = surname;
    user.nickname = nickname;
    user.role = role;
    
    return user;
  }

  checkIfUserIsAuthenticated(): boolean {
    const token = localStorage.getItem('jwt');
    return !!(token && !this._jwtHelper.isTokenExpired(token));
  }

  logOut(): void {
    localStorage.removeItem('jwt');
    localStorage.removeItem('refreshToken');
  }
}
