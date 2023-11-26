import { Component, OnInit, Input, HostListener, EventEmitter, Output} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Poetry } from '../../models/poetry';
import { AudioRecord } from '../../models/audioRecord';

@Component({
  selector: 'app-random-poetry',
  templateUrl: './random-poetry.component.html',
  styleUrls: ['./random-poetry.component.css']
})

export class RandomPoetryComponent implements OnInit {
  baseUrl:string = environment.baseUrl;
  @Output() closeWithData: EventEmitter<Poetry> = new EventEmitter<Poetry>();

  @Input() author!: string;
  @Input() selectedNum: number = -1;
  selectedPoetry: Poetry = new Poetry("", "", "");
  
  playerSwitch: boolean = false; 
  isLoad: boolean = false;


  constructor(private http: HttpClient) {

  }
  ngOnInit() {
    this.onLoad();
  }
  onLoad(){
    if(this.selectedNum > 0){
      this.byNumberFetch();
    }else{
      this.randomFetch();
    }
  }
  setDefaultData(author: string){
    this.author = author;
    this.selectedNum = 1;
    this.selectedPoetry = new Poetry("","","");
    this.playerSwitch = false;
    this.isLoad = false;
    
    this.onLoad();
  }


  playerRecordSwitch(){
    this.playerSwitch = !this.playerSwitch;
  }

  byNumberFetch(){
    this.isLoad = true;
    this.reset();
    this.http.get(`${this.baseUrl}/poetry/${this.author}/${this.selectedNum}`)
    .subscribe(
      (response: any) => {
        this.selectedPoetry = response;
        this.isLoad = false;
      },
      error => {
        console.error('Error fetching data:', error);
        this.isLoad = false;
      }
    );
  }
  
  randomFetch(){
    this.isLoad = true;
    this.reset();
    this.http.get(`${this.baseUrl}/poetry/random/${this.author}`)
    .subscribe(
      (response: any) => {
        this.selectedPoetry = response;
        this.selectedNum = parseInt(this.selectedPoetry.Name);
        this.isLoad = false;
      },
      error => {
        console.error('Error fetching data:', error);
        this.isLoad = false;
      }
    );
  }
  
  getNext(){
    if(this.playerSwitch) return;
    if (this.selectedPoetry.Text != ""){
      console.log("getNext is working:");
      console.log(`Befour: author:${this.author}, selectedNum:${this.selectedNum}`);
      this.selectedNum++;
      this.byNumberFetch();
      console.log(`After: author:${this.author}, selectedNum:${this.selectedNum}`);
    }
  }

  getPrev(){
    if(this.playerSwitch) return;
    if (this.selectedNum > 1){
      console.log("getPrev is working:");
      console.log(`Befour: author:${this.author}, selectedNum:${this.selectedNum}`);
      this.selectedNum--;
      this.byNumberFetch();
      console.log(`After: author:${this.author}, selectedNum:${this.selectedNum}`);
    }
  }

  edit(){
    this.closeWithData.emit(this.selectedPoetry);
  }

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if(this.isLoad) return;
    if(event.ctrlKey || event.metaKey){
      return;
    }
    
    if(this.playerSwitch) {
      
    } else {
      console.log("key has been cotched");
      if (event.key === 'ArrowLeft')
        this.getPrev();
      if (event.key === 'ArrowRight')
        this.getNext();
      if (event.key === 'r')
        this.randomFetch();
    };

    if (event.key === 's')
      this.playerRecordSwitch();
      
  }

  reset(){
    //this.playerSwitch = false;
    this.selectedPoetry = new Poetry("", "No found", "");
  }
}
