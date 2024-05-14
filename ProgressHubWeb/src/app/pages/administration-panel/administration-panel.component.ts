import { Component } from '@angular/core';
import {userModel} from "../../models/userModel";
import {JwtHelperService} from "@auth0/angular-jwt";
import {AuthService} from "../../services/auth.service";
import {Validators} from "@angular/forms";
import {Flowbite} from "../../flowbiteDecorator";

@Component({
  selector: 'app-administration-panel',
  templateUrl: './administration-panel.component.html',
  styleUrls: ['./administration-panel.component.css']
})
@Flowbite()
export class AdministrationPanelComponent {
  //user
  user:userModel|null = {} as userModel;
  isAuthenticated: boolean = false;
  source:string = {} as string;

  //tabs
  selectedTab: number = 0;

  constructor(private _jwtHelper: JwtHelperService, private _apiService: AuthService) {}

  ngOnInit(): void {
    this.isAuthenticated = this.isUserAuthenticated();
    if(this.user?.avatar!=""){
      this.source = ""+this.user?.avatar;
    }else{
      this.source = './assets/images/user-avatar.png';
    }
  }

  isUserAuthenticated(): boolean {
    const token = localStorage.getItem('jwt');
    if(this._apiService.checkIfUserIsAuthenticated()){
      this.user = this._apiService.getUserModelFromJwt(this._jwtHelper.decodeToken(token!));
      console.log(this.user);
      return true;
    }
    return false;
  }

  changeTab(tab: number): void {
    this.selectedTab = tab;
  }

  logOut(): void {
    this._apiService.logOut();
    this.isAuthenticated = false;
  }
}
