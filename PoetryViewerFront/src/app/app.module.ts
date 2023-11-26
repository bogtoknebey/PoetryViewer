import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RandomPoetryComponent } from './components/random-poetry/random-poetry.component';
import { CreatePoetryComponent } from './components/create-poetry/create-poetry.component';
import { FormsModule } from '@angular/forms';
import { SyntaxPoetryComponent } from './components/syntax-poetry/syntax-poetry.component';
import { PlayerComponent } from './components/player/player.component';
import { RecorderComponent } from './components/recorder/recorder.component';

@NgModule({
  declarations: [
    AppComponent,
    RandomPoetryComponent,
    CreatePoetryComponent,
    SyntaxPoetryComponent,
    PlayerComponent,
    RecorderComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
