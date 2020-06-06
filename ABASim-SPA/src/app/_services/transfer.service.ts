import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class TransferService {

  data: number;

  constructor() { }

  setData(data: number) {
    this.data = data;
  }

  getData() {
    const temp = this.data;
    this.clearData();
    return temp;
  }

  clearData() {
    this.data = undefined;
  }
}
