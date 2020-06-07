import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';
import { Team } from '../_models/team';
import { CoachSetting } from '../_models/coachSetting';
import { ExtendedPlayer } from '../_models/extendedPlayer';
import { Player } from '../_models/player';

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
      this.getCoachSettings();
    });
  }

  getCoachSettings() {
    this.teamService.getRosterForTeam(this.team.id).subscribe(result => {
      this.extendedPlayers = result;
    }, error => {
      this.alertify.error('Error getting players');
    });

    this.teamService.getCoachingSettings(this.team.id).subscribe(result => {
      this.coachSetting = result;
      console.log(this.coachSetting);
    }, error => {
      this.alertify.error('Error getting Coach Settings');
    });
  }

  getPlayerName(playerId: number) {
    const player = this.extendedPlayers.find(x => x.id === playerId);
    return player.firstName + ' ' + player.surname;
  }

  editCoaching() {
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

  cancelCoaching() {
    this.isEdit = 0;
  }

  goToTeam() {
    this.router.navigate(['/team']);
  }

  goToDepthCharts() {
    this.router.navigate(['/depthchart']);
  }

}
