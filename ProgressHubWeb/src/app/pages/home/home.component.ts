import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  constructor(private _jwtHelper: JwtHelperService) {}

  ngOnInit(): void {
    this.isUserAuthenticated();
  }

  isUserAuthenticated(): boolean {
    const token = localStorage.getItem('jwt');
    if (token && !this._jwtHelper.isTokenExpired(token)) {
      console.log(this._jwtHelper.decodeToken(token));
      return true;
    }
    return false;
  }
}
