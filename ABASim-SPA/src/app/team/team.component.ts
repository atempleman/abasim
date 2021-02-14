import { Component, OnInit, TemplateRef, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { League } from '../_models/league';
import { Team } from '../_models/team';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { WaivedPlayer } from '../_models/waivedPlayer';
import { TransferService } from '../_services/transfer.service';
import { PlayerInjury } from '../_models/playerInjury';
import { CompletePlayer } from '../_models/completePlayer';
import { TeamSalaryCapInfo } from '../_models/teamSalaryCapInfo';
import { PlayerContractDetailed } from '../_models/playerContractDetailed';
import { WaivedContract } from '../_models/waivedContract';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.css']
})
export class TeamComponent implements OnInit {
  league: League;
  team: Team;
  playingRoster: CompletePlayer[] = [];
  isAdmin = 0;
  playerCount = 0;
  selectedPlayer: CompletePlayer;
  message: number;
  statusGrades = 1;
  statusStats = 0;
  statusContracts = 0;
  teamCap: TeamSalaryCapInfo;
  remainingCapSpace = 0;

  primaryColor: string = '22, 24, 100';
  secondaryColor: string = '12,126,120';

  public modalRef: BsModalRef;

  teamsInjuries: PlayerInjury[] = [];
  teamContracts: PlayerContractDetailed[] = [];
  waivedContracts: WaivedContract[] = [];

  constructor(private router: Router, private leagueService: LeagueService, private alertify: AlertifyService,
              private authService: AuthService, private teamService: TeamService, private modalService: BsModalService,
              private transferService: TransferService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

    // get the league object - TODO - roll the league state into the object as a Dto and pass back
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.backgroundStyle();
      this.getPlayerInjuries();
      this.getRosterForTeam();
      this.getSalaryCapDetails();
      this.getTeamContracts();
    });
  }

  getTeamContracts() {
    this.teamService.getTeamContracts(this.team.id).subscribe(result => {
      this.teamContracts = result;
    }, error => {
      this.alertify.error('Error getting team contracts');
    });

    this.teamService.getWaivedContracts(this.team.id).subscribe(result => {
      this.waivedContracts = result;
    }, error => {
      this.alertify.error('Error getting waived contracts');
    });
  }

  getSalaryCapDetails() {
    this.teamService.getTeamSalaryCapDetails(this.team.id).subscribe(result => {
      this.teamCap = result;
    }, error => {
      this.alertify.error('Error getting salary cap details');
    }, () => {
      this.remainingCapSpace = this.teamCap.salaryCapAmount - this.teamCap.currentSalaryAmount;
    });
  }

  getPlayerInjuries() {
    this.teamService.getPlayerInjuriesForTeam(this.team.id).subscribe(result => {
      this.teamsInjuries = result;
    }, error => {
      this.alertify.error('Error getting teams injuries');
    });
  }

  checkIfInjured(playerId: number) {
    const injured = this.teamsInjuries.find(x => x.playerId === playerId);
    if (injured) {
      return 1;
    } else {
      return 0;
    }
  }

  public openModal(template: TemplateRef<any>, player: CompletePlayer) {
    this.selectedPlayer = player;
    this.modalRef = this.modalService.show(template);
  }

  getRosterForTeam() {
    this.teamService.getExtendedRosterForTeam(this.team.id).subscribe(result => {
      this.playingRoster = result;
      this.playerCount = this.playingRoster.length;
    }, error => {
      this.alertify.error('Error getting your roster');
    }, () => {
      console.log(this.playingRoster);
    });
  }

  confirmedWaived() {
    const waivePlayer: WaivedPlayer = {
      teamId: this.team.id,
      playerId: this.selectedPlayer.playerId
    };
    this.teamService.waivePlayer(waivePlayer).subscribe(result => {

    }, error => {
      this.alertify.error('Error waiving player');
    }, () => {
      this.getRosterForTeam();
      this.modalRef.hide();
    });
  }

  viewPlayer(player: ExtendedPlayer) {
    this.transferService.setData(player.playerId);
    this.router.navigate(['/view-player']);
  }

  goToCoaching() {
    this.router.navigate(['/coaching']);
  }

  goToDepthCharts() {
    this.router.navigate(['/depthchart']);
  }

  goToFreeAgents() {
    this.router.navigate(['/freeagents']);
  }

  goToTrades() {
    this.router.navigate(['/trades']);
  }

  gradesClick() {
    this.statusStats = 0;
    this.statusContracts = 0;
    this.statusGrades = 1;
  }

  statisticsClick() {
    this.statusGrades = 0;
    this.statusContracts = 0;
    this.statusStats = 1;
  }

  contractsClick() {
    this.statusGrades = 0;
    this.statusStats = 0;
    this.statusContracts = 1;
  }

  getMinutesAverage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.minutesStats / detailedPlayer.gamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getFGAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.fgmStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFGAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.fgaStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFgPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.fgmStats) / (detailedPlayer.fgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getThreeFGAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.threeFgmStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFGAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.threeFgaStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getThreeFgPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.threeFgmStats) / (detailedPlayer.threeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getFTAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.ftmStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.ftaStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getFTPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.ftmStats) / (detailedPlayer.ftaStats));
    const display = value.toFixed(3);
    return display;
  }

  getOrebAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.orebsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getDrebverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.drebsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalRebAverage(detailedPlayer: CompletePlayer) {
    const totalRebs = detailedPlayer.orebsStats + detailedPlayer.drebsStats;
    const value = (totalRebs / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalAstAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.astStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalStlAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.stlStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalBlkAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.blkStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalTovAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.toStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalFoulsAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.flsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getTotalPointsAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.ptsStats / detailedPlayer.gamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffMinutesAverage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.playoffMinutesStats / detailedPlayer.playoffGamesStats) / 60);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFgmStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFGAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFgaStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFgPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.playoffFgmStats) / (detailedPlayer.playoffFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffThreeFGAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffThreeFgmStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFGAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffThreeFgaStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffThreeFgPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.playoffThreeFgmStats) / (detailedPlayer.playoffThreeFgaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffFTAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFtmStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTAAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFtaStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffFTPercentage(detailedPlayer: CompletePlayer) {
    const value = ((detailedPlayer.playoffFtmStats) / (detailedPlayer.playoffFtaStats));
    const display = value.toFixed(3);
    return display;
  }

  getPlayoffOrebAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffOrebsStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffDrebverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffDrebsStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalRebAverage(detailedPlayer: CompletePlayer) {
    const totalRebs = detailedPlayer.playoffOrebsStats + detailedPlayer.playoffDrebsStats;
    // console.log('total rebs = ' + totalRebs);
    const value = (totalRebs / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalAstAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffAstStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalStlAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffStlStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalBlkAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffBlkStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalTovAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffToStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalFoulsAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffFlsStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  getPlayoffTotalPointsAverage(detailedPlayer: CompletePlayer) {
    const value = (detailedPlayer.playoffPtsStats / detailedPlayer.playoffGamesStats);
    const display = value.toFixed(1);
    return display;
  }

  backgroundStyle() {
    switch (this.team.id) {
      case 2:
        // Toronto
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 3:
        // Milwaukee
        this.primaryColor = '0,71,27';
        this.secondaryColor = '240,235,210';
        break;
      case 4:
        // Miami
        this.primaryColor = '152,0,46';
        this.secondaryColor = '249,160,27';
        break;
      case 5:
        // Denver
        this.primaryColor = '13,34,64';
        this.secondaryColor = '255,198,39';
        break;
      case 6:
        // Lakers
        this.primaryColor = '85,37,130';
        this.secondaryColor = '253,185,39';
        break;
      case 7:
        // Rockets
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 8:
        // Boston
        this.primaryColor = '0, 122, 51';
        this.secondaryColor = '139,111,78';
        break;
      case 9:
        // Indiana
        this.primaryColor = '0,45,98';
        this.secondaryColor = '253,187,48';
        break;
      case 10:
        // Orlando
        this.primaryColor = '0,125,197';
        this.secondaryColor = '196,206,211';
        break;
      case 11:
        // OKC
        this.primaryColor = '0,125,195';
        this.secondaryColor = '239,59,36';
        break;
      case 12:
        // Clippers
        this.primaryColor = '200,16,46';
        this.secondaryColor = '29,66,148';
        break;
      case 13:
        // Dallas
        this.primaryColor = '0,83,188';
        this.secondaryColor = '0,43,92';
        break;
      case 14:
        // 76ers
        this.primaryColor = '0,107,182';
        this.secondaryColor = '237,23,76';
        break;
      case 15:
        // Chicago
        this.primaryColor = '206,17,65';
        this.secondaryColor = '6,25,34';
        break;
      case 16:
        // Charlotte
        this.primaryColor = '29,17,96';
        this.secondaryColor = '0,120,140';
        break;
      case 17:
        // Utah
        this.primaryColor = '0,43,92';
        this.secondaryColor = '0,71,27';
        break;
      case 18:
        // Phoenix
        this.primaryColor = '29,17,96';
        this.secondaryColor = '229,95,32';
        break;
      case 19:
        // Memphis
        this.primaryColor = '93,118,169';
        this.secondaryColor = '18,23,63';
        break;
      case 20:
        // Brooklyn
        this.primaryColor = '0,0,0';
        this.secondaryColor = '119,125,132';
        break;
      case 21:
        // Detroit
        this.primaryColor = '200,16,46';
        this.secondaryColor = '29,66,138';
        break;
      case 22:
        // Washington
        this.primaryColor = '0,43,92';
        this.secondaryColor = '227,24,55';
        break;
      case 23:
        // Portland
        this.primaryColor = '224,58,62';
        this.secondaryColor = '6,25,34';
        break;
      case 24:
        // Sacromento
        this.primaryColor = '91,43,130';
        this.secondaryColor = '99,113,122';
        break;
      case 25:
        // Spurs
        this.primaryColor = '196,206,211';
        this.secondaryColor = '6,25,34';
        break;
      case 26:
        // Knicks
        this.primaryColor = '0,107,182';
        this.secondaryColor = '245,132,38';
        break;
      case 27:
        // Cavs
        this.primaryColor = '134,0,56';
        this.secondaryColor = '4,30,66';
        break;
      case 28:
        // Atlanta
        this.primaryColor = '225,68,52';
        this.secondaryColor = '196,214,0';
        break;
      case 29:
        // Minnesota
        this.primaryColor = '12,35,64';
        this.secondaryColor = '35,97,146';
        break;
      case 30:
        // GSW
        this.primaryColor = '29,66,138';
        this.secondaryColor = '255,199,44';
        break;
      case 32:
        // New Orleans
        this.primaryColor = '0,22,65';
        this.secondaryColor = '225,58,62';
        break;
      default:
        this.primaryColor = '22, 24, 100';
        this.secondaryColor = '12, 126, 120';
        break;
    }
  }

}
