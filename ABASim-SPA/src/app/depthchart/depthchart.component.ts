import { Component, OnInit } from '@angular/core';
import { League } from '../_models/league';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { Player } from '../_models/player';
import { DepthChart } from '../_models/depthChart';

@Component({
  selector: 'app-depthchart',
  templateUrl: './depthchart.component.html',
  styleUrls: ['./depthchart.component.css']
})
export class DepthchartComponent implements OnInit {
  league: League;
  team: Team;
  playingRoster: Player[] = [];
  depthCharts: DepthChart[] = [];
  newDepthCharts: DepthChart[] = [];
  dcPG01: DepthChart;
  dcPG02: DepthChart;
  dcPG03: DepthChart;
  dcSG01: DepthChart;
  dcSG02: DepthChart;
  dcSG03: DepthChart;
  dcSF01: DepthChart;
  dcSF02: DepthChart;
  dcSF03: DepthChart;
  dcPF01: DepthChart;
  dcPF02: DepthChart;
  dcPF03: DepthChart;
  dcC01: DepthChart;
  dcC02: DepthChart;
  dcC03: DepthChart;

  pg01Id: number;
  pg02Id: number;
  pg03Id: number;

  sg01Id: number;
  sg02Id: number;
  sg03Id: number;

  sf01Id: number;
  sf02Id: number;
  sf03Id: number;

  pf01Id: number;
  pf02Id: number;
  pf03Id: number;

  c01Id: number;
  c02Id: number;
  c03Id: number;

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private teamService: TeamService,
              private authService: AuthService) { }

  ngOnInit() {
    this.leagueService.getLeague().subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting league');
    });

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your team');
    }, () => {
      this.getRosterForTeam();
      this.getDepthCharts();
    });
  }

  getDepthCharts() {
    this.teamService.getDepthChartForTeamId(this.team.id).subscribe(result => {
      this.depthCharts = result;
    }, error => {
      this.alertify.error('Error getting depth charts');
    }, () => {
      // Now need to do stuff with them
      if (this.depthCharts.length === 0) {
        // This is when it is 0, need to do nothing here
        // this.dcPG01.playerId = this.pg01Id;
        this.dcPG01 = {
          id: 0,
          depth: 1,
          playerId: 0,
          position: 1,
          teamId: this.team.id
        };

        this.dcPG02 = {
          id: 0,
          depth: 2,
          playerId: 0,
          position: 1,
          teamId: this.team.id
        };

        this.dcPG03 = {
          id: 0,
          depth: 3,
          playerId: 0,
          position: 1,
          teamId: this.team.id
        };

        this.dcSG01 = {
          id: 0,
          depth: 1,
          playerId: 0,
          position: 2,
          teamId: this.team.id
        };

        this.dcSG02 = {
          id: 0,
          depth: 2,
          playerId: 0,
          position: 2,
          teamId: this.team.id
        };

        this.dcSG03 = {
          id: 0,
          depth: 3,
          playerId: 0,
          position: 2,
          teamId: this.team.id
        };

        this.dcSF01 = {
          id: 0,
          depth: 1,
          playerId: 0,
          position: 3,
          teamId: this.team.id
        };

        this.dcSF02 = {
          id: 0,
          depth: 2,
          playerId: 0,
          position: 3,
          teamId: this.team.id
        };

        this.dcSF03 = {
          id: 0,
          depth: 3,
          playerId: 0,
          position: 3,
          teamId: this.team.id
        };

        this.dcPF01 = {
          id: 0,
          depth: 1,
          playerId: 0,
          position: 4,
          teamId: this.team.id
        };

        this.dcPF02 = {
          id: 0,
          depth: 2,
          playerId: 0,
          position: 4,
          teamId: this.team.id
        };

        this.dcPF03 = {
          id: 0,
          depth: 3,
          playerId: 0,
          position: 4,
          teamId: this.team.id
        };

        this.dcC01 = {
          id: 0,
          depth: 1,
          playerId: 0,
          position: 5,
          teamId: this.team.id
        };

        this.dcC02 = {
          id: 0,
          depth: 2,
          playerId: 0,
          position: 5,
          teamId: this.team.id
        };

        this.dcC03 = {
          id: 0,
          depth: 3,
          playerId: 0,
          position: 5,
          teamId: this.team.id
        };
      } else {
        this.setupDepthChartObjects();
      }
    });
  }

  getRosterForTeam() {
    this.teamService.getRosterForTeam(this.team.id).subscribe(result => {
      this.playingRoster = result;
      // console.log(this.playingRoster);
    }, error => {
      this.alertify.error('Error getting your roster');
    });
  }

  saveDepthChart() {
    console.log('save hit');

    console.log(this.pg01Id);
    console.log(this.dcPG01);

    if (this.pg01Id) {
      this.dcPG01.playerId = +this.pg01Id;
    } else {
      this.dcPG01.playerId = 0;
    }

    if (this.pg02Id) {
      this.dcPG02.playerId = +this.pg02Id;
    } else {
      this.dcPG02.playerId = 0;
    }

    if (this.pg03Id) {
      this.dcPG03.playerId = +this.pg03Id;
    } else {
      this.dcPG03.playerId = 0;
    }

    if (this.sg01Id) {
      this.dcSG01.playerId = +this.sg01Id;
    } else {
      this.dcSG01.playerId = 0;
    }

    if (this.sg02Id) {
      this.dcSG02.playerId = +this.sg02Id;
    } else {
      this.dcSG02.playerId = 0;
    }

    if (this.sg03Id) {
      this.dcSG03.playerId = +this.sg03Id;
    } else {
      this.dcSG03.playerId = 0;
    }

    if (this.sf01Id) {
      this.dcSF01.playerId = +this.sf01Id;
    } else {
      this.dcSF01.playerId = 0;
    }

    if (this.sf02Id) {
      this.dcSF02.playerId = +this.sf02Id;
    } else {
      this.dcSF02.playerId = 0;
    }

    if (this.sf03Id) {
      this.dcSF03.playerId = +this.sf03Id;
    } else {
      this.dcSF03.playerId = 0;
    }

    if (this.pf01Id) {
      this.dcPF01.playerId = +this.pf01Id;
    } else {
      this.dcPF01.playerId = 0;
    }

    if (this.pf02Id) {
      this.dcPF02.playerId = +this.pf02Id;
    } else {
      this.dcPF02.playerId = 0;
    }

    if (this.pf03Id) {
      this.dcPF03.playerId = +this.pf03Id;
    } else {
      this.dcPF03.playerId = 0;
    }

    if (this.c01Id) {
      this.dcC01.playerId = +this.c01Id;
    } else {
      this.dcC01.playerId = 0;
    }

    if (this.c02Id) {
      this.dcC02.playerId = +this.c02Id;
    } else {
      this.dcC02.playerId = 0;
    }

    if (this.c03Id) {
      this.dcC03.playerId = +this.c03Id;
    } else {
      this.dcC03.playerId = 0;
    }

    // Now need to write to the database
    // Now need to create the array to pass
    const arrDC: DepthChart[] = [];
    arrDC.push(this.dcPG01);
    arrDC.push(this.dcPG02);
    arrDC.push(this.dcPG03);
    arrDC.push(this.dcSG01);
    arrDC.push(this.dcSG02);
    arrDC.push(this.dcSG03);
    arrDC.push(this.dcSF01);
    arrDC.push(this.dcSF02);
    arrDC.push(this.dcSF03);
    arrDC.push(this.dcPF01);
    arrDC.push(this.dcPF02);
    arrDC.push(this.dcPF03);
    arrDC.push(this.dcC01);
    arrDC.push(this.dcC02);
    arrDC.push(this.dcC03);

    // Now call the service passing the array
    this.teamService.saveDepthCharts(arrDC).subscribe(result => {
      // console.log(result);
    }, error => {
      this.alertify.error('Error saving depth charts');
    });
  }

  setupDepthChartObjects() {
    console.log('test');
    console.log(this.depthCharts);
    this.depthCharts.forEach(element => {
      const pos = element.position;
      const rank = element.depth;

      if (pos === 1) {
        if (rank === 1) {
          this.dcPG01 = element;
        } else if (rank === 2) {
          this.dcPG02 = element;
        } else if (rank === 3) {
          this.dcPG03 = element;
        }
      } else if (pos === 2) {
        if (rank === 1) {
          this.dcSG01 = element;
        } else if (rank === 2) {
          this.dcSG02 = element;
        } else if (rank === 3) {
          this.dcSG03 = element;
        }
      } else if (pos === 3) {
        if (rank === 1) {
          this.dcSF01 = element;
        } else if (rank === 2) {
          this.dcSF02 = element;
        } else if (rank === 3) {
          this.dcSF03 = element;
        }
      } else if (pos === 4) {
        if (rank === 1) {
          this.dcPF01 = element;
        } else if (rank === 2) {
          this.dcPF02 = element;
        } else if (rank === 3) {
          this.dcPF03 = element;
        }
      } else if (pos === 5) {
        console.log('embiid');
        console.log(rank);
        console.log(element);
        if (rank === 1) {
          console.log('insdie rank 1');
          // console.log('embiid2');
          // console.log(element);
          this.dcC01 = element;
          console.log(this.dcC01);
        } else if (rank === 2) {
          this.dcC02 = element;
        } else if (rank === 3) {
          this.dcC03 = element;
        }
      }
    });
  }

  getPlayerName(player: Player) {
    const p = this.playingRoster.find(x => x.id === player.id);
    return p.firstName + ' ' + ' ' + p.surname;
  }
}
