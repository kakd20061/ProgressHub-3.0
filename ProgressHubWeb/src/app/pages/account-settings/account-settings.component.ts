import { Component } from '@angular/core';
import {Flowbite} from "../../flowbiteDecorator";
import {userModel} from "../../models/userModel";
import {JwtHelperService} from "@auth0/angular-jwt";
import {AuthService} from "../../services/auth.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {tagModel} from "../../models/tagModel";
import {SaveTagsModel} from "../../models/SaveTagsModel";
import {AuthenticationResponseModel} from "../../models/authenticationResponseModel";

@Component({
  selector: 'app-account-settings',
  templateUrl: './account-settings.component.html',
  styleUrls: ['./account-settings.component.css']
})
@Flowbite()
export class AccountSettingsComponent {
  user:userModel|null = {} as userModel;
  isAuthenticated: boolean = false;
  selectedTab: number = 0;
  isValidPassword: boolean = true;
  isValidPasswordNew: boolean = true;
  tags: tagModel[] = {} as tagModel[];
  userTags : tagModel[] = {} as tagModel[];
  userTagsIds: string[] = [];
  selectedChips: any[] = [];
  hasPassword: boolean = true;
  changePasswordForm = new FormGroup({
    password: new FormControl('', [
      Validators.pattern(
        '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
      ),
    ]),

    newpassword: new FormControl('', [
      Validators.required,
      Validators.pattern(
        '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
      ),
    ]),
  });

  isEnabledPasswordButton: boolean = false;

  constructor(private _jwtHelper: JwtHelperService, private _apiService: AuthService) {}
  changeSelected(parameter: string, query: string) {

    const index = this.selectedChips.indexOf(query);
    if (index >= 0) {
      this.selectedChips.splice(index, 1);
    } else {
      this.selectedChips.push(query);
    }
  }
  saveTags(): void {
    let tags=this.selectedChips;
    let model : SaveTagsModel = {
      Email : this.user?.email!,
      TagsIds : tags
    }
    console.log(model);
    if(tags?.length==0){
      model.TagsIds = [];
    }
    let url:string = 'https://localhost:7034/api/settings/account/SaveTags';
    this._apiService.sendRequest(url, model).subscribe({
      next: () => {
        this.refresh().then(r => {});
      },
      error: (_) => {
        console.log('error');
      },
    });
  }
  reloadPage(){
    localStorage.setItem('selectedTabAccountSettings', this.selectedTab.toString());
    window.location.reload();
  }
  ngOnInit(): void {
    if(localStorage.getItem('hasPassword')!=null){
      this.hasPassword = JSON.parse(localStorage.getItem('hasPassword')!);
    }
    else
    {
      this.hasPassword = true;
    }
    if(this.hasPassword)this.changePasswordForm.get('password')?.setValidators(Validators.required);
    if(localStorage.getItem('selectedTabAccountSettings')!=null){
      this.selectedTab = parseInt(localStorage.getItem('selectedTabAccountSettings')!);
    }else{
      this.selectedTab = 0;
    }
    this.isAuthenticated = this.isUserAuthenticated();
    this.getTags();
    this.userTags = JSON.parse(this.user?.tags!);
    this.userTags.forEach((tag) => {
      this.userTagsIds.push(tag._Id);
    });
    this.subscribeToValueChanges();
    console.log(this.user);
  }
  getTags(): void {
    let url:string = 'https://localhost:7034/api/settings/account/GetAllTags';

    this._apiService.getTags(url).subscribe((data) => {
      this.tags=data;
    });
  }
  inputChangedPassword(): void {
    if(this.changePasswordForm.get('password')?.value?.length!>0 && this.changePasswordForm.get('newpassword')?.value?.length!>0){
      this.isEnabledPasswordButton = true;
    }
    else if(!this.hasPassword && this.changePasswordForm.get('newpassword')?.value?.length!>0)
    {
      this.isEnabledPasswordButton = true;
    }
    else{
      this.isEnabledPasswordButton = false;
    }
  }
  //todo: test this, update input after refresh (add hasPassword to refresh method),try to generate material chips instead of using ngFor in html, containers if succeeded or failed
  subscribeToValueChanges(): void {
    this.changePasswordForm.get('password')?.valueChanges.subscribe(() => {
      this.inputChangedPassword();
    });

    this.changePasswordForm.get('newpassword')?.valueChanges.subscribe(() => {
      this.inputChangedPassword();
    });
  }
  changeTab(tab: number): void {
    this.selectedTab = tab;
  }
  changePassword(): void {
    this.isValidPassword = this.changePasswordForm.get('password')?.valid!;
    this.isValidPasswordNew = this.changePasswordForm.get('newpassword')?.valid!;

    if (this.isValidPassword && this.isValidPasswordNew) {
      let model = {};
      if(this.hasPassword){
        model = {
          email: this.user?.email,
          password: this.changePasswordForm.get('newpassword')?.value,
          currentPassword: this.changePasswordForm.get('password')?.value,
        };
      }
      else{
        model = {
          email: this.user?.email,
          password: this.changePasswordForm.get('newpassword')?.value,
          currentPassword: '',
        };
      }

      let url:string = 'https://localhost:7034/api/settings/account/ChangePassword';
      this._apiService.sendRequest(url, model).subscribe({
        next: () => {
          this.refresh().then(r => {});
        },
        error: (_) => {
          console.log('error');
        },
      });
    }
  }
  isUserAuthenticated(): boolean {
    const token = localStorage.getItem('jwt');
    if(this._apiService.checkIfUserIsAuthenticated()){
      this.user = this._apiService.getUserModelFromJwt(this._jwtHelper.decodeToken(token!));
      return true;
    }
    return false;
  }

  logOut(): void {
    this._apiService.logOut();
    this.isAuthenticated = false;
  }


  async refresh() {
      const token = localStorage.getItem('jwt');
      const refreshToken: string = localStorage.getItem('refreshToken')!;
      if (token && refreshToken) {
        const credentials = JSON.stringify({
          accessToken: token,
          refreshToken: refreshToken,
        });

        const isRefreshSuccess = await new Promise<AuthenticationResponseModel>(
          () => {
            this._apiService
              .tryRefreshingTokens(
                token!,
                'https://localhost:7034/api/token/refresh',
                credentials
              )
              .subscribe({
                next: (res: AuthenticationResponseModel) => {
                  localStorage.setItem('jwt', res.accessToken);
                  localStorage.setItem('refreshToken', res.refreshToken);
                  this.reloadPage();
                },
                error: (_) => {
                  this.isAuthenticated = false;
                },
              });
          }
        );
      }
  }
}
