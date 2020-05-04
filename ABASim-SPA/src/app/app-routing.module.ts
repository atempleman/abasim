import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ContactComponent } from './contact/contact.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { RosterComponent } from './roster/roster.component';
import { PlayersComponent } from './players/players.component';
import { DraftComponent } from './draft/draft.component';
import { AdminComponent } from './admin/admin.component';
import { StatsComponent } from './stats/stats.component';
import { StandingsComponent } from './standings/standings.component';
import { ScheduleandresultsComponent } from './scheduleandresults/scheduleandresults.component';
import { FinancesComponent } from './finances/finances.component';
import { AuthGuard } from './_guards/auth.guard';
import { AdminleagueComponent } from './adminleague/adminleague.component';
import { AdminteamComponent } from './adminteam/adminteam.component';
import { AdmindraftComponent } from './admindraft/admindraft.component';
import { AdminAuthGuard } from './_guards/adminauth.guard';
import { InitiallotteryComponent } from './initiallottery/initiallottery.component';
import { DraftPlayerPoolComponent } from './draft-player-pool/draft-player-pool.component';
import { DraftboardComponent } from './draftboard/draftboard.component';
import { InitialDraftComponent } from './initial-draft/initial-draft.component';
import { AdminpreseasonComponent } from './adminpreseason/adminpreseason.component';
import { AdmintestengineComponent } from './admintestengine/admintestengine.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'roster', component: RosterComponent, canActivate: [AuthGuard] },
  { path: 'players', component: PlayersComponent, canActivate: [AuthGuard] },
  { path: 'draft', component: DraftComponent, canActivate: [AuthGuard] },
  { path: 'initiallottery', component: InitiallotteryComponent, canActivate: [AuthGuard] },
  { path: 'draftplayerpool', component: DraftPlayerPoolComponent, canActivate: [AuthGuard] },
  { path: 'draftboard', component: DraftboardComponent, canActivate: [AuthGuard] },
  { path: 'admin', component: AdminComponent, canActivate: [AdminAuthGuard] },
  { path: 'adminleague', component: AdminleagueComponent, canActivate: [AdminAuthGuard] },
  { path: 'adminteam', component: AdminteamComponent, canActivate: [AdminAuthGuard] },
  { path: 'admindraft', component: AdmindraftComponent, canActivate: [AdminAuthGuard] },
  { path: 'stats', component: StatsComponent, canActivate: [AuthGuard] },
  { path: 'standings', component: StandingsComponent, canActivate: [AuthGuard] },
  { path: 'initial-draft', component: InitialDraftComponent, canActivate: [AuthGuard] },
  { path: 'scheduleandresults', component: ScheduleandresultsComponent, canActivate: [AuthGuard] },
  { path: 'finances', component: FinancesComponent, canActivate: [AuthGuard] },
  { path: 'adminpreseason', component: AdminpreseasonComponent, canActivate: [AuthGuard] },
  { path: 'admintestengine', component: AdmintestengineComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }


