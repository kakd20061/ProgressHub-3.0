import { Pipe, PipeTransform } from '@angular/core';
import {tagModel} from "./models/tagModel";

@Pipe({
  name: 'objectToArray',
})
export class PipeFor implements PipeTransform {
  // The object parameter represents the values of the properties or index.
  transform = (objects: any = []) => {
    return Object.values<tagModel>(objects);
  }
}
