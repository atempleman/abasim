import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { League } from '../_models/league';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.css']
})
export class StatsComponent implements OnInit {
  league: League;

  scoringSelection = true;
  reboundingSelection = false;
  defenceSelection = false;
  shootingSelection = false;
  otherSelection = false;

  turnoversSelection = false;
  minuteSelection = false;
  foulsSelection = false;

  blocksSelection = false;
  stealsSelection = false;

  fgpSelection = false;
  threefgpSelection = false;
  ftpSelection = false;

  totalRebSelection = false;
  oRebSelection = false;
  dRebSelection = false;

  pointsSelection = false;
  assistsSelection = false;
  fgmSelection = false;
  fgaSelection = false;
  ftmSelection = false;
  ftaSelection = false;
  threefgmSelection = false;
  threefmaselection = false;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });
  }

  scoringClick() {
    this.otherSelection = false;
    this.shootingSelection = false;
    this.reboundingSelection = false;
    this.defenceSelection = false;
    this.scoringSelection = true;

    this.assistsSelection = false;
    this.fgmSelection = false;
    this.fgaSelection = false;
    this.ftmSelection = false;
    this.ftaSelection = false;
    this.threefgmSelection = false;
    this.threefmaselection = false;
    this.pointsSelection = true;
  }

  reboundingClick() {
    this.otherSelection = false;
    this.shootingSelection = false;
    this.scoringSelection = false;
    this.defenceSelection = false;
    this.reboundingSelection = true;

    this.oRebSelection = false;
    this.dRebSelection = false;
    this.totalRebSelection = true;
  }

  shootingClick() {
    this.otherSelection = false;
    this.reboundingSelection = false;
    this.scoringSelection = false;
    this.defenceSelection = false;
    this.shootingSelection = true;

    this.ftpSelection = false;
    this.threefgpSelection = false;
    this.fgpSelection = true;
  }

  defenceClick() {
    this.otherSelection = false;
    this.shootingSelection = false;
    this.scoringSelection = false;
    this.reboundingSelection = false;
    this.defenceSelection = true;

    this.blocksSelection = false;
    this.stealsSelection = true;
  }

  otherClick() {
    this.reboundingSelection = false;
    this.shootingSelection = false;
    this.scoringSelection = false;
    this.defenceSelection = false;
    this.otherSelection = true;

    this.minuteSelection = false;
    this.foulsSelection = false;
    this.turnoversSelection = true;
  }

  turnoverClick() {
    this.minuteSelection = false;
    this.foulsSelection = false;
    this.turnoversSelection = true;
  }

  foulsClick() {
    this.minuteSelection = false;
    this.turnoversSelection = false;
    this.foulsSelection = true;
  }

  mintuesClick() {
    this.turnoversSelection = false;
    this.foulsSelection = false;
    this.minuteSelection = true;
  }

  stealsClick() {
    this.blocksSelection = false;
    this.stealsSelection = true;
  }

  blocksClick() {
    this.stealsSelection = false;
    this.blocksSelection = true;
  }

  fgpClick() {
    this.threefgpSelection = false;
    this.ftpSelection = false;
    this.fgpSelection = true;
  }

  ftpClick() {
    this.fgpSelection = false;
    this.threefgpSelection = false;
    this.ftpSelection = true;
  }

  threepClick() {
    this.fgpSelection = false;
    this.ftpSelection = false;
    this.threefgpSelection = true;
  }

  totelRebClick() {
    this.oRebSelection = false;
    this.dRebSelection = false;
    this.totalRebSelection = true;
  }

  oRebClick() {
    this.dRebSelection = false;
    this.totalRebSelection = false;
    this.oRebSelection = true;
  }

  dRebClick() {
    this.totalRebSelection = false;
    this.oRebSelection = false;
    this.dRebSelection = true;
  }

  pointsClick() {
    this.assistsSelection = false;
    this.fgmSelection = false;
    this.fgaSelection = false;
    this.ftmSelection = false;
    this.ftaSelection = false;
    this.threefgmSelection = false;
    this.threefmaselection = false;
    this.pointsSelection = true;
  }

  assistsClick() {
    this.fgmSelection = false;
    this.fgaSelection = false;
    this.ftmSelection = false;
    this.ftaSelection = false;
    this.threefgmSelection = false;
    this.threefmaselection = false;
    this.pointsSelection = false;
    this.assistsSelection = true;
  }

  fgmClick() {
    this.fgaSelection = false;
    this.ftmSelection = false;
    this.ftaSelection = false;
    this.threefgmSelection = false;
    this.threefmaselection = false;
    this.pointsSelection = false;
    this.assistsSelection = false;
    this.fgmSelection = true;
  }

  fgaClick() {
    this.fgmSelection = false;
    this.ftmSelection = false;
    this.ftaSelection = false;
    this.threefgmSelection = false;
    this.threefmaselection = false;
    this.pointsSelection = false;
    this.assistsSelection = false;
    this.fgaSelection = true;
  }

  ftmClick() {
    this.fgaSelection = false;
    this.fgmSelection = false;
    this.ftaSelection = false;
    this.threefgmSelection = false;
    this.threefmaselection = false;
    this.pointsSelection = false;
    this.assistsSelection = false;
    this.ftmSelection = true;
  }

  ftaClick() {
    this.fgmSelection = false;
    this.ftmSelection = false;
    this.fgaSelection = false;
    this.threefgmSelection = false;
    this.threefmaselection = false;
    this.pointsSelection = false;
    this.assistsSelection = false;
    this.ftaSelection = true;
  }

  threefgaClick() {
    this.fgmSelection = false;
    this.ftmSelection = false;
    this.ftaSelection = false;
    this.threefgmSelection = false;
    this.fgaSelection = false;
    this.pointsSelection = false;
    this.assistsSelection = false;
    this.threefmaselection = true;
  }

  threefgmClick() {
    this.fgmSelection = false;
    this.ftmSelection = false;
    this.ftaSelection = false;
    this.threefmaselection = false;
    this.fgaSelection = false;
    this.pointsSelection = false;
    this.assistsSelection = false;
    this.threefgmSelection = true;
  }

  goToStandings() {
    this.router.navigate(['/standings']);
  }

  goToLeague() {
    this.router.navigate(['/league']);
  }

  goToSchedule() {
    this.router.navigate(['/schedule']);
  }

  goToTransactions() {
    this.router.navigate(['/transactions']);
  }

}
