import { Component } from '@angular/core';
import {ChangePasswordService} from "../change-password.service";

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  private email: string = '';
  constructor(private _changePasswordService: ChangePasswordService) {}

  ngOnInit():void {
    this.email = this._changePasswordService.getEmail();
    console.log(this.email);
  }
}
