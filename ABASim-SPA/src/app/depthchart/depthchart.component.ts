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

  constructor(private alertify: AlertifyService, private teamService: TeamService, private authService: AuthService,
              private router: Router) { }

  ngOnInit() {
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.getRosterForTeam();
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
    const player = this.playingRoster.find(x => x.id === dc.playerId);
    return player.firstName + ' ' + player.surname;
  }

  getPlayerName(playerId: number) {
    const player = this.playingRoster.find(x => x.id === playerId);
    return player.firstName + ' ' + player.surname;
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
