import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-player',
  templateUrl: './view-player.component.html',
  styleUrls: ['./view-player.component.css']
})
export class ViewPlayerComponent implements OnInit {
  detailsDisplayed = 0;
  statisticsDisplayed = 0;
  gradingDisplayed = 0;
  ratingsDisplayed = 0;
  tendanciesDisplayed = 0;

  constructor() { }

  ngOnInit() {
  }

  playerDetails() {

  }

  statistics() {

  }

  tendacies() {

  }

  ratings() {

  }

  gradings() {

  }

}
