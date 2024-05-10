import { Component } from '@angular/core';
import {ChangePasswordService} from "../../services/change-password.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {verificationModel} from "../../models/verificationModel";
import {AuthService} from "../../services/auth.service";
import {Router} from "@angular/router";
import {changePasswordModel} from "../../models/changePasswordModel";
import {Engine} from "@tsparticles/engine";
import {loadSlim} from "@tsparticles/slim";
import configs from "@tsparticles/configs";
import {NgParticlesService} from "@tsparticles/angular";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  changePasswordForm = new FormGroup({
    password: new FormControl('', [
      Validators.required,
      Validators.pattern(
        '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$'
      ),
    ]),
  });
  private email: string = '';
  isEnabled: boolean = false;
  isValidPassword: boolean = true;
  isSuccess: number = 0;
  isLoading: boolean = false;
  particlesOptions = configs.basic;

  constructor(private _changePasswordService: ChangePasswordService, private _apiService : AuthService, private _router:Router, private _ngParticlesService:NgParticlesService) {}

  //functions-------------------------------------------------------------------
  private SetUpBasic():void {
    this.particlesOptions.background!.color = "#D5F4EE";
    this.particlesOptions.particles!.color!.value = "#3FA99B";
    this.particlesOptions.fullScreen! = true;
    this.particlesOptions.particles!['links'] = {
      enable: true,
      distance: 150,
      color: "#3FA99B",
      opacity: 0.4,
      width: 1,
    }
    this.particlesOptions.particles!.color!.animation! = {
      enable: false,
      speed: 20,
      sync: true,
    };
  }

  ngOnInit():void {
    this.SetUpBasic();
    this._ngParticlesService.init(async (engine:Engine) => {
      console.log(engine);
      await loadSlim(engine);
    });

    this.email = this._changePasswordService.getEmail();
    this.subscribeToValueChanges();
  }
  async changePassword(): Promise<void> {
    if (this.changePasswordForm.valid) {
      this.isValidPassword = true;
      await this.sendRequest();
    } else {
      this.isValidPassword = false;
    }
  }
  async sendRequest(): Promise<void> {

    let model: changePasswordModel = {
      email: this.email,
      password: this.changePasswordForm.value.password,
    };
    this.isLoading = true;
    this._apiService
      .sendRequest(environment.backend.baseUrl+'features/change', model)
      .subscribe({
        next: (res):void => {
          this.isSuccess = 1;
          this.isLoading = false;
          setTimeout(() => {
            this._router.navigate(['/login']);
          }, 3000);
        },
        error: (err):void => {
          this.isSuccess = 2;
          this.isLoading = false;
          setTimeout(() => {
            this.isSuccess = 0;
            this.isEnabled = false;
          }, 3000);
        },
      });
  }

  subscribeToValueChanges(): void {
    this.changePasswordForm.get('password')?.valueChanges.subscribe((value) => {
      this.inputChanged(value);
    });
  }

  inputChanged(event: any): void {
    this.isEnabled = event.length > 0;
  }
}
