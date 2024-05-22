import {Component, ViewChild, AfterViewInit, inject} from '@angular/core';
import {userModel} from "../../models/userModel";
import {JwtHelperService} from "@auth0/angular-jwt";
import {AuthService} from "../../services/auth.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {Flowbite} from "../../flowbiteDecorator";
import {environment} from "../../../environments/environment";
import {UserAdministrationModel} from "../../models/userAdministrationModel";
import {MatTableDataSource} from "@angular/material/table";
import {MatPaginator} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {tagModel} from "../../models/tagModel";
import {MatChipEditedEvent, MatChipInputEvent} from "@angular/material/chips";
import {LiveAnnouncer} from "@angular/cdk/a11y";
import {COMMA, ENTER} from "@angular/cdk/keycodes";
import {catchError, EMPTY, of} from "rxjs";
import {UserEditDialogComponent} from "../../components/user-edit-dialog/user-edit-dialog.component";
import {MatDialog} from "@angular/material/dialog";
import {SharedService} from "../../services/shared.service";
export interface Fruit {
  name: string;
}
@Component({
  selector: 'app-administration-panel',
  templateUrl: './administration-panel.component.html',
  styleUrls: ['./administration-panel.component.css']
})
@Flowbite()
export class AdministrationPanelComponent {
  //user
  user: userModel | null = {} as userModel;
  isAuthenticated: boolean = false;
  source: string = {} as string;

  users: UserAdministrationModel[] = [];

  //tabs
  selectedTab: number = 0;


  //user management
  displayedColumns: string[] = ['avatar', 'role', 'nickname', 'email', 'name', 'lastName', 'dateOfBirth', 'gender','banExpirationDate'];
  dataSource = {} as MatTableDataSource<UserAdministrationModel>;
  @ViewChild(MatPaginator) paginator: MatPaginator = {} as MatPaginator;
  @ViewChild(MatSort) sort: MatSort = {} as MatSort;
  selectedUser: UserAdministrationModel = {} as UserAdministrationModel;


  //tags management
  tags: tagModel[] = [];

  constructor(private _jwtHelper: JwtHelperService, private _apiService: AuthService, public dialog: MatDialog, private sharedService: SharedService) {
    this.sharedService.getModalChangedData().subscribe(() => {
      this.GetAllUsers();
      dialog.closeAll();
    });
  }

  ngOnInit(): void {
    this.isAuthenticated = this.isUserAuthenticated();
    if (this.user?.avatar != "") {
      this.source = "" + this.user?.avatar;
    } else {
      this.source = './assets/images/user-avatar.png';
    }

    this.GetAllUsers();
    this.GetAllTags();
  }

  isUserAuthenticated(): boolean {
    const token = localStorage.getItem('jwt');
    if (this._apiService.checkIfUserIsAuthenticated()) {
      this.user = this._apiService.getUserModelFromJwt(this._jwtHelper.decodeToken(token!));
      console.log(this.user);
      return true;
    }
    return false;
  }

  changeTab(tab: number): void {
    this.selectedTab = tab;
    if (tab == 1) {
      this.GetAllUsers();
    }
    if (tab == 2) {
      this.GetAllTags();
    }
  }

  GetAllUsers(): void {
    let jwt = localStorage.getItem('jwt');
    this._apiService.getAllUsers(environment.backend.baseUrl + 'administration/GetAllUsers?token='+jwt).subscribe((data) => {
      this.users = data;
      this.dataSource = new MatTableDataSource<UserAdministrationModel>(this.users);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  GetAllTags(): void {
    let jwt = localStorage.getItem('jwt');
    this._apiService.getTags(environment.backend.baseUrl + 'administration/GetAllTags?token='+jwt).subscribe((data) => {
      this.tags = data;
    });
  }


  logOut(): void {
    this._apiService.logOut();
    this.isAuthenticated = false;
  }

  //tags

  addOnBlur = true;
  readonly separatorKeysCodes = [ENTER, COMMA] as const;


  add(event: MatChipInputEvent): void {
    const value = (event.value || '').trim();
    if (value) {
      this.saveTags(value);
    }
    event.chipInput!.clear();
  }

  remove(tag: tagModel): void {
    this.removeTag(tag);
  }

  edit(tag: tagModel, event: MatChipEditedEvent) {
    const value = event.value.trim();

    if (!value) {
      this.removeTag(tag);
      return;
    }

    this.updateTag(tag, value);
  }

  saveTags(tag: string): void {
    let jwt = localStorage.getItem('jwt');
    let model = {
      name: tag,
      token: jwt
    }
    this._apiService.sendRequest(environment.backend.baseUrl + 'administration/AddTag', model).subscribe(() => {
      this.GetAllTags();
    });
  }

  removeTag(tag: tagModel): void {
    let jwt = localStorage.getItem('jwt');
    let model = {
      name: tag.name,
      token: jwt
    }
    this._apiService.sendRequest(environment.backend.baseUrl + 'administration/RemoveTag', model).subscribe(() => {
      this.GetAllTags();
    });
  }

  updateTag(tag: tagModel, newName: string): void {
    let jwt = localStorage.getItem('jwt');
    let model = {
      oldName: tag.name,
      newName: newName,
      token: localStorage.getItem('jwt')
    }
    this._apiService.sendUpdateRequest(environment.backend.baseUrl + 'administration/UpdateTag', model).pipe(catchError(error => {
      return of(null)
    })).subscribe(() => {
      this.GetAllTags();
    });
  }

  //users
  selectUser(user: UserAdministrationModel): void {
    this.selectedUser = user;
    if (this.user?.role === 'Owner'){
      if(this.selectedUser.role !== 'Owner'){
        this.openDialog('0', '0');
      }
    }
    else{
      if(this.selectedUser.role !== 'Admin' && this.selectedUser.role !== 'Owner')
        this.openDialog('0', '0');
    }
  }

  openDialog(enterAnimationDuration: string, exitAnimationDuration: string): void {
    this.dialog.open(UserEditDialogComponent, {
      width: '550px',
      enterAnimationDuration,
      exitAnimationDuration,
      data: {email: this.selectedUser.email, role: this.selectedUser.role, banExpirationDate: this.selectedUser.banExpirationDate},
    });
  }
}

