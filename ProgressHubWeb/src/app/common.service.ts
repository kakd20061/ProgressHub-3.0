import { Injectable } from '@angular/core';
import {AuthService} from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  constructor(private _apiService:AuthService) { }

  onOtpChange(event: string[]): boolean {
    let counter: number = 0;
    event.forEach((element) => {
      if (!element || element.length == 0 || isNaN(parseInt(element))) {
        counter++;
      }
    });
    return counter == 0;
  }
}
