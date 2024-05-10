import {Component, OnInit} from '@angular/core';
import {Flowbite} from "../../flowbiteDecorator";
import {userModel} from "../../models/userModel";
import {JwtHelperService} from "@auth0/angular-jwt";
import {AuthService} from "../../services/auth.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {tagModel} from "../../models/tagModel";
import {SaveTagsModel} from "../../models/SaveTagsModel";
import {AuthenticationResponseModel} from "../../models/authenticationResponseModel";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-account-settings',
  templateUrl: './account-settings.component.html',
  styleUrls: ['./account-settings.component.css']
})
@Flowbite()
export class AccountSettingsComponent implements OnInit{

  //user
  user:userModel|null = {} as userModel;
  isAuthenticated: boolean = false;

  //tabs
  selectedTab: number = 0;

  //password change
  isValidPassword: boolean = true;
  isValidPasswordNew: boolean = true;
  hasPassword: boolean = true;
  changePasswordResult:number = 0;
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
  changePasswordIndicator: boolean = false;

  //tags
  tags: tagModel[] = {} as tagModel[];
  userTags : tagModel[] = {} as tagModel[];
  userTagsIds: string[] = [];
  selectedChips: any[] = [];
  tagResult:number = 0;
  tagIndicator: boolean = false;

  //change avatar
  avatarResult:number = 0;
  avatarIndicator: boolean = false;
  source:string = {} as string;
  file: File = {} as File;
  isEnableChangeAvatarButton: boolean = false;
  avatarIsSaved: boolean = true;

  //personal data
  personalDataForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(2)]),
    lastName: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
    ]),
    nickname: new FormControl('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    dateOfBirth: new FormControl(''),
    gender: new FormControl('0', [Validators.required, Validators.pattern('^[0-3]$')]),
  });
  isEnabledPersonalDataButton: boolean = false;
  isValidPersonalData: boolean[] = [true, true, true, true, true];
  personalDataIndicator: boolean = false;
  personalDataResult: number = 0;
  personalDataErrorMessage: string = 'Unexpected error occurred. Please try again later.';

  constructor(private _jwtHelper: JwtHelperService, private _apiService: AuthService) {}

  changePersonalData(): void {
    this.personalDataResult = 0;
    this.personalDataIndicator = true;
    this.isValidPersonalData[0] = this.personalDataForm.get('name')?.valid!;
    this.isValidPersonalData[1] = this.personalDataForm.get('lastName')?.valid!;
    this.isValidPersonalData[2] = this.personalDataForm.get('nickname')?.valid!;
    this.isValidPersonalData[3] = this.personalDataForm.get('dateOfBirth')?.valid!;
    this.isValidPersonalData[4] = this.personalDataForm.get('gender')?.valid!

    if (this.isValidPersonalData[0] && this.isValidPersonalData[1] && this.isValidPersonalData[2] && this.isValidPersonalData[3] && this.isValidPersonalData[4]) {
      let model = {
        email: this.user?.email,
        name: this.personalDataForm.get('name')?.value,
        lastName: this.personalDataForm.get('lastName')?.value,
        nickname: this.personalDataForm.get('nickname')?.value,
        dateOfBirth: this.personalDataForm.get('dateOfBirth')?.value,
        gender: this.personalDataForm.get('gender')?.value,
      };
      let url:string = environment.backend.baseUrl+'settings/account/ChangePersonalData';

      this._apiService.sendRequest(url, model).subscribe({
        next: () => {
          this.refresh().then(r => {});
          setTimeout(() => {
              this.personalDataIndicator = false;
              this.personalDataResult = 1;
              this.isEnabledPersonalDataButton = false;
            }, 1000
          );
        },
        error: (_) => {
          console.log('error');
          setTimeout(() => {
              this.personalDataIndicator = false;
              if(_.error.includes('Nickname')){
                this.personalDataErrorMessage = 'This nickname is already taken';
              }
              else{
                this.personalDataErrorMessage = 'Unexpected error occurred. Please try again later.';
              }
              this.personalDataResult = 2;
              this.isEnabledPersonalDataButton = false;
            }, 1000
          )
        },
      });
    }else{
      this.personalDataIndicator = false;
    }
  }

  saveAvatar(isDelete:boolean): void {
    this.avatarResult = 0;
    this.avatarIndicator = true;

    const formData: FormData = new FormData();
    if(isDelete){
      formData.append('email', this.user?.email!);
      formData.append('file', '');
    }else{
      formData.append('email', this.user?.email!);
      formData.append('file', this.file, this.file.name);
    }


    let url:string = environment.backend.baseUrl+'settings/account/ChangeAvatar';
    console.log(formData);
    this._apiService.sendRequest(url, formData).subscribe({
      next: () => {
        this.refresh().then(r => {});
        setTimeout(() => {
            this.avatarIndicator = false;
            this.avatarResult = 1;
            this.isEnableChangeAvatarButton = false;
            this.avatarIsSaved = true;
          }, 1000
        )
      },
      error: (_) => {
        setTimeout(() => {
            this.avatarIndicator = false;
            this.avatarResult = 2;
            this.isEnableChangeAvatarButton = false;
            this.source = './assets/images/user-avatar.png';
            this.avatarIsSaved = false;
          }, 1000
        )
      },
    });
  }
  updateSource($event: Event) {
    this.file=($event.target as HTMLInputElement).files![0];
    this.projectImage(($event.target as HTMLInputElement).files![0]);
    console.log(($event.target as HTMLInputElement).files![0]);
    this.avatarIsSaved = false;
  }

  projectImage(file: File) {
    let reader = new FileReader();

    reader.onload = (event: any) => {
      this.source = event.target.result;
      this.isEnableChangeAvatarButton = true;
    };

    reader.onerror = (event: any) => {
      console.log("File could not be read: " + event.target.error.code);
    };

    reader.readAsDataURL(file);
  }

  changeSelected(query: string) {
    if(this.selectedChips.includes(query)){
      this.selectedChips = this.selectedChips.filter((item) => item !== query);
    }
    else{
      this.selectedChips.push(query);
    }
  }
  saveTags(): void {
    this.tagResult = 0;
    this.tagIndicator = true;
    let tags= this.selectedChips;
    let model : SaveTagsModel = {
      Email : this.user?.email!,
      TagsIds : tags
    }
    let url:string = environment.backend.baseUrl+'settings/account/SaveTags';
    this._apiService.sendRequest(url, model).subscribe({
      next: () => {
        this.refresh().then(r => {});
        setTimeout(() => {
            this.tagIndicator = false;
            this.tagResult = 1;
          }, 1000
        )
      },
      error: (_) => {
        console.log('error');
        setTimeout(() => {
            this.tagIndicator = false;
            this.tagResult = 2;
          }, 1000
        )
      },
    });
  }

  initTags(): void {
    this.selectedChips = [];
    this.userTags.forEach((tag) => {
      this.selectedChips.push(tag._Id);
    });
  }

  ngOnInit(): void {
    this.isAuthenticated = this.isUserAuthenticated();
    if(this.user?.avatar!=""){
      this.source = ""+this.user?.avatar;
    }else{
      this.source = './assets/images/user-avatar.png';
    }
    console.log(this.source);
    this.selectedChips = [];
    let tempIds: string[] = [];
    if(localStorage.getItem('hasPassword')!=null){
      this.hasPassword = JSON.parse(localStorage.getItem('hasPassword')!);
    }
    else
    {
      this.hasPassword = true;
    }
    if(this.hasPassword)this.changePasswordForm.get('password')?.setValidators(Validators.required);
    this.getTags();
    this.userTags = JSON.parse(this.user?.tags!);
    this.userTags.forEach((tag) => {
      tempIds.push(tag._Id);
    });
    this.userTagsIds = tempIds;
    this.subscribeToValueChanges();
    this.initTags();

    this.setPersonalData();
  }

  setPersonalData(): void{
    this.personalDataForm.get('name')?.setValue(this.user?.name!);
    this.personalDataForm.get('lastName')?.setValue(this.user?.lastName!);
    this.personalDataForm.get('nickname')?.setValue(this.user?.nickname!);
    this.personalDataForm.get('dateOfBirth')?.setValue(this.user?.dateofbirth!);
    this.personalDataForm.get('gender')?.setValue(this.user?.gender.toString()!);
  }

  getTags(): void {
    let url:string = environment.backend.baseUrl+'settings/account/GetAllTags';

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

  inputChangedPersonalData(): void {
    if (this.personalDataForm.get('name')?.value?.length! > 0 && this.personalDataForm.get('lastName')?.value?.length! > 0 && this.personalDataForm.get('nickname')?.value?.length! > 0 && this.personalDataForm.get('gender')?.value?.length! > 0) {
      this.isEnabledPersonalDataButton = this.personalDataForm.get('name')?.value != this.user?.name || this.personalDataForm.get('lastName')?.value != this.user?.lastName || this.personalDataForm.get('nickname')?.value != this.user?.nickname || this.personalDataForm.get('dateOfBirth')?.value != this.user?.dateofbirth || this.personalDataForm.get('gender')?.value != this.user?.gender.toString();
    }
    else{
      this.isEnabledPersonalDataButton = false;
    }
  }
  subscribeToValueChanges(): void {
    this.changePasswordForm.get('password')?.valueChanges.subscribe(() => {
      this.inputChangedPassword();
    });

    this.changePasswordForm.get('newpassword')?.valueChanges.subscribe(() => {
      this.inputChangedPassword();
    });

    this.personalDataForm.get('name')?.valueChanges.subscribe(() => {
      this.inputChangedPersonalData();
    });

    this.personalDataForm.get('lastName')?.valueChanges.subscribe(() => {
      this.inputChangedPersonalData();
    });

    this.personalDataForm.get('nickname')?.valueChanges.subscribe(() => {
      this.inputChangedPersonalData();
    });

    this.personalDataForm.get('dateOfBirth')?.valueChanges.subscribe(() => {
      this.inputChangedPersonalData();
    });

    this.personalDataForm.get('gender')?.valueChanges.subscribe(() => {
      this.inputChangedPersonalData();
    });
  }
  changeTab(tab: number): void {
    this.selectedTab = tab;
    this.tagResult = 0;
    this.changePasswordResult = 0;
    if(this.user?.avatar!=""){
      this.source = ""+this.user?.avatar;
    }else{
      this.source = './assets/images/user-avatar.png';
    }
    this.file = {} as File;
    this.changePasswordForm.reset();
    this.isEnableChangeAvatarButton = false;
    this.isEnabledPasswordButton = false;
    this.avatarResult = 0;
    this.avatarIsSaved = true;
    if (tab==0){
      this.setPersonalData();
    }
    this.personalDataResult = 0;
    this.personalDataIndicator = false;
  }
  changePassword(): void {
    this.changePasswordResult = 0;
    this.changePasswordIndicator = true;
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

      let url:string = environment.backend.baseUrl+'settings/account/ChangePassword';
      this._apiService.sendRequest(url, model).subscribe({
        next: () => {
          this.refresh().then(r => {});
          setTimeout(() => {
              this.changePasswordIndicator = false;
              this.changePasswordResult = 1;
            }, 1000
          );
        },
        error: (_) => {
          console.log('error');
          setTimeout(() => {
              this.changePasswordIndicator = false;
              this.changePasswordResult = 2;
            }, 1000
          )
        },
      });
    }else{
      this.changePasswordIndicator = false;
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

  logOut(): void {
    this._apiService.logOut();
    this.isAuthenticated = false;
  }


  async refresh() {
      const token = localStorage.getItem('jwt');
      const refreshToken: string = localStorage.getItem('refreshToken')!;
        const credentials = JSON.stringify({
          accessToken: token,
          refreshToken: refreshToken,
        });

        const isRefreshSuccess = await new Promise<AuthenticationResponseModel>(
          () => {
            this._apiService
              .tryRefreshingTokens(
                environment.backend.baseUrl+'token/refresh',
                credentials
              )
              .subscribe({
                next: (res: AuthenticationResponseModel) => {
                  localStorage.setItem('jwt', res.accessToken);
                  localStorage.setItem('refreshToken', res.refreshToken);
                  localStorage.setItem('hasPassword', res.hasPassword.toString());
                  this.ngOnInit();
                },
                error: (_) => {
                  this.isAuthenticated = false;
                },
              });
          }
        );
  }
}
