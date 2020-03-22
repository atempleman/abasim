import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-freeagents',
  templateUrl: './freeagents.component.html',
  styleUrls: ['./freeagents.component.css']
})
export class FreeagentsComponent implements OnInit {
  league: League;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });
  }

}
