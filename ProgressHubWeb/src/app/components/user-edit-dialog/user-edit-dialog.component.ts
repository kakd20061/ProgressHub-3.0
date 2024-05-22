import { Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../services/auth.service";
import {environment} from "../../../environments/environment";
import {SharedService} from "../../services/shared.service";

@Component({
  selector: 'app-user-edit-dialog',
  templateUrl: './user-edit-dialog.component.html',
  styleUrls: ['./user-edit-dialog.component.css']
})
export class UserEditDialogComponent {
  changeRoleForm = new FormGroup({
    role: new FormControl('1', [Validators.required, Validators.pattern('^[0-1]$')]),
  });
  banUserForm = new FormGroup({
    banDate: new FormControl(''),
  });
  constructor(@Inject(MAT_DIALOG_DATA) public data: {email:string, role: string, banExpirationDate: string}, public _service:AuthService,private sharedService:SharedService) { }

  ngOnInit(): void {
    this.changeRoleForm.get('role')?.setValue(this.data.role === 'Admin' ? '0' : '1');
    this.banUserForm.get('banDate')?.setValue(this.data.banExpirationDate);
    //todo: repair input of tags
  }

  changeRole(): void {
    if (this.changeRoleForm.invalid) {
      return;
    }
    let model = {
      email: this.data.email,
      role: this.changeRoleForm.get('role')?.value,
      token: localStorage.getItem('jwt')
    };
    let url:string = environment.backend.baseUrl+'administration/ChangeUserRole';
    this._service.sendRequest(url, model).subscribe({
      next: () => {
        this.sharedService.sendModalChangedData();
      },
      error: (err) => {
         console.log(err);
      }
    });

  }

  banUser(): void {
    if (this.banUserForm.invalid) {
      return;
    }
    let model = {
      email: this.data.email,
      blockExpirationDate: this.banUserForm.get('banDate')?.value == '' ? null : this.banUserForm.get('banDate')?.value,
      token: localStorage.getItem('jwt')
    };
    let url:string = environment.backend.baseUrl+'administration/BlockUser';
    this._service.sendRequest(url, model).subscribe({
      next: () => {
        this.sharedService.sendModalChangedData();
      },
      error: (err) => {
         console.log(err);
      }
    });
  }

  unbanUser(): void {
    let model = {
      email: this.data.email,
      token: localStorage.getItem('jwt')
    }
    let url:string = environment.backend.baseUrl+'administration/UnblockUser';
    this._service.sendRequest(url,model).subscribe({
      next: () => {
        this.sharedService.sendModalChangedData();
      },
      error: (err) => {
         console.log(err);
      }
    });
  }


}
