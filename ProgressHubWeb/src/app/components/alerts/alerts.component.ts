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
  //todo: show only for 10 sec
  ngOnChanges(changes: SimpleChanges) {
    if (changes['Show']) {
      if(this.Show){
        this.showTagMsg();
      }
    }
  }
  hideTagMsg(): void {
    if(this.IsSuccess){
      let tagMsg = document.querySelector('.tagMsgG')!;
      if(!tagMsg.classList.contains('opacity-0') && !tagMsg.classList.contains('hidden')){
        tagMsg.classList.add('opacity-0');

        setTimeout(() => {
          tagMsg.classList.add('hidden');
        }, 300);
      }
    }
    else{
      let tagMsg = document.querySelector('.tagMsgE')!;
      if(!tagMsg.classList.contains('opacity-0') && !tagMsg.classList.contains('hidden')){
        tagMsg.classList.add('opacity-0');

        setTimeout(() => {
          tagMsg.classList.add('hidden');
        }, 300);
      }
    }
  }

  showTagMsg(): void {
    if(this.IsSuccess){
      let tagMsg = document.querySelector('.tagMsgG')!;
      if(tagMsg && tagMsg.classList.contains('opacity-0') && tagMsg.classList.contains('hidden')){
        tagMsg.classList.remove('opacity-0');
        tagMsg.classList.remove('hidden');
      }
    }
    else{
      let tagMsg = document.querySelector('.tagMsgE')!;
      if(tagMsg && tagMsg.classList.contains('opacity-0') && tagMsg.classList.contains('hidden')){
        tagMsg.classList.remove('opacity-0');
        tagMsg.classList.remove('hidden');
      }
    }
  }
}
