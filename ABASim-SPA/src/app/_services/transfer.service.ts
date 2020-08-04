import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class TransferService {

  data: number;
  state: number;

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

  setState(data: number) {
    this.state = data;
  }

  getState() {
    const temp = this.state;
    this.clearState();
    return temp;
  }

  clearState() {
    this.state = undefined;
  }
}
