import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule, ModalModule, BsModalRef, BsModalService } from 'ngx-bootstrap';
import { NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';

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
import { ScheduleandresultsComponent } from './scheduleandresults/scheduleandresults.component';
import { FinancesComponent } from './finances/finances.component';
import { CoachingComponent } from './coaching/coaching.component';
import { FreeagentsComponent } from './freeagents/freeagents.component';
import { TradesComponent } from './trades/trades.component';
import { DepthchartComponent } from './depthchart/depthchart.component';
import { AdminleagueComponent } from './adminleague/adminleague.component';
import { AdminteamComponent } from './adminteam/adminteam.component';
import { AdmindraftComponent } from './admindraft/admindraft.component';
import { InitiallotteryComponent } from './initiallottery/initiallottery.component';
import { DraftPlayerPoolComponent } from './draft-player-pool/draft-player-pool.component';
import { DraftboardComponent } from './draftboard/draftboard.component';
import { InitialDraftComponent } from './initial-draft/initial-draft.component';
import { JwtModule } from '@auth0/angular-jwt';
import { ViewPlayerComponent } from './view-player/view-player.component';
import { AdminpreseasonComponent } from './adminpreseason/adminpreseason.component';
import { AdmintestengineComponent } from './admintestengine/admintestengine.component';
import { TeamComponent } from './team/team.component';
import { LeagueComponent } from './league/league.component';

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
      ScheduleandresultsComponent,
      FinancesComponent,
      CoachingComponent,
      FreeagentsComponent,
      TradesComponent,
      DepthchartComponent,
      AdminleagueComponent,
      AdminteamComponent,
      AdmindraftComponent,
      InitiallotteryComponent,
      DraftPlayerPoolComponent,
      DraftboardComponent,
      InitialDraftComponent,
      LeagueComponent,
      ViewPlayerComponent,
      AdminpreseasonComponent,
      AdmintestengineComponent,
      TeamComponent,
      ViewPlayerComponent
   ],
   imports: [
      BrowserModule,
      AppRoutingModule,
      HttpClientModule,
      FormsModule,
      NgbCollapseModule,
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
