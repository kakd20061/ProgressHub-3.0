import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationResponseModel } from '../models/authenticationResponseModel';
import { HttpHeaders } from '@angular/common/http';
import {Observable} from "rxjs";
import {jwtUserModel} from "../models/jwtUserModel";
import {userModel} from "../models/userModel";
import {JwtHelperService} from "@auth0/angular-jwt";
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot} from "@angular/router";
import {tagModel} from "../models/tagModel";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private _http: HttpClient, private _jwtHelper: JwtHelperService, private _router: Router) {}

  sendRequest(url: string, form: any):Observable<any> {
    return this._http.post<any>(`${url}`, form);
  }

  resendRequest(url: string, email: string):Observable<any> {
    return this._http.post<any>(`${url}?email=${email}`, null);
  }
  tryRefreshingTokens(url: string, credentials: string):Observable<AuthenticationResponseModel> {
    return this._http.post<AuthenticationResponseModel>(url, credentials, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
    });
  }

  getTags(url:string): Observable<any> {
    return this._http.get<any>(url);
  }

  getUserModelFromJwt(jwt: any): userModel {
    let user: userModel = {} as userModel;
    let email = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];
    let name = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    let surname = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname'];
    let nickname = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    let role = jwt['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    let tags = jwt['Tags'];
    let avatar = jwt['Avatar'];
    let dateOfBirth = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth'];
    let gender = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender'];


    user.email = email;
    user.name = name;
    user.lastName = surname;
    user.nickname = nickname;
    user.role = role;
    user.tags = tags;
    user.avatar = avatar;
    user.dateofbirth = dateOfBirth;
    user.gender = gender;

    return user;
  }

  checkIfUserIsAuthenticated(): boolean {
    const token = localStorage.getItem('jwt');
    return !!(token && !this._jwtHelper.isTokenExpired(token));
  }

  logOut(): void {
    localStorage.removeItem('jwt');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('hasPassword');
  }
}
