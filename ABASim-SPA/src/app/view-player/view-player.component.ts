import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { AlertifyService } from '../_services/alertify.service';
import { TransferService } from '../_services/transfer.service';
import { PlayerService } from '../_services/player.service';
import { CompletePlayer } from '../_models/completePlayer';
import { League } from '../_models/league';
import { PlayerInjury } from '../_models/playerInjury';
import { Team } from '../_models/team';
import { CareerStats } from '../_models/careerStats';

@Component({
  selector: 'app-view-player',
  templateUrl: './view-player.component.html',
  styleUrls: ['./view-player.component.css']
})
export class ViewPlayerComponent implements OnInit {
  playerId: number;
  detailedPlayer: CompletePlayer;
  careerStats: CareerStats[] = [];
  league: League;
  imageSrc = 'http://placehold.it/150x150';

  statusStats = true;
  statusPlayoffStats = false;
  statusGrades = false;
  statusRatings = false;
  statusTendancies = false;
  statusCareerStats = false;
  statusContract = false;

  playerInjury: PlayerInjury;
  injurySet = 0;

  playersTeam: Team;

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService, private transferService: TransferService,
              private playerService: PlayerService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });

    this.playerId = this.transferService.getData();
    console.log('value: ' + this.playerId);
    this.playerService.playerForPlayerProfileById(this.playerId).subscribe(result => {
      this.detailedPlayer = result;
      this.imageSrc = 'https://nba-players.herokuapp.com/players/' + this.detailedPlayer.surname + '/' + this.detailedPlayer.firstName;
    }, error => {
      this.alertify.error('Error getting player profile');
    });

    this.teamService.getInjuryForPlayer(this.playerId).subscribe(result => {
      this.playerInjury = result;
      if (this.playerInjury) {
        this.injurySet = 1;
      }
    }, error => {
      this.alertify.error('Error checking player injury');
    });

    this.playerService.getCareerStats(this.playerId).subscribe(result => {
      this.careerStats = result;
    }, error => {
      this.alertify.error('Error getting career stats');
    });
  }

  getMinutesAverage() {
    const value = ((this.detailedPlayer.minutesStats / this.detailedPlayer.gamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getMinutesAverageForCareer(stats: CareerStats) {
    const value = ((stats.minutesStats / stats.gamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getFGAverage() {
    const value = (this.detailedPlayer.fgmStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFGAverageForCareer(stats: CareerStats) {
    const value = (stats.fgmStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFGAAverage() {
    const value = (this.detailedPlayer.fgaStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFGAAverageForCareer(stats: CareerStats) {
    const value = (stats.fgaStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFgPercentage() {
    const value = ((this.detailedPlayer.fgmStats) / (this.detailedPlayer.fgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getFgPercentageForCareer(stats: CareerStats) {
    const value = ((stats.fgmStats) / (stats.fgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getThreeFGAverage() {
    const value = (this.detailedPlayer.threeFgmStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFGAverageForCareer(stats: CareerStats) {
    const value = (stats.threeFgmStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFGAAverage() {
    const value = (this.detailedPlayer.threeFgaStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFGAAverageForCareer(stats: CareerStats) {
    const value = (stats.threeFgaStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFgPercentage() {
    const value = ((this.detailedPlayer.threeFgmStats) / (this.detailedPlayer.threeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getThreeFgPercentageForCareer(stats: CareerStats) {
    const value = ((stats.threeFgmStats) / (stats.threeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getFTAverage() {
    const value = (this.detailedPlayer.ftmStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTAverageForCareer(stats: CareerStats) {
    const value = (stats.ftmStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTAAverage() {
    const value = (this.detailedPlayer.ftaStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTAAverageForCareer(stats: CareerStats) {
    const value = (stats.ftaStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTPercentage() {
    const value = ((this.detailedPlayer.ftmStats) / (this.detailedPlayer.ftaStats));
    const display = value.toFixed(3);
    return display;
  }

  getFTPercentageForCareer(stats: CareerStats) {
    const value = ((stats.ftmStats) / (stats.ftaStats));
    const display = value.toFixed(3);
    return display;
  }

  getOrebAverage() {
    const value = (this.detailedPlayer.orebsStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getOrebAverageForCareer(stats: CareerStats) {
    const value = (stats.orebsStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getDrebverage() {
    const value = (this.detailedPlayer.drebsStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getDrebverageForCareer(stats: CareerStats) {
    const value = (stats.drebsStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalRebAverage() {
    const totalRebs = this.detailedPlayer.orebsStats + this.detailedPlayer.drebsStats;
    const value = (totalRebs / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalRebAverageForCareer(stats: CareerStats) {
    const totalRebs = stats.orebsStats + stats.drebsStats;
    const value = (totalRebs / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalAstAverage() {
    const value = (this.detailedPlayer.astStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalAstAverageForCareer(stats: CareerStats) {
    const value = (stats.astStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalStlAverage() {
    const value = (this.detailedPlayer.stlStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalStlAverageForCareer(stats: CareerStats) {
    const value = (stats.stlStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalBlkAverage() {
    const value = (this.detailedPlayer.blkStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalBlkAverageForCareer(stats: CareerStats) {
    const value = (stats.blkStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalTovAverage() {
    const value = (this.detailedPlayer.toStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalTovAverageForCareer(stats: CareerStats) {
    const value = (stats.toStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalFoulsAverage() {
    const value = (this.detailedPlayer.flsStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalFoulsAverageForCareer(stats: CareerStats) {
    const value = (stats.flsStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalPointsAverage() {
    const value = (this.detailedPlayer.ptsStats / this.detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalPointsAverageForCareer(stats: CareerStats) {
    const value = (stats.ptsStats / stats.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffMinutesAverage() {
    const value = ((this.detailedPlayer.playoffMinutesStats / this.detailedPlayer.playoffGamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffMinutesAverageForCareer(stats: CareerStats) {
    const value = ((stats.playoffMinutesStats / stats.playoffGamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAverage() {
    const value = (this.detailedPlayer.playoffFgmStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFgmStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAAverage() {
    const value = (this.detailedPlayer.playoffFgaStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFgaStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFgPercentage() {
    const value = ((this.detailedPlayer.playoffFgmStats) / (this.detailedPlayer.playoffFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffFgPercentageForCareer(stats: CareerStats) {
    const value = ((stats.playoffFgmStats) / (stats.playoffFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffThreeFGAverage() {
    const value = (this.detailedPlayer.playoffThreeFgmStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFGAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffThreeFgmStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFGAAverage() {
    const value = (this.detailedPlayer.playoffThreeFgaStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFGAAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffThreeFgaStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFgPercentage() {
    const value = ((this.detailedPlayer.playoffThreeFgmStats) / (this.detailedPlayer.playoffThreeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffThreeFgPercentageForCareer(stats: CareerStats) {
    const value = ((stats.playoffThreeFgmStats) / (stats.playoffThreeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffFTAverage() {
    const value = (this.detailedPlayer.playoffFtmStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFtmStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTAAverage() {
    const value = (this.detailedPlayer.playoffFtaStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTAAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFtaStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTPercentage() {
    const value = ((this.detailedPlayer.playoffFtmStats) / (this.detailedPlayer.playoffFtaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffFTPercentageForCareer(stats: CareerStats) {
    const value = ((stats.playoffFtmStats) / (stats.playoffFtaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffOrebAverage() {
    const value = (this.detailedPlayer.playoffOrebsStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffOrebAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffOrebsStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffDrebverage() {
    const value = (this.detailedPlayer.playoffDrebsStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffDrebverageForCareer(stats: CareerStats) {
    const value = (stats.playoffDrebsStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalRebAverage() {
    const totalRebs = this.detailedPlayer.playoffOrebsStats + this.detailedPlayer.playoffDrebsStats;
    const value = (totalRebs / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalRebAverageForCareer(stats: CareerStats) {
    const totalRebs = stats.playoffOrebsStats + stats.playoffDrebsStats;
    const value = (totalRebs / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalAstAverage() {
    const value = (this.detailedPlayer.playoffAstStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalAstAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffAstStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalStlAverage() {
    const value = (this.detailedPlayer.playoffStlStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalStlAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffStlStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalBlkAverage() {
    const value = (this.detailedPlayer.playoffBlkStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalBlkAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffBlkStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalTovAverage() {
    const value = (this.detailedPlayer.playoffToStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalTovAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffToStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalFoulsAverage() {
    const value = (this.detailedPlayer.playoffFlsStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalFoulsAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffFlsStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalPointsAverage() {
    const value = (this.detailedPlayer.playoffPtsStats / this.detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalPointsAverageForCareer(stats: CareerStats) {
    const value = (stats.playoffPtsStats / stats.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  gradesClick() {
    this.statusStats = false;
    this.statusRatings = false;
    this.statusTendancies = false;
    this.statusCareerStats = false;
    this.statusContract = false;
    this.statusGrades = true;
  }

  statisticsClick() {
    this.statusRatings = false;
    this.statusTendancies = false;
    this.statusGrades = false;
    this.statusCareerStats = false;
    this.statusContract = false;
    this.statusStats = true;
  }

  playoffsStatisticsClick() {
    this.statusRatings = false;
    this.statusTendancies = false;
    this.statusGrades = false;
    this.statusStats = false;
    this.statusCareerStats = false;
    this.statusContract = false;
    this.statusPlayoffStats = true;
  }

  ratingsClick() {
    this.statusTendancies = false;
    this.statusGrades = false;
    this.statusStats = false;
    this.statusCareerStats = false;
    this.statusContract = false;
    this.statusRatings = true;
  }

  tendanciesClick() {
    this.statusGrades = false;
    this.statusStats = false;
    this.statusRatings = false;
    this.statusCareerStats = false;
    this.statusContract = false;
    this.statusTendancies = true;
  }

  careerStatsClick() {
    this.statusGrades = false;
    this.statusStats = false;
    this.statusRatings = false;
    this.statusTendancies = false;
    this.statusContract = false;
    this.statusCareerStats = true;
  }

  contractClick() {
    this.statusGrades = false;
    this.statusStats = false;
    this.statusRatings = false;
    this.statusTendancies = false;
    this.statusCareerStats = false;
    this.statusContract = true;
  }

  viewTeam() {
    // Need to go a call to get the team id
    this.teamService.getTeamForTeamName(this.detailedPlayer.teamName).subscribe(result => {
      this.playersTeam = result;
    }, error => {
      this.alertify.error('Error getting players team');
    }, () => {
      this.transferService.setData(this.playersTeam.id);
      this.router.navigate(['/view-team']);
    });
  }
}
