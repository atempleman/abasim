import { Component, OnInit } from '@angular/core';
import { League } from '../_models/league';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { Player } from '../_models/player';
import { DepthChart } from '../_models/depthChart';
import { Router } from '@angular/router';
import { PlayerInjury } from '../_models/playerInjury';

@Component({
  selector: 'app-depthchart',
  templateUrl: './depthchart.component.html',
  styleUrls: ['./depthchart.component.css']
})
export class DepthchartComponent implements OnInit {
  isEdit = 0;
  team: Team;
  playingRoster: Player[] = [];
  depthCharts: DepthChart[] = [];
  teamsInjuries: PlayerInjury[] = [];

  pg01Id = 0;
  pg02Id = 0;
  pg03Id = 0;
  sg01Id = 0;
  sg02Id = 0;
  sg03Id = 0;
  sf01Id = 0;
  sf02Id = 0;
  sf03Id = 0;
  pf01Id = 0;
  pf02Id = 0;
  pf03Id = 0;
  c01Id = 0;
  c02Id = 0;
  c03Id = 0;

  injuryPg01Id = 0;
  injuryPg02Id = 0;
  injuryPg03Id = 0;
  injurySg01Id = 0;
  injurySg02Id = 0;
  injurySg03Id = 0;
  injurySf01Id = 0;
  injurySf02Id = 0;
  injurySf03Id = 0;
  injuryPf01Id = 0;
  injuryPf02Id = 0;
  injuryPf03Id = 0;
  injuryC01Id = 0;
  injuryC02Id = 0;
  injuryC03Id = 0;

  constructor(private alertify: AlertifyService, private teamService: TeamService, private authService: AuthService,
    private router: Router) { }

  ngOnInit() {
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.getPlayerInjuries();
      this.getRosterForTeam();
    });
  }

  getPlayerInjuries() {
    this.teamService.getPlayerInjuriesForTeam(this.team.id).subscribe(result => {
      this.teamsInjuries = result;
    }, error => {
      this.alertify.error('Error getting teams injuries');
    });
  }

  goToCoaching() {
    this.router.navigate(['/coaching']);
  }

  goToTeam() {
    this.router.navigate(['/team']);
  }

  goToFreeAgents() {
    this.router.navigate(['/freeagents']);
  }

  goToTrades() {
    this.router.navigate(['/trades']);
  }

  editDepthChart() {
    this.isEdit = 1;
  }

  cancelDepthChart() {
    this.isEdit = 0;
  }

  getRosterForTeam() {
    this.teamService.getRosterForTeam(this.team.id).subscribe(result => {
      this.playingRoster = result;
    }, error => {
      this.alertify.error('Error getting your roster');
    }, () => {
      this.playingRoster.forEach(element => {
        const injured = this.teamsInjuries.find(x => x.playerId === element.id);

        if (injured) {
          const index = this.playingRoster.indexOf(element, 0);
          this.playingRoster.splice(index, 1);
        }
      });
      this.getDepthCharts();
    });
  }

  getDepthCharts() {
    this.teamService.getDepthChartForTeamId(this.team.id).subscribe(result => {
      this.depthCharts = result;
      console.log(this.depthCharts);
    }, error => {
      this.alertify.error('Error getting depth charts');
    });
  }

  getDepthChartValue(position: number, rank: number) {
    const dc = this.depthCharts.find(x => x.position === position && x.depth === rank);

    // Check if the player is injured
    const injured = this.teamsInjuries.find(x => x.playerId === dc.playerId);
    if (injured) {
      if (position === 1 && rank === 1) {
        this.injuryPg01Id = 1;
      } else if (position === 1 && rank === 2) {
        this.injuryPg02Id = 1;
      } else if (position === 1 && rank === 3) {
        this.injuryPg03Id = 1;
      } else if (position === 2 && rank === 1) {
        this.injurySg01Id = 1;
      } else if (position === 2 && rank === 2) {
        this.injurySg02Id = 1;
      } else if (position === 2 && rank === 3) {
        this.injurySg03Id = 1;
      } else if (position === 3 && rank === 1) {
        this.injurySf01Id = 1;
      } else if (position === 3 && rank === 2) {
        this.injurySf02Id = 1;
      } else if (position === 3 && rank === 3) {
        this.injurySf03Id = 1;
      } else if (position === 4 && rank === 1) {
        this.injuryPf01Id = 1;
      } else if (position === 4 && rank === 2) {
        this.injuryPf02Id = 1;
      } else if (position === 4 && rank === 3) {
        this.injuryPf03Id = 1;
      } else if (position === 5 && rank === 1) {
        this.injuryC01Id = 1;
      } else if (position === 5 && rank === 2) {
        this.injuryC02Id = 1;
      } else if (position === 5 && rank === 3) {
        this.injuryC03Id = 1;
      }
    }
    const player = this.playingRoster.find(x => x.id === dc.playerId);
    return player.firstName + ' ' + player.surname;
  }

  getPlayerName(playerId: number) {
    const player = this.playingRoster.find(x => x.id === playerId);
    return player.firstName + ' ' + player.surname;
  }

  getPlayerNameWithInjuryCheck(playerId: number, gtPlayerNumber: number) {
    // Check if the player is injured
    const injured = this.teamsInjuries.find(x => x.playerId === playerId);
    if (injured) {
      if (gtPlayerNumber === 1) {
        this.injuryPg01Id = 1;
      } else if (gtPlayerNumber === 2) {
        this.injuryPg02Id = 1;
      } else if (gtPlayerNumber === 3) {
        this.injuryPg03Id = 1;
      } else if (gtPlayerNumber === 4) {
        this.injurySg01Id = 1;
      } else if (gtPlayerNumber === 5) {
        this.injurySg02Id = 1;
      } else if (gtPlayerNumber === 6) {
        this.injurySg03Id = 1;
      } else if (gtPlayerNumber === 7) {
        this.injurySf01Id = 1;
      } else if (gtPlayerNumber === 8) {
        this.injurySf02Id = 1;
      } else if (gtPlayerNumber === 9) {
        this.injurySf03Id = 1;
      } else if (gtPlayerNumber === 10) {
        this.injuryPf01Id = 1;
      } else if (gtPlayerNumber === 11) {
        this.injuryPf02Id = 1;
      } else if (gtPlayerNumber === 12) {
        this.injuryPf03Id = 1;
      } else if (gtPlayerNumber === 13) {
        this.injuryC01Id = 1;
      } else if (gtPlayerNumber === 14) {
        this.injuryC02Id = 1;
      } else if (gtPlayerNumber === 15) {
        this.injuryC03Id = 1;
      }
    } else {
      const player = this.playingRoster.find(x => x.id === playerId);
      return player.firstName + ' ' + player.surname;
    }
  }

  saveDepthChart() {
    this.depthCharts.forEach(dc => {
      const pid = this.getPlayerIdForDepthChartPosition(dc.position, dc.depth);
      dc.playerId = pid;
    });

    // Now call the service passing the array
    this.teamService.saveDepthCharts(this.depthCharts).subscribe(result => {
      // console.log(result);
    }, error => {
      this.alertify.error('Error saving depth charts');
    }, () => {
      this.alertify.success('Depth chart saved successfully');
      this.isEdit = 0;
    });
  }

  getPlayerIdForDepthChartPosition(pos: number, rank: number) {
    if (pos === 1) {
      if (rank === 1) {
        return +this.pg01Id;
      } else if (rank === 2) {
        return +this.pg02Id;
      } else if (rank === 3) {
        return +this.pg03Id;
      }
    } else if (pos === 2) {
      if (rank === 1) {
        return +this.sg01Id;
      } else if (rank === 2) {
        return +this.sg02Id;
      } else if (rank === 3) {
        return +this.sg03Id;
      }
    } else if (pos === 3) {
      if (rank === 1) {
        return +this.sf01Id;
      } else if (rank === 2) {
        return +this.sf02Id;
      } else if (rank === 3) {
        return +this.sf03Id;
      }
    } else if (pos === 4) {
      if (rank === 1) {
        return +this.pf01Id;
      } else if (rank === 2) {
        return +this.pf02Id;
      } else if (rank === 3) {
        return +this.pf03Id;
      }
    } else if (pos === 5) {
      if (rank === 1) {
        return +this.c01Id;
      } else if (rank === 2) {
        return +this.c02Id;
      } else if (rank === 3) {
        return +this.c03Id;
      }
    }
  }
}
