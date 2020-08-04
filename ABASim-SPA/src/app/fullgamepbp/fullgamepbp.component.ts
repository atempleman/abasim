import { Component, OnInit } from '@angular/core';
import { League } from '../_models/league';
import { GameDetails } from '../_models/gameDetails';
import { PlayByPlay } from '../_models/playByPlay';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { TransferService } from '../_services/transfer.service';
import { LeagueService } from '../_services/league.service';

@Component({
  selector: 'app-fullgamepbp',
  templateUrl: './fullgamepbp.component.html',
  styleUrls: ['./fullgamepbp.component.css']
})
export class FullgamepbpComponent implements OnInit {
  league: League;
  gameId: number;
  state: number;
  gameDetails: GameDetails;
  gameBegun = 0;
  playByPlays: PlayByPlay[] = [];
  playNumber = 0;
  // displayedPlayByPlays: PlayByPlay[] = [];
  numberOfPlays = 0;
  playNo = 0;
  displayBoxScoresButtons = 0;

  constructor(private alertify: AlertifyService, private authService: AuthService, private leagueService: LeagueService,
              private transferService: TransferService, private router: Router) { }

  ngOnInit() {
    this.gameId = this.transferService.getData();
    this.state = this.transferService.getState();

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    }, () => {
      this.getGameDetails();
    });
  }

  getGameDetails() {
    if (this.state === 0) {
      this.leagueService.getGameDetailsPreseason(this.gameId).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      }, () => {
        this.leagueService.getPlayByPlaysForId(this.gameId).subscribe(result => {
          this.playByPlays = result;
          const element = this.playByPlays[this.playByPlays.length - 1];
          this.numberOfPlays = element.ordering;
          this.playByPlays.sort((n1, n2) => {
            if (n1.ordering < n2.ordering) {
                return -1;
            }
            if (n1.ordering > n2.ordering) {
                return 1;
            }
            return 0;
          });
        }, error => {
          this.alertify.error('Error getting Play by Play');
        }, () => {
        });
      });
    } else if (this.state === 1) {
      this.leagueService.getGameDetailsSeason(this.gameId).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      }, () => {
        this.leagueService.getPlayByPlaysForId(this.gameId).subscribe(result => {
          this.playByPlays = result;
          const element = this.playByPlays[this.playByPlays.length - 1];
          this.numberOfPlays = element.ordering;
          this.playByPlays.sort((n1, n2) => {
            if (n1.ordering < n2.ordering) {
                return -1;
            }
            if (n1.ordering > n2.ordering) {
                return 1;
            }
            return 0;
          });
        }, error => {
          this.alertify.error('Error getting Play by Play');
        }, () => {
        });
      });
    } else if (this.state === 2) {
      this.leagueService.getGameDetailsPlayoffs(this.gameId).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      }, () => {
        this.leagueService.getPlayoffsPlayByPlaysForId(this.gameId).subscribe(result => {
          this.playByPlays = result;
          const element = this.playByPlays[this.playByPlays.length - 1];
          this.numberOfPlays = element.ordering;
          this.playByPlays.sort((n1, n2) => {
            if (n1.ordering < n2.ordering) {
                return -1;
            }
            if (n1.ordering > n2.ordering) {
                return 1;
            }
            return 0;
          });
        }, error => {
          this.alertify.error('Error getting Play by Play');
        }, () => {
        });
      });
    }
  }

  viewBoxScore(gameId: number) {
    this.transferService.setData(gameId);
    this.router.navigate(['/box-score']);
  }
}
