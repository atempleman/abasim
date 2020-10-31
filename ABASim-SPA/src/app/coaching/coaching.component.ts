import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { CoachSetting } from '../_models/coachSetting';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { Player } from '../_models/player';
import { PlayerInjury } from '../_models/playerInjury';
import { OffensiveStrategy } from '../_models/offensiveStrategy';
import { DefensiveStrategy } from '../_models/defensiveStrategyId';
import { Strategy } from '../_models/strategy';
import { SaveStrategy } from '../_models/saveStrategy';

@Component({
  selector: 'app-coaching',
  templateUrl: './coaching.component.html',
  styleUrls: ['./coaching.component.css']
})
export class CoachingComponent implements OnInit {
  isAdmin: number;
  team: Team;
  coachSetting: CoachSetting;
  extendedPlayers: Player[] = [];
  isEdit = 0;
  gotoOne: number;
  gotoTwo: number;
  gotoThree: number;
  teamsInjuries: PlayerInjury[] = [];

  injuredOne = 0;
  injuredTwo = 0;
  injuredThree = 0;

  goToTab = 1;
  offensiveStrategyTab = 0;
  defensiveStrategyTab = 0;

  defStrategySelection = 0;
  offStrategySelection = 0;

  offStrategies: OffensiveStrategy[] = [];
  defStrategies: DefensiveStrategy[] = [];
  teamStrategy: Strategy;

  constructor(private router: Router, private alertify: AlertifyService, private authService: AuthService,
              private teamService: TeamService) { }

  ngOnInit() {
    // Check to see if the user is an admin user
    this.isAdmin = this.authService.isAdmin();

    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.getPlayerInjuries();
      this.getCoachSettings();
      this.getStrategies();
    });
  }

  getPlayerInjuries() {
    this.teamService.getPlayerInjuriesForTeam(this.team.id).subscribe(result => {
      this.teamsInjuries = result;
    }, error => {
      this.alertify.error('Error getting teams injuries');
    });
  }

  getCoachSettings() {
    this.teamService.getRosterForTeam(this.team.id).subscribe(result => {
      this.extendedPlayers = result;
    }, error => {
      this.alertify.error('Error getting players');
    }, () => {
      this.extendedPlayers.forEach(element => {
        const injured = this.teamsInjuries.find(x => x.playerId === element.id);

        if (injured) {
          const index = this.extendedPlayers.indexOf(element, 0);
          this.extendedPlayers.splice(index, 1);
        }
      });
    });

    this.teamService.getCoachingSettings(this.team.id).subscribe(result => {
      this.coachSetting = result;
      console.log(this.coachSetting);
    }, error => {
      this.alertify.error('Error getting Coach Settings');
    });
  }

  getStrategies() {
    this.teamService.getStrategyForTeam(this.team.id).subscribe(result => {
      this.teamStrategy = result;
    }, error => {
      this.alertify.error('Error getting team strategy');
    }, () => {
      // console.log(this.teamStrategy);
    });

    this.teamService.getOffensiveStrategies().subscribe(result => {
      this.offStrategies = result;
    }, error => {
      this.alertify.error('Error getting offensive strategies');
    });

    this.teamService.getDefensiveStrategies().subscribe(result => {
      this.defStrategies = result;
    }, error => {
      this.alertify.error('Error getting defensive stratgies');
    });
  }

  getPlayerNameWithInjuredCheck(playerId: number, gtPlayerNumber: number) {
    // Check if the player is injured
    const injured = this.teamsInjuries.find(x => x.playerId === playerId);
    if (injured) {
      if (gtPlayerNumber === 1) {
        this.injuredOne = 1;
      } else if (gtPlayerNumber === 2) {
        this.injuredTwo = 1;
      } else if (gtPlayerNumber === 3) {
        this.injuredThree = 1;
      }
    } else {
      const player = this.extendedPlayers.find(x => x.id === playerId);
      return player.firstName + ' ' + player.surname;
    }
  }

  getPlayerName(playerId: number) {
    const player = this.extendedPlayers.find(x => x.id === playerId);
    return player.firstName + ' ' + player.surname;
  }

  editCoaching() {
    this.isEdit = 1;
  }

  editOffensiveStrategy() {
    this.isEdit = 1;
  }

  editDefensiveStrategy() {
    this.isEdit = 1;
  }

  saveCoaching() {
    // Need to get the values
    this.coachSetting.goToPlayerOne = +this.gotoOne;
    this.coachSetting.goToPlayerTwo = +this.gotoTwo;
    this.coachSetting.goToPlayerThree = +this.gotoThree;

    // Now pass this through to ther servie
    this.teamService.saveCoachingSettings(this.coachSetting).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving Coaching Settings');
    }, () => {
      this.alertify.success('Coach Settings saved successfully');
    });

    this.isEdit = 0;
  }

  saveStrategy() {
    if (this.offStrategySelection !== 0) {
      // tslint:disable-next-line: triple-equals
      const value = this.offStrategies.find(x => x.id == this.offStrategySelection);
      if (this.teamStrategy !== null) {
        this.teamStrategy.offensiveStrategyId = +this.offStrategySelection;
        this.teamStrategy.offensiveStrategyName = value.name;
        this.teamStrategy.offensiveStrategyDesc = value.description;
      } else {
        const ts: Strategy = {
          teamId: this.team.id,
          offensiveStrategyId: +this.offStrategySelection,
          defensiveStrategyId: 0,
          offensiveStrategyName: value.name,
          offensiveStrategyDesc: value.description,
          defensiveStrategyName: '',
          defensiveStrategyDesc: '',
        };
        this.teamStrategy = ts;
      }
    }

    if (this.defStrategySelection !== 0) {
      if (this.teamStrategy !== null) {
        // tslint:disable-next-line: triple-equals
        const value = this.defStrategies.find(x => x.id == this.defStrategySelection);

        if (this.teamStrategy !== null) {
          this.teamStrategy.defensiveStrategyId = +this.defStrategySelection;
          this.teamStrategy.defensiveStrategyName = value.name;
          this.teamStrategy.defensiveStrategyDesc = value.description;
        } else {
          const ts: Strategy = {
            teamId: this.team.id,
            offensiveStrategyId: 0,
            defensiveStrategyId: +this.defStrategySelection,
            offensiveStrategyName: '',
            offensiveStrategyDesc: '',
            defensiveStrategyName: value.name,
            defensiveStrategyDesc: value.description,
          };
          this.teamStrategy = ts;
        }
      }
    }

    // Now to save the update team strategy
    this.teamService.saveStrategy(this.teamStrategy).subscribe(result => {
    }, error => {
      this.alertify.error('Error saving strategy');
    }, () => {
      this.alertify.success('Team strategy saved successfully');
      this.isEdit = 0;
    });
  }

  cancelCoaching() {
    this.isEdit = 0;
  }

  cancelOffensiveStrategy() {
    this.isEdit = 0;
  }

  cancelDefensiveStrategy() {
    this.isEdit = 0;
  }

  goToTeam() {
    this.router.navigate(['/team']);
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

  goToTabClick() {
    this.offensiveStrategyTab = 0;
    this.defensiveStrategyTab = 0;
    this.goToTab = 1;
  }

  offensiveStrategyTabClick() {
    this.defensiveStrategyTab = 0;
    this.goToTab = 0;
    this.offensiveStrategyTab = 1;
  }

  defensiveStrategyTabClick() {
    this.goToTab = 0;
    this.offensiveStrategyTab = 0;
    this.defensiveStrategyTab = 1;
  }
}
