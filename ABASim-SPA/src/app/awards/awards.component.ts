import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { League } from '../_models/league';
import { Votes } from '../_models/votes';
import { AlertifyService } from '../_services/alertify.service';
import { LeagueService } from '../_services/league.service';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-awards',
  templateUrl: './awards.component.html',
  styleUrls: ['./awards.component.css']
})
export class AwardsComponent implements OnInit {
  league: League;
  mvpList: Votes[] = [];
  dpoyList: Votes[] = [];
  sixthList: Votes[] = [];
  firstTeam: Votes[] = [];
  secondTeam: Votes[] = [];
  thirdTeam: Votes[] = [];
  allnbateams: Votes[] = [];

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private transferService: TransferService, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league details');
    });

    this.leagueService.getMvpTopFive().subscribe(result => {
      this.mvpList = result;
    }, error => {
      this.alertify.error('Error getting MVP leaders');
    });

    this.leagueService.getDpoyTopFive().subscribe(result => {
      this.dpoyList = result;
    }, error => {
      this.alertify.error('Error getting DPOY leaders');
    });

    this.leagueService.getSixthManTopFive().subscribe(result => {
      this.sixthList = result;
    }, error => {
      this.alertify.error('Error getting 6th man leaders');
    });

    this.leagueService.getAllNBATeams().subscribe(result => {
      this.allnbateams = result;
      console.log(result);
    }, error => {
      this.alertify.error('Error getting All-ABA Teams');
    }, () => {
      this.firstTeam = this.allnbateams.splice(0, 5);
      this.secondTeam = this.allnbateams.splice(0, 5);
      this.thirdTeam = this.allnbateams.splice(0, 5);
    });
  }

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

}
