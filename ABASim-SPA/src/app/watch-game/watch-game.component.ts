import { Component, OnInit } from '@angular/core';
import { LeagueService } from '../_services/league.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { TransferService } from '../_services/transfer.service';
import { GameDetails } from '../_models/gameDetails';
import { PlayByPlay } from '../_models/playByPlay';
import { Router } from '@angular/router';
import { League } from '../_models/league';

@Component({
  selector: 'app-watch-game',
  templateUrl: './watch-game.component.html',
  styleUrls: ['./watch-game.component.css']
})
export class WatchGameComponent implements OnInit {
  league: League;
  gameId: number;
  gameDetails: GameDetails;
  gameBegun = 0;
  playByPlays: PlayByPlay[] = [];
  playNumber = 0;
  displayedPlayByPlays: PlayByPlay[] = [];
  numberOfPlays = 0;
  playNo = 0;
  displayBoxScoresButtons = 0;

  constructor(private alertify: AlertifyService, private authService: AuthService, private leagueService: LeagueService,
              private transferService: TransferService, private router: Router) { }

  ngOnInit() {
    this.gameId = this.transferService.getData();

    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    }, () => {
      this.getGameDetails();
    });
  }

  getGameDetails() {
    if (this.league.stateId === 6) {
      this.leagueService.getGameDetailsPreseason(this.gameId).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      });
    } else if (this.league.stateId === 7) {
      this.leagueService.getGameDetailsSeason(this.gameId).subscribe(result => {
        this.gameDetails = result;
      }, error => {
        this.alertify.error('Error getting game details');
      });
    }
  }

  beginGame() {
    this.gameBegun = 1;

    this.leagueService.getPlayByPlaysForId(this.gameId).subscribe(result => {
      this.playByPlays = result;
      const element = this.playByPlays[this.playByPlays.length - 1];
      this.numberOfPlays = element.ordering;

      this.playByPlays.sort((n1, n2) => {
        if (n1.ordering > n2.ordering) {
            return 1;
        }
        if (n1.ordering < n2.ordering) {
            return -1;
        }
        return 0;
      });
    }, error => {
      this.alertify.error('Error getting Play by Play');
    }, () => {
      // const refreshId = setInterval(() => {
      //   this.displayingPlayByPlays();
      //   if(this.numberOfPlays === this.playNumber) {
      //     clearInterval(refreshId);
      //     this.displayBoxScoresButtons = 1;
      //   }
      // }, 10000);
      const refreshId = setInterval(() => {
        this.displayPlays();
        console.log(this.numberOfPlays);
        if (this.numberOfPlays === this.playNo) {
          this.displayBoxScoresButtons = 1;
          clearInterval(refreshId);
        }
      }, 1000);
    });
  }

  displayPlays() {
    const filtered = this.playByPlays[this.playNo];
    this.displayedPlayByPlays.push(filtered);
    this.displayedPlayByPlays.sort((n1, n2) => {
      if (n1.ordering > n2.ordering) {
          return -1;
      }
      if (n1.ordering < n2.ordering) {
          return 1;
      }
      return 0;
    });
    this.playNo++;
  }

  displayingPlayByPlays() {
    const filtered = this.playByPlays.filter(x => x.playNumber === this.playNumber);

    filtered.forEach(element => {
      this.displayedPlayByPlays.push(element);
    });

    this.displayedPlayByPlays.sort((n1, n2) => {
      if (n1.ordering > n2.ordering) {
          return -1;
      }
      if (n1.ordering < n2.ordering) {
          return 1;
      }
      return 0;
    });

    this.playNumber++;
  }

  viewBoxScore(gameId: number) {
    this.transferService.setData(gameId);
    this.router.navigate(['/box-score']);
  }

}
