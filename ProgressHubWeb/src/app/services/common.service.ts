import { Injectable } from '@angular/core';
import {AuthService} from "./auth.service";
import configs from "@tsparticles/configs";
import {IOptions, RecursivePartial} from "@tsparticles/engine";

@Injectable({
  providedIn: 'root'
})
export class CommonService{
  public particlesOptions = {} as  RecursivePartial<IOptions>;
  constructor(private _apiService:AuthService) { this.SetUpBasic() }

  onOtpChange(event: string[]): boolean {
    let counter: number = 0;
    event.forEach((element) => {
      if (!element || element.length == 0 || isNaN(parseInt(element))) {
        counter++;
      }
    });
    return counter == 0;
  }

  private SetUpBasic():void {
    this.particlesOptions = configs.basic;
    this.particlesOptions.background!.color = "#D5F4EE";
    this.particlesOptions.particles!.color!.value = "#3FA99B";
    this.particlesOptions.fullScreen! = true;
    this.particlesOptions.particles!.size!.value = 5;
    this.particlesOptions.particles!['links'] = {
      enable: true,
      distance: 150,
      color: "#3FA99B",
      opacity: 0.4,
      width: 2,
    }
    this.particlesOptions.particles!.color!.animation! = {
      enable: false,
      speed: 20,
      sync: true,
    };
  }
}
