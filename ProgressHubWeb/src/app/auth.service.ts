import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticationResponseModel } from './models/authenticationResponseModel';
import { HttpHeaders } from '@angular/common/http';
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private _http: HttpClient) {}

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
}
