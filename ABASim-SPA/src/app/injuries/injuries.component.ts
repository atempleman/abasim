import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { League } from '../_models/league';
import { LeaguePlayerInjury } from '../_models/leaguePlayerInjury';
import { Player } from '../_models/player';
import { AlertifyService } from '../_services/alertify.service';
import { LeagueService } from '../_services/league.service';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-injuries',
  templateUrl: './injuries.component.html',
  styleUrls: ['./injuries.component.css']
})
export class InjuriesComponent implements OnInit {
  leagueInjuries: LeaguePlayerInjury[] = [];
  league: League;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private transferService: TransferService, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.spinner.show();
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league details');
    });

    this.leagueService.getLeagueInjuries().subscribe(result => {
      this.leagueInjuries = result;
    }, error => {
      this.alertify.error('Error getting league injuries');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  viewPlayer(player: Player) {
    this.transferService.setData(player.id);
    this.router.navigate(['/view-player']);
  }

}
