import { Component } from '@angular/core';
import {JwtHelperService} from "@auth0/angular-jwt";
import {jwtUserModel} from "../../models/jwtUserModel";
import {userModel} from "../../models/userModel";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  user:userModel|null = {} as userModel;
  constructor(private _jwtHelper: JwtHelperService, private _authService: AuthService) {}

  ngOnInit(): void {
    this.isUserAuthenticated();
  }

  isUserAuthenticated(): boolean {
    const token = localStorage.getItem('jwt');
    if(this._authService.checkIfUserIsAuthenticated()){
      this.user = this._authService.getUserModelFromJwt(this._jwtHelper.decodeToken(token!));

      console.log(this.user);

      return true;
    }
    return false;
  }
}
