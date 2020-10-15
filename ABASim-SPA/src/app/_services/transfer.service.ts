import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class TransferService {

  data: number;
  state: number;
  // nameData: string;

  constructor() { }

  setData(data: number) {
    this.data = data;
  }

  // setNameData(data: string) {
  //   this.nameData = data;
  // }

  getData() {
    const temp = this.data;
    this.clearData();
    return temp;
  }

  // getNameData() {
  //   const temp = this.nameData;
  //   this.clearNameData();
  //   return temp;
  // }

  clearData() {
    this.data = undefined;
  }

  // clearNameData() {
  //   this.nameData = undefined;
  // }

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
