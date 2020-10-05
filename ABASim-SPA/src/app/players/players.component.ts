import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { PlayerService } from '../_services/player.service';
import { Player } from '../_models/player';
import { TransferService } from '../_services/transfer.service';
import { AuthService } from '../_services/auth.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-players',
  templateUrl: './players.component.html',
  styleUrls: ['./players.component.css']
})
export class PlayersComponent implements OnInit {
  allPlayers: Player[] = [];
  searchForm: FormGroup;
  positionFilter = 0;

  constructor(private router: Router, private alertify: AlertifyService, private authService: AuthService,
              private transferService: TransferService, private playerService: PlayerService,
              private fb: FormBuilder, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.spinner.show();
    this.getPlayers();

    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

  getPlayers() {
    this.playerService.getAllPlayers().subscribe(result => {
      this.allPlayers = result;
    }, error => {
      this.alertify.error('Error getting players');
    }, () => {
      this.spinner.hide();
    });
  }

  viewPlayer(player: Player) {
    this.transferService.setData(player.id);
    this.router.navigate(['/view-player']);
  }

  filterTable() {
    this.spinner.show();
    // this.displayPaging = 1;
    const filter = this.searchForm.value.filter;

    // Need to call service
    this.playerService.filterPlayers(filter).subscribe(result => {
      this.allPlayers = result;
    }, error => {
      this.alertify.error('Error getting filtered players');
      this.spinner.hide();
    }, () => {
      this.spinner.hide();
    });
  }

  resetFilter() {
    this.spinner.show();
    // this.displayPaging = 0;
    this.getPlayers();

    this.searchForm = this.fb.group({
      filter: ['']
    });
  }

  filterByPos(pos: number) {
    this.spinner.show();
    console.log('ash');
    this.positionFilter = pos;

    if (pos === 0) {
    //   this.displayPaging = 0;
      this.getPlayers();
    } else {
    //   this.displayPaging = 1;
    // Now we need to update the listing appropriately
    this.playerService.getPlayerByPos(this.positionFilter).subscribe(result => {
        this.allPlayers = result;
      }, error => {
        this.alertify.error('Error getting filtered players');
        this.spinner.hide();
      }, () => {
        this.spinner.hide();
      });
    }
  }
}
