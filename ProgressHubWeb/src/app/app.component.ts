import {Component, OnInit} from '@angular/core';
import { loadFull } from 'tsparticles';
import { tsParticles } from "@tsparticles/engine";
import { NgParticlesService } from "@tsparticles/angular";
import {load} from "@angular-devkit/build-angular/src/utils/server-rendering/esm-in-memory-file-loader";
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

export class AppComponent implements OnInit {
  title = 'ProgressHubWeb';

  constructor(private readonly ngParticlesService: NgParticlesService) {}
  ngOnInit(): void {
    this.ngParticlesService.init(async (engine) => {
      console.log();
      await loadFull(engine);
    });
  }
}
