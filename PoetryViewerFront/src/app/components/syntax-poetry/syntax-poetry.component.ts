import { Component, OnInit, Input } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Syntax } from '../../models/syntax';
import { KeyValue } from '@angular/common';

@Component({
  selector: 'app-syntax-poetry',
  templateUrl: './syntax-poetry.component.html',
  styleUrls: ['./syntax-poetry.component.css']
})
export class SyntaxPoetryComponent {
  baseUrl:string = environment.baseUrl;
  @Input() author!: string;

  minLen: number = 1;
  syntax!: Syntax;
  inclusions!: { [key: string]: number };

  
  constructor(private http: HttpClient){

  }
  ngOnInit() {
    this.onLoad();
  }
  onLoad(){
    this.loadSyntaxData();
  }
  setDefaultData(author: string){
    this.author = author;
    this.minLen = 1;
    this.syntax = new Syntax('', { data: 0 });
    this.inclusions = { data: 0 };

    this.onLoad();
  }


  loadSyntaxData() {
    this.http.get(`${this.baseUrl}/syntax/${this.author}/${this.minLen}`).subscribe(
      (response: any) => {
        this.syntax = response;
        this.sortInclusionsDescending();
        console.log(this.syntax);
      },
      (error) => {
        console.error('Error fetching data:', error);
      }
    );
  }

  sortInclusionsDescending() {
    if (this.syntax && this.syntax.Inclusions) {
      const sortedInclusions = this.sortObjectByValuesDescending(this.syntax.Inclusions);
      this.syntax.Inclusions = sortedInclusions;
    }
  }
  sortObjectByValuesDescending(obj: { [key: string]: number }) {
    return Object.fromEntries(
      Object.entries(obj).sort((a, b) => b[1] - a[1])
    );
  }

  // Preserve original property order
  originalOrder = (a: KeyValue<string, number>, b: KeyValue<string, number>): number => {
    return 0;
  }
  // // Order by ascending property value
  // valueAscOrder = (a: KeyValue<string, number>, b: KeyValue<string, number>): number => {
  //   return a.value.localeCompare(b.value);
  // }
  // Order by descending property key
  keyDescOrder = (a: KeyValue<string, number>, b: KeyValue<string, number>): number => {
    return a.key > b.key ? -1 : (b.key > a.key ? 1 : 0);
  }
  


}
