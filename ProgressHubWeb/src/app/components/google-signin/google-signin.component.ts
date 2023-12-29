import {Component, EventEmitter, Input, Output} from '@angular/core';

declare global {
  interface Window {
    google: any;
  }
}
@Component({
  selector: 'app-google-signin',
  templateUrl: './google-signin.component.html',
  styleUrls: ['./google-signin.component.css'],
})
export class GoogleSigninComponent {
  @Output() loginWithGoogle: EventEmitter<any> = new EventEmitter<any>();
  @Input() isValid: boolean = true;

  createFakeGoogleWrapper = ():{click:()=>void} => {
    const googleLoginWrapper = document.createElement('div');
    googleLoginWrapper.style.display = 'none';
    googleLoginWrapper.classList.add('custom-google-button');
    document.body.appendChild(googleLoginWrapper);
    window.google.accounts.id.renderButton(googleLoginWrapper, {
      type: 'icon',
      width: '200',
    });

    const googleLoginWrapperButton:HTMLElement = googleLoginWrapper.querySelector(
      'div[role=button]'
    ) as HTMLElement;

    return {
      click: ():void => {
        googleLoginWrapperButton?.click();
      },
    };
  };

  handleGoogleLogin():void {
    this.loginWithGoogle.emit(this.createFakeGoogleWrapper());
  }
}
