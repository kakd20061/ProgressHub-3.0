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
  isAuthenticated: boolean = false;
  constructor(private _jwtHelper: JwtHelperService, private _authService: AuthService) {}

  ngOnInit(): void {
    this.isAuthenticated = this.isUserAuthenticated();
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
}
