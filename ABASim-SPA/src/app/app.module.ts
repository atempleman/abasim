import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule, ModalModule, BsModalRef, BsModalService } from 'ngx-bootstrap';
import { NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerModule } from 'ngx-spinner';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { HomeComponent } from './home/home.component';
import { ContactComponent } from './contact/contact.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { DashboardComponent } from './dashboard/dashboard.component';
import { PlayersComponent } from './players/players.component';
import { DraftComponent } from './draft/draft.component';
import { AdminComponent } from './admin/admin.component';
import { StatsComponent } from './stats/stats.component';
import { StandingsComponent } from './standings/standings.component';
import { CoachingComponent } from './coaching/coaching.component';
import { FreeagentsComponent } from './freeagents/freeagents.component';
import { TradesComponent } from './trades/trades.component';
import { DepthchartComponent } from './depthchart/depthchart.component';
import { AdmindraftComponent } from './admindraft/admindraft.component';
import { InitiallotteryComponent } from './initiallottery/initiallottery.component';
import { DraftPlayerPoolComponent } from './draft-player-pool/draft-player-pool.component';
import { DraftboardComponent } from './draftboard/draftboard.component';
import { InitialDraftComponent } from './initial-draft/initial-draft.component';
import { JwtModule } from '@auth0/angular-jwt';
import { ViewPlayerComponent } from './view-player/view-player.component';
import { AdmintestengineComponent } from './admintestengine/admintestengine.component';
import { TeamComponent } from './team/team.component';
import { LeagueComponent } from './league/league.component';
import { ScheduleComponent } from './schedule/schedule.component';
import { TransactionsComponent } from './transactions/transactions.component';
import { WatchGameComponent } from './watch-game/watch-game.component';
import { BoxScoreComponent } from './box-score/box-score.component';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { PlayoffsComponent } from './playoffs/playoffs.component';
import { PlayoffStatsComponent } from './playoff-stats/playoff-stats.component';

export function tokenGetter() {
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      NavbarComponent,
      FooterComponent,
      HomeComponent,
      ContactComponent,
      DashboardComponent,
      PlayersComponent,
      DraftComponent,
      AdminComponent,
      StatsComponent,
      StandingsComponent,
      CoachingComponent,
      FreeagentsComponent,
      TradesComponent,
      DepthchartComponent,
      AdmindraftComponent,
      InitiallotteryComponent,
      DraftPlayerPoolComponent,
      DraftboardComponent,
      InitialDraftComponent,
      LeagueComponent,
      ViewPlayerComponent,
      AdmintestengineComponent,
      TeamComponent,
      ViewPlayerComponent,
      StandingsComponent,
      ScheduleComponent,
      TransactionsComponent,
      WatchGameComponent,
      PlayoffsComponent,
      PlayoffStatsComponent,
      BoxScoreComponent
   ],
   imports: [
      BrowserModule,
      AppRoutingModule,
      HttpClientModule,
      FormsModule,
      NgxSpinnerModule,
      NgbCollapseModule,
      CarouselModule,
      ReactiveFormsModule,
      ModalModule.forRoot(),
      JwtModule.forRoot({
         config: {
             // tslint:disable-next-line: object-literal-shorthand
             tokenGetter: tokenGetter,
             whitelistedDomains: ['localhost:5000'],
             blacklistedRoutes: ['localhost:5000/api/auth']
         }
       })
   ],
   providers: [
      ErrorInterceptorProvider,
      AlertifyService,
      BsModalService,
      BsModalRef
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
