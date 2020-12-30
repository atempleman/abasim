import { Component, OnInit, TemplateRef } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { League } from '../_models/league';
import { AlertifyService } from '../_services/alertify.service';
import { Player } from '../_models/player';
import { PlayerService } from '../_services/player.service';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { Router } from '@angular/router';
import { TransferService } from '../_services/transfer.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { SignedPlayer } from '../_models/signedPlayer';
import { NgxSpinnerService } from 'ngx-spinner';
import { PlayerInjury } from '../_models/playerInjury';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { TeamSalaryCapInfo } from '../_models/teamSalaryCapInfo';
import { ContractOffer } from '../_models/contractOffer';

@Component({
  selector: 'app-freeagents',
  templateUrl: './freeagents.component.html',
  styleUrls: ['./freeagents.component.css']
})
export class FreeagentsComponent implements OnInit {
  league: League;
  team: Team;
  rosterSpotAvailable = true;
  freeAgents: Player[] = [];
  selectedPlayer: Player;
  public modalRef: BsModalRef;
  teamsInjuries: PlayerInjury[] = [];
  searchForm: FormGroup;
  positionFilter = 0;
  displayPaging = 0;

  contractYears = 1;
  year1Amount = 1000000;
  year2Amount = 0;
  year3Amount = 0;
  year4Amount = 0;
  year5Amount = 0;
  guarenteed1 = true;
  guarenteed2 = 0;
  guarenteed3 = 0;
  guarenteed4 = 0;
  guarenteed5 = 0;
  option = 1;

  primaryColor: string = '22, 24, 100';
  secondaryColor: string = '12,126,120';

  selectControl: FormControl = new FormControl();
  optionControl: FormControl = new FormControl();

  capInfo: TeamSalaryCapInfo;
  availableCapSpace = 0;
  contractOffers: ContractOffer[] = [];
  viewedOffer: ContractOffer;

  constructor(private alertify: AlertifyService, private playerService: PlayerService, private teamService: TeamService,
              private authService: AuthService, private router: Router, private transferService: TransferService,
              private modalService: BsModalService, private spinner: NgxSpinnerService, private fb: FormBuilder,
              private leagueServie: LeagueService) { }

  ngOnInit() {
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.backgroundStyle();
      this.CheckRosterSpots();
      this.GetSalaryCapDetails();
      this.GetContractOffers();
    });

    this.leagueServie.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league details');
    });

    this.GetFreeAgents();

    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

  GetContractOffers() {
    this.teamService.getContractOffersForTeam(this.team.id).subscribe(result => {
      this.contractOffers = result;
    }, error => {
      this.alertify.error('Error getting contract offers made');
    });
  }

  GetContractYears(offer: ContractOffer) {
    let years = 0;
    if (offer.yearFive > 0) {
      years = 5;
    } else if (offer.yearFour > 0) {
      years = 4;
    } else if (offer.yearThree > 0) {
      years = 3;
    } else if (offer.yearTwo > 0) {
      years = 2;
    } else if (offer.yearOne > 0) {
      years = 1;
    }
    return years;
  }

  GetSalaryCapDetails() {
    this.teamService.getTeamSalaryCapDetails(this.team.id).subscribe(result => {
      this.capInfo = result;
    }, error => {
      this.alertify.error('Error getting salary cap details');
    }, () => {
      this.availableCapSpace = this.capInfo.salaryCapAmount - this.capInfo.currentSalaryAmount;
    });
  }

  GetFreeAgents() {
    this.spinner.show();

    // Get the freeagents player listing
    this.playerService.getFreeAgents().subscribe(result => {
      this.freeAgents = result;
    }, error => {
      this.alertify.error('Error getting free agents');
    }, () => {
      this.getFreeAgentInjuries();
      this.spinner.hide();
    });
  }

  getFreeAgentInjuries() {
    this.teamService.getInjruiesForFreeAgents().subscribe(result => {
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

  CheckRosterSpots() {
    this.teamService.rosterSpotCheck(this.team.id).subscribe(result => {
      this.rosterSpotAvailable = result;
      console.log(this.rosterSpotAvailable);
    }, error => {
      this.alertify.error('Error checking roster spots');
    });
  }

  viewPlayer(player: Player) {
    this.transferService.setData(player.id);
    this.router.navigate(['/view-player']);
  }

  signPlayer() {
    const signedPlayer: SignedPlayer = {
      teamId: this.team.id,
      playerId: this.selectedPlayer.id
    };

    this.CheckRosterSpots();

    this.teamService.signPlayer(signedPlayer).subscribe(result => {

    }, error => {
      this.alertify.error('Error signing player');
    }, () => {
      this.modalRef.hide();
      this.GetFreeAgents();
      this.alertify.success('Player signed successfully');
    });
  }

  public openModal(template: TemplateRef<any>, player: Player) {
    this.selectedPlayer = player;
    this.modalRef = this.modalService.show(template);
  }

  public openViewModal(template: TemplateRef<any>, offer: ContractOffer) {

    this.viewedOffer = offer;
    this.modalRef = this.modalService.show(template);
  }

  goToTeam() {
    this.router.navigate(['/team']);
  }

  goToDepthCharts() {
    this.router.navigate(['/depthchart']);
  }

  goToCoaching() {
    this.router.navigate(['/coaching']);
  }

  goToTrades() {
    this.router.navigate(['/trades']);
  }

  filterByPos(pos: number) {
    this.spinner.show();
    this.positionFilter = pos;

    if (pos === 0) {
      // this.displayPaging = 0;

      this.GetFreeAgents();
    } else {
      // this.displayPaging = 1;

      // Now we need to update the listing appropriately
      this.playerService.getFreeAgentsByPos(this.positionFilter).subscribe(result => {
        this.freeAgents = result;
      }, error => {
        this.alertify.error('Error getting filtered players');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }
  }

  filterTable() {
    this.spinner.show();
    this.displayPaging = 1;
    const filter = this.searchForm.value.filter;

    // Need to call service
    this.playerService.filterFreeAgents(filter).subscribe(result => {
      this.freeAgents = result;
    }, error => {
      this.alertify.error('Error getting filtered players');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  resetFilter() {
    this.spinner.show();
    this.displayPaging = 0;
    this.GetFreeAgents();

    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

  yearsChanged() {
    this.contractYears = this.selectControl.value;
  }

  cancelContract() {
    this.teamService.deleteFreeAgentOffer(this.viewedOffer.contractId).subscribe(result => {

    }, error => {
      this.alertify.error('Error cancelling contract');
    }, () => {
      this.alertify.success('Contract offer has been cancelled');
      this.GetContractOffers();
      this.modalRef.hide();
    });
  }

  offerContract() {
    if (this.year1Amount < 1000000) {
      this.alertify.error('Error in offer - Minimum contract is $1,000,000 per season');
    } else if (this.year2Amount !== 0 && this.year2Amount < 1000000) {
      this.alertify.error('Error in offer - Minimum contract is $1,000,000 per season');
    } else if (this.year3Amount !== 0 && this.year3Amount < 1000000) {
      this.alertify.error('Error in offer - Minimum contract is $1,000,000 per season');
    } else if (this.year4Amount !== 0 && this.year4Amount < 1000000) {
      this.alertify.error('Error in offer - Minimum contract is $1,000,000 per season');
    } else if (this.year5Amount !== 0 && this.year5Amount < 1000000) {
      this.alertify.error('Error in offer - Minimum contract is $1,000,000 per season');
    } else {
      if (this.availableCapSpace < this.year1Amount && this.year1Amount !== 1000000) {
        if (this.availableCapSpace < 1000000) {
          this.alertify.error('You cannot afford this contract. You can offer a minimum contract of $1,000,000');
        } else {
          this.alertify.error('You cannot afford this contract. You can offer up to $' + this.availableCapSpace);
        }
      } else {
        this.alertify.success('Valid contract');

        let g1 = 0;
        let g2 = 0;
        let g3 = 0;
        let g4 = 0;
        let g5 = 0;

        // Fix guarentees
        if (this.guarenteed1) {
          g1 = 1;
        }

        if (this.guarenteed2) {
          g2 = 1;
        }

        if (this.guarenteed3) {
          g3 = 1;
        }

        if (this.guarenteed4) {
          g4 = 1;
        }

        if (this.guarenteed5) {
          g5 = 1;
        }

        // Now determine the option values
        let to = 0;
        let po = 0;

        if (this.option === 0) {
          to = 0;
          po = 0;
        } else if (this.option === 1) {
          to = 1;
          po = 0;
        } else if (this.option === 2) {
          to = 0;
          po = 1;
        }

        // Now to create the offer
        const contractOffer: ContractOffer = {
          playerId: this.selectedPlayer.id,
          teamId: this.team.id,
          yearOne: +this.year1Amount,
          guranteedOne: g1,
          yearTwo: +this.year2Amount,
          guranteedTwo: g2,
          yearThree: +this.year3Amount,
          guranteedThree: g3,
          yearFour: +this.year4Amount,
          guranteedFour: g4,
          yearFive: +this.year5Amount,
          guranteedFive: g5,
          teamOption: to,
          playerOption: po,
          daySubmitted: this.league.day,
          stateSubmitted: this.league.stateId,
          decision: 0,
          playerName: this.selectedPlayer.firstName + ' ' + this.selectedPlayer.surname,
          contractId: 0
        };

        this.teamService.saveContractOffer(contractOffer).subscribe(result => {

        }, error => {
          this.alertify.error('Error making contract offer');
        }, () => {
          this.alertify.success('Contract offer has been made');
          this.GetContractOffers();
          this.modalRef.hide();

          this.contractYears = 1;
          this.year1Amount = 1000000;
          this.year2Amount = 0;
          this.year3Amount = 0;
          this.year4Amount = 0;
          this.year5Amount = 0;
          this.option = 0;
          this.guarenteed1 = true;
          this.guarenteed2 = 0;
          this.guarenteed3 = 0;
          this.guarenteed4 = 0;
          this.guarenteed5 = 0;

          // Need to update the displayed cap space
          this.GetSalaryCapDetails();
        });
      }
    }
  }

  minAmount() {
    if (this.contractYears === 1) {
      this.year1Amount = 1000000;
    } else if (this.contractYears === 2) {
      this.year1Amount = 1000000;
      this.year2Amount = 1000000;
    } else if (this.contractYears === 3) {
      this.year1Amount = 1000000;
      this.year2Amount = 1000000;
      this.year3Amount = 1000000;
    } else if (this.contractYears === 4) {
      this.year1Amount = 1000000;
      this.year2Amount = 1000000;
      this.year3Amount = 1000000;
      this.year4Amount = 1000000;
    } else if (this.contractYears === 5) {
      this.year1Amount = 1000000;
      this.year2Amount = 1000000;
      this.year3Amount = 1000000;
      this.year4Amount = 1000000;
      this.year5Amount = 1000000;
    }
  }

  maxAmount() {
    if (this.contractYears === 1) {
      this.year1Amount = 25000000;
    } else if (this.contractYears === 2) {
      this.year1Amount = 25000000;
      this.year2Amount = 25000000;
    } else if (this.contractYears === 3) {
      this.year1Amount = 25000000;
      this.year2Amount = 25000000;
      this.year3Amount = 25000000;
    } else if (this.contractYears === 4) {
      this.year1Amount = 25000000;
      this.year2Amount = 25000000;
      this.year3Amount = 25000000;
      this.year4Amount = 25000000;
    } else if (this.contractYears === 5) {
      this.year1Amount = 25000000;
      this.year2Amount = 25000000;
      this.year3Amount = 25000000;
      this.year4Amount = 25000000;
      this.year5Amount = 25000000;
    }
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
