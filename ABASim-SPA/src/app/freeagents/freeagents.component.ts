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
    console.log('available cap space: ' + this.availableCapSpace);
    console.log('contract year 1: ' + this.year1Amount);
    console.log('contract year 2: ' + this.year2Amount);
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

        console.log('test multi');
        console.log(contractOffer);
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
}
