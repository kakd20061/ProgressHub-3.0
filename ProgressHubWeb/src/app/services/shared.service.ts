import { Injectable } from '@angular/core';
import {Observable, Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  constructor() { }

  private subject = new Subject<any>();
  sendModalChangedData() {
    this.subject.next("sent");
  }
  getModalChangedData(): Observable<any>{
    return this.subject.asObservable();
  }
}
