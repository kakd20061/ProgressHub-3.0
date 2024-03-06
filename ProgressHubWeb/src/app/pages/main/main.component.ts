import { Component } from '@angular/core';
import {IParticlesProps, NgParticlesService} from "@tsparticles/angular";
import {Container, Engine, ILoadParams, tsParticles} from "@tsparticles/engine";
import configs from "@tsparticles/configs";
import {loadSlim} from "@tsparticles/slim";
@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent {

  particlesOptions = configs.parallax;
  constructor(private _ngParticlesService:NgParticlesService) {}

  private SetUpParallax():void {
    this.particlesOptions.background!.color = "transparent";
    this.particlesOptions.particles!.color!.value = "#7ECFB6";
    this.particlesOptions.fullScreen! = true;
    this.particlesOptions.particles!['links'] = {
      enable: true,
      distance: 150,
      color: "#7ECFB6",
      opacity: 0.4,
      width: 1,
    }
  }

  ngOnInit(): void {
    this.SetUpParallax();
    this._ngParticlesService.init(async (engine:Engine) => {
      console.log(engine);
      await loadSlim(engine);
    });
  }
}
