import {Component, ViewChild, AfterViewInit, inject} from '@angular/core';
import {userModel} from "../../models/userModel";
import {JwtHelperService} from "@auth0/angular-jwt";
import {AuthService} from "../../services/auth.service";
import {Validators} from "@angular/forms";
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
export interface Fruit {
  name: string;
}
@Component({
  selector: 'app-administration-panel',
  templateUrl: './administration-panel.component.html',
  styleUrls: ['./administration-panel.component.css']
})
@Flowbite()
export class AdministrationPanelComponent{
  //user
  user:userModel|null = {} as userModel;
  isAuthenticated: boolean = false;
  source:string = {} as string;

  users: UserAdministrationModel[] = [];

  //tabs
  selectedTab: number = 0;


  //user management
  displayedColumns: string[] = ['avatar','role','nickname','email','name', 'lastName','dateOfBirth', 'gender'];
  dataSource = {} as MatTableDataSource<UserAdministrationModel>;
  @ViewChild(MatPaginator) paginator: MatPaginator =  {} as MatPaginator;
  @ViewChild(MatSort) sort: MatSort = {} as MatSort;


  //tags management
  tags: tagModel[] = [];
  constructor(private _jwtHelper: JwtHelperService, private _apiService: AuthService) {}

  ngOnInit(): void {
    this.isAuthenticated = this.isUserAuthenticated();
    if(this.user?.avatar!=""){
      this.source = ""+this.user?.avatar;
    }else{
      this.source = './assets/images/user-avatar.png';
    }
    this.GetAllUsers();
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
    if(tab==1){
      this.GetAllUsers();
    }
    if(tab==2){
      this.GetAllTags();
    }
  }

  GetAllUsers(): void {
    this._apiService.getAllUsers(environment.backend.baseUrl+'administration/GetAllUsers').subscribe((data) => {
      this.users = data;
      this.dataSource = new MatTableDataSource<UserAdministrationModel>(this.users);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  GetAllTags(): void {
    this._apiService.getTags(environment.backend.baseUrl+'administration/GetAllTags').subscribe((data) => {
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

  saveTags(task:string): void {
    this._apiService.addTag(environment.backend.baseUrl+'administration/AddTag', task).subscribe(() => {
      this.GetAllTags();
    });
  }

  removeTag(tag: tagModel): void {
    this._apiService.removeTag(environment.backend.baseUrl+'administration/RemoveTag', tag.name).subscribe(() => {
      this.GetAllTags();
    });
  }

  updateTag(tag: tagModel, newName:string): void {
    let model = {
      oldName: tag.name,
      newName: newName
    }
    this._apiService.updateTag(environment.backend.baseUrl+'administration/UpdateTag', model).pipe(catchError(error => {return of(null)})).subscribe(() => {
      this.GetAllTags();
    });
  }
}
