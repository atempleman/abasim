import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ContactComponent } from './contact/contact.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { PlayersComponent } from './players/players.component';
import { DraftComponent } from './draft/draft.component';
import { AdminComponent } from './admin/admin.component';
import { StatsComponent } from './stats/stats.component';
import { StandingsComponent } from './standings/standings.component';
import { AuthGuard } from './_guards/auth.guard';
import { AdmindraftComponent } from './admindraft/admindraft.component';
import { AdminAuthGuard } from './_guards/adminauth.guard';
import { InitiallotteryComponent } from './initiallottery/initiallottery.component';
import { DraftPlayerPoolComponent } from './draft-player-pool/draft-player-pool.component';
import { DraftboardComponent } from './draftboard/draftboard.component';
import { InitialDraftComponent } from './initial-draft/initial-draft.component';
import { AdmintestengineComponent } from './admintestengine/admintestengine.component';
import { TeamComponent } from './team/team.component';
import { ViewPlayerComponent } from './view-player/view-player.component';
import { CoachingComponent } from './coaching/coaching.component';
import { DepthchartComponent } from './depthchart/depthchart.component';
import { FreeagentsComponent } from './freeagents/freeagents.component';
import { TradesComponent } from './trades/trades.component';
import { LeagueComponent } from './league/league.component';
import { ScheduleComponent } from './schedule/schedule.component';
import { TransactionsComponent } from './transactions/transactions.component';
import { WatchGameComponent } from './watch-game/watch-game.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'team', component: TeamComponent, canActivate: [AuthGuard] },
  { path: 'view-player', component: ViewPlayerComponent, canActivate: [AuthGuard] },
  { path: 'coaching', component: CoachingComponent, canActivate: [AuthGuard] },
  { path: 'depthchart', component: DepthchartComponent, canActivate: [AuthGuard] },
  { path: 'freeagents', component: FreeagentsComponent, canActivate: [AuthGuard] },
  { path: 'trades', component: TradesComponent, canActivate: [AuthGuard] },
  { path: 'players', component: PlayersComponent, canActivate: [AuthGuard] },
  { path: 'league', component: LeagueComponent, canActivate: [AuthGuard] },
  { path: 'standings', component: StandingsComponent, canActivate: [AuthGuard] },
  { path: 'stats', component: StatsComponent, canActivate: [AuthGuard] },
  { path: 'schedule', component: ScheduleComponent, canActivate: [AuthGuard] },
  { path: 'transactions', component: TransactionsComponent, canActivate: [AuthGuard] },
  { path: 'admin', component: AdminComponent, canActivate: [AdminAuthGuard] },
  { path: 'watch-game', component: WatchGameComponent, canActivate: [AdminAuthGuard] },

  { path: 'draft', component: DraftComponent, canActivate: [AuthGuard] },
  { path: 'initiallottery', component: InitiallotteryComponent, canActivate: [AuthGuard] },
  { path: 'draftplayerpool', component: DraftPlayerPoolComponent, canActivate: [AuthGuard] },
  { path: 'draftboard', component: DraftboardComponent, canActivate: [AuthGuard] },
  { path: 'admindraft', component: AdmindraftComponent, canActivate: [AdminAuthGuard] },
  { path: 'initial-draft', component: InitialDraftComponent, canActivate: [AuthGuard] },
  { path: 'admintestengine', component: AdmintestengineComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }


