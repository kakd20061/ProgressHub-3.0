import {Component, SimpleChanges} from '@angular/core';
import {Input, Output} from '@angular/core';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.css']
})
export class AlertsComponent {
  @Input() message: string = '';
  @Input() IsSuccess: boolean = true;
  @Input() Show: boolean = false;
  @Input() AutoHideTimeInSec: number = 0;
  //todo: show only for 10 sec
  ngOnChanges(changes: SimpleChanges) {
    if (changes['Show']) {
      if(this.Show){
        this.showTagMsg();
      }
    }
  }
  hideTagMsg(): void {
    let tagMsg:any;
    if(this.IsSuccess){
      tagMsg = document.querySelector('.tagMsgG')!;

    }
    else{
      tagMsg = document.querySelector('.tagMsgE')!;
    }
    if(!tagMsg.classList.contains('opacity-0') && !tagMsg.classList.contains('hidden')){
      tagMsg.classList.add('opacity-0');
      setTimeout(() => {tagMsg.classList.add('hidden');}, 300);
    }
  }

  showTagMsg(): void {
    let tagMsg : any;
    if(this.IsSuccess){
      tagMsg = document.querySelector('.tagMsgG')!;
    }
    else{
      tagMsg = document.querySelector('.tagMsgE')!;
    }
    if(tagMsg && tagMsg.classList.contains('opacity-0') && tagMsg.classList.contains('hidden')){
      tagMsg.classList.remove('opacity-0');
      tagMsg.classList.remove('hidden');
    }
    if(this.AutoHideTimeInSec > 0){
      setTimeout(() => {this.hideTagMsg()}, this.AutoHideTimeInSec*1000);
    }
  }
}
