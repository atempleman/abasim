import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { League } from '../_models/league';
import { LeagueScoring } from '../_models/leagueScoring';
import { LeagueOther } from '../_models/leagueOther';
import { LeagueRebounding } from '../_models/leagueRebounding';
import { LeagueDefence } from '../_models/leagueDefence';
import { TransferService } from '../_services/transfer.service';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.css']
})
export class StatsComponent implements OnInit {
  league: League;
  scoringStats: LeagueScoring[] = [];
  otherStats: LeagueOther[] = [];
  reboundingStats: LeagueRebounding[] = [];
  defenceStats: LeagueDefence[] = [];

  pointsStats: LeagueScoring[] = [];
  assistStats: LeagueScoring[] = [];
  fgmStats: LeagueScoring[] = [];
  fgaStats: LeagueScoring[] = [];
  ftmStats: LeagueScoring[] = [];
  ftaStats: LeagueScoring[] = [];
  threefgmStats: LeagueScoring[] = [];
  threefgaStats: LeagueScoring[] = [];

  drebStats: LeagueRebounding[] = [];
  orebsStats: LeagueRebounding[] = [];
  totalRebsStats: LeagueRebounding[] = [];

  stealsStats: LeagueDefence[] = [];
  blocksStats: LeagueDefence[] = [];

  turnoverStats: LeagueOther[] = [];
  minutesStats: LeagueOther[] = [];
  foulStats: LeagueOther[] = [];

  threeperc: LeagueScoring[] = [];
  twoperc: LeagueScoring[] = [];
  ftperc: LeagueScoring[] = [];

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

  pointsSelection = true;
  assistsSelection = false;
  fgmSelection = false;
  fgaSelection = false;
  ftmSelection = false;
  ftaSelection = false;
  threefgmSelection = false;
  threefmaselection = false;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private transferService: TransferService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });

    this.leagueService.getLeagueScoring().subscribe(result => {
      this.scoringStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      // tslint:disable-next-line: max-line-length
      this.pointsStats = this.scoringStats.sort((a, b) => (a.points / a.gamesPlayed) < (b.points / b.gamesPlayed) ? 1 : (a.points / a.gamesPlayed) > (b.points / b.gamesPlayed) ? -1 : 0);
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

    this.leagueService.getLeagueScoring().subscribe(result => {
      this.scoringStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    });
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

    this.leagueService.getLeagueRebounding().subscribe(result => {
      this.reboundingStats = result;
    }, error => {
      this.alertify.error('Error getting rebounding stats');
    }, () => {
      // tslint:disable-next-line: max-line-length
      this.totalRebsStats = this.reboundingStats.sort((a, b) => (a.totalRebounds / a.gamesPlayed) < (b.totalRebounds / b.gamesPlayed) ? 1 : (a.totalRebounds / a.gamesPlayed) > (b.totalRebounds / b.gamesPlayed) ? -1 : 0);
    });
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

    this.leagueService.getLeagueScoring().subscribe(result => {
      this.scoringStats = result;
    }, error => {
      this.alertify.error('Error getting shotting stats');
    }, () => {
      // tslint:disable-next-line: max-line-length
      this.twoperc = this.scoringStats.sort((a, b) => (a.fgm / a.fga) < (b.fgm / b.fga) ? 1 : (a.fgm / a.fga) > (b.fgm / b.fga) ? -1 : 0);
    });
  }

  defenceClick() {
    this.otherSelection = false;
    this.shootingSelection = false;
    this.scoringSelection = false;
    this.reboundingSelection = false;
    this.defenceSelection = true;

    this.blocksSelection = false;
    this.stealsSelection = true;

    this.leagueService.getLeagueDefence().subscribe(result => {
      this.defenceStats = result;
    }, error => {
      this.alertify.error('Error getting defence stats');
    }, () => {
      // tslint:disable-next-line: max-line-length
      this.stealsStats = this.defenceStats.sort((a, b) => (a.steal / a.gamesPlayed) < (b.steal / b.gamesPlayed) ? 1 : (a.steal / a.gamesPlayed) > (b.steal / b.gamesPlayed) ? -1 : 0);
    });
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

    this.leagueService.getLeagueOther().subscribe(result => {
      this.otherStats = result;
    }, error => {
      this.alertify.error('Error getting scoring stats');
    }, () => {
      // tslint:disable-next-line: max-line-length
      this.turnoverStats = this.otherStats.sort((a, b) => (a.turnovers / a.gamesPlayed) < (b.turnovers / b.gamesPlayed) ? 1 : (a.turnovers / a.gamesPlayed) > (b.turnovers / b.gamesPlayed) ? -1 : 0);
    });
  }

  turnoverClick() {
    this.minuteSelection = false;
    this.foulsSelection = false;
    this.turnoversSelection = true;

    // tslint:disable-next-line: max-line-length
    this.turnoverStats = this.otherStats.sort((a, b) => (a.turnovers / a.gamesPlayed) < (b.turnovers / b.gamesPlayed) ? 1 : (a.turnovers / a.gamesPlayed) > (b.turnovers / b.gamesPlayed) ? -1 : 0);
  }

  foulsClick() {
    this.minuteSelection = false;
    this.turnoversSelection = false;
    this.foulsSelection = true;

    // tslint:disable-next-line: max-line-length
    this.foulStats = this.otherStats.sort((a, b) => (a.fouls / a.gamesPlayed) < (b.fouls / b.gamesPlayed) ? 1 : (a.fouls / a.gamesPlayed) > (b.fouls / b.gamesPlayed) ? -1 : 0);
  }

  mintuesClick() {
    this.turnoversSelection = false;
    this.foulsSelection = false;
    this.minuteSelection = true;

    // tslint:disable-next-line: max-line-length
    this.minutesStats = this.otherStats.sort((a, b) => (a.minutes / a.gamesPlayed) < (b.minutes / b.gamesPlayed) ? 1 : (a.minutes / a.gamesPlayed) > (b.minutes / b.gamesPlayed) ? -1 : 0);
  }

  stealsClick() {
    this.blocksSelection = false;
    this.stealsSelection = true;

    // tslint:disable-next-line: max-line-length
    this.stealsStats = this.defenceStats.sort((a, b) => (a.steal / a.gamesPlayed) < (b.steal / b.gamesPlayed) ? 1 : (a.steal / a.gamesPlayed) > (b.steal / b.gamesPlayed) ? -1 : 0);
  }

  blocksClick() {
    this.stealsSelection = false;
    this.blocksSelection = true;

    // tslint:disable-next-line: max-line-length
    this.blocksStats = this.defenceStats.sort((a, b) => (a.block / a.gamesPlayed) < (b.block / b.gamesPlayed) ? 1 : (a.block / a.gamesPlayed) > (b.block / b.gamesPlayed) ? -1 : 0);
  }

  fgpClick() {
    this.threefgpSelection = false;
    this.ftpSelection = false;
    this.fgpSelection = true;

    // tslint:disable-next-line: max-line-length
    this.twoperc = this.scoringStats.sort((a, b) => (a.fgm / a.fga) < (b.fgm / b.fga) ? 1 : (a.fgm / a.fga) > (b.fgm / b.fga) ? -1 : 0);
  }

  ftpClick() {
    this.fgpSelection = false;
    this.threefgpSelection = false;
    this.ftpSelection = true;

    // tslint:disable-next-line: max-line-length
    this.ftperc = this.scoringStats.sort((a, b) => (a.ftm / a.fta) < (b.ftm / b.fta) ? 1 : (a.ftm / a.fta) > (b.ftm / b.fta) ? -1 : 0);
  }

  threepClick() {
    this.fgpSelection = false;
    this.ftpSelection = false;
    this.threefgpSelection = true;

    // tslint:disable-next-line: max-line-length
    this.threeperc = this.scoringStats.sort((a, b) => (a.threeFgm / a.threeFga) < (b.threeFgm / b.threeFga) ? 1 : (a.threeFgm / a.threeFga) > (b.threeFgm / b.threeFga) ? -1 : 0);
  }

  totelRebClick() {
    this.oRebSelection = false;
    this.dRebSelection = false;
    this.totalRebSelection = true;

    // tslint:disable-next-line: max-line-length
    this.totalRebsStats = this.reboundingStats.sort((a, b) => (a.totalRebounds / a.gamesPlayed) < (b.totalRebounds / b.gamesPlayed) ? 1 : (a.totalRebounds / a.gamesPlayed) > (b.totalRebounds / b.gamesPlayed) ? -1 : 0);
  }

  oRebClick() {
    this.dRebSelection = false;
    this.totalRebSelection = false;
    this.oRebSelection = true;

    // tslint:disable-next-line: max-line-length
    this.orebsStats = this.reboundingStats.sort((a, b) => (a.offensiveRebounds / a.gamesPlayed) < (b.offensiveRebounds / b.gamesPlayed) ? 1 : (a.offensiveRebounds / a.gamesPlayed) > (b.offensiveRebounds / b.gamesPlayed) ? -1 : 0);
  }

  dRebClick() {
    this.totalRebSelection = false;
    this.oRebSelection = false;
    this.dRebSelection = true;

    // tslint:disable-next-line: max-line-length
    this.drebStats = this.reboundingStats.sort((a, b) => (a.defensiveRebounds / a.gamesPlayed) < (b.defensiveRebounds / b.gamesPlayed) ? 1 : (a.defensiveRebounds / a.gamesPlayed) > (b.defensiveRebounds / b.gamesPlayed) ? -1 : 0);
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

    // tslint:disable-next-line: max-line-length
    this.pointsStats = this.scoringStats.sort((a, b) => (a.points / a.gamesPlayed) < (b.points / b.gamesPlayed) ? 1 : (a.points / a.gamesPlayed) > (b.points / b.gamesPlayed) ? -1 : 0);
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

    // tslint:disable-next-line: max-line-length
    this.assistStats = this.scoringStats.sort((a, b) => (a.assists / a.gamesPlayed) < (b.assists / b.gamesPlayed) ? 1 : (a.assists / a.gamesPlayed) > (b.assists / b.gamesPlayed) ? -1 : 0);
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

    // tslint:disable-next-line: max-line-length
    this.fgmStats = this.scoringStats.sort((a, b) => (a.fgm / a.gamesPlayed) < (b.fgm / b.gamesPlayed) ? 1 : (a.fgm / a.gamesPlayed) > (b.fgm / b.gamesPlayed) ? -1 : 0);
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

    // tslint:disable-next-line: max-line-length
    this.fgaStats = this.scoringStats.sort((a, b) => (a.fga / a.gamesPlayed) < (b.fga / b.gamesPlayed) ? 1 : (a.fga / a.gamesPlayed) > (b.fga / b.gamesPlayed) ? -1 : 0);
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

    // tslint:disable-next-line: max-line-length
    this.ftmStats = this.scoringStats.sort((a, b) => (a.ftm / a.gamesPlayed) < (b.ftm / b.gamesPlayed) ? 1 : (a.ftm / a.gamesPlayed) > (b.ftm / b.gamesPlayed) ? -1 : 0);
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

    // tslint:disable-next-line: max-line-length
    this.ftaStats = this.scoringStats.sort((a, b) => (a.fta / a.gamesPlayed) < (b.fta / b.gamesPlayed) ? 1 : (a.fta / a.gamesPlayed) > (b.fta / b.gamesPlayed) ? -1 : 0);
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

    // tslint:disable-next-line: max-line-length
    this.threefgaStats = this.scoringStats.sort((a, b) => (a.threeFga / a.gamesPlayed) < (b.threeFga / b.gamesPlayed) ? 1 : (a.threeFga / a.gamesPlayed) > (b.threeFga / b.gamesPlayed) ? -1 : 0);
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

    // tslint:disable-next-line: max-line-length
    this.threefgmStats = this.scoringStats.sort((a, b) => (a.threeFgm / a.gamesPlayed) < (b.threeFgm / b.gamesPlayed) ? 1 : (a.threeFgm / a.gamesPlayed) > (b.threeFgm / b.gamesPlayed) ? -1 : 0);
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

  viewPlayer(player: number) {
    this.transferService.setData(player);
    this.router.navigate(['/view-player']);
  }

}
